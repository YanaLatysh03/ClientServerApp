using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestProject.Models
{
    [Serializable]
    public class ImageByteModel
    {
        public string Name { get; set; }

        public byte[] ImageByteArray { get; set; }
    }
}
