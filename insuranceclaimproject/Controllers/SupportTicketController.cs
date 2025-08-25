using insuranceclaimproject.Dtos.SupportTicket;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace insuranceclaimproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketService _supportTicketService;
        private readonly UserManager<AppUser> _userManager;

        public SupportTicketController(ISupportTicketService supportTicketService, UserManager<AppUser> userManager)
        {
            _supportTicketService = supportTicketService;
            _userManager = userManager;
        }

        // POST api/SupportTicket
        [HttpPost]
        [Authorize(Roles = "POLICYHOLDER, ADMIN, AGENT")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketDto createTicketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newTicket = await _supportTicketService.CreateTicketAsync(createTicketDto);
                return CreatedAtAction(
                    nameof(GetTicketById),
                    new { ticketId = newTicket.TicketId },
                    newTicket
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/SupportTicket/{ticketId}
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            var ticket = await _supportTicketService.GetTicketByIdAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(new { message = "Support ticket not found." });
            }

            var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRole = (await _userManager.FindByIdAsync(currentUserId)).Role;

            if (ticket.UserId != currentUserId && userRole != UserRole.ADMIN && userRole != UserRole.AGENT)
            {
                return Forbid();
            }

            var ticketDto = new SupportTicketDto
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                IssueDescription = ticket.IssueDescription,
                TicketStatus = ticket.TicketStatus.ToString(),
                CreatedDate = ticket.CreatedDate
            };
            return Ok(ticketDto);
        }

        // GET api/SupportTicket/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "ADMIN, AGENT,POLICYHOLDER")]
        public async Task<IActionResult> GetTicketsByUser(string userId)
        {
            var tickets = await _supportTicketService.GetTicketsByUserAsync(userId);
            if (!tickets.Any())
            {
                return NotFound(new { message = "No tickets found for this user." });
            }
            var ticketDtos = tickets.Select(t => new SupportTicketDto
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                IssueDescription = t.IssueDescription,
                TicketStatus = t.TicketStatus.ToString(),
                CreatedDate = t.CreatedDate
            });
            return Ok(ticketDtos);
        }

        // GET api/SupportTicket/all
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN, AGENT")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _supportTicketService.GetAllTicketsAsync();
            var ticketDtos = tickets.Select(t => new SupportTicketDto
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                IssueDescription = t.IssueDescription,
                TicketStatus = t.TicketStatus.ToString(),
                CreatedDate = t.CreatedDate
            });
            return Ok(ticketDtos);
        }

        // PUT api/SupportTicket/{ticketId}/status
        [HttpPut("{ticketId}/status")]
        [Authorize(Roles = "ADMIN, AGENT")]
        public async Task<IActionResult> UpdateTicketStatus(int ticketId, [FromBody] UpdateSupportTicketStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _supportTicketService.UpdateTicketStatusAsync(ticketId, statusDto.TicketStatus);
            if (!updated)
            {
                return NotFound(new { message = "Support ticket not found." });
            }
            return Ok(new { message = "Support ticket status updated successfully." });
        }
    }
}
