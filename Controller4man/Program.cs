using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Console = Colorful.Console;

namespace Controller4man
{
	static class Program
	{

		static List<string> message = new List<string>();
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
        {            //////////////////////////////////config

            double steer_range = 0.06;
            double steer_center = 0.13;
            double acc_range = 0.04;
            double acc_center = 0.138;


            //アイテムAスピード1.2倍
            double itemA_effect = 1.2;
            int itemA_time = 0;
            //アイテムBスピード0.5倍
            double itemB_effect = 0.5;
            int itemB_time = 0;

            string myadd = "127.0.0.1";
            int listenport = 6001;

            string piadd = "192.168.10.17";
            int piport = 6001;

            //メインループのWait時間(ms)
            int waitms = 100;

            Xbox360_JoyPad pad = new Xbox360_JoyPad();
            uint padIndex = 0;

            ////////////////////////////////////////ジョイパッド選択


            while (true)
            {
                uint[] pads = pad.GetJoypads();
                foreach (uint p in pads)
                { Console.WriteLine("connected joypad number:" + p); }

                Console.Write("please select joypad number:");

                string line = Console.ReadLine();
                if (line == "quit") { return; }

                if (!uint.TryParse(line, out padIndex))
                { continue; }

                break;
            }
            /////////////////////////////////////////////////////////////////////////////////
            ////ジョイパッド出力の確認///////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////
            /*
            while (true)
            {
                System.Threading.Thread.Sleep(100);

                JoyPad.JOYERR err = pad.GetPosEx(padIndex);
                JoyPad.JOYINFOEX ex = pad.JoyInfoEx;

                if (err != JoyPad.JOYERR.NOERROR)
                {
                    Console.WriteLine("エラー");
                    continue;
                }

                Console.Write(" " + pad.LeftStickX.ToString("+0.000;-0.000; 0.000"));
                Console.Write(" " + pad.LeftStickY.ToString("+0.000;-0.000; 0.000"));
                Console.Write(" " + pad.RightStickX.ToString("+0.000;-0.000; 0.000"));
                Console.Write(" " + pad.RightStickY.ToString("+0.000;-0.000; 0.000"));
                Console.Write(" " + pad.Trigger.ToString("+0.000;-0.000; 0.000"));

                Console.Write(" ");
                Console.Write(pad.ButtonA ? "*" : "-");
                Console.Write(pad.ButtonB ? "*" : "-");
                Console.Write(pad.ButtonX ? "*" : "-");
                Console.Write(pad.ButtonY ? "*" : "-");
                Console.Write(pad.ButtonLeftShoulder ? "*" : "-");
                Console.Write(pad.ButtonRightShoulder ? "*" : "-");
                Console.Write(pad.ButtonBack ? "*" : "-");
                Console.Write(pad.ButtonStart ? "*" : "-");
                Console.Write(pad.ButtonLeftStick ? "*" : "-");
                Console.Write(pad.ButtonRightStick ? "*" : "-");

                Console.WriteLine();
            }
            */
            /////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////





            ///////////////////////////////////////////開始待ち
            /*
			UdpReceiverSync rec = new UdpReceiverSync();

			Console.WriteLine("Waiting for 'start' message");

			rec.connect(myadd, listenport);
			while (true)
			{
				if (rec.recvwait().Equals("start"))
					break;

			}
			rec.disconnect();
            */
            ///////////////////////////////////////////ゲーム中

            //非同期受信
            UdpClient rec2 = new UdpClient(listenport);
			rec2.BeginReceive(new AsyncCallback(OnUdpData), rec2);

			//同期送信。（非同期でもいいけどめんどい）
			UdpSender udp = new UdpSender();
			udp.connect(piadd, piport);


			while (true)
			{
				System.Threading.Thread.Sleep(waitms);

				JoyPad.JOYERR err = pad.GetPosEx(padIndex);
				JoyPad.JOYINFOEX ex = pad.JoyInfoEx;

				if (err != JoyPad.JOYERR.NOERROR)
				{
					Console.WriteLine("joypad error");
					continue;
				}


				//アイテム効果後のレンジ
				double acc_range_efected = acc_range;
				if (itemA_time > 0)
				{
					acc_range_efected = acc_range_efected * itemA_effect;
					itemA_time -= waitms;
				}
				else if (itemB_time > 0)
				{

					acc_range_efected = acc_range_efected * itemB_effect;
					itemB_time -= waitms;
				}

                //指令値の計算
				double steer_cmd = (((pad.LeftStickX) + 1) / 2 )* steer_range + steer_center - steer_range/2;
				double acc_cmd = (((pad.Trigger) + 1) / 2) * acc_range_efected + acc_center - acc_range_efected / 2;

                //送信
				udp.send("s" + steer_cmd);
				udp.send("a" + acc_cmd);
                //表示
                Console.Write(" " + steer_cmd.ToString("+0.000;-0.000; 0.000"));
                Console.Write(" " + acc_cmd.ToString("+0.000;-0.000; 0.000"));
                Console.WriteLine();

                ///ボタンAでラジコンのPWM生成器の再起動
                if (pad.ButtonA)
                {
                    udp.send("q");
                    Console.Write("pwm generator restart");
                }

                //受信したメッセージで分岐する
                foreach (string mes in message)
				{
					if (mes.Equals("hw"))//human win　最優先
					{
						Console.WriteAscii("You win!!");
						Console.WriteLine("Any key push exit");
						Console.ReadKey();
						return;
					}
					else if (mes.Equals("cw"))//cpu win
					{
						Console.WriteAscii("You lose!!");
						Console.WriteLine("Any key push exit");
						Console.ReadKey();
						return;
					}
					else if (mes.Equals("ia"))//itemを取った
					{
						Console.WriteAscii("10sec speed up!!");
						itemA_time = 10 * 1000 ;
					}
					else if (mes.Equals("ib"))//itemを取られた
					{
						Console.WriteAscii("10sec speed down!!");
						itemB_time = 10 * 1000 ;
					}
				}
				message.Clear();				
			}

		}

		static void OnUdpData(IAsyncResult result)
		{
			// this is what had been passed into BeginReceive as the second parameter:
			UdpClient socket = result.AsyncState as UdpClient;
			// points towards whoever had sent the message:
			IPEndPoint source = new IPEndPoint(0, 0);
			// get the actual message and fill out the source:
			byte[] rcvBytes = socket.EndReceive(result, ref source);

			String rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);

			message.Add(rcvMsg);

			// do what you'd like with `message` here:
			//Console.WriteLine(rcvMsg + " from " + source);
			// schedule the next receive operation once reading is done:
			socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
		}
	}
}
