using System.Collections;
using System.Collections.Generic;
using BasicMatch3.CameraManager;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using BasicMatch3.Player;
using PlayerSettings;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class LevelManager : MonoBehaviour, ILevelGrid
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform candiesParent;
        [SerializeField] private List<LevelProperties> levelPropertiesList;
        private GridSpawner gridSpawner;
        private GridChecker gridChecker;
        private GridMovement gridMovement;

        private PlayerPrefController playerPrefController;
        private LevelProperties currentLevelProperties;
        private readonly Dictionary<int, LevelProperties> levelPropertiesDictionary = new Dictionary<int, LevelProperties>();

        public bool IsGridInitializing { get; private set; } = true;

        private IEnumerator scanGridCoroutine;
        private IEnumerator newLevelCoroutine;

        private void OnDestroy()
        {
            if (CoroutineHandler.Instance != null)
            {
                StopNewLevel();
                StopScanGrid();
            }
        }

        public void Initialize(PlayerPrefController playerPrefController)
        {
            this.playerPrefController = playerPrefController;

            UpdateLevelPropertiesDictionary();
            GetLevelPropertiesForCurrentLevel();

            cameraController.Initialize(currentLevelProperties, candyProperties.ScaleFactor);
            gridMovement = new GridMovement();
            gridChecker = new GridChecker();
            gridSpawner = new GridSpawner();
            gridSpawner.Initialize(this, gridChecker, gridMovement, candyProperties, currentLevelProperties, candiesParent);
            playerController.Initialize(gridMovement, gridSpawner, currentLevelProperties);

            StartNewLevel();
        }

        private void UpdateLevelPropertiesDictionary()
        {
            for (int i = 0; i < levelPropertiesList.Count; i++)
            {
                int level = i + 1;
                levelPropertiesDictionary.Add(level, levelPropertiesList[i]);
            }
        }

        private void GetLevelPropertiesForCurrentLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (!levelPropertiesDictionary.TryGetValue(currentLevel, out var levelProperties))
            {
                Debug.LogWarning("No level properties found for active scene: " + currentLevel);
                return;
            }

            currentLevelProperties = levelProperties;
        }

        private IEnumerator ScanGridCoroutine()
        {
            var moveDuration = IsGridInitializing ? 0f : candyProperties.MoveDuration;
            var destroyDuration = IsGridInitializing ? 0f : candyProperties.DestroyDuration;

            do
            {
                gridChecker.DestroyMatchedCandies(); // destroy matched candies
                yield return new WaitForSeconds(destroyDuration); // wait for destroying
                yield return gridMovement.StartFillCandyToEmptySlot(moveDuration); // after destroy filling up candies to empty slot
                yield return gridSpawner.StartCreateNewCandiesToEmptySlot(moveDuration / 3f); //  create new candies to empty after filling up
                gridChecker.CheckAllCandies(); // after new candies check grid again
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

        private IEnumerator NewLevelCoroutine()
        {
            gridSpawner.HideAllCandies();
            yield return CoroutineHandler.Instance.StartCoroutine(StartScanGrid());
            gridMovement.MoveAllCandiesToTheTop();
            gridSpawner.ShowAllCandies();
            yield return gridMovement.StartFallCandies(candyProperties.MoveDuration / 1.5f);
            IsGridInitializing = false;
            newLevelCoroutine = null;
        }

        // THIS IS FOR MANUAL GRID - TESTING - DISABLE IT IF NOT USING
        // private IEnumerator NewLevelCoroutine()
        // {
        //     IsGridInitializing = false;
        //     yield return CoroutineHandler.Instance.StartCoroutine(StartScanGrid());
        //     yield return new WaitForSeconds(candyProperties.MoveDuration);
        //     // yield return gridSpawner.StartFallCandies(candyProperties.MoveDuration / 1.5f);
        //     newLevelCoroutine = null;
        // }

        private void StartNewLevel()
        {
            if (newLevelCoroutine != null)
            {
                return;
            }

            newLevelCoroutine = NewLevelCoroutine();
            CoroutineHandler.Instance.StartCoroutine(newLevelCoroutine);
        }

        private void StopNewLevel()
        {
            if (newLevelCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(newLevelCoroutine);
                newLevelCoroutine = null;
            }
        }

        //----------------- TESTING -------------------

        public void RunNextLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (currentLevel + 1 >= levelPropertiesList.Count)
            {
                Debug.LogWarning("No level properties found for next level");
                return;
            }

            playerPrefController.SetMasterLevel(currentLevel + 1);
            CoroutineHandler.Instance.StopAllCoroutines();
            sceneLoader.LoadSameScene();
        }

        public void RunPreviousLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (currentLevel - 1 <= 0)
            {
                Debug.LogWarning("No level properties found for previous level");
                return;
            }

            playerPrefController.SetMasterLevel(currentLevel - 1);
            CoroutineHandler.Instance.StopAllCoroutines();
            sceneLoader.LoadSameScene();
        }
    }
}