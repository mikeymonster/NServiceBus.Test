using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.Test.Application;
using NServiceBus.Test.Models;
using SFA.DAS.UnitOfWork;

namespace NServiceBus.Test.Controllers
{
    public class HomeController : Controller
    {
        private IUnitOfWorkContext _unitOfWorkContext;

        private readonly ISender _sender;

        public HomeController(ISender sender, IUnitOfWorkContext unitOfWorkContext)
        {
            _sender = sender;
            _unitOfWorkContext = unitOfWorkContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new MessageViewModel { Message = "Your message here" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MessageViewModel messageModel)
        {
            try
            {
                var message = messageModel.Message ?? "Default mesage";
                await _sender.Send(message);

                return View(messageModel);
            }
            catch
            {
                return View();
            }
        }
    }
}