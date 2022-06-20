using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        private string path = "C:\\Users\\shoto\\source\\repos\\ApiTestProject\\TestProject\\image.json";

        public ProductController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [Route("uploadImage")]
        public async Task<string> Post(Product product)
        {
            FileStream fc = new FileStream(product.Path, FileMode.Open);

            try
            {
                if (fc.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + product.Name + ".JPG"))
                    {
                        fc.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    var list = new List<InputProduct>();
                    Guid guid = Guid.NewGuid();
                    InputProduct inputProduct = new InputProduct(product.Name, $"{_environment.WebRootPath}\\Upload\\{product.Name}", guid);
                    string json = System.IO.File.ReadAllText(path);

                    if (json.Length == 0)
                    {
                        list.Add(inputProduct);
                    }
                    else
                    {
                        list = JsonConvert.DeserializeObject<List<InputProduct>>(json);

                        list.Add(inputProduct);
                    }
                    
                    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);

                    System.IO.File.WriteAllText(path, convertedJson);

                    return "\\Upload\\" + product.Name;
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<List<InputProduct>> GetAll()
        {
            try
            {

                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path);
                }

                string json = System.IO.File.ReadAllText(path);
                var products = JsonConvert.DeserializeObject<List<InputProduct>>(json);
                return products;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("get-by-id")]
        public async Task<ActionResult<InputProduct>> GetProductById([FromBody] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                string json = System.IO.File.ReadAllText(path);
                var products = JsonConvert.DeserializeObject<List<InputProduct>>(json);
                var findedProduct = new InputProduct();

                foreach(var product in products)
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

                return findedProduct;
            }
            catch(Exception ex)
            {
                throw new ArgumentNullException(ex.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("delete-by-id")]
        public async Task<ActionResult> DeleteProductById([FromBody] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                string json = System.IO.File.ReadAllText(path);
                var products = JsonConvert.DeserializeObject<List<InputProduct>>(json);
                var findedProduct = new InputProduct();

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

                products.Remove(findedProduct);

                var convertedJson = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(path, convertedJson);

                return Ok();
            }
            catch(Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }

        [HttpPut]
        [Route("update-product")]
        public async Task<ActionResult<InputProduct>> UpdateProductById([FromBody] InputProduct inputProduct)
        {
            if (inputProduct.Id.Equals(Guid.Empty))
            {
                return BadRequest(new { errorText = "Incorrect request body." });
            }

            try
            {
                string json = System.IO.File.ReadAllText(path);
                var products = JsonConvert.DeserializeObject<List<InputProduct>>(json);
                var findedProduct = new InputProduct();

                foreach (var product in products)
                {
                    if (product.Id.Equals(inputProduct.Id))
                    {
                        product.Name = inputProduct.Name;
                        product.Path = inputProduct.Path;
                        findedProduct = product;
                        break;
                    }
                }

                if (findedProduct.Name is null && findedProduct.Path is null)
                {
                    return NoContent();
                }

                var convertedJson = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(path, convertedJson);

                return Ok();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }
    }
}
