using DeviceMonitorApp.Models;
using System;

namespace DeviceMonitorApp.Code.Notifications
{
    public class DeviceChangeEventArgs : EventArgs
    {
        public DeviceModel NewDevice { get; }
        public DeviceModel OldDevice { get; }

        public DeviceChangeEventArgs(DeviceModel newDdevice, DeviceModel oldDevice)
        {
            NewDevice = newDdevice;
            OldDevice = oldDevice;
        }
    }

}
