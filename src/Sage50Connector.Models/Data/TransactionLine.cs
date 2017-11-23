namespace Sage50Connector.Models.Data
{
    public class TransactionLine
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Account Account { get; set; }
    }
}