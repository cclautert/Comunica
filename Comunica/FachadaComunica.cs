using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comunica
{
    public class FachadaComunica
    {
        public static int Reconexao = 0;

        private static object lockObj = new Object();
        private static volatile FachadaComunica instancia;

        private FachadaComunica()
        {

        }

        public static FachadaComunica Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (lockObj)
                    {
                        if (instancia == null)
                            instancia = new FachadaComunica();
                    }
                }
                return instancia;
            }
        }

        public void ExecutarOperacao(string pArquivo, int pTipoEnvio)
        {
            ExecutarPedido(Arquivo.Instancia.LerPedido(pArquivo), pTipoEnvio);
        }

        public void ExecutarPedido(IList<Pedido> plstPedidos, int pTipoEnvio)
        {
            try
            {
                if (!plstPedidos.Any())
                    throw new Exception("Lista de pedidos não pode ser vazio!");

                //Serial
                if (pTipoEnvio == 1)
                {
                    foreach (Pedido objPedido in plstPedidos)
                        SequenciaExecucao(objPedido.Ip, objPedido.Porta, objPedido.IndiceInicial, objPedido.IndiceFinal);
                }
                else
                    //Paralelo
                    if (pTipoEnvio == 2)
                        Parallel.For(0, plstPedidos.Count(), index => { SequenciaExecucao(plstPedidos[index].Ip, plstPedidos[index].Porta, plstPedidos[index].IndiceInicial, plstPedidos[index].IndiceFinal); });
                    else
                        throw new Exception("Tipo de envio inexistente!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
                throw new Exception("Exception : " + e.ToString());
            }
        }

        public static void SequenciaExecucao(string pIp, int pPort, int pIndiceInicial, int pIndiceFinal)
        {
            try
            {
                Dictionary<string, int> dicIndices = new Dictionary<string, int>();
                DateTime? DataHora = null;
                string NumeroSerie = "";
                double ValorEnergia = 0;
                List<string> Linhas = new List<string>();

                //0x01
                NumeroSerie = ObterNumeroSerie(pIp, pPort);
                Linhas.Add(NumeroSerie);
                Console.WriteLine(NumeroSerie);

                //0x02
                dicIndices = ObterRegistroStatus(pIp, pPort);

                dicIndices = SincronizarIndices(dicIndices, pIndiceInicial, pIndiceFinal);

                //0x03
                for (int i = dicIndices["Inicial"]; i <= dicIndices["Final"]; i++)
                {
                    DataHora = null;
                    ValorEnergia = 0;

                    if (DefinirIndices(pIp, pPort, (short)i))
                    {
                        //0x04
                        DataHora = ObterDataHora(pIp, pPort);

                        //0x05
                        ValorEnergia = ObterValorEnergia(pIp, pPort);

                        Linhas.Add(i.ToString() + ";" + DataHora.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + ";" + Math.Round(ValorEnergia, 2, MidpointRounding.ToEven));
                        Console.WriteLine(i.ToString() + ";" + DataHora.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + ";" + Math.Round(ValorEnergia, 2, MidpointRounding.ToEven));
                    }
                }

                Arquivo.Instancia.GravarLeiturasPedido(Linhas.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
                throw new Exception("Exception : " + e.ToString());
            }
        }

        public static Dictionary<string, int> SincronizarIndices(Dictionary<string, int> pIndices, int pIndiceInicial, int pIndiceFinal)
        {
            try
            {
                if (pIndices == null)
                    throw new Exception("Indices não pode ser vazio");

                if (pIndices["Inicial"] < pIndiceInicial)
                    pIndices["Inicial"] = pIndiceInicial;

                if (pIndices["Final"] > pIndiceFinal)
                    pIndices["Final"] = pIndiceFinal;

                if (pIndices["Inicial"] > pIndices["Final"])
                    pIndices["Final"] = pIndices["Inicial"];
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
                throw new Exception("Exception : " + e.ToString());
            }

            return pIndices;
        }

        public static string ObterNumeroSerie(string pIp, int pPort)
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());           

            if (!objFuncao.VerificaCodFuncaoRetorno(retorno))
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //ValidaCheckSum
            if (!objFuncao.ValidarCheckSumRetorno(retorno))
                //Se um lado receber um frame com o checksum errado, ele deve enviar um frame de Erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //Verifica Frame de Erro
            if (objFuncao.VerificaFrameDeErro(retorno))
                // Se um lado receber um frame de Erro, ele deve reenviar o último frame enviado
                retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());

            //Verifica Tamanho do Frame
            if (!objFuncao.ValidarTamanhoMensagemRetorno(retorno))
                // Se um lado receber um frame com dados com tamanho inesperado ou com formato inválido, ele deve enviar um frame de erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            return objFuncao.ObterMenssagemRetornoNumeroSerie(retorno);
        }

        public static Dictionary<string, int> ObterRegistroStatus(string pIp, int pPort)
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerRegistroStatus());
            
            if (!objFuncao.VerificaCodFuncaoRetorno(retorno))
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //ValidaCheckSum
            if (!objFuncao.ValidarCheckSumRetorno(retorno))
                //Se um lado receber um frame com o checksum errado, ele deve enviar um frame de Erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //Verifica Frame de Erro
            if (objFuncao.VerificaFrameDeErro(retorno))
                // Se um lado receber um frame de Erro, ele deve reenviar o último frame enviado
                retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());

            //Verifica Tamanho do Frame
            if (!objFuncao.ValidarTamanhoMensagemRetorno(retorno))
                // Se um lado receber um frame com dados com tamanho inesperado ou com formato inválido, ele deve enviar um frame de erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            return objFuncao.ObterIndicesRegistroStatus(retorno);
        }

        public static bool DefinirIndices(string pIp, int pPort, short pIndice)
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerValidarIndice(pIndice));
            
            if (!objFuncao.VerificaCodFuncaoRetorno(retorno))
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //ValidaCheckSum
            if (!objFuncao.ValidarCheckSumRetorno(retorno))
                //Se um lado receber um frame com o checksum errado, ele deve enviar um frame de Erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //Verifica Frame de Erro
            if (objFuncao.VerificaFrameDeErro(retorno))
                // Se um lado receber um frame de Erro, ele deve reenviar o último frame enviado
                retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());            

            //Verifica Tamanho do Frame
            if (!objFuncao.ValidarTamanhoMensagemRetorno(retorno))
                // Se um lado receber um frame com dados com tamanho inesperado ou com formato inválido, ele deve enviar um frame de erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            if (!objFuncao.VerificaSucesso(retorno))
                return false;

            if (!objFuncao.ValidarRetornoRegistroPorIndices(retorno))
                return false;

            return true;
        }

        public static DateTime? ObterDataHora(string pIp, int pPort)
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerRegistroDataHora());
            
            if (!objFuncao.VerificaCodFuncaoRetorno(retorno))
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //ValidaCheckSum
            if (!objFuncao.ValidarCheckSumRetorno(retorno))
                //Se um lado receber um frame com o checksum errado, ele deve enviar um frame de Erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //Verifica Frame de Erro
            if (objFuncao.VerificaFrameDeErro(retorno))
                // Se um lado receber um frame de Erro, ele deve reenviar o último frame enviado
                retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());

            //Verifica Tamanho do Frame
            if (!objFuncao.ValidarTamanhoMensagemRetorno(retorno))
                // Se um lado receber um frame com dados com tamanho inesperado ou com formato inválido, ele deve enviar um frame de erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            return objFuncao.ObterRetornoDataHora(retorno);
        }

        public static double ObterValorEnergia(string pIp, int pPort)
        {
            byte[] retorno = null;
            Funcao objFuncao = new Funcao();

            retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerValorEnergia());
            
            if (!objFuncao.VerificaCodFuncaoRetorno(retorno))
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //ValidaCheckSum
            if (!objFuncao.ValidarCheckSumRetorno(retorno))
                //Se um lado receber um frame com o checksum errado, ele deve enviar um frame de Erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            //Verifica Frame de Erro
            if (objFuncao.VerificaFrameDeErro(retorno))
                // Se um lado receber um frame de Erro, ele deve reenviar o último frame enviado
                retorno = Conexao.Enviar(pIp, pPort, objFuncao.LerNumeroSerie());

            //Verifica Tamanho do Frame
            if (!objFuncao.ValidarTamanhoMensagemRetorno(retorno))
                // Se um lado receber um frame com dados com tamanho inesperado ou com formato inválido, ele deve enviar um frame de erro
                Conexao.Enviar(pIp, pPort, objFuncao.FrameDeErro());

            return objFuncao.ObterRetornoValorEnergia(retorno);
        }        
    }
}
