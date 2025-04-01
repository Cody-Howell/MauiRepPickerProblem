using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PickerProblem.MAUI.ViewModels;

public partial class MainPageViewModel : ObservableObject {
    [ObservableProperty]
    private ObservableCollection<Item> items;

    public List<string> Tags { get; } = new List<string> { "A", "B", "C", "D", "E" };

    [ObservableProperty]
    private string newTag = "";

    public MainPageViewModel() {
        items = new ObservableCollection<Item>();
    }

    [RelayCommand]
    private void AddItem() {
        Item i = new Item(NewTag);
        Items.Add(i);
        NewTag = "";
    }

    //[RelayCommand]
    //private void SortItems() {
    //    IEnumerable<Item> sortedItems = Items.OrderBy(a => a.Tag).ToArray();
    //    Items.Clear();
    //    Items = [.. sortedItems];
    //}

}

public partial class Item : ObservableObject {
    [ObservableProperty]
    private string tag;

    public Item(string t) {
        tag = t;
    }

    public override string ToString() {
        return $"Tag: {Tag}";
    }
}