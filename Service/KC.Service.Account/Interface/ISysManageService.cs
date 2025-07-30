using KC.Framework.Tenant;
using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KC.Service.Base.ECharts;
using KC.Service.DTO.Search;

namespace KC.Service.Account
{
    public interface ISysManageService : IEFService
    {
        #region Organization

        List<OrganizationDTO> FindAllOrgTrees();
        List<OrganizationDTO> FindOrgTreesWithUsers();
        List<OrganizationDTO> FindOrgTreesByIds(List<int> orgIds, string name);
        List<OrganizationDTO> FindOrgTreesByName(string name);
        List<OrganizationDTO> FindOrgTreesWithUsersByRoleIds(List<string> roleIds);
        List<OrganizationDTO> FindOrgTreesWithUsersByOrgIds(List<int> orgIds, List<int> exceptOrgIds);

        List<OrganizationDTO> FindAllOrganization();
        List<OrganizationDTO> FindOrganizationsWithUsersByUserId(string userId);
        List<OrganizationDTO> FindHigherOrganizationsWithUsersByUserId(string userId);

        OrganizationDTO GetOrganizationById(int id);
        OrganizationDTO GetOrganizationsWithUsersByOrgId(int orgId);

        List<int> FindOrganizationsByIdList(List<int> ids);

        bool ExistOrganizationName(int pid, string name);
        bool SaveOrganization(OrganizationDTO data, string currentUserId);
        bool RemoveOrganization(int id);

        #endregion

        #region 用户跟踪日志

        PaginatedBaseDTO<UserTracingLogDTO> FindPaginatedUserTracingLogs(int pageIndex, int pageSize, string name, string order);


        #endregion

        #region 用户登陆日志

        PaginatedBaseDTO<UserLoginLogDTO> FindPaginatedUserLoginLogs(int pageIndex, int pageSize, string name);
        Task<ChartOption> GetUserLoginReportData(DateTime? startDate, DateTime? endDate);
        Task<bool> AddUserLoginLogAsync(UserLoginLogDTO model);
        #endregion

        #region 测试TreeNode基类
        List<OrganizationDTO> Test_GetAllTreeNodeWithNestChild();
        OrganizationDTO Test_GetTreeNodeWithNestChildById(int id);
        List<OrganizationDTO> Test_GetTreeNodesWithNestParentAndChildById(int id);
        List<OrganizationDTO> Test_GetTreeNodesWithNestParentAndChildByFilter(string name);
        bool Test_UpdateExtendFields();
        #endregion

        //SeedEntity GetSeedByType(SeedType seedType, int step = 1);
    }
}
