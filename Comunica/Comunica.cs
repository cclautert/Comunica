using System;
using System.Collections.Generic;
using System.IO;
namespace Comunica
{
    class Comunica
    {
        static void Main(string[] args)
        {
            string strArquivo = "";
            string strTipoEnvio = "";
            //string strModoComunicacao = "";

            Dictionary<string, string> dicTipoEnvio = new Dictionary<string, string>();
            Dictionary<string, string> dicModoComunicacao = new Dictionary<string, string>();

            dicTipoEnvio.Add("1", "Serial");
            dicTipoEnvio.Add("2", "Paralelo");

            //dicModoComunicacao.Add("1", "Assíncrona");
            //dicModoComunicacao.Add("2", "Síncrona");

            while (strArquivo == "")
            {
                if (args.Length == 0)
                {
                    while (strArquivo == "")
                    {
                        Console.WriteLine("Digite o nome do arquivo a ser enviado");
                        strArquivo = Console.ReadLine();
                        Console.Clear();
                    }
                }
                else
                    strArquivo = args[0];

                if (!File.Exists(strArquivo))
                {
                    Console.WriteLine("O arquivo {0} não existe no diretório!", strArquivo);
                    Console.ReadLine();
                    strArquivo = "";
                }
            }

            while ((strTipoEnvio != "1") && (strTipoEnvio != "2"))
            {
                Console.WriteLine("Pedidos feitos em: 1-Serial 2-Paralelo?");
                strTipoEnvio = Console.ReadLine();
                Console.Clear();
            }

            //while ((strModoComunicacao != "1") && (strModoComunicacao != "2"))
            //{
            //    Console.WriteLine("A comunicação será: 1-Assíncrona; 2-Síncrona?");
            //    strModoComunicacao = Console.ReadLine();
            //    Console.Clear();
            //}

            Console.WriteLine("Dados Inseridos:");
            Console.WriteLine("Nome do Arquivo: {0}", strArquivo);
            Console.WriteLine("Tipo de Envio Selecionado: {0}", dicTipoEnvio[strTipoEnvio]);
            //Console.WriteLine("Modo de Comunicação Selecionado: {0}", dicModoComunicacao[strModoComunicacao]);
            Console.WriteLine("Pressione uma tecla para continuar!");
            Console.ReadLine();

            FachadaComunica.Instancia.ExecutarOperacao(strArquivo, Convert.ToInt32(strTipoEnvio));            
        }
    }
}
