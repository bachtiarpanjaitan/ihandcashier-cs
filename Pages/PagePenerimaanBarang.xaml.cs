using IhandCashier.Bepe.Controllers;
using IhandCashier.Bepe.Types;
using IhandCashier.Core.Maui.Providers;
using IhandCashier.Pages.Views;

namespace IhandCashier.Pages;

public partial class PagePenerimaanBarang
{
    public PagePenerimaanBarang()
	{
		InitializeComponent();
        SideMenus.Add("penerimaan_barang", new MenuItemPage { Label = "PENERIMAAN", Page =  typeof(GridPenerimaanBarang).FullName, Icon = "\uE558"});
        DefineLayoutTwoColumn();
    }
}