using EMPLOYEE.Application.Abstractions;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Auth;
using EMPLOYEE.Application.Features.Departments.DepartmentService;
using EMPLOYEE.Application.Features.Employees.EmployeeService;
using EMPLOYEE.Application.Features.Evaluations.Create;
using EMPLOYEE.Application.Features.Evaluations.Delete;
using EMPLOYEE.Application.Features.Evaluations.EvaluationService;
using EMPLOYEE.Application.Features.Evaluations.Query;
using EMPLOYEE.Application.Features.Evaluations.Update;
using EMPLOYEE.Data;
using EMPLOYEE.Middlware;
using EMPLOYEE.Models;
using EMPLOYEE.Repository;
using EMPLOYEE.Service;
using EMPLOYEE.Setting;
using EmployeeDomain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Logging (Serilog)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging());

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region Authentication & JWT
builder.Services.AddAuthentication(options => 
{ options
    .DefaultAuthenticateScheme = JwtBearerDefaults
    .AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults
    .AuthenticationScheme; }).
    AddJwtBearer(options => { var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        { ValidateIssuer = true, 
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"], 
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero, RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" 
        }; });
#endregion

#region Application Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<PasswordResetService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeEvaluationService, EmployeeEvaluationService>();
builder.Services.AddScoped<IPersonalEmailSend, PersonalEmailSend>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IEvaluationAppService, EvaluationAppService>();
builder.Services.AddScoped<IEvaluationAppService, EvaluationAppService>();
builder.Services.AddScoped<IEvaluationRatingService, EvaluationRatingService>();
builder.Services.AddScoped<CreateEvaluationService>();
builder.Services.AddScoped<UpdateEvaluationService>();
builder.Services.AddScoped<DeleteEvaluationService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.Configure<EmailSettind>(
builder.Configuration.GetSection("MailSettings"));
#endregion

#region MVC & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Build Application
var app = builder.Build();
using var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

// طلب وهمي لجعل EF Core و Identity تعمل التحميل الأولي
var dummyUser = await userManager.FindByEmailAsync("dummy@example.com");
if (dummyUser != null)
{
    var roles = await userManager.GetRolesAsync(dummyUser);
}

#endregion

//#region Global Exception Middleware (Slack)
//var slackWebhookUrl = builder.Configuration["Slack:WebhookUrl"];


//#endregion

#region Development Tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

#region Security Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Endpoints
app.MapControllers();
#endregion

#region Run
app.Run();
#endregion
