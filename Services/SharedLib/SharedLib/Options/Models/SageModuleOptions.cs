using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;
public class SageModuleOptions
{
    [MinLength(1)]
    public required string BaseUrl { get; set; }
    [MinLength(1)]
    public required string Username { get; set; }
    [MinLength(1)]
    public required string Password { get; set; }
    [MinLength(1)]
    public required string Token { get; set; }
    [MinLength(1)]
    public required string PoolAlias { get; set; }
}
