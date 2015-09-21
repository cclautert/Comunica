using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica.Tests
{
    [TestClass]
    public class ArquivoTests
    {
        public string FArquivo = "";
        public string[] FlstArquivo = null;

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Arquivo_LerPedido_Erro_Quando_Arquivo_Vazio()
        {
            Arquivo.Instancia.LerPedido(FArquivo);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Arquivo_LerPedido_Erro_Quando_Arquivo_Invalido()
        {
            FArquivo = "C:/Temp/Teste.txt";
            Arquivo.Instancia.LerPedido(FArquivo);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Arquivo_GravarLeiturasPedido_Erro_Arquivo_Vazio()
        {
            Arquivo.Instancia.GravarLeiturasPedido(FlstArquivo);
        }

        
    }
}
