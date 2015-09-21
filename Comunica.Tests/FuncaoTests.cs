using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Comunica.Tests
{
    [TestClass]
    public class FuncaoTests
    {
        const string FIP = "168.62.186.237";
        const int FPorta = 4000;
        const short FIndice = 12100;

        const byte CodFNumeroSerie = 0x01;
        const byte CodFRegistroStatus = 0x02;
        const byte CodFIndice = 0x03;
        const byte CodFDataHora = 0x04;
        const byte CodFValorEnergia = 0x05;
        const byte FrameHeader = 0x7D;
        const byte FValorZero = 0x00;
        const byte FValorErro = 0xFF;

        const byte CodRetornoFNumeroSerie = 0x81;
        const byte CodRetornoFRegistroStatus = 0x82;
        const byte CodRetornoFIndice = 0x83;
        const byte CodRetornoFDataHora = 0x84;
        const byte CodRetornoFValorEnergia = 0x85;

        private byte FHeader { get; set; }
        private byte TamanhoMsg { get; set; }
        private byte CodFUncao { get; set; }
        private byte[] Msg { get; set; }
        private byte CheckSum { get; set; }

        public FuncaoTests()
        {
            FHeader = 0x00;
            TamanhoMsg = 0x00;
            CodFUncao = 0x00;
            Msg = null;
            CheckSum = 0x00;
        }

        #region TestMethod ExpectedException

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Funcao_SetarFHeader_Erro_Quando_Valor_Zero()
        {     
            Funcao objFuncao = new Funcao();
            objFuncao.SetarFHeader(FHeader);            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Funcao_SetarCodFUncao_Erro_Quando_Valor_Zero()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarCodFUncao(CodFUncao);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Funcao_MontarFrame_Erro_Quando_Valor_Zero()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.MontarFrameCompleto(FHeader, TamanhoMsg, CodFUncao, Msg, CheckSum);
        }

        #endregion

        #region TestMethod

        [TestMethod]        
        public void Funcao_New_SetarFHeader()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarFHeader(FrameHeader);
            Assert.AreEqual(FrameHeader, objFuncao.FHeader);
        }

        [TestMethod]
        public void Funcao_New_SetarCodFUncao()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarCodFUncao(CodFNumeroSerie);
            Assert.AreEqual(CodFNumeroSerie, objFuncao.CodFuncao);
        }

        [TestMethod]
        public void Funcao_New_SetarTamanhoMsg()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarTamanhoMsg((byte)2);
            Assert.AreEqual((byte)2, objFuncao.TamanhoMsg);
        }

        [TestMethod]
        public void Funcao_New_SetarIndiceInicial()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarIndiceInicial(100);
            Assert.AreEqual(100, objFuncao.IndiceInicial);
        }

        [TestMethod]
        public void Funcao_New_SetarIndiceFinal()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarIndiceFinal(100);
            Assert.AreEqual(100, objFuncao.IndiceFinal);
        }

        [TestMethod]
        public void Funcao_New_SetarIndices()
        {
            Funcao objFuncao = new Funcao();
            objFuncao.SetarIndices(300, 500);
            Assert.AreEqual(300, objFuncao.IndiceInicial);
            Assert.AreEqual(500, objFuncao.IndiceFinal);
        }

        [TestMethod]
        public void Funcao_New_SetarFHeader_Not_Null()
        {
            Funcao objFuncao = new Funcao();
            Assert.IsNotNull(objFuncao.FHeader);
        }

        [TestMethod]
        public void Funcao_New_SetarTamanhoMsg_Not_Null()
        {
            Funcao objFuncao = new Funcao();
            Assert.IsNotNull(objFuncao.TamanhoMsg);
        }

        [TestMethod]
        public void Funcao_New_CalcularCheckSumEnvio()
        {
            Funcao objFuncao = new Funcao();
            byte[] arrbyte= null;
            objFuncao.CalcularCheckSumEnvio(FValorZero, CodFNumeroSerie, arrbyte);
            Assert.AreEqual(0x01, objFuncao.CheckSum);
        }

        [TestMethod]
        public void Funcao_New_CalcularCheckSumRetorno()
        {
            Funcao objFuncao = new Funcao();
            byte[] arrbyte = null;
            objFuncao.CalcularCheckSumRetorno(FValorZero, CodFNumeroSerie, arrbyte);
            Assert.AreEqual(0x01, objFuncao.CheckSum);
        }

        [TestMethod]
        public void Funcao_New_Montar_FrameDeErro()
        {
            Funcao objFuncao = new Funcao();
            byte[] arrbyte = objFuncao.FrameDeErro();
            Assert.AreEqual(FValorErro, arrbyte[2]);
        }

        [TestMethod]
        public void Funcao_New_Verificar_FrameDeErro()
        {
            Funcao objFuncao = new Funcao();
            List<byte> lstByte = new List<byte>();
            lstByte.Add(FrameHeader);
            lstByte.Add(FValorZero);
            lstByte.Add(FValorErro);

            bool flgFrameErro = objFuncao.VerificaFrameDeErro(lstByte.ToArray());
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Verificar_Sucesso_Definicao_Indices()
        {
            Funcao objFuncao = new Funcao();
            List<byte> lstByte = new List<byte>();
            lstByte.Add(FrameHeader);
            lstByte.Add(FValorZero);
            lstByte.Add(0x83);
            lstByte.Add(FValorZero);

            bool flgFrameErro = objFuncao.VerificaSucesso(lstByte.ToArray());
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_CheckSum()
        {
            Funcao objFuncao = new Funcao();
            List<byte> lstByte = new List<byte>();
            lstByte.Add(FrameHeader);
            lstByte.Add(CodFNumeroSerie);
            lstByte.Add(FValorZero);
            lstByte.Add(FValorZero);
            lstByte.Add(0x01);

            bool flgFrameErro = objFuncao.ValidarCheckSumRetorno(lstByte.ToArray());
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Obter_Menssagem_Retorno_Numero_Serie_()
        {
            Funcao objFuncao = new Funcao();
            List<byte> lstByte = new List<byte>();
            lstByte.Add(FrameHeader);
            lstByte.Add(0x03);//Tamanho Msg
            lstByte.Add(0x081);//Função Retorno
            lstByte.Add(0x41);//A
            lstByte.Add(0x42);//B
            lstByte.Add(0x00);//0
            lstByte.Add(0x01);//CheckSum

            string strMsg = objFuncao.ObterMenssagemRetornoNumeroSerie(lstByte.ToArray());
            Assert.AreEqual("AB", strMsg);
        }

        [TestMethod]
        public void Funcao_New_Enviar_Codigo_Resposa_Receber_Frame_De_Erro()
        {
            // Se o medidor receber um código de uma resposta, ele enviará um frame de erro. 
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();
            List<byte> lstByte = new List<byte>();
            lstByte.Add(FrameHeader);
            lstByte.Add(FValorZero);
            lstByte.Add(0x83);
            lstByte.Add(FValorZero);
            lstByte.Add(0x83);

            retorno = Conexao.Enviar(FIP, FPorta, lstByte.ToArray());
            bool flgFrameErro = objFuncao.VerificaFrameDeErro(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_Frame_Funcao_Retorno_Ler_Numero_Serie()
        {                        
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerNumeroSerie());
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_Frame_Funcao_Retorno_Ler_Registro_Status()
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerRegistroStatus());
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_Frame_Funcao_Retorno_Ler_Validar_Indice()
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerValidarIndice(FIndice));
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_Frame_Funcao_Retorno_Ler_Registro_DataHora()
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerRegistroDataHora());
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }

        [TestMethod]
        public void Funcao_New_Validar_Frame_Funcao_Retorno_Ler_Valor_Energia()
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(FIP, FPorta, objFuncao.LerValorEnergia());
            bool flgFrameErro = objFuncao.VerificaCodFuncaoRetorno(retorno);
            Assert.AreEqual(true, flgFrameErro);
        }
        

        #endregion
    }
}
