using System.Collections;
using System.Collections.Generic;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridChecker
    {
        private LevelProperties levelProperties;
        private CandyProperties candyProperties;
        private GridSpawner gridSpawner;
        private Candy[,] candyGrid;
        private int gridWidth;
        private int gridHeight;
        private int matchCounter;

        private IEnumerator fillCandyToEmptySlotCoroutine;
        public List<Candy> MatchedCandyList { get; } = new List<Candy>();
        private const int MatchThreshold = 3;

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, LevelProperties levelProperties, CandyProperties candyProperties)
        {
            this.candyGrid = candyGrid;
            this.gridSpawner = gridSpawner;
            this.levelProperties = levelProperties;
            this.candyProperties = candyProperties;

            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;
        }

        public void CheckAllCandies()
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
            if (MatchedCandyList.Contains(candyGrid[width, height]))
            {
                return;
            }

            MatchedCandyList.Add(candyGrid[width, height]);
        }

        private IEnumerator FillCandyToEmptySlotCoroutine(float moveDuration)
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
                                candyGrid[width, i].GridX = width;
                                candyGrid[width, i].GridY = height;
                                candyGrid[width, height] = candyGrid[width, i];
                                var startPosition = gridSpawner.GetCandyWorldPosition(width, i);
                                var targetPosition = gridSpawner.GetCandyWorldPosition(width, height);
                                candyGrid[width, height].StartMoving(startPosition, targetPosition);
                                candyGrid[width, i] = null;
                                break;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(moveDuration);
            fillCandyToEmptySlotCoroutine = null;
        }

        public IEnumerator StartFillCandyToEmptySlot(float moveDuration)
        {
            // StopFillCandyToEmptySlot();
            fillCandyToEmptySlotCoroutine = FillCandyToEmptySlotCoroutine(moveDuration);
            yield return CoroutineHandler.Instance.StartCoroutine(fillCandyToEmptySlotCoroutine);
        }

        public void StopFillCandyToEmptySlot()
        {
            if (fillCandyToEmptySlotCoroutine != null)
            {
                CoroutineHandler.Instance.StartCoroutine(fillCandyToEmptySlotCoroutine);
                fillCandyToEmptySlotCoroutine = null;
            }
        }

        public void DestroyMatchedCandies()
        {
            foreach (Candy candy in MatchedCandyList)
            {
                var width = candy.GridX;
                var height = candy.GridY;
                Object.Destroy(candy.gameObject);
                candyGrid[width, height] = null;
            }

            MatchedCandyList.Clear();
        }

        public int GetMatchedCandyCounts()
        {
            CheckAllCandies();
            return MatchedCandyList.Count;
        }
    }
}