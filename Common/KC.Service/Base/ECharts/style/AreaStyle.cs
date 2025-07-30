using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.style
{
    public class AreaStyle:Style<AreaStyle>
    {
        public new object color { get; set; }

        public AreaStyleType? type { get; set; }

        public new AreaStyle Color(object color)
        {
            this.color = color;
            return this;
        }

        public AreaStyle Type(AreaStyleType type)
        {
            this.type = type;
            return this;
        } 
    }
}
