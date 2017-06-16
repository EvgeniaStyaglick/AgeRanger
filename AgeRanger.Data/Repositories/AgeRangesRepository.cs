using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using AgeRanger.Interfaces.Data;

namespace AgeRanger.Data.Repositories
{
    public class AgeGroupRepository : IAgeGroupRepository
    {
        public async Task<IEnumerable<IAgeGroupData>> GetAgeGroups()
        {
            using (var context = new AgeRangerContext())
            {
                return await Task.Run(() => context.AgeGroup.ToList<IAgeGroupData>());
            }
        }
    }
}
