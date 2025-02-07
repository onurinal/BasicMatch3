using System.Collections;
using System.Collections.Generic;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridChecker
    {
        private const int MatchThreshold = 3;

        private LevelProperties levelProperties;
        private CandyProperties candyProperties;
        private GridSpawner gridSpawner;
        private Candy[,] candyGrid;
        private readonly List<Candy> matchedCandyList = new List<Candy>();

        private int gridWidth;
        private int gridHeight;
        private int matchCounter;

        private IEnumerator candyMovementCoroutine;
        private IEnumerator scanGridCoroutine;

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, CandyProperties candyProperties, LevelProperties levelProperties)
        {
            this.candyGrid = candyGrid;
            this.gridSpawner = gridSpawner;
            this.candyProperties = candyProperties;
            this.levelProperties = levelProperties;

            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;

            StartScanGrid();
        }

        private IEnumerator ScanGridCoroutine()
        {
            CheckAllCandies();
            do
            {
                DestroyMatchedCandies();
                FillCandyToEmptySlot();
                CreateNewCandyForEmptySlot();
                yield return new WaitForSeconds(candyProperties.FallDuration);
                yield return null;
                CheckAllCandies();
            } while (matchedCandyList.Count > 0);

            yield return null;
        }

        private void StartScanGrid()
        {
            StopScanGrid();
            scanGridCoroutine = ScanGridCoroutine();
            CoroutineHandler.Instance.StartCoroutine(scanGridCoroutine);
        }

        private void StopScanGrid()
        {
            if (scanGridCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(scanGridCoroutine);
                scanGridCoroutine = null;
            }
        }

        private void CheckAllCandies()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    CheckCandiesForHeight(width, height);
                    CheckCandiesForWidth(width, height);
                }
            }
        }

        private void CheckCandiesForHeight(int newWidth, int newHeight)
        {
            matchCounter++;

            for (int i = newHeight; i < gridHeight; i++)
            {
                if (i + 1 >= gridHeight) // to fix array boundary issue
                {
                    matchCounter = 0;
                    break;
                }

                if (candyGrid[newWidth, newHeight].CandyType == candyGrid[newWidth, i + 1].CandyType)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddListCandiesIfMatched(newWidth, i + 1);
                    }
                    else if (matchCounter == MatchThreshold)
                    {
                        for (int j = newHeight; j < newHeight + MatchThreshold; j++)
                        {
                            AddListCandiesIfMatched(newWidth, j);
                        }
                    }
                }
                else
                {
                    matchCounter = 0;
                    break;
                }
            }
        }

        private void CheckCandiesForWidth(int newWidth, int newHeight)
        {
            matchCounter++;

            for (int i = newWidth; i < gridWidth; i++)
            {
                if (i + 1 >= gridWidth) // to fix array boundary issue
                {
                    matchCounter = 0;
                    break;
                }

                if (candyGrid[newWidth, newHeight].CandyType == candyGrid[i + 1, newHeight].CandyType)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddListCandiesIfMatched(i + 1, newHeight);
                    }
                    else if (matchCounter == MatchThreshold)
                    {
                        for (int j = newWidth; j < newWidth + MatchThreshold; j++)
                        {
                            AddListCandiesIfMatched(j, newHeight);
                        }
                    }
                }
                else
                {
                    matchCounter = 0;
                    break;
                }
            }
        }

        private void AddListCandiesIfMatched(int width, int height)
        {
            if (matchedCandyList.Contains(candyGrid[width, height]))
            {
                return;
            }

            matchedCandyList.Add(candyGrid[width, height]);
        }

        private void FillCandyToEmptySlot()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    if (candyGrid[width, height] == null)
                    {
                        for (int i = height; i < gridHeight; i++)
                        {
                            if (candyGrid[width, i] != null)
                            {
                                candyGrid[width, height] = candyGrid[width, i];
                                candyGrid[width, i].GridX = width;
                                candyGrid[width, i].GridY = height;
                                StartCandyMovement(candyGrid[width, i].transform, gridSpawner.GetGridPosition(width, height));
                                candyGrid[width, i] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void CreateNewCandyForEmptySlot()
        {
            for (int width = 0; width < gridWidth; width++)
            {
                for (int height = 0; height < gridHeight; height++)
                {
                    if (candyGrid[width, height] == null)
                    {
                        var position = gridSpawner.GetGridPosition(width, gridHeight - 1);
                        candyGrid[width, height] = gridSpawner.CreateCandy(position, width, height);
                        var targetPosition = gridSpawner.GetGridPosition(width, height);
                        StartCandyMovement(candyGrid[width, height].transform, targetPosition);
                    }
                }
            }
        }

        private void DestroyMatchedCandies()
        {
            foreach (Candy candy in matchedCandyList)
            {
                var width = candy.GridX;
                var height = candy.GridY;
                Object.Destroy(candy.gameObject);
                candyGrid[width, height] = null;
            }

            matchedCandyList.Clear();
        }

        // ------------------------------- CANDY MOVEMENT ------------------------
        private IEnumerator StartCandyMovementCoroutine(Transform candyTransform, Vector3 targetPosition)
        {
            var elapsedTime = 0f;
            while (elapsedTime < candyProperties.FallDuration)
            {
                candyTransform.position = Vector3.Lerp(candyTransform.position, targetPosition, elapsedTime / candyProperties.FallDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            candyTransform.position = targetPosition;
        }

        private void StartCandyMovement(Transform candyTransform, Vector3 targetPosition)
        {
            candyMovementCoroutine = StartCandyMovementCoroutine(candyTransform, targetPosition);
            CoroutineHandler.Instance.StartCoroutine(candyMovementCoroutine);
        }

        private void StopCandyMovement()
        {
            if (candyMovementCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(candyMovementCoroutine);
                candyMovementCoroutine = null;
            }
        }
    }
}