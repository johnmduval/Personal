using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PortBalance
{


	public class PortBalance3
	{
		public static void Go()
		{
            var inputCsv = @"C:\Users\admin\Downloads\Portfolio_Position_Jun-25-2014.csv";

            //var newInvestmentAmount = 20944.0m + 799.0m + 1879.0m + 1907.0m + 3066.0m;
            var newInvestmentAmount = 12200.0m;

			var targetPercents = new Dictionary<string, decimal>()
			{
				{ "SPY", 20.0m },	// switch some to FHKCX?
				{ "IWV", 28.0m },   //
				{ "IJR", 12.0m },   //
				{ "IYR", 5.0m },    //
				{ "ACWX", 10.0m },  //
				{ "SCZ", 5.0m },    //
				{ "BKF", 10.0m },	// 
				{ "AGG", 5.0m },	// 
				{ "TIP", 5.0m },	// 
			};
			var currentPositions = ReadCsv(inputCsv, targetPercents);
			var currentPositionsBySecurity = currentPositions.GroupBy(e => e.Security);

			// list of (security, original value, price)
			var valuesBySecurity = currentPositionsBySecurity
				.Select(e => new { Security = e.Key, OriginalValue = e.Sum(e2 => e2.OriginalQuantity * e2.CurrentPrice), CurrentPrice = e.First().CurrentPrice })
				.ToList();
			
			// dictionary for security -> price
			var priceBySecurity = currentPositionsBySecurity
				.ToDictionary(e => e.Key, e => e.First().CurrentPrice);

			var dollarsAllocatedToSecurity = currentPositionsBySecurity.Select(e => new SecurityAndAmount(e.Key, 0.0m)).ToDictionary(e => e.Security, e => e.Amount);
			
			
#if !ITERATIVE
			while (newInvestmentAmount > 0.0m)
			{
				var totalOriginalValueAllSecurities =
					valuesBySecurity.Sum(e => e.OriginalValue) +	// original starting values
					dollarsAllocatedToSecurity.Sum(e => e.Value);	// allocated

				var percentBySecurity = valuesBySecurity
					.Select(e => 
					{
						var totalForSecurity = e.OriginalValue + dollarsAllocatedToSecurity[e.Security];
						return new { Security = e.Security, Percent = totalForSecurity / totalOriginalValueAllSecurities * 100.0m, TargetPercent = targetPercents[e.Security] };
					})
					.ToList();

				var pctDiffBySecurity = percentBySecurity
					.Select(e => new { Security = e.Security, Diff = e.TargetPercent - e.Percent })
					.ToList();

				var dollarDiffBySecurity = pctDiffBySecurity.Select(e => new SecurityAndAmount(e.Security, e.Diff / 100.0m * totalOriginalValueAllSecurities)).ToList();

				// get $1000 or whatever's left to invest
				var slice = Math.Min(1000, newInvestmentAmount);	
				newInvestmentAmount -= slice;

				var securityNeedsMost = dollarDiffBySecurity.OrderByDescending(e => e.Amount).First();
				var security = securityNeedsMost.Security;
//				slice = Math.Min(securityNeedsMost.Amount, slice);

				securityNeedsMost.Amount = securityNeedsMost.Amount - slice;
				dollarsAllocatedToSecurity[security] += slice;
			}
#else

			// find the security which is closest to its target (or the most over its target %)
			// use this security as the baseline -- we won't buy or sell any of these
			var baselineSecurity = dollarDiffBySecurity.OrderBy(e => e.DollarDiff).First();

			// compute what the total would look like if this security were the correct #
			var baselineSecurityPercent = targetPercents[baselineSecurity.Security];
			var baselineSecurityValue = valuesBySecurity.Where(e => e.Security == baselineSecurity.Security).Single().OriginalValue;

			var baselineTotalValue = baselineSecurityValue / (baselineSecurityPercent / 100.0m);

			var targetValues = targetPercents.Select(e => new { Security = e.Key, Percent = e.Value, TargetValue = e.Value / 100.0m * baselineTotalValue });

			var targetAndOriginalValues = targetValues.Select(e => new { Security = e.Security, TargetValue = e.TargetValue, OriginalValue = valuesBySecurity.First(e2 => e2.Security == e.Security).OriginalValue });
			var valueDifferences = targetAndOriginalValues.Select(e => new { Security = e.Security, ValueDiff = e.TargetValue - e.OriginalValue });

			var totalDifferences = valueDifferences.Sum(e => e.ValueDiff);

			var percentOfNewInvestmentBySecurity = valueDifferences.Select(e => new { Security = e.Security, Percent = e.ValueDiff / totalDifferences });

			var investmentAmountBySecurity = percentOfNewInvestmentBySecurity.Select(e => new { Security = e.Security, InvestmentAmount = e.Percent * newInvestmentAmount});

			var investmentSharesBySecurity = investmentAmountBySecurity.Select(e => new { Security = e.Security, Shares = e.InvestmentAmount / priceBySecurity[e.Security] });
#endif

			var startingTotal = valuesBySecurity.Sum(e => e.OriginalValue);
			var allocatedTotal = dollarsAllocatedToSecurity.Sum(e => e.Value);

			Console.WriteLine("startingTotal=" + startingTotal);
			Console.WriteLine("allocatedTotal=" + allocatedTotal);

			Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
				StringUtils.MakeFixedWidth("Sec", 5, false),
				StringUtils.MakeFixedWidth("TargetPct", 10, false),
				StringUtils.MakeFixedWidth("Price", 10, false),

				StringUtils.MakeFixedWidth("OrigVal", 10, false),
				StringUtils.MakeFixedWidth("OrigPct", 10, false),

				StringUtils.MakeFixedWidth("Alloc$", 10, false),
				StringUtils.MakeFixedWidth("AllocShr", 10, false),

				StringUtils.MakeFixedWidth("FinalVal", 10, false),
				StringUtils.MakeFixedWidth("FinalPct", 10, false)			
				);

			foreach (var values in valuesBySecurity)
			{
				var sec = values.Security;
				var startVals = valuesBySecurity.Where(e => e.Security == sec).Single();
				var startPct = startVals.OriginalValue / startingTotal;
				var price = priceBySecurity[sec];

				var allocVal = dollarsAllocatedToSecurity[sec];
				var allocShr = allocVal / priceBySecurity[sec];

				var finalVal = startVals.OriginalValue + allocVal;
				var finalPct = finalVal / (startingTotal + allocatedTotal);

				Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}", 
					StringUtils.MakeFixedWidth(sec, 5, false),
					StringUtils.MakeFixedWidth(targetPercents[sec].ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(price.ToString("#0.00"), 10, false),
					
					StringUtils.MakeFixedWidth(startVals.OriginalValue.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(startPct.ToString("#0.000"), 10, false),
					
					StringUtils.MakeFixedWidth(allocVal.ToString("#00.00"), 10, false),
					StringUtils.MakeFixedWidth(allocShr.ToString("#00.00"), 10, false),

					StringUtils.MakeFixedWidth(finalVal.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(finalPct.ToString("#0.000"), 10, false)
					);
			}
			Console.ReadLine();
		}

		private class Position
		{
			public string Security { get; set; }
			public decimal CurrentPrice { get; set; }

			public decimal OriginalQuantity { get; set; }
			public decimal TargetQuantity { get; set; }

			public decimal OriginalValue { get { return this.OriginalQuantity * this.CurrentPrice; } }
			public decimal TargetValue { get { return this.TargetQuantity * this.CurrentPrice; } }

			public override string ToString()
			{
				return string.Format("{0} {1} @ {2} (tgt={3})", Security, OriginalQuantity, CurrentPrice, TargetQuantity);
			}
		}
		
		
		static IEnumerable<Position> ReadCsv(string inputCsv, Dictionary<string, decimal> targetPercents)
		{
		    if (!File.Exists(inputCsv))
		        return new Position[] {};
			var rowList = new List<CsvRow>();
			using (var reader = new CsvFileReader(inputCsv))
			{
				var row = new CsvRow();
				while (reader.ReadRow(row))
				{
					rowList.Add(row);
					row = new CsvRow();
				}
			}

			// remove rows which are not the securities we're interested in
			var relevantSecurities = rowList.Where(e => targetPercents.Keys.Contains(e[1])).ToList();

			var positions = relevantSecurities.Select(e => new Position
			{
				Security = e[1],
				OriginalQuantity = decimal.Parse(e[3]),
			}).ToList();

			var _pricesFromSecurities = new Dictionary<string, decimal>();
			foreach (var row in relevantSecurities)
			{
				var security = row[1];
				if (_pricesFromSecurities.ContainsKey(security))
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
