namespace FBus_BE.Exceptions
{
    public class TripDateInvalidException : Exception
    {
        public Dictionary<string, string> Errors { get; set; }
        public TripDateInvalidException(Dictionary<string,string> errors) {
            Errors = errors;
        }
    }
}
