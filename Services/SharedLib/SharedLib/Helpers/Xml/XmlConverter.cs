using System.Text;
using System.Xml.Serialization;
using Serilog;

namespace SharedLib.Helpers.Xml;

public class XmlConverter : IXmlConverter
{

    public string SerializeToXml<T>(T obj)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriterWithEncoding(Encoding.UTF8);
        xmlSerializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }

    public T? DeserializeXml<T>(string xmlContent)
    {
        using var stringReader = new StringReader(xmlContent);
        var serializer = new XmlSerializer(typeof(T));
        try
        {
            return (T)serializer.Deserialize(stringReader)!;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Could not deserialize string to XML: {Msg}", ex.Message);
            return default;
        }
    }

}

public class StringWriterWithEncoding : StringWriter
{
    public override Encoding Encoding { get; }

    public StringWriterWithEncoding(Encoding encoding)
    {
        Encoding = encoding;
    }
}