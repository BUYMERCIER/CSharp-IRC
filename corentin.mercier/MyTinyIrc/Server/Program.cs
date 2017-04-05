using System;
using System.Text; // Useful for GetBytes
using System.Net; // Useful for IPAddress
using System.Net.Sockets; // Socket class
using System.IO; // Useful to read a file
using System.Linq; // Useful for the .all

namespace Server
{
	class MainClass
	{

		public static Socket socket;
		public static int port;
		public static IPAddress ip;
		// Incoming data from the client.  
		public static string data = null;
		public static string pseudo;

		// Main method
		public static void Main(string[] args)
		{

			if (check(args))
			{ // Checks the arguments fast
				try
				{ // Making variables look more intuitive
					port = Convert.ToInt16(args[0]);
					ip = IPAddress.Parse("127.0.0.1");

					if (args.Length == 2) 
					{ // If there is a file in args[1]
						string path = args[1];
						string file = File.ReadAllText(path);
						Server(port, file);
					}
					else
					{ // Else the file is considered as null
						Server(port);
					}

					Run();

					Console.WriteLine("Connection Established");
				}
				catch (Exception e)
				{
					throw e;
				}
			}
		}
		// End of Main()

		public static void Server(int port, string filename = null)
		{
			socket = new Socket(AddressFamily.InterNetwork,
			                    SocketType.Stream,ProtocolType.Tcp);
		}
		// End of  Server()

		// Runs the server
		public static void Run()
		{
			int maxWaitList = 100; // Random value cause it's not explained...
			IPEndPoint ep = new IPEndPoint(ip, port);
			Bind(ep);
			Listen(maxWaitList);
		}
		// End of Run()

		// "Bind the socket you declared to an entry point"
		private static void Bind(IPEndPoint endPoint)
		{
			try
			{
				socket.Bind(endPoint);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		// End Bind()

		// "Listen the endpoint of the socket and wait for any connection"
		private static void Listen(int maxWaitList)
		{  
			socket.Listen(maxWaitList);

			Console.WriteLine("Connected!");
			Console.WriteLine("Waiting for a message...");

			//byte[] lenpseudo = new byte[1];
			//Socket handler = socket.Accept();

			//handler.Receive(lenpseudo);
			//byte[] pseudobt = new byte[lenpseudo[0]];
			//handler.Receive(pseudobt);
			//pseudo = Encoding.UTF8.GetString(pseudobt);

			pseudo = "root";
			Socket handler;
			string text;

			while (true)
			{
				handler = socket.Accept();

				byte[] bytes = new Byte[1024];

				handler.Receive(bytes);
				text = Encoding.UTF8.GetString(bytes);

				if (text.Length >= 3)
				{
					if (text[0] == ',' && text[1] == ',' && text[2] == ',')
					{
						text = Encoding.UTF8.GetString(bytes);
						pseudo = "";
						for (int i = 3; i < text.Length - 1; i++)
						{
							pseudo += text[i];
						}
						bytes = new Byte[1024];
						handler.Receive(bytes);
					}
				}

				text = Encoding.UTF8.GetString(bytes);

				bytes = new Byte[1024];
				handler.Receive(bytes);
				string time = Encoding.UTF8.GetString(bytes);

				Console.WriteLine(time + " " + pseudo + " sent: " + text);

				handler.Close();
				pseudo = "root";
			}

		}

		// Checks the arguments given
		private static bool check(string[] args)
		{
			if (args.Length == 0)
			{
				throw new Exception("/*Bad usage of the binary*/" +
				                    "\nUsage : Server.exe port_number [file]");
			}
			else if (args.Length > 2)
			{
				throw new Exception("/*Too many arguments*/" +
				                    "\nUsage : Server.exe port_number [file]");
			}
			else
			{
				return true;
			}
		}
	} // End of the main class
}