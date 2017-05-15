using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ByondTopic
{
    public class ByondTopic
    {

        public async Task SendTopicCommand(string ip, string port, string command)
        {
            await GetData(ip, port, command);
        }

        public async Task<string> GetData(string ip, string port, string command)
        {
            try
            {
                var message = BuildMessage(command);
                var buffer = new byte[4096];

                var host = await Dns.GetHostEntryAsync(ip);
                var address = host.AddressList[0];
                var endPoint = new IPEndPoint(address, int.Parse(port));

                Socket sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await sender.ConnectAsync(endPoint);

                sender.Send(message);

                int bytesGot = sender.Receive(buffer);

                sender.Shutdown(SocketShutdown.Both);

                return ParseMessage(buffer, bytesGot);
            }
            catch
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
            byte[] packed = BitConverter.GetBytes((short) num);
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
                return Encoding.UTF8.GetString(msgBytes, 6, bytesGot - 5);
            }
            return null;
        }
    }
}
