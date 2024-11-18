using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SharedLib.Options.Models;
public class HangfireModuleOptions
{
    [Required] 
    public bool RunHangfireLocally { get; init; } = false;

    [Required]
    [MinLength(1)]
    public string Endpoint { get; init; }
    
    [Required]
    [MinLength(1)]
    public string ServerName { get; init; }
    
    [Required]
    [MinLength(1)]
    public string DashboardUsername { get; init; }
    
    [Required]
    [MinLength(1)]
    public string DashboardPassword { get; init; }

    
}
