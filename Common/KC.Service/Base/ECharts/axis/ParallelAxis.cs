using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.axis
{
    public class ParallelAxis:ChartAxis<ParallelAxis>
    {
        public int? dim { get; set; }


        public ParallelAxis Dim(int dim)
        {
            this.dim = dim;
            return this;
        }

    }
}
