﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class ListVoidableViewModel : DataAccess.Order
    {
        public decimal Total { get; set; }
    }
}
