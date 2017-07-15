using System;
using System.Collections.Generic;
using Obsidian.Applications.Mvvm.Logging;
using Obsidian.Applications.Services.Interfaces;

namespace Obsidian.Applications.Services.PortableImplementations
{
	public class ReplayLogger : /* ILoggerFacade, */ ILog
	{
		readonly Queue<Tuple<string, Category, Priority>> _savedLogs =
			new Queue<Tuple<string, Category, Priority>>();


		public Action<string, Category, Priority> Callback;


		public void Log(string message, Category category, Priority priority)
		{
			var date = DateTime.Now;
			var time = $"{date.Hour}:{date.Minute}:{date.Second}";
			string timestamped = $"{time} {message}";
			if (Callback != null)
			{
				Callback(timestamped, category, priority);
			}
			else
			{
				if (_savedLogs.Count > 100)
					_savedLogs.Dequeue();
				_savedLogs.Enqueue(new Tuple<string, Category, Priority>(timestamped, category, priority));
			}
		}

		public void ReplaySavedLogs(Action<string, Category, Priority> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			while (_savedLogs.Count > 0)
			{
				var log = _savedLogs.Dequeue();
				Callback(log.Item1, log.Item2, log.Item3);
			}
		}

		public void Debug(string p)
		{
			Log(p, Category.Debug, Priority.Low);
            System.Diagnostics.Debug.WriteLine("Debug: {0}", p);
        }


		public void Exception(Exception e)
		{
			Log(e.Message,Category.Exception, Priority.High);
            System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
        }
	}
}