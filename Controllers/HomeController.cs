using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            try
            {
                if (csvFile != null && csvFile.Length > 0)
                {
                    List<CsvInput> records = null;
                    using (var csv = new CsvReader(new StreamReader(csvFile.OpenReadStream())))
                    {   csv.Configuration.HasHeaderRecord = false; 
                        records = csv.GetRecords<CsvInput>().ToList();
                    }

                    bool result = _catalogue.SaveSellerCatalogue(records);

                    return View(result ? "~/Views/Home/Success.cshtml" : "~/Views/Home/Failure.cshtml");
                }
                else
                {
                    return View("~/Views/Home/Failure.cshtml");
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error: UploadSellerInfo. Exception: {0}", e.Message));
                return View("~/Views/Home/Failure.cshtml");
            }
            
        }

        [HttpPost("downloadcatalogue")]
        public IActionResult DownloadSellerCatalogue([FromForm]int sellerId)
        {

            try
            {
                int activeVersion = _catalogue.GetActiveCatalogueVersionId(sellerId);

                if (sellerId > 0 && activeVersion > 0)
                {
                    string csvData = _catalogue.GenerateSellerCatalogueCSV(sellerId, activeVersion);
                    return File(new UTF8Encoding().GetBytes(csvData), "text/csv", string.Format("seller-{0}-{1}.csv", sellerId, activeVersion));
                }
                else
                {
                    return View("~/Views/Home/SellerFailure.cshtml");   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error: DownloadSellerCatalogue. Exception: {0}", e.Message));
                return View("~/Views/Home/SellerFailure.cshtml");   
            }
        }
    }
}
