
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortBalance
{
	class Program
	{
		private static Dictionary<string, decimal> _targetPercents = new Dictionary<string, decimal>()
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
		//37,536.62
		//57,091.26
		//94627.88
		private static Dictionary<string, decimal> _simulatedOrderValues = new Dictionary<string, decimal>()
		{
			{ "SPY", 0m },
			{ "IWV", 0m },
			{ "IJR", 0m },
			{ "IYR", 0m },
			{ "ACWX", 0m },
			{ "SCZ", 0m },
			{ "BKF", 0m },
			{ "AGG", 0m },
			{ "TIP", 0m },
		};

		private static Dictionary<string, decimal> _pricesFromSecurities = new Dictionary<string, decimal>();

		private class Position
		{
			public string Security;
			public decimal Quantity;
			public override string ToString()
			{
				return Quantity + " " + Security;
			}
		}

		static IEnumerable<Position> ReadCsv()
		{
            var inputCsv = @"C:\Users\admin\Downloads\Portfolio_Position_May-30-2014.csv";

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

			// remove "bad" rows
			var skipOther = rowList.Where(e => _targetPercents.Keys.Contains(e[1])).ToList();

			var positions = skipOther.Select(e => new Position {
				Security = e[1], 
				Quantity = decimal.Parse(e[3]), 
			}).ToList();

			_pricesFromSecurities = new Dictionary<string, decimal>();
			foreach (var row in skipOther)
			{
				if (_pricesFromSecurities.ContainsKey(row[1]))
					continue;
				_pricesFromSecurities[row[1]] = decimal.Parse(row[4]);
			}

			return positions;
		}

		static IEnumerable<Position> SimulateOrders(IEnumerable<Position> positions)
		{
			var posList = positions.ToList();
			foreach (var order in _simulatedOrderValues)
			{
				if (order.Value == 0.0m)
					continue;
				var qty = order.Value / _pricesFromSecurities[order.Key];
				posList.Add(new Position { Security = order.Key, Quantity = qty });
			}
			return posList;
		}


		static void Main(string[] args)
		{
			PortBalance4.Go();
			return;

			var positions2 = ReadCsv();

			var simPositions = SimulateOrders(positions2);

			var groupedBySecurity = simPositions.GroupBy(e => e.Security);
			var secsAndValues = groupedBySecurity.Select(e => new { Security = e.Key, Value = e.Sum(e2 => e2.Quantity * _pricesFromSecurities[e.Key]) });
			//var priceFromSec = groupedBySecurity.ToDictionary(e => e.Key, e => e.Average(e2 => e2.Price));

			decimal total = secsAndValues.Sum(e => e.Value);

			Console.WriteLine("{0}\t${1}\t{2}%\t{3}%\t{4}%\t{5}sh @ ${6}",
				StringUtils.MakeFixedWidth("Security", 15, false),
				StringUtils.MakeFixedWidth("secValue", 10, false),
				StringUtils.MakeFixedWidth("pct", 10, false),
				StringUtils.MakeFixedWidth("targetPct", 10, false),
				StringUtils.MakeFixedWidth("diffPct", 10, false),
				StringUtils.MakeFixedWidth("sharesDiff", 10, false),
				StringUtils.MakeFixedWidth("price", 10, false)
				);

			foreach (var secValue in secsAndValues)
			{
				decimal targetPct;
				if (!_targetPercents.TryGetValue(secValue.Security, out targetPct))
					targetPct = 0.0m;
				var pct = secValue.Value / total * 100.0m;
				var diffPct = targetPct - pct;

				var price = _pricesFromSecurities[secValue.Security];
				var valueDiff = total * diffPct / 100.0m;
				var sharesDiff = valueDiff / price;

				Console.WriteLine("{0}\t${1}\t{2}%\t{3}%\t{4}%\t{5}sh @ ${6}", 
					StringUtils.MakeFixedWidth(secValue.Security, 15, false),
					StringUtils.MakeFixedWidth(secValue.Value.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(pct.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(targetPct.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(diffPct.ToString("#00.00"), 10, false),
					StringUtils.MakeFixedWidth(sharesDiff.ToString("#0.00"), 10, false),
					StringUtils.MakeFixedWidth(price.ToString("#0.00"), 10, false)
					);
			}

			Console.ReadLine();
		}
	}
}
