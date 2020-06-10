using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Socket.Client.Services
{
    /// <summary>
    /// Socket client
    /// </summary>
    public class SocketClient : IDisposable
    {
        private const string Host = "127.0.0.1";
        private const int Port = 6666;
        private const string TestFileName = "Demo.txt";

        public void Dispose()
        {
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="clientData">Data to be send</param>
        protected async Task SendAsync(byte[] clientData)
        {
            // IPAddress[] ipAddress = Dns.GetHostAddresses(Host);
            IPAddress ipAddress = IPAddress.Parse(Host);
            IPEndPoint ipEnd = new IPEndPoint(ipAddress, Port);
            System.Net.Sockets.Socket client = null;

            try
            {
                // Make a client socket to send data to server.
                client = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                // Connect to Socket Server
                client.Connect(ipEnd);
                Console.WriteLine($"Connected to {client.RemoteEndPoint.ToString()}");

                #region Send without callback

                // sender.Send(byteData);
                #endregion

                #region Send with callback

                client.BeginSend(clientData, 0, clientData.Length, 0, new AsyncCallback(this.SendCallback), client);
                #endregion

                #region Receive response from server

                // Receive the response from the remote device
                byte[] rtnBytes = new byte[1024]; // Data buffer for incoming data
                int bytesRec = client.Receive(rtnBytes);
                Console.WriteLine($"Echoed from server: {Encoding.ASCII.GetString(rtnBytes, 0, bytesRec)}");
                #endregion
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            finally 
            {
                if (client.Connected)
                    client.Close();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Send callback
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        protected void SendCallback(IAsyncResult ar)
        {
            // Do something ...

            Console.WriteLine("SendCallback");
        }
    }
}