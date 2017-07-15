using System;

namespace Obsidian.Cryptography.Api.DataTypes
{
    public sealed class RoundsExponent
    {
        public const byte DontMakeRounds = 0xff;

        public byte Value
        {
            get { return _value; }
        }
        readonly byte _value;



        public RoundsExponent(byte value)
        {
            if ((value < 4 || value > 31) && value != 0xff)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The RoundsExponent must be between 4 and 31 (inclusive) or 0xff for '{nameof(DontMakeRounds)}'.");

            _value = value;
        }
    }
}