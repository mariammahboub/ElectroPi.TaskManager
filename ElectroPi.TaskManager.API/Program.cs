using ElectroPi.TaskManager.API.Extensions;
using ElectroPi.TaskManager.Application;
using ElectroPi.TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration)
    .AddSwaggerWithVersioning();


var app = builder.Build();


await app.InitialiseDatabaseAsync();



app.UseGlobalExceptionMiddleware();   
app.UseCorrelationId();              
app.UseHttpsRedirection();         
app.UseCors("ElectroPiCorsPolicy"); 

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithVersioning();   
}

app.UseAuthentication();              
app.UseAuthorization();             


app.MapControllers();
app.MapHealthCheckEndpoint();        


app.Logger.LogInformation(
    "ElectroPi Task Manager API starting on {Environment}",
    app.Environment.EnvironmentName);

await app.RunAsync();

public partial class Program { }