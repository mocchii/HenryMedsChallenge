using HenryMedsApp.Models;
using HenryMedsApp.Services;
using Moq;
using Moq.EntityFrameworkCore;

namespace HenryMedsTest
{
    public class ScheduleServiceTest
    {
        ScheduleService _scheduleService;
        Mock<HenryMedsContext> _henryMedsContextMock;
        public ScheduleServiceTest() {
            _henryMedsContextMock = new Mock<HenryMedsContext>();
            _scheduleService = new ScheduleService(_henryMedsContextMock.Object);
        }
        [Fact]
        public void TestOpenAppointments()
        {
            var startDate = new DateTime(2024, 6, 28).AddHours(8);
            var endDate = new DateTime(2024, 6, 28).AddHours(10);
            var startClientDate = new DateTime(2024, 6, 28).AddHours(8).AddMinutes(15);
            var endClientDate = new DateTime(2024, 6, 28).AddHours(8).AddMinutes(30);
            var entitiesProvider = new List<ProviderSchedule>() { new ProviderSchedule() { ProviderId = 1, Active = true, StartDate = startDate, EndDate = endDate } };
            var entitiesClient = new List<ClientBooking>() { new ClientBooking() { ProviderId = 1, ClientId = 1, Active = true, StartDate = startClientDate, EndDate = endClientDate } };
            _henryMedsContextMock.Setup(x => x.ProviderSchedules).ReturnsDbSet(entitiesProvider);
            _henryMedsContextMock.Setup(x => x.ClientBookings).ReturnsDbSet(entitiesClient);
            var result = _scheduleService.GetScheduleByProviderIdDate(1, startDate).GetAwaiter().GetResult();
            Assert.True(result.Success);
            Assert.Equal(7, result.Items.Count());
        }
    }
}