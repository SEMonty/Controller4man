using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller4man
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{

			ssh test = new ssh("10.3.51.246", 22, "cdfw", "xxx");
			test.Connect();
			Console.WriteLine(test.send("pwd"));
			Console.WriteLine(test.send("ls"));
			test.Disconnect();

			/*
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			*/
			Xbox360_JoyPad pad = new Xbox360_JoyPad();
			uint padIndex = 0;

			padIndex = 1;
			// テスト環境でXbox360コントローラが1のため決め打ち 
			// 他の環境や別のジョイパッドを使用する場合は変更するか以下の入力フォームを使用 

			
			while (true)
			{
				uint[] pads = pad.GetJoypads();
				foreach (uint p in pads)
				{ Console.WriteLine("ジョイパッド:" + p); }

				Console.Write("パッドを選択して下さい:");

				string line = Console.ReadLine();
				if (line == "quit") { return; }

				if (!uint.TryParse(line, out padIndex))
				{ continue; }

				break;
			}

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
		}
	}
}
