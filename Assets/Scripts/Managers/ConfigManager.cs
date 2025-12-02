using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace PickMe.Managers
{
    public class ConfigManager : PersistentSingleton<ConfigManager>
    {
        [Header("Google Sheets Settings")]
        [SerializeField] private string googleSheetsBaseUrl = "https:
        [SerializeField] private string summonSheetGid = "0";
        [SerializeField] private string abilityListSheetGid = "0";
        [SerializeField] private string towerFloorsSheetGid = "0";
        [SerializeField] private string enemySheetGid = "0";
        private bool isConfigLoaded = false;
        public bool IsConfigLoaded => isConfigLoaded;
        protected override void OnAwake()
        {
            base.OnAwake();
            StartCoroutine(LoadConfigs());
        }
        private IEnumerator LoadConfigs()
        {
            Debug.Log("[ConfigManager] Начало загрузки конфигов...");
            yield return StartCoroutine(LoadConfig("summon", summonSheetGid));
            yield return StartCoroutine(LoadConfig("ability_list", abilityListSheetGid));
            yield return StartCoroutine(LoadConfig("tower_floors", towerFloorsSheetGid));
            yield return StartCoroutine(LoadConfig("enemy", enemySheetGid));
            isConfigLoaded = true;
            Debug.Log("[ConfigManager] Все конфиги загружены!");
            OnConfigsLoaded();
        }
        private IEnumerator LoadConfig(string configName, string gid)
        {
            string url = googleSheetsBaseUrl + gid;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string csvData = request.downloadHandler.text;
                    ProcessConfigData(configName, csvData);
                    Debug.Log($"[ConfigManager] Конфиг '{configName}' загружен успешно");
                }
                else
                {
                    Debug.LogError($"[ConfigManager] Ошибка загрузки конфига '{configName}': {request.error}");
                    // TODO: Загрузка из локального fallback файла
                }
            }
        }
        private void ProcessConfigData(string configName, string csvData)
        {
            // TODO: Реализовать парсинг CSV в зависимости от типа конфига
            switch (configName)
            {
                case "summon":
                    break;
                case "ability_list":
                    break;
                case "tower_floors":
                    break;
                case "enemy":
                    break;
            }
        }
        private void OnConfigsLoaded()
        {
            // TODO: Уведомить другие системы о готовности конфигов
        }
        public void ReloadConfigs()
        {
            isConfigLoaded = false;
            StartCoroutine(LoadConfigs());
        }
    }
}
