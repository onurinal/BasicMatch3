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

        private readonly List<Candy> emptyCandyList = new List<Candy>();
        private IEnumerator createNewCandiesCoroutine;
        private IEnumerator createNewGridCoroutine;

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
            gridChecker.Initialize(candyGrid, this, levelProperties, candyProperties);
            gridMovement.Initialize(candyGrid, levelManager, gridChecker, candyProperties);
        }

        private void CreateNewGrid()
        {
            candyGrid = new Candy[gridWidth, gridHeight];

            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    var candyPosition = GetCandyWorldPosition(width, height);
                    candyGrid[width, height] = CreateCandy(candyPosition, width, height);
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

        private Candy CreateCandy(Vector2 position, int width, int height)
        {
            var candy = Object.Instantiate(candyProperties.CandyPrefab, position, Quaternion.identity);
            candy.transform.SetParent(candiesParent);
            return candy.Initialize(width, height, levelManager);
        }

        private IEnumerator CreateNewCandyForEmptySlotCoroutine(float spawnGapBetweenCandies)
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
                            candyGrid[width, j] = CreateCandy(startPosition, width, j);
                            candyGrid[width, j].StartMoving(startPosition, targetPosition);
                            break;
                        }
                    }
                }

                yield return new WaitForSeconds(spawnGapBetweenCandies);
            }

            createNewCandiesCoroutine = null;
        }

        public IEnumerator StartCreateNewCandies(float spawnGapBetweenCandies)
        {
            // StopCreateNewCandies();
            createNewCandiesCoroutine = CreateNewCandyForEmptySlotCoroutine(spawnGapBetweenCandies);
            yield return CoroutineHandler.Instance.StartCoroutine(createNewCandiesCoroutine);
        }

        public void StopCreateNewCandies()
        {
            if (createNewCandiesCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(createNewCandiesCoroutine);
                createNewCandiesCoroutine = null;
            }
        }

        public void MoveAllCandiesToTheTop()
        {
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    // candyGrid[width, height].transform.position = GetCandyWorldPosition(width, gridHeight - 1);
                    var startPosition = GetCandyWorldPosition(width, height);
                    var targetPosition = GetCandyWorldPosition(width, gridHeight - 1);
                    candyGrid[width, height].StartMoving(startPosition, targetPosition);
                }
            }
        }

        private IEnumerator CreateNewGridCoroutine(float spawnGapBetweenCandies)
        {
            // make the candies start falling at the top of the grid
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    var startPosition = GetCandyWorldPosition(width, gridHeight - 1);
                    var targetPosition = GetCandyWorldPosition(width, height);
                    candyGrid[width, height].StartMoving(startPosition, targetPosition);
                }

                yield return new WaitForSeconds(spawnGapBetweenCandies);
            }

            createNewGridCoroutine = null;
        }

        public IEnumerator StartCreateNewGrid(float spawnGapBetweenCandies)
        {
            if (createNewGridCoroutine != null)
            {
                yield break;
            }

            createNewGridCoroutine = CreateNewGridCoroutine(spawnGapBetweenCandies);
            yield return CoroutineHandler.Instance.StartCoroutine(createNewGridCoroutine);
        }

        private void StopCreateNewGrid()
        {
            if (createNewGridCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(createNewGridCoroutine);
                createNewGridCoroutine = null;
            }
        }

        public void ShowAllCandies()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    candyGrid[width, height].CandySprite.enabled = true;
                }
            }
        }
    }
}