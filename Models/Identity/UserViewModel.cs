using System;

namespace Develover.WebUI.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Initial { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string SelectedRoles { get; set; }
        public string SelectedBranches { get; set; }

        public string Note { get; set; }
        public bool Status { get; set; }

    }
}
