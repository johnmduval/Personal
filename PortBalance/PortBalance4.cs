using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace PortBalance
{
    public class InvestmentCategory
    {
        public string Name { get; set; }
        public int TaxEfficiency { get; set; }  // lower = not efficient, higher = more efficient
        public decimal TargetPercent { get; set; }
        public IEnumerable<SecurityAndExpenseRatio> Securities { get; set; }
    }


    public class SecurityAndExpenseRatio
    {
        public string Symbol { get; set; }
        public decimal ExpenseRatio { get; set; }
        public string Description { get; set; }
        public bool Eliminate { get; set; }

        public SecurityAndExpenseRatio()
        {
            this.Eliminate = false;
        }
    }

    public class PortBalance4
	{
		public static void Go()
		{
            var inputCsv1 = @"C:\Users\admin\Downloads\john.csv";
            var inputCsv2 = @"C:\Users\admin\Downloads\sue.csv";

            var newInvestmentAmount = 13000.0m;
		    var liquidateBadSecurities = false;  // sell any Eliminate=true securities from tax-advantaged accounts

		    var targetPercents = new List<InvestmentCategory>
		    {
                // 
		        new InvestmentCategory { Name = "1 US Large Cap", TargetPercent = 20.0m, TaxEfficiency = 3,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "SPY", ExpenseRatio = 0.09m, Description = "S&P 500"},
		                                 new SecurityAndExpenseRatio {Symbol = "PEOPX", ExpenseRatio = 0.50m, Description = "S&P 500", Eliminate = true },
		                                 new SecurityAndExpenseRatio {Symbol = "FBGRX", ExpenseRatio = 0.74m, Description = "Blue Chip", Eliminate = true },
		                                 new SecurityAndExpenseRatio {Symbol = "FUSVX", ExpenseRatio = 0.05m, Description = "S&P 500 80%"}, }
		        },
		        new InvestmentCategory { Name = "2 US Broad Mkt", TargetPercent = 20.0m, TaxEfficiency = 3,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "IWV", ExpenseRatio = 0.20m, Description = "Russell 3000", Eliminate = true }, 
		                                 new SecurityAndExpenseRatio {Symbol = "FSEVX", ExpenseRatio = 0.07m, Description = "Mid / Small Cap"}, }
		        },
                new InvestmentCategory { Name = "3 US Small Cap", TargetPercent = 15.0m, TaxEfficiency = 4,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "IJR", ExpenseRatio = 0.17m, Description = "S&P SmallCap 600"}, }
		        },
                new InvestmentCategory { Name = "4 US Real Estate", TargetPercent = 5.0m, TaxEfficiency = 2,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "IYR", ExpenseRatio = 0.46m, Description = "Dow Jones U.S. Real Estate Index", Eliminate = true},
		                                 new SecurityAndExpenseRatio {Symbol = "SCHH", ExpenseRatio = 0.07m, Description = "S&P SmallCap 600"}, }
		        },
                new InvestmentCategory { Name = "5 Intl Large/Mid Cap", TargetPercent = 15.0m, TaxEfficiency = 5,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "ACWX", ExpenseRatio = 0.34m, Description = "All Country World Index (ACWI)", Eliminate = true },
                                         new SecurityAndExpenseRatio {Symbol = "VEA", ExpenseRatio = 0.09m, Description = "FTSE Developed ex North America Index" },
		                                 new SecurityAndExpenseRatio {Symbol = "VTMGX", ExpenseRatio = 0.09m, Description = "FTSE Developed ex North America Index"}, }                     
		        },
                new InvestmentCategory { Name = "6 Intl Small Cap", TargetPercent = 5.0m, TaxEfficiency = 5,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "SCZ", ExpenseRatio = 0.40m, Description = "Europe Asia Far East (EAFE) Small Cap"}, }
		        },
                new InvestmentCategory { Name = "7 Intl BRIC", TargetPercent = 10.0m, TaxEfficiency = 5,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "VEIEX", ExpenseRatio = 0.33m, Description = "Brazil Russia India China"},
		                                 new SecurityAndExpenseRatio {Symbol = "BKF", ExpenseRatio = 0.67m, Description = "Brazil Russia India China", Eliminate = true }, }
		        },
                new InvestmentCategory { Name = "8 Bonds", TargetPercent = 5.0m, TaxEfficiency = 1,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "AGG", ExpenseRatio = 0.08m, Description = "Barclays U.S. Aggregate Bond Index"},
		                                 new SecurityAndExpenseRatio {Symbol = "FSITX", ExpenseRatio = 0.10m, Description = "Barclays U.S. Aggregate Bond Index"}, }
		        },
                new InvestmentCategory { Name = "9 Inflation Protected", TargetPercent = 5.0m, TaxEfficiency = 1,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "TIP", ExpenseRatio = 0.20m, Description = "U.S. Treasury Inflation Protected Securities"}, }
		        },
		        new InvestmentCategory { Name = "9a Money Market/Cash", TargetPercent = 0.0m, TaxEfficiency = 5,
		            Securities = new[] { new SecurityAndExpenseRatio {Symbol = "FTEXX", ExpenseRatio = 0.09m, Description = "Municipal Money Market"},
		                                 new SecurityAndExpenseRatio {Symbol = "FDRXX", ExpenseRatio = 0.24m, Description = "Cash Reserves"}, }
		        },
            };

            // we only fetch prices for securities in the portfolio; if we are buying securities for the first time they must be hard-coded here for now
		    var newSecurities = new[]
		    {
		        new { Symbol = "VEIEX", LatestPrice = 27.65m },
		        new { Symbol = "SCHH", LatestPrice = 35.33m },
		    };

		    var taxAdvantagedAccounts = new[]
		    {
                "130392103",    // john roth
                "130510777",    // john rollover
                "210273953",    // john trad
                "414351547",    // john simple
                "130516597",    // sue rollover
                "210273929",    // sue trad
                "216737438",    // sue roth
                "24748",        // sue 401k
		    };

            // Build map [security symbol] -> [category name]
		    var securityToCategoryMap = new Dictionary<string, string>();
            targetPercents.ForEach(e =>
            {
                foreach (var s in e.Securities)
                    securityToCategoryMap.Add(s.Symbol, e.Name);
            });

            // Read all current security positions from CSVs, combine into list of positions, grouped by security symbol
		    if (!File.Exists(inputCsv1) || !File.Exists(inputCsv2))
		    {
		        Console.WriteLine("Input file(s) not found");
		        return;
		    }
            var currentPositions1 = ReadCsv(inputCsv1, targetPercents);
            var currentPositions2 = ReadCsv(inputCsv2, targetPercents);
            
            // If you know you're going to be selling some securities (within tax-advantaged accounts, where there is no immediate tax implication), 
            // they will be marked as "Eliminate=true" and we'll add the sale value to newInvestmentAmount
            Console.WriteLine("New investment amount: " + newInvestmentAmount); 
            if (liquidateBadSecurities)
		    {
                Console.WriteLine("Processing securities with Eliminate=true...");
                newInvestmentAmount += LiquidateBadSecurities(currentPositions1, targetPercents, taxAdvantagedAccounts);
                newInvestmentAmount += LiquidateBadSecurities(currentPositions2, targetPercents, taxAdvantagedAccounts);
                Console.WriteLine("New investment amount: " + newInvestmentAmount);
            }
            var expenseRatio = ComputeWeightedExpenseRatio(currentPositions1.Union(currentPositions2), targetPercents);
            Console.WriteLine("Effective aggregate expense ratio: " + expenseRatio.ToString("#0.000"));

		    var currentPositionsBySecurity = currentPositions1
		        .Union(currentPositions2)
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
                    priceBySecurity.Add(securitySymbol, positionGroup.First().CurrentPrice);

                var category = securityToCategoryMap[securitySymbol];
                var categoryAndAmount = valuesByCategory.SingleOrDefault(e => e.Category == category);
                if (categoryAndAmount == null)
                {
                    categoryAndAmount = new CategoryAndAmount(category, 0.0m);
                    valuesByCategory.Add(categoryAndAmount);
                }
                categoryAndAmount.Amount += positionGroup.Sum(e => e.OriginalQuantity * e.CurrentPrice);
            }
            
            // Securities which are not yet in the portfolio need a price:
            foreach (var sec in newSecurities)
		        priceBySecurity[sec.Symbol] = sec.LatestPrice;

            // this contains a running total of dollars to be allocated to each security (start at 0)
            var dollarsAllocatedToCategory = targetPercents
                .Select(e => new CategoryAndAmount(e.Name, 0.0m))
                .ToDictionary(e => e.Category, e => e.Amount);
			
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
					    var category = targetPercents.Single(e2 => e2.Name == e.Category);
                        return new { Category = e.Category, Percent = totalForCategory / totalValueAllCategories * 100.0m, TargetPercent = category.TargetPercent };
					})
					.ToList();

                var pctDiffByCategory = percentByCategory
                    .Select(e => new { Category = e.Category, PctDiff = e.TargetPercent - e.Percent })
					.ToList();

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

                categoryNeedsMost.Amount = categoryNeedsMost.Amount - slice;
                dollarsAllocatedToCategory[categoryName] += slice;
			}


            var startingTotal = valuesByCategory.Sum(e => e.Amount);
			var allocatedTotal = dollarsAllocatedToCategory.Sum(e => e.Value);

			Console.WriteLine("startingTotal=" + startingTotal);
			Console.WriteLine("allocatedTotal=" + allocatedTotal);

			Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
                StringUtils.MakeFixedWidth("Category", 25, false),

                StringUtils.MakeFixedWidth("Targ%", 7, false),
                StringUtils.MakeFixedWidth("Orig%", 7, false),
                StringUtils.MakeFixedWidth("Final%", 7, false),		

				StringUtils.MakeFixedWidth("Orig$", 10, false),
				StringUtils.MakeFixedWidth("Alloc$", 10, false),
				StringUtils.MakeFixedWidth("Final$", 10, false),

                StringUtils.MakeFixedWidth("Sec", 7, false),
				StringUtils.MakeFixedWidth("AllocShr", 10, false),
				StringUtils.MakeFixedWidth("Price", 10, false)
			);

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
                var bestSecSymbol = targetPercents.Single(e => e.Name == cat).Securities.OrderBy(e => e.ExpenseRatio).First().Symbol;
                var price = priceBySecurity[bestSecSymbol];
                var allocShr = allocVal / priceBySecurity[bestSecSymbol];
                
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", 
					StringUtils.MakeFixedWidth(cat, 25, false),
					StringUtils.MakeFixedWidth(targetPercents.Single(e => e.Name == cat).TargetPercent.ToString("#0.00"), 7, false),
                    StringUtils.MakeFixedWidth(startPct.ToString("#0.00"), 7, false),
                    StringUtils.MakeFixedWidth(finalPct.ToString("#0.00"), 7, false),
					
					StringUtils.MakeFixedWidth(startVals.Amount.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(allocVal.ToString("#00.00"), 10, false),
					StringUtils.MakeFixedWidth(finalVal.ToString("#0.00"), 10, false),

                    StringUtils.MakeFixedWidth(bestSecSymbol, 7, false), 
					StringUtils.MakeFixedWidth(allocShr.ToString("#00.00"), 10, false),
                    StringUtils.MakeFixedWidth("@" + price.ToString("#0.00"), 10, false)
					);
			}

		    foreach (var kvp in totalPercentByCategoryGroup)
		    {
		        Console.WriteLine("{0} Total: {1}", StringUtils.MakeFixedWidth(kvp.Key, 15), kvp.Value.ToString("#0.00"));  
		    }

            Console.ReadLine();
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

		private class Position
		{
			public string Security { get; set; }
			public decimal CurrentPrice { get; set; }

			public decimal OriginalQuantity { get; set; }

            public string Account { get; set; }

			public override string ToString()
			{
                return string.Format("{0} {1} @ {2} (acct={3})", Security, OriginalQuantity, CurrentPrice, Account);
			}
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

        static List<Position> ReadCsv(string inputCsv, IEnumerable<InvestmentCategory> categories)
		{
		    if (!File.Exists(inputCsv))
		        return new List<Position>();
			var rowList = new List<CsvRow>();
			using (var reader = new CsvFileReader(inputCsv))
			{
				var row = new CsvRow();
				while (reader.ReadRow(row))
				{
					if (row[1] != "Symbol") // skip header row
                        rowList.Add(row);
					row = new CsvRow();
				}
			}

			// remove rows which are not the securities we're interested in
            var categorizedSecurityRows = new List<CsvRow>();
            var uncategorizedSecurityRows = new List<CsvRow>();
		    var categorizedSecuritySymbols = categories.SelectMany(e => e.Securities.Select(e2 => e2.Symbol));
            rowList.ForEach(e =>
            {
                if (categorizedSecuritySymbols.Contains(e[1]))
                {
                    categorizedSecurityRows.Add(e);
                    return;
                }
                uncategorizedSecurityRows.Add(e);
                Console.WriteLine("Uncategorized security: {0}", e[1]);
            });
			var relevantSecurities = rowList.Where(e => categorizedSecuritySymbols.Contains(e[1])).ToList();

			var positions = relevantSecurities.Select(e => new Position
			{
				Security = e[1],
				OriginalQuantity = decimal.Parse(e[3]),
                Account = e[0].Trim(),
			}).ToList();

			var pricesFromSecurities = new Dictionary<string, decimal>();
			foreach (var row in relevantSecurities)
			{
				var security = row[1];
				if (pricesFromSecurities.ContainsKey(security))
					continue;
			    var priceString = row[4].Trim(new[] {'$'});
                var currentPrice = decimal.Parse(priceString);

				var positionsForSec = positions.Where(e => e.Security == security).ToList();
				positionsForSec.ForEach(e => e.CurrentPrice = currentPrice);
			}

			return positions;
		}
	}
}
