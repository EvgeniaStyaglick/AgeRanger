using AgeRanger.Interfaces;

namespace AgeRanger.BusinessLogic.Models
{
    public abstract class BaseModel : IEntity
    {
        public long Id { get; set; }
    }
}
