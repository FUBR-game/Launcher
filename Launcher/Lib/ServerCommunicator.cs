using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Launcher.Lib
{
    public class ServerCommunicator
    {
        private static ServerCommunicator _serverCommunicator;
        private TcpClient _client;
        public static IPEndPoint ManagerIpEndPoint = new IPEndPoint(IPAddress.Parse("62.210.180.72"), 61681);

        private ServerCommunicator()
        {
            _client = new TcpClient(ManagerIpEndPoint);
        }

        public static ServerCommunicator GetServerCommunicator =>
            _serverCommunicator ?? (_serverCommunicator = new ServerCommunicator());
    }
}