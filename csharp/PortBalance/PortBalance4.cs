using FinanceCommon;
using PersonalBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Http;
using System.Security.Policy;
using System.Text;

namespace PortBalance
{
    public class PortBalance4
	{
        private static readonly string[] taxFreeAccounts =
        {
            "130392103", // john roth
            "216737438", // sue roth
        };

        private static readonly string[] taxDeferredAccounts =
        {
            "130510777", // john rollover
            "210273953", // john trad
            "414351547", // john simple
            "130516597", // sue rollover
            "210273929", // sue trad
            "24748", // sue 401k
        };

        private static decimal ComputeNewInvestmentAmount()
        {
            return 255000m;
        }

        public static void Go()
		{ 
            var newInvestmentAmount = ComputeNewInvestmentAmount();
		    var liquidateBadSecurities = false;  // sell any Eliminate=true securities from tax-advantaged accounts

            // we only fetch prices for securities in the portfolio; if we are buying securities for the first time they must be hard-coded here for now
            var newSecurities = new SecurityAndPrice[]
		    {
                //new SecurityAndPrice("VEIEX", 27.65m ),
                //new SecurityAndPrice("SCHH", 35.33m ),
                //new SecurityAndPrice("VEA", 37.27m ),
		    };

            // verify target % adds up to 100%
		    var totalPct = InvestmentCategoryTargets.Instance.Targets.Sum(e => e.TargetPercent);
		    if (totalPct != 100.0m)
		    {
		        Console.WriteLine("Target percentages ({0}) don't add up to 100%", totalPct);
		        Console.ReadLine();
                return;
		    }

            // Build map [security symbol] -> [category name]
		    var securityToCategoryMap = new Dictionary<string, string>();
            InvestmentCategoryTargets.Instance.Targets.ForEach(e =>
            {
                foreach (var s in e.Securities)
                    securityToCategoryMap.Add(s.Symbol, e.Name);
            });

            var positionReader = new PositionReader();
            positionReader.Load();

            DumpCurrentPositions(positionReader.AggregateCurrentPositions);
            AnalyzeTaxEfficiency(positionReader.AggregateCurrentPositions);

            // If you know you're going to be selling some securities (within tax-advantaged accounts, where there is no immediate tax implication), 
            // they will be marked as "Eliminate=true" and we'll add the sale value to newInvestmentAmount
            Console.WriteLine("New investment amount: " + newInvestmentAmount); 
            if (liquidateBadSecurities)
		    {
                Console.WriteLine("Processing securities with Eliminate=true...");
		        var taxAdvantagedAccounts = taxDeferredAccounts.Union(taxFreeAccounts);
                newInvestmentAmount += LiquidateBadSecurities(positionReader.CurrentPositions1, InvestmentCategoryTargets.Instance.Targets, taxAdvantagedAccounts);
                newInvestmentAmount += LiquidateBadSecurities(positionReader.CurrentPositions2, InvestmentCategoryTargets.Instance.Targets, taxAdvantagedAccounts);
                Console.WriteLine("New investment amount: " + newInvestmentAmount);
            }
            var expenseRatio = ComputeWeightedExpenseRatio(positionReader.CurrentPositions1.Union(positionReader.CurrentPositions2), InvestmentCategoryTargets.Instance.Targets);
            Console.WriteLine("Effective aggregate expense ratio: " + expenseRatio.ToString("#0.000"));

            var currentPositionsBySecurity = positionReader.CurrentPositions1
                .Union(positionReader.CurrentPositions2)
		        .GroupBy(e => e.Security)
                .ToList();

            // Build map:  [security symbol] -> [Price]
			// Build list: [category name], [Total starting value of all securities in that category]
		    var priceBySecurity = new Dictionary<string, decimal>();
            var valuesByCategory = new List<CategoryAndAmount>();
            foreach (var positionGroup in currentPositionsBySecurity)
            {
                var securitySymbol = positionGroup.Key;
                if (!priceBySecurity.ContainsKey(securitySymbol))
                {
                    var currentPrice = positionGroup.First().CurrentPrice;
                    if (currentPrice == 0m)
                        throw new Exception("Price can't be zero!");
                    priceBySecurity.Add(securitySymbol, currentPrice);
                }

                var category = securityToCategoryMap[securitySymbol];
                var categoryAndAmount = valuesByCategory.SingleOrDefault(e => e.Category == category);
                if (categoryAndAmount == null)
                {
                    categoryAndAmount = new CategoryAndAmount(category, 0.0m);
                    valuesByCategory.Add(categoryAndAmount);
                }
                categoryAndAmount.Amount += positionGroup.Sum(e => e.OriginalQuantity * e.CurrentPrice);
            }

            bool investmentComingFromMoneyMarket = true;
            if (investmentComingFromMoneyMarket)
            {
                valuesByCategory.Single(e => e.Category.Contains("Money Market")).Amount -= newInvestmentAmount;
            }

            // Securities which are not yet in the portfolio need a price:
            foreach (var sec in newSecurities)
		    priceBySecurity[sec.Security] = sec.Price;

            // this contains a running total of dollars to be allocated to each security (start at 0)
            var dollarsAllocatedToCategory = InvestmentCategoryTargets.Instance.Targets
                .Select(e => new CategoryAndAmount(e.Name, 0.0m))
                .ToDictionary(e => e.Category, e => e.Amount);

            bool showPctDiff = false;
			while (newInvestmentAmount > 0.0m)
			{
                // original $ + allocated $ for all categories
				var totalValueAllCategories =
                    valuesByCategory.Sum(e => e.Amount) +	        // original starting values
                    dollarsAllocatedToCategory.Sum(e => e.Value);	// allocated so far

                var percentByCategory = valuesByCategory
					.Select(e => 
					{
                        var totalForCategory = e.Amount + dollarsAllocatedToCategory[e.Category];
					    var category = InvestmentCategoryTargets.Instance.Targets.Single(e2 => e2.Name == e.Category);
                        return new { Category = e.Category, Percent = totalForCategory / totalValueAllCategories * 100.0m, TargetPercent = category.TargetPercent };
					})
					.ToList();


                var pctDiffByCategory = percentByCategory
                    .Select(e => new { Category = e.Category, TargetPct = e.TargetPercent, Pct = e.Percent, PctDiff = (e.TargetPercent - e.Percent) / e.Percent })
					.ToList();

                if (showPctDiff)
                {
                    pctDiffByCategory.ForEach(e => Console.WriteLine("&&& {0} {1} {2} {3}",
                        e.Category.MakeFixedWidth(15, false),
                        e.TargetPct.ToString("#0.0").MakeFixedWidth(10, false),
                        e.Pct.ToString("#0.0").MakeFixedWidth(10, false),
                        e.PctDiff.ToString("#0.0%").MakeFixedWidth(10, false)
                        ));
                    //showPctDiff = false;
                }

                var dollarDiffByCategory = pctDiffByCategory.Select(e => new CategoryAndAmount(e.Category, e.PctDiff / 100.0m * totalValueAllCategories)).ToList();

                //dollarDiffByCategory.ForEach(e => Console.Write("{0} {1} ", 
                //    StringUtils.MakeFixedWidth(e.Category, 15, false), 
                //    StringUtils.MakeFixedWidth(e.Amount.ToString("#0.0"), 10, false)
                //    ));

				// get $1000 or whatever's left to invest
				var slice = Math.Min(1000, newInvestmentAmount);	
				newInvestmentAmount -= slice;

                var categoryNeedsMost = dollarDiffByCategory.OrderByDescending(e => e.Amount).First();
                var categoryName = categoryNeedsMost.Category;

                if (showPctDiff)
                {
                    Console.WriteLine("Allocating to " + categoryName);
                    Console.WriteLine("----------------------------");
                }

                categoryNeedsMost.Amount = categoryNeedsMost.Amount - slice;
                dollarsAllocatedToCategory[categoryName] += slice;
			}


            var startingTotal = valuesByCategory.Sum(e => e.Amount);
			var allocatedTotal = dollarsAllocatedToCategory.Sum(e => e.Value);

			Console.WriteLine("startingTotal=" + startingTotal);
			Console.WriteLine("allocatedTotal=" + allocatedTotal);

            var columnValues = new List<string>
            {
                "Category","Targ%","Orig%","Final%","$Orig","$Alloc","$Final","Sec","AllocShr","Price",
            };
            var columnWidths = new List<int> { 25, 7, 7, 7, 10, 10, 10, 7, 10, 10 };
            Console.WriteLine("{0}", StringFormatUtils.MakeColumnsFixedWidth(columnValues, columnWidths));

		    valuesByCategory.Sort((c1,c2) => String.Compare(c1.Category, c2.Category, StringComparison.Ordinal));
		    var totalPercentByCategoryGroup = new Dictionary<string, decimal>();
            foreach (var values in valuesByCategory)
			{
				var cat = values.Category;
				var startVals = valuesByCategory.Single(e => e.Category == cat);
				var startPct = (startVals.Amount * 100.0m) / startingTotal;

                var allocVal = dollarsAllocatedToCategory[cat];

				var finalVal = startVals.Amount + allocVal;
                var finalPct = (finalVal * 100.0m)/ (startingTotal + allocatedTotal);

			    var catGroup = GetCategoryGroup(cat);
			    if (!totalPercentByCategoryGroup.ContainsKey(catGroup))
			        totalPercentByCategoryGroup[catGroup] = 0.0m;
			    totalPercentByCategoryGroup[catGroup] += finalPct;

                // select the best security in the category (by selecting lowest ER)
                var bestSecSymbol = InvestmentCategoryTargets.Instance.Targets.Single(e => e.Name == cat).Securities.Where(e => e.Symbol != "CORE").OrderBy(e => e.ExpenseRatio).First().Symbol;
                var price = priceBySecurity[bestSecSymbol];
                var allocShr = allocVal / priceBySecurity[bestSecSymbol];

                var outputValues = new List<string>
                {
					cat,
                    InvestmentCategoryTargets.Instance.Targets.Single(e => e.Name == cat).TargetPercent.ToString("#0.00"),
                    startPct.ToString("#0.00"),
                    finalPct.ToString("#0.00"),
					
					startVals.Amount.ToString("#0.00"),
					allocVal.ToString("#00.00"),
					finalVal.ToString("#0.00"),

                    bestSecSymbol, 
					allocShr.ToString("#00.00"),
                    price.ToString("#0.00")
                };
                Console.WriteLine("{0}", StringFormatUtils.MakeColumnsFixedWidth(outputValues, columnWidths));
			}

		    foreach (var kvp in totalPercentByCategoryGroup)
		    {
		        Console.WriteLine("{0} Total: {1}", kvp.Key.MakeFixedWidth(25), kvp.Value.ToString("#0.00"));  
		    }

            Console.ReadLine();
		}

