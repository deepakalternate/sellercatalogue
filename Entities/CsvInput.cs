using System;
using CsvHelper.Configuration.Attributes;

namespace sellercatalogue.Entities
{
    public class CsvInput
    {
        [Index(0)]
        public int SellerId { get; set; }
        [Index(1)]
        public int SellerProdId { get; set; }
        [Index(2)]
        public string Name { get; set; }
        [Index(3)]
        public string Description { get; set; }
        [Index(4)]
        public int Qunatity { get; set; }
        [Index(5)]
        public int Price { get; set; }
    }
    
}