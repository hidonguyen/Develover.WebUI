using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class EmployeeDependentViewModel
    {
        public Guid EmployeeId { get; set; }
        public Guid Id { get; set; }
        public int SequenceNo { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Relationship { get; set; }

        //EmployeeBiography
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public Guid? GenderId { get; set; }
        public string Gender { get; set; }
        public Guid? MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; }
        public Guid? EthnicGroupId { get; set; }
        public string EthnicGroup { get; set; }
        //EmployeeBiography

        //PermanentAddress
        public Guid? PermanentAddressId { get; set; }
        public string PermanentAddressLine1 { get; set; }
        public string PermanentAddressLine2 { get; set; }
        public string PermanentAddressCity { get; set; }
        public string PermanentAddressCountry { get; set; }
        public string PermanentAddressNote { get; set; }
        public string PermanentAddress { get; set; }
        //PermanentAddress

        //TemporaryAddress
        public Guid? TemporaryAddressId { get; set; }
        public string TemporaryAddressLine1 { get; set; }
        public string TemporaryAddressLine2 { get; set; }
        public string TemporaryAddressCity { get; set; }
        public string TemporaryAddressCountry { get; set; }
        public string TemporaryAddressNote { get; set; }
        public string TemporaryAddress { get; set; }
        //TemporaryAddress

        public string Email { get; set; }
        public string Phone { get; set; }
        public string AlternativePhone { get; set; }
        
        public string Note { get; set; }
    }
}
