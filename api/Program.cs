using System.Text;
using api.Data;
using api.Features.Auth.Interfaces;
using api.Features.Auth.Repositories;
using api.Features.Auth.Services;
using api.Features.Expiration.Configs;
using api.Features.Expiration.Services;
using api.Features.SignalR;
using api.Features.Transaction.Context.Builders;
using api.Features.Transaction.Context.Factories;
using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Factories;
using api.Features.Transaction.Handlers;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Repository;
using api.Features.Transaction.Services;
using api.Features.Transaction.Validators;
using api.Features.User;
using api.Features.UserCredential.Context.Remove.ExtraData;
using api.Features.UserCredential.Context.Remove.Factories;
using api.Features.UserCredential.Context.Remove.Interfaces;
using api.Features.UserCredential.Context.Update.ExtraData;
using api.Features.UserCredential.Context.Update.Factories;
using api.Features.UserCredential.Context.Update.Interfaces;
using api.Features.UserCredential.Factories;
using api.Features.UserCredential.Handlers.Find;
using api.Features.UserCredential.Handlers.Register;
using api.Features.UserCredential.Handlers.Remove;
using api.Features.UserCredential.Handlers.Update;
using api.Features.UserCredential.Handlers.Validate;
using api.Features.UserCredential.Handlers.Verify;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Repositories;
using api.Features.UserCredential.Services;
using api.Features.Wallet;
using api.Shared.Handlers.Auth;
using api.Shared.Interfaces.Expiration;
using api.Shared.Interfaces.Wallet;
using api.Shared.Swagger;
using api.Shared.UserCredential.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4; //todo change this for production
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JWT");
//todo add a null check on signing key
var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]!);
// builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol(options =>
    {
        options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero
        };

        // Support SignalR authentication via access_token query param
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/userhub") ||
                     path.StartsWithSegments("/wallethub") ||
                     path.StartsWithSegments("/transactionhub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.Configure<ExpirationConfigTyped>(
    builder.Configuration.GetSection("ExpirationRules"));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMerchantOrHigher",
        policy => policy.Requirements.Add(new RoleHierarchyHandler.RoleRequirement("Merchant")));
});

// expiration
builder.Services.AddSingleton<IExpirationService, ExpirationService>();

// wallet
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// user
builder.Services.AddScoped<IUserService, UserService>();

// auth
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddSingleton<IAuthorizationHandler, RoleHierarchyHandler>();

// transaction
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionFactory, TransactionFactory>();
builder.Services.AddScoped<ITransactionHandlerFactory, TransactionHandlerFactory>();
builder.Services.AddScoped<QrPaymentHandler>();
builder.Services.AddScoped<RfidPaymentHandler>();
builder.Services.AddScoped<ITransactionValidator, TransactionValidator>();
builder.Services.AddScoped<IUserWalletValidator, UserWalletValidator>();
builder.Services.AddScoped<IVerificationHandler, VerificationHandler>();
builder.Services.AddScoped<QrPaymentContextBuilder>();
builder.Services.AddScoped<RfidPaymentContextBuilder>();
builder.Services.AddScoped<ITransactionContextBuilderFactory, TransactionContextBuilderFactory>();
builder.Services.AddScoped<ITransactionUpdateHandler, TransactionUpdateHandler>();
builder.Services.AddScoped<ITransactionQueryHandler, TransactionQueryHandler>();
//user credential
builder.Services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
builder.Services.AddScoped<IUserCredentialService, UserCredentialService>();
builder.Services.AddScoped<IRemoveCredentialContextBuilderFactory, RemoveCredentialContextBuilderFactory>();
builder.Services.AddScoped<RemoveRfidPinData>();
builder.Services.AddScoped<RemoveRfidTagData>();
builder.Services.AddScoped<IUpdateCredentialContextBuilderFactory, UpdateCredentialContextBuilderFactory>();
builder.Services.AddScoped<UpdateRfidTagData>();
builder.Services.AddScoped<UpdateRfidPinData>();
builder.Services.AddScoped<ICredentialFactory, CredentialFactory>();
builder.Services.AddScoped<ICredentialFinderHandler, CredentialFinderHandler>();
builder.Services.AddScoped<ICredentialRegistrationHandlerFactory, CredentialRegistrationHandlerFactory>();
builder.Services.AddScoped<RfidTagRegistrationHandler>();
builder.Services.AddScoped<RfidPinRegistrationHandler>();
builder.Services.AddScoped<ICredentialRemoverHandlerFactory, CredentialRemoverHandlerFactory>();
builder.Services.AddScoped<RfidTagRemoverHandler>();
builder.Services.AddScoped<RfidPinRemoverHandler>();
builder.Services.AddScoped<ICredentialUpdateHandlerFactory, CredentialUpdateHandlerFactory>();
builder.Services.AddScoped<RfidTagUpdateHandler>();
builder.Services.AddScoped<RfidPinUpdateHandler>();
builder.Services.AddScoped<ICredentialValidatorHandlerFactory, CredentialValidatorHandlerFactory>();
builder.Services.AddScoped<RfidTagValidatorHandler>();
builder.Services.AddScoped<ICredentialVerificationHandlerFactory, CredentialVerificationHandlerFactory>();
builder.Services.AddScoped<RfidTagVerificationHandler>();
builder.Services.AddScoped<RfidPinVerificationHandler>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//todo uncomment this if finished testing
// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<UserHub>("/userhub");

app.Run();