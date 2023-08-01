using Charlie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    //Code Sourced Mostly from UnityCodeMonkey.com (Awesome Grid building System)
    public class PlacedObject : MonoBehaviour
    {
        public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO, GridBuildingSystem gridBuildingSystem)
        {
            Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, 0, placedObjectTypeSO.GetRotationAngle(dir)));

            PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

            placedObject.placedObjectTypeSO = placedObjectTypeSO;
            placedObject.origin = origin;
            placedObject.dir = dir;
            placedObject.gridBuildingSystem = gridBuildingSystem;

            return placedObject;
        }

        public PlacedObjectTypeSO placedObjectTypeSO { get; private set; }
        private Vector2Int origin;
        public PlacedObjectTypeSO.Dir dir { get; private set; }
        public GridBuildingSystem gridBuildingSystem { get; private set; }

        public List<Vector2Int> GetGridPositionList()
        {
            return placedObjectTypeSO.GetGridPositionList(origin, dir);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
