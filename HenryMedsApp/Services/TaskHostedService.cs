using HenryMedsApp.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace HenryMedsApp.Services
{
    public class TaskHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduledTaskService = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                        await scheduledTaskService.ExpireClientSchedule();
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            });
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            return base.StopAsync(stoppingToken);
        }
    }

}
