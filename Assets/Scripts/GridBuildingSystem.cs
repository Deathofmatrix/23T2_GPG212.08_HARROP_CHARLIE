using Charlie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace ChocolateFactory
{
    //Code Sourced Mostly from UnityCodeMonkey.com (Awesome Grid building System)
    public class GridBuildingSystem : MonoBehaviour
    {
        public static GridBuildingSystem Instance { get; private set; }

        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;

        [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
        private PlacedObjectTypeSO placedObjectTypeSO;

        private Grid<GridObject> grid;
        private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Up;

        [SerializeField] private Transform gridCell;
        [SerializeField] private ItemSO itemTest1;
        [SerializeField] private ItemSO itemTest2;
        [SerializeField] private ItemSO outputTest;

        [SerializeField] private PlacedObjectTypeSO outputSO;
        [SerializeField] private Vector2 outputTile;
        [SerializeField] private PlacedObjectTypeSO.Dir outputDir = PlacedObjectTypeSO.Dir.Up;
        public ItemSO goalItem;

        private void Awake()
        {
            Instance = this;

            int gridWidth = 15;
            int gridHeight = 10;
            float cellsize = 10f;
            grid = new Grid<GridObject>(gridWidth, gridHeight, cellsize, Vector3.zero, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

            placedObjectTypeSO = null;
        }

        private void Start()
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    Transform tile = Instantiate(gridCell, grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * 0.5f, Quaternion.identity);
                    tile.SetParent(gameObject.transform);

                    if (x == outputTile.x && y == outputTile.y)
                    {
                        List<Vector2Int> gridPositionList = outputSO.GetGridPositionList(new Vector2Int(x, y), outputDir);

                        Vector2Int rotationOffset = outputSO.GetRotationOffset(outputDir);
                        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();
                        PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, y), outputDir, outputSO, this);

                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                        }

                        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
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

                    PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, y), dir, placedObjectTypeSO, this);

                    foreach(Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }

                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                outputTest = CraftingDatabase.CheckIfRecipieValid(PlacedObjectTypeSO.BuildingType.Combiner, itemTest1, itemTest2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }

            if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        }

        private void DeselectObjectType()
        {
            placedObjectTypeSO = null; RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType()
        {
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            grid.GetXY(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int y);

            if (placedObjectTypeSO != null)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else
            {
                return mousePosition;
            }
        }

        public Quaternion GetPlacedObjectRotation()
        {
            if (placedObjectTypeSO != null)
            {
                return Quaternion.Euler(0, 0, placedObjectTypeSO.GetRotationAngle(dir));
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSO()
        {
            return placedObjectTypeSO;
        }

        public PlacedObject GetPlacedObjectXY(int x, int y)
        {
            GridObject gridobject = grid.GetGridObject(x, y);
            PlacedObject placedobject = gridobject.GetPlacedObject();
            return placedobject;
        }

        public float GetCellSize()
        {
            return grid.GetCellSize();
        }
    }
}
