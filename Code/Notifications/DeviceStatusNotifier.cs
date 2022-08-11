using DeviceMonitorApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;

namespace DeviceMonitorApp.Code.Notifications
{
    public class DeviceStatusNotifier : INotifier, IDisposable
    {
        public delegate void DeviceMonitoringDelegate(object sender, DeviceChangeEventArgs args);

        private readonly string TableName = string.Empty;
        private readonly string ConnectionString = string.Empty;
        private SqlTableDependency<DeviceModel> _notifier;
        private IConfiguration _configuration;
        public event DeviceMonitoringDelegate OnDeviceStatusChanged;
        private readonly IDeviceService _deviceService;
        public DeviceStatusNotifier(IConfiguration configuration, IDeviceService deviceService)
        {
            _configuration = configuration;
            TableName = _configuration["AppSettings:TableName"] ?? throw new ArgumentNullException(nameof(TableName));
            ConnectionString = _configuration["ConnectionStrings:Default"] ?? throw new ArgumentNullException(nameof(ConnectionString));
            _deviceService = deviceService;
            #region mapper model to table
            var mapper = new ModelToTableMapper<DeviceModel>();
            mapper.AddMapping(c => c.Id, "activityid");
            mapper.AddMapping(c => c.Name, "keys");
            mapper.AddMapping(c => c.Status, "status");
            mapper.AddMapping(c => c.CashLevel, "cashlevel");
            mapper.AddMapping(c => c.Cassette, "Cassette");
            mapper.AddMapping(c => c.CardReader, "CardReader");
            mapper.AddMapping(c => c.CashJam, "CashJam");
            mapper.AddMapping(c => c.NetworkStatus, "networkstatus");
            mapper.AddMapping(c => c.ReceiptPrinter, "ReceiptPrinter");
            mapper.AddMapping(c => c.ReceiptPaper, "ReceiptPaper");
            #endregion
            _notifier = new SqlTableDependency<DeviceModel>(ConnectionString, TableName, mapper: mapper);
            _notifier.OnChanged += TableDependency_Changed;
            _notifier.Start();
        }
        public Task<List<DeviceModel>> GetDevices()
        {
            return _deviceService.GetDevices();
        }
        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC")
        {
            return _deviceService.GetDevices(skip, take, orderBy, direction);
        }
        public Task<int> GetTotalCount()
        {
            return _deviceService.GetTotalCount();
        }
        private void TableDependency_Changed(object sender, RecordChangedEventArgs<DeviceModel> e)
        {
            OnDeviceStatusChanged?.Invoke(this, new DeviceChangeEventArgs(e.Entity, e.EntityOldValues));
        }

        public void Dispose()
        {
            _notifier.Stop();
            _notifier.Dispose();
        }

        
    }

}
