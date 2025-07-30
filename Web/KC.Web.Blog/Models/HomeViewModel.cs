using KC.Framework.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace KC.Web.Config.Models
{
    [Serializable, DataContract(IsReference = true)]
    [ResponseCache(Duration = 30 * 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Tags = new List<string>();
            TopBlogs = new List<EFKeyValuePair<int, string>>();
            NewBlogs = new List<EFKeyValuePair<int, string>>();
            Categories = new List<EFKeyValuePair<int, string>>();
        }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string AboutMe { get; set; }
        [DataMember]
        public List<string> Tags { get; set; }
        [DataMember]
        public List<EFKeyValuePair<int, string>> TopBlogs { get; set; }
        [DataMember]
        public List<EFKeyValuePair<int, string>> NewBlogs { get; set; }
        [DataMember]
        public List<EFKeyValuePair<int, string>> Categories { get; set; }


        public static string GetColor(int colorIndex)
        {
            var btnStyles = new string[] {"btn btn-default btn-xs", "btn btn-primary btn-xs",
            "btn btn-success btn-xs", "btn btn-default btn-xs", "btn btn-info btn-xs", "btn btn-warning btn-xs", "btn btn-default btn-xs", "btn btn-danger btn-xs" };
            return btnStyles[colorIndex % btnStyles.Length];
        }
    }
}
