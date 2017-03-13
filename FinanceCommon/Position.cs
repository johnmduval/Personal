using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceCommon
{
    public class Position
    {
        public string Security { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal OriginalQuantity { get; set; }

        public string Account { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} @ {2} (acct={3})", Security, OriginalQuantity, CurrentPrice, Account);
        }
    }

}
