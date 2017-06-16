using AgeRanger.Interfaces;

namespace AgeRanger.Data.DBModels
{
    public class BaseDBEntity : IEntity
    {
        public long Id { get; set; }
    }
}
