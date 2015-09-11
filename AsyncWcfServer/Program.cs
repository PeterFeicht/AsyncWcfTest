using System;
using System.ServiceModel;
using AsyncWcfLib;

namespace AsyncWcfServer
{
	internal class Program
	{
		private static void Main(string[] args) {
			ServiceHost host = Util.CreateServiceHost();
			try {
				host.Open();
			} catch(Exception ex) {
				Console.WriteLine("Failed to open service host: {0}", ex);
				Console.ReadKey();
				return;
			}
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
			try {
				host.Close();
			} catch(Exception ex) {
				Console.WriteLine("Failed to close service host: {0}", ex);
				Console.ReadKey();
			}
		}
	}
}
