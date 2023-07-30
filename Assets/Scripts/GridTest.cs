using Charlie.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    public class GridTest : MonoBehaviour
    {
        private Grid<GridObject> grid;
        private Grid<StringGridObject> stringGrid;
        private void Start()
        {
            //grid = new Grid<GridObject>(20, 10, 8f, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
            stringGrid = new Grid<StringGridObject>(20, 10, 8f, transform.position, (Grid<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));
        }

        private void Update()
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();

            //if (Input.GetMouseButtonDown(0))
            //{
            //    GridObject gridObject = grid.GetGridObject(position);
            //    if (gridObject != null)
            //    {
            //        gridObject.AddValue(5);
            //    }
            //}
            if (Input.GetKeyDown(KeyCode.A)) { stringGrid.GetGridObject(position).AddLettter("A"); }
            if (Input.GetKeyDown(KeyCode.B)) { stringGrid.GetGridObject(position).AddLettter("B"); }
            if (Input.GetKeyDown(KeyCode.C)) { stringGrid.GetGridObject(position).AddLettter("C"); }

            if (Input.GetKeyDown(KeyCode.Alpha1)) { stringGrid.GetGridObject(position).AddNumber("1"); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { stringGrid.GetGridObject(position).AddNumber("2"); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { stringGrid.GetGridObject(position).AddNumber("3"); }
        }

    }

    public class GridObject
    {
        private const int MIN = 0;
        private const int MAX = 100;

        private Grid<GridObject> grid;
        private int x;
        private int y;
        private int value;

        //The GridObject contstructs with a reference to the grid that its in. This is mega clever DO IT
        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void AddValue(int addValue)
        {
            value += addValue;
            Mathf.Clamp(value, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public float GetValueNormalised()
        {
            return (float)value / MAX;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class StringGridObject
    {
        private Grid<StringGridObject> grid;
        private int x;
        private int y;

        private string letters;
        private string numbers;

        public StringGridObject(Grid<StringGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            letters = string.Empty;
            numbers = string.Empty;
        }

        public void AddLettter(string letter)
        {
            letters += letter;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void AddNumber(string number)
        {
            numbers += number;
            grid.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return letters + "\n" + numbers;
        }
    }
}
