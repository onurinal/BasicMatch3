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
        private GridSpawner gridSpawner;
        private Candy[,] candyGrid;
        private int gridWidth;
        private int gridHeight;
        private int matchCounter;

        public List<Candy> MatchedCandyList { get; } = new List<Candy>();
        private const int MatchThreshold = 3;

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, LevelProperties levelProperties)
        {
            this.candyGrid = candyGrid;
            this.gridSpawner = gridSpawner;
            this.levelProperties = levelProperties;

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

        public void FillCandyToEmptySlot()
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
                                var targetPosition = gridSpawner.GetGridPosition(width, height);
                                candyGrid[width, height].StartCandyMovement(targetPosition);
                                // StartCandyMovement(candyGrid[width, i].transform, gridSpawner.GetGridPosition(width, height));
                                // candyGrid[width, height].SetTargetPosition(gridSpawner.GetGridPosition(width, height));
                                // candyGrid[width, height].StartMoving();
                                candyGrid[width, i] = null;
                                break;
                            }
                        }
                    }
                }
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
    }
}