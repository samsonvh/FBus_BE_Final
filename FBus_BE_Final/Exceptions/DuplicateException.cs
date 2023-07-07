namespace FBus_BE.Exceptions
{
    public class DuplicateException : Exception
    {
        private readonly Dictionary<string,string> errors;

        public DuplicateException(Dictionary<string, string> errors)
        {
            this.errors = errors;
        }

        public Dictionary<string, string> GetErrors()
        {
            return this.errors;
        }
    }
}
