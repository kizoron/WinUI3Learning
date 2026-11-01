using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using static System.Net.Mime.MediaTypeNames;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestingWinUI
{ ///
    public class ListDetailItem
    { 
        public string Title { get; set; } 
        public string Text { get; set; }
        public DateTime DateCreated { get; set; } 
        public string DateCreatedFormatted => DateCreated.ToString("MMM d, yyyy h:mm tt"); 
    
    }
    public sealed partial class MainWindow : Window
    {

        public ObservableCollection<ListDetailItem> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.AppWindow.Resize(new SizeInt32(800, 600));

            Items = new ObservableCollection<ListDetailItem>();

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string text = await dataPackageView.GetTextAsync();

                InputBox.Text = text;
            }
            else
            {
                InputBox.Text = "Invalid input format";
            }
        }

        private void Button_Input(object sender, RoutedEventArgs e)
        {
            AddNewItem();
        }
        private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddNewItem();
            }
        }

        private void AddNewItem()
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text))
            {
                return;
            }

            ListDetailItem NewItem = new ListDetailItem()
            {
                Title = Convert.ToString(Items.Count + 1),
                Text = InputBox.Text,
                DateCreated = DateTime.Now,

            };

            Items.Add(NewItem);
            InputBox.Text = string.Empty;
        }

        private async void Button_Play(object sender, RoutedEventArgs e)
        {
            
        }



    }
}
