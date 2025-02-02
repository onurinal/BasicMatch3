using System.Collections.Generic;
using BasicMatch3.Candies;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GridChecker : MonoBehaviour
    {
        private Candy[,] candyGrid;
        private List<Candy> matchedCandyList = new List<Candy>();
        private int matchCounter = 0;

        private void Start()
        {
        }

        public void Initialize(Candy[,] candyGrid)
        {
            this.candyGrid = candyGrid;
            CheckAllCandies();
        }

        private void CheckCandiesIfMatch3(int newWidth, int newHeight, bool isCheckingWidth)
        {
            matchCounter++;

            int iteration = isCheckingWidth ? newWidth : newHeight;

            for (int i = iteration; i < candyGrid.GetLength(1); i++)
            {
                // if iteration is for last two candies and didn't match yet then break the loop
                if (i + 2 >= candyGrid.GetLength(1) && matchCounter <= 1)
                {
                    matchCounter = 0;
                    break;
                }

                if (i + 1 >= candyGrid.GetLength(1)) // to fix array boundary issue
                {
                    matchCounter = 0;
                    break;
                }

                if (!isCheckingWidth)
                {
                    if (candyGrid[newWidth, newHeight].CandyType == candyGrid[newWidth, i + 1].CandyType)
                    {
                        matchCounter++;

                        if (matchCounter > 3)
                        {
                            AddListCandiesIfMatched(newWidth, i + 1);
                        }
                        else if (matchCounter == 3)
                        {
                            AddListCandiesIfMatched(newWidth, newHeight);
                            AddListCandiesIfMatched(newWidth, i);
                            AddListCandiesIfMatched(newWidth, i + 1);
                        }
                    }
                    else
                    {
                        matchCounter = 0;
                        break;
                    }
                }
                else
                {
                    if (candyGrid[newWidth, newHeight].CandyType == candyGrid[i + 1, newHeight].CandyType)
                    {
                        matchCounter++;

                        if (matchCounter > 3)
                        {
                            AddListCandiesIfMatched(i + 1, newHeight);
                        }
                        else if (matchCounter == 3)
                        {
                            AddListCandiesIfMatched(newWidth, newHeight);
                            AddListCandiesIfMatched(i, newHeight);
                            AddListCandiesIfMatched(i + 1, newHeight);
                        }
                    }
                    else
                    {
                        matchCounter = 0;
                        break;
                    }
                }
            }
        }

        private void CheckAllCandies()
        {
            for (int width = 0; width < candyGrid.GetLength(0); width++)
            {
                for (int height = 0; height < candyGrid.GetLength(1); height++)
                {
                    CheckCandiesIfMatch3(width, height, false);
                    CheckCandiesIfMatch3(width, height, true);
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
    }
}