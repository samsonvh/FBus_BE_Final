namespace FBus_BE.Utils
{
    public class TextUtil
    {
        public static string Capitalize(string text)
        {
            text = text.Substring(0,1).ToUpper() + text.Substring(1).ToLower();
            return text;
        }
    }
}
