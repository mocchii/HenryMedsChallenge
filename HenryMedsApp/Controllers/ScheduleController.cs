using HenryMedsApp.Interface;
using HenryMedsApp.Models;
using HenryMedsApp.Models.Exceptions;
using HenryMedsApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static HenryMedsApp.Models.Message;

namespace HenryMedsApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : Controller
    {
        private readonly HenryMedsContext _context;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(HenryMedsContext henryMedsContext, IScheduleService scheduleService) {
            _context = henryMedsContext;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Requires a valid 2024-06-30 Datestring
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Provider/{providerId}/{date}")]
        public async Task<ActionResult<List<ProviderSchedule>>> GetProviderAppointments(int providerId, DateTime date) {
            if (providerId <= 0)
            {
                return BadRequest("ProviderId must be greater than 0.");
            }
            var result = await _scheduleService.GetScheduleByProviderIdDate(providerId, date);
            if (result.Success == true)
            {
                return Ok(result);
            }
            else {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Accepts any providerId > 0 and if DateTimes end in  :00, :15, :30, :45
        /// </summary>
        /// <param name="id"></param>
        /// <param name="providerSchedule"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Provider/{providerId}/CreateSchedule")]
        public async Task<ActionResult<List<ProviderSchedule>>> CreateProviderSchedule(int providerId, ProviderSchedule providerSchedule)
        {
            //Validate providerId to be valid providerid here if we had a fully fleshed out database.
            if (providerId <= 0) {
                return BadRequest("ProviderId must be greater than 0.");
            }
            //Double checking that provider and object match. Ideally we would have a token to also check on.
            if (providerId != providerSchedule.ProviderId)
            {
                return BadRequest("ProviderId must match.");
            }
            if (providerSchedule.StartDate.Date != providerSchedule.EndDate.Date)
            {
                return BadRequest("Date must be on the same day.");
            }
            if (providerSchedule.Interval % 15 != 0)
            {
                return BadRequest("Intervals must be in 15 minute intervals.");
            }
            providerSchedule.StartDate = providerSchedule.StartDate.TruncateToMinute();
            providerSchedule.EndDate = providerSchedule.EndDate.TruncateToMinute();
            var results = new List<ValidationResult>();
            var context = new ValidationContext(providerSchedule);
            bool isValid = Validator.TryValidateObject(providerSchedule, context, results, true);
            if (isValid)
            {
                var result = await _scheduleService.CreateProviderSchedule(providerSchedule);
                if (result.Success == true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else {
                return BadRequest(String.Join(";", results));
            }
        }

        /// <summary>
        /// Accepts any clientId > 0 and if DateTimes end in  :00, :15, :30, :45
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="providerSchedule"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Client/{clientId}/Reserve")]
        public async Task<ActionResult<ClientBooking>> CreateClientReserve(int clientId, ClientBooking schedule)
        {
            /* Validate clientId to be valid clientId in our database.
             * Imagining a provider might want to have this time interval different. Maybe fetched from the database with some sort of changable variable.
             */
            var timeInterval = 15;
            if (clientId <= 0)
            {
                return BadRequest("ClientId must be greater than 0.");
            }
            //Double checking that clientid and object match. Ideally we would have a token to also check on.
            if (clientId != schedule.ClientId)
            {
                return BadRequest("ClientId must match.");
            }
            schedule.StartDate = schedule.StartDate.TruncateToMinute();
            schedule.EndDate = schedule.EndDate.TruncateToMinute();
            var testHours = schedule.StartDate - DateTime.UtcNow;
            if (testHours.TotalHours < 24)
            {
                return BadRequest("Reservations must be made 24 hours in advance.");
            }
            var testMinutes = schedule.EndDate - schedule.StartDate;
            if (testMinutes.TotalMinutes != timeInterval)
            {
                return BadRequest($"Reservations must be made in {timeInterval} minute intervals.");
            }
            var results = new List<ValidationResult>();
            var context = new ValidationContext(schedule);
            bool isValid = Validator.TryValidateObject(schedule, context, results, true);
            if (isValid)
            {
                var result = await _scheduleService.CreateClientBooking(schedule);
                if (result.Success == true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest(String.Join(";", results));
            }
        }

        /// <summary>
        /// Requires clientid and matching bookingId to update flag
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/Client/{clientId}/Confirm")]
        public async Task<ActionResult<bool>> ConfirmClientBooking(int clientId, int bookingId)
        {
            if (clientId <= 0)
            {
                return BadRequest("ClientId must be greater than 0.");
            }
            if (bookingId <= 0)
            {
                return BadRequest("BookingId must be greater than 0.");
            }
            var result = await _scheduleService.ConfirmClientBooking(clientId, bookingId);
            if (result.Success == true)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
