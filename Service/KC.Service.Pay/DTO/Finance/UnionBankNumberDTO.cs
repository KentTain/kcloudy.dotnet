using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.DTO.Pay
{
    public class UnionBankNumberDTO
    {
        public int Id { get; set; }

        public int BNKNBR { get; set; }

        public int CTYCOD { get; set; }

        public string WHLNAM { get; set; }

        public long BRDNBR { get; set; }
    }
}
