using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Web.Account.DemoUtil;
using Microsoft.AspNetCore.Authorization;
using KC.Service.Base.ECharts.series;
using KC.Service.Base.ECharts;
using KC.Service.Base.ECharts.axis;
using KC.Service.Util;

namespace KC.Web.Account.Controllers
{
    public class EchartBarDemoController : Microsoft.AspNetCore.Mvc.Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetColumnData()
        {
            ChartOption option = new ChartOption();
            option.ToolTip().Trigger(TriggerType.axis)
               .AxisPointer().Type(AxisPointType.shadow);

            option.Legend("蒸发量", "降雨量");
            option.calculable = true;
            var x = new CategoryAxis();
            x.SetData(ChartsUtil.Months().Cast<object>().ToList());
            option.XAxis(x);
            var y = new ValueAxis();
            option.YAxis(y);
            var bar1 = new Bar("蒸发量");
            bar1.SetData(ChartsData.EvapsYear);
            bar1.MarkPoint().Data(new MarkData("最大值", MarkType.max), new MarkData("最小值", MarkType.min));
            bar1.MarkLine().Data(new MarkData("平均值", MarkType.average));

            var bar2 = new Bar("降水量");
            bar2.SetData(ChartsData.RainsYear);
            var mp1 = new MarkData("年最高");
            double rainMax = ChartsData.RainsYear.Max();
            mp1.value = rainMax;
            mp1.yAxis = rainMax + 0.5;
            mp1.xAxis = ChartsData.RainsYear.IndexOf(rainMax);
            mp1.symbolSize = 18;
            var mp2 = new MarkData("年最低");
            double rainMin = ChartsData.RainsYear.Min();
            mp2.Value(rainMin).YAxis(rainMin + 0.5).XAxis(ChartsData.RainsYear.IndexOf(rainMin));
            bar2.MarkPoint().Data(mp1, mp2);
            bar2.MarkLine().Data(new MarkData("平均值", MarkType.average));

            option.Series(bar1, bar2);

            //var result = EChartJsonUtil.ObjectToJson2(option);
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            return Json(option, settings);
        }

        [AllowAnonymous]
        public JsonResult GetHeapColumnData()
        {
            IList<string> ads = ChartsUtil.Ads();
            IList<string> ses = ChartsUtil.Ses();
            IList<int> data1 = ChartsUtil.Datas(7, 90, 240);
            IList<int> data2 = ChartsUtil.Datas(7, 190, 310);
            IList<int> data3 = ChartsUtil.Datas(7, 150, 410);
            IList<int> data4 = ChartsUtil.Datas(7, 300, 400);
            IList<int> data5 = ChartsUtil.Datas(7, 820, 1400);
            IList<int> data6 = ChartsUtil.Datas(7, 600, 1200);
            IList<int> data7 = ChartsUtil.Datas(7, 80, 400);
            IList<int> data8 = ChartsUtil.Datas(7, 100, 300);
            ads.ToList().AddRange(ses);
            ChartOption option = new ChartOption();
            option.ToolTip().Trigger(TriggerType.axis);
            option.Legend().SetData(new List<object>(ads.ToList()));
            option.ToolBox().Orient(OrientType.vertical);
            option.ToolBox().X(HorizontalType.right).Y(HorizontalType.center);
            option.calculable = true;
            var x = new CategoryAxis();
            x.SetData(ChartsUtil.Weeks().Cast<object>().ToList());
            option.XAxis(x);
            var y = new ValueAxis();
            option.YAxis(y);
            Bar b1 = new Bar(ads[0]);
            b1.stack = "广告";
            b1.data = data1;

            Bar b2 = new Bar(ads[1]);
            b2.stack = "广告";
            b2.data = data2;

            Bar b3 = new Bar(ads[2]);
            b3.stack = "广告";
            b3.data = data3;

            Bar b4 = new Bar(ads[3]);
            var mk1 = new MarkLine();
            mk1.ItemStyle().Normal().LineStyle().Type(LineStyleType.dashed);
            mk1.Data(new MarkData(MarkType.min), new MarkData(MarkType.max));
            b4.markLine = mk1;
            b4.data = data4;

            Bar b5 = new Bar(ads[4]);
            b5.stack = "搜索引擎";
            b5.data = data5;

            Bar b6 = new Bar(ses[0]);
            b6.stack = "搜索引擎";
            b6.data = data6;

            Bar b7 = new Bar(ses[1]);
            b7.stack = "搜索引擎";
            b7.data = data7;

            Bar b8 = new Bar(ses[2]);
            b8.stack = "搜索引擎";
            b8.data = data8;

            // option.Series<Line>(l1, l2, l3, l4, l5);
            option.Series(b1, b2, b3, b4, b5, b6, b7, b8);

            //var result = EChartJsonUtil.ObjectToJson2(option);
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            return Json(option, settings);
        }

        [AllowAnonymous]
        public JsonResult GetStdBarData()
        {
            ChartOption option = new ChartOption();
            option.Title().Text("世界人口总数").SubText("数据来源网络");
            option.ToolTip().Trigger(TriggerType.axis)
               .AxisPointer().Type(AxisPointType.shadow);
            option.Legend().Data("2011年", "2012年");
            option.calculable = true;
            var x = new ValueAxis();
            x.BoundaryGap(new List<double>() { 0, 0.01 });
            option.XAxis(x);
            var y = new CategoryAxis();
            y.SetData(new List<object>(ChartsUtil.Pops()));
            option.YAxis(y);
            var bar1 = new Bar("2011年");
            bar1.Data(18203, 23489, 29034, 104970, 131744, 630230);

            var bar2 = new Bar("2012年");
            bar2.Data(19325, 23438, 31000, 121594, 134141, 681807);

            option.Series(bar1, bar2);

            //var result = EChartJsonUtil.ObjectToJson2(option);
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            return Json(option, settings);
        }
    }


}
