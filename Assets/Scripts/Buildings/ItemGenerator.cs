using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static ChocolateFactory.PlacedObjectTypeSO;

namespace ChocolateFactory
{
    public class ItemGenerator : MonoBehaviour
    {
        [SerializeField] private ItemSO[] itemToProduceArray;
        [SerializeField] private ItemSO itemToProduce;
        [SerializeField] private BeltItem itemPrefab;

        private PlacedObject _placedObject;
        private PlacedObjectTypeSO _placedObjectTypeSO;
        private GridBuildingSystem _gridBuildingSystem;
        [SerializeField] private List<Vector2Int> _gridpositionList;

        private PlacedObjectTypeSO.Dir _dir;
        private float cellSize;

        [SerializeField] private float itemInterval;
        public Belt beltInSequence;

        private void Start()
        {
            SetGridInfo();
            cellSize = _gridBuildingSystem.GetCellSize();

            beltInSequence = null;
            beltInSequence = FindNextBelt();

            StartCoroutine(ProduceItem(itemInterval));
        }

        private void Update()
        {
            if (beltInSequence == null)
            {
                beltInSequence = FindNextBelt();
            }
        }

        public void SetGridInfo()
        {
            _placedObject = GetComponent<PlacedObject>();
            _placedObjectTypeSO = _placedObject.placedObjectTypeSO;
            _gridBuildingSystem = _placedObject.gridBuildingSystem;
            _dir = _placedObject.dir;

            foreach (Vector2Int position in _placedObject.GetGridPositionList())
            {
                _gridpositionList.Add(position);
            }

            switch (_placedObjectTypeSO.generatorType)
            {
                case GeneratorType.Undefined:
                    Debug.Log("No Item Defined");
                    break;
                case GeneratorType.Cocoa:
                    itemToProduce = itemToProduceArray[0];
                    break;
                case GeneratorType.Sugar:
                    itemToProduce = itemToProduceArray[1];
                    break;

            }
        }

        //public Vector3 GetItemPosition()
        //{
        //    Vector3 position = transform.position;

        //    Vector3 centerOfPlacedObject = position;

        //    switch (_dir)
        //    { 
        //        case Dir.Up:
        //            centerOfPlacedObject = new Vector3(position.x + (cellSize / 2), position.y + (cellSize / 2));
        //            break;
        //        case Dir.Right:
        //            centerOfPlacedObject = new Vector3(position.x + (cellSize / 2), position.y - (cellSize / 2));
        //            break;
        //        case Dir.Down:
        //            centerOfPlacedObject = new Vector3(position.x - (cellSize / 2), position.y - (cellSize / 2));
        //            break;
        //        case Dir.Left:
        //            centerOfPlacedObject = new Vector3(position.x - (cellSize / 2), position.y + (cellSize / 2));
        //            break;
        //    }

        //    return centerOfPlacedObject;

        //}

        private Belt FindNextBelt()
        {
            Vector2Int currentGridPosition = _gridpositionList[0];

            Vector2Int cellOfNextBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfNextBelt = new Vector2Int(currentGridPosition.x, currentGridPosition.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfNextBelt = new Vector2Int(currentGridPosition.x + 1, currentGridPosition.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfNextBelt = new Vector2Int(currentGridPosition.x, currentGridPosition.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfNextBelt = new Vector2Int(currentGridPosition.x - 1, currentGridPosition.y);
                    break;
            }
            //Debug.Log(cellOfNextBelt);
            PlacedObject placedObject = _gridBuildingSystem?.GetPlacedObjectXY(cellOfNextBelt.x, cellOfNextBelt.y);
            Belt nextBelt = placedObject?.gameObject.GetComponent<Belt>();

            if (nextBelt != null)
            {
                return nextBelt;
            }

            return null;
        }

        public void SpawnItem()
        {
            //Debug.Log("spawn item");
            BeltItem newitem = Instantiate(itemPrefab, beltInSequence.GetItemPosition(), Quaternion.identity, transform);
            newitem.UpdateItemSO(itemToProduce);
            beltInSequence.beltItem = newitem;
        }

        public IEnumerator ProduceItem(float interval)
        {
            //Debug.Log("CoroutineStarted");
            while (true)
            {
                while (beltInSequence == null || beltInSequence.isSpaceTaken)
                {
                    //Debug.Log("Waiting To Produce");
                    yield return null;
                }

                //Debug.Log("producing Item");
                yield return new WaitForSeconds(interval);
                SpawnItem();
            }
        }
    }
}
