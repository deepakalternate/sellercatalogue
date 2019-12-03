using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using sellercatalogue.Entities;

namespace sellercatalogue.DAL
{
    public class CatalogueRepository : ICatalogueRepository
    {
        private readonly IDbFactory _dbFactory;
        public CatalogueRepository(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // Function to get list of all the remaining proudcts in the seller's catalogue
        public List<CsvInput> GetRemainingProducts(string prodIds, int sellerId, int versionId)
        {
            List<CsvInput> products = new List<CsvInput>();

            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"select sellerprodid, name, description, qty, price from productversion where sellerid = {0} and versionid = {1} and sellerprodid not in ({2});", sellerId, versionId, prodIds);
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                        conn.Open();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    products.Add(new CsvInput {
                                        SellerId  = sellerId,
                                        SellerProdId = Convert.ToInt32(dr["sellerprodid"]),
                                        Qunatity = Convert.ToInt32(dr["qty"]),
                                        Price = Convert.ToInt32(dr["price"]),
                                        Name = Convert.ToString(dr["name"]),
                                        Description = Convert.ToString(dr["description"])
                                    });
                                }       
                            }
                        }
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }

            return products;
        }
        
        // Function to get the seller's last active version
        public int GetSellersLastActiveVersion(int sellerId)
        {
            int versionId = -1;

            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"select activeversion from seller where id = {0};", sellerId);
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                        conn.Open();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    versionId = Convert.ToInt32(dr["activeversion"]);
                                }       
                            }
                        }
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }

            return versionId;
        }

        // Function to insert the seller's data into database
        public bool SaveNewCatalogueData(List<CsvInput> inputs, int versionId)
        {
            bool isSaved = false;

            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"insert into productversion (sellerid, sellerprodid, name, description, qty, price, versionid) values {0};", CreateInsertData(inputs, versionId));
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                    
                        conn.Open();

                        isSaved = cmd.ExecuteNonQuery() > 0;
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }
            
            return isSaved;
        }

        // Private function to generate value string for insert query
        private string CreateInsertData(List<CsvInput> inputs, int versionId)
        {
            List<string> inps = new List<string>();

            foreach (var item in inputs)
            {
                inps.Add(string.Format("({0},{1},'{2}','{3}',{4},{5},{6})", item.SellerId, item.SellerProdId, item.Name, item.Description, item.Qunatity, item.Price, versionId));
            }
            return string.Join(",", inps);
        }

        // Function to register a new seller in the database
        public bool SaveNewSeller(int sellerId)
        {
            bool isSaved = false;

            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"insert into seller (id, name) values ({0}, 'seller-{0}')", sellerId);
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                    
                        conn.Open();

                        isSaved = cmd.ExecuteNonQuery() > 0;
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }
            
            return isSaved;
        }

        //Function which updates the seller's latest stock version
        public bool UpdateActiveVersion(int sellerId, int versionId)
        {
            bool isUpdated = false;

            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"UPDATE seller SET activeversion = {0} WHERE id = {1};", versionId, sellerId);
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                    
                        conn.Open();

                        isUpdated = cmd.ExecuteNonQuery() > 0;
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }
            
            return isUpdated;
        }

        // Function which gets the data of any version of the seller's catalogue
        public List<CsvInput> GenerateCatalogue(int sellerId, int versionId)
        {
            List<CsvInput> products = new List<CsvInput>();
            try
            {
                using (MySqlConnection conn = _dbFactory.GetConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = string.Format(@"select sellerprodid, name, description, qty, price from productversion where sellerid = {0} and versionid = {1};", sellerId, versionId);
                        cmd.CommandType = CommandType.Text;
                    
                        cmd.Connection = conn;
                        conn.Open();

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    products.Add(new CsvInput {
                                        SellerId  = sellerId,
                                        SellerProdId = Convert.ToInt32(dr["sellerprodid"]),
                                        Qunatity = Convert.ToInt32(dr["qty"]),
                                        Price = Convert.ToInt32(dr["price"]),
                                        Name = Convert.ToString(dr["name"]),
                                        Description = Convert.ToString(dr["description"])
                                    });
                                }       
                            }
                        }
                        
                        conn.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                throw;
            }

            return products;
        }
    }
}