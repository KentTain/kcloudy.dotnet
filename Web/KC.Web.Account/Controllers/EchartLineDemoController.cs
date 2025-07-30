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
using Newtonsoft.Json.Linq;
using KC.Service.Base.ECharts.style;
using KC.Service.Base.ECharts.series.data;
using KC.Service.Base.ECharts.feature;
using KC.Service.Util;

namespace KC.Web.Account.Controllers
{
    public class EchartLineDemoController : Microsoft.AspNetCore.Mvc.Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetLineData()
        {
            IList<string> ads = ChartsUtil.Ads();
            IList<string> weeks = ChartsUtil.Weeks();
            ChartOption option = new ChartOption();
            option.ToolTip().Trigger(TriggerType.axis);
            option.Legend().SetData(new List<object>(ads));


            var feature = new Feature();
            feature.DataView().Show(true).ReadOnly(false);
            feature.Mark().Show(true);
            feature.Restore().Show(true);
            feature.MagicType().Show(true).Type("line", "bar", "stack", "tiled");
            feature.SaveAsImage().Show(true);
            option.ToolBox().Show(true).SetFeature(feature);


            option.calculable = true;
            var x = new CategoryAxis();
            x.BoundaryGap(false).SetData(new List<object>(weeks));
            option.XAxis(x);
            option.YAxis(new ValueAxis());


            var l1 = new Line("邮件营销");
            var s = new ItemStyle();
            s.Normal().AreaStyle().Color("rgba(255,255,255,0.1)");
            l1.Stack("总量").Symbol(SymbolType.none).SetItemStyle(s);
            var sd = new SeriesData<int>(90);
            sd.Symbol(SymbolType.droplet).SymbolSize(5);
            l1.Data(120, 132, 301, 134, sd, 230, 210);

            var l2 = new Line("联盟广告");
            var sd2 = new SeriesData<int>(201);
            sd2.Symbol(SymbolType.star).SymbolSize(15)
                .ItemStyle().Normal().Label().Show(true)
                .TextStyle().FontSize(20).FontFamily("微软雅黑").FontWeight("bold");
            l2.Stack("总量").Smooth(true).Symbol(@"http://echarts.baidu.com/doc/asset/ico/favicon.png").SymbolSize(8)
                .Data(120, 82, sd2, 190, new SeriesData<int>(134).Symbol(SymbolType.none),
                new SeriesData<int>(134).Symbol(SymbolType.none), 190,
                new SeriesData<int>(230).Symbol(SymbolType.emptypin).SymbolSize(8), 190, 110);

            var l3 = new Line("直接访问");
            var s1 = new ItemStyle();
            s1.Normal().Color("red").LineStyle().Width(2).Type(LineStyleType.dashed);
            s1.Emphasis().Color("blue");
            l3.Stack("总量").Symbol(SymbolType.arrow).SymbolSize(6).SymbolRotate(-45)
                .SetItemStyle(s1);

            var s2 = new ItemStyle();
            s2.Normal().Color("yelowgreen");
            s2.Emphasis().Color("orange");
            var sd3 = new SeriesData<int>(390);
            sd3.Symbol("star6").SymbolSize(20).SymbolRotate(10)
                .Label().Show(true).Position(StyleLabelTyle.inside)
                .TextStyle().FontSize(20);

            l3.SetItemStyle(s2);
            l3.Data(320, 332, "-", 334, sd3, 330, 320);

            var l4 = new Line("搜索引擎");
            var s3 = new ItemStyle();
            s3.Normal().LineStyle().Width(2).Color("rgba(255,255,0,0.8)").ShadowColor("rgba(0,0,0,0.5)")
                        .ShadowBlur(10).ShadowOffsetX(8).ShadowOffsetY(8);
            s3.Emphasis().Label().Show(true);
            l4.SetItemStyle(s3);
            var sd4 = new SeriesData<int>(734);
            sd4.Symbol(SymbolType.emptyheart).SymbolSize(10);
            l4.Data(620, 732, 791, sd4, 890, 930, 820);

            Line l5 = new Line("视频广告");
            l5.stack = "总量";
            l5.data = ChartsUtil.Datas(7, 820, 1400); ;

            option.Series<Line>(l1, l2, l3, l4, l5);
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            //var result = EChartJsonUtil.ObjectToJson2(option);
            return Json(option, settings);
        }

