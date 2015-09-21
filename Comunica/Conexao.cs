using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comunica
{
    public class Conexao
    {
        public static int Reconexao = 0;

        public static byte[] Enviar(string pIp, int pPort, byte[] pMsg)
        {
            if (pIp == "")
                throw new Exception("Endereço ip não pode ser vazio!");

            byte[] bytes = new byte[128];

            try
            {
                bytes = EnviarMensagem(pIp, pPort, pMsg);
            }
            catch
            {
                Thread.Sleep(20000);
                if (Reconexao < 5)
                {
                    Reconexao++;
                    bytes = Enviar(pIp, pPort, pMsg);
                }
            }

            Reconexao = 0;
            return bytes;
        }

        public static byte[] EnviarMensagem(string pIp, int pPort, byte[] pMsg)
        {
            // buffer de dados
            byte[] bytes = new byte[128];

            // Conexão com o dispositivo remoto.
            try
            {
                IPAddress ipAddress = IPAddress.Parse(pIp);
                int PortServerAddress = pPort;

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PortServerAddress);

                // Cria o sockte TCP/IP.
                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                try
                {
                    sender.Connect(remoteEP);

                    // Encode the data string into a byte array.                    
                    byte[] msg = pMsg;

                    // Envia os Dados
                    int bytesSent = sender.Send(msg);

                    // Recebe resposta do dispositivo remoto.
                    int bytesRec = sender.Receive(bytes);

                    // Finaliza o socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    throw new Exception("ArgumentNullException: " + ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    throw new Exception("SocketException: " + se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    throw new Exception("Unexpected exception: " + e.ToString());
                }

                return bytes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new Exception("Exception : " + e.ToString());
            }
        }
    }
}
