using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalBase;

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
        private static List<int> ColumnWidths = new List<int> { 5, 3, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };

        public static string Header
        {
            get
            {
                var columnNames = new List<string>
                {
                    "Year",
                    "Age",
                    "RMD",

                    "TaxableValue",
                    "Spent",
                    "Amt Taxed",
                    "Tax",
                    
                    "PretaxValue",
                    "Spent",
                    "Amt Taxed",
                    "Tax",
                    
                    "PostTaxValue",
                    "Spent",
                    "Amt Taxed",
                    "Tax"
                };
                return StringFormatUtils.MakeColumnsFixedWidth(columnNames, ColumnWidths);
            }
        }
        public override string ToString()
        {
            var columnValues = new List<string>
            {
                Year.ToString(),
                Age.ToString(),
                Rmd.ToString("C0"),

                CurrentValue["taxable"].ToString("C0"),
                Spent["taxable"].ToString("C0"),
                AmountTaxed["taxable"].ToString("C0"),
                DistributionTaxes["taxable"].ToString("C0"),

                CurrentValue["pretax"].ToString("C0"),
                Spent["pretax"].ToString("C0"),
                AmountTaxed["pretax"].ToString("C0"),
                DistributionTaxes["pretax"].ToString("C0"),

                CurrentValue["posttax"].ToString("C0"),
                Spent["posttax"].ToString("C0"),
                AmountTaxed["posttax"].ToString("C0"),
                DistributionTaxes["posttax"].ToString("C0"),
            };
            return StringFormatUtils.MakeColumnsFixedWidth(columnValues, ColumnWidths);
        }
    }
}
