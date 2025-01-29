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

            for (int width = 0; width < candyGrid.GetLength(0); width++)
            {
                for (int height = 0; height < candyGrid.GetLength(1); height++)
                {
                    candyGrid[width, height] = CreateCandy(GetGridPosition(width, height));
                }
            }
        }

        private Vector2 GetGridPosition(int width, int height)
        {
            float candyPositionX;
            float candyPositionY;
            if (levelProperties.GridWidth % 2 == 0)
            {
                candyPositionX = CalculatePosition(levelProperties.GridWidth, width, true);
                candyPositionY = CalculatePosition(levelProperties.GridHeight, height, true);
            }
            else
            {
                candyPositionX = CalculatePosition(levelProperties.GridWidth, width, false);
                candyPositionY = CalculatePosition(levelProperties.GridHeight, height, false);
            }

            return new Vector2(candyPositionX, candyPositionY);
        }

        private float CalculatePosition(int gridWidthOrHeight, int point, bool isEven)
        {
            float additionalForEven = isEven ? (scaleFactor / 2) : 0;
            float scaleFactorXY = gridWidthOrHeight / 2;
            if (scaleFactorXY <= point)
            {
                return (scaleFactorXY - point) * -1 * scaleFactor + additionalForEven;
            }
            else
            {
                return (point - scaleFactorXY) * scaleFactor + additionalForEven;
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