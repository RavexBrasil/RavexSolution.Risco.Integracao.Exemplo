using System.ComponentModel.DataAnnotations.Schema;

namespace RavexSolution.Risco.Integracao.Exemplo.Entities;

[Dapper.Contrib.Extensions.Table("Mensagens")]
public class Mensagem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int IdMensagemRecebidaRisco { get; set; }
    public int IdEquipamento { get; set; }
    public int IdVeiculo { get; set; }
    public DateTime DataCriacaoNoEquipamento { get; set; }
    public string Placa { get; set; }
    public byte TipoMacro { get; set; }
    public string Texto { get; set; }
}