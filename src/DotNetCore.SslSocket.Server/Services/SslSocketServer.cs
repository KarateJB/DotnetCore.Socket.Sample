using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using DotNetCore.SslSocket.Server.Models;
using DotNetCore.SslSocket.Server.Utils;
using Lib.Socket.Server.Utils.Factory;
using Lib.Socket.Server.Utils.Handler;

namespace DotNetCore.SslSocket.Server.Services
{
    /// <summary>
    /// Socket server
    /// </summary>
    public static class SslSocketServer
    {
        /// <summary>
        /// Thread signal
        /// </summary>
        public static ManualResetEvent eventSignal = null;

        /// <summary>
        /// TCP listener
        /// </summary>
        public static TcpListener TcpListener = null;

        private const int Port = 6667;
        private const int MaxQueuedClientNumber = 100; // Max Queued Clients (that will be waiting for Server to accept and serve)
        private const int StreamReadTimeout = 5000; // 5 sec timeout for reading stream
        private const int StreamWriteTimeout = 5000; // 5 sec timeout for writing stream

        /// <summary>
        /// Stop Socket Server flag
        /// </summary>
        internal static bool IsClosed { get; set; }

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        static SslSocketServer()
        {
            eventSignal = new ManualResetEvent(false);
            TcpListener = new TcpListener(System.Net.IPAddress.Any, Port);
        }
        #endregion

        #region Start Socket Server

        /// <summary>
        /// Start the socket server
        /// </summary>
        internal static void Start()
        {
            IsClosed = false;
            TcpListener.Start(MaxQueuedClientNumber);
        }
        #endregion

        #region Stop Socket server

        /// <summary>
        /// Stop the socket server
        /// </summary>
        internal static void Stop()
        {
            TcpListener.Stop();
            IsClosed = true;
        }
        #endregion

        #region Listen

        internal static void Listen()
        {
            // Set the event to nonsignaled state
            eventSignal.Reset();

            Console.WriteLine("[SSL Socket] Waiting for a request...");

            try
            {
                TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), TcpListener);
            }
            catch (Exception ex)
            {
                LoggerProvider.Logger.Error(ex, "Socket listen error");
            }

            // Wait until a connection is made before continuing
            eventSignal.WaitOne();
        }
        #endregion

        /// <summary>
        /// AcceptCallback
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue
            eventSignal.Set();

            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient handler = listener.EndAcceptTcpClient(ar);

            // SslStream sslStream = new SslStream(client.GetStream(), true);
            var sslStream = new System.Net.Security.SslStream(
                handler.GetStream(), false, new RemoteCertificateValidationCallback(CertHelper.ValidateServerCertificate), null);
            sslStream.ReadTimeout = StreamReadTimeout;
            sslStream.WriteTimeout = StreamWriteTimeout;

            try
            {
                sslStream.AuthenticateAsServer(CertHelper.ServerCertificate, false, SslProtocols.Tls12, false);

                // Create the state object
                var state = new SslStreamState();
                state.SslStream = sslStream;

                sslStream.BeginRead(state.Buffer, 0, state.BufferSize, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                sslStream.Close();
                handler.Close();
                LoggerProvider.Logger.Error(ex, "Socket error");
            }
        }

        #region ReadCallback

        private static void ReadCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            // Retrieve the state object and the SslStream from the asynchronous state object
            SslStreamState state = (SslStreamState)ar.AsyncState;
            System.Net.Security.SslStream handler = state.SslStream;

            // Read data from the client socket.
            int bytesRead = handler.EndRead(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far
                state.Content.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data
                content = state.Content.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the client. Display it on the console
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    // Handle the message request
                    // using var requestHandler = new MsgRequestHandler();
                    // requestHandler.HandleAsync(state).Wait();

                    // Handle the file-uploading request
                    using var requestHandler = new FileRequestHandler();
                    requestHandler.HandleAsync(state).Wait();

                    // Echo something back to the client
                    Send(handler, $"Received data on {DateTime.Now.ToString()}");
                }
                else
                {
                    // Not all data received. Get more
                    handler.BeginRead(state.Buffer, 0, state.BufferSize, new AsyncCallback(ReadCallback), state);
                }
            }
        }
        #endregion

        #region Send

        private static void Send(System.Net.Security.SslStream handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device
            handler.BeginWrite(byteData, 0, byteData.Length, new AsyncCallback(SendCallback), handler);
        }
        #endregion

        #region SendCallback

        private static void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object
            System.Net.Security.SslStream handler = (System.Net.Security.SslStream)ar.AsyncState;

            // Complete sending the data to the remote device
            handler.EndWrite(ar);

            handler.ShutdownAsync().Wait();
            handler.Close();
        }
        #endregion
    }
}