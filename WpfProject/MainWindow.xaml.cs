using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using TestProject.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Drawing.Image image;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileUri = new Uri(openFileDialog.FileName);
                imgDynamic.Source = new BitmapImage(fileUri);
                image = System.Drawing.Image.FromFile(openFileDialog.FileName);
            }
        }

        public async void SendData_Click(object sender, RoutedEventArgs e)
        {
            var byteArray = default(byte[]);
            using var memoryStream = new MemoryStream();

            if (image == default || ImageName.Text.Length == 0)
            {
                MessageBox.Show("Вы не добавили данные");
            }
            else
            {
                image.Save(memoryStream, image.RawFormat);
                byteArray = memoryStream.ToArray();

                var product = new ImageByteModel { Name = ImageName.Text, ImageByteArray = byteArray };

                using var client = new HttpClient();

                using var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                using var result = await client.PostAsync(new Uri((string)App.Current.Resources["UploadRequestAdress"]), content);

                if (result.IsSuccessStatusCode)
                {
                    MessageBox.Show("Вы успешно добавили данные");
                }
                else
                {
                    MessageBox.Show("Данные не были добавлены");
                }
            }
        }

        public void OpenGetOneWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowForOneObject windowForOneObject = new WindowForOneObject();
            windowForOneObject.Show();
        }

        public void OpenWindowForTable_Click(object sender, RoutedEventArgs e)
        {
            WindowForTable windowForTable = new WindowForTable();
            windowForTable.Show();
        }

        public void OpenWindowForUpdate_Click(object sender, RoutedEventArgs e)
        {
            WindowForUpdate windowForUpdate = new WindowForUpdate();
            windowForUpdate.Show();
        }
    }
}