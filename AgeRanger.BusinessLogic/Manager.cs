using System.Linq;
using System.Collections.Generic;
using AgeRanger.Interfaces.Logic;
using AgeRanger.Interfaces.Data;
using System.Threading.Tasks;
using AgeRanger.BusinessLogic.Models;
using AgeRanger.BusinessLogic.Exceptions;

namespace AgeRanger.BusinessLogic
{
    public class Manager : IManager
    {

        private IAgeGroupRepository _ageRangeRepository;
        private IPersonRepository _personRepository;

        public Manager(IAgeGroupRepository ageRangeRepository, IPersonRepository personRepository)
        {
            _ageRangeRepository = ageRangeRepository;
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<IPersonModel>> GetPersonList()
        {
            var ageGroups = await GetAgeGroups();
 
            var persons = await _personRepository.GetPersons();

            return persons.Select(p => GetPersonModel(p, ageGroups));
        }

        public async Task<IPersonModel> GetPersonById(long id)
        {
            var ageGroups = await GetAgeGroups();

            var person = await _personRepository.GetPerson(id);

            if (person == null)
            {
                throw new PersonNotFoundException($"Person with id = {id} is not in database.");
            }

            return GetPersonModel(person, ageGroups);
        }

        public async Task<IPersonModel> AddOrUpdatePerson(IPersonModel person)
        {
            var ageGroups = await GetAgeGroups();

            var updatedPerson = await _personRepository.AddOrUpdate(
                person.Id, person.FirstName, person.LastName, person.Age);

            return GetPersonModel(updatedPerson, ageGroups);
        }

        public async Task RemovePerson(long id)
        {
            await _personRepository.Delete(id);
        }

        #region Private methods

        private async Task<IEnumerable<IAgeGroupModel>> GetAgeGroups()
        {
            var ageGroups = await _ageRangeRepository.GetAgeGroups();
            if (ageGroups.Count() == 0)
            {
                throw new AgeGroupsEmptyException("There are no age groups in database. Contact DB Administrator.");
            }

            return ageGroups.Select(ag => new AgeGroupModel()
                            {
                                Id = ag.Id,
                                MinAge = ag.MinAge,
                                MaxAge = ag.MaxAge,
                                Description = ag.Description
                            });
        }

        private static IPersonModel GetPersonModel(IPersonData data, IEnumerable<IAgeGroupModel> groups)
        {
            return new PersonModel()
            {
                Id = data.Id,
                Age = data.Age,
                FirstName = data.FirstName,
                LastName = data.LastName,
                AgeGroup = FindAgeGroup(data.Age, groups)
            };
        }

        private static IAgeGroupModel FindAgeGroup(long? age, IEnumerable<IAgeGroupModel> groups)
        {
            if (age == null)
            {
                // We don't allow null for Age in UI, but if it appear somehow in DB 
                //it should be interpreted as group with minimum age as null
                return groups.FirstOrDefault(g => (g.MinAge == null));
            }

            return groups.FirstOrDefault(g => (g.MinAge == null || age >= g.MinAge)
                                           && (g.MaxAge == null || age < g.MaxAge));
        }

        #endregion
    }
}
