using System;

namespace TestProject.Models
{
    public class ImageModelForUpdate
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] ImageByteArray { get; set; }
    }
}
