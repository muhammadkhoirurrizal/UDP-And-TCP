using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerUDPTCP
{
    class Server
    {
        private static TcpListener tcpListener;
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();

        static int Mulai = 0;

        static void Main(string[] args)
        {
            Thread WantClient = new Thread(Send);
            WantClient.Start();

            tcpListener = new TcpListener(IPAddress.Any, 4444);
            tcpListener.Start();

            Console.WriteLine("Server started");

            while (true)
            {
                if (Mulai <= 2)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    tcpClientsList.Add(tcpClient);
                    Thread thread = new Thread(ClientListener);
                    thread.Start(tcpClient);
                    Mulai++;
                    if (Mulai ==2)
                    {
                        WantClient.Abort();
                    }
                }
            }
        }

        public static void ClientListener(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader reader = new StreamReader(tcpClient.GetStream());

            Console.WriteLine("Client connected");

            while (true)
            {
                string message = reader.ReadLine();
                Console.WriteLine(message);
                BroadCast(message, tcpClient);
            }
        }

        public static void BroadCast(string msg, TcpClient Client)
        {
            foreach (TcpClient client in tcpClientsList)
            {
                if (client != Client)
                {
                    StreamWriter sWriter = new StreamWriter(client.GetStream());
                    sWriter.WriteLine(msg);
                    sWriter.Flush();
                }
            }
        }

        static void Send()
        {
            //Mengirim ke Semua Jaringan
            var Server = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes("Host Created");
            var ClientEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                Server.EnableBroadcast = true;
                Server.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
            }
        }
    }
}
