using Amazon;
using Amazon.RDS.Util;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace RDSProxyDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class Demo : ControllerBase
{
    private readonly ILogger<Demo> _logger;

    public Demo(ILogger<Demo> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var pwd = RDSAuthTokenGenerator.GenerateAuthToken(RegionEndpoint.USWest2,
            Environment.GetEnvironmentVariable("DB_HOST"),
            3306,
            Environment.GetEnvironmentVariable("DB_USER"));

        _logger.LogInformation("\nToken: {Pwd}\n", pwd);
        
        var sb = new MySqlConnectionStringBuilder();
        sb.Server = Environment.GetEnvironmentVariable("DB_HOST");
        sb.UserID = Environment.GetEnvironmentVariable("DB_USER");
        sb.Database = Environment.GetEnvironmentVariable("DB_DATABASE");
        sb.SslMode = MySqlSslMode.Required;

        sb.Password = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_PASSWORD"))
            ? pwd
            : Environment.GetEnvironmentVariable("DB_PASSWORD");

        // sb.Password = "minhasenha789";

        try
        {
            var mySqlConnection = new MySqlConnection(sb.ConnectionString);
            mySqlConnection.Open();
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }

        return Ok("Connection Opened");
    }
}