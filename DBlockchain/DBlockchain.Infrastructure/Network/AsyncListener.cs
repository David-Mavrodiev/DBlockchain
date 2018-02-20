using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DBlockchain.Infrastructure.Network
{
    public class AsyncListener
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public IResponseFabric responseFabric;
        public static int Port = 0;
        public static Socket listener;

        public AsyncListener(IResponseFabric responseFabric)
        {
            this.responseFabric = responseFabric;
        }

        public void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024 * 100];

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            var ips = Dns.GetHostByName(hostName).AddressList;
            string ip = ips[ips.Length - 1].ToString();

            if (Port == 0)
            {
                Console.Write("Choose port: ");
                int port = int.Parse(Console.ReadLine());
                Port = port;
            }
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            Console.WriteLine($"Address is {ip}:{Port}");

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                //while (true)
                //{
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                //allDone.WaitOne();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            SocketDataBody content = null;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.
                content = JsonConvert.DeserializeObject<SocketDataBody>(state.sb.ToString());

                if (content != null)
                {
                    this.responseFabric.ReceiveResponse(content);

                    var body = CommandsReflector.GetGlobalCommand(content.CommandName).Item1.Aggregate(content);
                    content.Body = body;
                    content.Type = SocketDataType.Receive;

                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public void Send(Socket handler, SocketDataBody data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data.ToString());

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
