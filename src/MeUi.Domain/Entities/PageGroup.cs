namespace MeUi.Domain.Entities;

public class PageGroup : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public ICollection<Page> Pages { get; set; } = new HashSet<Page>();
}