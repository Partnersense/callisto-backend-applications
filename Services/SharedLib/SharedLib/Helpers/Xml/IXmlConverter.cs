namespace SharedLib.Helpers.Xml;

public interface IXmlConverter
{
    public string SerializeToXml<T>(T obj);

    public T? DeserializeXml<T>(string xmlContent);

}