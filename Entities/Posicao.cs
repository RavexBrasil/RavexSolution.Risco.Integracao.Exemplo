using System.ComponentModel.DataAnnotations.Schema;

namespace RavexSolution.Risco.Integracao.Exemplo.Entities;

[Table("Posicoes")]
public class Posicao
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public long IdPosicaoRisco { get; set; }
    public int IdRastreador { get; set; }
    public int? IdVeiculo { get; set; }
    public DateTime EventoDatahora { get; set; }
    public string? CpfMotorista { get; set; }
    public string? Placa { get; set; }
    public Decimal? GPS_Latitude { get; set; }
    public Decimal? GPS_Longitude { get; set; }
    public int? GPS_Direcao { get; set; }
    public int? Hodometro { get; set; }
    public bool? Ignicao { get; set; }
}