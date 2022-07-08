using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public static ILogger<ProductController> _logger;
        private string jsonFile = "image.json";

        public ProductController(IWebHostEnvironment environment, ILogger<ProductController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost]
        [Route("uploadImage")]
        public ActionResult Post(ImageByteModel product)
        {
            if (product.ImageByteArray == default && product.Name.Length == default)
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                using var ms = new MemoryStream(product.ImageByteArray);

                using var image = Image.FromStream(ms);

                using var fc = new MemoryStream();
                image.Save(fc, ImageFormat.Jpeg);

                if (!Directory.Exists($"{_environment.WebRootPath}\\Upload\\"))
                {
                    Directory.CreateDirectory($"{_environment.WebRootPath}\\Upload\\");
                }

                using FileStream fileStream = System.IO.File.Create($"{_environment.WebRootPath}\\Upload\\{product.Name}.JPG");
                fc.WriteTo(fileStream);

                image.Dispose();

                var list = new List<ImageJsonModel>();
                var guid = Guid.NewGuid();
                var inputProduct = new ImageJsonModel(product.Name, $"{_environment.WebRootPath}\\Upload\\{product.Name}.JPG", guid);
                var json = System.IO.File.ReadAllText(jsonFile);

                if (json.Length == 0)
                {
                    list.Add(inputProduct);
                }
                else
                {
                    list = JsonConvert.DeserializeObject<List<ImageJsonModel>>(json);
                    list.Add(inputProduct);
                }

                var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFile, convertedJson);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to upload image");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all")]
        public ActionResult<List<ImageJsonModel>> GetAll()
        {
            try
            {

                if (!System.IO.File.Exists(jsonFile))
                {
                    System.IO.File.Create(jsonFile);
                }

                var json = System.IO.File.ReadAllText(jsonFile);
                var products = JsonConvert.DeserializeObject<List<ImageJsonModel>>(json);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get all images");
                return GetExceptionMessage(ex);
            }
        }

        [HttpPost]
        [Route("get-by-id")]
        public ActionResult<ImageJsonModel> GetProductById([FromBody] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                var json = System.IO.File.ReadAllText(jsonFile);
                var products = JsonConvert.DeserializeObject<List<ImageJsonModel>>(json);
                var findedProduct = new ImageJsonModel();

                foreach (var product in products)
                {
                    if (product.Id.Equals(id))
                    {
                        findedProduct = product;
                        break;
                    }
                }

                if (findedProduct.Name is null && findedProduct.Path is null)
                {
                    return NoContent();
                }

                return Ok(findedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get one image by Id");
                return GetExceptionMessage(ex);
            }
        }

        [HttpDelete]
        [Route("delete-by-id")]
        public ActionResult DeleteProductById([FromBody] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                var json = System.IO.File.ReadAllText(jsonFile);
                var products = JsonConvert.DeserializeObject<List<ImageJsonModel>>(json);
                var findedProduct = new ImageJsonModel();

                foreach (var product in products)
                {
                    if (product.Id.Equals(id))
                    {
                        findedProduct = product;
                        break;
                    }
                }

                if (findedProduct.Name is null && findedProduct.Path is null)
                {
                    return NoContent();
                }

                System.IO.File.Delete(findedProduct.Path);
                products.Remove(findedProduct);
                

                var convertedJson = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFile, convertedJson);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to delete images");
                return GetExceptionMessage(ex);
            }
        }

        [HttpPut]
        [Route("update-product")]
        public ActionResult UpdateProductById([FromBody] ImageModelForUpdate inputProduct)
        {
            if (inputProduct.Id.Equals(Guid.Empty) || inputProduct.Name == default || inputProduct.ImageByteArray == default)
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                var json = System.IO.File.ReadAllText(jsonFile);
                var products = JsonConvert.DeserializeObject<List<ImageJsonModel>>(json);
                var findedProduct = new ImageJsonModel();

                foreach (var product in products)
                {
                    if (product.Id.Equals(inputProduct.Id))
                    {
                        using var ms = new MemoryStream(inputProduct.ImageByteArray);
                        Image image;
                        image = Image.FromStream(ms);

                        using MemoryStream fc = new MemoryStream();
                        image.Save(fc, ImageFormat.Jpeg);

                        if (fc.Length > 0)
                        {
                            using var fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + inputProduct.Name + ".JPG");

                            fc.WriteTo(fileStream);

                            image.Dispose();
                        }
                        else
                        {
                            return BadRequest();
                        }

                        product.Name = inputProduct.Name;
                        product.Path = $"{_environment.WebRootPath}\\Upload\\{inputProduct.Name}.JPG";
                        findedProduct = product;
                        break;
                    }
                }

                var convertedJson = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFile, convertedJson);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update image");
                return GetExceptionMessage(ex);
            }
        }

        private ActionResult GetExceptionMessage(Exception ex) => ex switch
        {
            ArgumentNullException => BadRequest("The request has invalid syntax"),
            KeyNotFoundException => NotFound("The requested resource was not found."),
            _ => StatusCode(50, "Unknown exception, please contact the System Administrator"),
        };
    }
}
