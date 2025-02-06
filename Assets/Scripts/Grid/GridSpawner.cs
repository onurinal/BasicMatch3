using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridSpawner
    {
        private LevelProperties levelProperties;
        private CandyProperties candyProperties;
        private GridChecker gridChecker;
        private Candy[,] candyGrid;
        private Transform candiesParent;

        public void Initialize(CandyProperties candyProperties, LevelProperties levelProperties, GridChecker gridChecker, Transform candiesParent)
        {
            this.candyProperties = candyProperties;
            this.gridChecker = gridChecker;
            this.candiesParent = candiesParent;
            this.levelProperties = levelProperties;
            CreateNewGrid();
            gridChecker.Initialize(candyGrid, this, candyProperties, levelProperties);
        }

        private void CreateNewGrid()
        {
            var gridWidth = levelProperties.GridWidth;
            var gridHeight = levelProperties.GridHeight;
            candyGrid = new Candy[gridWidth, gridHeight];

            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    var candyPosition = GetGridPosition(width, height);
                    candyGrid[width, height] = CreateCandy(candyPosition, width, height);
                }
            }
        }

        public Vector2 GetGridPosition(int width, int height)
        {
            return new Vector2(width * candyProperties.ScaleFactor, height * candyProperties.ScaleFactor);
        }

        public Candy CreateCandy(Vector2 position, int width, int height)
        {
            var candy = Object.Instantiate(candyProperties.CandyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            return candy.Initialize(width, height);
        }
    }
}