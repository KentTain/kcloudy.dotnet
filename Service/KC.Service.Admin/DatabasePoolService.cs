using AutoMapper;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Model.Admin;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Logging;

namespace KC.Service.Admin
{
    public class DatabasePoolService : EFServiceBase, IDatabasePoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<DatabasePool> _databasePoolRepository;

        public DatabasePoolService(
            IMapper mapper,
            IDbRepository<DatabasePool> databasePoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<DatabasePoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _databasePoolRepository = databasePoolRepository;
        }

        public List<DatabasePoolDTO> FindAllDatabasePools()
        {
            var data = _databasePoolRepository.FindAll().ToList();
            return _mapper.Map<List<DatabasePoolDTO>>(data);
        }

        public PaginatedBaseDTO<DatabasePoolDTO> FindDatabasePoolsByFilter(int pageIndex, int pageSize,
            string server, string database, string userName)
        {
            Expression<Func<DatabasePool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(server))
            {
                predicate = predicate.And(m => m.Server.Contains(server));
            }
            if (!string.IsNullOrWhiteSpace(database))
            {
                predicate = predicate.And(m => m.Database.Contains(database));
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                predicate = predicate.And(m => m.UserName.Contains(userName));
            }

            var data = _databasePoolRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.DatabasePoolId, false);

            var total = data.Item1;
            var rows = _mapper.Map<List<DatabasePoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<DatabasePoolDTO>(pageIndex, pageSize, total, rows);
        }

        public DatabasePoolDTO GetDatabasePoolById(int id)
        {
            var data = _databasePoolRepository.GetById(id);
            return _mapper.Map<DatabasePoolDTO>(data);
        }

        public bool SaveDatabasePool(DatabasePoolDTO model)
        {
            //var uPH = EncryptPasswordUtil.EncryptPassword(model.UserPasswordHash);
            //model.UserPasswordHash = string.Empty;
            //model.UserPasswordHash = uPH;

            var data = _mapper.Map<DatabasePool>(model);
            if (data.DatabasePoolId == 0)
            {
                return _databasePoolRepository.Add(data);
            }
            else
            {
                return _databasePoolRepository.Modify(data, true);
            }
        }

        public bool RemoveDatabasePool(int id)
        {
            return _databasePoolRepository.SoftRemoveById(id);
        }

        public string TestDatabaseConnection(DatabasePoolDTO model, string privateKey = null)
        {
            var connectionString = model.GetDatabaseConnectionString(privateKey);
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Closed
                        || connection.State == ConnectionState.Broken)
                    {
                        //Connection   is   not   available  
                        return "Connection is not available";
                    }
                    else
                    {
                        //Connection   is   available  
                        return string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format(
                        "连接数据库[Connection String：{0}]出错; " + Environment.NewLine +
                        "错误消息：{1}; " + Environment.NewLine +
                        "错误详情：{2}",
                        connectionString, ex.Message, ex.StackTrace);
                    Logger.LogError(errorMsg);
                    return errorMsg;
                }
                finally
                {
                    //Close DataBase
                    //关闭数据库连接
                    connection.Close();
                }
            }
        }
    }
}
