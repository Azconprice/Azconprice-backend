using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IClientService service) : Controller
    {
        private readonly IClientService _service = service;
    }
}
