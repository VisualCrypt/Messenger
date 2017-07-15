using System;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Cryptography.Api.Implementations
{
	public class SymmetricKeyRepository
	{
		KeyMaterial64 _keyMaterial64;

		public KeyMaterial64 GetPasswordHash()
		{
			if (_keyMaterial64 == null)
				throw new InvalidOperationException("SymmetricKeyRepository: _keyMaterial64 is null.");
			return _keyMaterial64;
		}

		public void SetPasswordHash(KeyMaterial64 keyMaterial64)
		{
			_keyMaterial64 = keyMaterial64;
		}

		public void Clear()
		{
			if (_keyMaterial64 != null)
				_keyMaterial64.GetBytes().FillWithZeros();
			_keyMaterial64 = null;
		}
	}
}