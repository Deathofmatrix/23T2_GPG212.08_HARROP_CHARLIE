using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static ChocolateFactory.PlacedObjectTypeSO;

namespace ChocolateFactory
{
    public class ItemCombiner : MonoBehaviour
    {
        [SerializeField] private ItemSO itemToProduce;
        [SerializeField] private BeltItem currentBeltItem1;
        [SerializeField] private BeltItem currentBeltItem2;

        private PlacedObject _placedObject;
        private GridBuildingSystem _gridBuildingSystem;
        [SerializeField] private List<Vector2Int> _gridPositionList;

        private PlacedObjectTypeSO.Dir _dir;
        private float cellSize;

        [SerializeField] private float processTime;
        public Belt inputBelt1;
        public Belt inputBelt2;
        public Belt outputBelt;
        [SerializeField] private Vector3 itemMovePosition;

        public bool isBuildingInputting;
        public bool isBuildingProcessing;
        public bool isBuildingRunning;

        private void Start()
        {
            SetGridInfo();
            cellSize = _gridBuildingSystem.GetCellSize();

            itemMovePosition = GetItemPositionInput();

            inputBelt1 = null;
            inputBelt2 = null;
            FindInputBelts(out inputBelt1, out inputBelt2);

            outputBelt = null;
            outputBelt = FindOutputBelt();
        }

        private void Update()
        {
            if (inputBelt1 == null || inputBelt2 == null)
            {
                FindInputBelts(out inputBelt1, out inputBelt2);
            }

            if (outputBelt == null)
            {
                outputBelt = FindOutputBelt();
            }

            if (inputBelt1 != null && inputBelt2 != null && !isBuildingRunning)
            {
                if (inputBelt1.beltItem != null && inputBelt1.beltItem.item != null && inputBelt2.beltItem != null && inputBelt2.beltItem.item != null)
                {
                    StartCoroutine(MoveItemsToMachine());
                }
            }

            if (currentBeltItem1 != null && currentBeltItem2 != null)
            {
                CompareItems();
            }

            if (outputBelt != null && !outputBelt.isSpaceTaken)
            {
                if (isBuildingRunning && !isBuildingInputting && !isBuildingProcessing)
                {
                    OutputItem();
                }
            }
        }

        private IEnumerator MoveItemsToMachine()
        {
            if (inputBelt1.beltItem == currentBeltItem1 || inputBelt2.beltItem == currentBeltItem2)
            {
                yield return null;
            }

            inputBelt1.isSpaceTaken = true; 
            inputBelt2.isSpaceTaken = true;

            isBuildingInputting = true;
            isBuildingRunning = true;

            currentBeltItem1 = inputBelt1.beltItem;
            currentBeltItem2 = inputBelt2.beltItem;

            currentBeltItem1.item.transform.parent = transform;
            currentBeltItem2.item.transform.parent = transform;

            var step = inputBelt1._beltManager.speed * Time.deltaTime;

            while (currentBeltItem1.item.transform.position != itemMovePosition && currentBeltItem2.item.transform.position != itemMovePosition)
            {
                //Debug.Log("moving " + inputBelt.beltItem.item.transform.position);
                currentBeltItem1.item.transform.position = Vector3.MoveTowards(currentBeltItem1.transform.position, itemMovePosition, step);
                currentBeltItem2.item.transform.position = Vector3.MoveTowards(currentBeltItem2.transform.position, itemMovePosition, step);

                yield return null;
            }

            inputBelt1.isSpaceTaken = false;
            inputBelt2.isSpaceTaken = false;
            currentBeltItem1.GetComponent<SpriteRenderer>().enabled = false;
            currentBeltItem2.GetComponent<SpriteRenderer>().enabled = false;
            currentBeltItem1.item.transform.parent = transform;
            inputBelt1.beltItem = null;
            inputBelt2.beltItem = null;

            //Check if belt is equal to inputbelt1 or inputbelt2 
            //if equal to inputbelt1 then check if currentbeltitem1 is null then do the same with inputbelt2
            //set the currentbeltitem 1 & 2 to the items on the belts

            //move the beltItems to the combiner

            //diable the spriterenderer of the beltitems
            //cahnge the parent to the combiner
            //set the .ispacetaken to false of both belts
            //set the belt item of both belts to null

            isBuildingInputting = false;
        }

        private void CompareItems()
        {
            if (isBuildingInputting) return;

            isBuildingProcessing = true;

            //compare currentbetitem1 and currentbeltitem2 with the Database
            //Change ItemToProduce to the result
            ItemSO compareResult =
                CraftingDatabase.CheckIfRecipieValid(
                    _placedObject.placedObjectTypeSO.buildingType,
                    currentBeltItem1.GetItemSO(),
                    currentBeltItem2.GetItemSO());

            itemToProduce = compareResult;

            ChangeItems(itemToProduce);
        }

        private void ChangeItems(ItemSO newItemSO)
        {
            if (currentBeltItem1 == null || currentBeltItem2 == null) return;

            currentBeltItem1.UpdateItemSO(newItemSO);
            Destroy(currentBeltItem2.item);

            isBuildingProcessing = false;
        }