        private static void DumpCurrentPositions(List<Position> positions)
        {
            var secCol = "Security".MakeFixedWidth(10);
            var valCol = "Value".MakeFixedWidth(10);
            var acctCol = "Account".MakeFixedWidth(10);
            Console.WriteLine($"  {secCol}\t{valCol}\t{acctCol}");
            positions.Sort((a,b) => a.Security.CompareTo(b.Security));
            positions.ForEach(e =>
            {
                var sec = e.Security.MakeFixedWidth(10);
                var position = (e.OriginalQuantity * e.CurrentPrice).ToString("$0.00").MakeFixedWidth(10, false);
                var acct = e.Account.MakeFixedWidth(10);
                Console.WriteLine($"  {sec}\t{position}\t{acct}");
            });
        }

        private static void AnalyzeTaxEfficiency(List<Position> positions)
        {
            // group each position by tax efficiency & account type (taxable, tax-deferred, tax-free)
            // print out grid ordered by tax efficiency (vertical = tax efficiency, horiz = account type)
            // later...

        }

        private static string GetCategoryGroup(string category)
        {
            category = category.Substring(category.IndexOf(' ') + 1);
            if (category.StartsWith("US")) return "US Equity";
            if (category.StartsWith("Intl")) return "Intl Equity";
            if (category.StartsWith("Bond")) return "Bond";
            if (category.StartsWith("Inflation")) return "Inflation";
            return "unknown: " + category;
        }

