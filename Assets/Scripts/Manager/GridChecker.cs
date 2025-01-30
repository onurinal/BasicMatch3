using BasicMatch3.Candies;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GridChecker : MonoBehaviour
    {
        private Candy[,] candyGrid;
        private int matchCounter = 0;

        private void Start()
        {
        }

        public void Initialize(Candy[,] candyGrid)
        {
            this.candyGrid = candyGrid;
            CheckMatchForEveryCandy();
        }

        private void IsMatch3ForHeight(int newWidth, int newHeight)
        {
            candyGrid[newWidth, newHeight].IsMatching = true;
            matchCounter++;

            for (int i = newHeight; i < candyGrid.GetLength(1); i++)
            {
                if (i + 2 >= candyGrid.GetLength(1) && matchCounter <= 1)
                {
                    if (!candyGrid[newWidth, newHeight].IsAlreadyMatched)
                    {
                        candyGrid[newWidth, newHeight].IsMatching = false;
                    }

                    matchCounter = 0;
                    break;
                }

                if (i + 1 >= candyGrid.GetLength(1))
                {
                    if (matchCounter >= 3)
                    {
                        matchCounter = 0;
                        break;
                    }

                    if (matchCounter <= 2)
                    {
                        if (!candyGrid[newWidth, i].IsAlreadyMatched)
                        {
                            candyGrid[newWidth, i].IsMatching = false;
                        }

                        if (!candyGrid[newWidth, newHeight].IsAlreadyMatched)
                        {
                            candyGrid[newWidth, newHeight].IsMatching = false;
                        }
                    }

                    matchCounter = 0;
                    break;
                }

                if (candyGrid[newWidth, newHeight].CandyType == candyGrid[newWidth, i + 1].CandyType)
                {
                    candyGrid[newWidth, i + 1].IsMatching = true;
                    matchCounter++;
                }
                else if (matchCounter >= 3)
                {
                    matchCounter = 0;
                    break;
                }
                else if (matchCounter <= 2)
                {
                    if (!candyGrid[newWidth, newHeight].IsAlreadyMatched)
                    {
                        candyGrid[newWidth, newHeight].IsMatching = false;
                    }

                    if (!candyGrid[newWidth, i].IsAlreadyMatched)
                    {
                        candyGrid[newWidth, i].IsMatching = false;
                    }

                    matchCounter = 0;
                    break;
                }
            }
        }

        private void IsMatchForWidth(int newWidth, int newHeight)
        {
            candyGrid[newWidth, newHeight].IsMatching = true;
            matchCounter++;

            for (int i = newWidth; i < candyGrid.GetLength(0); i++)
            {
                if (i + 1 >= candyGrid.GetLength(0))
                {
                    if (matchCounter >= 3)
                    {
                        matchCounter = 0;
                        break;
                    }

                    if (matchCounter <= 2)
                    {
                        if (!candyGrid[i, newHeight].IsAlreadyMatched)
                        {
                            candyGrid[i, newHeight].IsMatching = false;
                        }

                        if (!candyGrid[newWidth, newHeight].IsAlreadyMatched)
                        {
                            candyGrid[newWidth, newHeight].IsMatching = false;
                        }
                    }

                    matchCounter = 0;
                    break;
                }

                if (candyGrid[newWidth, newHeight].CandyType == candyGrid[i + 1, newHeight].CandyType)
                {
                    candyGrid[i + 1, newHeight].IsMatching = true;
                    matchCounter++;
                }
                else if (matchCounter >= 3)
                {
                    matchCounter = 0;
                    break;
                }
                else if (matchCounter <= 2)
                {
                    if (!candyGrid[newWidth, newHeight].IsAlreadyMatched)
                    {
                        candyGrid[newWidth, newHeight].IsMatching = false;
                    }

                    if (!candyGrid[i, newHeight].IsAlreadyMatched)
                    {
                        candyGrid[i, newHeight].IsMatching = false;
                    }

                    matchCounter = 0;
                    break;
                }
            }
        }

        private void CheckMatchForEveryCandy()
        {
            for (int width = 0; width < candyGrid.GetLength(0); width++)
            {
                for (int height = 0; height < candyGrid.GetLength(1); height++)
                {
                    IsMatch3ForHeight(width, height);
                    MarkCandiesIfMatchingForHeight(width, height);
                    IsMatchForWidth(width, height);
                    MarkCandiesIfMatchingForWidth(width, height);
                }
            }
        }

        private void MarkCandiesIfMatchingForHeight(int width, int height)
        {
            for (int newHeight = height; newHeight < candyGrid.GetLength(1); newHeight++)
            {
                if (candyGrid[width, newHeight].IsMatching)
                {
                    candyGrid[width, newHeight].IsAlreadyMatched = true;
                }
            }
        }

        private void MarkCandiesIfMatchingForWidth(int width, int height)
        {
            for (int newWidth = width; newWidth < candyGrid.GetLength(0); newWidth++)
            {
                if (candyGrid[newWidth, height].IsMatching)
                {
                    candyGrid[newWidth, height].IsAlreadyMatched = true;
                }
            }
        }
    }
}