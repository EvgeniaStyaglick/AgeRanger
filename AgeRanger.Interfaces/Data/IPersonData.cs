namespace AgeRanger.Interfaces.Data
{
    public interface IPersonData : IEntity
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        long? Age { get; set; }
    }
}
