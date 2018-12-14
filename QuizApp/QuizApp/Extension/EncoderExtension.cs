using System;
using System.Text;

namespace QuizApp.Extension
{
    public static class EncoderExtension
    {
        public static string Base64ToString(this Encoding encoding, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var base64encodedbytes = Convert.FromBase64String(value);
                return encoding.GetString(base64encodedbytes);
            }
            else
                return string.Empty;
        }

        public static string StringToBase64(this Encoding encoding, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var stringBytes = encoding.GetBytes(value);
                return Convert.ToBase64String(stringBytes);
            }
            else
                return string.Empty;
        }
    }
}
