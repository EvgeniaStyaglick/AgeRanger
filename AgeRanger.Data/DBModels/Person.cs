using AgeRanger.Interfaces.Data;

namespace AgeRanger.Data.DBModels
{
    public partial class Person : BaseDBEntity, IPersonData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? Age { get; set; }
    }
}
