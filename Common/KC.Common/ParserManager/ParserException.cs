using System;

namespace KC.Common.ParserManager
{
    public class ParserException : ApplicationException
    {
        public ParserException(string str) : base(str) { }

        public override string ToString()
        {
            return Message;
        }
    }
}
