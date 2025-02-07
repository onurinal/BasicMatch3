using System.Collections;
using BasicMatch3.CameraManager;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelProperties levelProperties;
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform candiesParent;

        private GridSpawner gridSpawner;
        private GridChecker gridChecker;

        private IEnumerator scanGridCoroutine;

        public void Start()
        {
            cameraController.Initialize(levelProperties, candyProperties.ScaleFactor);
            gridChecker = new GridChecker();
            gridSpawner = new GridSpawner();
            gridSpawner.Initialize(candyProperties, levelProperties, gridChecker, candiesParent);

            StartScanGrid();
        }

        private IEnumerator ScanGridCoroutine()
        {
            gridChecker.CheckAllCandies();
            do
            {
                gridChecker.DestroyMatchedCandies();
                gridChecker.FillCandyToEmptySlot();
                yield return new WaitForSeconds(candyProperties.FallDuration / 3f);
                gridSpawner.CreateNewCandyForEmptySlot();
                yield return new WaitForSeconds(candyProperties.FallDuration);
                gridChecker.CheckAllCandies();
                yield return null;
            } while (gridChecker.MatchedCandyList.Count > 0);
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
    }
}