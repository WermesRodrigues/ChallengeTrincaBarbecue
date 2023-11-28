using Domain.Entities;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Domain.Services.Interfaces;
using System.Net;

namespace Serverless_Api.Functions.Bbq.GetBbqCart
{
    public partial class RunGetBbqShopCart
    {
        private readonly Person _person;
        private readonly ISvcBbqService _svcbbqService;

        public RunGetBbqShopCart(Person person, ISvcBbqService svcbbqService)
        {
            _person = person;
            _svcbbqService = svcbbqService;
        }

        [Function(nameof(RunGetBbqShopCart))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{bbqId}/cart")] HttpRequestData req, string bbqId)
        {
            //check if is null to return....
            if (_person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            var response = await _svcbbqService.GetBarbecueCart(_person.Id, bbqId);

            if (response.IsOk)
                return await req.CreateResponse(HttpStatusCode.Created, response.SnapshotObj);

            return await req.CreateResponse(HttpStatusCode.BadRequest, response.MessageEvent);
        }
    }
}
