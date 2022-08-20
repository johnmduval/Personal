namespace Projections
{
    public class AccountInfo
    {
        public string Desc { get; set; }
        public decimal StartingValue { get; set; }
        public double AnnualReturnPercent { get; set; }
        public decimal AnnualContributions { get; set; }
        public decimal CostBasis { get; set; }
        public decimal CurrentValue { get; set; }
        public double TaxRate { get; set; }
    }
}