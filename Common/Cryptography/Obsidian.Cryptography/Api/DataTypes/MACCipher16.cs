﻿using System;

namespace Obsidian.Cryptography.Api.DataTypes
{
	public sealed class MACCipher16 : SecureBytes
	{
		public MACCipher16(byte[] data) : base(data)
		{
			// perform datatype-specific validation here
			if (data.Length != 16)
				throw new ArgumentOutOfRangeException("data", "The length must be 16 bytes.");
		}
	}
}