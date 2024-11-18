using System.Xml.Serialization;

namespace SharedLib.Models.Sage;

public class SageRequestDto
{
    [XmlRoot("query", Namespace = "http://www.adonix.com/WSS")]
    public class Query
    {
        [XmlElement("callContext", Type = typeof(CAdxCallContext))]
        public CAdxCallContext CallContext { get; set; } = new();

        [XmlElement("publicName")]
        public string PublicName { get; set; } = string.Empty;

        [XmlElement("objectKeys", Type = typeof(ArrayOfCAdxParamKeyValue))]
        public ArrayOfCAdxParamKeyValue ObjectKeys { get; set; } = new();

        [XmlElement("listSize")]
        public int ListSize { get; set; }
    }

    [XmlType("CAdxCallContext", Namespace = "http://www.adonix.com/WSS")]
    public class CAdxCallContext
    {
        [XmlElement("codeLang")]
        public string CodeLang { get; set; } = string.Empty;

        [XmlElement("poolAlias")]
        public string PoolAlias { get; set; } = string.Empty;

        [XmlElement("poolId")]
        public string PoolId { get; set; } = string.Empty;

        [XmlElement("requestConfig")]
        public string RequestConfig { get; set; } = string.Empty;
    }

    [XmlType("ArrayOfCAdxParamKeyValue", Namespace = "http://www.adonix.com/WSS")]
    public class ArrayOfCAdxParamKeyValue
    {
        [XmlElement("ArrayOfCAdxParamKeyValue")]
        public List<CAdxParamKeyValue> Items { get; set; } = [];
    }

    [XmlType("CAdxParamKeyValue", Namespace = "http://www.adonix.com/WSS")]
    public class CAdxParamKeyValue
    {
        [XmlElement("key")]
        public string Key { get; set; } = string.Empty;

        [XmlElement("value")]
        public string Value { get; set; } = string.Empty;
    }
}