using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgeRanger.Interfaces.Data
{
    public interface IAgeGroupRepository
    {
        Task<IEnumerable<IAgeGroupData>> GetAgeGroups();
    }
}
