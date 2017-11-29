using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sage50Connector.Core
{
    /// <summary>
    /// Uses by Json Serializer for control ShouldSerialize Property excluding payload
    /// </summary>
    public class NoPayloadContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = _ => property.PropertyName != "payload";
            return property;
        }
    }
}