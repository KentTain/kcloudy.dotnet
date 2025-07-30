using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class WithdrawalsDTO 
    {
        public WithdrawalsDTO()
        {
            BankList=new List<BankAccountDTO>();
        }

        //T0可提取余额
        public decimal T0AvailableBalance { get; set; }
        //T1可提取余额
        public decimal T1AvailbaleBalance { get; set; }

        public decimal AvailableBalance { get; set; }

        public List<BankAccountDTO> BankList { get; set; }

        public List<WithdrawRuleDTO> Rule { get; set; }

        public decimal TodayMax { get; set; }
    }
}
