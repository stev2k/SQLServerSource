using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SQLServerSource.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SqlController : ControllerBase
    {
        private readonly ILogger<SqlController> _logger;

        public SqlController(ILogger<SqlController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Table")]

        public string Get(string tableName)
        {
            var connectionString = "Data Source=172.31.32.1;Initial Catalog=DemoDatabase;User=SA;Password=abcDEF123#;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var tableRegex = new Regex(@"^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$");
                if (tableRegex.IsMatch(tableName))
                {
                    if ((Convert.ToBoolean(connection.QueryFirst<int>("SELECT COUNT(*) FROM sys.tables WHERE NAME = '" + tableName + "'"))))
                    {
                       return string.Concat(connection.Query<string>(@"SELECT * FROM dbo.[" + tableName + "] FOR JSON AUTO", buffered: false));
                    }
                    else return "table does not exist";
                }
                else
                {
                    return "not a valid table name";
                }
            }

        }
    }
}
