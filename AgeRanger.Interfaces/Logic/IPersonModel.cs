namespace AgeRanger.Interfaces.Logic
{
    public interface IPersonModel : IEntity
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        long? Age { get; set; }

        IAgeGroupModel AgeGroup { get; set; }
    }
}
