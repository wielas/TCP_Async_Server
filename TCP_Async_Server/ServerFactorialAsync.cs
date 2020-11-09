using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace TCP_Async_Server
{
    class ServerFactorialAsync : ServerFactorial
    {
        #region Fields

        private int counter = 0;

        #endregion

        #region Constructors

        public ServerFactorialAsync(IPAddress address, int port) : base(address, port) { }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes asynchronous TCP Server
        /// </summary>
        public override void Start()
        {
            StartListening();
            AcceptClient();
        }

        public string[] ReceivedMessages { get; } = new string[100];
        public string[] SentMessages { get; } = new string[100];

        /// <summary>
        /// Listens for incoming clients
        /// </summary>
        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();

                Stream = tcpClient.GetStream();

                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);

                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
            }
        }

        private void TransmissionCallback(IAsyncResult asyncResult)
        {
            TcpClient = (TcpClient)asyncResult.AsyncState;
            TcpClient.Close();
        }

        

        /// <summary>
        /// Takes incoming messages and returns answer
        /// </summary>
        /// <param name="stream">Client stream</param>
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int number = 0;
            double result;
            byte[] sent_back = new byte[1024];
            byte[] hello = new byte[100];
            hello = Encoding.Default.GetBytes("Input one or two digit number to calculate factorial\n");
            stream.Write(hello, 0, hello.Length);
            while (true)
            {
                int msg_len = stream.Read(buffer, 0, 1024);

                if (buffer[0] >= 49 && buffer[0] <= 57 && buffer[1] >= 48 && buffer[1] <= 57)
                {
                    number = (buffer[0] - 48) * 10 + (buffer[1] - 48);
                    result = 1;
                    while (number != 1)
                    {
                        result = result * number;
                        number = number - 1;
                    }
                    Console.WriteLine(result);
                    sent_back = Encoding.Default.GetBytes(result.ToString() + "\n");
                    stream.Write(sent_back, 0, sent_back.Length);
                }

                else if (buffer[0] >= 49 && buffer[0] <= 57)
                {
                    number = buffer[0] - 48;
                    result = 1;
                    while (number != 1)
                    {
                        result = result * number;
                        number = number - 1;
                    }
                    Console.WriteLine(result);
                    sent_back = Encoding.Default.GetBytes(result.ToString() + "\n");
                    stream.Write(sent_back, 0, sent_back.Length);
                }
            }
        }

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        #endregion

    }
}
