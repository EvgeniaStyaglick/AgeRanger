using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgeRanger.Interfaces.Logic
{
    public interface IManager
    {
        Task<IEnumerable<IPersonModel>> GetPersonList();

        Task<IPersonModel> GetPersonById(long id);

        Task<IPersonModel> AddOrUpdatePerson(IPersonModel person);

        Task RemovePerson(long id);
    }
}
