using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgeRanger.Interfaces.Data
{
    public interface IPersonRepository
    {
        Task<IEnumerable<IPersonData>> GetPersons();

        Task<IPersonData> GetPerson(long id);

        Task<IPersonData> AddOrUpdate(long id, string firstName, string lastName, long? age);

        Task Delete(long id);
    }
}
