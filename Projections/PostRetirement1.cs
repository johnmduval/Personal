using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projections
{
    public class PostRetirement1
    {

        private decimal Spend(decimal amountToSpend, AccountGrowthInfo accountGrowthInfo)
        {
            if (accountGrowthInfo.ProjectionValue > 0.0m)
            {
                var spendFromAccount = Math.Min(amountToSpend, accountGrowthInfo.ProjectionValue);
                accountGrowthInfo.ProjectionValue -= spendFromAccount;
                amountToSpend -= spendFromAccount;
            }
            return amountToSpend;
        }

        public void Run(int retirementYear, int deathYear1, int deathYear2, List<AccountGrowthInfo> growthInfoList, DrawdownInfo drawdownInfo)
        {
            var taxableGrowthInfo = growthInfoList.Single(e => e.Desc == "taxable");
            var preTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "pretax");
            var postTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "posttax");

            //growthInfoList.ForEach(e => e.ProjectionValue = e.StartingValue);
            var endYear = Math.Max(deathYear1, deathYear2);
            for (int year = retirementYear; year < endYear; year++)
            {

                // Order of withdrawal:
                // 1) RMDs
                // 2) Taxable flows (brokerage)
                // 3) If expect higher marginal tax bracket in future
                //    a) Tax-deferred (pre-tax)
                //    b) Tax-free (post-tax)
                // 4) If expect lower marginal tax bracket in future
                //    a) Tax-free (post-tax)
                //    b) Tax-deferred (pre-tax)


                // Roth IRA tax-free 
                // Others are taxed as regular income
                var x = RequiredMinimumDistribution.Compute(year - 1970, preTaxGrowthInfo.ProjectionValue);
                var amountToSpend = Math.Max(drawdownInfo.AnnualSpending);
                amountToSpend = Spend(amountToSpend, taxableGrowthInfo);
                amountToSpend = Spend(amountToSpend, preTaxGrowthInfo);
                amountToSpend = Spend(amountToSpend, postTaxGrowthInfo);
                if (amountToSpend > 0.0m)
                {
                    Console.WriteLine("OUT OF MONEY");
                    return;
                }
                growthInfoList.ForEach(e =>
                {
                    e.ProjectionValue = (decimal)((double)e.ProjectionValue * (1 + e.AnnualReturnPercent / 100.0));
                });

                var vals = string.Join("\t", growthInfoList.Select(e => e.ProjectionValue.ToString("C0")));
                Console.WriteLine("{0} ({1}): {2}", year, year - 1970, vals);
            }
        }

    }
}
