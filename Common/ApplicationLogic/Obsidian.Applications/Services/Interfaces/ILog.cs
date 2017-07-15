using System;

namespace Obsidian.Applications.Services.Interfaces
{
	public interface ILog
	{
		void Debug(string info);

		void Exception(Exception e);
	}
}
