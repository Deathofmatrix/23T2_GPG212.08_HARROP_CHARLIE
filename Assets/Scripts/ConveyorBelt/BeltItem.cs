using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    public class BeltItem : MonoBehaviour
    {
        private static int itemID;
        private int currentItemID;

        public GameObject item;

        [SerializeField] private ItemSO _itemSO;
        [SerializeField] private ItemSO.ItemType _itemType = ItemSO.ItemType.Undefined;
        [SerializeField] private ItemSO.Tier _tier = ItemSO.Tier.Undefined;

        private void Awake()
        {
            item = gameObject;
        }

        private void Start()
        {
            currentItemID = itemID++;

            UpdateItemSO(_itemSO);
        }

        public ItemSO GetItemSO() => _itemSO;

        public void UpdateItemSO(ItemSO newItemSO)
        {
            if (newItemSO != null)
            {
                _itemSO = newItemSO;
                _itemType = newItemSO.GetItemType();
                _tier = newItemSO.GetItemTier();
                name = $"{newItemSO.name} {currentItemID}";
            }
        }
    }
}
