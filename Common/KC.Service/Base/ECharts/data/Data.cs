﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.data
{
    public class Data : BasicData<Data>
    {
        public string extra { get; set; }

        public Data Extra(string extra)
        {
            this.extra = extra;
            return this;
        }
    }
}
