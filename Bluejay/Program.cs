using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace Bluejay
{
	class Program
	{
		private const string SupportedTags = "\t%name% - Original Filename\n" + 
											 "\t%title% - Title\n" +
		                                     "\t%artists% - Artists\n" +
											 "\t%performers% - Performers\n" +
		                                     "\t%album% - Album\n" +
		                                     "\t%composers% - Composers\n" +
		                                     "\t%year% - Year\n" +
		                                     "\t%track% - Track\n";

		private const string ProperUsage = "Correct Usage: <directory to monitor> <directory structure format> [-o <log file>] [-i <monitor interval | default: 5 sec>] [-help]";
		private const string ExampleUsage = "Example Usage: ./bluejay music \"%artists%\%albums%\%track% - %title%\" -o bluejay.log";

		private const int MessageDelay = 1; //Note that this is in seconds.

		private static string _monitor;
		private static string _structure;
		private static StreamWriter _log;
		private static readonly Queue Messages = new Queue();
		private static int _delay = 5; //Note that this is in seconds.

		static void Write(string msg)
		{
			if (_log != null)
			{
				_log.WriteLine(msg);
				_log.Flush();
			}
			else
				Console.WriteLine(msg);
		}

		static void Error(string msg)
		{
			Write(msg);
			Environment.Exit(0);
		}

		static void Monitor()
		{
			var ignore = new ArrayList();

			while (true)
			{
				var files = Directory.GetFiles(_monitor);

				foreach(var path in files)
				{
					if (!ignore.Contains(path))
					{
						try
						{
							var file = TagLib.File.Create(path);
							var newPath = _structure;

							newPath = newPath.Replace("%name%", Path.GetFileNameWithoutExtension(path));
							newPath = newPath.Replace("%title%", file.Tag.Title);
							newPath = newPath.Replace("%artists%", file.Tag.JoinedAlbumArtists);
							newPath = newPath.Replace("%performers%", file.Tag.JoinedPerformers);
							newPath = newPath.Replace("%album%", file.Tag.Album);
							newPath = newPath.Replace("%composers%", file.Tag.JoinedComposers);
							newPath = newPath.Replace("%year%", file.Tag.Year + "");
							newPath = newPath.Replace("%track%", file.Tag.Track + "");

							newPath = _monitor + "\\" + newPath + Path.GetExtension(path);

							var directory = Path.GetDirectoryName(newPath);

							if (directory != null)
							{
								Directory.CreateDirectory(directory);
							}

							File.Move(path, newPath);

							Messages.Enqueue(String.Format("Moved {0} to {1}.", path, newPath));
						}
						catch (PathTooLongException)
						{
							ignore.Add(path);
							Messages.Enqueue(
								String.Format(
									"Could not move the file <{0}> because the resulting path length would be too long. " +
									"This file path will be ignored for the duration of the program.", path
									)
								);
						}
						catch (ArgumentException)
						{
							ignore.Add(path);
							Messages.Enqueue(
								String.Format(
									"Could not move the file <{0}> because the resulting path was not valid. Please verify that " +
									"specified directory structure is correct. A tag may also be the cause of this problem. " +
									"This file path will be ignored for the duration of the program.", path
									)
								);
						}
						catch (IOException)
						{
							//This probably was raised because a file was still being copied/moved.
							//Simply wait for the file to not be in use.
						}
						catch (Exception e)
						{
							ignore.Add(path);
							Messages.Enqueue(e.ToString());
						}
					}
				}

				Thread.Sleep(_delay * 1000);
			}

		//ReSharper disable once FunctionNeverReturns
		//This function is working as intended and should never end.
		}

		static void InvalidArguments()
		{
			Write("Error: Invalid arguments passed.\n");
			ShowHelp();
		}

		static void ShowHelp()
		{
			Error(String.Format("{0}\n\nSupported Format Tags:\n{1}\n{2}", ProperUsage, SupportedTags, ExampleUsage));
		}

		static void Main(string[] args)
		{

			for (int n = 0; n < args.Length; n++)
			{
				if(n > 6)
					InvalidArguments();

				string arg = args[n];

				switch (arg)
				{
					case "-o":
						if (n + 1 < args.Length)
							_log = File.AppendText(args[n + 1]);
							
						n++;
						break;

					case "-i":
						if (n + 1 < args.Length)
							if (!(int.TryParse(args[n + 1], out _delay)))
							{
								InvalidArguments();
							}

						n++;
						break;

					case "-help":
						ShowHelp();
						break;

					default:
						if (_monitor == null)
							_monitor = args[n];
						else if (_structure == null)
							_structure = args[n];
						else
							InvalidArguments();
						break;

				}
			}

			if(_monitor == null || _structure == null)
				InvalidArguments();

			//ReSharper disable once AssignNullToNotNullAttribute
			//_monitor is never NULL when it reaches here because of the check on line 169.
			if (Directory.Exists(_monitor))
			{
				var thread = new Thread(Monitor);
				thread.Start();

				while (true)
				{
					while(Messages.Count > 0)
						Write((string) Messages.Dequeue());

					Thread.Sleep(MessageDelay * 1000);
				}
			}

			Error(String.Format("The directory {0} does not exist.", args[0]));
		}
	}
}
