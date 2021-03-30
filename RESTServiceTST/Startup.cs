using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace RESTServiceTST
{
	public sealed class Startup
	{
		private const string HealthCheckEndpoint = "/healthCheck";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		private IConfiguration Configuration { get; }

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
			services.AddHealthChecks();
			services.AddVersionedApiExplorer();
			services.AddApiVersioning(o =>
			{
				o.ReportApiVersions = true;
				o.ApiVersionReader = new UrlSegmentApiVersionReader();
				o.DefaultApiVersion = ApiVersion.Default;
			});
			services.AddMemoryCache();
			services.AddResponseCaching();
			services.AddControllers()
				.AddNewtonsoftJson(
					o =>
					{
						o.SerializerSettings.DateParseHandling = DateParseHandling.None;
						o.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
						o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
						o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
					}
				)
				.AddControllersAsServices();
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder =>
						builder.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowAnyOrigin());
			});

			services.AddSwaggerGen();
			return services.BuildServiceProvider();
		}

		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{

			var logger = loggerFactory.CreateLogger<Startup>();
			logger.LogInformation("Logger injected successfully");

			if (Configuration.GetValue<bool>("AllowCacheControlByClient"))
			{
				app.UseResponseCaching();
				logger.LogInformation("Using header based caching policy");
			}

			app.UseHealthChecks(HealthCheckEndpoint);
			logger.LogInformation($"Using health checks on endpoint: {HealthCheckEndpoint}");

			app.UseCors();
			logger.LogInformation("Using CORS: */*/*");

			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			app.UseSwagger();
			app.UseSwaggerUI();
		}
	}
}
