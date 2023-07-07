namespace FBus_BE.Exceptions
{
    public class NotEnoughStationForRouteException : Exception
    {
        public string InforMessage { get; set; }
        public NotEnoughStationForRouteException(int id, int count) {
            InforMessage = "Route " + id + " must have at least 2 stations instead of " + count + " in order to be activated.";
        }
    }
}
