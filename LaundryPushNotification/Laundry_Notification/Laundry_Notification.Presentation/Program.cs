using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;
using Laundry_Notification.Application.Interface;
using Laundry_Notification.Application.Interfaces;
using Laundry_Notification.Infrastructure.Repositories;
using Laundry_Notification.Infrastructure.Service;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddSingleton(provider =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "Config", "Firebase", "ServiceAccountKey.json");

    using var jsonStream = File.OpenRead(path);
    using var jsonDoc = JsonDocument.Parse(jsonStream);
    var projectId = jsonDoc.RootElement.GetProperty("project_id").GetString();

    var firestoreClientBuilder = new FirestoreClientBuilder
    {
        CredentialsPath = path
    };
    var client = firestoreClientBuilder.Build();

    return FirestoreDb.Create(projectId, client);
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
