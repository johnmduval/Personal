using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceCommon
{
    // tax efficiency:  https://www.bogleheads.org/wiki/Principles_of_tax-efficient_fund_placement
    // ___Efficient___ (Place anywhere)
    //Low-yield money market, cash, short-term bond funds
    //Tax-managed stock funds
    //Large-cap and total-market stock index funds
    //Balanced index funds
    //Small-cap or mid-cap index funds
    //Value index funds

    //___Moderately inefficient___
    //Moderate-yield money market, bond funds
    //Total-market bond funds
    //Active stock funds

    //___Very inefficient___  (Place in Tax-Free or Tax-Deferred)
    //Real estate or REIT funds
    //High-turnover active funds
    //High-yield corporate bonds
    public class InvestmentCategoryTargets
    {
        public List<InvestmentCategory> Targets { get; private set; }

        public static InvestmentCategoryTargets Instance { get { return lazy.Value; } }
        private static readonly Lazy<InvestmentCategoryTargets> lazy = new Lazy<InvestmentCategoryTargets>(() => new InvestmentCategoryTargets());
        private InvestmentCategoryTargets()
        {
            this.Targets = new List<InvestmentCategory>
            {
                // U.S. stocks
		        new InvestmentCategory { Name = "1 US Large Cap", TargetPercent = 15.0m, TaxEfficiency = 3,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "SPY", ExpenseRatio = 0.09m, Description = "S&P 500"},
                        //new SecurityAndExpenseRatio {Symbol = "PEOPX", ExpenseRatio = 0.50m, Description = "S&P 500", Eliminate = true },
                        //new SecurityAndExpenseRatio {Symbol = "FBGRX", ExpenseRatio = 0.74m, Description = "Blue Chip", Eliminate = true },
		                new SecurityAndExpenseRatio {Symbol = "FUSVX", ExpenseRatio = 0.05m, Description = "S&P 500 80%"}, // Sue 401k
                        new SecurityAndExpenseRatio {Symbol = "FXAIX", ExpenseRatio = 0.015m, Description = "S&P 500 80%"},
                    }
                },
                new InvestmentCategory { Name = "2 US Broad Mkt", TargetPercent = 15.0m, TaxEfficiency = 3,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "IWV", ExpenseRatio = 0.20m, Description = "Russell 3000", Eliminate = true },
                        new SecurityAndExpenseRatio {Symbol = "FSEVX", ExpenseRatio = 0.07m, Description = "Mid / Small Cap"},
                        new SecurityAndExpenseRatio {Symbol = "FSMAX", ExpenseRatio = 0.045m, Description = "Mid / Small Cap"},
                    }
                },
                new InvestmentCategory { Name = "3 US Small Cap", TargetPercent = 15.0m, TaxEfficiency = 4,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "IJR", ExpenseRatio = 0.12m, Description = "S&P SmallCap 600"},
                    }
                },
                new InvestmentCategory { Name = "4 US Real Estate", TargetPercent = 7.0m, TaxEfficiency = 2,
                    Securities = new[]
                    { 
                        //new SecurityAndExpenseRatio {Symbol = "IYR", ExpenseRatio = 0.46m, Description = "Dow Jones U.S. Real Estate Index", Eliminate = true},
		                new SecurityAndExpenseRatio {Symbol = "SCHH", ExpenseRatio = 0.07m, Description = "U.S. Select REIT Index"},
                    }
                },

                // International stocks
                new InvestmentCategory { Name = "5 Intl Large/Mid Cap", TargetPercent = 15.0m, TaxEfficiency = 5,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "ACWX", ExpenseRatio = 0.34m, Description = "All Country World Index (ACWI)", Eliminate = true },
                        new SecurityAndExpenseRatio {Symbol = "VEA", ExpenseRatio = 0.09m, Description = "FTSE Developed ex North America Index" },
                        new SecurityAndExpenseRatio {Symbol = "VTMGX", ExpenseRatio = 0.09m, Description = "FTSE Developed ex North America Index"},    // Sue 401k
                    }
                },
                new InvestmentCategory { Name = "6 Intl Small Cap", TargetPercent = 7.0m, TaxEfficiency = 5,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "SCZ", ExpenseRatio = 0.40m, Description = "Europe Asia Far East (EAFE) Small Cap"},
                    }
                },
                new InvestmentCategory { Name = "7 Intl BRIC", TargetPercent = 7.0m, TaxEfficiency = 5,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "VEIEX", ExpenseRatio = 0.33m, Description = "Brazil Russia India China"},
                        new SecurityAndExpenseRatio {Symbol = "VEMAX", ExpenseRatio = 0.14m, Description = "China Taiwan India Brazil SAfrica Russia "},
                        //new SecurityAndExpenseRatio {Symbol = "BKF", ExpenseRatio = 0.67m, Description = "Brazil Russia India China", Eliminate = true }, 
                    }
                },

                // U.S. bonds
                new InvestmentCategory { Name = "8 Bonds", TargetPercent = 13.0m, TaxEfficiency = 1,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "AGG", ExpenseRatio = 0.08m, Description = "iShares Core US Aggregate Bond"},
                        new SecurityAndExpenseRatio {Symbol = "FSITX", ExpenseRatio = 0.10m, Description = "Fidelity Spartan US Bond Idx Advtg"}, // Sue 401k
                        new SecurityAndExpenseRatio {Symbol = "FXNAX", ExpenseRatio = 0.025m, Description = "Bloomberg Barclays U.S. Aggregate Bond Index"},
                    }
                },
                //new InvestmentCategory { Name = "8a High-Yield Bonds", TargetPercent = 5.0m, TaxEfficiency = 1,
                //    Securities = new[] { new SecurityAndExpenseRatio {Symbol = "???", ExpenseRatio = 0.00m, Description = "Barclays U.S. Aggregate Bond Index"},
                //    }
                //},
                new InvestmentCategory { Name = "9 Inflation Protected", TargetPercent = 6.0m, TaxEfficiency = 1,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "TIP", ExpenseRatio = 0.20m, Description = "U.S. Treasury Inflation Protected Securities"},
                    }
                },

                // Other
		        new InvestmentCategory { Name = "9a Money Market/Cash", TargetPercent = 0.0m, TaxEfficiency = 5,
                    Securities = new[]
                    {
                        new SecurityAndExpenseRatio {Symbol = "SPAXX", ExpenseRatio = 0.42m, Description = "Government Money Market Fund"},
                        new SecurityAndExpenseRatio {Symbol = "FTEXX", ExpenseRatio = 0.4m, Description = "Municipal Money Market"},
                        new SecurityAndExpenseRatio {Symbol = "FDRXX", ExpenseRatio = 0.24m, Description = "Cash Reserves"},
                        new SecurityAndExpenseRatio {Symbol = "CORE", ExpenseRatio = 0.00M, Description = "FDIC-INSURED DEPOSIT SWEEP"},
                    }
                },
            };
        }

    }

    public class InvestmentCategory
    {
        public string Name { get; set; }
        public int TaxEfficiency { get; set; }  // lower (1) = not efficient, higher (5) = more efficient
        public decimal TargetPercent { get; set; }
        public IEnumerable<SecurityAndExpenseRatio> Securities { get; set; }
    }
}
