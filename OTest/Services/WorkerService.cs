using OTest.Services;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly ReqResService _reqResService;
    private readonly MongoDbService _mongoDbService;
    private Timer _getUsersTimer;
    private Timer _createUserTimer;

    public WorkerService(ILogger<WorkerService> logger, ReqResService reqResService, MongoDbService mongoDbService)
    {
        _logger = logger;
        _reqResService = reqResService;
        _mongoDbService = mongoDbService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _getUsersTimer = new Timer(async _ => await PerformGetUsers(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        _createUserTimer = new Timer(async _ => await PerformCreateUser(), null, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
        return Task.CompletedTask;
    }

    private async Task PerformGetUsers()
    {
        _logger.LogInformation("Performing GET users operation...");
        try
        {
            var usersJson = await _reqResService.GetUsers();
            await _mongoDbService.StoreUsers(usersJson);
            _logger.LogInformation("Stored users at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while performing GET users.");
        }
    }

    private async Task PerformCreateUser()
    {
        _logger.LogInformation("Performing POST create user operation...");
        try
        {
            //var UserJson = await _reqResService.CreateUser("User_" + Guid.NewGuid().ToString(), "Developer");
            //var name = $"User_{new Random().Next(1000, 9999)}";
            //var job = "Developer";
            //var userJson = await _reqResService.CreateUser(name, job);

            var randomPart = new Random().Next(1000, 9999);
            var guidPart = Guid.NewGuid().ToString().Substring(0, 8); 
            var name = $"User_{randomPart}_{guidPart}";
            var job = "Developer";
            var UserJson = await _reqResService.CreateUser(name, job);

            await _mongoDbService.StoreUser(UserJson);
            _logger.LogInformation("Stored a new user at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while performing POST create user.");
        }
    }

    public override void Dispose()
    {
        _getUsersTimer?.Dispose();
        _createUserTimer?.Dispose();
        base.Dispose();
    }
}

//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using OTest.Services;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//public class WorkerService : BackgroundService
//{
//    private readonly ILogger<WorkerService> _logger;
//    private readonly ReqResService _reqResService;
//    private readonly MongoDbService _mongoDbService;
//    private readonly Random _random;
//    private DateTime _lastGetUsersTime;
//    private DateTime _lastCreateUserTime;

//    public WorkerService(ILogger<WorkerService> logger, ReqResService reqResService, MongoDbService mongoDbService)
//    {
//        _logger = logger;
//        _reqResService = reqResService;
//        _mongoDbService = mongoDbService;
//        _random = new Random();
//        _lastGetUsersTime = DateTime.UtcNow;
//        _lastCreateUserTime = DateTime.UtcNow;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Worker service started.");

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            var currentTime = DateTime.UtcNow;

//            if ((currentTime - _lastGetUsersTime).TotalMinutes >= 5)
//            {
//                _logger.LogInformation("Performing GET users operation...");
//                await PerformGetUsers();
//                _lastGetUsersTime = currentTime;
//            }

//            if ((currentTime - _lastCreateUserTime).TotalMinutes >= 10)
//            {
//                _logger.LogInformation("Performing POST create user operation...");
//                await PerformCreateUser();
//                _lastCreateUserTime = currentTime;
//            }

//            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
//        }

//        _logger.LogInformation("Worker service stopping.");
//    }

//    private async Task PerformGetUsers()
//    {
//        try
//        {
//            var usersJson = await _reqResService.GetUsers();
//            await _mongoDbService.StoreUsers(usersJson);
//            _logger.LogInformation("Stored users at: {time}", DateTimeOffset.Now);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "An error occurred while performing GET users.");
//        }
//    }

//    private async Task PerformCreateUser()
//    {
//        try
//        {
//            var name = $"User_{_random.Next(1000, 9999)}";
//            var job = "Developer";
//            var userJson = await _reqResService.CreateUser(name, job);
//            await _mongoDbService.StoreUser(userJson);
//            _logger.LogInformation("Stored a new user at: {time}", DateTimeOffset.Now);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "An error occurred while performing POST create user.");
//        }}
//        //public async Task LoginAndStoreToken()
//        //{
//        //    try
//        //    {
//        //        string email = "example@example.com"; 
//        //        string password = "yourpassword"; 
//        //        var tokenJson = await _reqResService.LoginUser(email, password);
//        //        await _mongoDbService.StoreToken(tokenJson);
//        //        _logger.LogInformation("Stored login token at: {time}", DateTimeOffset.Now);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "An error occurred while performing login and storing token");
//        //    }
//        //}

//}
