using IhandCashier.Bepe.Extensions;
using IhandCashier.Bepe.Providers;

namespace IhandCashier.Bepe.Components;

public static class FilterOne
{
    private static EventHandler<TextChangedEventArgs> _entrySearchChangedHandler;
    private static readonly Entry Search = new()
    {
        HorizontalOptions = LayoutOptions.End,
        VerticalOptions = LayoutOptions.Center,
        Placeholder = "Ketik untuk mencari",
        WidthRequest = 250,
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        MinimumWidthRequest = 50,
        AutomationId = "Search"
    };
    private static readonly Button AddBtn = new()
    {
        HorizontalOptions = LayoutOptions.End,
        VerticalOptions = LayoutOptions.Center,
        Text = "Tambah",
        WidthRequest = 100,
        Margin = new Thickness(5, 0)
    };
    private static readonly Label ModuleLabel = new()
    {
        Text = "",
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Center,
        MinimumWidthRequest = 150,
        FontAttributes = FontAttributes.Bold,
        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        Margin = new Thickness(20,0,0,0)
        
    };
    private static readonly Grid Grid = new()
    {
        ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto }, // Kolom untuk label modul
            new ColumnDefinition { Width = GridLength.Star }, // Kolom untuk komponen kanan pertama
            new ColumnDefinition { Width = GridLength.Auto },
        }
    };
    
    public static void Initialize(string moduleName)
    {
        ModuleLabel.Text = moduleName;
        Grid.Add(ModuleLabel,0);
        Grid.Add(Search,1);
        Grid.Add(AddBtn,2);
        DatagridProvider.HeaderFrame.Content = Grid;
        Search.Text = "";
    }
    
    public static void FilterOneHandlers(EventHandler<TextChangedEventArgs> searchHandler)
    {
        if (_entrySearchChangedHandler != null) 
        {
            Search.TextChanged -= _entrySearchChangedHandler;
        }
        _entrySearchChangedHandler = searchHandler;
        Search.DebounceTextChanged(OnSearchTextChanged, 1000);
    }
    
    private static void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _entrySearchChangedHandler?.Invoke(sender, e);
    }
}