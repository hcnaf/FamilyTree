using FamilyTree.Domain.Entities.Tree;
using System;

namespace FamilyTree.Domain.Entities.PersonContent
{
    public class CommonEventToPersons
    {
        public Guid Id { get; set; }

        public Guid CommonEventId { get; set; }
        public CommonEvent CommonEvent { get; set; }

        public int ParticipantId { get; set; }
        public Person Participant { get; set; }
    }
}
