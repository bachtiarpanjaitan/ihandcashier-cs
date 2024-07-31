﻿using IhandCashier.Bepe.Constants;
using IhandCashier.Pages;

namespace IhandCashier.Layouts;

public partial class MainLayout : UniqueTabPage
{

    public MainLayout()
	{
		InitializeComponent();
        SetupMenu();
        SetupTab();

    }

	private void SetupMenu()
	{
        //SAYA
        var mbMe = new MenuBarItem { Text = "Saya" };
        var meProfile = new MenuFlyoutItem { Text = "Profil", CommandParameter = Menu.MenuItemType.Profile };
        var meSetting = new MenuFlyoutItem { Text = "Pengaturan", CommandParameter = Menu.MenuItemType.Setting };
        var meExit = new MenuFlyoutItem { Text = "Keluar" };
        mbMe.Add(meProfile);
        mbMe.Add(meSetting);
        mbMe.Add(meExit);

        //MASTER
        var mbMaster = new MenuBarItem { Text = "Master" };
        var masterBarang = new MenuFlyoutItem { Text = "Data Barang" };
        var masterKasir = new MenuFlyoutItem { Text = "Data Kasir" };
        var masterPemasok = new MenuFlyoutItem { Text = "Data Pemasok" };
        var masterPelanggan = new MenuFlyoutItem { Text = "Data Pelanggan" };
        mbMaster.Add(masterBarang);
        mbMaster.Add(masterKasir);
        mbMaster.Add(masterPemasok);
        mbMaster.Add(masterPelanggan);

        //TRANSAKSI
        var mbTransaksi = new MenuBarItem { Text = "Transaksi" };
        var transPenerimaan = new MenuFlyoutItem { Text = "Penerimaan Barang" };
        var transPenjualan = new MenuFlyoutItem { Text = "Penjualan Barang" };
        var transStok = new MenuFlyoutSubItem { Text = "Stok" };
        var cTransStok = new MenuFlyoutItem { Text = "Stok Barang" };
        var cTransAddjusment = new MenuFlyoutItem { Text = "Penyesuaian Barang" };
        var transRet = new MenuFlyoutSubItem { Text = "Retur" };
        var cRetJual= new MenuFlyoutItem { Text = "Retur Jual" };
        var cRetBeli= new MenuFlyoutItem { Text = "Retur Beli" };

        transStok.Add(cTransStok);
        transStok.Add(cTransAddjusment);

        transRet.Add(cRetBeli);
        transRet.Add(cRetJual);


        mbTransaksi.Add(transPenerimaan);
        mbTransaksi.Add(transPenjualan);
        mbTransaksi.Add(transStok);
        mbTransaksi.Add(transRet);

        //REPORT
        var mbReport = new MenuBarItem { Text = "Laporan" };
        var rep_1 = new MenuFlyoutItem { Text = "Laporan Ringkasan Bisnis" };
        var rep_2 = new MenuFlyoutItem { Text = "Laporan Jual Beli" };
        var rep_3 = new MenuFlyoutItem { Text = "Laporan Hutang Piutang" };
        var rep_4 = new MenuFlyoutItem { Text = "Laporan Stok Barang" };
        var rep_5 = new MenuFlyoutItem { Text = "Laporan Lainnya" };
        mbReport.Add(rep_1);
        mbReport.Add(rep_2);
        mbReport.Add(rep_3);
        mbReport.Add(rep_4);
        mbReport.Add(rep_5);

        this.MenuBarItems.Add(mbMe);
        this.MenuBarItems.Add(mbMaster);
        this.MenuBarItems.Add(mbTransaksi);
        this.MenuBarItems.Add(mbReport);

        meProfile.Clicked += OnMenuItemClicked;
        meSetting.Clicked += OnMenuItemClicked;
       
    }

    private void OnMenuItemClicked(object sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuBarItem)
        {
            if (menuBarItem.CommandParameter is Menu.MenuItemType menuItemType)
            {
                var data = menuBarItem?.CommandParameter as Enum;
                var menus = Menu.ListMenu()[data];
                object instance = Activator.CreateInstance(menus);
                AddTab(data, (Page)instance);

            }
        }
    }

    private void SetupTab()
    {
        AddTab(Menu.MenuItemType.Home,new PageHome());
    }

}
