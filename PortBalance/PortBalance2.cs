using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortBalance
{
	public class PortBalance2
	{
		public static void Go()
		{
			var inputCsv = @"C:\Users\jduval\Downloads\Portfolio_Position_Dec-31-2013.csv";

			var newInvestmentAmount =
				10314.73m + 100000.0m + // joint 
				713.82m + // roth 
				3114.49m + // roll 
				1701.92m + // trad 
				6503.14m;				// simp 

			var targetPercents = new Dictionary<string, decimal>()
			{
				{ "SPY", 25.0m },
				{ "IWV", 25.0m },
				{ "IJR", 10.0m },
				{ "IYR", 5.0m },
				{ "ACWX", 10.0m },
				{ "SCZ", 5.0m },
				{ "BKF", 10.0m },
				{ "AGG", 5.0m },
				{ "TIP", 5.0m },
			};
			var currentPositions = ReadCsv(inputCsv, targetPercents);
			var currentPositionsBySecurity = currentPositions.GroupBy(e => e.Security);

			// get Security and OriginalValue
			var valuesBySecurity = currentPositionsBySecurity
				.Select(e => new { Security = e.Key, OriginalValue = e.Sum(e2 => e2.OriginalQuantity * e2.CurrentPrice), CurrentPrice = e.First().CurrentPrice })
				.ToList();
			var originalValuesBySecurity = valuesBySecurity
				.ToDictionary(e => e.Security, e => e.OriginalValue);

			var priceBySecurity = currentPositionsBySecurity
				.ToDictionary(e => e.Key, e => e.First().CurrentPrice);

			var totalOriginalValueAllSecurities = valuesBySecurity.Sum(e => e.OriginalValue);
			var percentBySecurity = valuesBySecurity
				.Select(e => new { Security = e.Security, Percent = e.OriginalValue / totalOriginalValueAllSecurities * 100.0m, TargetPercent = targetPercents[e.Security] })
				.ToList();

			var pctDiffBySecurity = percentBySecurity
				.Select(e => new { Security = e.Security, Diff = e.TargetPercent - e.Percent })
				.ToList();

			var dollarDiffBySecurity = pctDiffBySecurity.Select(e => new { Security = e.Security, DollarDiff = e.Diff / 100.0m * totalOriginalValueAllSecurities });
			
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

			Console.WriteLine("totalOriginalValueAllSecurities=" + totalOriginalValueAllSecurities);
			Console.WriteLine("baselineTotalValue=" + baselineTotalValue);

			Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
				StringUtils.MakeFixedWidth("Sec", 5, false),
				StringUtils.MakeFixedWidth("OrigVal", 10, false),
				StringUtils.MakeFixedWidth("CurrentPx", 10, false),
				StringUtils.MakeFixedWidth("Percent", 10, false),
				StringUtils.MakeFixedWidth("TargetPct", 10, false),
				StringUtils.MakeFixedWidth("Baseline", 10, false),
				StringUtils.MakeFixedWidth("TargetVal", 10, false),
				StringUtils.MakeFixedWidth("PctOfNew", 10, false),
				StringUtils.MakeFixedWidth("InvAmt", 10, false),
				StringUtils.MakeFixedWidth("InvShares", 10, false)
				);

			foreach (var values in valuesBySecurity)
			{
				var a = percentBySecurity.Where(e => e.Security == values.Security).Single();
				var isBaseline = baselineSecurity.Security == values.Security;
				var b = targetValues.Where(e => e.Security == values.Security).Single();
				var c = percentOfNewInvestmentBySecurity.Where(e => e.Security == values.Security).Single();
				var d = investmentAmountBySecurity.Where(e => e.Security == values.Security).Single();
				var f = investmentSharesBySecurity.Where(e => e.Security == values.Security).Single();

				Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", 
					StringUtils.MakeFixedWidth(values.Security, 5, false),
					StringUtils.MakeFixedWidth(values.OriginalValue.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(values.CurrentPrice.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(a.Percent.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(a.TargetPercent.ToString("#00.00"), 10, false),
					StringUtils.MakeFixedWidth(isBaseline.ToString(), 10, false),
					StringUtils.MakeFixedWidth(b.TargetValue.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(c.Percent.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(d.InvestmentAmount.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(f.Shares.ToString("#0.00"), 10, false)
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
				var currentPrice = decimal.Parse(row[4]);

				var positionsForSec = positions.Where(e => e.Security == security).ToList();
				positionsForSec.ForEach(e => e.CurrentPrice = currentPrice);
			}

			return positions;
		}
	}
}
