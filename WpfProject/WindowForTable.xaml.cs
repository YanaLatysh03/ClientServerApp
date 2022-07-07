using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TestProject.Models;
using WpfProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for WindowForTable.xaml
    /// </summary>
    public partial class WindowForTable : Window
    {
        private BindingList<ProductModel> bindingList;

        public WindowForTable()
        {
            InitializeComponent();
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<ImageJsonModel> listOfAllProducts;
            using var client = new HttpClient();

            using var result = await client.GetAsync((string)App.Current.Resources["GetAllRequestAdress"]);

            if (!result.IsSuccessStatusCode)
            {
                MessageBox.Show("Не удалось получить данные");
            }
            else
            {
                var jsonResult = result.Content.ReadAsStringAsync().Result;

                listOfAllProducts = JsonConvert.DeserializeObject<List<ImageJsonModel>>(jsonResult);


                bindingList = new BindingList<ProductModel>();

                for (int i = 0; i < listOfAllProducts.Count; i++)
                {
                    var model = new ProductModel()
                    {
                        Id = listOfAllProducts[i].Id,
                        Name = listOfAllProducts[i].Name,
                        Image = new BitmapImage(new Uri(listOfAllProducts[i].Path))
                    };

                    bindingList.Add(model);
                }
                dgProduct.ItemsSource = bindingList;
            }
        }

        private void dgProduct_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = e.Source;

            DataGrid scv = (DataGrid)sender;
            scv.RaiseEvent(eventArg);
            e.Handled = true;
        }
    }
}