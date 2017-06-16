using AgeRanger.BusinessLogic;
using AgeRanger.BusinessLogic.Exceptions;
using AgeRanger.BusinessLogic.Models;
using AgeRanger.Data.DBModels;
using AgeRanger.Interfaces.Data;
using AgeRanger.Interfaces.Logic;
using AgeRanger.Tests.TestUtils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgeRanger.Tests.BusinessLogic
{
    [TestFixture]
    public class ManagerTests
    {
        #region Test data

        private static IEnumerable<IAgeGroupData> _groups = new List<AgeGroup>()
        {
             new AgeGroup() { Id = 1, MinAge = null, MaxAge = 10, Description = "TestGroup1" },
             new AgeGroup() { Id = 2, MinAge = 10, MaxAge = 100, Description = "TestGroup2" },
             new AgeGroup() { Id = 3, MinAge = 100, MaxAge = null, Description = "TestGroup3" }
        };

        private static IEnumerable<IAgeGroupData> _emptyGroups = new List<AgeGroup>();

        private static IEnumerable<IPersonData> _persons = new List<Person>()
        {
            new Person() { Id = 1, FirstName = "F1", LastName = "L1", Age = 0 },
            new Person() { Id = 2, FirstName = "F1", LastName = "L1", Age = 5 },
            new Person() { Id = 3, FirstName = "F1", LastName = "L1", Age = 10 },
            new Person() { Id = 4, FirstName = "F1", LastName = "L1", Age = 50 },
            new Person() { Id = 5, FirstName = "F1", LastName = "L1", Age = 100 },
            new Person() { Id = 6, FirstName = "F1", LastName = "L1", Age = 555 },
            new Person() { Id = 7, FirstName = "F7", LastName = "L7", Age = null }
        };

        private static IEnumerable<IPersonData> _emptyPersons = new List<Person>();

        #endregion

        #region Person list tests

        [Test]
        public void Manager_PersonsList_Successfull()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPersons()).Returns(Task.FromResult(_persons));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var result = manager.GetPersonList().Result;

            var sourceList = _persons.ToList();
            var resultList = result.ToList();

            Assert.AreEqual(sourceList.Count(), resultList.Count());
            CheckPerson(sourceList[0], resultList[0], 1);
            CheckPerson(sourceList[1], resultList[1], 1);
            CheckPerson(sourceList[2], resultList[2], 2);
            CheckPerson(sourceList[3], resultList[3], 2);
            CheckPerson(sourceList[4], resultList[4], 3);
            CheckPerson(sourceList[5], resultList[5], 3);
            // We don't allow null for Age in UI, but if it appear somehow in DB 
            //it should be interpreted as group with minimum age as null
            CheckPerson(sourceList[6], resultList[6], 1);
        }

        [Test]
        public void Manager_PersonsList_GroupsEmpty_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_emptyGroups));
            personRepo.Setup(x => x.GetPersons()).Returns(Task.FromResult(_persons));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonList().Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(AgeGroupsEmptyException)) as AgeGroupsEmptyException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_PersonsList_Empty_Successfull()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPersons()).Returns(Task.FromResult(_emptyPersons));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var result = manager.GetPersonList().Result;

            Assert.AreEqual(0, result.ToList().Count());
        }

        [Test]
        public void Manager_PersonsList_GroupsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).ThrowsAsync(new TestException());
            personRepo.Setup(x => x.GetPersons()).Returns(Task.FromResult(_persons));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonList().Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_PersonsList_PersonsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPersons()).ThrowsAsync(new TestException());

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonList().Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region Get person by Id tests

        [Test]
        public void Manager_PersonById_Successfull()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPerson(testId)).Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var result = manager.GetPersonById(testId).Result;

            CheckPerson(personData, result, 1);
        }

        [Test]
        public void Manager_PersonById_EmptyGroups_ThrowsEsception()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_emptyGroups));
            personRepo.Setup(x => x.GetPerson(testId)).Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonById(testId).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(AgeGroupsEmptyException)) as AgeGroupsEmptyException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_PersonById_NotFound_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = -1;
            IPersonData personData = null;

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPerson(testId)).Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonById(testId).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(PersonNotFoundException)) as PersonNotFoundException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_PersonsById_GroupsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);

            groupsRepo.Setup(x => x.GetAgeGroups()).ThrowsAsync(new TestException());
            personRepo.Setup(x => x.GetPerson(testId)).Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonById(testId).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_PersonsById_PersonsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.GetPerson(testId)).ThrowsAsync(new TestException());

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(() => manager.GetPersonById(testId).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region Get person by Id tests

        [Test]
        public void Manager_AddOrUpdate_Edit_Successfull()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonModel()
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName,
                Age = personData.Age,
                Id = personData.Id
            };

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.AddOrUpdate(personModel.Id,
                                        personModel.FirstName, 
                                        personModel.LastName,
                                        personModel.Age 
                                        ))
                .Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var result = manager.AddOrUpdatePerson(personModel).Result;

            CheckPerson(personData, result, 1);
        }

        [Test]
        public void Manager_AddOrUpdate_Add_Successfull()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonModel()
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName,
                Age = personData.Age,
                Id = -1
            };

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_groups));
            personRepo.Setup(x => x.AddOrUpdate(-1,
                                        personModel.FirstName,
                                        personModel.LastName,
                                        personModel.Age
                                        ))
                .Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var result = manager.AddOrUpdatePerson(personModel).Result;

            CheckPerson(personData, result, 1);
        }

        [Test]
        public void Manager_AddOrUpdate_EmptyGroups_ThrowsEsception()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).Returns(Task.FromResult(_emptyGroups));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => manager.AddOrUpdatePerson(new PersonModel()).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() 
                    == typeof(AgeGroupsEmptyException)) as AgeGroupsEmptyException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_AddOrUpdate_GroupsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();

            groupsRepo.Setup(x => x.GetAgeGroups()).ThrowsAsync(new TestException());

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => manager.AddOrUpdatePerson(new PersonModel()).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        [Test]
        public void Manager_AddOrUpdate_PersonsRepoException_ThrowsException()
        {
            var groupsRepo = new Mock<IAgeGroupRepository>();
            var personRepo = new Mock<IPersonRepository>();
            var testId = 2;
            var personData = _persons.FirstOrDefault(p => p.Id == testId);
            var personModel = new PersonModel()
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName,
                Age = personData.Age,
                Id = -1
            };

            groupsRepo.Setup(x => x.GetAgeGroups()).ThrowsAsync(new TestException());
            personRepo.Setup(x => x.AddOrUpdate(personModel.Id,
                            personModel.FirstName,
                            personModel.LastName,
                            personModel.Age
                            ))
                .Returns(Task.FromResult(personData));

            var manager = new Manager(groupsRepo.Object, personRepo.Object);

            var aggregateException = Assert.Throws<AggregateException>(
                () => manager.AddOrUpdatePerson(personModel).Wait());
            var resException = aggregateException.InnerExceptions
                .FirstOrDefault(x => x.GetType() == typeof(TestException)) as TestException;

            Assert.That(resException, Is.Not.Null);
        }

        #endregion

        #region utils

        private static void CheckPerson(IPersonData personData, IPersonModel personModel, long expectedGroupId)
        {
            Assert.IsNotNull(personModel);
            Assert.AreEqual(personData.Id, personModel.Id);
            Assert.AreEqual(personData.FirstName, personModel.FirstName);
            Assert.AreEqual(personData.LastName, personModel.LastName);
            Assert.AreEqual(personData.Age, personModel.Age);
            Assert.IsNotNull(personModel.AgeGroup);
            Assert.AreEqual(expectedGroupId, personModel.AgeGroup.Id);
        }

        #endregion
    }
}
