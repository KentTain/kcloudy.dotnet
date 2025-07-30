using KC.Service.Base.ECharts.feature;
using KC.Service.Base.ECharts.style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base.ECharts
{
    public class ToolBox:Basic<ToolBox>
    {     
        public new OrientType? orient { get; set; }

        public new int? itemGap { get; set; }

        public int? itemSize { get; set; }

        public IList<string> color { get; set; }

        public string disableColor { get; set; }

        public string effectiveColor { get; set; }

        public bool? showTitle { get; set; }

        public TextStyle textStyle { get; set; }

        public Feature feature { get; set; }

        public ItemStyle iconStyle { get; set; }
        

        public ToolBox ShowTitle(bool showTitle)
        {
            this.showTitle = showTitle;
            return this;
        }

        public ToolBox EffectiveColor(string effectiveColor)
        {
            this.effectiveColor = effectiveColor;
            return this;
        }


        public ToolBox DisableColor(string disableColor)
        {
            this.disableColor = disableColor;
            return this;
        }

        public ToolBox ItemSize(int itemSize)
        {
            this.itemSize = itemSize;
            return this;
        }

        public new ToolBox ItemGap(int itemGap)
        {
            this.itemGap = itemGap;
            return this;
        }

        public new ToolBox Orient(OrientType orient)
        {
            this.orient = orient;
            return this;
        }


        public Feature Feature()
        {
            if (feature == null)
                feature = new feature.Feature();
            return feature;
        }

        public ToolBox SetFeature(Feature feature)
        {
            this.feature = feature;
            return this;
        }

        public TextStyle TextStyle()
        {
            if (textStyle == null)
                this.textStyle = new style.TextStyle();
            return this.textStyle;
        }

        public ItemStyle IconStyle()
        {
            if (iconStyle==null)
            {
                this.iconStyle = new ItemStyle();
            }
            return this.iconStyle;
        }
    }
}
