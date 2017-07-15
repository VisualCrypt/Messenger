using Obsidian.Cryptography.Api.Implementations;

namespace Obsidian.Cryptography.Api.Interfaces
{
	public interface IPlatform
	{
		byte[] GenerateRandomBytes(int length);

		byte[] ComputeSHA512(byte[] data);

		byte[] ComputeSHA256(byte[] data);

        byte[] ComputeAESRound(AESDir aesDir, byte[] currentIV, byte[] inputData, byte[] keyBytes);

	}
}