namespace Sage50Connector.Models.Payloads
{
    public class TransactionLine
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Account Account { get; set; }
    }
}