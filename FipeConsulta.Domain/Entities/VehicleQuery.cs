namespace FipeConsulta.Domain.Entities
{
    public class VehicleQuery
    {
        public int CodigoTabelaReferencia { get; set; }  // Incluído no nome do arquivo
        public int CodigoTipoVeiculo { get; set; }
        public int CodigoMarca { get; set; }
        public int CodigoModelo { get; set; }
        public int AnoModelo { get; set; }
        public int CodigoTipoCombustivel { get; set; }
        public string TipoConsulta { get; set; }

        // Gera o nome do arquivo com base nos parâmetros da consulta, agora incluindo CodigoTabelaReferencia
        public string GenerateFileName()
        {
            return $"{CodigoTabelaReferencia}_{CodigoMarca}_{CodigoModelo}_{AnoModelo}_{CodigoTipoCombustivel}.txt";
        }
    }
}
