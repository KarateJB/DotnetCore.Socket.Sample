using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DotNetCore.Socket.Server
{
    /// <summary>
    /// Socket server
    /// </summary>
    public static class SocketServer
    {
        /// <summary>
        /// Thread signal
        /// </summary>
        public static ManualResetEvent AllDone = null;

        /// <summary>
        /// Socket listener
        /// </summary>
        public static System.Net.Sockets.Socket SocketListener = null;

        private const int Port = 6666;
        private const int MaxQueuedClientNumber = 100; // Max Queued Clients (that will be waiting for Server to accept and serve)
        private const int StreamReadTimeout = 5000; // 5 sec timeout for reading stream
        private const int StreamWriteTimeout = 5000; // 5 sec timeout for writing stream

        private static readonly IPEndPoint IpEndpoint = null; // Network endpoint

        /// <summary>
        /// Constructor
        /// </summary>
        static SocketServer()
        {
            AllDone = new ManualResetEvent(false);

            // IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            // IPAddress ipAddress = ipHostInfo.AddressList[0];
            // ipEndpoint = new IPEndPoint(ipAddress, 5656);
            // this.socketListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IpEndpoint = new IPEndPoint(IPAddress.Any, Port);
            SocketListener = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        /// <summary>
        /// Stop Socket Server flag
        /// </summary>
        internal static bool IsClosed { get; set; }

        /// <summary>
        /// Start the socket server
        /// </summary>
        internal static void Start()
        {
            IsClosed = false;
            SocketListener.Bind(IpEndpoint);
            SocketListener.Listen(MaxQueuedClientNumber);
        }

        /// <summary>
        /// Stop the socket server
        /// </summary>
        internal static void Stop()
        {
            SocketListener.Close();
            IsClosed = true;
        }

        #region Listen

        internal static void Listen()
        {
            // Set the event to nonsignaled state
            AllDone.Reset();

            Console.WriteLine("[Socket] Waiting for a request...");

            try
            {
                // Start an asynchronous socket to listen for connections
                SocketListener.BeginAccept(new AsyncCallback(AcceptCallback), SocketListener);
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Error(ex, "Socket listen error");
            }

            // Wait until a connection is made before continuing
            AllDone.WaitOne();
        }
        #endregion

        #region AcceptCallback
        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue
            AllDone.Set();

            // Get the socket that handles the client request
            System.Net.Sockets.Socket listener = (System.Net.Sockets.Socket)ar.AsyncState;
            System.Net.Sockets.Socket handler = listener.EndAccept(ar);

            listener.ReceiveTimeout = StreamReadTimeout;
            listener.SendTimeout = StreamWriteTimeout;

            try
            {
                // Create the state object
                var state = new SocketState();
                state.WorkSocket = handler;
                handler.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                handler.Close();
                LoggerFactory.Instance.Error(ex, "Socket error");
            }
        }
        #endregion

        #region ReadCallback

        private static void ReadCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            // Retrieve the state object and the handler socket from the asynchronous state object
            SocketState state = (SocketState)ar.AsyncState;
            System.Net.Sockets.Socket handler = state.WorkSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

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

                    // HACK: RequestHandler
                    using var requestHandler = new RequestHandler();
                    requestHandler.HandleFileAsync(state).Wait();

                    // Echo something back to the client
                    Send(handler, "Data received!");
                }
                else
                {
                    // Not all data received. Get more
                    handler.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
                }
            }
        }
        #endregion

        #region Send

        private static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        #endregion

        #region SendCallback

        private static void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object
            System.Net.Sockets.Socket handler = (System.Net.Sockets.Socket)ar.AsyncState;

            // Complete sending the data to the remote device
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        #endregion
    }
}