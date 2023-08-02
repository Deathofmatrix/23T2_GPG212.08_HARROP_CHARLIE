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
                if (inputBelt1.beltItem.item != null && inputBelt2.beltItem.item != null)
                {
                    MoveItemsToMachine();
                }
            }

            if (currentBeltItem1 != null && currentBeltItem2 != null)
            {
                CompareItems();
            }
        }

        private void MoveItemsToMachine()
        {
            isBuildingInputting = true;
            isBuildingRunning = true;

            //Check if belt is equal to inputbelt1 or inputbelt2 
            //if equal to inputbelt1 then check if currentbeltitem1 is null then do the same with inputbelt2
            //set the currentbeltitem 1 & 2 to the items on the belts

            //move the beltItems to the combiner

            //diable the spriterenderer of the beltitems
            //cahnge the parent to the combiner
            //set the .ispacetaken to false of both belts
            //set the belt item of both belts to null
            //set isspacetaken of combiner to true

            isBuildingInputting = false;
        }

        private void CompareItems()
        {
            isBuildingProcessing = true;
            if (isBuildingInputting) return;

            //compare currentbetitem1 and currentbeltitem2 with the Database
            //Change ItemToProduce to the result

            ItemSO compareResult =
                CraftingDatabase.CheckIfRecipieValid(
                    _placedObject.placedObjectTypeSO.buildingType,
                    currentBeltItem1.GetItemSO(),
                    currentBeltItem2.GetItemSO());

            itemToProduce = compareResult;
            isBuildingProcessing = false;
        }

        private void ChangeItems()
        {

        }

        private void OutputItems()
        {
            if (!isBuildingRunning && isBuildingProcessing) return;
            //things
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
