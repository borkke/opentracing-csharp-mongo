using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing.Tag;
using OpenTracing.Util;
using Samples.WebApp.Clients;
using Samples.WebApp.Models;

namespace Samples.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDoughnutClient _doughnutClient;
        private readonly IUserClient _userClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IDoughnutClient doughnutClient, IUserClient userClient, ILogger<HomeController> logger)
        {
            _doughnutClient = doughnutClient;
            _userClient = userClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var listOfDoughnuts = new List<DoughnutListViewModel>();

            var doughnuts = await _doughnutClient.GetAll();
            _logger.LogDebug("Returned list of {doughnut.count} doughnuts", doughnuts.Count);
            foreach (var doughnut in doughnuts)
            {
                var doughnutToAdd = new DoughnutListViewModel
                {
                    Id = doughnut.Id,
                    Color = doughnut.Color,
                    Price = doughnut.Price
                };

                var user = await _userClient.GetById(doughnut.OwnerId);
                doughnutToAdd.Owner = $"{user.FirstName} {user.LastName}";

                listOfDoughnuts.Add(doughnutToAdd);
            }

            return View(listOfDoughnuts);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
