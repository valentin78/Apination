namespace Sage50Connector.Models.Data
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