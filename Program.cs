using Dapper;
using Microsoft.Data.Sqlite;
using NLog.Web;
using RavexSolution.Risco.Integracao.Client;
using RavexSolution.Risco.Integracao.Client.Extensions;
using RavexSolution.Risco.Integracao.Exemplo.Workers;

var xBuilder = WebApplication.CreateBuilder(args);
{
    CriarBancoDeDadosSqlite(); // Apague esse método quando trocar a sua conexão do banco de dados

    xBuilder.Configuration.AddUserSecrets<Program>();
    xBuilder.Host.UseNLog();

    var xConfiguracao = xBuilder.Configuration
        .GetSection(nameof(Configuracao))
        .Get<Configuracao>(p => p.ErrorOnUnknownConfiguration = true);

    xBuilder.Services
        .AdicionarRiscoService<PosicoesWorker>(xConfiguracao)
        .AdicionarRiscoService<MensagensWorker>(xConfiguracao)
        .AddSingleton(xBuilder.Configuration)
        ;
}

var xBuild = xBuilder.Build();
{
    var xPosicoesWorker = xBuild.Services.GetRequiredService<PosicoesWorker>();
    var xMensagensWorker = xBuild.Services.GetRequiredService<MensagensWorker>();

    var xWorkers = new[]
    {
        xPosicoesWorker.RunAsync(),
        xMensagensWorker.RunAsync()
    };
    await xBuild.StartAsync();
    await Task.WhenAny(xWorkers);
}

void CriarBancoDeDadosSqlite()
{
    var xDbPath = Path.Combine(Environment.CurrentDirectory, "db.sqlite");
    if (!File.Exists(xDbPath))
        File.Create(xDbPath).Close();

    using var xSqlConnection = new SqliteConnection("Data Source=" + xDbPath);
    {
        var xSql = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "tabelas.sql"));

        xSqlConnection.Open();
        xSqlConnection.Execute(xSql);
    }
}