using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Framework.Helpers
{
    public static class StringHelper
    {
        public static string XorString(this string input, string key = null)
        {
            if (key == null)
                key = DateTime.Today.ToString("yyyyMMdd");               

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ key[(i % key.Length)]));
            String result = sb.ToString();

            return result;
        }
    }
}
