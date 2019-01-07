using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.DoughnutApi.Services;
using Samples.Shared;

namespace Samples.DoughnutApi.Controllers
{
    [Route("api/doughnut")]
    [ApiController]
    public class DoughnutController : ControllerBase
    {
        private readonly ILogger<DoughnutController> _logger;
        private readonly DoughnutService _doughnutService;

        public DoughnutController(ILogger<DoughnutController> logger)
        {
            _logger = logger;
            _doughnutService = new DoughnutService();
        }

        [HttpGet]
        public ActionResult<List<Doughnut>> Get()
        {
            _logger.LogInformation("Getting all doughnuts.");
            var doughnuts = _doughnutService.Get();
            return Ok(doughnuts);
        }

        [HttpGet("{id}")]
        public ActionResult<Doughnut> GetById(string id)
        {
            _logger.LogInformation("Getting doughnut by {id}.", id);
            var doughnuts = _doughnutService.GetById(id);
            return Ok(doughnuts);
        }

        [HttpPost]
        public ActionResult Create(Doughnut doughnut)
        {
            _doughnutService.Create(doughnut);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Update(Doughnut doughnut, string id)
        {
            _doughnutService.Update(doughnut, id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            _doughnutService.Delete(id);
            return NoContent();
        }

    }
}
