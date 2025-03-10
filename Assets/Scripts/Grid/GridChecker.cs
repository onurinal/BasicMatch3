using System.Collections.Generic;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridChecker
    {
        public List<Candy> MatchedCandyList { get; } = new List<Candy>();
        private const int MatchThreshold = 3;
        private const int BombCandyThreshold = MatchThreshold + 1;
        private const int RainbowCandyThreshold = MatchThreshold + 2;

        private CandyProperties candyProperties;
        private ILevelGrid levelManager;
        private GridSpawner gridSpawner;
        private Candy[,] candyGrid;
        private int gridWidth;
        private int gridHeight;

        private enum Direction
        {
            Horizontal,
            Vertical
        }

        public void Initialize(Candy[,] candyGrid, GridSpawner gridSpawner, LevelProperties levelProperties, CandyProperties candyProperties,
            ILevelGrid levelManager)
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
                    CheckCandiesInDirection(width, height, Direction.Vertical);
                    CheckCandiesInDirection(width, height, Direction.Horizontal);
                }
            }
        }

        private void CheckCandiesInDirection(int startX, int startY, Direction direction)
        {
            var startIndex = direction == Direction.Horizontal ? startX : startY;
            var gridSize = direction == Direction.Horizontal ? gridWidth : gridHeight;
            int matchCounter = 1;

            for (int i = startIndex; i < gridSize; i++)
            {
                var secondCandyX = direction == Direction.Horizontal ? i + 1 : startX;
                var secondCandyY = direction == Direction.Vertical ? i + 1 : startY;

                if (i + 1 >= gridSize || candyGrid[startX, startY] == null || candyGrid[secondCandyX, secondCandyY] == null) // to fix array boundary issue
                {
                    break;
                }

                if (IsCandyRainbow(startX, startY, secondCandyX, secondCandyY)) // if the candy is rainbow, do not add the matched list
                {
                    break;
                }

                if (candyGrid[startX, startY].CandyType == CandyType.Bomb)
                {
                    matchCounter++;

                    if (direction == Direction.Horizontal)
                    {
                        startX++;
                    }
                    else
                    {
                        startY++;
                    }

                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = startIndex; j < startIndex + MatchThreshold; j++)
                        {
                            var newX = direction == Direction.Horizontal ? j : startX;
                            var newY = direction == Direction.Vertical ? j : startY;
                            AddToMatchedCandies(newX, newY);
                        }
                    }

                    continue;
                }

                if (candyGrid[secondCandyX, secondCandyY].CandyType == CandyType.Bomb)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddToMatchedCandies(secondCandyX, secondCandyY);
                    }

                    if (matchCounter == MatchThreshold)
                    {
                        for (int j = startIndex; j < startIndex + MatchThreshold; j++)
                        {
                            var newX = direction == Direction.Horizontal ? j : startX;
                            var newY = direction == Direction.Vertical ? j : startY;
                            AddToMatchedCandies(newX, newY);
                        }
                    }

                    continue;
                }

                if (candyGrid[startX, startY].CandyType == candyGrid[secondCandyX, secondCandyY].CandyType)
                {
                    matchCounter++;

                    if (matchCounter > MatchThreshold)
                    {
                        AddToMatchedCandies(secondCandyX, secondCandyY);
                    }
                    else if (matchCounter == MatchThreshold)
                    {
                        for (int j = startIndex; j < startIndex + MatchThreshold; j++)
                        {
                            var newX = direction == Direction.Horizontal ? j : startX;
                            var newY = direction == Direction.Vertical ? j : startY;
                            AddToMatchedCandies(newX, newY);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private bool IsCandyRainbow(int firstCandyX, int firstCandyY, int secondCandyX, int secondCandyY)
        {
            return candyGrid[firstCandyX, firstCandyY].CandyType == CandyType.Rainbow || candyGrid[secondCandyX, secondCandyY].CandyType == CandyType.Rainbow;
        }

        private void AddToMatchedCandies(int width, int height)
        {
            if (!MatchedCandyList.Contains(candyGrid[width, height]))
            {
                MatchedCandyList.Add(candyGrid[width, height]);
            }
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
                        if (counter == BombCandyThreshold)
                        {
                            if (i + 2 < MatchedCandyList.Count)
                            {
                                if (MatchedCandyList[i].CandyType == MatchedCandyList[i + 2].CandyType)
                                {
                                    counter++;
                                    if (counter == RainbowCandyThreshold)
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
            if (candy == null) return;

            candy.DestroyCandy(levelManager.IsGridInitializing);
            candyGrid[width, height] = null;
        }

        // bomb candy destroy entire rows and columns
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
                if (candy != null && candy.CandyType == candyType)
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