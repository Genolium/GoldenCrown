using GoldenCrown.Attributes;
using GoldenCrown.DTOs;
using GoldenCrown.Features.Finance.Commands;
using GoldenCrown.Features.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [MyAuthorize]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var balance = await _mediator.Send(new GetBalanceQuery());
                return Ok(new BalanceResponse { Balance = balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "Счет успешно пополнен" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "Перевод выполнен успешно" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] GetHistoryQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}