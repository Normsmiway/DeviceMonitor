using DeviceMonitorApp.Code;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace DeviceMonitorApp.Models
{
    public class DeviceModel : IDisposable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? CashLevel { get; set; } = 0;
        public string NetworkStatus { get; set; }
        public int CardReader { get; set; }
        public int CashJam { get; set; }
        public int Cassette { get; set; }
        public int ReceiptPaper { get; set; }
        public int ReceiptPrinter { get; set; }

        public DeviceModel()
        {
            Sanitize();
        }

        private void Sanitize()
        {
            CashLevel = MathHelper.Clamp(CashLevel.Value, 0, 100);
            //  CashJam = MathHelper.Clamp(CashJam, 0, 1);
            //if (CashLevel == 0) { Status = DeviceStatus.Down; }
            //if (CashJam == 0) { Status = DeviceStatus.Down; }
        }
        public void Update(DeviceModel newDevice)
        {
            Name = newDevice.Name;
            CashLevel = newDevice.CashLevel;
            Status = newDevice.Status;
            NetworkStatus = newDevice.NetworkStatus;
            CardReader = newDevice.CardReader;
            CashJam = newDevice.CashJam;
            Cassette = newDevice.Cassette;
            ReceiptPaper = newDevice.ReceiptPaper;
            ReceiptPrinter = newDevice.ReceiptPrinter;
            Sanitize();
        }

        public void Dispose()
        {

        }
    }

    public class Stats
    {
        public decimal UpCount { get; set; }
        public decimal DownCount { get; set; }
        public decimal OtherCount { get; set; }
        public decimal TotalCount { get; set; }

        public decimal Performace
        {

            get
            {
                if (TotalCount == 0) { TotalCount = 1; }
                return Math.Round((UpCount / TotalCount) * 100, 2);
            }

        }

        public decimal NegPerformace
        {

            get
            {
                if (TotalCount == 0) { TotalCount = 1; }
                return Math.Round((DownCount / TotalCount) * 100, 2);
            }

        }

        public decimal OtherPerformace
        {
            get
            {
                if (TotalCount == 0) { TotalCount = 1; }
                return Math.Round((OtherCount / TotalCount) * 100, 2);
            }

        }
    }
    public class MathHelper
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable
        {
            // todo - implementation
            T output = value;
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            return output;
        }
    }
}
