using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MovieCrawler.Dao
{
    public class MovieDapper : BaseDapper
    {
        public MovieDapper(IOptions<AppSettings> options) : base(options)
        {
        }

        public int BulkInsert(List<DBMovie> movies)
        {
            if (movies == null || movies.Count == 0)
            {
                return 0;
            }

            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute(@"INSERT INTO movie
                        (`Name`,`Intro`,`Cover`,`Link`,`Type`,`Resources`,`PublishTime`,`CreateTime`)
                        VALUES (@Name,@Intro,@Cover,@Link,@Type,@Resources,@PublishTime, now()) ON DUPLICATE KEY UPDATE UpdateTime=now();",
                        movies.Where(h => !string.IsNullOrEmpty(h.Resources)), transaction: transaction);
                transaction.Commit();
                return result;
            }

        }



    }
}