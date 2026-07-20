using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using product.Models;

namespace product.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]

        public IActionResult Get()
        {
            List<Product> products = new();

            string? connectionString =
    _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string not found");
            }
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT * FROM product";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            ProductName = reader["product_name"].ToString(),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            ProductDescription = reader["product_description"].ToString(),
                            Photo = reader["photo"].ToString()
                        });
                    }
                }
            }

            return Ok(products);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string not found");
            }

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "DELETE FROM product WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No product found with id {id}");
                    }
                }
            }

            return Ok(new
            {
                Message = "Product deleted successfully",
                Id = id
            });
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, Product data)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string not found");
            }

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            UPDATE product
            SET
                product_id = @product_id,
                product_name = @product_name,
                quantity = @quantity,
                product_description = @product_description,
                photo = @photo
            WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@product_id", data.ProductId);
                    cmd.Parameters.AddWithValue("@product_name", data.ProductName);
                    cmd.Parameters.AddWithValue("@quantity", data.Quantity);
                    cmd.Parameters.AddWithValue("@product_description", data.ProductDescription);
                    cmd.Parameters.AddWithValue("@photo", data.Photo);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"No product found with id {id}");
                    }
                }
            }

            return Ok(new
            {
                Message = "Product updated successfully",
                Id = id
            });
        }
        [HttpPost]
        public IActionResult SaveRecord(Product data)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection")!;

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                INSERT INTO product
                (
                    product_id,
                    product_name,
                    quantity,
                    product_description,
                    photo
                )
                VALUES
                (
                    @product_id,
                    @product_name,
                    @quantity,
                    @product_description,
                    @photo
                )";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@product_id", data.ProductId);
                        cmd.Parameters.AddWithValue("@product_name", data.ProductName);
                        cmd.Parameters.AddWithValue("@quantity", data.Quantity);
                        cmd.Parameters.AddWithValue("@product_description", data.ProductDescription);
                        cmd.Parameters.AddWithValue("@photo", data.Photo);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = "Product saved"
                            });
                        }

                        return BadRequest(new
                        {
                            success = false,
                            message = "Save failed"
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateQuantity(int id, [FromBody] QuantityUpdateModel model)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Product Id");
            }

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string not found");
            }

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            UPDATE product
            SET quantity = @quantity
            WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@quantity", model.Quantity);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound($"Product not found with id {id}");
                    }
                }
            }

            return Ok(new
            {
                success = true,
                message = "Quantity updated successfully"
            });
        }
        [HttpPost("buy")]
        public IActionResult BuyProduct(Product data)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection")!;

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                INSERT INTO bookedorder
                (
                    productId,
                    productName,
                    productDescription,
                    photo,
                    quantity,
                    qntvalue
                )
                VALUES
                (
                    @product_id,
                    @product_name,
                    @product_descryption,
                    @photo,
                    @quantity,
                    @qntvalue
                )";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@product_id", data.ProductId);
                        cmd.Parameters.AddWithValue("@product_name", data.ProductName);
                        cmd.Parameters.AddWithValue("@product_descryption", data.ProductDescription);
                        cmd.Parameters.AddWithValue("@photo", data.Photo);
                        cmd.Parameters.AddWithValue("@quantity", data.Quantity);
                        cmd.Parameters.AddWithValue("@qntvalue", data.QntValue);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = "Order booked successfully"
                            });
                        }

                        return BadRequest("Order not saved");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}