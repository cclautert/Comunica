using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica.Tests
{
    [TestClass]
    public class ConexaoTests
    {
        const string FIP = "168.62.186.237";
        const int FPorta = 4000;

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Conexao_Enviar_Erro_Quando_Valor_Ip_Em_Branco()
        {
            byte[] msg = null;
            Conexao.EnviarMensagem("", FPorta, msg);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Conexao_Enviar_Erro_Quando_Valor_Porta_Menor_Que_Zero()
        {
            byte[] msg = null;
            Conexao.EnviarMensagem(FIP, -1, msg);
        }

        [TestMethod]
        public void Conexao_Enviar_Verificar_Retorno()
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();
            
            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerRegistroStatus());
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }
    }
}
