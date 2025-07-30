using KC.Service.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.WeChat.Models
{
    public class RoleDetailViewModel
    {
        public RoleDetailViewModel()
        {
            Users = new List<UserDTO>();
            MenuNodes = new List<MenuNodeDTO>();
            Permissions = new List<PermissionDTO>();
        }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string DisplayName { get; set; }

        public bool IsSystemRole { get; set; }

        public virtual ICollection<UserDTO> Users { get; set; }

        public virtual ICollection<MenuNodeDTO> MenuNodes { get; set; }

        public virtual ICollection<PermissionDTO> Permissions { get; set; }
    }
}
