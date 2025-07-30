using KC.Service.Base.ECharts.feature;
using KC.Service.Base.ECharts.style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts
{
    public class DataRange:Basic<DataRange>
    {
        public new int? itemGap { get; set; }

        public new int? itemHeight { get; set; }

        public new int? min { get; set; }

        public new int? max { get; set; }

        public int? precision { get; set; }

        public IList<Split> splitList { get; set; }

        public feature.Range range { get; set; }

        public object selectedMode { get; set; }

        public bool? calculable { get; set; }

        public bool? hoverLink { get; set; }

        public bool? realtime { get; set; }

        public IList<string> color { get; set; }

        public object formatter { get; set; }

        public IList<string> text { get; set; }

        public TextStyle textStyle { get; set; }

        public DataRange ItemGap(int itemGap)
        {
            this.itemGap = itemGap;
            return this;
        }

    }
}
