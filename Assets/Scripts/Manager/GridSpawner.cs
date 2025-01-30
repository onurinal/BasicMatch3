using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GridChecker gridChecker;
        [SerializeField] private LevelProperties levelProperties;
        [SerializeField] private Candy candyPrefab;
        [SerializeField] private Transform candiesParent;

        private Candy[,] candyGrid;

        private readonly float scaleFactor = 0.45f;

        private void Start()
        {
            UpdateCameraPosition();
            CreateNewGrid();
            gridChecker.Initialize(candyGrid);
        }

        private void CreateNewGrid()
        {
            candyGrid = new Candy[levelProperties.GridWidth, levelProperties.GridHeight];

            for (int width = 0; width < candyGrid.GetLength(0); width++)
            {
                for (int height = 0; height < candyGrid.GetLength(1); height++)
                {
                    var candyPosition = GetGridPosition(width, height);
                    candyGrid[width, height] = CreateCandy(candyPosition);
                }
            }
        }

        private Vector2 GetGridPosition(int width, int height)
        {
            return new Vector2(width * scaleFactor, height * scaleFactor);
        }

        private Candy CreateCandy(Vector2 position)
        {
            var candy = Instantiate(candyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            return candy.Initialize();
        }

        private void UpdateCameraPosition()
        {
            float newCameraPositionX = CalculateCameraPosition(levelProperties.GridWidth);
            float newCameraPositionY = CalculateCameraPosition(levelProperties.GridHeight);

            mainCamera.transform.position = new Vector3(newCameraPositionX, newCameraPositionY, mainCamera.transform.position.z);
        }

        private float CalculateCameraPosition(int dimension)
        {
            if (dimension % 2 == 0)
            {
                return (dimension / 2 * scaleFactor) - (scaleFactor / 2);
            }
            else
            {
                return dimension / 2 * scaleFactor;
            }
        }
    }
}