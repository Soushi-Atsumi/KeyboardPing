/*
 * KeyboardPing - Send echo requests to the keyboard.
 * Copyright (c) 2019 Soushi Atsumi. All rights reserved.
 *
 * This Source Code Form is subject to the terms of The MIT License.
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace KeyboardPing {
	class Program {
		static void Main(string[] args) {
			bool shouldRevert = false;
			bool shouldShowHelp = false;
			int interval = 1000;
			int pingCounts = 4;
			String commandArguments = "";
			String commandFilename = null;
			String key = Keys.NUMLOCK;

			for (int i = 0; i < args.Length && !shouldShowHelp; i += 2) {
				switch (args[i]) {
					case Options.COMMAND:
						if (i + 1 == args.Length) {
							Console.Error.WriteLine("Command is not specified.");
							shouldShowHelp = true;
						} else {
							String[] command = args[i + 1].Split(new Char[] { ' ' }, 2);
							commandFilename = command[0];

							if (command.Length == 2) {
								commandArguments = command[1];
							}
						}

						break;
					case Options.COUNT:
						if (i + 1 == args.Length) {
							Console.Error.WriteLine("Count is not specified.");
							shouldShowHelp = true;
						} else if (!int.TryParse(args[i + 1], out pingCounts)) {
							Console.Error.WriteLine($"{args[i + 1]} is invalid value for the count.");
							shouldShowHelp = true;
						}

						break;
					case Options.HELP:
						shouldShowHelp = true;
						break;
					case Options.INTERVAL:
						if (i + 1 == args.Length) {
							Console.Error.WriteLine("Interval is not specified.");
							shouldShowHelp = true;
						} else if (!int.TryParse(args[i + 1], out interval)) {
							Console.Error.WriteLine($"{args[i + 1]} is invalid value for the interval.");
							shouldShowHelp = true;
						} else if (interval <= 0) {
							Console.WriteLine("Interval must be more than zero.");
							shouldShowHelp = true;
						}

						break;
					case Options.KEY:
						if (i + 1 == args.Length) {
							Console.Error.WriteLine($"Key is not specified.");
							shouldShowHelp = true;
						} else {
							key = args[i + 1];
						}

						break;
					case Options.LIST:
						Console.WriteLine(Keys.AvailableKeys);
						return;
					case Options.REVERT:
						shouldRevert = true;
						i--;
						break;
					default:
						Console.Error.WriteLine($"Unknown Option: {args[i]}.");
						shouldShowHelp = true;
						break;
				}
			}

			if (shouldShowHelp) {
				Console.WriteLine();
				Console.WriteLine("======================================================");
				Console.WriteLine("KeyboardPing Ver 1.0.0 - Send echo requests to the keyboard.");
				Console.WriteLine("Copyright (c) 2019 Soushi Atsumi. All rights reserved.");
				Console.WriteLine();
				Console.WriteLine("This project is licensed under The MIT License.");
				Console.WriteLine("======================================================");
				Console.WriteLine();
				Console.WriteLine("KeyboardPing [-c command] [-n count] [-h] [-i] [-k key] [-l] [-r]");
				Console.WriteLine("    -c command  The command which is executed after exceed counts.");
				Console.WriteLine("    -n count    Maximum counts of the sending failure. Ping until terminated if zero or lower is passed.");
				Console.WriteLine("                The default value is 4.");
				Console.WriteLine("                Type Control + C to terminate the program.");
				Console.WriteLine("    -h          Display this help.");
				Console.WriteLine("    -i interval Interval of pings in milliseconds. The default value is 1000.");
				Console.WriteLine("    -k key      The key what you want to send. The default value is {NUMLOCK}.");
				Console.WriteLine("    -l          List avalable keys.");
				Console.WriteLine("    -r          Use the sending success for counts instead of the sending faulure.");
				Console.WriteLine();
				Console.WriteLine("Examples");
				Console.WriteLine("    Example 1: Send an echo request with a 'A' key four times.");
				Console.WriteLine(@"    PS C:\> KeyboardPing -k 'A'" + Environment.NewLine);
				Console.WriteLine("    Example 2: Shutdown the computer after failed to send ten times. This is useful when the keyboard does not work.");
				Console.WriteLine(@"    PS C:\> KeyboardPing -c 'shutdown /s' -n 10" + Environment.NewLine);
				Console.WriteLine("    Example 3: Keep pinging infinitely. You need to terminate the program by yourself.");
				Console.WriteLine(@"    PS C:\> KeyboardPing -n 0" + Environment.NewLine);
				Console.WriteLine("    Example 4: Type 'Hello' repeatedly.");
				Console.WriteLine(@"    PS C:\> KeyboardPing -k 'Hello'" + Environment.NewLine);
				Console.WriteLine("    Example 5: Type a 'A' key rapidly. Strongly recommend use this option with the '-n' option and the '-r' option.");
				Console.WriteLine(@"    PS C:\> KeyboardPing -i 10 -k 'A' -n 100 -r" + Environment.NewLine);

				return;
			}

			if (pingCounts <= 0) {
				Console.WriteLine("Keep pinging infinitely.");
			}

			if (commandFilename != null) {
				Console.WriteLine($"Run \"{commandFilename}{(commandArguments == "" ? "" : $" {commandArguments}")}\" after stop.");
			}

			while (true) {
				bool succeeded;

				try {
					Thread.Sleep(interval);
					System.Windows.Forms.SendKeys.SendWait(key);
					Console.WriteLine($"{DateTime.Now}: Succeeded.");
					succeeded = true;
				} catch (ThreadInterruptedException) {
					Console.WriteLine($"{DateTime.Now}: Interrupted.");
					break;
				} catch (Exception e) {
					Console.Error.WriteLine($"{DateTime.Now}: Failed({e.Message}).");
					succeeded = false;
				}

				if ((shouldRevert ? succeeded : !succeeded) && --pingCounts == 0) {
					if (commandFilename != null) {
						try {
							System.Diagnostics.Process.Start(commandFilename, commandArguments);
						} catch (Exception e) {
							Console.Error.WriteLine(e.Message);
						}
					}

					break;
				}
			}
		}

		class Keys {
			public const String BACKSPACE = "{BACKSPACE}";
			public const String BREAK = "{BREAK}";
			public const String CAPSLOCK = "{CAPSLOCK}";
			public const String DEL = "{DELETE}";
			public const String DOWNARROW = "{DOWN}";
			public const String END = "{END}";
			public const String ENTER = "{ENTER}";
			public const String ESC = "{ESC}";
			public const String HELP = "{HELP}";
			public const String HOME = "{HOME}";
			public const String INS = "{INSERT}";
			public const String LEFTARROW = "{LEFT}";
			public const String NUMLOCK = "{NUMLOCK}";
			public const String PAGEDOWN = "{PGDN}";
			public const String PAGEUP = "{PGUP}";
			public const String PRINTSCREEN = "{PRTSC}";
			public const String RIGHTARROW = "{RIGHT}";
			public const String SCROLLLOCK = "{SCROLLLOCK}";
			public const String TAB = "{TAB}";
			public const String UPARROW = "{UP}";
			public const String F1 = "{F1}";
			public const String F2 = "{F2}";
			public const String F3 = "{F3}";
			public const String F4 = "{F4}";
			public const String F5 = "{F5}";
			public const String F6 = "{F6}";
			public const String F7 = "{F7}";
			public const String F8 = "{F8}";
			public const String F9 = "{F9}";
			public const String F10 = "{F10}";
			public const String F11 = "{F11}";
			public const String F12 = "{F12}";
			public const String F13 = "{F13}";
			public const String F14 = "{F14}";
			public const String F15 = "{F15}";
			public const String F16 = "{F16}";
			public const String KEYPADADD = "{ADD}";
			public const String KEYPADSUBTRACT = "{SUBTRACT}";
			public const String KEYPADMULTIPLY = "{MULTIPLY}";
			public const String KEYPADDIVIDE = "{DIVIDE}";
			public readonly static String AvailableKeys =
				$"---Ordinary Keys---{Environment.NewLine}" +
				$"!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{{|}}~{Environment.NewLine}" +
				$"---Special Keys---{Environment.NewLine}" +
				$"BACKSPACE:      {BACKSPACE}{Environment.NewLine}" +
				$"BREAK:          {BREAK}{Environment.NewLine}" +
				$"CAPSLOCK:       {CAPSLOCK}{Environment.NewLine}" +
				$"DEL:            {DEL}{Environment.NewLine}" +
				$"DOWNARROW:      {DOWNARROW}{Environment.NewLine}" +
				$"END:            {END}{Environment.NewLine}" +
				$"ENTER:          {ENTER}{Environment.NewLine}" +
				$"ESC:            {ESC}{Environment.NewLine}" +
				$"HELP:           {HELP}{Environment.NewLine}" +
				$"HOME:           {HOME}{Environment.NewLine}" +
				$"INS:            {INS}{Environment.NewLine}" +
				$"LEFTARROW:      {LEFTARROW}{Environment.NewLine}" +
				$"NUMLOCK:        {NUMLOCK}{Environment.NewLine}" +
				$"PAGEDOWN:       {PAGEDOWN}{Environment.NewLine}" +
				$"PAGEUP:         {PAGEUP}{Environment.NewLine}" +
				$"PRINTSCREEN:    {PRINTSCREEN}{Environment.NewLine}" +
				$"RIGHTARROW:     {RIGHTARROW}{Environment.NewLine}" +
				$"SCROLLLOCK:     {SCROLLLOCK}{Environment.NewLine}" +
				$"TAB:            {TAB}{Environment.NewLine}" +
				$"UPARROW:        {UPARROW}{Environment.NewLine}" +
				$"F1:             {F1}{Environment.NewLine}" +
				$"F2:             {F2}{Environment.NewLine}" +
				$"F3:             {F3}{Environment.NewLine}" +
				$"F4:             {F4}{Environment.NewLine}" +
				$"F5:             {F5}{Environment.NewLine}" +
				$"F6:             {F6}{Environment.NewLine}" +
				$"F7:             {F7}{Environment.NewLine}" +
				$"F8:             {F8}{Environment.NewLine}" +
				$"F9:             {F9}{Environment.NewLine}" +
				$"F10:            {F10}{Environment.NewLine}" +
				$"F11:            {F11}{Environment.NewLine}" +
				$"F12:            {F12}{Environment.NewLine}" +
				$"F13:            {F13}{Environment.NewLine}" +
				$"F14:            {F14}{Environment.NewLine}" +
				$"F15:            {F15}{Environment.NewLine}" +
				$"F16:            {F16}{Environment.NewLine}" +
				$"KEYPADADD:      {KEYPADADD}{Environment.NewLine}" +
				$"KEYPADSUBTRACT: {KEYPADSUBTRACT}{Environment.NewLine}" +
				$"KEYPADMULTIPLY: {KEYPADMULTIPLY}{Environment.NewLine}" +
				$"KEYPADDIVIDE:   {KEYPADDIVIDE}{Environment.NewLine}";
		}

		class Options {
			public const String COMMAND = "-c";
			public const String COUNT = "-n";
			public const String HELP = "-h";
			public const String INTERVAL = "-i";
			public const String KEY = "-k";
			public const String LIST = "-l";
			public const String REVERT = "-r";
			public readonly List<String> AllOptions = new List<String> { COUNT, COMMAND, KEY, REVERT };
		}
	}
}
