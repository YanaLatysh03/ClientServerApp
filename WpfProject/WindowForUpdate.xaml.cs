using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using TestProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for WindowForUpdate.xaml
    /// </summary>
    public partial class WindowForUpdate : Window
    {
        public System.Drawing.Image image;

        public WindowForUpdate()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                imgDynamic.Source = new BitmapImage(fileUri);
                image = System.Drawing.Image.FromFile(openFileDialog.FileName);
            }
        }

        public async void SendData_Click(object sender, RoutedEventArgs e)
        {
            var byteArray = default(byte[]);
            using var memoryStream = new MemoryStream();

            if (image == default || Name.Text.Length == 0 || Id.Text.Length == 0)
            {
                MessageBox.Show("Вы не добавили данные");
            }
            else
            {
                image.Save(memoryStream, image.RawFormat);
                byteArray = memoryStream.ToArray();
                memoryStream.Flush();

                var product = new ImageModelForUpdate { Id = new Guid(Id.Text), Name = Name.Text, ImageByteArray = byteArray };

                using var client = new HttpClient();

                using var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                using var result = await client.PutAsync(new Uri((string)App.Current.Resources["UpdateRequestAdress"]), content);
                
                if (result.IsSuccessStatusCode)
                {
                    MessageBox.Show("Вы успешно обновили данные");
                }
                else
                {
                    MessageBox.Show("Не удалось обновить данные");
                }
            }
        }
    }
}
