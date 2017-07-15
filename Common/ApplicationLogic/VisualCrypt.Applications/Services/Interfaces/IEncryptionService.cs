using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Applications.Services.Interfaces
{
	public interface IEncryptionService
	{
		Response SetPassword(string unprunedUTF16LEPassword);

		Response<string> GenerateRandomPassword();

		Response<string> SanitizePassword(string unsanitizedPassword);
	   
	}
}