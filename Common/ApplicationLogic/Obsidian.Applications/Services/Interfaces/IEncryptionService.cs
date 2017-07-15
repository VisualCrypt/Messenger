using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Applications.Services.Interfaces
{
	public interface IEncryptionService
	{
		Response SetPassword(string unprunedUTF16LEPassword);

		Response<string> GenerateRandomPassword();

		Response<string> SanitizePassword(string unsanitizedPassword);
	   
	}
}