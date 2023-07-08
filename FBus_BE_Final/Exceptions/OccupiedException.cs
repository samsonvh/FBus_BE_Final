namespace FBus_BE.Exceptions
{
    public class OccupiedException : Exception
    {
        public Dictionary<string, string> Errors { get; set; }
        public OccupiedException(Dictionary<string, string> errors)
        {
            Errors = errors;
        }
    }
}
