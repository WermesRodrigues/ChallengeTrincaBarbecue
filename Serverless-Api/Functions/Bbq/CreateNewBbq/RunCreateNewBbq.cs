using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services.Interfaces;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _person;
        private readonly ISvcBbqService _svcbbqService;

        public RunCreateNewBbq(ISvcBbqService svcbbqService, Person person)
        {
            _svcbbqService = svcbbqService;
            _person = person;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>() ?? null;

            //check if is null to return....
            if (input == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            //call a service to create a new Barbecue.....
            var response = await _svcbbqService.CreateNewBarbecue(input.Date, input.Reason, input.IsTrincasPaying);

            //if there is not wrong then return Created
            if (response.IsOk)
                return await req.CreateResponse(HttpStatusCode.Created, response.SnapshotObj);

            return await req.CreateResponse(HttpStatusCode.BadRequest, response.MessageEvent);
        }
    }
}
