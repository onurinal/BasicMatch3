using UnityEngine;

namespace PlayerSettings
{
    public class PlayerPrefController
    {
        private string masterLevel;
        private const int DefaultLevel = 1;

        public void SetMasterLevel(int level)
        {
            PlayerPrefs.SetInt(masterLevel, level);
        }

        public int GetMasterLevel()
        {
            if (!PlayerPrefs.HasKey(masterLevel))
            {
                PlayerPrefs.SetInt(masterLevel, DefaultLevel);
            }

            return PlayerPrefs.GetInt(masterLevel);
        }
    }
}