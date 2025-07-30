using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO
{
    public class FilterBaseDTO
    {
        private int _pageIndex;
        public int PageIndex
        {
            get
            {
                if (_pageIndex <= 0)
                    _pageIndex = 1;
                else if (_pageIndex >= 100)
                    _pageIndex = 100;
                return _pageIndex;
            }
            set { _pageIndex = value; }
        }

        public int PageSize
        {
            get { return 20; }
        }
    }
}
