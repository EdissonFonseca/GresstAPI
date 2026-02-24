using Gresst.API.Configuration;
using Gresst.API.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.AddGresstLogging();
builder.Services.AddGresstApiVersioning();
builder.Services.AddGresstMultitenancy();
builder.AddGresstJwtAuthentication();
builder.Services.AddGresstAuthorizationPolicies();
builder.Services.AddGresstAuthenticationServices();
builder.AddGresstDatabase();
builder.Services.AddGresstMappers();
builder.Services.AddGresstRepositories();
builder.Services.AddGresstApplicationServices();
builder.AddGresstCors();
builder.ConfigureGresstKestrel();
// GraphQL
builder.Services.AddGresstGraphQL();

var app = builder.Build();
app.UseGresstPipeline();
app.MapGraphQL();
app.Run();
