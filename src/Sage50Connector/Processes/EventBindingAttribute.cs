using System;
using Sage50Connector.Models;

namespace Sage50Connector.Processes
{
    /// <summary>
    /// Event binding type attribute
    /// </summary>
    internal class EventBindingAttribute : Attribute
    {
        public EventBindingTypes Type { get; set; }
    }
}