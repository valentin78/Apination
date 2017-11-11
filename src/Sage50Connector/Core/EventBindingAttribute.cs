using System;
using Sage50Connector.Models;

namespace Sage50Connector.Core
{
    /// <summary>
    /// Event binding type attribute
    /// </summary>
    internal class EventBindingAttribute : Attribute
    {
        public byte Type { get; set; }
    }
}