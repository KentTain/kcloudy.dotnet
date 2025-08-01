using KC.Service.Base.ECharts.axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.category
{
    public class XCategory
    {
        public AxisLine axisLine { get; set; }

        public AxisTick axisTick { get; set; }

        public AxisLabel axisLabel { get; set; }

        public SplitLine splitLine { get; set; }

        public SplitArea splitArea { get; set; }

        public KC.Service.Base.ECharts.axis.AxisLabel AxisLabel()
        {
            axisLabel = new AxisLabel();
            return axisLabel;
        }

    }

    public class Category<T> : XCategory
		where T : class
    {
        public string name { get; set; }

        public string nameLocation { get; set; }

        public string position { get; set; }

        public int? splitNumber { get; set; }

        public T Name()
        {
            return this as T;
        }
    }

    public class ValueCategory : Category<ValueCategory>
    {

        public string name2 { get; set; }

        public string postiton2 { get; set; }

    }
}
