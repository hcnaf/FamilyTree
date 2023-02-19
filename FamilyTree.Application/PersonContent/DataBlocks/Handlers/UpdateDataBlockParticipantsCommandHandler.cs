using FamilyTree.Application.Common.Exceptions;
using FamilyTree.Application.Common.Helpers;
using FamilyTree.Application.Common.Interfaces;
using FamilyTree.Application.PersonContent.DataBlocks.Commands;
using FamilyTree.Domain.Entities.PersonContent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyTree.Application.PersonContent.DataBlocks.Handlers
{
    public class UpdateDataBlockParticipantsCommandHandler : IRequestHandler<UpdateDataBlockParticipantsCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDataBlockParticipantsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateDataBlockParticipantsCommand request, CancellationToken cancellationToken)
        {
            var dataBlock = await _context.DataBlocks
                .Include(x => x.Participants)
                .SingleOrDefaultAsync(db => db.Id == request.BlockId, cancellationToken);

            if (dataBlock == null)
                throw new NotFoundException(nameof(DataBlock), request.BlockId);

            //if (dataBlock.DataCategory.PersonId != request.UserId)
            //    throw new InvalidOperationException("Participants can be edited only from original DataBlock.");

            CollectionsMerger.Merge(
                dataBlock.Participants,
                request.ParticipantIds,
                x => x.PersonId,
                x => x,
                add: (int personId) => dataBlock.Participants.Add(new PersonToDataBlocks
                {
                    PersonId = personId,
                    DataBlockId = dataBlock.Id,
                }),
                remove: (PersonToDataBlocks p2d) => dataBlock.Participants.Remove(p2d));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
