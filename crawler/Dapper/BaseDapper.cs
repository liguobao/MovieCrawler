using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;

namespace MovieCrawler.Dao
{
    public class BaseDapper
    {

        protected AppSettings _appSettings;

        public BaseDapper(IOptions<AppSettings> configuration)
        {
            this._appSettings = configuration.Value;
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(_appSettings.MySQLString);
        }
    }
}