using System.Xml.Serialization;

namespace SharedLib.Models.Sage;

//xml schema https://github.com/Sage-ERP-X3/soap-tester-client/blob/master/SageX3WebServiceClient/Web%20References/CAdxWebServiceXmlCC/CAdxWebServiceXmlCC.wsdl
[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class Envelope
{
    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public Body Body { get; set; } = new();
}

public class Body
{
    [XmlElement(ElementName = "queryResponse", Namespace = "http://www.adonix.com/WSS")]
    public QueryResponse QueryResponse { get; set; } = new();
}

[XmlRoot(ElementName = "queryResponse", Namespace = "http://www.adonix.com/WSS")]
public class QueryResponse
{
    [XmlElement(ElementName = "queryReturn", Namespace = "")]
    public CAdxResultXml QueryReturn { get; set; } = new();

    [XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public string EncodingStyle { get; set; } = string.Empty;
}

[XmlInclude(typeof(CAdxResultXml))]
[XmlRoot(Namespace = "http://www.adonix.com/WSS", ElementName = "CAdxResultXml")]
public class CAdxResultXml
{
    [XmlArray(ElementName = "messages", Namespace = "")]
    [XmlArrayItem(ElementName = "CAdxMessage", Namespace = "http://www.adonix.com/WSS")]
    public List<CAdxMessage> Messages { get; set; } =[];

    [XmlElement(ElementName = "resultXml", Namespace = "")]
    public string? ResultXml { get; set; } 

    [XmlElement(ElementName = "status", Namespace = "")]
    public int Status { get; set; }

    [XmlElement(ElementName = "technicalInfos", Namespace = "")]
    public CAdxTechnicalInfos TechnicalInfos { get; set; } = new();
}

[XmlRoot(ElementName = "CAdxMessage", Namespace = "http://www.adonix.com/WSS")]
public class CAdxMessage
{
    [XmlElement(ElementName = "type", Namespace = "http://www.adonix.com/WSS")]
    public string Type { get; set; } = string.Empty;

    [XmlElement(ElementName = "message", Namespace = "http://www.adonix.com/WSS")]
    public string Message { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "status", Namespace = "")]
public class Status
{
    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public int Text { get; set; }
}

public class BooleanAttribute
{
    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public bool Text { get; set; }
}

public class StringAttribute
{
    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public bool Text { get; set; }
}

public class IntAttribute
{
    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public bool Text { get; set; }
}
public class DoubleAttribute
{
    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public bool Text { get; set; }
}

[XmlRoot(Namespace = "http://www.adonix.com/WSS", ElementName = "CAdxTechnicalInfos")]
public class CAdxTechnicalInfos
{
    [XmlElement(ElementName = "busy")]
    public BooleanAttribute Busy { get; set; } = new();

    [XmlElement(ElementName = "changeLanguage")]
    public BooleanAttribute ChangeLanguage { get; set; } = new();

    [XmlElement(ElementName = "changeUserId")]
    public BooleanAttribute ChangeUserId { get; set; } = new();

    [XmlElement(ElementName = "flushAdx")]
    public BooleanAttribute FlushAdx { get; set; } = new();

    [XmlElement(ElementName = "loadWebsDuration")]
    public DoubleAttribute LoadWebsDuration { get; set; } = new();

    [XmlElement(ElementName = "nbDistributionCycle")]
    public IntAttribute NbDistributionCycle { get; set; } = new();

    [XmlElement(ElementName = "poolDistribDuration")]
    public DoubleAttribute PoolDistribDuration { get; set; } = new();

    [XmlElement(ElementName = "poolEntryIdx")]
    public IntAttribute PoolEntryIdx { get; set; } = new();

    [XmlElement(ElementName = "poolExecDuration")]
    public DoubleAttribute PoolExecDuration { get; set; } = new();

    [XmlElement(ElementName = "poolRequestDuration")]
    public DoubleAttribute PoolRequestDuration { get; set; } = new();

    [XmlElement(ElementName = "poolWaitDuration")]
    public DoubleAttribute PoolWaitDuration { get; set; } = new();

    [XmlElement(ElementName = "processReport", IsNullable = true)]
    public string? ProcessReport { get; set; }

    [XmlElement(ElementName = "processReportSize")]
    public int ProcessReportSize { get; set; } = new();

    [XmlElement(ElementName = "reloadWebs")]
    public bool ReloadWebs { get; set; } = new();

    [XmlElement(ElementName = "resumitAfterDBOpen")]
    public bool ResumitAfterDBOpen { get; set; }

    [XmlElement(ElementName = "rowInDistribStack", IsNullable = true)]
    public int? RowInDistribStack { get; set; }

    [XmlElement(ElementName = "totalDuration")]
    public double TotalDuration { get; set; }

    [XmlElement(ElementName = "traceRequest")]
    public string TraceRequest { get; set; } = string.Empty;

    [XmlElement(ElementName = "traceRequestSize")]
    public int TraceRequestSize { get; set; }

    [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Type { get; set; } = string.Empty;
}
