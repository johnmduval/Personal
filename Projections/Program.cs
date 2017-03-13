using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel.FinancialFunctions;
using FinanceCommon;

namespace Projections
{
    class Program
    {
        static void Main(string[] args)
        {
            // key dates/ages
            const int retirementAge = 65;
            const int deathAge1 = 85;
            const int deathAge2 = 95;
            var startingYear = DateTime.Now.Year;
            const int retirementYear = 1970 + retirementAge;
            const int deathYear1 = 1970 + deathAge1;
            const int deathYear2 = 1970 + deathAge2;

            #region current account balances
            var positionReader = new PositionReader();
            positionReader.Load();
            // current balances, by tax treatment
            // taxable (brokerage)
            var taxableAccountValue = positionReader.GetTaxableAccountsTotal();

            // pre-tax (trad)
            var preTaxAccountValue = positionReader.GetPreTaxAccountsTotal();
            //var preTaxAccountValues = new decimal[]
            //{
            //    decimal.Parse("426,000"),
            //    decimal.Parse("70,000"),
            //    decimal.Parse("41,000"),
            //    decimal.Parse("426,000"),
            //    decimal.Parse("22,000"),
            //    decimal.Parse("70,000"),
            //    decimal.Parse("319,000"),
            //};

            // post-tax (roth)
            var postTaxAccountValue = positionReader.GetPostTaxAccountsTotal();
            //var postTaxAccountValues = new decimal[]
            //{
            //    decimal.Parse("30,000"),
            //    decimal.Parse("40,000"),
            //};
            #endregion

            decimal annualContributionsTaxable = decimal.Parse("50,000");
            decimal annualContributionsPretax = decimal.Parse("52,000");

            // annual return on investments (averaged)
            const double annualReturnPercent = 6;

            // ------------------------------------------------
            // pre-retirement info
            var convertToRothAmount = decimal.Parse("0,000,000");
            const double incomeTaxCurrent = 39.5;

            // ------------------------------------------------
            // post-retirement info
            const double incomeTaxRetirement = 39.5;
            const double longTermCapGainsRateRetirement = 20.0;
            var drawdownInfo = new DrawdownInfo
            {
                AnnualSpending = decimal.Parse("200,000"),
                InflationRate = 3.2
            };

            var growthInfoList = new List<AccountInfo>
            {
                new AccountInfo { Desc = "taxable", StartingValue = taxableAccountValue, AnnualContributions = annualContributionsTaxable, AnnualReturnPercent = annualReturnPercent, TaxRate = longTermCapGainsRateRetirement },
                new AccountInfo { Desc = "pretax", StartingValue = preTaxAccountValue, AnnualContributions = annualContributionsPretax, AnnualReturnPercent = annualReturnPercent, TaxRate = incomeTaxRetirement },
                new AccountInfo { Desc = "posttax", StartingValue = postTaxAccountValue, AnnualContributions = decimal.Parse("0"), AnnualReturnPercent = annualReturnPercent, TaxRate = 0.0 },
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
