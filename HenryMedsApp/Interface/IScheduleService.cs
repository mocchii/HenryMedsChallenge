using HenryMedsApp.Models;
using static HenryMedsApp.Models.Message;

namespace HenryMedsApp.Interface
{
    public interface IScheduleService
    {
        public Task<GenericItem<List<Appointments>>> GetScheduleByProviderIdDate(int id, DateTime date);
        public Task<GenericItem<ProviderSchedule>> CreateProviderSchedule(ProviderSchedule input);
        public Task<GenericItem<ClientBooking>> CreateClientBooking(ClientBooking input);
        public Task<GenericItem<bool>> ConfirmClientBooking(int clientId, int bookingId);
        public Task ExpireClientSchedule();
    }
}
