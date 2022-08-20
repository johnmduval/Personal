using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceCommon
{
    public class SecurityAndExpenseRatio
    {
        public string Symbol { get; set; }
        public decimal ExpenseRatio { get; set; }
        public string Description { get; set; }
        public bool Eliminate { get; set; }

        public SecurityAndExpenseRatio()
        {
            this.Eliminate = false;
        }
    }
}
