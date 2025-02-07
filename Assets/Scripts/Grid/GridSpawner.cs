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
        private int gridWidth, gridHeight;

        public void Initialize(CandyProperties candyProperties, LevelProperties levelProperties, GridChecker gridChecker, Transform candiesParent)
        {
            this.candyProperties = candyProperties;
            this.gridChecker = gridChecker;
            this.candiesParent = candiesParent;
            this.levelProperties = levelProperties;
            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;

            CreateNewGrid();
            gridChecker.Initialize(candyGrid, this, levelProperties);
        }

        private void CreateNewGrid()
        {
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

        private Candy CreateCandy(Vector2 position, int width, int height)
        {
            var candy = Object.Instantiate(candyProperties.CandyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            return candy.Initialize(width, height);
        }

        public void CreateNewCandyForEmptySlot()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    if (candyGrid[width, height] == null)
                    {
                        var position = GetGridPosition(width, gridHeight - 1);
                        candyGrid[width, height] = CreateCandy(position, width, height);
                        var targetPosition = GetGridPosition(width, height);
                        candyGrid[width, height].StartCandyMovement(targetPosition);
                        // StartCandyMovement(candyGrid[width, height].transform, targetPosition);
                        // candyGrid[width, height].SetTargetPosition(targetPosition);
                        // candyGrid[width, height].StartMoving();
                    }
                }
            }
        }
    }
}