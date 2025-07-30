using KC.Service.Base.ECharts.style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.feature
{
    public class Mark
    {
        public bool? show { get; set; }

        public MarkTitle title { get; set; }

        public Mark Show(bool show)
        {
            this.show = show;
            return this;
        }

        public MarkTitle Title()
        {
            if (title == null)
                title = new MarkTitle();
            return title;
        }

        public LineStyle lineStyle { get; set; }

        public LineStyle LineStyle()
        {
            if (lineStyle == null)
                this.lineStyle = lineStyle;
            return lineStyle;
        }
    }
}
