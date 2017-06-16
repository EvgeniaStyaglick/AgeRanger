using AgeRanger.Data.DBModels;
using AgeRanger.Interfaces.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace AgeRanger.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        public async Task<IEnumerable<IPersonData>> GetPersons()
        {
            using (var context = new AgeRangerContext())
            {
                return await Task.Run(() => context.Person.ToList<IPersonData>());
            }
        }

        public async Task<IPersonData> GetPerson(long id)
        {
            using (var context = new AgeRangerContext())
            {
                return await Task.Run(() => context.Person.FirstOrDefault(p => p.Id == id));
            }
        }

        public async Task<IPersonData> AddOrUpdate(long id, string firstName, string lastName, long? age)
        {
            using (var context = new AgeRangerContext())
            {
                var dbPerson = await Task.Run(() => context.Person.FirstOrDefault(p => p.Id == id));
                if (dbPerson == null)
                {
                    dbPerson = new Person()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Age = age
                    };

                    context.Add(dbPerson);
                }
                else
                {
                    dbPerson.Age = age;
                    dbPerson.FirstName = firstName;
                    dbPerson.LastName = lastName;
                }

                await Task.Run(() => context.SaveChanges());
                return dbPerson;
            }
        }

        public async Task Delete(long id)
        {
            using (var context = new AgeRangerContext())
            {
                var dbPerson = await Task.Run(() => context.Person.FirstOrDefault(p => p.Id == id));
                if (dbPerson != null)
                {
                    context.Person.Remove(dbPerson);
                }

                await Task.Run(() => context.SaveChanges());
            }
        }
    }
}
