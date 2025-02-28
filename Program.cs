using DuAnTN.Services;
using DuAnTN.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình HttpClient cho API
builder.Services.AddHttpClient<CategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7248/api/"); // Địa chỉ API
});

// Đăng ký Session & HttpContext
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Đăng ký Services (Scoped để tránh memory leak)
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<FoodService>();
builder.Services.AddScoped<DinerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<UserInfoServices>();
builder.Services.AddScoped<AuthService>();

// Đăng ký HttpClient cho các Service
builder.Services.AddHttpClient<FoodService>();
builder.Services.AddHttpClient<DinerService>();
builder.Services.AddHttpClient<AddressService>();
builder.Services.AddHttpClient<UserInfoServices>();
builder.Services.AddHttpClient<AuthService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//  Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Issuer"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Kích hoạt Session
app.UseSession();
app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
