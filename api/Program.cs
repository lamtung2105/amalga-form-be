using api.Provider;
using api.Services;
using HandlebarsDotNet;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(c => c.AddPolicy("allowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddControllers();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PrescriptionService>();
// Đăng ký helper tại đây
Handlebars.RegisterHelper("inc", (writer, context, parameters) =>
{
    var value = parameters[0] is int i ? i : Convert.ToInt32(parameters[0]);
    writer.WriteSafeString((value + 1).ToString());
});
string connectionString;
var environment = builder.Environment.EnvironmentName;
if (environment == Environments.Production)
{
    var server = Environment.GetEnvironmentVariable("SERVER");
    var db = Environment.GetEnvironmentVariable("DB");
    var acc = Environment.GetEnvironmentVariable("ACC");
    var pwd = Environment.GetEnvironmentVariable("PASSWD");

    connectionString = $"Data Source={server};Initial Catalog={db};User ID={acc};Password={pwd};Encrypt=False;TrustServerCertificate=True";
}
else connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new Exception("Missing connection string: Default");
builder.Services.AddSingleton(new ConnectionStringProvider(connectionString));
var app = builder.Build();
app.UseCors("allowAll");
app.MapControllers();
app.Run();
