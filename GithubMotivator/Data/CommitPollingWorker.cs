using GithubMotivator.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GithubMotivator.Services
{
    public class CommitPollingWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(5);

        public CommitPollingWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repositoryService = scope.ServiceProvider.GetRequiredService<IRepositoryService>();
                    await repositoryService.SyncAllRepositoriesAsync();
                }
                catch (Exception ex)
                {
                    // Log the error (can use ILogger if available)
                    Console.WriteLine($"Error in CommitPollingWorker: {ex.Message}");
                }
            }
        }
    }
}