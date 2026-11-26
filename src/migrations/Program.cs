using OrbitPay.Migrations;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__orbitdb")
    ?? throw new InvalidOperationException("Connection string 'orbitdb' não encontrada.");

var result = DatabaseMigrator.Run(connectionString);

if (!result.Successful)
{
    Console.Error.WriteLine(result.Error);
    return 1;
}

Console.WriteLine("Migrations aplicadas com sucesso.");
return 0;