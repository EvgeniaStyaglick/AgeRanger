using AgeRanger.BusinessLogic.Models;
using AgeRanger.Interfaces.Data;
using AgeRanger.Interfaces.Logic;
using AgeRanger.Tests.TestUtils;
using AgeRangerWeb.Controllers;
using AgeRangerWeb.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;

namespace AgeRanger.Tests.Controllers
{
    [TestFixture]
    public class PersonControllerTests
    {
        #region Test data

        private static IEnumerable<IPersonModel> _persons = new List<PersonModel>()
        {
            new PersonModel() { Id = 1, FirstName = "F1", LastName = "L1", Age = 0 ,
                AgeGroup = new AgeGroupModel { Id = 1, MinAge = null, MaxAge = 10, Description = "G1" } },
            new PersonModel() { Id = 2, FirstName = "F1", LastName = "L1", Age = 55 ,
                AgeGroup = new AgeGroupModel { Id = 2, MinAge = 10, MaxAge = 100, Description = "G2" } },
            new PersonModel() { Id = 3, FirstName = "F1", LastName = "L1", Age = 555 ,
                AgeGroup = new AgeGroupModel { Id = 3, MinAge = 100, MaxAge = null, Description = "G3" } }
        };

        private static IEnumerable<IPersonModel> _emptyPersons = new List<PersonModel>();

        #endregion

        #region Get list tests

        [Test]
        public void PersonController_GetList_WithItems_Successfull()
        {
            var manager = new Mock<IManager>();

            manager.Setup(x => x.GetPersonList()).Returns(Task.FromResult(_persons));

            var controller = new PersonController(manager.Object);

            var result = controller.Get().Result;

            var sourceList = _persons.ToList();
            var resultList = result.ToList();

            Assert.AreEqual(sourceList.Count(), resultList.Count());
            CheckPerson(sourceList[0], resultList[0]);
            CheckPerson(sourceList[1], resultList[1]);
            CheckPerson(sourceList[2], resultList[2]);
         }

        [Test]
        public void PersonController_GetList_Empty_Successfull()
        {
            var manager = new Mock<IManager>();

            manager.Setup(x => x.GetPersonList()).Returns(Task.FromResult(_emptyPersons));

            var controller = new PersonController(manager.Object);

            var result = controller.Get().Result;

            Assert.AreEqual(0, result.ToList().Count());
        }

        [Test]
        public void PersonController_GetList_Exception_ThrowsException()
        {
            var manager = new Mock<IManager>();

            manager.Setup(x => x.GetPersonList()).ThrowsAsync(new TestException());

            var controller = new PersonController(manager.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => controller.Get().Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region Get person by id tests

        [Test]
        public void PersonController_GetById_Successfull()
        {
            var manager = new Mock<IManager>();
            var testId = 2;
            var foundPerson = _persons.FirstOrDefault(p => p.Id == testId);

            manager.Setup(x => x.GetPersonById(testId)).Returns(Task.FromResult(foundPerson));

            var controller = new PersonController(manager.Object);

            var result = controller.Get(testId).Result;

            CheckPerson(foundPerson, result);
         }

        [Test]
        public void PersonController_GetById_ThrowsException()
        {
            var manager = new Mock<IManager>();

            manager.Setup(x => x.GetPersonById(-1)).ThrowsAsync(new TestException());

            var controller = new PersonController(manager.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => controller.Get(-1).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region Create person tests

        [Test]
        public void PersonController_Create_Successfull()
        {
            var manager = new Mock<IManager>();
            var testId = 2;
            var person = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonViewModel()
            {
                Id = -1,
                Age = person.Age.HasValue ? person.Age.Value : -1,
                FirstName = person.FirstName,
                LastName = person.LastName,
                AgeGroup = person.AgeGroup.Description
            };

            manager.Setup(x => x.AddOrUpdatePerson(
                It.Is<IPersonModel>(m => m.FirstName == personModel.FirstName
                        && m.LastName == personModel.LastName
                        && m.Age == personModel.Age)))
                .Returns(Task.FromResult(person));

            var controller = new PersonController(manager.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            var result = controller.Post(personModel).Result;

            PersonViewModel personViewModelResult;
            result.TryGetContentValue<PersonViewModel>(out personViewModelResult);

            CheckPerson(person, personViewModelResult);
        }

        [Test]
        public void PersonController_Create_ThrowsException()
        {
            var manager = new Mock<IManager>();
            var testId = 2;
            var person = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonViewModel()
            {
                Id = -1,
                Age = 67,
                FirstName = "FF1",
                LastName = "LL1"
            };

            manager.Setup(x => x.AddOrUpdatePerson(
                It.Is<IPersonModel>(m => m.FirstName == personModel.FirstName
                        && m.LastName == personModel.LastName
                        && m.Age == personModel.Age)))
                    .ThrowsAsync(new TestException());

            var controller = new PersonController(manager.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => controller.Post(personModel).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region Update person tests

        [Test]
        public void PersonController_Update_Successfull()
        {
            var manager = new Mock<IManager>();
            var testId = 2;
            var person = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonViewModel()
            {
                Id = -1,
                Age = person.Age.HasValue ? person.Age.Value : -1,
                FirstName = person.FirstName,
                LastName = person.LastName,
                AgeGroup = person.AgeGroup.Description
            };

            manager.Setup(x => x.AddOrUpdatePerson(
                It.Is<IPersonModel>(m => m.FirstName == personModel.FirstName
                        && m.LastName == personModel.LastName
                        && m.Age == personModel.Age)))
                .Returns(Task.FromResult(person));

            var controller = new PersonController(manager.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            var result = controller.Put(personModel).Result;

            PersonViewModel personViewModelResult;
            result.TryGetContentValue<PersonViewModel>(out personViewModelResult);

            CheckPerson(person, personViewModelResult);
        }

        [Test]
        public void PersonController_Update_ThrowsException()
        {
            var manager = new Mock<IManager>();
            var testId = 2;
            var person = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonViewModel()
            {
                Id = -1,
                Age = 67,
                FirstName = "FF1",
                LastName = "LL1"
            };

            manager.Setup(x => x.AddOrUpdatePerson(
                It.Is<IPersonModel>(m => m.FirstName == personModel.FirstName
                        && m.LastName == personModel.LastName
                        && m.Age == personModel.Age)))
                    .ThrowsAsync(new TestException());

            var controller = new PersonController(manager.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => controller.Put(personModel).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region utils

        private static void CheckPerson(IPersonModel personModel, PersonViewModel viewModel)
        {
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(personModel.Id, viewModel.Id);
            Assert.AreEqual(personModel.FirstName, viewModel.FirstName);
            Assert.AreEqual(personModel.LastName, viewModel.LastName);
            Assert.AreEqual(personModel.Age, viewModel.Age);
            Assert.AreEqual(personModel.AgeGroup.Description, viewModel.AgeGroup);
        }

        #endregion

    }
}
