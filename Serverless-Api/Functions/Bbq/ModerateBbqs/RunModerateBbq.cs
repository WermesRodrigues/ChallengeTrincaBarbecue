
using Domain.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunModerateBbq
    {
        private readonly ISvcBbqService _svcbbqService;

        public RunModerateBbq(ISvcBbqService svcbbqService)
        {
            _svcbbqService = svcbbqService;
        }


        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var moderationRequest = await req.Body<ModerateBbqRequest>() ?? null;

            //check if is null to return....
            if (moderationRequest == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            //call a service to invite Moderates for Barbecue.....
            var response = await _svcbbqService.ModerateBarbecue(id, moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay);

            //if there is not wrong then return Created
            if (response.IsOk)
                return await req.CreateResponse(System.Net.HttpStatusCode.OK, response.SnapshotObj);

            return await req.CreateResponse(System.Net.HttpStatusCode.BadRequest, response.MessageEvent);
        }
    }
}
