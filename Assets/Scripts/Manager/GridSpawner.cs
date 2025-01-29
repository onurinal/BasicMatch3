using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private GridChecker gridChecker;
        [SerializeField] private LevelProperties levelProperties;
        [SerializeField] private Candy candyPrefab;
        [SerializeField] private Transform candiesParent;

        private Candy[,] candyGrid;

        private readonly float scaleFactor = 0.45f;

        private void Start()
        {
            CreateNewGrid();
            gridChecker.Initialize(candyGrid);
        }

        private void CreateNewGrid()
        {
            candyGrid = new Candy[levelProperties.GridHeight, levelProperties.GridWidth];

            for (int height = 0; height < candyGrid.GetLength(0); height++)
            {
                for (int width = 0; width < candyGrid.GetLength(1); width++)
                {
                    candyGrid[height, width] = CreateCandy(GetGridPosition(height, width));
                }
            }
        }

        private Vector2 GetGridPosition(int height, int width)
        {
            float candyPositionX;
            float candyPositionY;
            if (levelProperties.GridWidth % 2 == 0)
            {
                candyPositionX = CalculatePositionForEven(levelProperties.GridWidth, width);
                candyPositionY = CalculatePositionForEven(levelProperties.GridHeight, height);
            }
            else
            {
                candyPositionX = CalculatePositionForOdd(levelProperties.GridWidth, width);
                candyPositionY = CalculatePositionForOdd(levelProperties.GridHeight, height);
            }

            return new Vector2(candyPositionX, candyPositionY);
        }

        private float CalculatePositionForEven(int widthOrHeight, int point)
        {
            float scaleFactorX = widthOrHeight / 2;

            if (scaleFactorX <= point)
            {
                return (scaleFactorX - point) * -1 * scaleFactor + (scaleFactor / 2);
            }
            else
            {
                return (point - scaleFactorX) * scaleFactor + (scaleFactor / 2);
            }
        }

        private float CalculatePositionForOdd(int widthOrHeight, int point)
        {
            float scaleFactorXY = widthOrHeight / 2;
            if (scaleFactorXY <= point)
            {
                return (scaleFactorXY - point) * -1 * scaleFactor;
            }
            else
            {
                return (point - scaleFactorXY) * scaleFactor;
            }
        }

        private Candy CreateCandy(Vector2 position)
        {
            var candy = Instantiate(candyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            return candy.Initialize();
        }
    }
}