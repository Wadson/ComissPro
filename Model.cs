using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComissPro
{
    internal class Model
    {
        // Model Classes
        public class FluxoCaixaModel
        {
            public int FluxoCaixaID { get; set; }
            public string TipoMovimentacao { get; set; } // "ENTRADA" ou "SAIDA"
            public double Valor { get; set; }
            public DateTime DataMovimentacao { get; set; } = DateTime.Now;
            public string Descricao { get; set; }
            public int? PrestacaoID { get; set; } // Nullable para movimentações não vinculadas a prestações
        }
        public class UsuarioMODEL
        {
            public int UsuarioID { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
            public string TipoUsuario { get; set; }
        }
        public class VendaMODEL
        {
            public int VendaID { get; set; }
            public int VendedorID { get; set; }
            public int ProdutoID { get; set; }
            public int QuantidadeVendida { get; set; }
            public DateTime DataVenda { get; set; }
        }
        public class ComissaoMODEL
        {
            public int ComissaoID { get; set; }
            public int VendedorID { get; set; }
            public double ValorComissao { get; set; }
            public DateTime DataGeracao { get; set; }
        }


        public class VendedorMODEL
        {
            public int VendedorID { get; set; }
            public string Nome { get; set; }
            public string CPF { get; set; }
            public string Telefone { get; set; }
            public double Comissao { get; set; }
        }

        public class ProdutoMODEL
        {
            public int ProdutoID { get; set; }
            public string NomeProduto { get; set; }
            public double Preco { get; set; }
            public string Unidade { get; set; }                       
        }

        public class EntregasModel
        {
            public int EntregaID { get; set; }
            public int VendedorID { get; set; }
            public int ProdutoID { get; set; } // Já existe, mas não será exibido
            public int QuantidadeEntregue { get; set; }
            public DateTime? DataEntrega { get; set; }
            public double Preco { get; set; } // Preço Unitário
            public double Comissao { get; set; }
            public string Nome { get; set; }
            public string NomeProduto { get; set; }
            public double Total { get; set; } // Preço Total (QuantidadeEntregue * Preco)
            public bool Prestacaorealizada { get; set; } // 0 = Não, 1 = Sim

            public override string ToString()
            {
                return $"{Nome} - Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
            }
        }

        public class PrestacaoContasModel
        {
            public int PrestacaoID { get; set; }
            public int EntregaID { get; set; }
            public int QuantidadeVendida { get; set; }
            public int QuantidadeDevolvida { get; set; }
            public double ValorRecebido { get; set; }
            public double Comissao { get; set; }
            public DateTime DataPrestacao { get; set; } = DateTime.Now;
            public string Nome { get; set; }
            public int VendedorID { get; set; }
        }
        // Para Relatório de Desempenho de Vendas
        public class DesempenhoVendasModel
        {
            public long VendedorID { get; set; }
            public string Nome { get; set; }
            public int EntregaID { get; set; }
            public long QuantidadeEntregue { get; set; }
            public int QuantidadeVendida { get; set; }
            public int QuantidadeDevolvida { get; set; }
            public double ValorRecebido { get; set; }
            public double Comissao { get; set; }
        }

        // Para Relatório Geral de Vendas e Comissões
        public class GeralVendasComissoesModel
        {
            public long VendedorID { get; set; }
            public string Nome { get; set; }
            public long TotalEntregue { get; set; }
            public long TotalVendido { get; set; }
            public long TotalDevolvido { get; set; }
            public double TotalRecebido { get; set; }
            public double TotalComissao { get; set; }
        }
    }
}
