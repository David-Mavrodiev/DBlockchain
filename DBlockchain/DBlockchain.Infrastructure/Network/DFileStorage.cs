using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

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

                        if (Encoding.UTF8.GetString(dataByte).Contains("get-file"))
                        {
                            var template = "get-file -address {0} -name {1}";
                            var args = ReverseStringFormat(template, Encoding.UTF8.GetString(dataByte));

                            var targetIp = args[0].Split(':')[0];
                            var targetPort = int.Parse(args[0].Split(':')[1]);
                            var targetFileName = args[1].Trim('\0');

                            Console.WriteLine($"Get file {targetIp}:{targetPort} -> {targetFileName}...");
                            Console.WriteLine($"File is located in {folderPath}{targetFileName}...");

                            this.SendFile(targetIp, targetPort, $"{folderPath}{targetFileName}", targetFileName);
                            
                            continue;
                        }
                        else
                        {
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
                    }

                    handlerSocket = null;
                }
            }
        }

        public void SendFile(string remoteHostIP, int remoteHostPort, string longFileName, string shortFileName)
        {
            try
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
            catch
            {

            }   
        }

        private static List<string> ReverseStringFormat(string template, string str)
        {
            string pattern = "^" + Regex.Replace(template, @"\{[0-9]+\}", "(.*?)") + "$";

            Regex r = new Regex(pattern);
            Match m = r.Match(str);

            List<string> ret = new List<string>();

            for (int i = 1; i < m.Groups.Count; i++)
            {
                ret.Add(m.Groups[i].Value);
            }

            return ret;
        }
    }
}
