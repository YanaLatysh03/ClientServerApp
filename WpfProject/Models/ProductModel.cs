using System;
using System.Drawing;
using System.Windows.Media;

namespace WpfProject.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ImageSource Image { get; set; }
    }
}
