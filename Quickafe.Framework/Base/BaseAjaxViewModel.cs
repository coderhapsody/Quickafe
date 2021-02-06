using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Framework.Base
{
    public class BaseAjaxViewModel
    {
        public BaseAjaxViewModel(bool isSuccess = true, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
    }
}
