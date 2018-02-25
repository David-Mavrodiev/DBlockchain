using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DBlockchain.Infrastructure.Network
{
    public class DFileStorage
    {
        private List<Tuple<string, int>> peers;

        public DFileStorage(List<Tuple<string, int>> peers)
        {
            this.peers = peers;
        }

        public void StartFileListener()
        {
            string hostName = Dns.GetHostName();
            var ips = Dns.GetHostByName(hostName).AddressList;
            IPAddress ip = ips[ips.Length - 1];
            int port = 333;

            TcpListener tcpListener = new TcpListener(ip, port);
            tcpListener.Start();
            while (true)
            {
                Socket handlerSocket = tcpListener.AcceptSocket();
                if (handlerSocket.Connected)
                {
                    string fileName = string.Empty;
                    NetworkStream networkStream = new NetworkStream(handlerSocket);
                    int thisRead = 0;
                    int blockSize = 1024;
                    Byte[] dataByte = new Byte[blockSize];
                    lock (this)
                    {
                        string folderPath = @"DecentralizedStorage/";
                        int receivedBytesLen = handlerSocket.Receive(dataByte);
                        int fileNameLen = BitConverter.ToInt32(dataByte, 0);
                        fileName = Encoding.ASCII.GetString(dataByte, 4, fileNameLen);

                        if (!File.Exists(folderPath + fileName))
                        {
                            Stream fileStream = File.OpenWrite(folderPath + fileName);
                            fileStream.Write(dataByte, 4 + fileNameLen, (1024 - (4 + fileNameLen)));
                            while (true)
                            {
                                thisRead = networkStream.Read(dataByte, 0, blockSize);
                                fileStream.Write(dataByte, 0, thisRead);
                                if (thisRead == 0)
                                    break;
                            }
                            fileStream.Close();

                            foreach (var peer in this.peers)
                            {
                                Console.WriteLine($"Send received file to {peer.Item1}:{peer.Item2}...");
                                this.SendFile(peer.Item1, peer.Item2, (folderPath + fileName), fileName);
                            }
                        }
                    }

                    handlerSocket = null;
                }
            }
        }

        public void SendFile(string remoteHostIP, int remoteHostPort, string longFileName, string shortFileName)
        {
            if (!string.IsNullOrEmpty(remoteHostIP))
            {
                byte[] fileNameByte = Encoding.ASCII.GetBytes(shortFileName);
                byte[] fileData = File.ReadAllBytes(longFileName);
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4); fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                TcpClient clientSocket = new TcpClient(remoteHostIP, remoteHostPort);
                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Write(clientData, 0, clientData.GetLength(0));
                networkStream.Close();
            }
        }
    }
}
