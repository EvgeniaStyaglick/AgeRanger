using AgeRanger.Interfaces.Logic;

namespace AgeRanger.BusinessLogic.Models
{
    public class PersonModel : BaseModel, IPersonModel
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; } 

        public long? Age { get; set; }

        public IAgeGroupModel AgeGroup { get; set; }
    }
}
