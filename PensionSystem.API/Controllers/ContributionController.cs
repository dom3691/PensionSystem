using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Infrastructure.ExceptionHandler;

namespace PensionSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContributionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create Contribution")]
        public async Task<IActionResult> Create([FromBody] CreateContributionCommand command)
        {
           // command.IsVoluntary = true;
            try
            {
                var contribution = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetContributionSummary), new { memberId = command.MemberId }, contribution);
            }
            catch (BusinessException ex)
            {
                // Return a 400 Bad Request if business rules are violated
                return BadRequest(ex.Message);
            }

            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }

        }

        [HttpPut("monthly/{id}")]
        public async Task<IActionResult> UpdateMonthlyContribution(Guid id, [FromBody] UpdateContributionCommand command)
        {
            if (id != command.Id)
                return BadRequest("Contribution ID mismatch.");

            command.IsVoluntary = false;  // Ensure it's a monthly contribution
            var contribution = await _mediator.Send(command);
            return Ok(contribution);
        }


        [HttpDelete("monthly/{id}")]
        public async Task<IActionResult> SoftDeleteMonthlyContribution(Guid id)
        {
            var result = await _mediator.Send(new 
                SoftDeleteContributionCommand { Id = id });
            return result ? NoContent() : NotFound();
        }


        [HttpPost("voluntary")]
        public async Task<IActionResult> AddVoluntaryContribution([FromBody] CreateContributionCommand command)
        {
            command.IsVoluntary = true;  // Ensure it's a voluntary contribution
            var contribution = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetVoluntaryContribution), new { id = contribution.Id }, contribution);
        }


        [HttpPut("voluntary/{id}")]
        public async Task<IActionResult> UpdateVoluntaryContribution(Guid id, [FromBody] UpdateContributionCommand command)
        {
            if (id != command.Id)
                return BadRequest("Contribution ID mismatch.");

            command.IsVoluntary = true;  // Ensure it's a voluntary contribution
            var contribution = await _mediator.Send(command);
            return Ok(contribution);
        }

        [HttpGet("voluntary/{id}")]
        public async Task<IActionResult> GetVoluntaryContribution(Guid id)
        {
            var contribution = await _mediator.Send(new GetContributionQuery { Id = id });
            return Ok(contribution);
        }

        [HttpGet("voluntary/member/{memberId}")]
        public async Task<IActionResult> GetVoluntaryContributionsByMember(Guid memberId)
        {
            var contributions = await _mediator.Send(new GetContributionsByMemberQuery { MemberId = memberId, IsVoluntary = true });
            return Ok(contributions);
        }

        [HttpDelete("voluntary/{id}")]
        public async Task<IActionResult> SoftDeleteVoluntaryContribution(Guid id)
        {
            var result = await _mediator.Send(new SoftDeleteContributionCommand { Id = id });
            return result ? NoContent() : NotFound();
        }

        [HttpGet("summary/{memberId}")]
        public async Task<IActionResult> GetContributionSummary(Guid memberId)
        {
            var summary = await _mediator.Send(new GetContributionSummaryQuery { MemberId = memberId });
            return Ok(summary);
        }

        [HttpGet("statement/{memberId}")]
        public async Task<IActionResult> GetContributionStatement(Guid memberId)
        {
            var statement = await _mediator.Send(new GenerateContributionStatementQuery { MemberId = memberId });
            return Ok(statement);
        }

        [HttpGet("eligibility/{memberId}")]
        public async Task<IActionResult> CheckEligibilityForBenefits(Guid memberId)
        {
            var eligibility = await _mediator.Send(new CheckEligibilityQuery { MemberId = memberId });
            return Ok(eligibility);
        }
    }
}
