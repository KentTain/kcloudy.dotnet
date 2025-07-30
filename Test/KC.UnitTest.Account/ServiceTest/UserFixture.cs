using KC.DataAccess.Account;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KC.UnitTest.Account
{
    public class UserFixture : CommonFixture
    {
        /// <summary>
        /// 组织架构： </br>
        ///  ----企业（主管：齐总经理(角色：总经理)、员工：小齐(角色：总经理助理)）</br>
        ///      ----销售部 </br>
        ///          ----分管经理： 齐副经理（角色：销售经理、采购经理） </br>
        ///          ----主管：     肖经理(角色：销售经理)  </br>
        ///          ----员工：     小肖(角色：销售助理)） </br>
        ///      ----采购部 </br>
        ///          ----分管经理： 齐副经理（角色：销售经理、采购经理） </br>
        ///          ----主管：     蔡经理(角色：采购经理)、 </br>
        ///          ----员工：     小蔡(角色：采购助理)） </br>
        ///      ----人事部 </br>
        ///          ----主管：     任经理(角色：人事经理) </br>
        ///          ----员工：     小任(角色：人事助理)）
        ///      ----IT部 </br>
        ///          ----主管：     季经理(角色：系统管理员) </br>
        ///          ----员工：     小季(角色：系统管理员)）
        /// </summary>
        #region 组织架构及人员信息
        #region 企业：齐总经理、小齐
        //一级组织：企业
        public const int wfComDeptId = OrganizationConstants.企业_Id;
        public const string wfComDeptCode = OrganizationConstants.企业_Code;

        public const string wfComRoleId = RoleConstants.GmAssistantRoleId;
        public const string wfComManRoleId = RoleConstants.GeneralManagerRoleId;

        /// <summary>
        /// 齐总经理：总经理（企业）
        /// </summary>
        public const string wfComAdminUserId = "FD4A2460-A18B-4EA9-8933-597FAA02EA0C";
        public const string wfComAdminUserName = "齐总经理";
        public static List<string> wfComAdminUser_RoleIds = new List<string>() { wfComManRoleId };
        public static List<int> wfComAdminUser_OrgIds = new List<int>() { wfComDeptId };
        public static List<string> wfComAdminUser_OrgCodes = new List<string>() { wfComDeptCode };
        /// <summary>
        /// 齐副经理：分管经理（销售部、采购部）
        /// </summary>
        public const string wfComSubAdminUserId = "DD5E4F06-B3D2-4ADA-9EF5-35CC168C54D0";
        public const string wfComSubAdminUserName = "齐副经理";
        public static List<string> wfComSubAdminUser_RoleIds = new List<string>() { wfSaleManRoleId, wfBuyManRoleId };
        public static List<int> wfComSubAdminUser_OrgIds = new List<int>() { wfComDeptId, wfSaleDeptId, wfBuyDeptId };
        public static List<string> wfComSubAdminUser_OrgCodes = new List<string>() { wfComDeptCode, wfSaleDeptCode, wfBuyDeptCode };
        /// <summary>
        /// 小齐：总经理助理（企业）
        /// </summary>
        public const string wfComTestUserId = "37F7E998-A232-436B-BC57-3550AEE97ABB";
        public const string wfComTestUserName = "小齐";
        public static List<string> wfComTestUser_RoleIds = new List<string>() { wfComRoleId };
        public static List<int> wfComTestUser_OrgIds = new List<int>() { wfComDeptId };
        public static List<string> wfComTestUser_OrgCodes = new List<string>() { wfComDeptCode };
        #endregion

        #region 销售部：齐副经理、肖经理、小肖

        //二级级组织：销售部
        public const int wfSaleDeptId = OrganizationConstants.销售部_Id;
        public const string wfSaleDeptCode = OrganizationConstants.销售部_Code;

        public const string wfSaleRoleId = RoleConstants.SaleRoleId;
        public const string wfSaleManRoleId = RoleConstants.SaleManagerRoleId;

        /// <summary>
        /// 肖经理：销售经理（销售部）
        /// </summary>
        public const string wfSaleAdminUserId = "70072027-F68A-4729-9A36-A068A56A0173";
        public const string wfSaleAdminUserName = "肖经理";
        public static List<string> wfSaleAdminUser_RoleIds = new List<string>() { wfSaleManRoleId };
        public static List<int> wfSaleAdminUser_OrgIds = new List<int>() { wfSaleDeptId };
        public static List<string> wfSaleAdminUser_OrgCodes = new List<string>() { wfSaleDeptCode };
        /// <summary>
        /// 小肖：销售助理（销售部）
        /// </summary>
        public const string wfSaleTestUserId = "A3D0B891-2C04-4E85-81E5-9BEB7452D172";
        public const string wfSaleTestUserName = "小肖";
        public static List<string> wfSaleTestUser_RoleIds = new List<string>() { wfSaleRoleId };
        public static List<int> wfSaleTestUser_OrgIds = new List<int>() { wfSaleDeptId };
        public static List<string> wfSaleTestUser_OrgCodes = new List<string>() { wfSaleDeptCode };
        #endregion

        #region 采购部：齐副经理、蔡经理、小蔡
        //二级级组织：采购部
        public const int wfBuyDeptId = OrganizationConstants.采购部_Id;
        public const string wfBuyDeptCode = OrganizationConstants.采购部_Code;

        public const string wfBuyRoleId = RoleConstants.PurchaseRoleId;
        public const string wfBuyManRoleId = RoleConstants.PurchaseManagerRoleId;

        /// <summary>
        /// 蔡经理：采购经理（销售部）
        /// </summary>
        public const string wfBuyAdminUserId = "E393915D-E4B5-4A05-A84C-88ABFF58CE19";
        public const string wfBuyAdminUserName = "蔡经理";
        public static List<string> wfBuyAdminUser_RoleIds = new List<string>() { wfBuyManRoleId };
        public static List<int> wfBuyAdminUser_OrgIds = new List<int>() { wfBuyDeptId };
        public static List<string> wfBuyAdminUser_OrgCodes = new List<string>() { wfBuyDeptCode };
        /// <summary>
        /// 小蔡：采购助理（销售部）
        /// </summary>
        public const string wfBuyTestUserId = "0A9836BF-30A1-46E1-B27A-2E8AAC06D305";
        public const string wfBuyTestUserName = "小蔡";
        public static List<string> wfBuyTestUser_RoleIds = new List<string>() { wfBuyRoleId };
        public static List<int> wfBuyTestUser_OrgIds = new List<int>() { wfBuyDeptId };
        public static List<string> wfBuyTestUser_OrgCodes = new List<string>() { wfBuyDeptCode };
        #endregion

        #region 人事部：任经理、小任
        //二级级组织：IT部
        public const int wfHrDeptId = OrganizationConstants.人事部_Id;
        public const string wfHrDeptCode = OrganizationConstants.人事部_Code;

        public const string wfHrManRoleId = RoleConstants.HrManagerRoleId;
        public const string wfHrRoleId = RoleConstants.HrRoleId;

        /// <summary>
        /// 任经理：人事经理（人事部）
        /// </summary>
        public const string wfHrAdminUserId = "53DC1661-4392-4AF6-81D1-6C559F65921D";
        public const string wfHrAdminUserName = "任经理";
        public static List<string> wfHrAdminUser_RoleIds = new List<string>() { wfHrManRoleId };
        public static List<int> wfHrAdminUser_OrgIds = new List<int>() { wfHrDeptId };
        public static List<string> wfHrAdminUser_OrgCodes = new List<string>() { wfHrDeptCode };
        /// <summary>
        /// 小任：人事助理（人事部）
        /// </summary>
        public const string wfHrTestUserId = "D85A6760-A310-44EE-B6A3-3DB95EA22463";
        public const string wfHrTestUserName = "小季";
        public static List<string> wfHrTestUser_RoleIds = new List<string>() { wfHrManRoleId };
        public static List<int> wfHrTestUser_OrgIds = new List<int>() { wfHrDeptId };
        public static List<string> wfHrTestUser_OrgCodes = new List<string>() { wfHrRoleId };
        #endregion

        #region IT部：季经理、小季
        //二级级组织：IT部
        public const int wfItDeptId = OrganizationConstants.IT部_Id;
        public const string wfItDeptCode = OrganizationConstants.IT部_Code;

        public const string wfItManRoleId = RoleConstants.AdminRoleId;

        /// <summary>
        /// 季经理：系统管理员（IT部）
        /// </summary>
        public const string wfItAdminUserId = "7FD0C40E-4F2E-4FEB-922B-B7A4487DE875";
        public const string wfItAdminUserName = "季经理";
        public static List<string> wfItAdminUser_RoleIds = new List<string>() { wfItManRoleId };
        public static List<int> wfItAdminUser_OrgIds = new List<int>() { wfItDeptId };
        public static List<string> wfItAdminUser_OrgCodes = new List<string>() { wfItDeptCode };
        /// <summary>
        /// 小季：系统管理员（IT部）
        /// </summary>
        public const string wfItTestUserId = "59E88BB5-C1F0-4899-89B7-A32A0714B447";
        public const string wfItTestUserName = "小季";
        public static List<string> wfItTestUser_RoleIds = new List<string>() { wfItManRoleId };
        public static List<int> wfItTestUser_OrgIds = new List<int>() { wfItDeptId };
        public static List<string> wfItTestUser_OrgCodes = new List<string>() { wfItDeptCode };
        #endregion

        //审批人设置：组织/角色/用户设置
        public const string node3_ExecuteOrgCodes = wfComDeptCode + "," + wfSaleDeptCode;
        public const string node3_ExecuteOrgNames = "企业, 销售部";
        public const string node3_ExecuteRoleIds = wfComManRoleId + "," + wfSaleManRoleId;
        public const string node3_ExecuteRoleNames = "总经理, 销售经理";
        public const string node3_ExecuteUserIds = wfSaleAdminUserId;
        public const string node3_ExecuteUserNames = wfSaleAdminUserName;
        #endregion

        #region 初始化数据

        private List<User> GetAllUsers()
        {
            var allUsers = new List<User>() {
                // 企业：齐总经理、齐副经理、小齐
                new User()
                {
                    Id = wfComAdminUserId,
                    UserName = wfComAdminUserName,
                    DisplayName = wfComAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfComManRoleId,
                            UserId = wfComAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfComDeptId,
                            UserId = wfComAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfComSubAdminUserId,
                    UserName = wfComSubAdminUserName,
                    DisplayName = wfComSubAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfSaleManRoleId,
                            UserId = wfComSubAdminUserId,
                        },
                        new UsersInRoles()
                        {
                            RoleId = wfBuyManRoleId,
                            UserId = wfComSubAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfSaleDeptId,
                            UserId = wfComSubAdminUserId,
                        },
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfBuyDeptId,
                            UserId = wfComSubAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfComTestUserId,
                    UserName = wfComTestUserName,
                    DisplayName = wfComTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfComRoleId,
                            UserId = wfComTestUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfComDeptId,
                            UserId = wfComTestUserId,
                        }
                    },
                },
                // 采购部：蔡经理、小蔡
                new User()
                {
                    Id = wfItAdminUserId,
                    UserName = wfItAdminUserName,
                    DisplayName = wfItAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfItManRoleId,
                            UserId = wfItAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfItDeptId,
                            UserId = wfItAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfItTestUserId,
                    UserName = wfItTestUserName,
                    DisplayName = wfItTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfItManRoleId,
                            UserId = wfItTestUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfItDeptId,
                            UserId = wfItTestUserId,
                        }
                    },
                },
                // 销售部：肖经理、小肖
                new User()
                {
                    Id = wfSaleAdminUserId,
                    UserName = wfSaleAdminUserName,
                    DisplayName = wfSaleAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfSaleManRoleId,
                            UserId = wfSaleAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfSaleDeptId,
                            UserId = wfSaleAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfSaleTestUserId,
                    UserName = wfSaleTestUserName,
                    DisplayName = wfSaleTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfSaleRoleId,
                            UserId = wfSaleTestUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfSaleDeptId,
                            UserId = wfSaleTestUserId,
                        }
                    },
                },
                // 人事部：任经理、小任 
                new User()
                {
                    Id = wfHrAdminUserId,
                    UserName = wfHrAdminUserName,
                    DisplayName = wfHrAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfHrManRoleId,
                            UserId = wfHrAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfHrDeptId,
                            UserId = wfHrAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfHrTestUserId,
                    UserName = wfHrTestUserName,
                    DisplayName = wfHrTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfHrRoleId,
                            UserId = wfHrTestUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfHrDeptId,
                            UserId = wfHrTestUserId,
                        }
                    },
                },
                // IT部：季经理、小季
                new User()
                {
                    Id = wfBuyAdminUserId,
                    UserName = wfBuyAdminUserName,
                    DisplayName = wfBuyAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfBuyManRoleId,
                            UserId = wfBuyAdminUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfBuyDeptId,
                            UserId = wfBuyAdminUserId,
                        }
                    },
                },
                new User()
                {
                    Id = wfBuyTestUserId,
                    UserName = wfBuyTestUserName,
                    DisplayName = wfBuyTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoles = new List<UsersInRoles>(){
                        new UsersInRoles()
                        {
                            RoleId = wfBuyRoleId,
                            UserId = wfBuyTestUserId,
                        }
                    },
                    UserOrganizations = new List<UsersInOrganizations>(){
                        new UsersInOrganizations()
                        {
                            OrganizationId = wfBuyDeptId,
                            UserId = wfBuyTestUserId,
                        }
                    },
                }
            };

            return allUsers;
        }

        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return Base32.ToBase32(bytes);
        }
        internal static class Base32
        {
            private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            public static string ToBase32(byte[] input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                StringBuilder sb = new StringBuilder();
                for (int offset = 0; offset < input.Length;)
                {
                    byte a, b, c, d, e, f, g, h;
                    int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                    sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                    sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                    sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                    sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                    sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                    sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                    sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                    sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
                }

                return sb.ToString();
            }

            public static byte[] FromBase32(string input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }
                input = input.TrimEnd('=').ToUpperInvariant();
                if (input.Length == 0)
                {
                    return new byte[0];
                }

                var output = new byte[input.Length * 5 / 8];
                var bitIndex = 0;
                var inputIndex = 0;
                var outputBits = 0;
                var outputIndex = 0;
                while (outputIndex < output.Length)
                {
                    var byteIndex = _base32Chars.IndexOf(input[inputIndex]);
                    if (byteIndex < 0)
                    {
                        throw new FormatException();
                    }

                    var bits = Math.Min(5 - bitIndex, 8 - outputBits);
                    output[outputIndex] <<= bits;
                    output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

                    bitIndex += bits;
                    if (bitIndex >= 5)
                    {
                        inputIndex++;
                        bitIndex = 0;
                    }

                    outputBits += bits;
                    if (outputBits >= 8)
                    {
                        outputIndex++;
                        outputBits = 0;
                    }
                }
                return output;
            }

            // returns the number of bytes that were output
            private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
            {
                uint b1, b2, b3, b4, b5;

                int retVal;
                switch (offset - input.Length)
                {
                    case 1: retVal = 2; break;
                    case 2: retVal = 4; break;
                    case 3: retVal = 5; break;
                    case 4: retVal = 7; break;
                    default: retVal = 8; break;
                }

                b1 = (offset < input.Length) ? input[offset++] : 0U;
                b2 = (offset < input.Length) ? input[offset++] : 0U;
                b3 = (offset < input.Length) ? input[offset++] : 0U;
                b4 = (offset < input.Length) ? input[offset++] : 0U;
                b5 = (offset < input.Length) ? input[offset++] : 0U;

                a = (byte)(b1 >> 3);
                b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
                c = (byte)((b2 >> 1) & 0x1f);
                d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
                e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
                f = (byte)((b4 >> 2) & 0x1f);
                g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
                h = (byte)(b5 & 0x1f);

                return retVal;
            }
        }
        #endregion

        /// <summary>
        /// 是否开启删除初始化数据
        /// </summary>
        private const bool IsDeleteInitData = false;
        public override void SetUpData()
        {
            #region Identity注入至Serivces
            Services
                .AddMemoryCache()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<ComAccountContext>();
            Services.AddIdentity<User, Role>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = false;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = null;
            })
                .AddEntityFrameworkStores<ComAccountContext>()
                .AddDefaultTokenProviders();

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

            InjectTenant(TestTenant);

            #region AutoMapper对象注入
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Account.AutoMapper.AutoMapperConfiguration.GetAllProfiles());

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            Services.AddSingleton(config);
            var mapper = config.CreateMapper();
            Services.AddSingleton(mapper);

            #endregion

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Account.DependencyInjectUtil.InjectService(Services);

            #endregion

            _logger = LoggerFactory.CreateLogger(nameof(UserFixture));
            _serviceProvider = Services.BuildServiceProvider();

            #region 初始化Test用户及相关的角色
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                try
                {
                    var allUsers = GetAllUsers();
                    var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    foreach (var user in allUsers)
                    {
                        var success = userManager.CreateAsync(user, "123456").Result;
                        _logger.LogInformation("----Set up " + user.DisplayName + " is success? " + success);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                    if (ex.InnerException != null)
                        _logger.LogError(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                }

            }
            #endregion
        }

        /* 删除脚本：用户及组织、角色数据
        delete [cTest].[sys_UsersInOrganizations] where UserId in ('FD4A2460-A18B-4EA9-8933-597FAA02EA0C','37F7E998-A232-436B-BC57-3550AEE97ABB','DD5E4F06-B3D2-4ADA-9EF5-35CC168C54D0','53DC1661-4392-4AF6-81D1-6C559F65921D','70072027-F68A-4729-9A36-A068A56A0173','A3D0B891-2C04-4E85-81E5-9BEB7452D172','E393915D-E4B5-4A05-A84C-88ABFF58CE19','0A9836BF-30A1-46E1-B27A-2E8AAC06D305','59E88BB5-C1F0-4899-89B7-A32A0714B447','7FD0C40E-4F2E-4FEB-922B-B7A4487DE875')
        delete [cTest].[sys_UsersInRoles] where UserId in ('FD4A2460-A18B-4EA9-8933-597FAA02EA0C','37F7E998-A232-436B-BC57-3550AEE97ABB','DD5E4F06-B3D2-4ADA-9EF5-35CC168C54D0','53DC1661-4392-4AF6-81D1-6C559F65921D','70072027-F68A-4729-9A36-A068A56A0173','A3D0B891-2C04-4E85-81E5-9BEB7452D172','E393915D-E4B5-4A05-A84C-88ABFF58CE19','0A9836BF-30A1-46E1-B27A-2E8AAC06D305','59E88BB5-C1F0-4899-89B7-A32A0714B447','7FD0C40E-4F2E-4FEB-922B-B7A4487DE875')
        delete [cTest].[sys_User] where Id in ('FD4A2460-A18B-4EA9-8933-597FAA02EA0C','37F7E998-A232-436B-BC57-3550AEE97ABB','DD5E4F06-B3D2-4ADA-9EF5-35CC168C54D0','53DC1661-4392-4AF6-81D1-6C559F65921D','70072027-F68A-4729-9A36-A068A56A0173','A3D0B891-2C04-4E85-81E5-9BEB7452D172','E393915D-E4B5-4A05-A84C-88ABFF58CE19','0A9836BF-30A1-46E1-B27A-2E8AAC06D305','59E88BB5-C1F0-4899-89B7-A32A0714B447','7FD0C40E-4F2E-4FEB-922B-B7A4487DE875')
        GO

        SELECT * FROM [cTest].[sys_User]
        SELECT * FROM [cTest].[sys_UsersInOrganizations]
        SELECT * FROM [cTest].[sys_UsersInRoles]
        GO
        */
        public override void TearDownData()
        {
            try
            {
                if (IsDeleteInitData)
                {
                    using (var scope1 = _serviceProvider.CreateScope())
                    {
                        var allUsers = GetAllUsers();
                        var wfService = scope1.ServiceProvider.GetService<IAccountService>();
                        foreach (var user in allUsers)
                        {
                            var success = wfService.RemoveUserById(user.Id, RoleConstants.AdminUserId, RoleConstants.AdminUserName);
                            _logger.LogInformation("----Tear down " + user.DisplayName + " is success? " + success);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
    }
}