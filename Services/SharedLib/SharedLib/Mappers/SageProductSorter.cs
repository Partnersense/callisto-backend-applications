using SharedLib.Constants;
using SharedLib.Models.Sage;

namespace SharedLib.Mappers;


public static class SageProductSorter
{
    /// <summary>
    /// Groups QueryLine objects by a derived product code, only including lines with relevant status codes.
    /// </summary>
    /// <param name="lines">The collection of QueryLine objects to group.</param>
    /// <returns>A collection of grouped lines by product code.</returns>
    public static IEnumerable<IGrouping<string, QueryLine>> GroupProducts(IEnumerable<QueryLine> lines)
    {
        return lines
            .Where(line => line.Fields.Exists(field => field.Name == SageConstants.Fields.ProductField.PartNo))
            .GroupBy(GetGroupingKey);
    }

    /// <summary>
    /// Retrieves the grouping key from a line based on the PartNo field.
    /// </summary>
    /// <param name="line">The line from which to extract the grouping key.</param>
    /// <returns>The derived product code used for grouping.</returns>
    private static string GetGroupingKey(QueryLine line)
    {
        var partNoField = line.Fields.FirstOrDefault(field => field.Name == SageConstants.Fields.ProductField.PartNo);
        return partNoField!.Value;
    }

    /// <summary>
    /// Processes the PartNo field to extract or derive a product code.
    /// </summary>
    /// <param name="partNo">The part number string from which to derive the product code.</param>
    /// <returns>The product code.</returns>
    private static string GetProductCodeFromPartNo(string partNo)
    {
        var parts = partNo.Split('-');
        if (parts.Length < 2)
        {
            return partNo; // Return the original part number if it does not contain a dash.
        }
        // Product code ending on "B" is a reference for "b-stock" product. Item still groups with other sibling items referenced for "a-stock" ( Without ending with "B").   
        var secondPart = parts[1].EndsWith("B") ? parts[1].Substring(0, parts[1].Length - 1) : parts[1];
        return $"{parts[0]}-{secondPart}";
    }
}
