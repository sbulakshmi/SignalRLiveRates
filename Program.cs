using SignalRLiveRates.Hubs;
using SignalRLiveRates.Service;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});
builder.Services.AddSingleton<LiveRatesGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<LiveRatesHub>("/liveRatesHub");
// app.MapHub<LiveRatesServerHub>("/liveRatesHubServer");
// app.Use(async (context, next) =>
// {
//     var myHubContext = context.RequestServices
//                             .GetRequiredService<IHubContext<LiveRatesHub>>();

//     await LiveRatesGenerator((IHubContext)myHubContext);

//     await next.Invoke();
// }
var liveRatesGenerator = app.Services.GetRequiredService<LiveRatesGenerator>();
await liveRatesGenerator.InitLiveRates();
app.Run();

