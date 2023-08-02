using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            if (_inputBelt != null && _inputBelt.beltItem != null && _inputBelt.beltItem.item != null)
            {
                DestroyItem();
            }
        }

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
