﻿using System;
namespace IhandCashier.Bepe.Types
{
    public class MenuItemPage
	{
        public string Label { get; set; }
        public string Page { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public bool Enable { get; set; } = true;
		public Color TextColor { get; set; } = Colors.Black;
        public MenuItemPage()
		{

		}
	}
}

