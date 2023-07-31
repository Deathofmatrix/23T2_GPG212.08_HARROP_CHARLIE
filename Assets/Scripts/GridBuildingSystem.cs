using Charlie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace ChocolateFactory
{
    public class GridBuildingSystem : MonoBehaviour
    {
        [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
        private PlacedObjectTypeSO placedObjectTypeSO;

        private Grid<GridObject> grid;
        private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

        private void Awake()
        {
            int gridWidth = 10;
            int gridHeight = 10;
            float cellsize = 10f;
            grid = new Grid<GridObject>(gridWidth, gridHeight, cellsize, Vector3.zero, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

            placedObjectTypeSO = placedObjectTypeSOList[0];
        }

        public class GridObject
        {
            private Grid<GridObject> grid;
            private int x;
            private int y;
            private PlacedObject placedObject;

            public GridObject(Grid<GridObject> grid, int x, int y)
            {
                this.grid = grid;
                this.x = x;
                this.y = y;
            }

            public void SetPlacedObject(PlacedObject placedObject)
            {
                this.placedObject = placedObject;
                grid.TriggerGridObjectChanged(x, y);
            }

            public PlacedObject GetPlacedObject()
            {
                return placedObject;
            }


            public void ClearPlacedObject()
            {
                placedObject = null;
                grid.TriggerGridObjectChanged(x, y);
            }

            public bool CanBuild()
            {
                return placedObject == null;
            }

            public override string ToString()
            {
                return $"{x}, {y}\n{placedObject}";
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                grid.GetXY(UtilsClass.GetMouseWorldPosition(), out int x, out int y);


                List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y), dir);

                //test can build
                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        //cannot build here
                        canBuild = false;
                        break;
                    }
                }

                if (canBuild)
                {
                    Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

                    PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, y), dir, placedObjectTypeSO);

                    foreach(Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }
                }
                else
                {
                    Debug.Log("Cannot build here!");
                }
            }

            //destroy Object
            if (Input.GetMouseButtonDown(1))
            {
                GridObject gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject != null)
                {
                    //demolish
                    placedObject.DestroySelf();

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = PlacedObjectTypeSO.GetNextDir(dir);
                Debug.Log(dir.ToString());
            }

            if (Input.GetKeyUp(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; }
            if (Input.GetKeyUp(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; }
            if (Input.GetKeyUp(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; }
        }
    }
}
