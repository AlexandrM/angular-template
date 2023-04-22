namespace AngularTemplate.Web;

using System.IO;
using System.Linq;
using Core.Extensions;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Identity;
using Identity.Domain;
using Identity.ExternalProviders;
using Identity.Repositories;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Core.Identity;
using Data.Repositories.Users;
using DbContext;
using Services.Users;
using Shared;
using Identity.Configure;
using Infrastructure;
using Microsoft.Extensions.Logging.Console;
using IdentityServer4.Services;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		AddServices(builder);

		var app = builder.Build();
		Configure(app);

		app.Run();
	}

	private static void AddServices(WebApplicationBuilder builder)
	{
		builder.Services.AddLogging(logging =>
			logging.AddSimpleConsole(options =>
			{
				//options.SingleLine = true;
				options.TimestampFormat = "[HH:mm:ss] ";
				options.ColorBehavior = LoggerColorBehavior.Enabled;
			})
		);
		//File Logger
		builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

		// shared configuration
		builder.Configuration.AddConfiguration(SharedConfiguration.CreateConfigurationContainer());

		// Database
		var databaseProvider = builder.Configuration["DatabaseProvider"];
		if (databaseProvider == null)
			throw new NullReferenceException($"Configuration DatabaseProvider {databaseProvider}");

		var connectionString = builder.Configuration.GetConnectionString(databaseProvider);
		if (connectionString == null)
			throw new NullReferenceException($"Configuration ConnectionString {databaseProvider}");

		var connectionConfiguration = new ConnectionConfiguration(databaseProvider, connectionString);
		RepositoryInitializer.Initialize(builder.Services, connectionConfiguration);

		// Configurations
		builder.Services.Configure<ExternalProvidersConfig>(
			builder.Configuration.GetSection("ExternalProviders")
		);

		InitializeGlobalSettings(builder);

		AddIdentityServices(builder);

		AddRepositories(builder);

		AddBusinessServices(builder);

		builder.Services
			.AddControllers()
			.AddJsonOptions(options =>
			{
				options.AllowInputFormatterExceptionMessages = builder.Environment.IsDevelopment();
			});

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		//builder.Services.AddEndpointsApiExplorer();
		//builder.Services.AddSwaggerGen();

		builder.Services.AddCors();
	}

	private static void InitializeGlobalSettings(WebApplicationBuilder builder)
	{
		builder.Services.Configure<GlobalSettings>(_ => { });
		builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GlobalSettings>>().Value);
	}

	private static void AddIdentityServices(WebApplicationBuilder builder)
	{
		// add identity
		builder.Services
			.AddIdentity<ApplicationUser<Guid>, ApplicationRole>()
			.AddUserStore<UserStore>()
			.AddRoleStore<RoleStore>()
			.AddUserManager<AppUserManager>()
			.AddDefaultTokenProviders();

		builder.Services.AddScoped<ICorsPolicyService, CorsPolicyService>();
		builder.Services.AddScoped<IPasswordHasher<ApplicationUser<Guid>>, PasswordHasher>();

		// Identity Services
		builder.Services.AddScoped<IUserStore<ApplicationUser<Guid>>, UserStore>();
		builder.Services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();

		// External providers
		builder.Services.AddScoped<IExternalProviderValidator, ExternalProviderValidatorAuth0>();
		builder.Services.AddScoped<IExternalProviderValidator, ExternalProviderValidatorGoogle>();

		// Configure Identity options and password complexity here
		builder.Services.Configure<IdentityOptions>(options =>
		{
			// User settings
			options.User.RequireUniqueEmail = true;

			// Password settings
			options.Password.RequireDigit = true;
			options.Password.RequiredLength = 8;
			options.Password.RequireNonAlphanumeric = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireLowercase = true;

			// Lockout settings
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
			options.Lockout.MaxFailedAccessAttempts = 10;
		});

		// Adds IdentityServer.
		builder.Services
			.AddIdentityServer(o =>
			{
				o.UserInteraction = new UserInteractionOptions()
				{
					LoginUrl = "/authentication/signin",
					LoginReturnUrlParameter = "returnUrl"
				};
			})
			.AddSigningCredential(CreateSigningCredential())
			.AddPersistedGrantStore<PersistedGrantStore>()
			.AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
			.AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
			.AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
			.AddInMemoryClients(IdentityServerConfig.GetClients())
			.AddAspNetIdentity<ApplicationUser<Guid>>()
			.AddProfileService<ProfileService>()
			.AddExtensionGrantValidator<ExtensionGrantValidator>();

		var authentication = builder.Services
			.AddAuthentication(options =>
			{
				options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
				options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
			});

		var frontEndHost = builder.Configuration.GetValue<string>("FrontEnd:Host");
		var isDynamicFrontEndHost = frontEndHost.IsMissing();
		if (isDynamicFrontEndHost)
		{
			builder.Services.ConfigureOptions<ConfigureIdentityServerOptions>();
			authentication.AddIdentityServerAuthentication();
		}
		else
		{
			authentication.AddIdentityServerAuthentication(options =>
			{
				options.RequireHttpsMetadata = false;
				options.ApiName = IdentityServerConfig.ApiName;
				options.Authority = frontEndHost;
			});
		}
	}

	private static void AddRepositories(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<IUserExternalRepository, UserExternalRepository>();
	}

	private static void AddBusinessServices(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<UserService, UserService>();
	}

	private static readonly ConcurrentDictionary<string, PhysicalFileInfo> _staticFilesCache = new();

	private static bool _firstRequest = true;
	private static readonly object LockFirstRequest = new();

	private static void Configure(WebApplication app)
	{
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			IdentityModelEventSource.ShowPII = true;
		}
		else
		{
			IdentityModelEventSource.ShowPII = true;
			app.UseExceptionHandler("/Error");
		}

		app.UseDefaultFiles();
		app.UseStaticFiles();

		// configuration on first request
		app.Use(async (ctx, next) =>
		{
			var path = ctx.Request.Path;

			GlobalSettings? globalSettings = null;

			if (_firstRequest)
			{
				// lock
				lock (LockFirstRequest)
				{
					// recheck
					if (_firstRequest)
					{
						_firstRequest = false;

						// set real frontend host
						globalSettings = ctx.RequestServices.GetRequiredService<GlobalSettings>();
						globalSettings.Host = GetFrontendHost(ctx);

						// update identity server
						var options = ctx.RequestServices.GetRequiredService<IdentityServerOptions>();
						options.IssuerUri = globalSettings.Host;

						// update identity server client redirect uri
						var clients = ctx.RequestServices.GetRequiredService<IEnumerable<Client>>();
						var client = clients.FirstOrDefault()!;
						client.RedirectUris = new List<string>
						{
							$"{globalSettings.Host}/silent-refresh.html"
						};
					}
				}
			}

			// 
			if (path.Value.EqualsIgnoreCase("/.well-known/openid-configuration"))
			{
				globalSettings ??= ctx.RequestServices.GetRequiredService<GlobalSettings>();
				ctx.SetIdentityServerOrigin(globalSettings.Host);
			}

			await next();
		});

		// send index.html to any routes except coreRoutes
		var coreRoutes = new[]
		{
			// API routes
			new PathString("/api"),
			// Identity server routes
			new PathString("/.well-known"),
			new PathString("/connect"),
			new PathString("/silent-refresh.html")
		};

		var webRootPath = app.Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
		var wwwrootPath = Path.Combine(webRootPath, "wwwroot");
		app.Use(async (context, next) =>
		{
			//app.Logger.LogWarning($"?path: {context.Request.Path}");
			var path = context.Request.Path;

			if (path.Value == null) return;
			if (coreRoutes.Any(x => path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase)))
			{
				//app.Logger.LogWarning($"*path: {path.Value}");
				await next();
			}
			else
			{
				if (!_staticFilesCache.TryGetValue(path.Value, out var physicalFileInfo))
				{
					var segments = path.Value.Split('/', '\\');
					var fileInfo = new FileInfo(Path.Combine(wwwrootPath, segments.LastOrDefault()!));
					if (!fileInfo.Exists)
					{
						//app.Logger.LogWarning($"-path: {fileInfo.FullName}");
						return;
					}

					//app.Logger.LogWarning($"+path: {fileInfo.FullName}");
					physicalFileInfo = new PhysicalFileInfo(fileInfo);
					_staticFilesCache.TryAdd(path.Value, physicalFileInfo);
				}

				await context.Response.SendFileAsync(physicalFileInfo);
				await context.Response.CompleteAsync();
			}
		});

		app.UseCors(builder => builder
			.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod());

		app.UseIdentityServer();
		app.UseAuthorization();

		app.MapControllers();
	}

	private static string GetFrontendHost(HttpContext ctx)
	{
		// get X-Forwarded-Proto when reversed proxy used
		// internet <-> nginx (https) <-> site(http)
		var host = $"{ctx.Request.Headers["X-Forwarded-Proto"]}";

		host = host.IsMissing()
			? ctx.Request.IsHttps ? "https" : "http"
			: host!;

		host += $"://{ctx.Request.Host.Value}";

		return host;
	}

	private static SigningCredentials CreateSigningCredential()
	{
		var rsaSecurityKey = new RsaSecurityKey(new RSACryptoServiceProvider(2048));
		var credentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

		return credentials;
	}
}