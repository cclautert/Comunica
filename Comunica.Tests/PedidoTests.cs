using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica.Tests
{
    [TestClass]
    public class PedidoTests
    {
        const string FIP = "168.62.186.237";
        const int FPorta = 4000;
        const short FIndice = 12100;
        const int FIndiceInicial = 12100;
        const int FIndiceFinal = 12150;

        public string Ip { get; set; }
        public int Porta { get; set; }
        public int IndiceInicial { get; set; }
        public int IndiceFinal { get; set; }

        public PedidoTests()
        {
            Ip = "";
            Porta = 0;
            IndiceInicial = 0;
            IndiceFinal = 0;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Pedido_SetarIP_Erro_Quando_Valor_Em_Branco()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarIP(Ip);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Pedido_SetarIP_Erro_Quando_IP_Invalido()
        {
            Pedido objPedido = new Pedido();
            Ip = "300.168.0.100";
            objPedido.SetarIP(Ip);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Pedido_SetarPorta_Erro_Quando_Valor_Zero()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarPorta(Porta);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Pedido_SetarPorta_Erro_Quando_Valor_Fora_Do_Range()
        {
            Pedido objPedido = new Pedido();
            Porta = 70000;
            objPedido.SetarPorta(Porta);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Pedido_SetarIndices_Erro_Quando_Valor_Indice_Final_Menor_Que_Indice_Inicial()
        {
            Pedido objPedido = new Pedido();
            IndiceInicial = 100;
            IndiceFinal = 50;
            objPedido.SetarIndices(IndiceInicial, IndiceFinal);
        }

        [TestMethod]
        public void Pedido_New_Setar_IP()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarIP(FIP);
            Assert.AreEqual(FIP, objPedido.Ip);
        }

        [TestMethod]
        public void Pedido_New_Setar_Porta()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarPorta(FPorta);
            Assert.AreEqual(FPorta, objPedido.Porta);
        }

        [TestMethod]
        public void Pedido_New_Setar_Indice_Inicial()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarIndiceInicial(FIndiceInicial);
            Assert.AreEqual(FIndiceInicial, objPedido.IndiceInicial);
        }

        [TestMethod]
        public void Pedido_New_Setar_Indice_Final()
        {
            Pedido objPedido = new Pedido();
            objPedido.SetarIndiceFinal(FIndiceFinal);
            Assert.AreEqual(FIndiceFinal, objPedido.IndiceFinal);
        }

        [TestMethod]
        public void Pedido_New_Validar_Indice()
        {
            Pedido objPedido = new Pedido();
            bool flgIndice = objPedido.ValidaIndiceInicialFinal(FIndiceInicial, FIndiceFinal);
            Assert.AreEqual(true, flgIndice);
        }
    }
}
