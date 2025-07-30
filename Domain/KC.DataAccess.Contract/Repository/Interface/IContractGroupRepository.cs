using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Model.Contract;

namespace KC.DataAccess.Contract.Repository
{
    public interface IContractGroupRepository : Database.IRepository.IDbRepository<ContractGroup>
    {
        ContractGroup GetContract(string id);
        ContractGroup GetContractByBlobId(string blobId);
        ContractGroup GetContractByRelationData(string relationData);
        ContractGroup GetContractGroup(Guid id, string contractTitle = "");
        ContractGroup GetContractGroupByBlobId(string blobId);
        ContractGroup GetContractGroupById(Guid id);
        ContractGroup GetContractIncludeUserContract(Expression<Func<ContractGroup, bool>> predicate);
        Task<Tuple<int, List<ContractGroup>>> GetContractPageListIncludeAsync<K>(int pageIndex, int pageSize, Expression<Func<ContractGroup, bool>> predicate, Expression<Func<ContractGroup, K>> keySelector, bool ascending, params Expression<Func<ContractGroup, object>>[] propertySelectors);
        Task<Tuple<int, List<ContractGroup>>> GetContractsByFilterAsync<K>(int pageIndex, int pageSize, Expression<Func<ContractGroup, bool>> predicate, Expression<Func<ContractGroup, K>> keySelector, bool ascending, params Expression<Func<ContractGroup, object>>[] propertySelectors);
        Tuple<int, List<ContractGroupOperationLog>> GetMyContractGroupLog(int pageIndex = 1, int pageSize = 10, int logType = -1, string remark = "", string key = "");
        Tuple<int, List<ContractGroup>> GetSignContractGroupList(int pageIndex, int pageSize, ContractType? type, string txtsearch);
        Tuple<int, List<ContractGroup>> GetSignContractGroupList(int page, int rows, DateTime? startTime, DateTime? endTime, string bannedcontract, string key, ContractStatus? contractStatu, ContractType? contracttype, CustomerType? customerType, string currentUserId, string currentTenantName, string currentDispayName);
    }
}