using System.Reflection;
using DbUp;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__orbitdb")
    ?? throw new InvalidOperationException("Connection string 'orbitdb' não encontrada.");

EnsureDatabase.For.SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .WithTransactionPerScript()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.Error.WriteLine(result.Error);
    return 1;
}

Console.WriteLine("Migrations aplicadas com sucesso.");
return 0;