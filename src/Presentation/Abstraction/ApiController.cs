using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Abstraction;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected readonly ISender Sender = sender;
}
