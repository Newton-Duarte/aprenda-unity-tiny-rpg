using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int itemId;
    public new string name = "New Item";
    public Sprite icon = null;
    public bool stackQuantity;
    public int quantity;
    public bool isConsumible;
    public bool isQuestItem;
    public bool isUniqueBuy;
    public int buyPrice;
    public GameObject itemPrefab;

    public void useItem()
    {
        switch(itemId)
        {
            case 1: // Potion
                Player _player = FindObjectOfType(typeof(Player)) as Player;
                _player.usePotion();
                break;
        }
    }
}
