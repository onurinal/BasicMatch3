using UnityEngine;
using UnityEngine.SceneManagement;

namespace BasicMatch3.Manager
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadSameScene()
        {
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadNextScene()
        {
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning("Scene " + SceneManager.GetActiveScene().name + " is out of range");
                return;
            }

            SceneManager.LoadScene(sceneIndex + 1);
        }
    }
}