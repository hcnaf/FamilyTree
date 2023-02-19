using Dapper;
using FamilyTree.Application.Common.Interfaces;
using FamilyTree.Application.Copying.Commands;
using FamilyTree.Application.FamilyTrees.ViewModels;
using FamilyTree.Application.Media.Audios.Commands;
using FamilyTree.Application.Media.Audios.Queries;
using FamilyTree.Application.Media.Audios.ViewModels;
using FamilyTree.Application.Media.Participants.Queries;
using FamilyTree.Application.People.ViewModels;
using FamilyTree.Application.PersonContent.DataBlocks.ViewModels;
using FamilyTree.WebUI.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTree.WebUI.Controllers.Media
{
    [Authorize]
    [Route("Media/[controller]/[action]/{id?}")]
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

            ViewBag.Participants = a;

            return a;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartModel>>> GetAll2()
        {
           
            string connectionString = "Server=COMPUTER\\SQLEXPRESS;Database=FamilyTreePzkv22;Trusted_Connection=True;MultipleActiveResultSets=True";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var a = db.Query<PartModel>("SELECT DataHolderType, Data, IsDeletable, DataBlockId FROM DataHolder").ToList();

                var f = a.FindAll(p=>p.DataBlockId == 8);
                //ViewBag.Pers = f;
                return f;
            }

        }
    }
}
