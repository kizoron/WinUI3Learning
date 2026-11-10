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
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        public string? VideoName { get; set; }
        public string? Author { get; set; } 
        public string? VideoImage { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Description { get; set; }
        public string link { get; set; } = "";
        
        public void ReplaceNullValues()
        {
            VideoImage = VideoImage ?? "";

            VideoName = VideoName ?? "Untitled";
            Author = Author ?? "Unknown";
            Description = Description ?? "";
        }
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

        // add video
        private async void Button_Input(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text))
            {
                return;
            }

            string json = await GetVideoInfo(InputBox.Text);

            AddNewItem(json);
            InputBox.Text = string.Empty;
        }

        private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Button_Input(sender, e);
            }
        }

        public static async Task<string> GetVideoInfo(string videoUrl)
        {
            using var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, videoUrl);

            // request.Headers.TryAddWithoutValidation("User-Agent",
              //  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"); 

            // 2. Добавляем cookie, чтобы обойти экран согласия (GDPR)
            // Без этого YouTube может вернуть страницу "Before you continue" без данных о видео
           // request.Headers.TryAddWithoutValidation("Cookie", "SOCS=CAI; CONSENT=YES+cb");

            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Ошибка запроса: {response.StatusCode}");
                return null;
            }

            string html = await response.Content.ReadAsStringAsync();

            //video container data
            var regex = new Regex(@"ytInitialPlayerResponse\s*=\s*(\{.+?\})\s*;", RegexOptions.Multiline);
            var match = regex.Match(html);

            if (!match.Success)
            {
                Debug.WriteLine("говно");
                return null;
            }

            string json = match.Groups[1].Value;

            return json;     
        }


        private void AddNewItem(string json)
        {
            ListDetailItem NewItem = new ListDetailItem()
            {
                link = InputBox.Text
            };

            //parsing json for searching needed info 
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("videoDetails", out var videoDetails))
            {
                if (videoDetails.TryGetProperty("title", out var titleElement))
                {
                    NewItem.VideoName = titleElement.GetString();
                }
                
                if (videoDetails.TryGetProperty("thumbnail", out var thumbnail) &&
                        thumbnail.TryGetProperty("thumbnails", out var thumbnails) &&
                            thumbnails.GetArrayLength() > 0)
                {                   
                    var lastThumbnail = thumbnails[thumbnails.GetArrayLength() - 1];
                    if (lastThumbnail.TryGetProperty("url", out var urlElement))
                    {
                        NewItem.VideoImage = urlElement.GetString();
                    }
                }



                if (videoDetails.TryGetProperty("author", out var authorElement))
                {
                    NewItem.Author = authorElement.GetString();
                }

                if (videoDetails.TryGetProperty("lengthSeconds", out var lengthElement))
                {
                    string lengthStr = lengthElement.GetString();

                    if (int.TryParse(lengthStr, out int lengthSeconds))
                    {
                        NewItem.Duration = TimeSpan.FromSeconds(lengthSeconds);
                    }
                }

                if (videoDetails.TryGetProperty("shortDescription", out var descriptionElement))
                {
                    NewItem.Description = descriptionElement.GetString();
                }
            }                

            Items.Add(NewItem);
        }


        private async void Button_Play(object sender, RoutedEventArgs e)
        {

        }

    }
}
