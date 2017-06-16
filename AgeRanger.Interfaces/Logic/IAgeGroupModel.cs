namespace AgeRanger.Interfaces.Logic
{
    public interface IAgeGroupModel : IEntity
    {
        long? MinAge { get; set; }

        long? MaxAge { get; set; }

        string Description { get; set; }
    }
}
