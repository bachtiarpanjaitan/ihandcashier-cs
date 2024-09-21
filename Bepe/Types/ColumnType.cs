using System.Globalization;
using CommunityToolkit.Maui.Views;
using IhandCashier.Bepe.Components;
using IhandCashier.Bepe.Constants;
using IhandCashier.Bepe.Helpers;
using IhandCashier.Core.Maui.Providers;
using Syncfusion.Maui.DataGrid;

namespace IhandCashier.Bepe.Types
{
    public class ColumnType
    {
        public ColumnTypes Type { get; set; } = ColumnTypes.Text;
        public string MappingName { get; set; } = "id";
        public string MappingImage { get; set; } = "";
        public string HeaderText { get; set; } = "ID";
        public ColumnWidthMode ColumnMode { get; set; } = ColumnWidthMode.Fill;
        public double? Width { get; set; }

        public TextAlignment TextAlignment { get; set; } = TextAlignment.Start;

        public int ImageHeight = 20;
        public int ImageWidth = 20;

        public string Format { get; set; } = "";
        
        public DataGridColumn Create()
        {
            DataGridColumn column = Type switch
            {
                ColumnTypes.Text => new DataGridTextColumn
                {
                    MappingName = MappingName, HeaderText = HeaderText, CellTextAlignment = TextAlignment, ColumnWidthMode = ColumnMode, Format = Format
                },
                ColumnTypes.Numeric => new DataGridNumericColumn()
                {
                    MappingName = MappingName, HeaderText = HeaderText, CellTextAlignment = TextAlignment, ColumnWidthMode = ColumnMode, Format = Format
                },
                ColumnTypes.Currency => new DataGridNumericColumn()
                {
                    MappingName = MappingName, 
                    HeaderText = HeaderText, 
                    CellTextAlignment = TextAlignment.End, 
                    ColumnWidthMode = ColumnMode, 
                    Format = "C2",
                    CultureInfo = new CultureInfo("id-ID")
                },
                ColumnTypes.Date => new DataGridTextColumn()
                {
                    MappingName = MappingName, 
                    HeaderText = HeaderText, 
                    CellTextAlignment = TextAlignment.Start, 
                    ColumnWidthMode = ColumnMode, 
                    Format = Format != "" ? Format : "dddd, yyyy-MM-dd",
                    CultureInfo = new CultureInfo("id-ID")
                },
                ColumnTypes.Datetime => new DataGridTextColumn()
                {
                    MappingName = MappingName, 
                    HeaderText = HeaderText, 
                    CellTextAlignment = TextAlignment.Start, 
                    ColumnWidthMode = ColumnMode, 
                    Format =  "dddd, yyyy-MM-dd HH:mm:ss",
                    CultureInfo = new CultureInfo(Format != "" ? Format :"id-ID")
                },
                ColumnTypes.Checkbox => new DataGridCheckBoxColumn()
                {
                    MappingName = MappingName, HeaderText = HeaderText, CellTextAlignment = TextAlignment, ColumnWidthMode = ColumnMode, Format = Format
                },
                ColumnTypes.Image => new DataGridTemplateColumn()
                {
                    Width =  Width??100, MappingName = MappingName, HeaderText = HeaderText, CellTextAlignment = TextAlignment, ColumnWidthMode = ColumnMode, CellTemplate = new DataTemplate(
                        () =>
                        {
                            Image image = new Image
                            {
                                HeightRequest = ImageHeight,
                                WidthRequest = ImageWidth
                            };
                            
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += Image_Tapped;
                            image.GestureRecognizers.Add(tapGestureRecognizer);
                            image.SetBinding(Image.SourceProperty, MappingImage);
                            return image;
                        })
                },
                ColumnTypes.Detail => new DataGridTemplateColumn()
                {
                    Width =  Width??100,
                    HeaderText = HeaderText, 
                    CellTextAlignment = TextAlignment, 
                    ColumnWidthMode = ColumnMode,
                    CellTemplate  = new DataTemplate(() =>
                    {
                        var btn = new Button { Text = "Detail", TextColor = Colors.Coral ,BorderColor = Colors.Transparent, Margin = new Thickness(0,0,0,0) };
                        btn.SetBinding(Button.CommandParameterProperty, ".");
                        btn.Clicked += ShowDetail;
                        return btn;
                    })
                },
                _ => throw new ArgumentException("Invalid column type")
            };
            column.CellStyle = new Style(typeof(DataGridCell))
            {
                Setters = { 
                    new Setter
                    {
                        Property = DataGridCell.PaddingProperty,
                        Value = new Thickness(8,3)
                    },
                    new Setter
                    {
                        Property = DataGridCell.MarginProperty,
                        Value = new Thickness(8,3)
                    }
                }
            };
            return column;
        }

        private async void ShowDetail(object sender, EventArgs e)
        {
            
            if (sender is Button btn)
            {
                dynamic context = btn?.CommandParameter;
                
                var manager = new PopupManager();
                var imagePopup = new DetailPreviewPopup()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };

                if (context != null)
                {
                    imagePopup.SetData(context.Views);
                    await manager.ShowPopupAsync(imagePopup);
                }
                
            }
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (sender is Image img)
            {
                var manager = new PopupManager();
                var imagePopup = new ImagePreviewPopup()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                imagePopup.SetImage(img.Source,500,500);
                await manager.ShowPopupAsync(imagePopup);
            }
        }

    }
}