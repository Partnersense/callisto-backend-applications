using SharedLib.Models.Sage;

namespace SharedLib.Mappers;
public static class SageMapperTools
{
    public static string? GetFieldByName(this List<Field> fields, string name)
    {
        return fields.Find(f => f.Name.Equals(name))?.Value;
    }

    public static string? GetFieldByName(this List<SageStockResultField> fields, string name)
    {
        return fields.Find(f => f.Name.Equals(name))?.Value;
    }
}
