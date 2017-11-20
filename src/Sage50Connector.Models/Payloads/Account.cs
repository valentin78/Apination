namespace Sage50Connector.Models.Payloads
{
    public class Account
    {
        public Account()
        {
            Classification = "Cash";
        }
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsInactive { get; set; }
        /// <summary>
        /// AccountClassification enum
        /// </summary>
        public string Classification { get; set; }
    }
}
