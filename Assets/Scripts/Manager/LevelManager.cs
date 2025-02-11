using System.Collections;
using BasicMatch3.CameraManager;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using BasicMatch3.Player;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private LevelProperties levelProperties;
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform candiesParent;

        private GridSpawner gridSpawner;
        private GridChecker gridChecker;
        private GridMovement gridMovement;

        private IEnumerator scanGridCoroutine;

        public void Start()
        {
            Initialize();
            // StartScanGrid();
            CoroutineHandler.Instance.StartCoroutine(StartScanGrid());
        }

        private void Initialize()
        {
            cameraController.Initialize(levelProperties, candyProperties.ScaleFactor);
            gridMovement = new GridMovement();
            gridChecker = new GridChecker();
            gridSpawner = new GridSpawner();
            gridSpawner.Initialize(this, gridChecker, gridMovement, candyProperties, levelProperties, candiesParent);
            playerController.Initialize(gridMovement, gridSpawner, levelProperties);
        }

        private IEnumerator ScanGridCoroutine()
        {
            gridChecker.CheckAllCandies();
            do
            {
                yield return new WaitForSeconds(0.5f);
                gridChecker.DestroyMatchedCandies();
                yield return gridChecker.StartFillCandyToEmptySlot();
                yield return gridSpawner.StartCreateNewCandies();
                gridChecker.CheckAllCandies();
                yield return null;
            } while (gridChecker.MatchedCandyList.Count > 0);
        }

        public IEnumerator StartScanGrid()
        {
            StopScanGrid();
            scanGridCoroutine = ScanGridCoroutine();
            yield return CoroutineHandler.Instance.StartCoroutine(scanGridCoroutine);
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