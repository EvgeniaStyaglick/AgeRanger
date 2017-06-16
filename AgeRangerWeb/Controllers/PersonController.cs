using System.Web.Http;
using AgeRangerWeb.Models;
using AgeRanger.Interfaces.Logic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Linq;
using AgeRanger.BusinessLogic.Models;

namespace AgeRangerWeb.Controllers
{
    public class PersonController : ApiController
    {
        private IManager _manager;

        public PersonController(IManager manager)
        {
            _manager = manager;
        }

        // GET api/person
        [ActionName("get"), HttpGet]
        public async Task<IEnumerable<PersonViewModel>> Get()
        {
            var persons = await _manager.GetPersonList();

            return persons.Select(p => new PersonViewModel(p));
        }

        // GET api/person/5  
        public async Task<PersonViewModel> Get(long id)
        {
            var person = await _manager.GetPersonById(id);

            return new PersonViewModel(person);
        }

        // POST api/person  
        public async Task<HttpResponseMessage> Post(PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newPerson = await _manager.AddOrUpdatePerson(new PersonModel()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = model.Age
                });

                return Request.CreateResponse(HttpStatusCode.Created, 
                    new PersonViewModel(newPerson));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // PUT api/person/5  
        public async Task<HttpResponseMessage> Put(PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updatedPerson = await _manager.AddOrUpdatePerson(new PersonModel()
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = model.Age
                });

                return Request.CreateResponse(HttpStatusCode.OK,
                    new PersonViewModel(updatedPerson));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/person/5  
        public async Task<HttpResponseMessage> Delete(long id)
        {
            await _manager.RemovePerson(id);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}