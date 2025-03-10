using BasicMatch3.Candies;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class ManualGrid
    {
        private GridSpawner gridSpawner;
        private CandyProperties candyProperties;
        private Candy[,] candyGrid;
        private Transform candiesParent;

        public readonly bool IsManualGrid = false;

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, CandyProperties candyProperties, Transform candiesParent)
        {
            this.gridSpawner = gridSpawner;
            this.candyGrid = candyGrid;
            this.candyProperties = candyProperties;
            this.candiesParent = candiesParent;
        }

        public void CreateNewCandies()
        {
            var candyPosition = gridSpawner.GetCandyWorldPosition(0, 0);
            CreateCandy(candyPosition, 0, 0, candyProperties.CandyPrefab, CandyType.Biscuit);
            candyPosition = gridSpawner.GetCandyWorldPosition(0, 1);
            CreateCandy(candyPosition, 0, 1, candyProperties.CandyPrefab, CandyType.Chocolate);
            candyPosition = gridSpawner.GetCandyWorldPosition(0, 2);
            CreateCandy(candyPosition, 0, 2, candyProperties.CandyPrefab, CandyType.Pasta);
            candyPosition = gridSpawner.GetCandyWorldPosition(0, 3);
            CreateCandy(candyPosition, 0, 3, candyProperties.CandyPrefab, CandyType.Donut);
            candyPosition = gridSpawner.GetCandyWorldPosition(1, 0);
            CreateCandy(candyPosition, 1, 0, candyProperties.BombCandyPrefab, CandyType.Bomb); // TEST THIS ONE
            candyPosition = gridSpawner.GetCandyWorldPosition(1, 1);
            CreateCandy(candyPosition, 1, 1, candyProperties.CandyPrefab, CandyType.Pasta);
            candyPosition = gridSpawner.GetCandyWorldPosition(1, 2);
            CreateCandy(candyPosition, 1, 2, candyProperties.CandyPrefab, CandyType.IceCream);
            candyPosition = gridSpawner.GetCandyWorldPosition(1, 3);
            CreateCandy(candyPosition, 1, 3, candyProperties.CandyPrefab, CandyType.Biscuit);
            candyPosition = gridSpawner.GetCandyWorldPosition(2, 0);
            CreateCandy(candyPosition, 2, 0, candyProperties.CandyPrefab, CandyType.Biscuit);
            candyPosition = gridSpawner.GetCandyWorldPosition(2, 1);
            CreateCandy(candyPosition, 2, 1, candyProperties.CandyPrefab, CandyType.IceCream);
            candyPosition = gridSpawner.GetCandyWorldPosition(2, 2);
            CreateCandy(candyPosition, 2, 2, candyProperties.CandyPrefab, CandyType.Chocolate);
            candyPosition = gridSpawner.GetCandyWorldPosition(2, 3);
            CreateCandy(candyPosition, 2, 3, candyProperties.CandyPrefab, CandyType.Pasta);
            candyPosition = gridSpawner.GetCandyWorldPosition(3, 0);
            CreateCandy(candyPosition, 3, 0, candyProperties.CandyPrefab, CandyType.IceCream);
            candyPosition = gridSpawner.GetCandyWorldPosition(3, 1);
            CreateCandy(candyPosition, 3, 1, candyProperties.CandyPrefab, CandyType.Donut);
            candyPosition = gridSpawner.GetCandyWorldPosition(3, 2);
            CreateCandy(candyPosition, 3, 2, candyProperties.CandyPrefab, CandyType.Biscuit);
            candyPosition = gridSpawner.GetCandyWorldPosition(3, 3);
            CreateCandy(candyPosition, 3, 3, candyProperties.CandyPrefab, CandyType.Pasta);
        }

        private void CreateCandy(Vector2 position, int width, int height, Candy candyPrefab, CandyType candyType)
        {
            var candy = Object.Instantiate(candyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            // candyGrid[width, height] = candy.InitializeForTest(width, height, candyType);
        }
    }
}