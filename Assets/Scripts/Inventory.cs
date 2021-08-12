using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    GameController _gameController;
    Player _player;

    [Header("Stash")]
    public GameObject stashPanel;
    public Transform stashSlotGroup;
    InventorySlot[] stashSlots;
    public List<Item> stashItems;
    public int[] stashItemsQuantity;

    [Header("Inventory")]
    public GameObject inventoryPanel;
    public Transform slotGroup;
    InventorySlot[] slots;
    public List<Item> items;
    public int[] itemsQuantity;
    public int coins;
    public Text coinsText;

    [Header("Shop")]
    public GameObject shopPanel;

    [Header("Confirmation")]
    public GameObject panelConfirmation;
    public Image imageConfirmation;
    public Text titleConfirmation;
    public Text descriptionConfirmation;
    Item panelConfirmationItem;
    int confirmationId; // 0 = delete item, 1 = use item

    //public string[] inputLabel;
    //public Item[] usableItems;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        _player = FindObjectOfType(typeof(Player)) as Player;
        slots = slotGroup.GetComponentsInChildren<InventorySlot>();
        stashSlots = stashSlotGroup.GetComponentsInChildren<InventorySlot>();
        updateUI();
        inventoryPanel.SetActive(false);
        stashPanel.SetActive(false);
        panelConfirmation.SetActive(false);
        shopPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (
            Input.GetButtonDown("Cancel") &&
            !panelConfirmation.activeSelf &&
            !shopPanel.activeSelf && 
            !stashPanel.activeSelf
           )
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf)
            {
                updateUI();
            }
        }

        /*
        if (Input.GetButtonDown(inputLabel[0]))
        {
            if (isItemInInventory(usableItems[0]))
            {
                usableItems[0].useItem();
                removeItem(usableItems[0], 1);
            }
        }
        */
    }

    void updateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].addItem(items[i]);
            }
            else
            {
                slots[i].clearSlot();
            }
        }

        if (_gameController.currentGameState == GameState.stash)
        {
            for (int i = 0; i < stashSlots.Length; i++)
            {
                if (i < stashItems.Count)
                {
                    stashSlots[i].addItem(stashItems[i]);
                }
                else
                {
                    stashSlots[i].clearSlot();
                }
            }
        }

        coinsText.text = coins.ToString();
    }

    public void getItem(Item item)
    {
        if (!item.stackQuantity)
        {
            items.Add(item);
        }
        else
        {
            if (isItemInInventory(item))
            {
                itemsQuantity[item.itemId] += item.quantity;
            }
            else
            {
                items.Add(item);
                itemsQuantity[item.itemId] += item.quantity;
            }
        }

        updateUI();
    }

    public void removeItem(Item item, int quantity)
    {
        if (!item.stackQuantity)
        {
            items.Remove(item);
        }
        else
        {
            itemsQuantity[item.itemId] -= quantity;
            if (itemsQuantity[item.itemId] <= 0)
            {
                items.Remove(item);
            }
        }

        updateUI();
    }

    public bool isItemInInventory(Item item)
    {
        bool value = false;

        foreach(Item i in items)
        {
            if (i == item) { value = true; }
        }

        return value;
    }

    public bool isInventoryFull()
    {
        return items.Count == slots.Length;
    }

    void openPanelConfirmation()
    {
        imageConfirmation.sprite = panelConfirmationItem.icon;
        titleConfirmation.text = panelConfirmationItem.name;
        descriptionConfirmation.text = confirmationId == 0 ? "Deseja excluir este item?" : "Deseja usar este item?";
        panelConfirmation.SetActive(true);
        inventoryPanel.SetActive(false);
    }

    void closePanelConfirmation()
    {
        panelConfirmationItem = null;
        panelConfirmation.SetActive(false);
        inventoryPanel.SetActive(true);
    }

    public void useItemConfirmation(Item item)
    {
        panelConfirmationItem = item;
        confirmationId = 1;
        openPanelConfirmation();
    }

    public void deleteItemConfirmation(Item item)
    {
        panelConfirmationItem = item;
        confirmationId = 0;
        openPanelConfirmation();
    }

    public void confirmChoice()
    {
        switch(confirmationId)
        {
            case 0:
                removeItem(panelConfirmationItem, itemsQuantity[panelConfirmationItem.itemId]);
                closePanelConfirmation();
                break;
            case 1:
                panelConfirmationItem.useItem();
                removeItem(panelConfirmationItem, 1);
                closePanelConfirmation();
                break;
        }
    }

    public void dropItem()
    {
        Vector3 dropPosition;
        float x = 0, y = 0;
        float raySize = 0.27f;

        switch(_player.directionId)
        {
            case 0:
                x = 0;
                y = -0.25f;
                raySize = 0.27f;
                break;
            case 1:
                x = 0;
                y = 0.1f;
                raySize = 0.15f;
                break;
            case 2:
                if (_player.isLookLeft)
                {
                    x = -0.25f;
                }
                else
                {
                    x = 0.15f;
                }
                y = -0.05f;
                raySize = 0.27f;
                break;
        }

        if (!_player.canDropItem(raySize))
        {

            dropPosition = new Vector3(x, y, 0);

            Instantiate(panelConfirmationItem.itemPrefab, _player.transform.position + dropPosition, transform.localRotation);
            removeItem(panelConfirmationItem, 1);
            closePanelConfirmation();
        }

    }

    public void openShopPanel()
    {
        shopPanel.SetActive(true);
        inventoryPanel.SetActive(true);

        updateInventorySlots();

        coinsText.text = coins.ToString();
        _gameController.setGameState(GameState.shop);
    }

    public void closeShopPanel()
    {
        shopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        _gameController.setGameState(GameState.gameplay);
    }

    public void buyItem(Item item)
    {
        if (!item.stackQuantity)
        {
            items.Add(item);
        }
        else
        {
            if (isItemInInventory(item))
            {
                itemsQuantity[item.itemId] += item.quantity;
            }
            else
            {
                items.Add(item);
                itemsQuantity[item.itemId] += item.quantity;
            }
        }

        updateInventorySlots();
    }

    void updateInventorySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].addItem(items[i]);
                slots[i].disableSlotInteractivity();
            }
            else
            {
                slots[i].clearSlot();
            }
        }
    }

    public void setCoins(int value)
    {
        coins += value;
        coinsText.text = coins.ToString();
    }

    public void openStash()
    {
        _gameController.setGameState(GameState.stash);

        foreach(InventorySlot slot in slots)
        {
            slot.disableRemoveItemIconFromSlot();
            slot.btnItem.interactable = true;
        }

        stashPanel.SetActive(true);
        inventoryPanel.SetActive(true);
    }

    public void closeStash()
    {
        _gameController.setGameState(GameState.gameplay);
        stashPanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    public bool isStashFull()
    {
        return stashItems.Count == stashSlots.Length;
    }

    public void stashItem(Item item)
    {
        if (!item.stackQuantity)
        {
            stashItems.Add(item);
        }
        else
        {
            if (isItemInStash(item))
            {
                stashItemsQuantity[item.itemId] += item.quantity;
            }
            else
            {
                stashItems.Add(item);
                stashItemsQuantity[item.itemId] += item.quantity;
            }
        }

        updateUI();
    }

    public bool isItemInStash(Item item)
    {
        bool value = false;

        foreach (Item i in stashItems)
        {
            if (i == item)
            {
                value = true;
                break;
            }
        }

        print($"Item is {(value ? "in stash" : "not in stash")}");

        return value;
    }

    public void getStashItem(Item item)
    {
        getItem(item);
        removeStashItem(item, 1);
    }

    public void removeStashItem(Item item, int quantity)
    {
        if (!item.stackQuantity)
        {
            stashItems.Remove(item);
        }
        else
        {
            stashItemsQuantity[item.itemId] -= quantity;
            if (stashItemsQuantity[item.itemId] <= 0)
            {
                stashItems.Remove(item);
            }
        }

        updateUI();
    }
}
