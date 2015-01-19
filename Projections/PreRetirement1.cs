using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projections
{
    public class PreRetirement1
    {
        public void RothConversion(decimal convertToRothAmount, double taxRate, List<AccountGrowthInfo> growthInfoList)
        {
            var preTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "pretax");
            var postTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "posttax");

            if (preTaxGrowthInfo.StartingValue < convertToRothAmount)
                throw new Exception("convertToRothAmount too high");

            preTaxGrowthInfo.StartingValue -= convertToRothAmount;
            var addToRothAmount = (double)convertToRothAmount*(1.0 - taxRate);
            postTaxGrowthInfo.StartingValue += (decimal)addToRothAmount;
        }

        public void Run(int startingYear, int retirementYear, List<AccountGrowthInfo> growthInfoList)
        {
            growthInfoList.ForEach(e => e.ProjectionValue = e.StartingValue);
            for (int year = startingYear; year < retirementYear; year++)
            {
                growthInfoList.ForEach(e =>
                {
                    e.ProjectionValue += e.AnnualContributions;
                    e.ProjectionValue = (decimal)((double)e.ProjectionValue * (1 + e.AnnualReturnPercent / 100.0));
                });

                var vals = string.Join("\t", growthInfoList.Select(e => e.ProjectionValue.ToString("C0")));
                Console.WriteLine("{0} ({1}): {2}", year, year - 1970, vals);
            }
        }

    }
}
