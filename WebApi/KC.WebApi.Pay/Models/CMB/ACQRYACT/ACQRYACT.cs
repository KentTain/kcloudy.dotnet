using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class ACQRYACT
    {
        public ACQRYACT()
        {
            CCYNBR = "10";
            STSCOD = "0";
        }
        public string CCYNBR { get; set; }

        public string STSCOD { get; set; }
    }
}