using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace ConsoleApp4
{
	internal class Program
	{
		static void Main(string[] args)
		{
            Program.LogMessage("App Started");

			while (true)
			{

				Program.TestSocket();

                Program.LogMessage($"Waiting 5 seconds before sending next message");

                Thread.Sleep(5000); // Wait 5 seconds before sending next message
			}
		}


		public static string TestSocket()
		{
			string sResponse = "";

			string messageText = $"New Message sent at {DateTime.Now.ToLongTimeString()}";

			IPAddress oIpAddress = IPAddress.Parse("192.168.1.242");

            IPEndPoint oRemoteEndPoint = new(oIpAddress, 5020);

			using (Socket oSender = new(oIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
			{
				// Connect to the required endpoint
				oSender.Connect(oRemoteEndPoint);

				// Send the request and receive the response
				using (NetworkStream oNetworkStream = new(socket: oSender, ownsSocket: true))
				{

					// Send the request
					using (TextWriter oWriter = new StreamWriter(oNetworkStream, Encoding.UTF8))
					using (TextReader oReader = new StreamReader(oNetworkStream, Encoding.UTF8))
					{
						Program.LogMessage($"Write Message: {messageText}");

						Stopwatch oStopWatch = new Stopwatch();

						oStopWatch.Start();

                        oWriter.Write(messageText);

						oWriter.Flush();

						// Close the outgoing connection to complete sending the request.
						oSender.Shutdown(SocketShutdown.Send);

                        // Read the response.
                        sResponse = oReader.ReadToEnd();

						oStopWatch.Stop();


                        Program.LogMessage($"Message Returned: {sResponse} in {oStopWatch.ElapsedMilliseconds}ms");

						if (oStopWatch.ElapsedMilliseconds > 4000)
						{
                            Program.LogMessage("*********SUCCESS**********");
                        }
						else 
						{
                            Program.LogMessage("*********FAILED**********");
                        }
                    }
				}
			}

			return sResponse;

		}

		public static void LogMessage(string message)
		{
			Console.WriteLine($"CLIENT: {message}");
		}
	}



}