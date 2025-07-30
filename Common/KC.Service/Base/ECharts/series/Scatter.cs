using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts.series
{
    public class Scatter :Rectangular<Scatter>
    {

        public bool? large { get; set; }

        public int? largeThreshold { get; set; }

        public new bool? hoverAnimation { get; set; }

        public Scatter LargeThreshold(int largeThreshold)
        {
            this.largeThreshold = largeThreshold;
            return this;
        }


        public new Scatter HoverAnimation(bool hoverAnimation)
        {
            this.hoverAnimation = hoverAnimation;
            return this;
        }

        public Scatter Large(bool large)
        {
            this.large = large;
            return this;
        }


        public Scatter() {
            this.type = ChartType.scatter;
        }

        public Scatter(string name) : this() {
            this.name = name;
        }
    }
}
