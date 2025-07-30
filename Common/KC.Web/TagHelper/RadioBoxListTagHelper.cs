using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.TagHelper
{
    [HtmlTargetElement(Attributes = "asp-radiolistbox, asp-modelname")]
    public class RadioBoxListTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {
        [HtmlAttributeName("asp-radiolistbox")]
        public IEnumerable<SelectListItem> Items { get; set; }

        [HtmlAttributeName("asp-modelname")]
        public string ModelName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var i = 0;
            if (Items != null && Items.Any())
            {
                foreach (var item in Items)
                {
                    var selected = item.Selected ? @"checked=""checked""" : "";
                    var disabled = item.Disabled ? @"disabled=""disabled""" : "";

                    var html = $@"<label><input type=""radio"" {selected} {disabled} id=""{ModelName}_{i}__Selected"" name=""{ModelName}[{i}].Selected"" value=""true"" /> {item.Text}</label>";
                    html += $@"<input type=""hidden"" id=""{ModelName}_{i}__Value"" name=""{ModelName}[{i}].Value"" value=""{item.Value}"">";
                    html += $@"<input type=""hidden"" id=""{ModelName}_{i}__Text"" name=""{ModelName}[{i}].Text"" value=""{item.Text}"">";

                    output.Content.AppendHtml(html);

                    i++;
                }

                output.Attributes.SetAttribute("class", "th-rdolstbx");
            }
            base.Process(context, output);
        }
    }
}
