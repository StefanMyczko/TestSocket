using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TestSocketServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program.LogMessage("Socket Server Started");

            Program oProgram = new Program();

            oProgram.StartListening();

            while (true)
            {
                Thread.Sleep(100);
            }
        }


        private Socket oClientSocket;


        private void StartListening()
        {


            string sHostName = Dns.GetHostName();
            IPHostEntry oIpHostInfo = Dns.GetHostEntry(sHostName);

            IPAddress oIpAddress = oIpHostInfo.AddressList.FirstOrDefault(ipa => ipa.AddressFamily == AddressFamily.InterNetwork);

            if (oIpAddress == null)
            {
                oIpAddress = oIpHostInfo.AddressList.FirstOrDefault(ipa => ipa.AddressFamily != AddressFamily.InterNetwork);
            }
           

            IPEndPoint oLocalEndPoint = new IPEndPoint(oIpAddress, 5020);

            Program.LogMessage($"IPEndPoint {oLocalEndPoint.ToString()}");

            // Create a TCP/IP socket.  
            this.oClientSocket = new Socket(oIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            this.oClientSocket.Bind(oLocalEndPoint);
            this.oClientSocket.Listen(100);

            Program.LogMessage($"Start Listening");
            // Start an asynchronous socket to listen for connections.  
            this.oClientSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.oClientSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {

            // Begin waiting for next message.
            this.oClientSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.oClientSocket);

            // Get the socket that handles the client request.  
            Socket oListener = (Socket)ar.AsyncState;
            Socket oHandler = oListener.EndAccept(ar);

            using (NetworkStream oNetworkStream = new NetworkStream(oHandler, true))
            {
                // Read the incoming request.
                using (TextReader oReader = new StreamReader(oNetworkStream, Encoding.ASCII))
                using (TextWriter oWriter = new StreamWriter(oNetworkStream, Encoding.ASCII))
                {
                    string sText = oReader.ReadToEnd();

                    Program.LogMessage($"Received: {sText}");
                    Program.LogMessage($"Waiting 5 seconds before sending response");

                    Thread.Sleep(5000); // Wait 5 seconds before sending response

                    string sResponseText = $"Message Received: {sText}";

                    Program.LogMessage($"Sending response: {sResponseText}");

                    oWriter.Write(sResponseText);

                    oWriter.Flush();

                }
            }

           
        }


        public static void LogMessage(string message)
        {
            Console.WriteLine($"SERVER: {message}");
        }
    }
}