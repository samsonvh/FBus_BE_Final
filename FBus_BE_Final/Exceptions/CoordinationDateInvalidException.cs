namespace FBus_BE.Exceptions
{
    public class CoordinationDateInvalidException : Exception
    {
        public Dictionary<string, string> Errors { get; set; }
        public CoordinationDateInvalidException(Dictionary<string,string> errors) {
            Errors = errors;
        }
    }
}
