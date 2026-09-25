namespace Chixi.After
{
    // Instrumentacion para la comparativa. No forma parte del diseno.
    public static class Metrics
    {
        public static int AdapterPolls;
        public static int PlatformNotifications;
        public static void Reset() { AdapterPolls = 0; PlatformNotifications = 0; }
    }
}
