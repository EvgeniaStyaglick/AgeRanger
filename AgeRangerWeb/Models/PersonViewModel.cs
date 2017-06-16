using AgeRanger.Interfaces.Logic;
using System.ComponentModel.DataAnnotations;

namespace AgeRangerWeb.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        { }

        public PersonViewModel(IPersonModel person)
        {
            Id = person.Id;
            FirstName = person.FirstName;
            LastName = person.LastName;
            Age = person.Age.HasValue ? person.Age.Value : -1;
            AgeGroup = person.AgeGroup?.Description;
        }

        public long Id { get; set; } = -1;

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        public long Age { get; set; }

        public string AgeGroup { get; set; }
    }
}