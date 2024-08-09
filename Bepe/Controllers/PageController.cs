﻿using System;
using System.Reflection;
using IhandCashier.Bepe.Components;
using IhandCashier.Bepe.Constants;
using IhandCashier.Bepe.Helpers;
using IhandCashier.Bepe.Interfaces;
using IhandCashier.Bepe.Types;

namespace IhandCashier.Bepe.Controllers
{
	public class PageController : ContentView,IPageInterface
	{
        private ContentView _contentView = new();
        public Dictionary<string, MenuItemPage> SideMenus = new();
        private ContentLayoutTwoColumn _layout = new();
        public PageController()
		{

		}

        public void DefineLayoutTwoColumn()
        {
            SideMenu sm = new();
            sm.SetMenuItems(SideMenus);
            sm.ItemTapped += OnClickSideMenuItemAsync;
            VerticalStackLayout sideMenu = sm.CreateSideMenu();
            
            _layout.SetSideMenu(sideMenu);
            Content = _layout.GenerateFrame();
        }

        public void OnClickSideMenuItemAsync(object obj, EventHandlerPageArgs e)
        {
            Type type = Type.GetType(e.Page);
            if(type != null)
            {
               try {
                    _contentView = (ContentView)Activator.CreateInstance(type);
                    _layout.SetContent(_contentView);
                    //Content = layout.GenerateFrame();
               }
               catch (TargetInvocationException ex)
               {
                   Console.WriteLine("Cannot create instance " + ex.InnerException?.Message);
               }
            }
           
        }
    }
}