        private static decimal ComputeWeightedExpenseRatio(IEnumerable<Position> positions, List<InvestmentCategory> categories)
        {
            var securityToExpenseRatio = categories.SelectMany(e => e.Securities).ToDictionary(e => e.Symbol, e => e.ExpenseRatio);
            var totalValue = positions.Sum(e => e.CurrentPrice * e.OriginalQuantity);
            decimal weightedTotal = 0.0m;
            foreach (var pos in positions)
            {
                var fraction = (pos.CurrentPrice*pos.OriginalQuantity) / totalValue;
                weightedTotal += fraction * securityToExpenseRatio[pos.Security];
            }
            return weightedTotal;
        }



        private static decimal LiquidateBadSecurities(List<Position> positions, List<InvestmentCategory> categories,
            IEnumerable<string> accounts)
        {
            var securitiesToSell = categories
                .SelectMany(e => e.Securities)
                .Where(e2 => e2.Eliminate)
                .Select(e3 => e3.Symbol)
                .ToList();

            var positionsToSell = positions
                .Where(e => accounts.Contains(e.Account) && securitiesToSell.Contains(e.Security))
                .ToList();
               
            positionsToSell.ForEach(e => Console.WriteLine("Sell {0} from account {1} ({2:C})", e.Security, e.Account, e.CurrentPrice * e.OriginalQuantity));

            decimal valueOfSoldPositions = 0.0m;
            positionsToSell.ForEach(e => valueOfSoldPositions += e.OriginalQuantity * e.CurrentPrice);
            positions.RemoveAll(positionsToSell.Contains);
            return valueOfSoldPositions;
        }

	}
}
