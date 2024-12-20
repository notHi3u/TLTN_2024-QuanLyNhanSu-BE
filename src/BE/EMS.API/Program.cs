﻿
using Common.Configurations;
using Common.Helpers;
using EMS.API.Endpoints.Account;
using EMS.API.Endpoints.EM;
using EMS.Application.Automapper;
using EMS.Application.Services.Account;
using EMS.Application.Services.EM;
using EMS.Domain.Models;
using EMS.Domain.Models.Account;
using EMS.Domain.Repositories.Account;
using EMS.Domain.Repositories.EM;
using EMS.Infrastructure.Contexts;
using EMS.Infrastructure.Repositories.Account;
using EMS.Infrastructure.Repositories.EM;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EMS (Employee Management System)",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer jhfdkj.jkdsakjdsa.jkdsajk\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.EnableAnnotations();
});

builder.Services.Configure<JsonOptions>(options =>
{
    // Set ReferenceHandler to ignore cycles to prevent loops in serialization
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

//Rate limiting middlewares
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

// Add Identity services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.Requirements.Add(new RoleRequirement("Admin")));

    options.AddPolicy("HR", policy =>
        policy.Requirements.Add(new RoleRequirement("HR")));

    options.AddPolicy("DepartmentManager", policy =>
    {
        policy.Requirements.Add(new RoleRequirement("Department Manager"));
        policy.Requirements.Add(new DepartmentManagerRequirement("DepartmentId")); // Custom department-specific check
    });

    options.AddPolicy("Employee", policy =>
        policy.Requirements.Add(new RoleRequirement("Employee")));
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddHealthChecks();

//Register application services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<ITfaService, TfaService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register EMS services and repositories
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped<IEmployeeRelativeService, EmployeeRelativeService>();
builder.Services.AddScoped<IEmployeeRelativeRepository, EmployeeRelativeRepository>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IHolidayLeavePolicyService, HolidayLeavePolicyService>();
builder.Services.AddScoped<IHolidayLeavePolicyRepository, HolidayLeavePolicyRepository>();

builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

builder.Services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
builder.Services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();

builder.Services.AddScoped<ISalaryRecordService, SalaryRecordService>();
builder.Services.AddScoped<ISalaryRecordRepository, SalaryRecordRepository>();

builder.Services.AddScoped<ITimeCardService, TimeCardService>();
builder.Services.AddScoped<ITimeCardRepository, TimeCardRepository>();

builder.Services.AddScoped<IWorkRecordService, WorkRecordService>();
builder.Services.AddScoped<IWorkRecordRepository, WorkRecordRepository>();

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<Role>>();

builder.Services.AddScoped<IAuthorizationHandler, DepartmentManagerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();


// Add DbContext configuration
builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseNpgsql(
               builder.Configuration.GetConnectionString("EMS"),
               b => b.MigrationsAssembly("EMS.PostgresMigrations")));
builder.Services.AddIdentityApiEndpoints<User>().AddRoles<Role>()
    .AddEntityFrameworkStores<AppDbContext>();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader() // Cho phép tất cả các header
                                //.AllowCredentials();
                          ;
                      });
});
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
    options.ExcludedHosts.Add("example.com");
    options.ExcludedHosts.Add("www.example.com");
});

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var response = new
        {
            // Tạo cấu trúc JSON cho phản hồi lỗi
            error = "An error occurred while processing your request.",
            details = exception?.Message // Thêm thông tin chi tiết lỗi nếu cần
        };

        var responseJson = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(responseJson);
    });
});
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Features.Set<IAntiforgeryValidationFeature>(null);
    await next();
});


app.UseAuthentication();
app.UseAuthorization();

//app.UseAntiforgery();

app.UseRateLimiter();

//Minimals APIs
//Map group Endpoints
//app.MapGroup("/Auth").MapIdentityApi<User>();
IdentityEndpoints.MapIdentityApi<User>(app);
UserEndpoints.Map(app);
RoleEndpoints.Map(app);
PermissionEndpoints.Map(app);
AttendanceEndpoints.Map(app);
DepartmentEndpoints.Map(app);
EmployeeEndpoints.Map(app);
EmployeeRelativeEndpoints.Map(app);
HolidayLeavePolicyEndpoints.Map(app);
LeaveBalanceEndpoints.Map(app);
LeaveRequestEndpoints.Map(app);
SalaryRecordEndpoints.Map(app);
TimeCardEndpoints.Map(app);
WorkRecordEndpoints.Map(app);

//IdentityEndpoints.MapCustomIdentityApi<User>(app);


// Run the application
app.Run();