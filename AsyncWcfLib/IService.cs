using System.ServiceModel;

namespace AsyncWcfLib
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        bool ShouldDoWork(int rnd);

        [OperationContract]
        bool DoWork(byte[] work);

        [OperationContract]
        bool WorkDone(bool success);
    }
}
