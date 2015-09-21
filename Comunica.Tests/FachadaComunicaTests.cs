using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica.Tests
{
    [TestClass]
    public class FachadaComunicaTests
    {
        public string Arquivo = "";
        const string FIP = "168.62.186.237";
        const int FPorta = 4000;
        const int FIndiceInicial = 12100;
        const int FIndiceFinal = 12150;

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_ExecutarOperacao_Erro_Quando_Arquivo_Vazio()
        {
            FachadaComunica.Instancia.ExecutarOperacao(Arquivo, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_ExecutarOperacao_Erro_Quando_Tipo_Envio_Nao_Existe()
        {
            string strArquivo = "C:/Teste.txt";
            FachadaComunica.Instancia.ExecutarOperacao(strArquivo, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_ExecutarPedido_Erro_Quando_Lista_Pedido_Null()
        {
            FachadaComunica.Instancia.ExecutarPedido(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_ExecutarPedido_Erro_Quando_Lista_Pedido_Vazio()
        {
            List<Pedido> lstPedido = new List<Pedido>();
            FachadaComunica.Instancia.ExecutarPedido(lstPedido, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_ExecutarPedido_Erro_Quando_Tipo_Envio_Nao_Existe()
        {
            List<Pedido> lstPedido = new List<Pedido>();
            lstPedido.Add(new Pedido { Ip = FIP, Porta = FPorta, IndiceInicial = FIndiceInicial, IndiceFinal = FIndiceFinal });
            FachadaComunica.Instancia.ExecutarPedido(lstPedido, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_SequenciaExecucao_Erro_Quando_Ip_Vazio()
        {
            FachadaComunica.SequenciaExecucao("", FPorta, FIndiceInicial, FIndiceFinal);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FachadaComunica_SincronizarIndices_Erro_Quando_Dicionario_Vazio()
        {
            Dictionary<string, int> dicIndices = null;
            FachadaComunica.SincronizarIndices(dicIndices, FIndiceInicial, FIndiceFinal);
        }        
    }
}
