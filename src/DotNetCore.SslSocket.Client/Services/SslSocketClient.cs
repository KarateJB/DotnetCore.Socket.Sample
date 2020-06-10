using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.SslSocket.Client.Utils;

namespace DotNetCore.SslSocket.Client.Services
{
    /// <summary>
    /// Socket client
    /// </summary>
    public class SslSocketClient : IDisposable
    {
        private const string Host = "127.0.0.1";
        private const int Port = 6667;
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
            TcpClient client = new TcpClient(Host, Port);

            try
            {
                SslStream sslStream =
                    new SslStream(client.GetStream(),
                    false, new RemoteCertificateValidationCallback(CertHelper.ValidateServerCertificate), null);

                var certs = new X509CertificateCollection();
                certs.Add(CertHelper.ClientCertificate);

                try
                {
                    await sslStream.AuthenticateAsClientAsync("localhost", certs, System.Security.Authentication.SslProtocols.Tls12, false);

                    #region Send without callback
                    sslStream.Write(clientData);
                    sslStream.Flush();
                    #endregion

                    #region Send with callback
                    ////sslStream.BeginWrite(clientData, 0, clientData.Length, new AsyncCallback(this.SendCallback), sslStream);
                    ////sslStream.Flush();
                    #endregion

                    #region Receive response from server

                    // Read with a enough buffer
                    // Note by using TLS 1 by System.Security.Authentication.SslProtocols.Tls, the stream will first get 1 byte followed by the rest of the bytes.
                    // So this way wont work on TLS 1, use recursively-read way instead.
                    // See https://stackoverflow.com/a/48753002/7045253
                    byte[] rtnBytes = new byte[1024]; // Data buffer for incoming data
                    int bytesRec = sslStream.Read(rtnBytes);
                    Console.WriteLine($"Echoed from server: {Encoding.ASCII.GetString(rtnBytes, 0, bytesRec)}");

                    // Or read recursively
                    // string serverMsg = await this.readStreamAsync(sslStream);
                    // Console.WriteLine($"Echoed from server: {serverMsg}");

                    #endregion
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sslStream.Close();
                    client.Close();
                }
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

        private async Task<string> readStreamAsync(SslStream sslStream)
        {
            byte[] buffer = new byte[1024];
            StringBuilder content = new StringBuilder();
            int bytesRead = -1;

            do
            {
                bytesRead = sslStream.Read(buffer, 0, buffer.Length);
                content.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                if (content.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytesRead != 0);

            return await Task.FromResult(content.ToString());
        }
    }
}