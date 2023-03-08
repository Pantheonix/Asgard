var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddFastEndpoints();

var jwtConfig = new JwtConfig();
builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddJWTBearerAuth(jwtConfig.SecretKey);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
