using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using RavexSolution.Risco.Integracao.Client;
using RavexSolution.Risco.Integracao.Client.HttpClients.Risco;
using RavexSolution.Risco.Integracao.Contracts.WebService;
using RavexSolution.Risco.Integracao.Exemplo.Entities;

namespace RavexSolution.Risco.Integracao.Exemplo.Workers;

public class PosicoesWorker: RiscoPosicaoServiceBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RiscoPosicaoServiceBase> _logger;

    public PosicoesWorker(IRiscoHttpClient riscoHttpClient
        , Configuracao pConfiguracao
        , IConfiguration pConfiguration
        , ILogger<RiscoPosicaoServiceBase> pLogger)
        : base(riscoHttpClient, pConfiguracao, pLogger)
    {
        _configuration = pConfiguration;
        _logger = pLogger;
    }

    protected override async Task PersistirPosicoes(IReadOnlyList<PosicoesEmEquipamentoV3> pPosicoes)
    {
        try
        {
            var xPosicoes = pPosicoes.Select(p => new Posicao
            {
                IdPosicaoRisco = p.IdPosicao,
                IdRastreador = p.IdRastreador,
                IdVeiculo = p.IdVeiculo,
                EventoDatahora = p.Evento_Datahora,
                CpfMotorista = p.CpfMotorista,
                Placa = p.Placa,
                GPS_Direcao = p.GPS_Direcao,
                GPS_Latitude = p.GPS_Latitude,
                GPS_Longitude = p.GPS_Longitude,
                Hodometro = p.Hodometro,
                Ignicao = p.Ignicao
            }).ToList();

            await using var xConnection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
            xConnection.Insert(xPosicoes);
        }
        catch (Exception xException)
        {
            _logger.LogError(xException, "{Mensagem}"
                , xException.Message);
            _logger.LogError("Erro ao persistir posições, finalizando aplicação.");
            Environment.Exit(-1);
        }
    }

    protected override async Task<long> ObterUltimoId()
    {
        try
        {
            await using var xConnection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
            var xRetorno = await xConnection.QueryFirstOrDefaultAsync<long?>("SELECT MAX(IdPosicaoRisco) FROM Posicoes");

            return xRetorno ?? 0;
        }
        catch (Exception xException)
        {
            _logger.LogError(xException, "{Mensagem}"
                , xException.Message);
            _logger.LogError("Erro ao obter cursor de posições, finalizando aplicação.");
            Environment.Exit(-1);
            throw;
        }
    }
}