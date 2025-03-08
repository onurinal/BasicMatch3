using System.Collections;
using System.Collections.Generic;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using BasicMatch3.Manager;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridSpawner
    {
        private LevelManager levelManager;
        private GridChecker gridChecker;
        private GridMovement gridMovement;
        private LevelProperties levelProperties;
        private CandyProperties candyProperties;
        private Candy[,] candyGrid;
        private Transform candiesParent;
        private int gridWidth, gridHeight;

        private IEnumerator createNewCandiesCoroutine;
        private IEnumerator createNewGridCoroutine;

        private ManualGrid manualGrid; // REMOVE OR DISABLE AFTER TESTING FEATURES

        public void Initialize(LevelManager levelManager, GridChecker gridChecker, GridMovement gridMovement, CandyProperties candyProperties,
            LevelProperties levelProperties, Transform candiesParent)
        {
            this.levelManager = levelManager;
            this.gridChecker = gridChecker;
            this.gridMovement = gridMovement;
            this.candyProperties = candyProperties;
            this.candiesParent = candiesParent;
            this.levelProperties = levelProperties;
            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;

            CreateNewGrid();

            // REMOVE OR DISABLE AFTER TESTING FEATURES
            manualGrid = new ManualGrid();
            manualGrid.Initialize(candyGrid, this, candyProperties, levelManager, candiesParent);

            if (manualGrid.IsManualGrid)
            {
                manualGrid.CreateNewCandies();
            }
            else
            {
                CreateCandiesToNewGrid();
            }

            gridChecker.Initialize(candyGrid, this, levelProperties, candyProperties, levelManager);
            gridMovement.Initialize(candyGrid, levelManager, this, gridChecker, candyProperties, levelProperties);
        }

        private void CreateNewGrid()
        {
            candyGrid = new Candy[gridWidth, gridHeight];
        }

        private void CreateCandiesToNewGrid()
        {
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    var candyPosition = GetCandyWorldPosition(width, height);
                    CreateCandy(candyPosition, width, height, candyProperties.CandyPrefab);
                }
            }
        }

        public Vector2 GetCandyWorldPosition(int width, int height)
        {
            return new Vector2(width * candyProperties.ScaleFactor, height * candyProperties.ScaleFactor);
        }

        public Candy GetCandyPrefabAtIndex(int width, int height)
        {
            return candyGrid[width, height];
        }

        public void CreateCandy(Vector2 position, int width, int height, Candy candyPrefab)
        {
            var candy = Object.Instantiate(candyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            candyGrid[width, height] = candy.Initialize(width, height, levelManager);
        }

        private IEnumerator CreateNewCandyToEmptySlotCoroutine(float spawnGapBetweenCandies)
        {
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        if (candyGrid[width, j] == null)
                        {
                            var startPosition = GetCandyWorldPosition(width, gridHeight - 1);
                            var targetPosition = GetCandyWorldPosition(width, j);
                            CreateCandy(startPosition, width, j, candyProperties.CandyPrefab);
                            candyGrid[width, j].Move(targetPosition);
                            break;
                        }
                    }
                }

                yield return new WaitForSeconds(spawnGapBetweenCandies);
            }

            yield return new WaitForSeconds(candyProperties.MoveDuration - spawnGapBetweenCandies);
            createNewCandiesCoroutine = null;
        }

        public IEnumerator StartCreateNewCandiesToEmptySlot(float spawnGapBetweenCandies)
        {
            createNewCandiesCoroutine = CreateNewCandyToEmptySlotCoroutine(spawnGapBetweenCandies);
            yield return CoroutineHandler.Instance.StartCoroutine(createNewCandiesCoroutine);
        }

        public void StopCreateNewCandiesToEmptySlot()
        {
            if (createNewCandiesCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(createNewCandiesCoroutine);
                createNewCandiesCoroutine = null;
            }
        }

        public void ShowAllCandies()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    if (candyGrid[width, height] != null)
                    {
                        candyGrid[width, height].CandySprite.enabled = true;
                    }
                }
            }
        }
    }
}