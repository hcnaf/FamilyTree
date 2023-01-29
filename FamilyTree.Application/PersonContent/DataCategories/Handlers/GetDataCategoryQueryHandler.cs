using FamilyTree.Application.Common.Exceptions;
using FamilyTree.Application.Common.Interfaces;
using FamilyTree.Application.PersonContent.DataBlocks.ViewModels;
using FamilyTree.Application.PersonContent.DataCategories.Queries;
using FamilyTree.Application.PersonContent.DataCategories.ViewModels;
using FamilyTree.Application.PersonContent.DataHolders.ViewModels;
using FamilyTree.Application.Privacy.ViewModels;
using FamilyTree.Domain.Entities.PersonContent;
using FamilyTree.Domain.Entities.Tree;
using FamilyTree.Domain.Enums.PersonContent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyTree.Application.PersonContent.DataCategories.Handlers
{
    public class GetDataCategoryQueryHandler : IRequestHandler<GetDataCategoryQuery, DataCategoryVm>
    {
        private const string DataHolderPrivacyFiller = "#####################";

        private readonly IApplicationDbContext _context;

        private readonly IDateTimeService _dateTimeService;

        public GetDataCategoryQueryHandler(IApplicationDbContext context, IDateTimeService dateTimeService)
        {
            _context = context;
            _dateTimeService = dateTimeService;
        }

        public async Task<DataCategoryVm> Handle(GetDataCategoryQuery request, CancellationToken cancellationToken)
        {
            DataCategory dataCategory = await _context.DataCategories
                .Include(dc => dc.DataBlocks)
                    .ThenInclude(db => db.DataHolders)
                        .ThenInclude(dh => dh.Privacy)
                .Include(dc => dc.DataBlocks)
                    .ThenInclude(db => db.Participants)
                .SingleOrDefaultAsync(dc => dc.CreatedBy.Equals(request.UserId) &&
                                            dc.Id == request.DataCategoryId,
                                      cancellationToken);

            if (dataCategory == null)
                throw new NotFoundException(nameof(DataCategory), request.DataCategoryId);

            DataCategoryVm result = new DataCategoryVm()
            {
                Id = dataCategory.Id,
                Name = dataCategory.Name,
                DataCategoryType = dataCategory.DataCategoryType,
                IsDeletable = dataCategory.IsDeletable.Value
            };
            result.DataBlocks = new List<DataBlockDto>();

            List<DataBlock> dataBlocks = dataCategory.DataBlocks
                .OrderBy(db => db.OrderNumber)
                .ToList();

            foreach (DataBlock dataBlock in dataBlocks)
            {
                DataBlockDto dataBlockDto = new DataBlockDto()
                {
                    Id = dataBlock.Id,
                    Title = dataBlock.Title
                };
                dataBlockDto.DataHolders = new List<DataHolderDto>();
                dataBlockDto.Participants = new List<ParticipantVM>();

                List<DataHolder> dataHolders = dataBlock.DataHolders
                    .OrderBy(dh => dh.OrderNumber)
                    .ToList();

                foreach (DataHolder dataHolder in dataHolders)
                {
                    DataHolderDto dataHolderDto = new DataHolderDto()
                    {
                        Id = dataHolder.Id,
                        Title = dataHolder.Title,
                        DataHolderType = dataHolder.DataHolderType,
                        Data = dataHolder.Data,
                        IsDeletable = dataHolder.IsDeletable.Value,
                        Privacy = new PrivacyEntityDto()
                        {
                            Id = dataHolder.Privacy.Id,
                            BeginDate = dataHolder.Privacy.BeginDate,
                            EndDate = dataHolder.Privacy.EndDate,
                            IsAlways = dataHolder.Privacy.IsAlways.Value,
                            PrivacyLevel = dataHolder.Privacy.PrivacyLevel
                        }
                    };

                    if (!dataHolderDto.Privacy.IsAlways)
                    {
                        var nowTime = _dateTimeService.Now;

                        if (nowTime >= dataHolderDto.Privacy.BeginDate &&
                            nowTime <= dataHolderDto.Privacy.EndDate)
                        {
                            dataHolderDto.Data = DataHolderPrivacyFiller;
                        }
                    }

                    dataBlockDto.DataHolders.Add(dataHolderDto);
                }

                var people = dataBlock.Participants != null && dataBlock.Participants.Any()
                    ? await _context.People
                        .AsNoTracking()
                        .Include(x => x.DataCategories)
                            .ThenInclude(x => x.DataBlocks)
                                .ThenInclude(x => x.DataHolders)
                        .Where(x => dataBlock.Participants.Select(p => p.PersonId).Contains(x.Id))
                        .ToArrayAsync()
                    : Array.Empty<Person>();

                var participants = people
                    .Select(x => new ParticipantVM
                    {
                        Id = x.Id,
                        Name = x.DataCategories
                            ?.FirstOrDefault(dc => dc.DataCategoryType == DataCategoryType.PersonInfo).DataBlocks
                            ?.FirstOrDefault().DataHolders
                            ?.FirstOrDefault(x => x.DataHolderType == DataHolderType.Surname).Data,
                        IsSelected = true,
                    }).ToList();

                dataBlockDto.Participants = participants;

                result.DataBlocks.Add(dataBlockDto);
            }

            return result;
        }
    }
}
