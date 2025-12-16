using AutoMapper;

using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Clients;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;
using PaymentGateway.Services.Models.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IPaymentRequestValidator, PaymentRequestValidator>();
builder.Services.AddSingleton<IBankAPiClient, BankApiClient>();
builder.Services.AddSingleton<IBankService, BankService>();
builder.Services.AddSingleton<IMerchantRepository, MerchantRepository>();
builder.Services.AddSingleton<IMerchantService, MerchantService>();

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<PaymentResponse, PaymentGateway.Repositories.Models.PaymentResponse>();
}, new LoggerFactory());
builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
