using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Features.Commands;

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

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterMemberCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var member = await _mediator.Send(command);
            return Ok(member);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _mediator.Send(new SoftDeleteMemberCommand { Id = id });
            return result ? NoContent() : NotFound();
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var member = await _mediator.Send(new GetMemberQuery { Id = id });
        //    return Ok(member);
        //}

    }
}
