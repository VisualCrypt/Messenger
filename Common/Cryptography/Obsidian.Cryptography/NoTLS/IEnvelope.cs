using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Cryptography.NoTLS
{
    public interface IEnvelope
    {
	    byte[] EncipheredPayload { get; }
    }
}
