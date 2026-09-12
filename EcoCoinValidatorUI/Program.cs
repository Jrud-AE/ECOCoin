using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(Options =>
{
    Options.ServiceName = "EcoCoinValidatorUI";
});

// See https://aka.ms/new-console-template for more information

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Eco Coin Validation Running.  Press any key to end.");
EcoCoinValidator.Controller C = new EcoCoinValidator.Controller();
Console.ReadKey();