using DeviceMonitorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceMonitorApp.Code
{

    public interface IDeviceMockService
    {
        public IAsyncEnumerable<List<DeviceModel>> GetDevices();
        public IAsyncEnumerable<DeviceModel> GetDevice();
        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC");
        public Task<int> GetTotalCount();
    }
    public class MockDeviceService : IDeviceMockService
    {
        private static readonly string[] Statuses = new[]
        {
            "up","down","supervising"
        };
        private static readonly string[] Names = new[]
       {
            "Fst ATM", "Bracing ATM", "Chilly ATM", "Cool ATM", "Mild ATM", "Warm ATM", "Balmy ATM", "Hot ATM", "Sweltering ATM", "Scorching ATM",
            "Ger ATM", "Sey ATM", "Fat ATM", "Cool ATM", "Bridge ATM", "Dat ATM", "Uyi ATM", "Salama ATM", "Boo ATM", "General ATM"
        };
        private static readonly int[] CashJams = new[] { 0, 1 };
    
        //public Task<List<DeviceModel>> GetDevices()
        //{
        //    var rng = new Random();
        //    var result = Task.FromResult(Enumerable.Range(1, 100)
        //        .Select(device => new DeviceModel
        //        {
        //            Name = Names[rng.Next(Names.Length)],
        //            Status = Statuses[rng.Next(Statuses.Length)],
        //            CashLevel = rng.Next(1, 100),
        //            CashJam = rng.Next(0, 1)
        //        }).ToList());
        //    return result;
        //}

        public async IAsyncEnumerable<List<DeviceModel>> GetDevices()
        {
            var rng = new Random();
            for (int i = 0; i < 10; i++)
            {
                _ = Task.Delay(1000);
                var result = await Task.FromResult(Enumerable.Range(1, 4)
                    .Select(device => new DeviceModel
                    {
                        Name = Names[rng.Next(Names.Length)],
                        Status = Statuses[rng.Next(Statuses.Length)],
                        CashLevel = rng.Next(1, 100),
                        CashJam = CashJams[rng.Next(CashJams.Length)]
                    }).ToList());

                yield return result;
            }
        }

        public async IAsyncEnumerable<DeviceModel> GetDevice()
        {
            var rng = new Random();
            for (int i = 0; i < 100; i++)
            {
                yield return await CreateDevice(rng);

            }
        }

        private static async Task<DeviceModel> CreateDevice(Random rng)
        {
            await Task.Delay(rng.Next(250, 1000));
            return new DeviceModel
            {
                Name = Names[rng.Next(Names.Length)],
                Status = Statuses[rng.Next(Statuses.Length)],
                CashLevel = rng.Next(1, 100),
                CashJam = CashJams[rng.Next(CashJams.Length)]
            };
        }

        public Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC")
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalCount()
        {
            throw new NotImplementedException();
        }
    }
}
