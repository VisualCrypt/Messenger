using System;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Cryptography.Api.Interfaces
{
	public interface IEncryptionProgress : IProgress<EncryptionProgress>
	{
		int Percent { get; set; }

		string Message { get; set; }
	}
}