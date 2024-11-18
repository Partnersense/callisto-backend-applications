using System.Xml.Serialization;

namespace SharedLib.Models.Sage;

[XmlRoot("RESULT")]
public class SageStockResultDto
{
    [XmlElement("GRP")]
    public SageStockResultGroup Group { get; set; } = new();

    [XmlElement("TAB")] 
    public SageStockResultTable Table { get; set; } = new();
}


[XmlType("GRP")]
public class SageStockResultGroup
{
    [XmlAttribute("ID")]
    public string Id { get; set; } = string.Empty;

    [XmlElement("FLD")] 
    public List<SageStockResultField> Fields { get; set; } = [];
}

[XmlType("TAB")]
public class SageStockResultTable
{
    [XmlAttribute("DIM")]
    public int Dim { get; set; }

    [XmlAttribute("ID")]
    public string Id { get; set; } = string.Empty;

    [XmlAttribute("SIZE")]
    public int Size { get; set; }

    [XmlElement("LIN")]
    public List<SageStockResultLine> Lines { get; set; } = [];
}

[XmlType("LIN")]
public class SageStockResultLine
{
    [XmlAttribute("NUM")]
    public int Num { get; set; }

    [XmlElement("FLD")]
    public List<SageStockResultField> Fields { get; set; } = [];
}

[XmlType("FLD")]
public class SageStockResultField
{
    [XmlAttribute("NAME")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("TYPE")]
    public string Type { get; set; } = string.Empty;

    [XmlText]
    public string Value { get; set; } = string.Empty;
}