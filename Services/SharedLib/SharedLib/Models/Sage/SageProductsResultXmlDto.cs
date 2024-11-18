using System.Xml.Serialization;

namespace SharedLib.Models.Sage;

[XmlRoot(ElementName = "FLD")]
public class Field
{
    [XmlAttribute(AttributeName = "NAME")] 
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "TYPE")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public string Value { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "LIN")]
public class QueryLine
{
    [XmlAttribute(AttributeName = "NUM")]
    public int Number { get; set; }

    [XmlElement(ElementName = "FLD")] 
    public List<Field> Fields { get; set; } = [];
}

[XmlRoot(ElementName = "RESULT")]
public class QueryResult
{
    [XmlAttribute(AttributeName = "DIM")]
    public int Dimension { get; set; }

    [XmlAttribute(AttributeName = "SIZE")]
    public int Size { get; set; }

    [XmlElement(ElementName = "LIN")] 
    public List<QueryLine> Lines { get; set; } = [];

}

public class ResultXmlContainer
{
    [XmlElement(ElementName = "RESULT")] 
    public QueryResult QueryResult { get; set; } = new();
}

