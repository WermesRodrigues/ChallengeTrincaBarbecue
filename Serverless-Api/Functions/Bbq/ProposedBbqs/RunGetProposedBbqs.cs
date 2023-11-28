using System.Net;
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _person;
        private readonly ISvcBbqService _svcbbqService;

        public RunGetProposedBbqs(Person person, ISvcBbqService svcbbqService)
        {
            _person = person;
            _svcbbqService = svcbbqService;
        }


        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            //check if is null to return....
            if (_person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            var response = await _svcbbqService.GetProposedBarbecues(_person.Id);

            if (response.IsOk)
                return await req.CreateResponse(HttpStatusCode.OK, response.SnapshotObj);

            return await req.CreateResponse(HttpStatusCode.BadRequest, response.MessageEvent);
        }
    }
}
