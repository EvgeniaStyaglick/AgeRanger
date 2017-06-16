using AgeRanger.Interfaces.Data;

namespace AgeRanger.Data.DBModels
{
    public partial class AgeGroup: BaseDBEntity, IAgeGroupData
    {
        public long? MinAge { get; set; }
        public long? MaxAge { get; set; }
        public string Description { get; set; }
    }
}
