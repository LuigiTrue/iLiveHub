using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PolicyService.Infrastructure.Persistence;
using PolicyService.Application.Services;
using PolicyService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PolicyServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();