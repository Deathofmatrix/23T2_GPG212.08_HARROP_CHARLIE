using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using static ChocolateFactory.PlacedObjectTypeSO;

namespace ChocolateFactory
{
    public class WorldOutput : MonoBehaviour
    {
        public delegate void ScoreIncrease();
        public static event ScoreIncrease OnScoreIncreased;

        [SerializeField] private Belt _inputBelt;

        [SerializeField] private Vector3Int _gridPosition;

        private GridBuildingSystem _gridBuildingSystem;
        private PlacedObject _placedObject;
        [SerializeField] private PlacedObjectTypeSO.Dir _dir;
        private float cellSize;

        [SerializeField] private ItemSO _goalItem;
        [SerializeField] private float _interval;
        [SerializeField] private Transform itemLocation;
        [SerializeField] private bool isDestroying;

        [SerializeField] private bool isCoroutineRunning = false;

        private void IncreaseScore()
        {
            if (OnScoreIncreased != null) OnScoreIncreased();
        }

        private void Start()
        {
            SetGridInfo();

            _inputBelt = null;
            _inputBelt = FindInputBelt();
        }

        private void Update()
        {
            if (_inputBelt == null)
            {
                _inputBelt = FindInputBelt();
            }

            if (AreAllItemsAccountedFor())
            {
                StartCoroutine(DestroyItemDelay());
            }
        }

        private bool AreAllItemsAccountedFor()
        {
            return _inputBelt && _inputBelt.beltItem && _inputBelt.beltItem.item;
        }

        private IEnumerator DestroyItemDelay()
        {
            if (isCoroutineRunning)
            {
                yield return null;
            }

            isCoroutineRunning = true;

            if (!isDestroying)
            {
                float step = _inputBelt._beltManager.speed * Time.deltaTime;

                //TODO Check if the Position to position move correctly(has to be exactly equal)

                Debug.Log("_inputBelt is [" + _inputBelt + "].");
                Debug.Log("_inputBelt.beltItem is [" + _inputBelt.beltItem + "].");
                Debug.Log("_inputBelt.beltItem.item is [" + _inputBelt.beltItem.item + "].");
                Debug.Log("itemLocation is [" + itemLocation + "].");

                while (_inputBelt.beltItem.item.transform.position != itemLocation.position)
                {
                    _inputBelt.beltItem.item.transform.position = Vector3.MoveTowards(_inputBelt.beltItem.item.transform.position, itemLocation.position, step);

                    yield return null;
                }

                isDestroying = true;
            }
            else
            {
                DestroyItem();
            }

            isCoroutineRunning = false;
            yield return null;
        }

        //private IEnumerator DestroyItemDelayTest()
        //{
        //    if (!isDestroying)
        //    {
        //        isDestroying = true;

        //        yield return new WaitForSeconds(_interval);

        //        DestroyItem();
        //    }
        //}

        private void DestroyItem()
        {
            if (_inputBelt.beltItem.GetItemSO() == _goalItem)
            {
                IncreaseScore();
                //play good sound
            }

            Destroy(_inputBelt.beltItem.item);
            _inputBelt.beltItem = null;
            _inputBelt.isSpaceTaken = false;
            isDestroying = false;
        }


        private Belt FindInputBelt()
        {
            Vector2Int cellOfNextBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x, _gridPosition.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x - 1, _gridPosition.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x, _gridPosition.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x + 1, _gridPosition.y);
                    break;
            }
            //Debug.Log(cellOfNextBelt);
            PlacedObject placedObject = _gridBuildingSystem?.GetPlacedObjectXY(cellOfNextBelt.x, cellOfNextBelt.y);
            Belt inputBelt = placedObject?.gameObject.GetComponent<Belt>();

            if (inputBelt != null)
            {
                return inputBelt;
            }

            return null;
        }
        public void SetGridInfo()
        {
            _placedObject = GetComponent<PlacedObject>();
            _dir = _placedObject.dir;
            _gridBuildingSystem = _placedObject.gridBuildingSystem;

            _goalItem = _gridBuildingSystem.goalItem;

            Vector2Int vector2IntLocation = _placedObject.GetGridPositionList()[0];
            Vector3Int location = new Vector3Int(vector2IntLocation.x, vector2IntLocation.y, 0);
            _gridPosition = location;
        }

    }
}
