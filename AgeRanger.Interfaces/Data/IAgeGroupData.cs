using System;

namespace AgeRanger.Interfaces.Data
{
    public interface IAgeGroupData : IEntity
    {
        long? MinAge { get; set; }

        long? MaxAge { get; set; }

        string Description { get; set; }
    }
}
