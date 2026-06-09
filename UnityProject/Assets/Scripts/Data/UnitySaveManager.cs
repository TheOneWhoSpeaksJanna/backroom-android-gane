using System;
using UnityEngine;

namespace Desolation.Data
{
    [Serializable]
    public class GameStateData
    {
        public int maxUnlockedLevel = 0;
        public int currentDifficulty = 1; // 0=Easy, 1=Normal, 2=Hard
        public float lookSensitivity = 1.0f;
        public bool soundHumEnabled = true;
    }

    public static class UnitySaveManager
    {
        private const string SaveKey = "Desolation_Backrooms_Save";

        public static GameStateData LoadProgress()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                try
                {
                    string rawJson = PlayerPrefs.GetString(SaveKey);
                    return JsonUtility.FromJson<GameStateData>(rawJson);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[SaveManager] Error reading JSON save: " + ex.Message);
                }
            }

            // Fallback default setup state
            return new GameStateData();
        }

        public static void SaveProgress(GameStateData data)
        {
            try
            {
                string rawJson = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(SaveKey, rawJson);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError("[SaveManager] Error writing JSON save: " + ex.Message);
            }
        }

        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("[SaveManager] User progress reset completed.");
        }
    }
}
