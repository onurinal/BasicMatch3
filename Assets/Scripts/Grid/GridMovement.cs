using System.Collections;
using BasicMatch3.Candies;
using BasicMatch3.Level;
using BasicMatch3.Manager;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridMovement
    {
        private LevelManager levelManager;
        private GridSpawner gridSpawner;
        private GridChecker gridChecker;
        private CandyProperties candyProperties;
        private LevelProperties levelProperties;
        private Candy[,] candyGrid;

        private IEnumerator swapCandyCoroutine;
        private IEnumerator fillCandyToEmptySlotCoroutine;

        public void Initialize(Candy[,] candyGrid, LevelManager levelManager, GridSpawner gridSpawner, GridChecker gridChecker, CandyProperties candyProperties, LevelProperties levelProperties)
        {
            this.levelManager = levelManager;
            this.gridSpawner = gridSpawner;
            this.candyGrid = candyGrid;
            this.gridChecker = gridChecker;
            this.candyProperties = candyProperties;
            this.levelProperties = levelProperties;
        }

        private IEnumerator SwapCandiesCoroutine(Candy firstCandy, Candy secondCandy)
        {
            if (!secondCandy)
            {
                yield break;
            }

            var firstCandyPosition = firstCandy.transform.position;
            var secondCandyPosition = secondCandy.transform.position;

            firstCandy.StartSwapping(firstCandyPosition, secondCandyPosition);
            secondCandy.StartSwapping(secondCandyPosition, firstCandyPosition);
            yield return new WaitForSeconds(candyProperties.MoveDuration);

            SwapCandies(firstCandy, secondCandy);

            if (firstCandy.CandyType == CandyType.Rainbow)
            {
                gridChecker.ApplyRainbowCandy(firstCandy, secondCandy.CandyType);
                yield return CoroutineHandler.Instance.StartCoroutine(levelManager.StartScanGrid());
                swapCandyCoroutine = null;
                yield break;
            }

            var matchCount = gridChecker.GetMatchedCandyCounts();
            if (matchCount > 0)
            {
                yield return CoroutineHandler.Instance.StartCoroutine(levelManager.StartScanGrid());
                swapCandyCoroutine = null;
                yield break;
            }

            SwapCandies(firstCandy, secondCandy);
            var newFirstCandyPosition = firstCandy.transform.position;
            var newSecondCandyPosition = secondCandy.transform.position;

            firstCandy.StartSwapping(newFirstCandyPosition, newSecondCandyPosition);
            secondCandy.StartSwapping(newSecondCandyPosition, newFirstCandyPosition);
            yield return new WaitForSeconds(candyProperties.MoveDuration);
            swapCandyCoroutine = null;
        }

        private void SwapCandies(Candy candy1, Candy candy2)
        {
            var tempX = candy1.GridX;
            var tempY = candy1.GridY;
            candy1.GridX = candy2.GridX;
            candy1.GridY = candy2.GridY;
            candy2.GridX = tempX;
            candy2.GridY = tempY;
            candyGrid[candy1.GridX, candy1.GridY] = candy1;
            candyGrid[candy2.GridX, candy2.GridY] = candy2;
        }

        public void StartSwapCandies(Candy firstCandy, Candy secondCandy)
        {
            if (swapCandyCoroutine != null)
            {
                return;
            }

            swapCandyCoroutine = SwapCandiesCoroutine(firstCandy, secondCandy);
            CoroutineHandler.Instance.StartCoroutine(swapCandyCoroutine);
        }

        public void StopSwapCandies()
        {
            if (swapCandyCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(swapCandyCoroutine);
                swapCandyCoroutine = null;
            }
        }

        private IEnumerator FillCandyToEmptySlotCoroutine(float moveDuration)
        {
            for (int width = 0; width < levelProperties.GridWidth; width++)
            {
                for (int height = 0; height < levelProperties.GridHeight; height++)
                {
                    if (candyGrid[width, height] == null)
                    {
                        for (int i = height; i < levelProperties.GridHeight; i++)
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

        public void MoveAllCandiesToTheTop()
        {
            for (int height = 0; height < levelProperties.GridHeight; height++)
            {
                for (int width = 0; width < levelProperties.GridWidth; width++)
                {
                    var targetPosition = gridSpawner.GetCandyWorldPosition(width, levelProperties.GridHeight - 1);
                    candyGrid[width, height].MoveToTop(targetPosition);
                }
            }
        }
    }
}