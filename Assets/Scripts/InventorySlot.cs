using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Inventory _inventory;
    GameController _gameController;

    Item item;
    public Image icon;
    public Text quantityText;
    public Button btnItem;
    public Button btnRemove;

    public bool isStash;

    void Start()
    {
        _inventory = FindObjectOfType(typeof(Inventory)) as Inventory;
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        btnItem.interactable = false;
        btnRemove.gameObject.SetActive(false);
    }

    public void addItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.icon;
        icon.enabled = true;

        if (!isStash)
        {
            if (_gameController.currentGameState != GameState.stash)
            {
                if (newItem.isConsumible)
                {
                    btnItem.interactable = true;
                }
                else
                {
                    btnItem.interactable = false;
                }

                if (!newItem.isQuestItem)
                {
                    btnRemove.gameObject.SetActive(true);
                }
                else
                {
                    btnRemove.gameObject.SetActive(false);
                }

                if (newItem.stackQuantity)
                {
                    quantityText.gameObject.SetActive(true);
                    quantityText.text = _inventory.itemsQuantity[newItem.itemId].ToString();
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }
            else
            {
                btnItem.interactable = true;
                btnRemove.gameObject.SetActive(false);

                if (newItem.stackQuantity)
                {
                    quantityText.gameObject.SetActive(true);
                    quantityText.text = _inventory.itemsQuantity[newItem.itemId].ToString();
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }
        }
        else if (isStash)
        {
            btnItem.interactable = true;
            btnRemove.gameObject.SetActive(false);

            if (newItem.stackQuantity)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = _inventory.stashItemsQuantity[newItem.itemId].ToString();
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void clearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        quantityText.gameObject.SetActive(false);
        btnItem.interactable = false;
        btnRemove.gameObject.SetActive(false);
    }

    public void useItem()
    {
        if (_gameController.currentGameState != GameState.stash)
        {
            if (item != null && item.isConsumible)
            {
                //item.useItem();
                //_inventory.removeItem(item, 1);
                _inventory.useItemConfirmation(item);
            }
        }
        else if (_gameController.currentGameState == GameState.stash)
        {
            if (!_inventory.isStashFull() && !isStash)
            {
                print("Add item to stash");
                _inventory.stashItem(item);
                _inventory.removeItem(item, 1);
            }
            else if (!_inventory.isInventoryFull() && isStash)
            {
                print("Get item from stash");
                _inventory.getStashItem(item);
            }
        }
    }

    public void removeItem()
    {
        //_inventory.removeItem(item, _inventory.itemsQuantity[item.itemId]);
        _inventory.deleteItemConfirmation(item);
    }

    public void disableSlotInteractivity()
    {
        btnItem.interactable = false;
        btnRemove.gameObject.SetActive(false);
    }

    public void disableRemoveItemIconFromSlot()
    {
        btnRemove.gameObject.SetActive(false);
    }
}
