using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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
{
    /// <summary>

    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.AppWindow.Resize(new SizeInt32(800, 600));
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

        private async void Button_Input(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Play(object sender, RoutedEventArgs e)
        {
            
        }



    }
}
