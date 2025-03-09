﻿using System.Collections;
using System.Collections.Generic;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using BasicMatch3.Manager;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridChecker
    {
        private const int MatchThreshold = 3;

        private CandyProperties candyProperties;
        private LevelManager levelManager;
        private GridSpawner gridSpawner;
        private Candy[,] candyGrid;
        private int gridWidth;
        private int gridHeight;

        public List<Candy> MatchedCandyList { get; } = new List<Candy>();

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, LevelProperties levelProperties, CandyProperties candyProperties,
            LevelManager levelManager)
        {
            this.candyGrid = candyGrid;
            this.gridSpawner = gridSpawner;
            this.candyProperties = candyProperties;
            this.levelManager = levelManager;

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

        private void CheckCandiesForHeight(int width, int height)
        {
            int matchCounter = 1;
            int firstHeight = height;

            for (int i = height; i < gridHeight; i++)
            {
                if (i + 1 >= gridHeight) // to fix array boundary issue
                {
                    break;
                }

                if (candyGrid[width, height] == null || candyGrid[width, i + 1] == null)
                {
                    break;
                }

                if (IsCandyRainbowForHeight(width, i)) // if the candy is rainbow, do not add the matched list
                {
                    break;
                }

                if (candyGrid[width, height].CandyType == CandyType.Bomb)
                {
                    matchCounter++;
                    height++;
                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstHeight; j < firstHeight + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(width, j);
                        }
                    }

                    continue;
                }

                if (candyGrid[width, i + 1].CandyType == CandyType.Bomb)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddCandiesMatchedList(width, i + 1);
                    }

                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstHeight; j < firstHeight + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(width, j);
                        }
                    }

                    continue;
                }

                if (candyGrid[width, height].CandyType == candyGrid[width, i + 1].CandyType)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddCandiesMatchedList(width, i + 1);
                    }
                    else if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstHeight; j < firstHeight + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(width, j);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void CheckCandiesForWidth(int width, int height)
        {
            int matchCounter = 1;
            int firstWidth = width;

            for (int i = width; i < gridWidth; i++)
            {
                if (i + 1 >= gridWidth) // to fix array boundary issue
                {
                    break;
                }

                if (candyGrid[width, height] == null || candyGrid[i + 1, height] == null)
                {
                    break;
                }

                if (IsCandyRainbowForWidth(i, height)) // if the candy is rainbow, do not add the matched list
                {
                    break;
                }

                if (candyGrid[width, height].CandyType == CandyType.Bomb)
                {
                    matchCounter++;
                    width++;
                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstWidth; j < firstWidth + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(j, height);
                        }
                    }

                    continue;
                }

                if (candyGrid[i + 1, height].CandyType == CandyType.Bomb)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddCandiesMatchedList(i + 1, height);
                    }

                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstWidth; j < firstWidth + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(j, height);
                        }
                    }

                    continue;
                }

                if (candyGrid[width, height].CandyType == candyGrid[i + 1, height].CandyType)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddCandiesMatchedList(i + 1, height);
                    }
                    else if (matchCounter == MatchThreshold)
                    {
                        for (int j = firstWidth; j < firstWidth + MatchThreshold; j++)
                        {
                            AddCandiesMatchedList(j, height);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private bool IsCandyRainbowForHeight(int width, int height)
        {
            return candyGrid[width, height].CandyType == CandyType.Rainbow || candyGrid[width, height + 1].CandyType == CandyType.Rainbow;
        }

        private bool IsCandyRainbowForWidth(int width, int height)
        {
            return candyGrid[width, height].CandyType == CandyType.Rainbow || candyGrid[width + 1, height].CandyType == CandyType.Rainbow;
        }

        private void AddCandiesMatchedList(int width, int height)
        {
            if (MatchedCandyList.Contains(candyGrid[width, height]))
            {
                return;
            }

            MatchedCandyList.Add(candyGrid[width, height]);
        }

        public void DestroyMatchedCandies()
        {
            if (MatchedCandyList != null)
            {
                // if the grid is generating at the start then do not spawn any special candy
                // otherwise if matchThreshold + 1 same type candies matched then spawn bomb candy
                // if matchThreshold + 2 same type candies matched then spawn rainbow candy

                // If there is any bomb candy in matched list then first apply bomb candy effect
                if (!levelManager.IsGridInitializing)
                {
                    ApplyBombCandyIfInMatchedList();
                    CheckMatchCandiesToCreateSpecialCandy();
                }
                else
                {
                    foreach (Candy candy in MatchedCandyList)
                    {
                        DestroyCandy(candy, candy.GridX, candy.GridY);
                    }
                }

                MatchedCandyList.Clear();
            }
        }

        private void CheckMatchCandiesToCreateSpecialCandy()
        {
            var counter = 1;
            for (int i = 0; i < MatchedCandyList.Count; i++)
            {
                var width = MatchedCandyList[i].GridX;
                var height = MatchedCandyList[i].GridY;
                var candyPosition = gridSpawner.GetCandyWorldPosition(MatchedCandyList[i].GridX, MatchedCandyList[i].GridY);

                if (i + 1 < MatchedCandyList.Count)
                {
                    if (MatchedCandyList[i].CandyType == MatchedCandyList[i + 1].CandyType)
                    {
                        counter++;
                        if (counter == MatchThreshold + 1)
                        {
                            if (i + 2 < MatchedCandyList.Count)
                            {
                                if (MatchedCandyList[i].CandyType == MatchedCandyList[i + 2].CandyType)
                                {
                                    counter++;
                                    if (counter == MatchThreshold + 2)
                                    {
                                        DestroyCandy(MatchedCandyList[i], width, height);
                                        gridSpawner.CreateCandy(candyPosition, width, height, candyProperties.RainbowCandyPrefab);
                                        counter = 1;
                                        continue;
                                    }
                                }
                            }

                            DestroyCandy(MatchedCandyList[i], width, height);
                            gridSpawner.CreateCandy(candyPosition, width, height, candyProperties.BombCandyPrefab);
                            counter = 1;
                            continue;
                        }
                    }
                    else
                    {
                        counter = 1;
                    }
                }

                DestroyCandy(MatchedCandyList[i], width, height);
            }
        }

        private void DestroyCandy(Candy candy, int width, int height)
        {
            if (candy != null)
            {
                candy.DestroyCandy(levelManager.IsGridInitializing);
                candyGrid[width, height] = null;
            }
        }

        private void ApplyBombCandyIfInMatchedList()
        {
            foreach (var candy in MatchedCandyList)
            {
                if (candy.CandyType == CandyType.Bomb)
                {
                    var bombCandyWidth = candy.GridX;
                    var bombCandyHeight = candy.GridY;

                    for (int width = 0; width < gridWidth; width++)
                    {
                        if (bombCandyWidth == width)
                        {
                            continue;
                        }

                        DestroyCandy(candyGrid[width, bombCandyHeight], width, bombCandyHeight);
                    }

                    for (int height = 0; height < gridHeight; height++)
                    {
                        if (bombCandyHeight == height)
                        {
                            continue;
                        }

                        DestroyCandy(candyGrid[bombCandyWidth, height], bombCandyWidth, height);
                    }

                    DestroyCandy(candy, bombCandyWidth, bombCandyHeight);
                }
            }
        }

        public void ApplyRainbowCandy(Candy rainbowCandy, CandyType candyType)
        {
            foreach (var candy in candyGrid)
            {
                if (candy.CandyType == candyType)
                {
                    DestroyCandy(candy, candy.GridX, candy.GridY);
                }
            }

            DestroyCandy(rainbowCandy, rainbowCandy.GridX, rainbowCandy.GridY);
        }

        public int CheckGridAndGetMatchedCandyCounts()
        {
            CheckAllCandies();
            return MatchedCandyList.Count;
        }
    }
}