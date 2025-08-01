﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.axis
{
    public class CategoryAxis : ChartAxis<CategoryAxis>
    {
        public bool? boundaryGap { get; set; }
      
        public CategoryAxis()
        {
            type = AxisType.category;
        }

        public CategoryAxis BoundaryGap(bool boundaryGap)
        {
            this.boundaryGap = boundaryGap;
            return this;
        }

    }
}
