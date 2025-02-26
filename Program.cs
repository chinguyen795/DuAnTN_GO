using DuAnTN.Services;
using DuAnTN.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<CategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7248/api/"); // Địa chỉ API
});
// Add services to the container.
builder.Services.AddScoped<CategoryService>(); // hoặc AddTransient hoặc AddSingleton tùy thuộc vào yêu cầu
builder.Services.AddScoped<FoodService>();
builder.Services.AddScoped<DinerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AddressService>();

// Các dịch vụ khác
builder.Services.AddControllersWithViews();
// Đăng ký HttpClient
builder.Services.AddHttpClient<FoodService>();
builder.Services.AddHttpClient<DinerService>();
builder.Services.AddHttpClient<FoodService>();
builder.Services.AddHttpClient<DinerService>();
builder.Services.AddHttpClient<AddressService>();

// Thêm Razor Pages vào dịch vụ
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

//// Định tuyến với khu vực (Areas)
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "Admin",
//    pattern: "{area:exists}/{controller=QuanLy}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "Saller",
//    pattern: "{area:exists}/{controller}/{action=index}/{id?}");

app.Run();