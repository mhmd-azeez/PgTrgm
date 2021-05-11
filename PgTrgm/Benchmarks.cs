using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Npgsql;

using System.Threading.Tasks;

namespace PgTrgm
{
    public class Program
    {
        public static void Main(string[] args) =>
            BenchmarkRunner.Run<Benchmarks>();
    }

    public class Benchmarks
    {
        private NpgsqlConnection _indexDb;
        private NpgsqlConnection _noIndexDb;

        [GlobalSetup]
        public async Task Setup()
        {
            var indexConnectionString = "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=name_search_index;";
            _indexDb = new NpgsqlConnection(indexConnectionString);
            await _indexDb.OpenAsync();

            var connectionString = "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=name_search;";
            _noIndexDb = new NpgsqlConnection(connectionString);
            await _noIndexDb.OpenAsync();
        }

        [Benchmark]
        public async Task LikeOnGinIndex()
        {
            const string sql = @"select * from persons p 
where surname_normalized like '%tche%' and forenames_normalized like '%nde%'";

            using (var command = _indexDb.CreateCommand())
            {
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }

        [Benchmark]
        public async Task Like()
        {
            const string sql = @"select * from persons p 
where surname_normalized like '%tche%' and forenames_normalized like '%nde%'";

            using (var command = _noIndexDb.CreateCommand())
            {
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }

        [Benchmark]
        public async Task SimilarityOnGINIndex()
        {
            const string sql = @"select id, forenames, surname, ((similarity('mitchel', surname_normalized) + similarity('andersen', forenames_normalized)) / 2) as score from persons p
where forenames_normalized % 'andersen' and surname_normalized % 'mitchel'
order by score desc
limit 10";

            using (var command = _indexDb.CreateCommand())
            {
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
