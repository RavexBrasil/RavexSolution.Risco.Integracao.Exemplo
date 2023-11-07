using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using RavexSolution.Risco.Integracao.Client;
using RavexSolution.Risco.Integracao.Client.HttpClients.Risco;
using RavexSolution.Risco.Integracao.Contracts.WebService;
using RavexSolution.Risco.Integracao.Exemplo.Entities;

namespace RavexSolution.Risco.Integracao.Exemplo.Workers;

public class MensagensWorker: RiscoMensagemServiceBase
{
    private readonly ILogger<RiscoMensagemServiceBase> _logger;
    private readonly IConfiguration _configuration;

    public MensagensWorker(IRiscoHttpClient pRiscoHttpClient
        , Configuracao pConfiguracao
        , ILogger<RiscoMensagemServiceBase> pLogger
        , IConfiguration pConfiguration)
        : base(pRiscoHttpClient, pConfiguracao, pLogger)
    {
        _logger = pLogger;
        _configuration = pConfiguration;
    }

    protected override async Task PersistirMensagens(IReadOnlyList<MensagemRecebidaDeEquipamentoV1> pMensagens)
    {
        try
        {
            var xMensagens = pMensagens.Select(p => new Mensagem
            {
                IdMensagemRecebidaRisco = p.IdMensagemRecebida,
                IdEquipamento = p.IdEquipamento,
                IdVeiculo = p.IdVeiculo,
                DataCriacaoNoEquipamento = p.DataCriacaoNoEquipamento,
                Placa = p.Placa,
                TipoMacro = p.TipoMacro,
                Texto = p.Texto
            }).ToList();

            await using var xConnection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
            xConnection.Insert(xMensagens);
        }
        catch (Exception xException)
        {
            _logger.LogError(xException, "{Mensagem}"
                , xException.Message);
            _logger.LogError("Erro ao persistir mensagens, finalizando aplicação.");
            Environment.Exit(-1);
        }
    }

    protected override async Task<long> ObterUltimoId()
    {
        try
        {
            await using var xConnection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
            var xRetorno = await xConnection.QueryFirstOrDefaultAsync<long?>("SELECT MAX(IdMensagemRecebidaRisco) FROM Mensagens");

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