        private void OutputItem()
        {
            if (currentBeltItem1.GetItemSO() != itemToProduce) return;

            currentBeltItem1.item.transform.position = outputBelt.GetItemPosition();
            currentBeltItem1.GetComponent<SpriteRenderer>().enabled = true;
            outputBelt.beltItem = currentBeltItem1;
            currentBeltItem1 = null;
            currentBeltItem2 = null;
            itemToProduce = null;

            isBuildingRunning = false;
        }

        private void FindInputBelts(out Belt belt1, out Belt belt2)
        {
            Vector2Int currentGridPosition0 = _gridPositionList[0];
            Vector2Int currentGridPosition1 = _gridPositionList[1];
            Vector2Int currentGridPosition2 = _gridPositionList[2];
            Vector2Int currentGridPosition3 = _gridPositionList[3];


            Vector2Int cellOfInputBelt1 = Vector2Int.zero;
            Vector2Int cellOfInputBelt2 = Vector2Int.zero;

            belt1 = null;
            belt2 = null;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfInputBelt1 = new Vector2Int(currentGridPosition0.x, currentGridPosition0.y - 1);
                    cellOfInputBelt2 = new Vector2Int(currentGridPosition2.x, currentGridPosition2.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfInputBelt1 = new Vector2Int(currentGridPosition1.x - 1, currentGridPosition1.y);
                    cellOfInputBelt2 = new Vector2Int(currentGridPosition0.x - 1, currentGridPosition0.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfInputBelt1 = new Vector2Int(currentGridPosition3.x, currentGridPosition3.y + 1);
                    cellOfInputBelt2 = new Vector2Int(currentGridPosition1.x, currentGridPosition1.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfInputBelt1 = new Vector2Int(currentGridPosition2.x + 1, currentGridPosition2.y);
                    cellOfInputBelt2 = new Vector2Int(currentGridPosition3.x + 1, currentGridPosition3.y);
                    break;
            }

            //Debug.Log("input" + cellOfInputBelt);
            PlacedObject placedObject1 = _gridBuildingSystem?.GetPlacedObjectXY(cellOfInputBelt1.x, cellOfInputBelt1.y);
            PlacedObject placedObject2 = _gridBuildingSystem?.GetPlacedObjectXY(cellOfInputBelt2.x, cellOfInputBelt2.y);
            Belt inputBelt1 = placedObject1?.gameObject.GetComponent<Belt>();
            Belt inputBelt2 = placedObject2?.gameObject.GetComponent<Belt>();

            if (inputBelt1 != null)
            {
                belt1 = inputBelt1;
            }

            if (inputBelt2 != null)
            {
                belt2 = inputBelt2;
            }
        }

        private Belt FindOutputBelt()
        {
            Vector2Int currentGridPosition0 = _gridPositionList[0];
            Vector2Int currentGridPosition1 = _gridPositionList[1];
            Vector2Int currentGridPosition2 = _gridPositionList[2];
            Vector2Int currentGridPosition3 = _gridPositionList[3];

            Vector2Int cellOfOutputBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition3.x, currentGridPosition3.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition2.x + 1, currentGridPosition2.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition0.x, currentGridPosition0.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition1.x - 1, currentGridPosition1.y);
                    break;
            }
            //Debug.Log("output " + cellOfOutputBelt);
            PlacedObject placedObject = _gridBuildingSystem?.GetPlacedObjectXY(cellOfOutputBelt.x, cellOfOutputBelt.y);
            Belt nextBelt = placedObject?.gameObject.GetComponent<Belt>();

            if (nextBelt != null)
            {
                return nextBelt;
            }
            return null;
        }

        public Vector3 GetItemPositionInput()
        {
            Vector3 position = _gridBuildingSystem.GetPlacedObjectXY(_gridPositionList[0].x, _gridPositionList[0].y).transform.position;

            Vector3 centerOfPlacedObject = Vector3.zero;

            switch (_dir)
            {
                case Dir.Up:
                    centerOfPlacedObject = new Vector3(position.x + (cellSize / 2), position.y + (cellSize / 2));
                    break;
                case Dir.Right:
                    centerOfPlacedObject = new Vector3(position.x + (cellSize / 2), position.y - (cellSize / 2));
                    break;
                case Dir.Down:
                    centerOfPlacedObject = new Vector3(position.x - (cellSize / 2), position.y - (cellSize / 2));
                    break;
                case Dir.Left:
                    centerOfPlacedObject = new Vector3(position.x - (cellSize / 2), position.y + (cellSize / 2));
                    break;
            }
            return centerOfPlacedObject;

        }

        public void SetGridInfo()
        {
            _placedObject = GetComponent<PlacedObject>();
            _gridBuildingSystem = _placedObject.gridBuildingSystem;
            _dir = _placedObject.dir;

            foreach (Vector2Int position in _placedObject.GetGridPositionList())
            {
                _gridPositionList.Add(position);
            }
        }
    }
}
