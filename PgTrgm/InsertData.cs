using Bogus;

using Dapper.Contrib.Extensions;

using Npgsql;

using System;
using System.Text;
using System.Threading.Tasks;

namespace PgTrgm
{
    public class Person
    {
        public int id { get; set; }
        public string forenames { get; set; }
        public string forenames_normalized { get; set; }
        public string surname { get; set; }
        public string surname_normalized { get; set; }
    }

    class InsertData
    {
        static async Task Main(string[] args)
        {
            var connectionString = "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=name_search;";
            var generator = new Faker<Person>()
                .RuleFor(p => p.forenames, f => f.Name.FirstName())
                .RuleFor(p => p.forenames_normalized, (f, p) => Normalize(p.forenames))
                .RuleFor(p => p.surname, f => f.Name.LastName())
                .RuleFor(p => p.surname_normalized, (f, p) => Normalize(p.surname));

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                for (int i = 0; i < 9_000_000; i++)
                {
                    var person = generator.Generate();
                    await connection.InsertAsync(person);

                    if (i % 1000 == 0)
                        Console.WriteLine(i);
                }
            }
        }

        private static readonly string[] _punctuation = new string[] { ".", "'", ",", ";", ":", " ", "-" };
        static string Normalize(string name)
        {
            var builder = new StringBuilder(name.ToLowerInvariant());

            foreach (var p in _punctuation)
            {
                builder.Replace(p, "");
            }

            return builder.ToString();
        }
    }
}