        [AllowAnonymous]
        public JsonResult GetStdLineData()
        {
            IList<string> weeks = ChartsUtil.Weeks();

            IList<int> datas1 = ChartsUtil.Datas(7, 10, 15);

            IList<int> datas2 = ChartsUtil.Datas(7, -2, 5);

            int min = datas2.Min();
            int index = datas2.IndexOf(min);

            ChartOption option = new ChartOption();

            option.Title(new Title()
            {
                show = true,
                text = "未来一周天气变化",
                subtext = "纯虚构数据"
            });

            option.tooltip = new ToolTip()
            {
                trigger = TriggerType.axis
            };

            option.legend = new Legend()
            {
                data = new List<object>(){
                   "最高温度",
                   "最低温度"
                 }
            };

            option.calculable = true;

            option.xAxis = new List<Axis>()
            {
                new CategoryAxis()
                {
                    type = AxisType.category,
                    boundaryGap= false,
                    data = new List<object>(weeks)
                }
            };

            option.yAxis = new List<Axis>()
            {
                new ValueAxis()
                {
                    type = AxisType.value,
                    axisLabel = new AxisLabel(){
                        formatter=new JRaw("{value} ℃").ToString()
                    }
                }
            };
            option.series = new List<object>()
            {
                    new Line()
                    {
                        name = "最高温度",
                        type =  ChartType.line,
                        data =  datas1,
                        markPoint =  new MarkPoint()
                        {
                                data = new List<object>()
                                {
                                    new MarkData()
                                    {
                                         name = "最大值",
                                         type= MarkType.max,
                                    },
                                    new MarkData()
                                    {
                                         name = "最小值",
                                         type= MarkType.min,
                                    }
                                }
                        },
                        markLine = new MarkLine()
                        {
                                data = new List<object>()
                                {
                                     new MarkData()
                                     {
                                          name = "平均值",
                                          type = MarkType.average
                                     }
                                }
                       }
              },
              new Line(){
                name="最低温度",
                type = ChartType.line,
                data = datas2,
                markPoint= new MarkPoint(){
                 data = new List<object>(){
                    new MarkData(){
                     name="周最低",
                     value = min,
                     xAxis = index,
                     yAxis = min+0.5,
                    }
                 }
                },
               markLine = new MarkLine(){
                data = new List<object>(){
                   new MarkData(){
                    type = MarkType.average,
                    name = "平均值"
                   }
                }
               }
              }
            };
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            //var result = EChartJsonUtil.ObjectToJson2(option);
            return Json(option, settings);
        }

        [AllowAnonymous]
        public JsonResult GetHeapLineData()
        {
            IList<string> ads = ChartsUtil.Ads();
            IList<string> weeks = ChartsUtil.Weeks();
            IList<int> data1 = ChartsUtil.Datas(7, 90, 240);
            IList<int> data2 = ChartsUtil.Datas(7, 190, 310);
            IList<int> data3 = ChartsUtil.Datas(7, 150, 410);
            IList<int> data4 = ChartsUtil.Datas(7, 300, 400);
            IList<int> data5 = ChartsUtil.Datas(7, 820, 1400);

            ChartOption option = new ChartOption();
            option.legend = new Legend()
            {
                data = new List<object>(ads)
            };

            option.tooltip = new ToolTip()
            {
                trigger = TriggerType.axis
            };

            option.calculable = true;

            option.XAxis(new CategoryAxis()
            {
                type = AxisType.category,
                boundaryGap = false,
                data = new List<object>(weeks),
            });

            option.YAxis(new ValueAxis()
            {
                type = AxisType.value
            });

            Line l1 = new Line(ads[0]);
            l1.stack = "总量";
            l1.data = data1;

            Line l2 = new Line(ads[1]);
            l2.stack = "总量";
            l2.data = data2;

            Line l3 = new Line(ads[2]);
            l3.stack = "总量";
            l3.data = data3;

            Line l4 = new Line(ads[3]);
            l4.stack = "总量";
            l4.data = data4;

            Line l5 = new Line(ads[4]);
            l5.stack = "总量";
            l5.data = data5;

            option.Series<Line>(l1, l2, l3, l4, l5);

            //var result = EChartJsonUtil.ObjectToJson2(option);
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            return Json(option, settings);
        }
    }


}
