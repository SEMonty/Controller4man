using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller4man
{
	public class UdpSender
	{
		System.Net.Sockets.UdpClient udp;
		string remoteHost;
		int remotePort;
		public void connect(string add,int port)
		{
			//データを送信するリモートホストとポート番号
			remoteHost = add;
			remotePort = port;

			//UdpClientオブジェクトを作成する
			udp = new System.Net.Sockets.UdpClient();
		}
		public void disconnect()
		{

			udp.Close();
		}
		public void send(string msg)
		{
			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(msg);
			//リモートホストを指定してデータを送信する
			udp.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);

		}
	}
}
