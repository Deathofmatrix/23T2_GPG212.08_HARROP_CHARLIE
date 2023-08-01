using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ChocolateFactory
{
    [CreateAssetMenu()]
    public class ItemSO : ScriptableObject
    {
        public enum ItemType { Undefined, CocoaPowder, Sugar, Chocolate, Lollies }
        public enum Tier { Undefined, Raw, Refined, Combined }

        [SerializeField] private Sprite sprite;
        [SerializeField] private string nameString;
        [SerializeField] private ItemType itemType;
        [SerializeField] private Tier itemTier;

        public ItemType GetItemType()
        {
            return itemType;
        }

        public Tier GetItemTier()
        {
            return itemTier;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
