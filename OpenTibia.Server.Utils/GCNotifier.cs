using System;

namespace OpenTibia.Utilities
{
    public class GcNotifier
    {
        public static event EventHandler GarbageCollected;

        ~GcNotifier()
        {
            if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                return;
            }

            new GcNotifier();

            GarbageCollected?.Invoke(null, EventArgs.Empty);
        }

        public static void Start()
        {
            new GcNotifier();
        }
    }
}
