using KOLOS1_POPRAWA.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddScoped<IDbService, DbService>();
var app = builder.Build();


app.UseHttpsRedirection();
app.MapControllers();

app.Run();