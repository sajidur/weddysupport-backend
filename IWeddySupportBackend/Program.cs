
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using IWeddySupport;
using IWeddySupport.Repository;
using IWeddySupport.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .AllowAnyOrigin()// Allow these origins
            .AllowAnyHeader() // Allow any headers
            .AllowAnyMethod()); // Allow any HTTP methods
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IWeddySupportDbContext>();
// Register your application-specific services
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<IWeddySupportDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))), ServiceLifetime.Scoped);

builder.Services.AddAuthorization();
// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AppSecret"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            // Uncomment if you have set Issuer and Audience in your JWT
            //ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });



var app = builder.Build();
app.UseCors("AllowSpecificOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Firebase initialization
var firebaseSettings = new FirebaseSettings();
builder.Configuration.GetSection("Firebase").Bind(firebaseSettings);

var credentialJson = new
{
    type = "service_account",
    project_id = firebaseSettings.ProjectId,
    private_key_id = firebaseSettings.PrivateKeyId,
    private_key = firebaseSettings.PrivateKey.Replace("\\n", "\n"),
    client_email = firebaseSettings.ClientEmail,
    client_id = firebaseSettings.ClientId,
    auth_uri = firebaseSettings.AuthUri,
    token_uri = firebaseSettings.TokenUri,
    auth_provider_x509_cert_url = firebaseSettings.AuthProviderX509CertUrl,
    client_x509_cert_url = firebaseSettings.ClientX509CertUrl
};

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(System.Text.Json.JsonSerializer.Serialize(credentialJson))
});




// Serve static files from the "uploads" directory
var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/uploads"
});


// Middleware for authentication and authorization
app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.UseStaticFiles();//to serve as static files

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<IWeddySupportDbContext>();
    // context.Database.Migrate();
}

app.Run();
