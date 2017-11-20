namespace Sage50Connector.Models.Payloads
{
    public class PhoneNumber
    {
        public string Number { get; set; }
        /// <summary>
        /// one of: PhoneNumber1, PhoneNumber2, Fax1
        /// </summary>
        public string Key { get; set; }
    }
}