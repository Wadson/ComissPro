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
            public string Tipo { get; set; }
            public int QuantidadePorBloco { get; set; }
        }

        public class EntregasModel
        {
            public int EntregaID { get; set; }
            public long VendedorID { get; set; }
            public long ProdutoID { get; set; }
            public long QuantidadeEntregue { get; set; }
            public DateTime? DataEntrega { get; set; } = DateTime.Now;
            public bool PrestacaoRealizada { get; set; } = false;
            public string NomeVendedor { get; set; }
            public string NomeProduto { get; set; }
            public double Preco { get; set; }
            public double Total { get; set; }
            public double Comissao { get; set; } // Percentual direto (ex.: 40 para 40%)

            public override string ToString()
            {
                return $"{NomeVendedor} - Entrega {EntregaID} - {QuantidadeEntregue} unidades ({DataEntrega:dd/MM/yyyy})";
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
            public string NomeVendedor { get; set; }
            public int VendedorID { get; set; }
        }
    }
}
