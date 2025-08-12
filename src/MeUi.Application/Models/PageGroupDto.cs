namespace MeUi.Application.Models;

public class PageGroupDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public ICollection<PageDto> Pages { get; set; } = [];
}