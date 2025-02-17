using System.Collections;
using BasicMatch3.Candies;
using BasicMatch3.Manager;
using UnityEngine;

namespace BasicMatch3.Grid
{
    public class GridMovement
    {
        private LevelManager levelManager;
        private GridChecker gridChecker;
        private CandyProperties candyProperties;
        private Candy[,] candyGrid;
        private IEnumerator swapCandyCoroutine;

        public void Initialize(Candy[,] candyGrid, LevelManager levelManager, GridChecker gridChecker, CandyProperties candyProperties)
        {
            this.levelManager = levelManager;
            this.candyGrid = candyGrid;
            this.gridChecker = gridChecker;
            this.candyProperties = candyProperties;
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
    }
}