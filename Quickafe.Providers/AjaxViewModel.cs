using Quickafe.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.ViewModels
{
    public class AjaxViewModel : BaseAjaxViewModel
    {
        public dynamic Data { get; private set; }

        public AjaxViewModel(bool isSuccess = true, dynamic data = null, string message = null) : 
            base(isSuccess, message: message)
        {
            Data = data;
        }
    }
}
