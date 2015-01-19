using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel.FinancialFunctions;

namespace Projections
{
    class Program
    {
        static void Main(string[] args)
        {
            // key dates/ages
            var retirementAge = 60;
            var deathAge1 = 90;
            var deathAge2 = 95;
            var startingYear = DateTime.Now.Year;
            var retirementYear = 1970 + retirementAge;
            var deathYear1 = 1970 + deathAge1;
            var deathYear2 = 1970 + deathAge2;

            #region current account balances
            // current balances, by tax treatment
            // taxable (brokerage)
            var taxableAccountValues = new decimal[]
            {
                decimal.Parse("1,030,000"),
            };

            // pre-tax (trad)
            var preTaxAccountValues = new decimal[]
            {
                decimal.Parse("426,000"),
                decimal.Parse("70,000"),
                decimal.Parse("41,000"),
                decimal.Parse("426,000"),
                decimal.Parse("22,000"),
                decimal.Parse("70,000"),
                decimal.Parse("319,000"),
            };

            // post-tax (roth)
            var postTaxAccountValues = new decimal[]
            {
                decimal.Parse("30,000"),
                decimal.Parse("40,000"),
            };
            #endregion

            // annual return on investments (averaged)
            var annualReturnPercent = 5.0;

            // ------------------------------------------------
            // pre-retirement info
            var convertToRothAmount = decimal.Parse("200,000");
            var incomeTaxCurrent = 39.5;
            var longTermCapGainsRateCurrent = 20.0; 
            var growthInfoList = new List<AccountGrowthInfo>
            {
                new AccountGrowthInfo { Desc = "taxable", StartingValue = taxableAccountValues.Sum(), AnnualContributions = decimal.Parse("50,000"), AnnualReturnPercent = annualReturnPercent },
                new AccountGrowthInfo { Desc = "pretax", StartingValue = preTaxAccountValues.Sum(), AnnualContributions = decimal.Parse("52,000"), AnnualReturnPercent = annualReturnPercent },
                new AccountGrowthInfo { Desc = "posttax", StartingValue = postTaxAccountValues.Sum(), AnnualContributions = decimal.Parse("0"), AnnualReturnPercent = annualReturnPercent },
            };

            // ------------------------------------------------
            // post-retirement info
            var incomeTaxRetirement = 39.5;
            var longTermCapGainsRateRetirement = 20.0;
            var drawdownInfo = new DrawdownInfo
            {
                AnnualSpending = decimal.Parse("250,000")
            };

            var preRetire = new PreRetirement1();
            preRetire.RothConversion(convertToRothAmount, incomeTaxCurrent, growthInfoList);
            preRetire.Run(startingYear, retirementYear, growthInfoList);

            Console.WriteLine("___ RETIREMENT ___");


            var postRetire = new PostRetirement1();
            postRetire.Run(retirementYear, deathYear1, deathYear2, growthInfoList, drawdownInfo);



            //var brokerageValue = Financial.Fv(
            //    annualReturnPercent / 100.0,
            //    retirementYear - startingYear,
            //    (double)decimal.Parse("50,000"),
            //    (double)taxableAccountValues.Sum(),
            //    PaymentDue.BeginningOfPeriod);

            //Console.WriteLine("projected taxable at retirement: {0}", brokerageValue);
            Console.ReadLine();
        }




    }
}
