using Enferno.Services.StormConnect.Contracts.Product.Models;
using SharedLib.Util;

namespace SharedLib.Mappers;
public static class NorceMapper
{
    public static ParametricValue ConstructParametricTextValue(string parametricCode, string value, string cultureCode)
    {
        return new ParametricValue
        {
            ParametricCode = parametricCode,
            Cultures = new List<ParametricValueCulture>
            {
                new()
                {
                    ValueText = value,
                    CultureCode = cultureCode,
                }
            }
        };
    }

    public static ParametricValue ConstructParametricListValue(string parametricCode, string valueCode, string cultureCode, string? value = "")
    {
        var parametric = new ParametricValue
        {
            ParametricCode = parametricCode,
            Cultures = new List<ParametricValueCulture>
            {
                new ()
                {
                    CultureCode = cultureCode
                }
            }
        };
        
        var valueList = new ParametricValueList
        {
            Code = valueCode,
        };

        if (!string.IsNullOrWhiteSpace(value))
        {
            valueList.Value = value;
        }
        parametric.ValueList = valueList;


        return parametric;
    }
}
