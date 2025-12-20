using PickMe.Infrastructure;

namespace PickMe.Utils
{
    /// <summary>
    /// Helper for managing event subscriptions in menus.
    /// </summary>
    public static class EventSubscriptionHelper
    {
        /// <summary>
        /// Subscribes to events if EventController is initialized.
        /// </summary>
        public static void SubscribeIfReady<T>(System.Action<T> handler) where T : struct
        {
            if (EventController.IsInitialized)
            {
                EventController.Instance.Subscribe(handler);
            }
        }

        /// <summary>
        /// Unsubscribes from events if EventController is initialized.
        /// </summary>
        public static void UnsubscribeIfReady<T>(System.Action<T> handler) where T : struct
        {
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe(handler);
            }
        }
    }
}

