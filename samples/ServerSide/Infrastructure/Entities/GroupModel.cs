using System;
using Microsoft.VisualBasic;

namespace ServerSide.Infrastructure.Entities
{
    public class GroupModel
    {
        public string GroupName { get; set; }
        public string PasswordsAssociatedTo { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
