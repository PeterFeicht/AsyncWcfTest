using System.ServiceModel;
using System.Threading.Tasks;

namespace AsyncWcfLib
{
	[ServiceContract(Name = "IService")]
	public interface IServiceAsync : IService
	{
		[OperationContract]
		Task<bool> ShouldDoWorkAsync(int rnd);

		[OperationContract]
		Task<bool> DoWorkAsync(byte[] work);

		[OperationContract]
		Task<bool> WorkDoneAsync(bool success);
	}
}
