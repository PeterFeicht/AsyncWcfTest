using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace AsyncWcfLib
{
	public static class Util
	{
		private const string cEndpointName = "net.tcp://127.0.0.1:8003/TestService";

		public static Uri CreateAddress(bool clientSide = false) {
			return clientSide ? new Uri(cEndpointName + "/Soap") : new Uri(cEndpointName);
		}

		public static NetTcpBinding CreateBinding() {
			return new NetTcpBinding {
				CloseTimeout = TimeSpan.FromSeconds(10),
				OpenTimeout = TimeSpan.FromSeconds(10),
				ReceiveTimeout = TimeSpan.FromMinutes(2),
				SendTimeout = TimeSpan.FromMinutes(2),
				Security = new NetTcpSecurity {
					Mode = SecurityMode.None,
					Transport = new TcpTransportSecurity {
						ClientCredentialType = TcpClientCredentialType.None,
						ProtectionLevel = ProtectionLevel.None
					}
				}
			};
		}

		public static ServiceHost CreateServiceHost() {
			ServiceHost host = new ServiceHost(new ServiceImpl(), CreateAddress());
			host.Description.Behaviors.Add(new ServiceMetadataBehavior());
			host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "MEX");
			host.AddServiceEndpoint(typeof(IService), CreateBinding(), "Soap");
			return host;
		}
	}
}
