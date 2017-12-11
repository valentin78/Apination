using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Sage50Connector.Core
{
    class CompaniesListSection: IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var companiesList = new List<Company>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var name = childNode.Attributes["name"].InnerText;
                companiesList.Add(new Company() { Name = name });
            }
            return companiesList;
        }
    }

    public class Company
    {
        public string Name { get; set; }
    }
}
