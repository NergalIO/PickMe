using System.Collections;
using PickMe.Core.Managers;
using PickMe.UI.Menus.Base;
using UnityEngine;

namespace PickMe.Utils
{
    /// <summary>
    /// Utility methods for menu operations.
    /// </summary>
    public static class MenuUtils
    {
        /// <summary>
        /// Opens a menu and sets data after it's activated. Use this for menus that need data passed after opening.
        /// </summary>
        public static IEnumerator OpenMenuAndSetData<T>(string menuId, System.Action<T> setDataAction) where T : Menu
        {
            if (!UIController.IsInitialized)
            {
                Debug.LogWarning($"MenuUtils: UIController not initialized, cannot open menu '{menuId}'");
                yield break;
            }

            UIController.Instance.Open(menuId);
            yield return null; // Wait one frame for menu to be activated

            var menu = UIController.Instance.GetMenu(menuId);
            if (menu is T targetMenu)
            {
                setDataAction?.Invoke(targetMenu);
            }
            else
            {
                Debug.LogWarning($"MenuUtils: Menu '{menuId}' is not of type {typeof(T).Name} or not found");
            }
        }

        /// <summary>
        /// Clears all child objects from a transform container.
        /// </summary>
        public static void ClearContainer(Transform container)
        {
            if (container == null) return;

            for (int i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i);
                if (Application.isPlaying)
                {
                    Object.Destroy(child.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Closes current menu through UIController if available.
        /// </summary>
        public static void CloseCurrentMenu()
        {
            if (UIController.IsInitialized)
            {
                UIController.Instance.CloseCurrent();
            }
        }
    }
}

