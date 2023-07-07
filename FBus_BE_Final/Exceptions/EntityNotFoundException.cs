namespace FBus_BE.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; }
        public int Id { get; }
        public string InforMessage { get; }
        public EntityNotFoundException(string entityName, int id) {
            EntityName = entityName;
            Id = id;
            InforMessage = "Could not find this " + EntityName + " by Id = " + Id;
        }
    }
}
