
using Microsoft.OpenApi.Models;
using OTest.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient<ReqResService>();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddHostedService<WorkerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTest API", Version = "v1" });
}


);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTest API V1");
    });
}   

else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTest API V1");
   //בשביל להביא מחרוזת ריקה
        c.RoutePrefix = string.Empty;
    });
}
 app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();
//Console.WriteLine("Application started on port: " + builder.WebHost.GetSetting("urls"));