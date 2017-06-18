using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Byond
{
    public class ByondTopic
    {

        public void SendTopicCommand(string ip, string port, string command)
        {
            GetData(ip, port, command);
        }

        public string GetData(string ip, string port, string command)
        {
            try
            {
                var message = BuildMessage(command);
                var buffer = new byte[4096];
                var address = IPAddress.Parse(ip);
                var endPoint = new IPEndPoint(address, int.Parse(port));

                Socket sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(endPoint);

                sender.Send(message);

                int bytesGot = sender.Receive(buffer);

                sender.Shutdown(SocketShutdown.Both);

                return ParseMessage(buffer, bytesGot);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private byte[] BuildMessage(string command)
        {
            command = "?" + command;

            byte[] message = Encoding.ASCII.GetBytes(command);
            byte[] sendingBytes = new byte[message.Length + 10];

            sendingBytes[1] = 0x83;
            Pack(message.Length + 6).CopyTo(sendingBytes, 2);

            message.CopyTo(sendingBytes, 9);

            return sendingBytes;

            //i have no idea what the fuck i've actually done lol
            //thx to Rotem12 on byond forums
        }

        private byte[] Pack(int num)
        {
            byte[] packed = BitConverter.GetBytes((short)num);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(packed);
            }
            return packed;
        }

        private string ParseMessage(byte[] msgBytes, int bytesGot)
        {
            if ((msgBytes[0] == 0x00) && (msgBytes[1] == 0x83))
            {
                var resp = Encoding.UTF8.GetString(msgBytes, 6, bytesGot - 5);
                return resp;
            }
            return null;
        }
    }
}
