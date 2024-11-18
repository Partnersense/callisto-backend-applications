using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;
public class JobModuleOptions
{
    [MinLength(1)]
    public required string JobId { get; init; }
    [MinLength(1)]
    public required string CronExpression { get; init; }
}
