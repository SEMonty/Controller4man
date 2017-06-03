using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Controller4man
{
	/// <summary>
	/// 同期UDP受信
	/// </summary>
	public class UdpReceiverSync

	{

		System.Net.Sockets.UdpClient udp;

		public void connect(string add,int port)
		{
			//バインドするローカルIPとポート番号
			string localIpString = add;
			System.Net.IPAddress localAddress =
				System.Net.IPAddress.Parse(localIpString);
			int localPort = port;

			//UdpClientを作成し、ローカルエンドポイントにバインドする
			System.Net.IPEndPoint localEP =	new System.Net.IPEndPoint(localAddress, localPort);
			udp = new System.Net.Sockets.UdpClient(localEP);
		}

		public string recvwait()
		{
			//データを受信する
			System.Net.IPEndPoint remoteEP = null;
			byte[] rcvBytes = udp.Receive(ref remoteEP);
			
			String rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);
			//受信したデータと送信者の情報を表示する

			Console.WriteLine("受信したデータ:{0}", rcvMsg);
			Console.WriteLine("送信元アドレス:{0}/ポート番号:{1}", remoteEP.Address, remoteEP.Port);

			//データを文字列に変換する
			return rcvMsg;
		}
		public void disconnect()
		{
			//UdpClientを閉じる
			udp.Close();
		}
	}
}

