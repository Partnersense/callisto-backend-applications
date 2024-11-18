using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;
public class NorceProductFeedModuleOptions
{
    [MinLength(1)]
    public required string ChannelKey { get; init; }
}
