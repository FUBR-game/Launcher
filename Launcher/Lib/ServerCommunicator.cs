using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Launcher.Models;
using Newtonsoft.Json;

namespace Launcher.Lib
{
    public class ServerCommunicator
    {
        private static ServerCommunicator _serverCommunicator;

        public static IPEndPoint ManagerIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 61681);

//        public static IPEndPoint ManagerIpEndPoint = new IPEndPoint(IPAddress.Parse("62.210.180.72"), 61681);
        private TcpClient _client;

        private ServerCommunicator()
        {
            _client = new TcpClient();
            _client.Connect(ManagerIpEndPoint);

            new Thread(() => { ReceiveMessage(); }).Start();
        }

        public static ServerCommunicator GetServerCommunicator() =>
            _serverCommunicator ?? (_serverCommunicator = new ServerCommunicator());

        public void SetOwnUserStatus(UserStatus status)
        {
            var messageObject = new
            {
                MessageType = "StatusChange",
                MessageData = JsonConvert.SerializeObject(new
                    {UserStatus = UserStatus.Online, userId = User.GetCurrentUser().UserId})
            };

            SendMessage(messageObject);
        }

        private void ReceiveMessage()
        {
            var networkStream = _client.GetStream();
            while (_client.Connected)
            {
                if (networkStream.DataAvailable)
                {
                    byte[] messageByteArray;
                    using (var memStream = new MemoryStream())
                    {
                        // Read message size
                        var dataSize = new byte[4];
                        networkStream.Read(dataSize, 0, 4);
                        var messageSize = BitConverter.ToInt32(dataSize);

                        // Read message
                        var numBytesRead = 0;
                        while (numBytesRead != messageSize)
                        {
                            var data = new byte[messageSize];
                            numBytesRead = networkStream.Read(data, 0, messageSize - numBytesRead);
                            memStream.Write(data, 0, numBytesRead);
                        }

                        messageByteArray = memStream.ToArray();
                    }

                    // Parse message
                    var jsonString = Encoding.UTF8.GetString(messageByteArray);
                    var message = (Message) JsonConvert.DeserializeObject(jsonString, typeof(Message));

                    var returnData = ExecuteMessage(message);

                    continue;
                }

                Thread.Sleep(500);
            }
        }

        private bool SendMessage(object messageObject)
        {
            if (!_client.Connected) return false;

            var message = JsonConvert.SerializeObject(messageObject);

            // Get stream to write to
            var stream = _client.GetStream();

            // Convert Message to byte array
            var messageByteArray = Encoding.UTF8.GetBytes(message);

            // Determine message length
            var messageLength = messageByteArray.Length;
            var messageLengthByteArray = new byte[4];
            messageLengthByteArray = BitConverter.GetBytes(messageLength);

            // Prepend message length to message
            var fullMessage = new byte[messageLength + 4];
            Buffer.BlockCopy(messageLengthByteArray, 0, fullMessage, 0, messageLengthByteArray.Length);
            Buffer.BlockCopy(messageByteArray, 0, fullMessage, messageLengthByteArray.Length, messageByteArray.Length);

            // Send full messsage
            stream.Write(fullMessage);
            stream.Flush();
            return true;
        }

        public void GetUserStatus(User friend)
        {
            var messageObject = new
            {
                MessageType = "GetUserStatus", MessageData = JsonConvert.SerializeObject(new {userId = friend.UserId})
            };
            SendMessage(messageObject);
        }

        public event EventHandler<StatusChangeEventArgs> FriendChangesStatus;

        public virtual void OnFriendChangesStatus(StatusChangeEventArgs e)
        {
            FriendChangesStatus?.Invoke(this, e);
        }

        public object ExecuteMessage(Message message)
        {
            switch (message.MessageType)
            {
                case "StatusChangeAck":

                    break;
                case "ReturnGetUserStatus":
                    var messageData = new {userId = 0, userStatus = UserStatus.Offline};
                    messageData = JsonConvert.DeserializeAnonymousType(message.MessageData, messageData);
                    var statusChangeEvent = new StatusChangeEventArgs(messageData.userId, messageData.userStatus);
                    OnFriendChangesStatus(statusChangeEvent);
                    break;
                case "JoinQueueAck":
                    break;
                case "LeaveQueueAck":
                    break;
                default:
                    return null;
            }

            return null;
        }
    }

    public class StatusChangeEventArgs : EventArgs
    {
        public int UserId;
        public UserStatus UserStatus;

        public StatusChangeEventArgs(int userId, UserStatus userStatus)
        {
            UserId = userId;
            UserStatus = userStatus;
        }
    }
}