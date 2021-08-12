using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    Inventory _inventory;

    public Item shopItem;
    public Image itemImage;
    public Text itemTitleText;
    public Text itemPriceText;
    public Button btnBuy;

    // Start is called before the first frame update
    void Start()
    {
        _inventory = FindObjectOfType(typeof(Inventory)) as Inventory;

        itemImage.sprite = shopItem.icon;
        itemTitleText.text = shopItem.name;
        itemPriceText.text = shopItem.buyPrice.ToString();

        updateBuyButton();
    }

    public void buyItem()
    {
        if (!_inventory.isInventoryFull() && _inventory.coins >= shopItem.buyPrice)
        {
            _inventory.setCoins(shopItem.buyPrice * -1);
            _inventory.buyItem(shopItem);
            confirmBuy();
        }
    }

    void updateBuyButton()
    {
        if (_inventory.coins >= shopItem.buyPrice) { btnBuy.interactable = true; }
        else { btnBuy.interactable = false; }
    }

    void confirmBuy()
    {
        ShopSlot[] shopSlots = FindObjectsOfType(typeof(ShopSlot)) as ShopSlot[];

        foreach(ShopSlot shopSlot in shopSlots)
        {
            shopSlot.updateBuyButton();
        }
    }
}
