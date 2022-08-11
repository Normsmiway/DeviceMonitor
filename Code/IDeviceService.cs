using DeviceMonitorApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMonitorApp.Code
{
    public interface IDeviceService
    {
        public Task<List<DeviceModel>> GetDevices();
        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC");
        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC", DateTimeOffset? startDate = default, DateTimeOffset?  endDate = default);
        public Task<Stats> GetDeviceStats(string deviceName, DateTimeOffset? startDate, DateTimeOffset? endDate);
        public Task<int> GetTotalCount();
        public Task<int> GetArchivesTotalCount();
        public Task<List<string>> GetDeviceNames(string coulmnName = "keys");
    }
}
