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
        private decimal _cp;
        public decimal CurrentPrice 
        {
            get { return _cp; } 
            set 
            {
                if (value == 0m)
                    throw new Exception("Price cannot be zero!");
                _cp = value; 
            }
        }

        public decimal OriginalQuantity { get; set; }

        public string Account { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} @ {2} (acct={3})", Security, OriginalQuantity, CurrentPrice, Account);
        }
    }

}
