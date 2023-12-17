using System.Net;
using API.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class AnalyzeController: ControllerBase
{
    [HttpPost(Name = "")]
    public ActionResult Analyze(AnalyzeDto dto)
    { 
        var value = Execution.Execution.Exec(dto.AnalyzeCode);
        if (value == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Invalid expression");
        
        var valueInt = value.Value.intValue;
        var responseMessage = "The result of the expression is: " + valueInt;
        
        return Ok(responseMessage);
    }
}