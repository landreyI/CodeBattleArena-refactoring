using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LangProgrammingController : Controller
    {
        private readonly ILangProgrammingService _langProgrammingService;
        public LangProgrammingController(ILangProgrammingService langProgramming) 
        {
            _langProgrammingService = langProgramming;
        }

        [HttpGet("list-langs-programming")]
        public async Task<IActionResult> GetLangsProgramming(CancellationToken cancellationToken)
        {
            var langsProgramming = await _langProgrammingService.GetLangProgrammingListAsync(cancellationToken);
            return Ok(langsProgramming);
        }
    }
}
