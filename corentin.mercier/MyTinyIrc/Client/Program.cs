using System;
using System.Text; // Useful for GetBytes
using System.Net; // Useful for IPAddress
using System.Net.Sockets; // Socket class
using System.IO; // Useful to read a file
using System.Linq; // Useful for the .all

namespace Client
{
	class MainClass
	{
		// Declaring the socket
		private static Socket socket;
		public static string error;
		public static string pseudo;

		public static void Main(string[] args)
		{
			error = "Unknown error";
			pseudo = "root";

			try
			{
				if (check(args))
				{
					var ipstring = args[0];
					var portstring = args[1];
					int port = Convert.ToInt16(portstring);

					var ip = IPAddress.Parse(ipstring);

					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

					socket.Connect(ip, port);
					bool check = false;

					Console.WriteLine("You are successfully connected to the server");

					if (args.Length > 2)
					{ // If there is a file in args[2]
						if (args[args.Length - 2] == "-p")
						{
							pseudo = ",,," + args[args.Length - 1];
							byte[] pseudobt = System.Text.Encoding.UTF8.GetBytes(pseudo);
							socket.Send(pseudobt);
							if (args.Length == 4)
							{
								check = true;
							}
						}

						if (args[2] != "-p")
						{
							string path = args[2];
							byte[] file = File.ReadAllBytes(path);
							socket.Send(file);
							Console.WriteLine("File sent");
						}

					}
					else { check = true; }

					if (check)
					{
							Console.Write("Type here the message you want to send: ");
							string message = Console.ReadLine();
							byte[] bt = System.Text.Encoding.UTF8.GetBytes(message);
							socket.Send(bt);
							Console.WriteLine("Message sent");
					}

					string time = Time(DateTime.Now);
					byte[] timebt = System.Text.Encoding.UTF8.GetBytes(time);
					socket.Send(timebt);

					socket.Close();
				}
				else
				{
					Console.WriteLine(error);
					throw new Exception("Argument error");
				}
			}
			catch (Exception e)
			{
				throw e;
			}

		}

		// Checks the arguments given - min 2 max 3
		private static bool check(string[] args) {
			if (args.Length < 2 || args.Length > 5)
			{
				error = "/*Bad usage of the binary*" +
					"/\nUsage : Client.exe ip_address port_number [file]";
				return false;
			}
			else
			{
				if (ValidateIPv4(args[0]))
				{
					return true;
				}
				else
				{
					error = "IP address is not valid";
					return false;
				}
			}
		}
		// End of check()

		public static bool ValidateIPv4(string ipString)
		{
			if (String.IsNullOrWhiteSpace(ipString))
			{
				return false;
			}

			string[] splitValues = ipString.Split('.');
			if (splitValues.Length != 4)
			{
				return false;
			}

			byte tempForParsing;

			return splitValues.All(r => byte.TryParse(r, out tempForParsing));
		}
		// End of ValidateIP

		public static string Time(DateTime date) 
		{
			return date.ToString("HH:m:s");
		}
		// End time
	}
}
