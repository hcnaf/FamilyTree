using FamilyTree.Application.Common.Exceptions;
using FamilyTree.Application.Common.Interfaces;
using FamilyTree.Application.Media.Audios.Queries;
using FamilyTree.Application.Media.Audios.ViewModels;
using FamilyTree.Domain.Entities.PersonContent;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using FamilyTree.Application.Media.Participants.Queries;
using Microsoft.EntityFrameworkCore;
using FamilyTree.Application.PersonContent.DataBlocks.ViewModels;
using FamilyTree.Domain.Entities.Tree;
using System.Linq;
using FamilyTree.Domain.Enums.PersonContent;

namespace FamilyTree.Application.Media.Participants.Handlers
{
    public class GetParticipantsQueryHandler : IRequestHandler<GetParticipantsQuery, List<ParticipantVM>>
    {
        private readonly IApplicationDbContext _context;

        public GetParticipantsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParticipantVM>> Handle(GetParticipantsQuery request, CancellationToken cancellationToken)
        {
            var participants = await _context.DataBlocks
                .AsNoTracking()
                .Where(db => db.Id == request.DataBlockId)
                .Select(x => x.Participants)
                .SingleOrDefaultAsync();

            var people = participants != null && participants.Any()
                    ? await _context.People
                        .AsNoTracking()
                        .Include(x => x.DataCategories)
                            .ThenInclude(x => x.DataBlocks)
                                .ThenInclude(x => x.DataHolders)
                        .Where(x => participants.Select(p => p.PersonId).Contains(x.Id))
                        .ToArrayAsync()
                    : Array.Empty<Person>();

            var result = people
                .Select(x => new ParticipantVM
                {
                    Id = x.Id,
                    Name = x.DataCategories
                        ?.FirstOrDefault(dc => dc.DataCategoryType == DataCategoryType.PersonInfo).DataBlocks
                        ?.FirstOrDefault().DataHolders
                        ?.FirstOrDefault(x => x.DataHolderType == DataHolderType.Surname).Data,
                    IsSelected = true,
                }).ToList();

            return result;
        }
    }
}
