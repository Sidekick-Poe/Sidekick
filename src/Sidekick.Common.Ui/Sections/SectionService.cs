namespace Sidekick.Common.Ui.Sections;

public class SectionService
{
    public event Action? OnChanged;

    public List<string> BodySectionNames { get;  } = [];

    public void AddBody(string name)
    {
        BodySectionNames.Add(name);
        OnChanged?.Invoke();
    }

    public void RemoveBody(string name)
    {
        BodySectionNames.Remove(name);
        OnChanged?.Invoke();
    }
}
