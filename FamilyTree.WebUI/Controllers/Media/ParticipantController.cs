using FamilyTree.Application.Common.Interfaces;
using FamilyTree.Application.Copying.Commands;
using FamilyTree.Application.Media.Audios.Commands;
using FamilyTree.Application.Media.Audios.Queries;
using FamilyTree.Application.Media.Audios.ViewModels;
using FamilyTree.Application.Media.Participants.Queries;
using FamilyTree.Application.PersonContent.DataBlocks.ViewModels;
using FamilyTree.WebUI.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyTree.WebUI.Controllers.Media
{
    public class ParticipantController : ApiControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public ParticipantController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ParticipantVM>>> GetAll(int dataBlockId)
        {
            var a = await Mediator.Send(new GetParticipantsQuery()
            {
                DataBlockId = dataBlockId
            });
            return a;
        }
    }
}
