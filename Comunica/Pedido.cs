using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica
{
    public class Pedido
    {
        public string Ip { get; set; }
        public int Porta { get; set; }
        public int IndiceInicial { get; set; }
        public int IndiceFinal { get; set; }
            
        public Pedido()
        { 
        }
    
        public void SetarIP(string pIp)
        {
            if (pIp == "")            
                throw new Exception("Endereço ip não pode ser vazio!");

            if (!ValidaIPv4(pIp))            
                throw new Exception("Endereço ip inválido");            

            Ip = pIp;
        }

        public void SetarPorta(int pPorta)
        {
            if (pPorta <= 0)            
                throw new Exception("Endereço de porta não pode ser menor ou igual a zero!");            

            if (pPorta > 65535)            
                throw new Exception("Endereço de porta não pode ser maior que 65535!");            

            Porta = pPorta;
        }

        public void SetarIndiceInicial(int pIndiceInicial)
        {
            IndiceInicial = pIndiceInicial;
        }

        public void SetarIndiceFinal(int pIndiceFinal)
        {
            if (!ValidaIndiceInicialFinal(IndiceInicial, pIndiceFinal))            
                throw new Exception("Indice final não pode ser menor que o indice inicial!");            

            IndiceFinal = pIndiceFinal;
        }

        public void SetarIndices(int pIndiceInicial, int pIndiceFinal)
        {
            SetarIndiceInicial(pIndiceInicial);
            SetarIndiceFinal(pIndiceFinal);

            if (!ValidaIndiceInicialFinal(pIndiceInicial, pIndiceFinal))            
                throw new Exception("Indeice final não pode ser menor que o indice inicial!");            

            IndiceInicial = pIndiceInicial;
            IndiceFinal = pIndiceFinal;
        }

        public bool ValidaIndiceInicialFinal(int pIndiceInicial, int pIndiceFinal)
        {
            if (pIndiceInicial > pIndiceFinal)
                return false;

            return true;
        }

        public static bool ValidaIPv4(string value)
        {
            var partes = value.Split('.');

            // Verifica se existe 4 partes
            if (!(partes.Length == 4)) return false;

            // for each partes
            foreach (var parte in partes)
            {
                int q;
                // if parse fails 
                // or length of parsed int != length of quad string (i.e.; '1' vs '001')
                // or parsed int < 0
                // or parsed int > 255
                // return false
                if (!Int32.TryParse(parte, out q)
                    || !q.ToString().Length.Equals(parte.Length)
                    || q < 0
                    || q > 255) { return false; }
            }
            return true;
        }
    }
}
