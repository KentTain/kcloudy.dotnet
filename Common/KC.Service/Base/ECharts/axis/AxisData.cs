using KC.Service.Base.ECharts.style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.axis
{
    public class AxisData<T>
    {
        public T value { get; set; }

        public TextStyle textStyle { get; set; }

        public TextStyle TextStyle()
        {
            if (this.textStyle == null)
                textStyle = new style.TextStyle();
            return textStyle;
        }
    }

    
}
