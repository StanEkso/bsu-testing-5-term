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
        Execution.SyntaxAnalyze.Analyzer analyzer = new (dto.AnalyzeCode);
        bool valid = analyzer.Parse();

        if (!valid)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            response.Content = new StringContent(analyzer.Error ?? "Unknown error");

            return BadRequest(analyzer.Error ?? "Unknown error");
        }

        return Ok("Ok");
    }
}