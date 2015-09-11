using System.ServiceModel;
using System.Threading;

namespace AsyncWcfLib
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	internal class ServiceImpl : IService
	{
		#region IService Members

		public bool ShouldDoWork(int rnd) {
			return rnd >= 0;
		}

		public bool DoWork(byte[] work) {
			if(work == null) {
				return false;
			}
			if(work.Length > 0) {
				Thread.Sleep(work[0]);
			}
			return true;
		}

		public bool WorkDone(bool success) {
			return success;
		}

		#endregion
	}
}
