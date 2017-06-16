using AgeRanger.Interfaces.Logic;

namespace AgeRanger.BusinessLogic.Models
{
    public class AgeGroupModel : BaseModel, IAgeGroupModel
    {
        public long? MinAge { get; set; }

        public long? MaxAge { get; set; }

        public string Description { get; set; }
    }
}
