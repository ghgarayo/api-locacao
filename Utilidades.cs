using System;
using System.Security.Cryptography;

namespace SistemaLocacao
{
    public static class Utilidades
    {

        public static string LeString(string msg)  // Verifica se uma string está vazia ou é nula. Recebe uma mensagem e imprime antes da entrada.
        {
            string? str = "";
            while (true)
            {
                Console.WriteLine(msg);
                str = Console.ReadLine();

                if (str != null && !str.Trim().Equals(""))
                {
                    return str.Trim();
                }
                else
                {
                    Console.WriteLine("Opção Inválida, tente novamente!\n");
                }
            }
        }

        public static string LeString() // Verifica se uma string está vazia ou é nula.
        {
            string? str = "";
            while (true)
            {
                str = Console.ReadLine();

                if (str != null && !str.Trim().Equals(""))
                {
                    return str.Trim();
                }
                else
                {
                    Console.WriteLine("Opção Inválida, tente novamente!\n");
                }
            }
        }

        public static string GerarHash(string password)
        {
            //converte password de string ("array de chars") para array de bytes
            byte[] passwordEmBytes = System.Text.Encoding.UTF8.GetBytes(password);
            //gera a hash correspondente usando o sha256
            byte[] hashEmBytes = SHA256.Create().ComputeHash(passwordEmBytes);
            //converte array de bytes de novo para string
            string hashEmString = BitConverter.ToString(hashEmBytes);
            //remove os hifens
            hashEmString = hashEmString.Replace("-", String.Empty);
            //retorna hash
            return hashEmString;
        }

    }
}