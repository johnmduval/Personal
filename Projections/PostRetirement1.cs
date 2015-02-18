using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projections
{
    public class PostRetirement1
    {
        private decimal ComputeTaxes(AccountInfo accountInfo, decimal amountToSpend, AnnualDebugInfo debug)
        {
            // compute taxes
            decimal taxes;
            switch (accountInfo.Desc)
            {
                case "taxable":
                    // estimate capital gains:
                    var totalCapitalGains = accountInfo.CurrentValue - accountInfo.CostBasis;
                    var percentCapitalGains = (double)totalCapitalGains / (double)(accountInfo.CurrentValue);
                    var capitalGains = (double)amountToSpend*percentCapitalGains;
                    var basisSpent = (double) amountToSpend - capitalGains;
                    accountInfo.CostBasis -= (decimal)basisSpent;

                    debug.AmountTaxed[accountInfo.Desc] = (decimal)capitalGains;
                    debug.TaxRate[accountInfo.Desc] = accountInfo.TaxRate;
                    taxes = (decimal)(capitalGains*accountInfo.TaxRate/100.0);
                    break;
                case "pretax":
                    // treat as ordinary income -- use the tax rate provided
                    debug.AmountTaxed[accountInfo.Desc] = amountToSpend;
                    taxes = (decimal)((accountInfo.TaxRate / 100.0) * (double)amountToSpend);
                    break;
                case "posttax":
                    taxes = 0.0m;
                    break;
                default:
                    throw new ArgumentException("Bad account type: " + accountInfo.Desc);
            }
            return taxes;
        }

        // deducts up to 'amountToSpend' from the specified account, returns amountToSpend minus how much was deducted 
        private decimal Spend(decimal amountToSpend, AccountInfo accountInfo, AnnualDebugInfo debug)
        {
            if (amountToSpend == 0.0m)
                return amountToSpend;

            if (accountInfo.CurrentValue <= 0.0m) 
                return amountToSpend;

            decimal taxes = ComputeTaxes(accountInfo, amountToSpend, debug);

            decimal deductFromAccount, spentOnExpenses, spentOnTaxes;
            if (amountToSpend + taxes > accountInfo.CurrentValue)
            {
                // If the spending money plus taxes > account current value, we are going to deduct everything from the account
                // However, we don't know how much of this money goes to taxes, and how much is spending money
                deductFromAccount = accountInfo.CurrentValue;

                var percentSpendingMoney = amountToSpend / (amountToSpend + taxes);
                spentOnExpenses = percentSpendingMoney*accountInfo.CurrentValue;
                spentOnTaxes = accountInfo.CurrentValue - spentOnExpenses;
            }
            else
            {
                // If the spending money plus taxes < account current value, then we deduct (spending + taxes) from the account
                deductFromAccount = amountToSpend + taxes;
                spentOnExpenses = amountToSpend;
                spentOnTaxes = taxes;
            }

            debug.Spent[accountInfo.Desc] = spentOnExpenses;
            debug.DistributionTaxes[accountInfo.Desc] = spentOnTaxes;

            accountInfo.CurrentValue -= deductFromAccount;
            amountToSpend -= spentOnExpenses;
            return amountToSpend;
        }

        public void Run(int retirementYear, int deathYear1, int deathYear2, List<AccountInfo> growthInfoList, DrawdownInfo drawdownInfo)
        {
            var taxableGrowthInfo = growthInfoList.Single(e => e.Desc == "taxable");
            var preTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "pretax");
            var postTaxGrowthInfo = growthInfoList.Single(e => e.Desc == "posttax");

            //growthInfoList.ForEach(e => e.CurrentValue = e.StartingValue);
            var endYear = Math.Max(deathYear1, deathYear2);
            for (int year = retirementYear; year <= endYear; year++)
            {

                var debug = new AnnualDebugInfo
                {
                    Year = year,
                    Age = year - 1970,
                };
                debug.CurrentValue["taxable"] =growthInfoList.Single(e => e.Desc == "taxable").CurrentValue;
                debug.CurrentValue["pretax"] =growthInfoList.Single(e => e.Desc == "pretax").CurrentValue;
                debug.CurrentValue["posttax"] =growthInfoList.Single(e => e.Desc == "posttax").CurrentValue;

                // Order of withdrawal:
                // 1) RMDs
                // 2) Taxable flows (brokerage)
                // 3) If expect higher marginal tax bracket in future
                //    a) Tax-deferred (pre-tax)
                //    b) Tax-free (post-tax)
                // 4) If expect lower marginal tax bracket in future
                //    a) Tax-free (post-tax)
                //    b) Tax-deferred (pre-tax)
                
                // 1) spend RMDs first
                debug.Rmd = RequiredMinimumDistribution.Compute(year - 1970, preTaxGrowthInfo.CurrentValue);
                var amountToSpend = Math.Max(drawdownInfo.AnnualSpending, debug.Rmd);
                if (debug.Rmd > 0.0m)
                {
                    var residualRmd = Spend(debug.Rmd, preTaxGrowthInfo, debug);
                    amountToSpend = amountToSpend - debug.Rmd + residualRmd;
                }

                // 2) taxable next
                amountToSpend = Spend(amountToSpend, taxableGrowthInfo, debug);

                // 3) tax-deferred next
                amountToSpend = Spend(amountToSpend, preTaxGrowthInfo, debug);

                // 4) tax-free next
                amountToSpend = Spend(amountToSpend, postTaxGrowthInfo, debug);

                growthInfoList.ForEach(e =>
                {
                    e.CurrentValue = (decimal)((double)e.CurrentValue * (1 + e.AnnualReturnPercent / 100.0));
                });
                drawdownInfo.AnnualSpending *= (decimal)(1 + drawdownInfo.InflationRate/100.0);
                Console.WriteLine("{0}", debug);

                if (amountToSpend > 0.0m)
                {
                    Console.WriteLine("OUT OF MONEY");
                    return;
                }
            }
        }

    }
}
