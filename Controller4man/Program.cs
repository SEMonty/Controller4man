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

			

			Xbox360_JoyPad pad = new Xbox360_JoyPad();
			uint padIndex = 0;

			padIndex = 1;

			
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
