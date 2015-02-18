using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projections
{
    public class PreRetirement1
    {
        public void RothConversion(decimal convertToRothAmount, double taxRate, List<AccountInfo> growthInfoList)
        {
            var preTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "pretax");
            var postTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "posttax");

            if (preTaxGrowthInfo.StartingValue < convertToRothAmount)
                throw new Exception("convertToRothAmount too high");

            preTaxGrowthInfo.StartingValue -= convertToRothAmount;
            var addToRothAmount = (double)convertToRothAmount*(1.0 - taxRate / 100);
            postTaxGrowthInfo.StartingValue += (decimal)addToRothAmount;
        }

        public void Run(int startingYear, int retirementYear, List<AccountInfo> growthInfoList)
        {
            growthInfoList.ForEach(e => e.CurrentValue = e.StartingValue);
            Console.WriteLine(AnnualDebugInfo.Header);
            for (int year = startingYear; year < retirementYear; year++)
            {
                growthInfoList.ForEach(e =>
                {
                    e.CurrentValue += e.AnnualContributions;
                    e.CostBasis += e.AnnualContributions;
                    e.CurrentValue = (decimal)((double)e.CurrentValue * (1 + e.AnnualReturnPercent / 100.0));
                });

                var debug = new AnnualDebugInfo
                {
                    Year = year, 
                    Age = year - 1970,
                    Rmd = 0.0m, 
                };
                debug.CurrentValue["taxable"] = growthInfoList.Single(e => e.Desc == "taxable").CurrentValue;
                debug.CurrentValue["pretax"] = growthInfoList.Single(e => e.Desc == "pretax").CurrentValue;
                debug.CurrentValue["posttax"] = growthInfoList.Single(e => e.Desc == "posttax").CurrentValue;

                Console.WriteLine("{0}", debug);
            }
        }

    }
}
