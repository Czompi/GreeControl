using CzomPack;
using CzomPack.Attributes;
using CzomPack.Extensions;
using CzomPack.Logging;
using GreeControl.Proxy.Database;
using GreeNativeSdk;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Reflection;

namespace GreeControlProxy;

[Executable]
public partial class Program
{
    static List<AirConditioner> AirConList = new();
    static CancellationTokenSource cts = new();
    static partial void Main(Arguments args)
    {
        Settings.Application = new(Assembly.GetAssembly(typeof(Program)));
        Settings.WorkingDirectory = Path.GetFullPath("./");
        Log.Logger = Logger.GetLogger();
        var builder = WebApplication.CreateBuilder(args.GetArgumentList());

        builder.Host.UseSerilog();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddSqlite<DatabaseContext>("Data Source=devices.db;");


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            // app.UseMigrationsEndPoint();
        }

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DatabaseContext>();
            context.Database.EnsureCreated();
            // DbInitializer.Initialize(context);
        }

        app.UseHttpsRedirection();


        TaskHelper.RecurringTask(() => RefreshAirConList(), 600, cts.Token);

        app.MapGet("/devices", (DatabaseContext db) =>
        {
            return db.Devices.Select(dev => new { Id = dev.Id, Name = $"AC-{dev.Name}"});
        })
        .WithName("ScanDevices");

        app.MapGet("/device/{id}/settings", async (string id) =>
        {
            DeviceSettings config = new();
            try
            {
                AirConditionerController acc = new(AirConList.First(ac => ac.Id.EqualsIgnoreCase(id)));
                await acc.UpdateDeviceStatus();
                return Results.Json(new DeviceSettings(acc.Parameters), statusCode: 200);
            }
            catch (Exception ex)
            {
                Logger.Error("Error while getting settings from '{id}'. Error: {exception}", id, ex);
            }
            //return Results.Json(new ErrorResult("DeviceNotFound", $"Device with id {id} not found."), statusCode: 404);
            return Results.NotFound();
        })
        .WithName("GetDeviceConfig");

        app.MapPost("/device/{id}/settings", async (string id, [FromBody] DeviceSettings newSettings) =>
        {
            DeviceSettings config = new();
            try
            {
                AirConditionerController acc = new(AirConList.First(ac => ac.Id.EqualsIgnoreCase(id)));
                await acc.UpdateDeviceStatus();
                Thread.Sleep(100);
                var settings = new DeviceSettings(acc.Parameters);

                if (settings.PowerState != newSettings.PowerState) await acc.SetDeviceParameter("Pow", newSettings.PowerState ? 1 : 0);
                if (settings.Mode != newSettings.Mode) await acc.SetDeviceParameter("Mod", (int)newSettings.Mode);
                if (settings.Temperature != newSettings.Temperature)
                {
                    await acc.SetDeviceParameter("SetTem", newSettings.Temperature);
                    Thread.Sleep(100);
                    await acc.SetDeviceParameter("TemUn", (int)newSettings.TemperatureUnit);
                }
                if (settings.FanSpeed != newSettings.FanSpeed) await acc.SetDeviceParameter("WdSpd", (int)newSettings.FanSpeed);
                if (settings.Air != newSettings.Air) await acc.SetDeviceParameter("Air", newSettings.Air ? 1 : 0);
                if (settings.XFan != newSettings.XFan) await acc.SetDeviceParameter("Blo", newSettings.XFan ? 1 : 0);
                if (settings.Health != newSettings.Health) await acc.SetDeviceParameter("Health", newSettings.Health ? 1 : 0);
                if (settings.SleepMode != newSettings.SleepMode) await acc.SetDeviceParameter("SwhSlp", newSettings.SleepMode ? 1 : 0);
                if (settings.Light != newSettings.Light) await acc.SetDeviceParameter("Lig", newSettings.Light ? 1 : 0);
                if (settings.SwingHorizontal != newSettings.SwingHorizontal) await acc.SetDeviceParameter("SwingLfRig", (int)newSettings.SwingHorizontal);
                if (settings.SwingVertical != newSettings.SwingVertical) await acc.SetDeviceParameter("SwUpDn", (int)newSettings.SwingVertical);
                if (settings.Quiet != newSettings.Quiet) await acc.SetDeviceParameter("Quiet", newSettings.Quiet ? 1 : 0);
                if (settings.Turbo != newSettings.Turbo) await acc.SetDeviceParameter("Tur", newSettings.Turbo ? 1 : 0);
                if (settings.MaintainSteadyTemperature != newSettings.MaintainSteadyTemperature) await acc.SetDeviceParameter("StHt", newSettings.MaintainSteadyTemperature ? 1 : 0);
                if (settings.HeatCoolType != newSettings.HeatCoolType) await acc.SetDeviceParameter("HeatCoolType", newSettings.HeatCoolType);
                if (settings.TemRec != newSettings.TemRec) await acc.SetDeviceParameter("TemRec", newSettings.TemRec);
                if (settings.EnergySavingMode != newSettings.EnergySavingMode) await acc.SetDeviceParameter("SvSt", newSettings.EnergySavingMode ? 1 : 0);

                Thread.Sleep(100);
                await acc.UpdateDeviceStatus();

                return new DeviceSettings(acc.Parameters);
            }
            catch (Exception ex)
            {
                Logger.Error("Error occurred during configuration of {id}. Error: {exception}", id, ex);
            }
            return config;
        })
        .WithName("PostDeviceConfig");

        app.Run();

    }

    #region Background tasks
    private static async void RefreshAirConList()
    {
        try
        {
            var main_network = await Scanner.Scan("10.1.15.255");
            AirConList.AddRange(main_network);
        }
        catch (Exception ex)
        {
            Logger.Error("Error during scanning main network. Error: {exception}", ex);
        }
        try
        {
            var seco_network = await Scanner.Scan("10.1.16.255");
            AirConList.AddRange(seco_network);
        }
        catch (Exception ex)
        {
            Logger.Error("Error during scanning secondary network. Error: {exception}", ex);
        }
        try
        {
            DatabaseContext db = new();
            db.Database.EnsureCreated();
            foreach (var ac in AirConList)
            {
                if (!db.Devices.Any(dev => dev.Id.ToLower().Equals(ac.Id.ToLower())))
                {
                    db.Devices.Add(ac);
                    db.SaveChanges();
                    Logger.Info("Device {id} added.", ac.Id);
                }
                else
                {
                    Logger.Info("Device {id} already exists.", ac.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Error during database insert. Error: {exception}", ex);
        }
    }
    #endregion

}