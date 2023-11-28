

using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using static Serverless_Api.RunAcceptInvite;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _person;
        private readonly ISvcBbqService _svcbbqService;

        public RunDeclineInvite(Person person, ISvcBbqService svcbbqService)
        {
            _person = person;
            _svcbbqService = svcbbqService;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>() ?? null;

            //check if is null to return....
            if (answer == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input answer is required.");
            }

            //generic AcceptAndDeclineBarbecueInvite to accept and decline the invitations...
            var response = await _svcbbqService.AcceptAndDeclineBarbecueInvite(inviteId, _person.Id, answer.IsVeg, BarbecueInvitationType.Decline);

            if (response.IsOk)
                return await req.CreateResponse(System.Net.HttpStatusCode.OK, response.SnapshotObj);

            return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, response.SnapshotObj);
        }
    }
}
