using System;
using System.Globalization;


namespace Auremo.Network
{
    public class Datum
    {
        private static readonly NumberStyles m_FloatStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        private static readonly IFormatProvider m_FloatFormat = CultureInfo.InvariantCulture;

        public string Key { get; set; } = "";
        public string Value { get; set; } = "";

        public int IntValue()
        {
            if (int.TryParse(Value, out int result))
            {
                return result;
            }
            else
            {
                throw new Exception("Expected integer following " + Key + ".");
            }
        }

        public double DoubleValue()
        {
            if (double.TryParse(Value, m_FloatStyle, m_FloatFormat, out double result))
            {
                return result;
            }
            else
            {
                throw new Exception("Expected double following " + Key + ".");
            }
        }

        public bool BoolValue()
        {
            if (Value == "0")
            {
                return false;
            }
            else if (Value == "1")
            {
                return true;
            }
            else
            {
                throw new Exception("Expected bool following " + Key + ".");
            }
        }

        public override string ToString()
        {
            return Key + ": " + Value;
        }
    }
}
