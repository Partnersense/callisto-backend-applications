using System.Xml.Serialization;

namespace SharedLib.Models.Sage
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class RunEnvelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public RunBody Body { get; set; } = new();
    }

    [XmlType("Body")]
    public class RunBody
    {
        [XmlElement(ElementName = "runResponse", Namespace = "http://www.adonix.com/WSS")]
        public RunResponse Response { get; set; } = new();
    }

    [XmlRoot(ElementName = "runResponse", Namespace = "http://www.adonix.com/WSS")]
    public class RunResponse
    {
        [XmlElement(ElementName = "runReturn", Namespace = "")]
        public CAdxResultXml Return { get; set; } = new();

        [XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string EncodingStyle { get; set; } = string.Empty;
    }
}
