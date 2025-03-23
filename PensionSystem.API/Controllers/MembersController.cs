using MediatR;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Application.Features.Queries;

namespace PensionSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Add Member")]
        public async Task<IActionResult> Register([FromBody] RegisterMemberCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { id }, id);
        }

        [HttpPut("Update Member{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var member = await _mediator.Send(command);
            return Ok(member);
        }

        [HttpDelete("Delete Member{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteMemberCommand { Id = id });
            return result ? NoContent() : NotFound();
        }

        [HttpGet("RetrieveMember{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var member = await _mediator.Send(new GetMemberQuery { Id = id });
            return Ok(member);
        }

    }
}
