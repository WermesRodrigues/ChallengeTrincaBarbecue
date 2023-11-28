
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _person;
        private readonly ISvcBbqService _svcbbqService;

        public RunAcceptInvite(Person person, ISvcBbqService svcbbqService)
        {
            _person = person;
            _svcbbqService = svcbbqService;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>() ?? null;

            //check if is null to return....
            if (answer == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input answer is required.");
            }

            //generic AcceptAndDeclineBarbecueInvite to accept and decline the invitations...
            var response = await _svcbbqService.AcceptAndDeclineBarbecueInvite(inviteId, _person.Id, answer.IsVeg, BarbecueInvitationType.Accept);

            if (response.IsOk)
                return await req.CreateResponse(System.Net.HttpStatusCode.OK, response.SnapshotObj);

            return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, response.MessageEvent);
        }
    }
}
