using Domain;
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly Person _person;
        private readonly ISvcPersonService _svcpersonService;
        public RunGetInvites(Person person, ISvcPersonService svcpersonService)
        {
            _person = person;
            _svcpersonService = svcpersonService;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            //check if is null to return....
            if (_person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            var person = await _svcpersonService.GetAsync(_person.Id);

            //check if the person exists......
            if (person != null)
            {
                return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
            }
            else
                return await req.CreateResponse(HttpStatusCode.BadRequest, "Person not found!");
        }
    }
}
