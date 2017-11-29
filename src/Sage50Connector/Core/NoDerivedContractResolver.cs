using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sage50Connector.Core
{
    /// <summary>
    /// Uses by Json Serializer for control ShouldSerialize Property
    /// </summary>
    public class NoDerivedContractResolver : DefaultContractResolver
    {
        private readonly Type _baseType;

        public NoDerivedContractResolver(Type baseType)
        {
            _baseType = baseType;
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = _ => property.DeclaringType == _baseType;
            return property;
        }
    }
}