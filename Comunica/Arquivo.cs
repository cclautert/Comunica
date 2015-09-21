using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comunica
{
    public class Arquivo
    {
        private static object lockObj = new Object();
        private static volatile Arquivo instancia;
        public static Arquivo Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (lockObj)
                    {
                        if (instancia == null)
                            instancia = new Arquivo();
                    }
                }
                return instancia;
            }
        }    

        public IList<Pedido> LerPedido(string pArquivo)
        {
            try
            {
                if ((!string.IsNullOrEmpty(pArquivo)) && (File.Exists(pArquivo)))
                {
                    StreamReader objReader = new StreamReader(pArquivo);
                    string sLine = "";
                    IList<Pedido> lstPedido = new List<Pedido>();

                    while (sLine != null)
                    {
                        int count = 0;
                        Pedido objPedido = new Pedido();

                        sLine = objReader.ReadLine();
                        if (sLine != null)
                        {
                            string[] campos = sLine.Split(' ');
                            foreach (string s in campos)
                            {
                                if (s.Trim() != "")
                                {
                                    switch (count)
                                    {
                                        case 0:
                                            objPedido.SetarIP(s.Trim());
                                            break;
                                        case 1:
                                            objPedido.SetarPorta(Convert.ToInt32(s.Trim()));
                                            break;
                                        case 2:
                                            objPedido.SetarIndiceInicial(Convert.ToInt32(s.Trim()));
                                            break;
                                        case 3:
                                            objPedido.SetarIndiceFinal(Convert.ToInt32(s.Trim()));
                                            break;
                                        default:
                                            break;
                                    }
                                    count++;
                                }
                            }
                            lstPedido.Add(objPedido);
                        }
                    }

                    return lstPedido;
                }
                else
                {
                    Console.WriteLine("Arquivo não Existe!");
                    throw new Exception("Arquivo não Existe!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
                throw new Exception("Exception : " + e.ToString());
            }
        }        

        public void GravarLeiturasPedido(string[] pArquivo)
        {
            if (pArquivo == null)
                throw new Exception("Não existe dados a serem gravados!");

            string Data = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();// +DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            FileStream fs = new FileStream("Leitura_" + Data + ".csv", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            foreach (string line in pArquivo)
                sw.WriteLine(line);

            sw.Flush();
            sw.Close();
        }

    }
}
