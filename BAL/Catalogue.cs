using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool SaveSellerCatalogue(List<CsvInput> input)
        {
            bool result = false;

            if (input != null && input.Any())
            {
                int sellerId = input.FirstOrDefault().SellerId;

                int versionId = _catalogueRepo.GetSellersLastActiveVersion(sellerId);

                if (versionId == -1 && !_catalogueRepo.SaveNewSeller(sellerId))
                {
                    return result;
                }

                input = RemoveDuplicates(input);

                if (versionId > 0)
                {
                    string remainingProdIds = string.Join(',', input.Select(x => x.SellerProdId));
                    List<CsvInput> remainingProds = _catalogueRepo.GetRemainingProducts(remainingProdIds, sellerId, versionId);
                    input.AddRange(remainingProds);
                }

                versionId = versionId == -1 ? 1 : versionId++;

            }

            return result;
        }

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