using System;
using System.Collections.Generic;
using System.Text;

namespace Webthucpham.Utilities.Exceptions
{
    public class WebthucphamException : Exception
    {
        public WebthucphamException()
        {
        }

        public WebthucphamException(string message)
            : base(message)
        {
        }

        public WebthucphamException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
