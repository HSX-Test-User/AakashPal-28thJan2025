
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ClicksAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClickZoneController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ClickZoneController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) {
                return BadRequest("No file uploaded.");
            }

            var clicks = new List<Click>();
            var zones = new List<Zone>();


            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int totalRows = worksheet.Dimension.End.Row;
                    for (int row = 2; row <= totalRows; row++)
                    {
                        clicks.Add(new Click
                        {
                            X = worksheet.Cells[row, 2].GetValue<double>(),
                            Y = worksheet.Cells[row, 3].GetValue<double>(),
                        });

                        string cordinates = worksheet.Cells[row, 8].GetValue<string>();
                        var coArray = cordinates.Split(",");
                        zones.Add(new Zone
                        {
                            Name = worksheet.Cells[row,7].GetValue<string>(),
                            X1 = int.Parse(coArray[0]), 
                            Y1 = int.Parse(coArray[1]),
                            X2 = int.Parse(coArray[2]),
                            Y2 = int.Parse(coArray[3]),
                        });
                    }
                }
            }
            //TODO: we need to add SQL connection here
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var sql = "INSERT INTO ClickZone (X, Y) VALUES (@X, @Y)";
                await connection.ExecuteAsync(sql, clicks);

                var sql2 = "INSERT INTO Zone (Name, X1, Y1, X2, Y2) VALUES (@Name, @X1, @Y1, @X2, @Y2)";
                await connection.ExecuteAsync(sql2, zones);
            }
            return Ok("File processed and data saved");
        }

        [HttpGet("clickCounts")]
        public async Task<IActionResult> GetClickCounts()
        {
            //TODO: we need to add SQL connection here
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var sql = @"
                    SELECT z.Name AS ZoneName, COUNT(c.Id) as ClickCount
                    FROM Zone z
                    LEFT JOIN ClickZone c ON c.X BETWEEN z.X1 AND z.X2 AND c.Y BETWEEN z.Y1 AND z.Y2
                    GROUP BY z.Id";
                var result = await connection.QueryAsync(sql);
                return Ok(result);
            }
        }
    }
}

