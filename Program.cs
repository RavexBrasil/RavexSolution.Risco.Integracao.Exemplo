using System.Data.SQLite;
using NLog.Web;
using RavexSolution.Risco.Integracao.Client;
using RavexSolution.Risco.Integracao.Client.Extensions;
using RavexSolution.Risco.Integracao.Exemplo.Workers;

var xBuilder = WebApplication.CreateBuilder(args);
{
    CriarBanco(); // Apague esse método quando trocar a sua conexão de banco

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

void CriarBanco()
{
    SQLiteConnection.CreateFile("db.sqlite");
    using var mDbConnection = new SQLiteConnection("Data Source=db.sqlite;Version=3;");
    mDbConnection.Open();
    string sql = @"
        CREATE TABLE IF NOT EXISTS Posicoes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            IdPosicaoRisco INTEGER NOT NULL,
            IdRastreador INTEGER NOT NULL,
            IdVeiculo INTEGER NOT NULL,
            EventoDatahora DATETIME NOT NULL,
            CpfMotorista TEXT,
            Placa TEXT,
            GPS_Latitude DECIMAL,
            GPS_Longitude DECIMAL,
            GPS_Direcao INTEGER,
            Hodometro INTEGER,
            Ignicao INTEGER
        );

        CREATE INDEX IF NOT EXISTS Posicoes_IdPosicaoRisco ON Posicoes (IdPosicaoRisco);

        CREATE TABLE IF NOT EXISTS  Mensagens (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            IdMensagemRecebidaRisco INTEGER NOT NULL,
            IdEquipamento INTEGER NOT NULL,
            IdVeiculo INTEGER NOT NULL,
            DataCriacaoNoEquipamento DATETIME NOT NULL,
            Placa TEXT,
            TipoMacro INTEGER NOT NULL,
            Texto TEXT
        );
        CREATE INDEX Mensagens_IdMensagemRecebidaRisco ON Mensagens (IdMensagemRecebidaRisco);
    ";
    var command = new SQLiteCommand(sql, mDbConnection);
    command.ExecuteNonQuery();
}
