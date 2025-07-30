
using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO.Search;

namespace KC.Service.Account
{
    public interface IAccountService : IEFService
    {
        #region User
        Task<bool> ExistUserEmailAsync(string eamil);
        Task<bool> ExistUserPhoneAsync(string phone);
        Task<bool> ExistUserNameAsync(string userName);

        List<UserSimpleDTO> FindAllUsersWithRolesAndOrgs();
        List<UserSimpleDTO> FindUsersByUserIds(List<string> userIds);
        List<UserSimpleDTO> FindUsersByRoleIds(List<string> roleIds);
        List<UserSimpleDTO> FindUsersByOrgIds(List<int> orgIds);
        List<UserSimpleDTO> FindUserManagersByUserId(string userId);
        List<UserSimpleDTO> FindUserSupervisorsByUserId(string userId);
        Task<List<UserSimpleDTO>> FindUsersByDataPermissionFilterAsync(DataPermissionSearchDTO searchModel);


        PaginatedBaseDTO<UserDTO> FindPaginatedUsersByOrgIds(int pageIndex, int pageSize, string email, string phone, string name, WorkflowBusStatus? status, PositionLevel? positionLevel, List<int> orgIds);

        Task<UserSimpleDTO> GetSimpleUserByIdAsync(string userId);
        Task<UserSimpleDTO> GetSimpleUserWithOrgsAndRolesByUserIdAsync(string userId);
        Task<UserDTO> GetUserWithOrgsAndRolesByUserIdAsync(string userId);
        
        Task<UserContactInfoDTO> GetUserContactInfoByIdAsync(string userId);
        
        Task<IdentityResult> UserRegister(UserRegisterDTO data);
        Task<IdentityResult> CreateUser(UserDTO data, string operatorId, string operatorName, string[] organizations, string gussourl);
        bool UpdateUser(UserDTO data, string operatorId, string operatorName, string[] organizations);
         
        bool UpdateRoleInUser(string userId, List<string> addList, string operatorId, string operatorName);
        bool UpdateOrganizationInUser(string userId, List<int> addList, string operatorId, string operatorName);

        bool RemoveUserById(string id, string operatorId, string operatorName);
        bool ReactUserById(string userId, string operatorId, string operatorName);

        #endregion

        #region DownLoad User Excel && Import User data from Excel
        bool ImportUserDataFromExcel(Stream excelData);

        #endregion
        
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<IdentityResult> ChangeAdminRawInfoAsync(string newPassword, string adminEmail, string adminPhone);
        Task<IdentityResult> ChangeMailPhoneAsync(string userId, string email, string phone);
        Task<IdentityResult> AuditUserStatus(string userId, WorkflowBusStatus status);
    }
}
