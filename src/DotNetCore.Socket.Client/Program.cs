using System;
using System.Threading.Tasks;
using DotNetCore.Socket.Client.Services;

namespace DotNetCore.Socket.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start Socket client...");

            try
            {
                // Send text to Socket server
                ////using var msgSender = new MsgSender();
                ////await msgSender.SendMsgAsync();

                // Send file to Socket server
                using var fileSender = new FileSender();
                await fileSender.SendFileAsync();

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
