using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using product.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public object SecurityAlgorithms { get; private set; }

    [HttpPost]
    [Route("login")]
    public IActionResult Login(LoginRequest request)
    {
        string connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();

            string query = @"
            SELECT 
                ua.id,
                ua.name,
                ur.role
            FROM useraccess ua
            LEFT JOIN userrole ur
                ON ua.id = ur.useraccessId
            WHERE ua.name=@name
            AND ua.password=@password";

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@name", request.Name);
                cmd.Parameters.AddWithValue("@password", request.Password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string userId = reader["id"].ToString()!;
                        string username = reader["name"].ToString()!;
                        string role = reader["role"].ToString()!;

                        string token = GenerateJwtToken(
                            userId,
                            username,
                            role);

                        return Ok(new LoginResponse
                        {
                            Token = token,
                            UserId = userId,
                            UserName = username,
                            Role = role
                        });
                    }
                }
            }
        }

        return Unauthorized("Invalid Username or Password");
    }

    private string GenerateJwtToken(
        string userId,
        string userName,
        string role)
    {
        var key = Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]!);

        var claims = new[]
        {
            new Claim("UserId", userId),
            new Claim("UserName", userName),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.Now.AddMinutes(60),
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(key),
        Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
    )
);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}




