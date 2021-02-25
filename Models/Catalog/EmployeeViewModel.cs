using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        //EmployeeIdentification
        public string IdentificationNo { get; set; }
        public string PersonalTaxCode { get; set; }
        //EmployeeIdentification

        //EmployeeBiography
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string EthnicGroup { get; set; }
        //EmployeeBiography

        //Address
        public string PermanentAddress { get; set; }
        public string TemporaryAddress { get; set; }
        //Address

        public string Email { get; set; }
        public string Phone { get; set; }

        public DateTime? HireDate { get; set; }

        public string Division { get; set; }
        public string Position { get; set; }

        public string SocialInsuranceNo { get; set; }
        public DateTime? SocialInsuranceDate { get; set; }
        public string HealthInsuranceNo { get; set; }
        public DateTime? HealthInsuranceDate { get; set; }

        public string AlternativePhone { get; set; }
        public string Note { get; set; }

        public bool Status { get; set; }
    }
}
