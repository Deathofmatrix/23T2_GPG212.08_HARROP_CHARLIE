using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    public class Belt : MonoBehaviour
    {
        private static int _beltID = 0;

        public Belt beltInSequence;
        public BeltItem beltItem;
        public bool isSpaceTaken;

        private BeltManager _beltManager;

        private LineRenderer _lineRenderer;


        [SerializeField] private Vector3Int _gridPosition;

        private GridBuildingSystem _gridBuildingSystem;
        private PlacedObject _placedObject;
        private PlacedObjectTypeSO _placedObjectTypeSO;
        [SerializeField] private PlacedObjectTypeSO.Dir _dir;
        private float cellSize;

        private void Start()
        {
            SetGridInfo();

            _beltManager = FindObjectOfType<BeltManager>();
            beltInSequence = null;
            beltInSequence = FindNextBelt();
            gameObject.name = $"Belt: {_beltID++}";
            _lineRenderer = GetComponent<LineRenderer>();

            cellSize = _gridBuildingSystem.GetCellSize();
        }

        private void Update()
        {
            if (beltInSequence == null)
            {
                beltInSequence = FindNextBelt();
                ActivateLineRenderer(false);
            }

            if (beltInSequence != null)
            {
                ActivateLineRenderer(true);
            }

            if (beltItem != null && beltItem.item != null)
            {
                StartCoroutine(StartBeltMove());
            }
        }

        public Vector3 GetItemPosition()
        {
            Vector3 position = transform.position;

            Vector2 centerOfBelt = position;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    centerOfBelt = new Vector3(position.x + (cellSize/2), position.y + (cellSize / 2));
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    centerOfBelt = new Vector3(position.x + (cellSize / 2), position.y - (cellSize / 2));
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    centerOfBelt = new Vector3(position.x - (cellSize / 2), position.y - (cellSize / 2));
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    centerOfBelt = new Vector3(position.x - (cellSize / 2), position.y + (cellSize / 2));
                    break;
            }
 
            return centerOfBelt;

        }

        private IEnumerator StartBeltMove()
        {
            Debug.Log("BeltMoveStarted");
            isSpaceTaken = true;
            beltItem.item.transform.parent = transform;

            if (beltItem.item != null && beltInSequence != null && beltInSequence.isSpaceTaken == false)
            {
                Vector3 toPosition = beltInSequence.GetItemPosition();

                beltInSequence.isSpaceTaken = true;

                var step = _beltManager.speed * Time.deltaTime;

                while (beltItem.item.transform.position != toPosition)
                {
                    beltItem.item.transform.position = Vector3.MoveTowards(beltItem.transform.position, toPosition, step);

                    yield return null;
                }

                isSpaceTaken = false;
                beltInSequence.beltItem = beltItem;
                beltItem = null;
            }
        }

        private Belt FindNextBelt()
        {
            Vector2Int cellOfNextBelt = Vector2Int.zero;

            switch (_dir)
            {
                case PlacedObjectTypeSO.Dir.Up:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x, _gridPosition.y + 1);
                    break;
                case PlacedObjectTypeSO.Dir.Right:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x + 1, _gridPosition.y);
                    break;
                case PlacedObjectTypeSO.Dir.Down:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x, _gridPosition.y - 1);
                    break;
                case PlacedObjectTypeSO.Dir.Left:
                    cellOfNextBelt = new Vector2Int(_gridPosition.x - 1, _gridPosition.y);
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


        public void SetGridInfo()
        {
            _placedObject = GetComponent<PlacedObject>();
            _placedObjectTypeSO = _placedObject.placedObjectTypeSO;
            _dir = _placedObject.dir;
            _gridBuildingSystem = _placedObject.gridBuildingSystem;

            Vector2Int vector2IntLocation = _placedObject.GetGridPositionList()[0];
            Vector3Int location = new Vector3Int(vector2IntLocation.x, vector2IntLocation.y, 0);
            _gridPosition = location;
        }

        public void ActivateLineRenderer(bool isActive)
        {
            if (isActive)
            {
                _lineRenderer.SetPosition(0, GetItemPosition());
                _lineRenderer.SetPosition(1, beltInSequence.GetItemPosition());
                _lineRenderer.enabled = true;
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
    }
}
