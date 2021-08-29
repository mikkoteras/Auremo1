using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Auremo.Network
{
    public class Response
    {
        private static readonly NumberStyles m_FloatStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        private static readonly IFormatProvider m_FloatFormat = CultureInfo.InvariantCulture;

        public string StatusLine
        {
            get;
            set;
        } = null;

        public bool IsOk
        {
            get;
            set;
        } = true;

        public bool IsAck => !IsOk;

        public int AckError
        {
            get;
            set;
        } = -1;

        public int AckCommandListNumber
        {
            get;
            set;
        } = -1;

        public string FailedCommand
        {
            get;
            set;
        } = null;

        public string AckMessage
        {
            get;
            set;
        } = null;

        public IList<Datum> Data
        {
            get;
            set;
        } = new List<Datum>();

        public byte[] BinaryObject
        {
            get;
            set;
        } = null;

        public bool HasData => Data.Count > 0;

        public bool HasBinaryObject => BinaryObject != null;

        public Datum FindFirstDatumWithKey(string key)
        {
            return Data.FirstOrDefault(datum => datum.Key == key);
        }

        public string Parse(string key)
        {
            return FindFirstDatumWithKey(key).Value;
        }

        public string ParseOptional(string key)
        {
            return FindFirstDatumWithKey(key)?.Value;
        }

        public int ParseInt(string key)
        {
            return FindFirstDatumWithKey(key).IntValue();
        }

        public int? ParseOptionalInt(string key)
        {
            return FindFirstDatumWithKey(key)?.IntValue();
        }

        public bool ParseBool(string key)
        {
            return FindFirstDatumWithKey(key).BoolValue();
        }

        public bool? ParseOptionalBool(string key)
        {
            return FindFirstDatumWithKey(key)?.BoolValue();
        }

        public double ParseDouble(string key)
        {
            return FindFirstDatumWithKey(key).DoubleValue();
        }

        public double? ParseOptionalDouble(string key)
        {
            return FindFirstDatumWithKey(key)?.DoubleValue();
        }

        #region TODO remove all these

        public static string ParseStringDatum(string key, IDictionary<string, string> values)
        {
            if (values.ContainsKey(key))
            {
                return values[key]; // TODO might need to be expanded later
            }
            else
            {
                throw new Exception("Key " + key + " not found in response data.");
            }
        }

        public static string ParseOptionalStringDatum(string key, IDictionary<string, string> values)
        {
            return values.ContainsKey(key) ? values[key] : null;
        }

        public static int ParseIntegerDatum(string key, IDictionary<string, string> values)
        {
            if (values.ContainsKey(key))
            {
                if (int.TryParse(values[key], out int result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Expected integer following " + key + ".");
                }
            }
            else
            {
                throw new Exception("Key " + key + " not found in response data.");
            }
        }

        /*
        public static int? ParseOptionalIntegerDatum(string key, IDictionary<string, string> values)
        {
            if (values.ContainsKey(key))
            {
                if (int.TryParse(values[key], out int result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Expected integer following " + key + ".");
                }
            }
            else
            {
                return null;
            }
        }
        */

        public static float ParseFloatDatum(string key, IDictionary<string, string> values)
        {
            if (values.ContainsKey(key))
            {
                if (values[key] == "nan")
                {
                    return float.NaN;
                }
                else if (values[key] == "inf")
                {
                    return float.PositiveInfinity;
                }
                else if (values[key] == "-inf")
                {
                    return float.NegativeInfinity;
                }
                else if (float.TryParse(values[key], m_FloatStyle, m_FloatFormat, out float result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Expected float following " + key + ".");
                }
            }
            else
            {
                throw new Exception("Key " + key + " not found in response data.");
            }
        }

        public static float? ParseOptionalFloatDatum(string key, IDictionary<string, string> values)
        {
            if (values.ContainsKey(key))
            {
                if (float.TryParse(values[key], m_FloatStyle, m_FloatFormat, out float result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Expected float following " + key + ".");
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Exceptions

        public class Exception : System.Exception
        {
            public Exception(string message) :
                base("Network.Response exception: " + message)
            {
            }
        }

        #endregion
    }
}
