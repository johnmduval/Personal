using PersonalBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceCommon
{
    public class PositionReader
    {
        public List<Position> CurrentPositions1 { get; set; }
        public List<Position> CurrentPositions2 { get; set; }
        public List<Position> AggregateCurrentPositions { get; set; }

        public decimal GetTaxableAccountsTotal()
        {
            var total = this.AggregateCurrentPositions
                .Where(e => _taxableAccounts.Contains(e.Account))
                .Sum(e => e.CurrentPrice * e.OriginalQuantity);
            return total;
        }

        public decimal GetPreTaxAccountsTotal()
        {
            var total = this.AggregateCurrentPositions
                .Where(e => _preTaxAccounts.Contains(e.Account))
                .Sum(e => e.CurrentPrice * e.OriginalQuantity);
            return total;
        }

        public decimal GetPostTaxAccountsTotal()
        {
            var total = this.AggregateCurrentPositions
                .Where(e => _postTaxAccounts.Contains(e.Account))
                .Sum(e => e.CurrentPrice * e.OriginalQuantity);
            return total;
        }

        private static readonly string inputCsv1 = @"C:\Users\John Duval\Downloads\john.csv";
        private static readonly string inputCsv2 = @"C:\Users\John Duval\Downloads\sue.csv";

        private List<string> _taxableAccounts = new List<string>
        {
            "X10830658"     // joint brokerage      (1.3m)
        };

        // pre-tax (e.g. traditional IRAs)
        private List<string> _preTaxAccounts = new List<string>
        {
            "130510777",    // john rollover IRA    (472)
            "210273953",    // john traditional IRA (75)
            "414351547",    // john simple IRA      (43)
            "130516597",    // sue rollover IRA     (24)
            "210273929",    // sue traditional IRA  (88)
            "24748",        // sue 401(k)           (534)
        };

        // post-tax (e.g. roth IRAs)
        private List<string> _postTaxAccounts = new List<string>
        {
            "130392103",    // john Roth IRA        (31)
            "216737438",    // sue Roth IRA         (49)
        };

        private List<Position> ReadCsv(string inputCsv, IEnumerable<InvestmentCategory> categories, string ignoreAccount = null)
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
            var categorizedSecurityRows = new List<CsvRow>();   // interested in these
            var uncategorizedSecurityRows = new List<CsvRow>(); // not interested (category unknown)
            var accountIgnoredRows = new List<CsvRow>();        // not interested (account ignored)
            var categorizedSecuritySymbols = categories.SelectMany(e => e.Securities.Select(e2 => e2.Symbol));
            rowList.ForEach(e =>
            {
                var account = e[0];
                var symbol = e[1];
                if (ignoreAccount != null && account == ignoreAccount)
                {
                    accountIgnoredRows.Add(e);
                    Console.WriteLine("Ignoring: file={0}, acct={1}, symbol={2}", inputCsv, account, symbol);
                    return;
                }

                if (!categorizedSecuritySymbols.Contains(symbol))
                {
                    uncategorizedSecurityRows.Add(e);
                    Console.WriteLine("Uncategorized security: {0}", symbol);
                    return;
                }
                categorizedSecurityRows.Add(e);
            });

            var positions = categorizedSecurityRows.Select(e => new Position
            {
                Security = e[1],
                OriginalQuantity = decimal.Parse(e[3]),
                Account = e[0].Trim(),
            }).ToList();

            var pricesFromSecurities = new Dictionary<string, decimal>();
            foreach (var row in categorizedSecurityRows)
            {
                var security = row[1];
                if (pricesFromSecurities.ContainsKey(security))
                    continue;
                var priceString = row[4].Trim(new[] { '$' });
                var currentPrice = decimal.Parse(priceString);

                var positionsForSec = positions.Where(e => e.Security == security).ToList();
                positionsForSec.ForEach(e => e.CurrentPrice = currentPrice);
            }

            return positions;
        }

        private void DownloadInputFiles()
        {
            //var requestUri =
            //    "https://oltx.fidelity.com/ftgw/fbc/ofpositions/snippet/portfolioPositions?ALL_ACCTS=Y&SAVE_SETTINGS_WASH_SALE=N&UNADJUSTED_COST_BASIS_INFORMATION=&EXCLUDE_WASH_SALE_IND=&SHOW_FOREIGN_CURRENCY=&REFRESH_DATA=N&REPRICE_FROM_CACHE=Y&ALL_POS=Y&ALL_ACCTS=Y&TXN_SORT_ORDER=0&TABLE_SORT_ORDER=0&TABLE_SORT_DIRECTION=A&SAVE_SETTINGS=N&pf=N&CSV=Y&TXN_COLUMN_SORT_JSON_INFO=&SORT_COL_IND=&IS_ACCOUNT_CHANGED=Y&DISP_FULL_DESC=Y&FONT_SIZE=S&viewBy=&displayBy=&group-by=0&desc=0&NEXTGEN=Y&ACTION=&SHOW_FULL_SECURITY_NAME=N&REQUESTED_SHOW_TYPE_IND=All&REQUESTED_SHOW_TYPE_IND=Mutual+Funds&REQUESTED_SHOW_TYPE_IND=CIT%2F529&REQUESTED_SHOW_TYPE_IND=Stocks%2FETFs";
            //var httpClient = new HttpClient();
            //var ret = httpClient.GetAsync(requestUri).ConfigureAwait(false);
            //var x = ret.Wait();
        }

        public void Load()
        {
            DownloadInputFiles();

            // Read all current security positions from CSVs, combine into list of positions, grouped by security symbol
            if (!File.Exists(inputCsv1) || !File.Exists(inputCsv2))
            {
                Console.WriteLine("Input file(s) not found");
                Console.ReadLine();
                return;
            }
            this.CurrentPositions1 = ReadCsv(inputCsv1, InvestmentCategoryTargets.Instance.Targets);
            this.CurrentPositions2 = ReadCsv(inputCsv2, InvestmentCategoryTargets.Instance.Targets, "X10830658");

            this.AggregateCurrentPositions = this.CurrentPositions1.Union(this.CurrentPositions2).ToList();

        }
    }
}
