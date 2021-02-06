using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers
{
    public class QuickafeException : Exception
    {
        public QuickafeException(string message) : base(message)
        {
        }
    }
}
