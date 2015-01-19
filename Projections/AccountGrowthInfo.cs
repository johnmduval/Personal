namespace Projections
{
    public class AccountGrowthInfo
    {
        public string Desc { get; set; }
        public decimal StartingValue { get; set; }
        public double AnnualReturnPercent { get; set; }
        public decimal AnnualContributions { get; set; }
        public decimal ProjectionValue { get; set; }
    }
}