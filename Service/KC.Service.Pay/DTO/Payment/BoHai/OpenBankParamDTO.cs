using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay.BoHai
{
    public class OpenBankParamDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 绑定银行卡Id
        /// </summary>
        //[DataMember]
        //public int BankId { get; set; }


        /// <summary>
        /// 渤海银行账号
        /// </summary>
        [DataMember]
        public BoHaiBankAccountDTO BoHaiBankAccountDTO { get; set; }
    }
}
