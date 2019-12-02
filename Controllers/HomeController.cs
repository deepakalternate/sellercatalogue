using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sellercatalogue.BAL;
using sellercatalogue.Entities;
using sellercatalogue.Models;

namespace sellercatalogue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICatalogue _catalogue;

        public HomeController(ILogger<HomeController> logger, ICatalogue catalogue)
        {
            _logger = logger;
            _catalogue = catalogue;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("sellerupload")]
        public IActionResult UploadSellerInfo([FromForm] IFormFile csvFile)
        {
            if (csvFile != null && csvFile.Length > 0 && csvFile.ContentType.Equals("text/csv", StringComparison.InvariantCultureIgnoreCase))
            {
                List<CsvInput> records = null;
                using (var csv = new CsvReader(new StreamReader(csvFile.OpenReadStream())))
                {   csv.Configuration.HasHeaderRecord = false; 
                    records = csv.GetRecords<CsvInput>().ToList();
                }

                bool result = _catalogue.SaveSellerCatalogue(records);

                    
                return Ok(records);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("downloadcatalogue")]
        public IActionResult DownloadSellerCatalogue([FromForm]int sellerId)
        {
            /*
            List<Customer> catalogue = new List<Customer> {new Customer {
                Id = 1,
                Value = 2,
                Name = "A"
            },
            new Customer {
                Id = 2,
                Value = 3,
                Name = "B"
            }};

            var stream = new MemoryStream();
            var writeFile = new StreamWriter(stream);
            var csv = new CsvWriter(writeFile);

            csv.WriteRecords(catalogue);

            stream.Position = 0; //reset stream
            return File(stream, "application/octet-stream", "Reports.csv");
            */
            return Ok();
        }
    }
}
