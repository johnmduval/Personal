using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projections
{
    public class AnnualDebugInfo
    {
        public int Year { get; set; }
        public int Age { get; set; }
        public decimal Rmd { get; set; }
        public Dictionary<string, decimal> CurrentValue { get; set; }
        public Dictionary<string, decimal> Spent { get; set; }
        public Dictionary<string, decimal> DistributionTaxes { get; set; }
        public Dictionary<string, decimal> AmountTaxed { get; set; }
        public Dictionary<string, double> TaxRate { get; set; }

        public AnnualDebugInfo()
        {
            this.CurrentValue = new Dictionary<string, decimal>();
            this.Spent = new Dictionary<string, decimal>();
            this.DistributionTaxes = new Dictionary<string, decimal>();
            this.AmountTaxed = new Dictionary<string, decimal>();
            this.TaxRate = new Dictionary<string, double>();

            var types = new[] {"taxable", "pretax", "posttax"};
            Array.ForEach(types, e =>
            {
                this.Spent[e] = 0.0m;
                this.CurrentValue[e] = 0.0m;
                this.DistributionTaxes[e] = 0.0m;
                this.AmountTaxed[e] = 0.0m;
                this.TaxRate[e] = 0.0;
            });
        }

        public static string Header
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14}",
                    StringUtils.MakeFixedWidth("Year", 5),
                    StringUtils.MakeFixedWidth("Age", 3),
                    StringUtils.MakeFixedWidth("RMD", 15),

                    StringUtils.MakeFixedWidth("TaxableValue", 15),
                    StringUtils.MakeFixedWidth("Spent", 15),
                    StringUtils.MakeFixedWidth("Amt Taxed", 15),
                    StringUtils.MakeFixedWidth("Tax ", 15),
                    
                    StringUtils.MakeFixedWidth("PretaxValue", 15),
                    StringUtils.MakeFixedWidth("Spent", 15),
                    StringUtils.MakeFixedWidth("Amt Taxed", 15),
                    StringUtils.MakeFixedWidth("Tax", 15),
                    
                    StringUtils.MakeFixedWidth("PostTaxValue", 15),                    
                    StringUtils.MakeFixedWidth("Spent", 15),
                    StringUtils.MakeFixedWidth("Amt Taxed", 15),
                    StringUtils.MakeFixedWidth("Tax", 15)
                    );
            }
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14}",
                StringUtils.MakeFixedWidth(Year.ToString(), 5),
                StringUtils.MakeFixedWidth(Age.ToString(), 3),
                StringUtils.MakeFixedWidth(Rmd.ToString("C0"), 15),

                StringUtils.MakeFixedWidth(CurrentValue["taxable"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(Spent["taxable"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(AmountTaxed["taxable"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(DistributionTaxes["taxable"].ToString("C0"), 15),

                StringUtils.MakeFixedWidth(CurrentValue["pretax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(Spent["pretax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(AmountTaxed["pretax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(DistributionTaxes["pretax"].ToString("C0"), 15),

                StringUtils.MakeFixedWidth(CurrentValue["posttax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(Spent["posttax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(AmountTaxed["posttax"].ToString("C0"), 15),
                StringUtils.MakeFixedWidth(DistributionTaxes["posttax"].ToString("C0"), 15)
                );
        }
    }
}
