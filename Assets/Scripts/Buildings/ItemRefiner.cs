using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using static ChocolateFactory.PlacedObjectTypeSO;

namespace ChocolateFactory
{
    public class ItemRefiner : MonoBehaviour
    {
        [SerializeField] private ItemSO itemToProduce;
        [SerializeField] private BeltItem currentBeltItem;

        private PlacedObject _placedObject;
        private GridBuildingSystem _gridBuildingSystem;
        [SerializeField] private List<Vector2Int> _gridPositionList;

        private PlacedObjectTypeSO.Dir _dir;
        private float cellSize;

        [SerializeField] private float processTime;
        public Belt inputBelt;
        public Belt outputBelt;
        [SerializeField] private Vector3 itemMovePosition;

        public bool isBuildingRunning;
        public bool isBuildingProcessing;
        public bool isSpaceTaken;

        private void Start()
        {
            SetGridInfo();
            cellSize = _gridBuildingSystem.GetCellSize();

            itemMovePosition = GetItemPositionInput();

            inputBelt = null;
            inputBelt = FindInputBelt();

            outputBelt = null;
            outputBelt = FindOutputBelt();
        }

        private void Update()
        {
            if (inputBelt == null)
            {
                inputBelt = FindInputBelt();
            }

            if (outputBelt == null)
            {
                outputBelt = FindOutputBelt();
            }

            if (!isBuildingRunning && inputBelt != null && inputBelt.beltItem != null && inputBelt.beltItem.item != null)
            {
                MoveItemToMachine();
            }

            if (isBuildingRunning && currentBeltItem != null && isSpaceTaken/*currentBeltItem.GetItemSO() != itemToProduce*/)
            {
                OutputItem();
            }

            //Debug.Log(GetItemPositionInput());
        }

        private void MoveItemToMachine()
        {
            //StartCoroutine(inputBelt.StartBeltMoveToBuilding(itemMovePosition));
            StartCoroutine(StartBeltMoveToBuilding());
            currentBeltItem = inputBelt.beltItem;
            
        }

        public IEnumerator StartBeltMoveToBuilding()
        {
            //Debug.Log("BeltMoveStarted");
            inputBelt.isSpaceTaken = true;
            inputBelt.beltItem.item.transform.parent = transform;

            if (inputBelt.beltItem.item != null && !isBuildingRunning)
            {
                isBuildingRunning = true;

                var step = inputBelt._beltManager.speed * Time.deltaTime;

                while (inputBelt.beltItem.item.transform.position != itemMovePosition)
                {
                    //Debug.Log("moving " + inputBelt.beltItem.item.transform.position);
                    inputBelt.beltItem.item.transform.position = Vector3.MoveTowards(inputBelt.beltItem.transform.position, itemMovePosition, step);

                    yield return null;
                }

                inputBelt.beltItem.GetComponent<SpriteRenderer>().enabled = false;
                inputBelt.beltItem.item.transform.parent = gameObject.transform;
                inputBelt.isSpaceTaken = false;
                inputBelt.beltItem = null;
                isSpaceTaken = true;
            }
        }

        public void OutputItem()
        {
            StartCoroutine(ProcessItem(processTime));
        }


        public void ChangeItem()
        {
            if (currentBeltItem != null)
            {
                itemToProduce = CraftingDatabase.CheckIfRecipieValid(_placedObject.placedObjectTypeSO.buildingType, currentBeltItem.GetItemSO(), null);
                Debug.Log(_placedObject.placedObjectTypeSO.buildingType);
                currentBeltItem.UpdateItemSO(itemToProduce);
            }
        }

        public IEnumerator ProcessItem(float interval)
        {
            if (!isBuildingProcessing)
            {
                isBuildingProcessing = true;
                ChangeItem();
                while (outputBelt == null || outputBelt.isSpaceTaken || currentBeltItem == null)
                {

                    //Debug.Log("Waiting To Produce");

                    yield return null;
                }

                yield return new WaitForSeconds(interval);
                MoveItemToBelt();

                isBuildingProcessing = false;
                isSpaceTaken = false;
                //Debug.Log("producing Item"); 

            }
        }

        public void MoveItemToBelt()
        {
            currentBeltItem.item.transform.position = outputBelt.GetItemPosition();
            currentBeltItem.GetComponent<SpriteRenderer>().enabled = true;
            outputBelt.beltItem = currentBeltItem;
            currentBeltItem = null;
            itemToProduce = null;
            isBuildingRunning = false;

            
        }

        private Belt FindInputBelt()
        {
            Vector2Int currentGridPosition0 = _gridPositionList[0];
            Vector2Int currentGridPosition1 = _gridPositionList[1];

            Vector2Int cellOfInputBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfInputBelt = new Vector2Int(currentGridPosition0.x, currentGridPosition0.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfInputBelt = new Vector2Int(currentGridPosition0.x - 1, currentGridPosition0.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfInputBelt = new Vector2Int(currentGridPosition1.x, currentGridPosition1.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfInputBelt = new Vector2Int(currentGridPosition1.x + 1, currentGridPosition1.y);
                    break;
            }

            //Debug.Log("input" + cellOfInputBelt);
            PlacedObject placedObject = _gridBuildingSystem?.GetPlacedObjectXY(cellOfInputBelt.x, cellOfInputBelt.y);
            Belt previousBelt = placedObject?.gameObject.GetComponent<Belt>();

            if (previousBelt != null)
            {
                return previousBelt;
            }

            return null;
        }

        private Belt FindOutputBelt()
        {
            Vector2Int currentGridPosition0 = _gridPositionList[0];
            Vector2Int currentGridPosition1 = _gridPositionList[1];

            Vector2Int cellOfOutputBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition1.x, currentGridPosition1.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition1.x + 1, currentGridPosition1.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition0.x, currentGridPosition0.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfOutputBelt = new Vector2Int(currentGridPosition0.x - 1, currentGridPosition0.y);
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
            //return new Vector3(position.x, position.y);

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
