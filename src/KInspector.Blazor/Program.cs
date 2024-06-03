using Autofac;
using Autofac.Extensions.DependencyInjection;

using KInspector.Blazor.Services;
using KInspector.Core;
using KInspector.Infrastructure;
using KInspector.Reports;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((containerBuilder) =>
    {
        containerBuilder.RegisterModule<CoreModule>();
        containerBuilder.RegisterModule<InfrastructureModule>();
        containerBuilder.RegisterModule<ReportsModule>();
        containerBuilder.RegisterModule<ActionsModule>();
    });

builder.Services.AddScoped<StateContainer>();

builder.Services.AddRazorPages().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddServerSideBlazor();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
