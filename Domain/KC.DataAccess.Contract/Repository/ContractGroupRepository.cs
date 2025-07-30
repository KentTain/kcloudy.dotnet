using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using KC.Database.EFRepository;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Model.Contract;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Contract.Repository
{
    public class ContractGroupRepository : EFRepositoryBase<ContractGroup>, IContractGroupRepository
    {
        public ContractGroupRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// 合同列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param> 
        /// <param name="contractStatu">合同状态</param>
        /// <param name="key">合同标题，编号，创建人</param>
        /// <param name="contracttype">合同类型</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<Tuple<int, List<ContractGroup>>> GetContractPageListIncludeAsync<K>(int pageIndex, int pageSize, Expression<Func<ContractGroup, bool>> predicate, Expression<Func<ContractGroup, K>> keySelector, bool ascending, params Expression<Func<ContractGroup, object>>[] propertySelectors)
        {
            IQueryable<ContractGroup> query = EFContext.Set<ContractGroup>();
            if (propertySelectors.Any())
            {
                foreach (Expression<Func<ContractGroup, object>> propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }
            var tempData = query.Where(predicate).AsNoTracking();
            int recordCount = tempData.Count();
            var res = ascending ? await tempData.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync() : await tempData.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<int, List<ContractGroup>>(recordCount, res);
        }

        public async Task<Tuple<int, List<ContractGroup>>> GetContractsByFilterAsync<K>(int pageIndex, int pageSize, Expression<Func<ContractGroup, bool>> predicate, Expression<Func<ContractGroup, K>> keySelector, bool ascending, params Expression<Func<ContractGroup, object>>[] propertySelectors)
        {
            IQueryable<ContractGroup> query = EFContext.Set<ContractGroup>();
            if (propertySelectors.Any())
            {
                foreach (Expression<Func<ContractGroup, object>> propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                    query = query.Include(m => m.ContractGroupOperationLog);
                }
            }
            var tempData = query.Where(predicate).AsNoTracking();
            int recordCount = tempData.Count();
            var res = ascending ? await tempData.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync() : await tempData.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<int, List<ContractGroup>>(recordCount, res);
        }

        /// <summary>
        /// 合同列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param> 
        /// <param name="contractStatu">合同状态</param>
        /// <param name="key">合同标题，编号，创建人</param>
        /// <param name="contracttype">合同类型</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Tuple<int, List<ContractGroup>> GetSignContractGroupList(int page, int rows, DateTime? startTime, DateTime? endTime, string bannedcontract, string key, ContractStatus? contractStatu, ContractType? contracttype, CustomerType? customerType, string currentUserId, string currentTenantName, string currentDispayName)
        {
            Expression<Func<ContractGroup, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(key))
            {
                predicate = predicate.And(m => m.ContractTitle.Contains(key) || m.ContractNo.Contains(key) || m.UserName.Contains(key) || m.RelationData == key);
            }
            if (contracttype.HasValue)
            {
                predicate = predicate.And(m => m.Type == contracttype.Value);
            }
            if (contractStatu.HasValue)
            {
                predicate = predicate.And(m => m.Statu == contractStatu.Value);
            }
            if (!string.IsNullOrEmpty(bannedcontract))
            {
                var inttype = bannedcontract.Split(',');
                foreach (var item in inttype)
                {
                    var itemi = int.Parse(item);
                    predicate = predicate.And(m => m.Type != (ContractType)itemi);
                }
            }
            if (customerType.HasValue && customerType.Value == 0)
            {
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    predicate = predicate.And(m => m.UserContract.Any(c => c.CustomerType == customerType.Value && c.StaffId== currentUserId));
                }
            }
            else
            {
                predicate = predicate.And(m => m.UserContract.Any(c => c.UserId == currentTenantName && c.UserName == currentDispayName));
            }
            if (startTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startTime.Value);
            }
            if (endTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate <= endTime.Value);
            }
            var query = EFContext.Set<ContractGroup>().Include(m => m.UserContract).Where(predicate).AsNoTracking();
            int recordCount = query.Count();
            return new Tuple<int, List<ContractGroup>>(recordCount, query.OrderByDescending(m => m.CreatedDate).Skip((page - 1) * rows).Take(rows).ToList());
        }



        public Tuple<int, List<ContractGroup>> GetSignContractGroupList(int pageIndex, int pageSize, ContractType? type, string txtsearch)
        {
            Expression<Func<ContractGroup, bool>> predicate = m => !m.IsDeleted && m.Statu == ContractStatus.Complete && m.UserContract.Any(c => c.Statu == UserContractStatus.Sign);
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.Type == type.Value);
            }
            if (!string.IsNullOrEmpty(txtsearch))
            {
                predicate = predicate.And(m => m.ContractTitle.Contains(txtsearch) || m.ContractNo.Contains(txtsearch) || m.UserName.Contains(txtsearch) || m.RelationData == txtsearch);
            }
            var query = EFContext.Set<ContractGroup>().Include(m => m.UserContract).Where(predicate).AsNoTracking();
            int recordCount = query.Count();
            return new Tuple<int, List<ContractGroup>>(recordCount, query.OrderByDescending(m => m.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// 我的合同日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="statu"></param>
        /// <param name="operatorId"></param>
        /// <param name="remark"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public Tuple<int, List<ContractGroupOperationLog>> GetMyContractGroupLog(int pageIndex = 1, int pageSize = 10, int logType = -1, string remark = "", string key = "")
        {
            Expression<Func<ContractGroupOperationLog, bool>> predicate = m => true;
            if (logType >= 0)
            {
                predicate = predicate.And(m => m.Type == (ProcessLogType)logType);
            }
            if (!string.IsNullOrEmpty(remark))
            {
                predicate = predicate.And(m => m.Remark.Contains(remark));
            }
            if (!string.IsNullOrEmpty(key))
            {
                predicate = predicate.And(m => m.ContractGroup.ContractNo.Contains(key) || m.ContractGroup.ContractTitle.Contains(key));
            }
            var query = EFContext.Set<ContractGroupOperationLog>().Include(m => m.ContractGroup).Where(predicate).AsNoTracking();
            int recordCount = query.Count();
            return new Tuple<int, List<ContractGroupOperationLog>>(recordCount, query.OrderByDescending(m => m.OperateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }


        public ContractGroup GetContractGroup(Guid id, string contractTitle = "")
        {
            if (string.IsNullOrEmpty(contractTitle))
            {

                return
                    Entities.Include(m => m.UserContract).AsNoTracking().FirstOrDefault(m => !m.IsDeleted && m.Id == id);
            }
            return
                   Entities.Include(m => m.UserContract).AsNoTracking().OrderByDescending(m => m.CreatedDate).FirstOrDefault(m => !m.IsDeleted && m.ContractTitle.Contains(contractTitle));
        }

        public ContractGroup GetContract(string id)
        {
            return Entities.Include(m => m.UserContract).FirstOrDefault(m => !m.IsDeleted && m.ContractNo == id);
        }

        public ContractGroup GetContractIncludeUserContract(Expression<Func<ContractGroup, bool>> predicate)
        {
            return Entities.Include(m => m.UserContract).AsNoTracking().FirstOrDefault(predicate);
        }
        public ContractGroup GetContractGroupById(Guid id)
        {
            return
                    EFContext.Context.Set<ContractGroup>()
                        .Include(m => m.UserContract)
                        .FirstOrDefault(m => !m.IsDeleted && m.Id == id);
        }

        //public ContractGroupTotal GetcontractTotal()
        //{
        //    var result = UnitOfWork.Context.Database.SqlQuery<ContractGroupTotal>("PROC_QueryContractTotal");
        //    return result.FirstOrDefault();
        //}

        public ContractGroup GetContractGroupByBlobId(string blobId)
        {
            return Entities.FirstOrDefault(m => !m.IsDeleted && m.Statu == ContractStatus.Complete && m.BlobId == blobId);
        }
        public ContractGroup GetContractByBlobId(string blobId)
        {
            return Entities.Include(m => m.UserContract).FirstOrDefault(m => !m.IsDeleted && m.BlobId == blobId);
        }
        public ContractGroup GetContractByRelationData(string relationData)
        {
            return Entities.Include(m => m.UserContract).FirstOrDefault(m => !m.IsDeleted && m.RelationData == relationData);
        }
    }
}
