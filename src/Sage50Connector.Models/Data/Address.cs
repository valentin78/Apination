namespace Sage50Connector.Models.Data
{
    public class Address
    {
        public Address()
        {
            State = "";
        }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SalesTaxCode { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}