using IhandCashier.Bepe.Components;
using IhandCashier.Bepe.Constants;
using IhandCashier.Bepe.Database;
using IhandCashier.Bepe.Entities;
using IhandCashier.Bepe.Helpers;
using IhandCashier.Core.Maui.Providers;
using IhandCashier.Bepe.Services;
using IhandCashier.Bepe.Types;
using IhandCashier.Pages.Forms;
using Syncfusion.Maui.DataGrid;

namespace IhandCashier.Pages.Views;

public partial class GridDataPemasok
{
    private const string ModuleName = "Data Pemasok";
    SupplierService _service  = ServiceLocator.ServiceProvider.GetService<SupplierService>();
    Supplier _selectedProduct;
    Pagination<Supplier> _pagination;
    public GridDataPemasok()
    {
        InitializeComponent();
        
        FilterOne.Initialize(ModuleName);
            
        ResetView();
        List<ColumnType> columns = [
            new ColumnType { Type = ColumnTypes.Numeric,TextAlignment = TextAlignment.Center,MappingName = "id", ColumnMode = ColumnWidthMode.FitByCell ,HeaderText = "ID", Format = "N0" },
            new ColumnType { Type = ColumnTypes.Text, MappingName = "nama", HeaderText = "NAMA"},
            new ColumnType { Type = ColumnTypes.Text, MappingName = "telepon", HeaderText = "TELEPON"},
            new ColumnType { Type = ColumnTypes.Text, MappingName = "alamat", HeaderText = "ALAMAT"}
        ];
        SetDatagridColumns(columns);
        Content = DatagridProvider.LayoutDatagrid;
    
        DatagridProvider.ShowLoader();
        Device.BeginInvokeOnMainThread(() =>
        {
            _pagination = new Pagination<Supplier>(_service, typeof(FilterOne));
            DatagridProvider.AddDatagridCellHandler(OnClick);
            DatagridProvider.HideLoader();
        });
    }

    private void OnClick(object sender, DataGridCellTappedEventArgs e)
    {
        _selectedProduct = e.RowData as Supplier;
        if (_selectedProduct != null) Console.WriteLine($"Barang : {_selectedProduct.nama}");
    }
}