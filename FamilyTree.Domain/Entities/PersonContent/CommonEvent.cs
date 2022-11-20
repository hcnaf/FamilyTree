using FamilyTree.Domain.Entities.Tree;
using System;
using System.Collections.Generic;

namespace FamilyTree.Domain.Entities.PersonContent
{
    public class CommonEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        
        public ICollection<CommonEventToPersons> CommonEventToPersons { get; set; }
    }
}
