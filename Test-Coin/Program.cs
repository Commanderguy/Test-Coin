using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using CoinFramework;

namespace Test_Coin
{
    public class Program
    {
        static void Main(string[] args)
        {
            AccountView view = new AccountView();
            Console.WriteLine("Public key size:");
            Console.WriteLine(ByteArrToString(view.publicKey));

            Console.ReadKey();
        }
        
        static string ByteArrToString(byte[] arr)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                builder.Append(arr[i].ToString("x2"));
                if (i % 10 == 0) builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}
