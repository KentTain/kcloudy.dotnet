﻿using KC.Service.Base.ECharts.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.series
{
    public class MarkData:BasicData<MarkData>
    {
      
        public MarkData() : base()
        {

        }

        public MarkData(string name, MarkType type):this()
        {
            this.name = name;
            this.type = type;
        }

        public MarkData(MarkType type) : this()
        {
            this.type = type;
        }

        public MarkData(string name) : this()
        {
            this.name = name;
        }

       


    }
}
