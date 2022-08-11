using DeviceMonitorApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DeviceMonitorApp.Code.Notifications.DeviceStatusNotifier;

namespace DeviceMonitorApp.Code.Notifications
{
    public interface INotifier
    {
        public event DeviceMonitoringDelegate OnDeviceStatusChanged;
        Task<List<DeviceModel>> GetDevices();
        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC");
        public Task<int> GetTotalCount();
    }

}
