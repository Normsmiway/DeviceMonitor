using Dapper;
using DeviceMonitorApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceMonitorApp.Code
{

    public class DeviceService : IDeviceService
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;
        private readonly string query;
        private string queryPaginated;
        private readonly string TableName;
        private string recordCountQuery;
        private readonly string archive;
        private readonly string archiveDateColumn;

        public DeviceService(IConfiguration config)
        {
            _config = config;
            connectionString = _config["ConnectionStrings:Default"] ?? throw new ArgumentNullException(nameof(connectionString));
            query = _config["Queries:GetDevices"] ?? throw new ArgumentNullException(nameof(query));
            queryPaginated = _config["Queries:GetDevicesPagineted"] ?? throw new ArgumentNullException(nameof(query));
            recordCountQuery = _config["Queries:RecordCount"] ?? throw new ArgumentNullException(nameof(query));
            TableName = _config["AppSettings:TableName"] ?? throw new ArgumentNullException(nameof(TableName));
            archive = _config["AppSettings:Archive"] ?? throw new ArgumentNullException(nameof(archive));
            archiveDateColumn = _config["AppSettings:ArchiveDateColumn"] ?? throw new ArgumentNullException(nameof(archiveDateColumn));
            query = string.Format(query, TableName);
            //recordCountQuery = string.Format(recordCountQuery, TableName);
            // $"select * from [Article] ORDER BY {orderBy} {direction} OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY; "
        }


        public async Task<List<DeviceModel>> GetDevices()
        {
            using SqlConnection con = new SqlConnection(connectionString);
            var results = await con.QueryAsync<dynamic>(query);
            return results.Select(d => new DeviceModel()
            {
                Id = d.activityid,
                Name = d.keys,
                Status = d.status,
                CashLevel = d.cashlevel,
                CashJam = d.CashJam
            }).ToList();

            #region
            //var result = devices.Select(device => new Device()
            //{
            //    ActivityId = device.activityid,
            //    CardReader = device.CardReader,
            //    CashJam = device.CashJam,
            //    CashLevel = device.CashLevel,
            //    Cassette = device.Cassette,
            //    Keys = device.keys,
            //    NetworkStatus = device.networkstatus,
            //    ReceiptPaper = device.ReceiptPaper,
            //    ReceiptPrinter = device.ReceiptPrinter,
            //    Status = device.status
            //});
            //return result;
            #endregion
        }
        public async Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC")
        {
            // queryPaginated = string.Format(queryPaginated, TableName, orderBy, direction,take , skip);
            queryPaginated = $"SELECT * FROM {TableName} ORDER BY {orderBy} {direction} OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
            using SqlConnection con = new SqlConnection(connectionString);
            return await QueryListAsync(con, query);
        }
        public async Task<int> GetTotalCount()
        {
            recordCountQuery = string.Format(recordCountQuery, TableName);
            using SqlConnection con = new SqlConnection(connectionString);
            var res = await con.QueryFirstAsync<int>(recordCountQuery);

            return res;
        }

        public async Task<List<string>> GetDeviceNames(string coulmnName = "keys")
        {
            string query = $"SELECT DISTINCT {coulmnName} FROM {TableName} ";
            using SqlConnection con = new SqlConnection(connectionString);
            var res = await con.QueryAsync<string>(query);

            return res.ToList();
        }
        public async Task<int> GetArchivesTotalCount()
        {
            recordCountQuery = string.Format(recordCountQuery, archive);
            using SqlConnection con = new SqlConnection(connectionString);
            var res = await con.QueryFirstAsync<int>(recordCountQuery);

            return res;
        }
        public async Task<List<DeviceModel>> GetDevices(int skip, int take, string orderBy, string direction = "ASC", DateTimeOffset? startDate = default, DateTimeOffset? endDate = default)
        {
            string query = $"SELECT * FROM {archive}  WHERE {archiveDateColumn} BETWEEN '{startDate.Value.DateTime}' and '{endDate.Value.DateTime}' ORDER BY {orderBy} {direction} OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
            // SELECT* FROM[DsphereATM].[dbo].[atm_data_Archive] WHERE datetime_status BETWEEN '2019-05-22 12:00:00.283' and '2019-05-22 12:25:21.390' ORDER BY datetime_status Asc OFFSET 2 ROWS FETCH NEXT 30 ROWS ONLY
            using SqlConnection con = new SqlConnection(connectionString);
            return await QueryListAsync(con, query);
        }

        public async Task<Stats> GetDeviceStats(string deviceName, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            string query = QueryBuilder.Build(deviceName, archiveDateColumn, startDate.Value.DateTime, endDate.Value.DateTime,
                DeviceStatus.Up, DeviceStatus.Down, archive, "keys", "[status]");
            using SqlConnection con = new SqlConnection(connectionString);
            var results = await con.QueryFirstAsync<Stats>(query);
            return results;

            #region
            //.Select(d => new Stats()
            //{
            //    DownCount = d.DownCount,
            //    UpCount = d.UpCount,
            //    TotalCount = d.TotalCount,
            //    OtherCount = d.OtherCount
            //}).FirstOrDefault();
            #endregion

        }
        private async Task<List<DeviceModel>> QueryListAsync(SqlConnection con, string query)
        {
            var results = await con.QueryAsync<dynamic>(query);
            return results.Select(d => new DeviceModel()
            {
                Id = d.activityid,
                Name = d.keys,
                Status = d.status,
                CashLevel = d.cashlevel,
                CashJam = d.CashJam
            }).ToList();
        }


    }

    public class QueryBuilder
    {


        public static string Build(string name, string dateColumn, DateTime startDate, DateTime endDate, string upStatusValue = "up", string downStatusValue = "down", string tableName = "atm_data_Archive", string nameColumn = "keys", string statusColumn = "[status]")
        {
            string clause = $"and {dateColumn} BETWEEN '{startDate}' and '{endDate}'";
            string nameStatusClause = $"from {tableName} where {nameColumn} = '{name}' and {statusColumn} ";
            string query = $"SELECT" +
         $"(SELECT COUNT(*)  {nameStatusClause} = '{upStatusValue}'  {clause}) AS UpCount," +
         $"(SELECT COUNT(*)  {nameStatusClause} = '{downStatusValue}'  {clause}) AS DownCount," +
         $"(SELECT COUNT(*)  from {tableName} where {nameColumn}='{name}' and {statusColumn}!='{downStatusValue}' and [status]!='{upStatusValue}'  {clause}) AS OtherCount," +
         $"(SELECT COUNT(*)  from {tableName} where {nameColumn}='{name}' {clause}) AS TotalCount";
            return query;
        }

    }
}
