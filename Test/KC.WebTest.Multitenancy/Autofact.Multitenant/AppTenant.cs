using System.Collections.Generic;

namespace Com.WebTest.Multitenancy
{
	public class AppTenant
    {
        public string Name { get; set; }
        public IEnumerable<string> Hostnames { get; set; }
    }
}
