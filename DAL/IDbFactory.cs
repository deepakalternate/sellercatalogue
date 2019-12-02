using MySql.Data.MySqlClient;

namespace sellercatalogue.DAL
{
    public interface IDbFactory
    {
        MySqlConnection GetConnection();
    }
}