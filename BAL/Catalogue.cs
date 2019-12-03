using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sellercatalogue.DAL;
using sellercatalogue.Entities;

namespace sellercatalogue.BAL
{
    public class Catalogue : ICatalogue
    {
        private readonly ICatalogueRepository _catalogueRepo;

        public Catalogue(ICatalogueRepository catalogueRepo)
        {
            _catalogueRepo = catalogueRepo;
        }

        //Business Function to generate the csv string for the file
        public string GenerateSellerCatalogueCSV(int sellerId, int versionId)
        {
            List<CsvInput> prods = _catalogueRepo.GenerateCatalogue(sellerId, versionId);
            return GenerateCSVString(prods);
        }

        // Private function which creates a string from a list
        private string GenerateCSVString(List<CsvInput> input)
        {
            
            StringBuilder sb = new StringBuilder();

            foreach (var item in input)
            {
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", item.SellerId, item.SellerProdId, item.Name, item.Description, item.Qunatity, item.Price));
            }

            return sb.ToString();
        }

        // Business function which processes the input provided by the user and saves the data after validation
        public bool SaveSellerCatalogue(List<CsvInput> input)
        {
            bool result = false;

            if (input != null && input.Any())
            {
                int sellerId = input.FirstOrDefault().SellerId; //Gets seller id from input csv file

                int versionId = _catalogueRepo.GetSellersLastActiveVersion(sellerId); // Gets current active version for seller

                if (versionId == -1 && !_catalogueRepo.SaveNewSeller(sellerId)) // Saves seller if version is invalid and checks if seller was saved properly if not return false and ends flow
                {
                    return result;
                }

                input = RemoveDuplicates(input); // Private function called which takes decision on input and removes duplicates based on business function

                if (versionId > 0) // Gets remaiing products only when the seller has an active catalogue
                {
                    string remainingProdIds = string.Join(',', input.Select(x => x.SellerProdId));
                    List<CsvInput> remainingProds = _catalogueRepo.GetRemainingProducts(remainingProdIds, sellerId, versionId);
                    input.AddRange(remainingProds);
                }

                int newversionId = versionId == -1 ? 1 : versionId + 1;
                bool entriesSaved = _catalogueRepo.SaveNewCatalogueData(input, newversionId); // Saves the catalogue

                if (entriesSaved) // On successfully saving the catalogue, the active version for the seller is updated
                {
                    result = _catalogueRepo.UpdateActiveVersion(sellerId, newversionId);
                }

            }

            return result;
        }

        // Business function to return the current active catalogue version of seller
        public int GetActiveCatalogueVersionId(int sellerId)
        {
            return _catalogueRepo.GetSellersLastActiveVersion(sellerId);
        }

        // Private function to return duplicates from list of entries
        private List<CsvInput> RemoveDuplicates(List<CsvInput> input)
        {
            Dictionary<string, CsvInput> inputDict = new Dictionary<string, CsvInput>();

            foreach (var item in input)
            {
                string key = string.Format("{0}_{1}", item.SellerId, item.SellerProdId);

                if (inputDict.ContainsKey(key))
                {
                    int qty = inputDict[key].Qunatity;
                    int price = Math.Min(inputDict[key].Price, item.Price);

                    if (qty == 0 || item.Qunatity == 0)
                    {
                        qty = 0;
                    }
                    else
                    {
                        qty += item.Qunatity;
                    }

                    inputDict[key] = item;
                    inputDict[key].Qunatity = qty;
                    inputDict[key].Price = price;
                }
                else
                {
                    inputDict[key] = item;
                }
            }

            return inputDict.Values.ToList<CsvInput>();
        }
    }
}