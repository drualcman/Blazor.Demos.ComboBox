namespace Blazor.Demos.ComboBox.Models;

public class ListOfElements
{
    public int Id { get;  }
    public string Name { get; }

    public ListOfElements(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
