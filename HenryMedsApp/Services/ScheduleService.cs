using HenryMedsApp.Interface;
using HenryMedsApp.Models;
using HenryMedsApp.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static HenryMedsApp.Models.Message;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HenryMedsApp.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HenryMedsContext _context;
        public ScheduleService(HenryMedsContext henryMedsContext)
        {
            _context = henryMedsContext;
        }

        public async Task<GenericItem<List<Appointments>>> GetScheduleByProviderIdDate(int providerId, DateTime date)
        {
            try {
                List<Appointments> openAppointments = new List<Appointments>();
                var providerResult = await _context.ProviderSchedules.Where(p => p.Active == true && p.ProviderId == providerId && p.StartDate.Date.Equals(date.Date)).ToListAsync();
                var clientResult = await _context.ClientBookings.Where(p => p.Active == true && p.ProviderId == providerId && p.StartDate.Date.Equals(date.Date)).ToListAsync();
                if (providerResult != null)
                {
                    foreach (var searchedResult in providerResult)
                    {
                        DateTime start = searchedResult.StartDate;
                        DateTime end = searchedResult.StartDate.AddMinutes(searchedResult.Interval);
                        while (end <= searchedResult.EndDate)
                        {
                            var newOpening = new Appointments(searchedResult.ProviderId, start, end);
                            if (!clientResult.Where(x => x.StartDate >= start && x.EndDate <= end).Any())
                            {
                                //This line is to just get rid of repeat provider schedulings. In reality we would probably have an update method or validation to prevent duplicates in the database.
                                if (!openAppointments.Contains(newOpening))
                                {
                                    openAppointments.Add(newOpening);
                                }
                            }
                            start = start.AddMinutes(searchedResult.Interval);
                            end = end.AddMinutes(searchedResult.Interval);
                        }
                    }
                }
                return new GenericItem<List<Appointments>>()
                {
                    Items = openAppointments,
                    Success = true,
                    Message = "Created Data"
                };
            }
            catch (Exception ex)
            {
                //Log exception here
                return new GenericItem<List<Appointments>>()
                {
                    Items = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericItem<ProviderSchedule>> CreateProviderSchedule(ProviderSchedule inputSchedule)
        {
            /* No time to write a validation here, but some sort of validation to ensure one date does not have over lapping times
             * Imagining a provider has open hours between x to y and y+1 to z where there is a deadzone gap inbetween.
             */
            try {
                if (inputSchedule.Interval == 0)
                {
                    //default 15
                    inputSchedule.Interval = 15;
                }
                var result = await _context.ProviderSchedules.AddAsync(inputSchedule);
                await _context.SaveChangesAsync();
                return new GenericItem<ProviderSchedule>()
                {
                    Items = result.Entity,
                    Success = true,
                    Message = "Created Data"
                };
            }
            catch (Exception ex)
            {
                //Log exception here
                return new GenericItem<ProviderSchedule>()
                {
                    Items = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericItem<ClientBooking>> CreateClientBooking(ClientBooking input)
        {
            try
            {
                var providerResult = await _context.ProviderSchedules.Where(p => p.Active == true && p.ProviderId == input.ProviderId && p.StartDate.Date.Equals(input.StartDate.Date)).ToListAsync();
                if (!providerResult.Any() || !providerResult.Where(s => s.StartDate <= input.StartDate && s.EndDate >= input.EndDate).Any()) {
                    throw new AppointmentException("Provider is unavailable.");
                }
                var clientResult = await _context.ClientBookings.Where(p => p.Active == true && p.ProviderId == input.ProviderId && p.StartDate.Date.Equals(input.StartDate.Date)).ToListAsync();
                if (clientResult.Where(s => s.StartDate >= input.StartDate && s.EndDate <= input.EndDate).Any())
                {
                    throw new AppointmentException("Time is unavailable.");
                }
                input.Active = true;
                input.CreateDate = DateTime.UtcNow;
                input.IsReserved = false;
                var result = await _context.ClientBookings.AddAsync(input);
                await _context.SaveChangesAsync();

                return new GenericItem<ClientBooking>() {
                    Items = result.Entity,
                    Success = true,
                    Message = "Created Data"
                };
            }
            catch (AppointmentException ex) {
                //Log exception here
                return new GenericItem<ClientBooking>()
                {
                    Items = null,
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                //Log exception here
                return new GenericItem<ClientBooking>()
                {
                    Items = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericItem<bool>> ConfirmClientBooking(int clientId, int bookingId)
        {
            try
            {
                var result = await _context.ClientBookings.Where(s => s.ClientId == clientId && s.BookingId == bookingId && s.Active == true && s.IsReserved == false).FirstOrDefaultAsync();
                if (result == null)
                {
                    throw new AppointmentException("No appointment found.");
                }
                else {
                    var timeDiff = result.CreateDate - DateTime.UtcNow;
                    if (timeDiff.TotalMinutes < 30)
                    {
                        result.IsReserved = true;
                    }
                    else {
                        result.Active = false;
                        await _context.SaveChangesAsync();
                        throw new AppointmentException("Appointment Expired.");
                    }
                }
                await _context.SaveChangesAsync();
                return new GenericItem<bool>()
                {
                    Items = true,
                    Success = true,
                    Message = "Updated Data"
                };
            }
            catch (Exception ex)
            {
                //Log exception here
                return new GenericItem<bool>()
                {
                    Items = false,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task ExpireClientSchedule()
        {
            try
            {
                var pastTime = DateTime.UtcNow.AddMinutes(-30);
                var result = await _context.ClientBookings.Where(s => s.Active == true && s.IsReserved == false && s.CreateDate < pastTime).ToListAsync();
                foreach(var timeCheck in result) { 
                    timeCheck.Active = false;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
                throw;
            }
        }
    }
}
