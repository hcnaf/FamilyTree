using MediatR;
using System.Collections.Generic;

namespace FamilyTree.Application.PersonContent.DataBlocks.Commands
{
    public class UpdateDataBlockParticipantsCommand : IRequest
    {
        public string UserId { get; set; }

        public int BlockId { get; set; }

        public ICollection<int> ParticipantIds { get; set; }
    }
}
