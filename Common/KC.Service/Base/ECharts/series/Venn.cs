using KC.Service.Base.ECharts.style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.series
{
    public class Venn : ChartSeries<Venn>
    {

        public Venn()
        {
            this.type = ChartType.venn;
        }

        public Venn(string name) : this() {
            this.name = name;
        }

    }
}
