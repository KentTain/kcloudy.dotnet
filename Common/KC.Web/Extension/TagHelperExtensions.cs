using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Web.Extension
{
    [Microsoft.AspNetCore.Razor.TagHelpers.HtmlTargetElement("label", Attributes = "asp-for")]
    public class ActionLinkForPermissionTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {

    }

}
