using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceCommon
{
    public class SecurityAndAmount
    {
        public SecurityAndAmount(string security, decimal amount)
        {
            this.Security = security;
            this.Amount = amount;
        }
        public string Security { get; private set; }
        public decimal Amount { get; set; }
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Security, this.Amount);
        }
    }

    public class SecurityAndPrice
    {
        public SecurityAndPrice(string security, decimal price)
        {
            this.Security = security;
            this.Price = price;
        }
        public string Security { get; private set; }
        public decimal Price { get; set; }
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Security, this.Price);
        }
    }
    
    public class CategoryAndAmount
    {
        public CategoryAndAmount(string category, decimal amount)
        {
            this.Category = category;
            this.Amount = amount;
        }
        public string Category { get; private set; }
        public decimal Amount { get; set; }
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Category, this.Amount);
        }
    }
}
