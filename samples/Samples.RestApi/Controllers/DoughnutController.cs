using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using OpenTracing.Util;
using Samples.RestApi.Services;
using Samples.Shared;

namespace Samples.RestApi.Controllers
{
    [Route("api/doughnut")]
    [ApiController]
    public class DoughnutController : ControllerBase
    {
        private readonly DoughnutService _doughnutService;
        private readonly ITracer _tracer;

        public DoughnutController()
        {
            _doughnutService = new DoughnutService();
            _tracer = GlobalTracer.Instance;
        }

        [HttpGet]
        public ActionResult<List<Doughnut>> Get()
        {
            var doughnuts = _doughnutService.Get();
            return Ok(doughnuts);
        }

        [HttpGet("{id}")]
        public ActionResult<Doughnut> GetById(string id)
        {
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
