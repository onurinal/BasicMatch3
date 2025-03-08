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
        private int gridWidth, gridHeight;

        private IEnumerator swapCandyCoroutine;
        private IEnumerator fillCandyToEmptySlotCoroutine;
        private IEnumerator fallCandiesCoroutine;

        public void Initialize(Candy[,] candyGrid, LevelManager levelManager, GridSpawner gridSpawner, GridChecker gridChecker, CandyProperties candyProperties, LevelProperties levelProperties)
        {
            this.levelManager = levelManager;
            this.gridSpawner = gridSpawner;
            this.candyGrid = candyGrid;
            this.gridChecker = gridChecker;
            this.candyProperties = candyProperties;
            this.levelProperties = levelProperties;

            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;
        }

        private IEnumerator SwapCandiesCoroutine(Candy firstCandy, Candy secondCandy)
        {
            if (!secondCandy)
            {
                yield break;
            }

            var firstCandyPosition = firstCandy.transform.position;
            var secondCandyPosition = secondCandy.transform.position;

            firstCandy.Move(secondCandyPosition);
            secondCandy.Move(firstCandyPosition);
            yield return new WaitForSeconds(candyProperties.MoveDuration);

            SwapCandies(firstCandy, secondCandy);

            // if player swap first candy and first candy is rainbow then apply rainbow candy power up
            if (firstCandy.CandyType == CandyType.Rainbow)
            {
                gridChecker.ApplyRainbowCandy(firstCandy, secondCandy.CandyType);
                yield return CoroutineHandler.Instance.StartCoroutine(levelManager.StartScanGrid());
                swapCandyCoroutine = null;
                yield break;
            }

            // after swapping candies if there is a match, then start scan grid
            var matchCount = gridChecker.CheckGridAndGetMatchedCandyCounts();
            if (matchCount > 0)
            {
                yield return CoroutineHandler.Instance.StartCoroutine(levelManager.StartScanGrid());
                swapCandyCoroutine = null;
                yield break;
            }

            // if there is no match after swapping, make sure to go back candies previous positions
            SwapCandies(firstCandy, secondCandy);
            var newFirstCandyPosition = firstCandy.transform.position;
            var newSecondCandyPosition = secondCandy.transform.position;

            firstCandy.Move(newSecondCandyPosition);
            secondCandy.Move(newFirstCandyPosition);
            yield return new WaitForSeconds(candyProperties.MoveDuration);
            swapCandyCoroutine = null;
        }

        private void SwapCandies(Candy candy1, Candy candy2)
        {
            var tempX = candy1.GridX;
            var tempY = candy1.GridY;
            candy1.SetIndices(candy2.GridX, candy2.GridY);
            candy2.SetIndices(tempX, tempY);
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

        // after the matched candies destroyed, this method moves the existing candies in the grid into the remaining empty slots
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
                                candyGrid[width, i].SetIndices(width, height);
                                candyGrid[width, height] = candyGrid[width, i];

                                var targetPosition = gridSpawner.GetCandyWorldPosition(width, height);

                                candyGrid[width, height].Move(targetPosition);
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

        // Move all candies to the top without delay to make them fal smoothly at the start
        public void MoveAllCandiesToTheTop()
        {
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    var targetPosition = gridSpawner.GetCandyWorldPosition(width, levelProperties.GridHeight - 1);
                    candyGrid[width, height].MoveWithNoDelay(targetPosition);
                }
            }
        }

        // make all candies fall from the top of the grid at the start
        private IEnumerator FallCandiesCoroutine(float spawnGapBetweenCandies)
        {
            for (int height = 0; height < gridHeight; height++)
            {
                for (int width = 0; width < gridWidth; width++)
                {
                    var targetPosition = gridSpawner.GetCandyWorldPosition(width, height);
                    candyGrid[width, height].Move(targetPosition);
                }

                yield return new WaitForSeconds(spawnGapBetweenCandies);
            }

            fallCandiesCoroutine = null;
        }

        public IEnumerator StartFallCandies(float spawnGapBetweenCandies)
        {
            if (fallCandiesCoroutine != null)
            {
                yield break;
            }

            fallCandiesCoroutine = FallCandiesCoroutine(spawnGapBetweenCandies);
            yield return CoroutineHandler.Instance.StartCoroutine(fallCandiesCoroutine);
        }

        private void StopFallCandies()
        {
            if (fallCandiesCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(fallCandiesCoroutine);
                fallCandiesCoroutine = null;
            }
        }
    }
}