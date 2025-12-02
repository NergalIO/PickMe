using System.IO;
using UnityEngine;
namespace PickMe.Managers
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        private const string SAVE_FILE_NAME = "savegame.json";
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        private PlayerSaveData currentSaveData;
        public PlayerSaveData CurrentSaveData => currentSaveData;
        protected override void OnAwake()
        {
            base.OnAwake();
            LoadGame();
        }
        public void LoadGame()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(SaveFilePath);
                    currentSaveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);
                    Debug.Log("[SaveManager] Игра загружена успешно");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveManager] Ошибка загрузки игры: {e.Message}");
                    CreateNewSave();
                }
            }
            else
            {
                CreateNewSave();
            }
        }
        public void SaveGame()
        {
            if (currentSaveData == null)
            {
                Debug.LogWarning("[SaveManager] Нет данных для сохранения!");
                return;
            }
            try
            {
                UpdateSaveData();
                string jsonData = JsonUtility.ToJson(currentSaveData, true);
                File.WriteAllText(SaveFilePath, jsonData);
                Debug.Log("[SaveManager] Игра сохранена успешно");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Ошибка сохранения игры: {e.Message}");
            }
        }
        private void CreateNewSave()
        {
            currentSaveData = new PlayerSaveData();
            Debug.Log("[SaveManager] Создано новое сохранение");
            SaveGame();
        }
        private void UpdateSaveData()
        {
            // TODO: Собрать данные из всех менеджеров
            if (CharacterManager.HasInstance)
            {
            }
            if (CombatManager.HasInstance)
            {
                currentSaveData.currentFloor = CombatManager.Instance.CurrentFloor;
            }
        }
        public void DeleteSave()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                CreateNewSave();
                Debug.Log("[SaveManager] Сохранение удалено");
            }
        }
    }
    [System.Serializable]
    public class PlayerSaveData
    {
        public int currentFloor = 1;
        public int tickets = 0;
        public int rubies = 0;
        // TODO: Добавить другие данные (персонажи, здания и т.д.)
    }
}
