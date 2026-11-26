using DbUp;
using DbUp.Engine;

namespace OrbitPay.Migrations;

public static class DatabaseMigrator
{
    public static DatabaseUpgradeResult Run(string connectionString, bool logToConsole = true)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var builder = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseMigrator).Assembly)
            .WithTransactionPerScript();

        builder = logToConsole ? builder.LogToConsole() : builder.LogToNowhere();

        return builder.Build().PerformUpgrade();
    }
}