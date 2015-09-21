using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica
{
    public class Funcao
    {
        const byte CodFNumeroSerie = 0x01;
        const byte CodFRegistroStatus = 0x02;
        const byte CodFIndice = 0x03;
        const byte CodFDataHora = 0x04;
        const byte CodFValorEnergia = 0x05;
        const byte FrameHeader = 0x7D;
        const byte FValorZero = 0x00;
        const byte FValorErro = 0xFF;

        const byte CodRetFNumeroSerie = 0x81;
        const byte CodRetFRegistroStatus = 0x82;
        const byte CodRetFIndice = 0x83;
        const byte CodRetFDataHora = 0x84;
        const byte CodRetFValorEnergia = 0x85;

        public byte FHeader { get; set; }
        public byte TamanhoMsg { get; set; }
        public byte CodFuncao { get; set; }
        public byte[] Msg { get; set; }
        public byte CheckSum { get; set; }
        public int IndiceInicial { get; set; }
        public int IndiceFinal { get; set; }
        public IList<int> Indices { get; set; }

        public Funcao()
        {

        }        

        public void SetarFHeader(byte pFHeader)
        {
            if (pFHeader == FValorZero)
                throw new Exception("FHeader não pode ser Zero"); 

            FHeader = pFHeader;
        }

        public void SetarTamanhoMsg(byte pTamanhoMsg)
        {
            TamanhoMsg = pTamanhoMsg;
        }

        public void SetarCodFUncao(byte pCodFuncao)
        {
            if (pCodFuncao == FValorZero)
                throw new Exception("Função não pode ser 0x00!");

            CodFuncao = pCodFuncao;
        }

        public void SetarMsg(byte[] pMsg)
        {
            Msg = pMsg;
        }

        public void SetarCheckSum(byte pCheckSum)
        {
            CheckSum = pCheckSum;
        }

        public void SetarIndiceInicial(int pIndiceInicial)
        {
            IndiceInicial = pIndiceInicial;
        }

        public void SetarIndiceFinal(int pIndiceFinal)
        {
            IndiceFinal = pIndiceFinal;
        }

        public void SetarIndices(int pIndiceInicial, int pIndiceFinal)
        {
            SetarIndiceInicial(pIndiceInicial);
            SetarIndiceFinal(pIndiceFinal);

            IndiceInicial = pIndiceInicial;
            IndiceFinal = pIndiceFinal;
        }

        public List<byte> MontarFrameCompleto(byte pFHeader, byte pTamanhoMsg, byte pCodFuncao, byte[] pMsg, byte pCheckSum)
        {
            SetarFHeader(pFHeader);
            SetarTamanhoMsg(pTamanhoMsg);
            SetarCodFUncao(pCodFuncao);
            SetarMsg(pMsg);
            SetarCheckSum(CalcularCheckSumEnvio(pTamanhoMsg, pCodFuncao, pMsg));

            List<byte> Frame = new List<byte>();
            Frame.Add(FHeader);
            Frame.Add(TamanhoMsg);
            Frame.Add(CodFuncao);
            if (pMsg != null)
                Frame.AddRange(pMsg);            
            Frame.Add(CheckSum);

            return Frame;
        }

        public byte[] MontarFrame(byte pCodFuncao, byte[] pMsg)
        {
            if (pMsg != null)
                return MontarFrameCompleto(FrameHeader, (byte)pMsg.Length, pCodFuncao, pMsg, FValorZero).ToArray();
            else
                return MontarFrameCompleto(FrameHeader, FValorZero, pCodFuncao, pMsg, FValorZero).ToArray();
        }

        public byte CalcularCheckSumEnvio(byte pTamanhoMsg, byte pCodFUncao, byte[] pMsgRetorno)
        {
            byte checkSum = 0x00;

            checkSum ^= pTamanhoMsg;
            checkSum ^= pCodFUncao;

            for (int i = 0; i < TamanhoMsg; i++)            
                checkSum ^= pMsgRetorno[i];            

            SetarCheckSum(checkSum);

            return checkSum;
        }

        public byte CalcularCheckSumRetorno(byte pTamanhoMsg, byte pCodFUncao, byte[] pMsgRetorno)
        {
            byte checkSum = 0x00;

            checkSum ^= pTamanhoMsg;
            checkSum ^= pCodFUncao;

            for (int i = 0; i < TamanhoMsg; i++)
                checkSum ^= pMsgRetorno[i + 3];

            SetarCheckSum(checkSum);

            return checkSum;
        }

        public byte[] FrameDeErro()
        {
            return MontarFrame(FValorErro, null);
        }

        public bool VerificaFrameDeErro(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[2] != FValorZero)
                if (pMsgRetorno[2] == FValorErro)
                    return true;

            return false;
        }

        public bool VerificaSucesso(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[3] == FValorZero)
                return true;

            return false;
        }

        public bool VerificaCodFuncaoRetorno(byte[] pMsgRetorno)
        {
            if (CodFuncao == CodFNumeroSerie)
                if (pMsgRetorno[2] == CodRetFNumeroSerie)
                    return true;

            if (CodFuncao == CodFRegistroStatus)
                if (pMsgRetorno[2] == CodRetFRegistroStatus)
                    return true;

            if (CodFuncao == CodFIndice)
                if (pMsgRetorno[2] == CodRetFIndice)
                    return true;

            if (CodFuncao == CodFDataHora)
                if (pMsgRetorno[2] == CodRetFDataHora)
                    return true;

            if (CodFuncao == CodFValorEnergia)
                if (pMsgRetorno[2] == CodRetFValorEnergia)
                    return true;

            return false;
        }

        public byte[] LerNumeroSerie()
        {
            return MontarFrame(CodFNumeroSerie, null);
        }

        public bool ValidarCheckSumRetorno(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[1] != FValorZero)
                SetarTamanhoMsg(pMsgRetorno[1]);

            if (pMsgRetorno[2] != FValorZero)
                SetarCodFUncao(pMsgRetorno[2]);
            
            byte CheckSumRecebido = pMsgRetorno[TamanhoMsg + 3];

            CalcularCheckSumRetorno(TamanhoMsg, CodFuncao, pMsgRetorno);

            if (CheckSum != CheckSumRecebido)
                return false;

            return true;
        }

        public bool ValidarTamanhoMensagemRetorno(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[1] != FValorZero)
                SetarTamanhoMsg(pMsgRetorno[1]);

            return true;
        }

        public string ObterMenssagemRetornoNumeroSerie(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[1] != FValorZero)
               SetarTamanhoMsg(pMsgRetorno[1]);

            byte[] Resposta = new byte[TamanhoMsg - 1];            

            for (int i = 0; i < TamanhoMsg - 1; i++)            
                Resposta[i] = pMsgRetorno[i + 3];

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(Resposta);
        }

        public byte[] LerRegistroStatus()
        {
            return MontarFrame(CodFRegistroStatus, null);            
        }
        
        public Dictionary<string, int> ObterIndicesRegistroStatus(byte[] pMsgRetorno)
        {
            Dictionary<string, int> dicIndices = new Dictionary<string, int>();

            if (pMsgRetorno[1] != FValorZero)
                SetarTamanhoMsg(pMsgRetorno[1]);            

            byte[] Resposta = new byte[TamanhoMsg];

            for (int i = 0; i < TamanhoMsg; i++)            
                Resposta[i] = pMsgRetorno[i + 3];            

            if (Resposta.Count() == 4)
            {
                bool[] bits = Resposta.SelectMany(GetBits).ToArray();

                string valorAntigo = "";
                string valorNovo = "";

                //Obtem o Ano
                for (int i = 0; i < bits.Count(); i++)
                {
                    if (i < 16)
                        valorAntigo = valorAntigo + Convert.ToInt32(bits[i]).ToString();

                    if (i >= 16)
                        valorNovo = valorNovo + Convert.ToInt32(bits[i]).ToString();
                }
                dicIndices.Add("Inicial", Convert.ToInt16(valorAntigo, 2));
                dicIndices.Add("Final", Convert.ToInt16(valorNovo, 2));
            }

            return dicIndices;
        }

        public static IEnumerable<bool> GetBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }

        public byte[] LerValidarIndice(short pIndice)
        {
            List<byte> Mensagem = new List<byte>();

            byte[] bDados = BitConverter.GetBytes(pIndice);

            Mensagem.Add(bDados[1]);
            Mensagem.Add(bDados[0]);

            return MontarFrame(CodFIndice, Mensagem.ToArray());
        }

        public bool ValidarRetornoRegistroPorIndices(byte[] pMsgRetorno)
        {
            if (pMsgRetorno[3] == FValorZero)
                return true;

            return false;
        }

        public byte[] LerRegistroDataHora()
        {
            return MontarFrame(CodFDataHora, null);
        }        

        public DateTime? ObterRetornoDataHora(byte[] pMsgRetorno)
        {
            Dictionary<string, int> dicIndices = new Dictionary<string, int>();
            DateTime RetDataHoraAux = new DateTime();

            if (pMsgRetorno[1] != FValorZero)
                SetarTamanhoMsg(pMsgRetorno[1]);

            byte[] DataHora = new byte[TamanhoMsg];

            for (int i = 0; i < TamanhoMsg; i++)
                DataHora[i] = pMsgRetorno[i + 3];

            bool[] bits = DataHora.SelectMany(GetBits).ToArray();            

            if (bits.Count() > 0)
            {
                string valorAno = "";
                string valorMes = "";
                string valorDia = "";
                string valorHoras = "";
                string valorMinutos = "";
                string valorSegundos = "";
               
                for (int i = 0; i < bits.Count(); i++)
                {
                    if (i < 12)
                        valorAno = valorAno + Convert.ToInt32(bits[i]).ToString();

                    if ((i >= 12) && (i < 16))
                        valorMes = valorMes + Convert.ToInt32(bits[i]).ToString();

                    if ((i >= 16) && (i < 21))
                        valorDia = valorDia + Convert.ToInt32(bits[i]).ToString();

                    if ((i >= 21) && (i < 26))
                        valorHoras = valorHoras + Convert.ToInt32(bits[i]).ToString();

                    if ((i >= 26) && (i < 32))
                        valorMinutos = valorMinutos + Convert.ToInt32(bits[i]).ToString();

                    if ((i >= 32) && (i < 38))
                        valorSegundos = valorSegundos + Convert.ToInt32(bits[i]).ToString();
                }

                int Ano = Convert.ToInt32(valorAno, 2);
                int Mes = Convert.ToInt32(valorMes, 2);
                int Dia = Convert.ToInt32(valorDia, 2);
                int Hora = Convert.ToInt32(valorHoras, 2);
                int Min = Convert.ToInt32(valorMinutos, 2);
                int Seg = Convert.ToInt32(valorSegundos, 2);

                DateTime RetDataHora = new DateTime(Ano, Mes, Dia, Hora, Min, Seg);
                RetDataHoraAux = RetDataHora;
            }

            return RetDataHoraAux;
        }

        public byte[] LerValorEnergia()
        {
            return MontarFrame(CodFValorEnergia, null);
        }

        public double ObterRetornoValorEnergia(byte[] pMsgRetorno)
        {
            Dictionary<string, int> dicIndices = new Dictionary<string, int>();

            if (pMsgRetorno[1] != FValorZero)
                SetarTamanhoMsg(pMsgRetorno[1]);

            byte[] Resposta = new byte[TamanhoMsg];

            for (int i = 0; i < TamanhoMsg; i++)            
                Resposta[i] = pMsgRetorno[i + 3];

            Array.Reverse(Resposta);
            return BitConverter.ToSingle(Resposta, 0);
        }

        static double Hex32toFloat(string Hex32Input)
        {
            double doubleout = 0;
            UInt64 bigendian;
            bool success = UInt64.TryParse(Hex32Input,
                System.Globalization.NumberStyles.HexNumber, null, out bigendian);
            if (success)
            {
                double fractionDivide = Math.Pow(2, 23);               
                int sign = (bigendian & 0x80000000) == 0 ? 1 : -1;
                Int64 exponent = ((Int64)(bigendian & 0x7F800000) >> 23) - (Int64)127;
                UInt64 fraction = (bigendian & 0x007FFFFF);
                if (fraction == 0)
                    doubleout = sign * Math.Pow(2, exponent);
                else
                    doubleout = sign * (1 + (fraction / fractionDivide)) * Math.Pow(2, exponent);
            }
            return doubleout;
        }
    }
}
