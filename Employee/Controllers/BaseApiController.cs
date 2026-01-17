using EMPLOYEE.Application.Common;
using EMPLOYEE.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMPLOYEE.Controllers
{
    namespace EMPLOYEE.Controllers
    {
        [ApiController]
        public abstract class BaseApiController : ControllerBase
        {
            protected IActionResult FromResult(Result result)
            {
                if (result.Success)
                    return Ok("Done");

                return BadRequest(result.Error);
            }

            protected IActionResult FromResult<T>(Result<T> result)
            {
                if (result.Success)
                    return Ok(result.Data);

                return BadRequest(result.Error);
            }
        }
    }

}
