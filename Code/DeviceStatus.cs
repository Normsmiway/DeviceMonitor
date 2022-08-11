namespace DeviceMonitorApp.Code
{
    public class DeviceStatus
    {
        public static string Up => nameof(Up).ToLowerInvariant();
        public static string Down => nameof(Down).ToLowerInvariant();
        public static string Supervising => nameof(Supervising).ToLowerInvariant();
    }
}
