using System.Collections.Generic;
using sellercatalogue.Entities;

namespace sellercatalogue.DAL
{
    public interface ICatalogueRepository
    {
        int GetSellersLastActiveVersion(int sellerId);
        bool SaveNewSeller(int sellerId);
        List<CsvInput> GetRemainingProducts(string prodIds, int sellerId, int versionId);
        bool SaveNewCatalogueData(List<CsvInput> inputs, int versionId);
        bool UpdateActiveVersion(int sellerId, int versionId);
        List<CsvInput> GenerateCatalogue(int sellerId, int versionId);
    }
}