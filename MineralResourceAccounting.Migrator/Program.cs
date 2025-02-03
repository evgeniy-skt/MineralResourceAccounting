using (var serviceProvider = Config.CreateServices())
using (var scope = serviceProvider.CreateScope())
{
   Config.UpdateDatabase(scope.ServiceProvider);
}