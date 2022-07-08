using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using TestProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for WindowForOneObject.xaml
    /// </summary>
    public partial class WindowForOneObject : Window
    {
        public WindowForOneObject()
        {
            InitializeComponent();
        }

        public async void GetOne_Click(object sender, RoutedEventArgs e)
        {
            if (ImageId.Text.Length == 0)
            {
                MessageBox.Show("Укажите Id картинки");
            }
            else
            {
                using var client = new HttpClient();

                using var content = new StringContent(JsonConvert.SerializeObject(ImageId.Text), Encoding.UTF8, "application/json");
                using var result = await client.PostAsync(new Uri((string)App.Current.Resources["GetOneRequestAdress"]), content);

                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show("Картинки с таким Id нет");
                }
                else
                {
                    var jsonResult = result.Content.ReadAsStringAsync().Result;

                    var productResult = JsonConvert.DeserializeObject<ImageJsonModel>(jsonResult);

                    imgDynamic.Source = new BitmapImage(new Uri(productResult.Path));
                    ImageId.Text = productResult.Name;
                }
            }
        }

        public async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ImageId.Text.Length == 0)
            {
                MessageBox.Show("Укажите Id картинки");
            }
            else
            {
                using var client = new HttpClient();
                var message = new HttpRequestMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(ImageId.Text), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri((string)App.Current.Resources["DeleteRequestAdress"])
                };

                using var result = await client.SendAsync(message);

                if (result.IsSuccessStatusCode)
                {
                    MessageBox.Show("Объект успешно удален");
                }
                else
                {
                    MessageBox.Show("Объект не удален");
                }
            }
        }
    }
}
