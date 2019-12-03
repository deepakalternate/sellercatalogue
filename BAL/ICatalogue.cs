using System.Collections.Generic;
using sellercatalogue.Entities;

namespace sellercatalogue.BAL
{
    public interface ICatalogue
    {
        bool SaveSellerCatalogue(List<CsvInput> input);
        int GetActiveCatalogueVersionId(int sellerId);
        string GenerateSellerCatalogueCSV(int sellerId, int versionId);
    }
}