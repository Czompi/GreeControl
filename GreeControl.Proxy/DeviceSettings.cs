using CzomPack.Logging;
using System.Text.Json.Serialization;

namespace GreeControlProxy;

public class DeviceSettings
{
    public DeviceSettings()
    {
    }

    public DeviceSettings(Dictionary<string, int> properties) : base()
    {
        try
        {
            PowerState = properties["Pow"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set PowerState", ex);
            throw new EndOfStreamException("Device not found or not compatible.");
        }

        try
        {
            Mode = (DeviceMode)properties["Mod"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Mode", ex);
        }

        try
        {

            Temperature = properties["SetTem"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Temperature", ex);
        }

        try
        {
            TemperatureUnit = (TemperatureUnit)properties["TemUn"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set TemperatureUnit", ex);
        }

        try
        {
            FanSpeed = (FanSpeed)properties["WdSpd"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set FanSpeed", ex);
        }

        try
        {
            Air = properties["Air"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Air", ex);
        }

        try
        {
            XFan = properties["Blo"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set XFan", ex);
        }

        try
        {
            Health = properties["Health"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Health", ex);
        }

        try
        {
            SleepMode = properties["SwhSlp"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set SleepMode", ex);
        }

        try
        {
            Light = properties["Lig"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Light", ex);
        }

        try
        {
            SwingHorizontal = (SwingHorizontal)properties["SwingLfRig"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set SwingHorizontal", ex);
        }

        try
        {
            SwingVertical = (SwingVertical)properties["SwUpDn"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set SwingVertical", ex);
        }

        try
        {
            Quiet = properties["Quiet"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Quiet", ex);
        }

        try
        {
            Turbo = properties["Tur"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set Turbo", ex);
        }

        try
        {
            MaintainSteadyTemperature = properties["StHt"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set MaintainSteadyTemperature", ex);
        }

        try
        {
            HeatCoolType = properties["HeatCoolType"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set HeatCoolType", ex);
        }

        try
        {
            TemRec = properties["TemRec"];
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set TemRec", ex);
        }

        try
        {
            EnergySavingMode = properties["SvSt"] == 1;
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot set EnergySavingMode", ex);
        }

    }

    /// <summary>
    /// Power state of the device.
    /// </summary>
    public bool PowerState { get; init; }

    /// <summary>
    /// Operation mode.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DeviceMode Mode { get; init; }

    /// <summary>
    /// Temperature. Set <seealso cref="TemperatureUnit"/> as well.
    /// </summary>
    public int Temperature { get; init; }

    /// <summary>
    /// Unit the temp is in.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TemperatureUnit TemperatureUnit { get; init; }

    /// <summary>
    /// Fan speed
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FanSpeed FanSpeed { get; init; }

    /// <summary>
    /// Controls the state of the fresh air valve (not available on all units)
    /// </summary>
    public bool Air { get; init; }

    /// <summary>
    /// "Blow" or "X-Fan", this function keeps the fan running for a while after shutting down. Only usable in Dry and Cool mode.
    /// </summary>
    public bool XFan { get; init; }

    /// <summary>
    /// Controls Health ("Cold plasma") mode, only for devices equipped with "anion generator", which absorbs dust and kills bacteria.
    /// </summary>
    public bool Health { get; init; }

    /// <summary>
    /// Sleep mode, which gradually changes the temperature in Cool, Heat and Dry mode.
    /// </summary>
    public bool SleepMode { get; init; }

    /// <summary>
    /// Turns all indicators and the display on the unit on or off
    /// </summary>
    public bool Light { get; init; }

    /// <summary>
    /// Controls the swing mode of the horizontal air blades (available on limited number of devices, e.g. some Cooper & Hunter units - thanks to mvmn)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SwingHorizontal SwingHorizontal { get; init; }

    /// <summary>
    /// Controls the swing mode of the vertical air blades
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SwingVertical SwingVertical { get; init; }

    /// <summary>
    /// Controls the Quiet mode which slows down the fan to its most quiet speed. Not available in Dry and Fan mode.
    /// </summary>
    public bool Quiet { get; init; }

    /// <summary>
    /// Sets fan speed to the maximum. Fan speed cannot be changed while active and only available in Dry and Cool mode.
    /// </summary>
    public bool Turbo { get; init; }

    /// <summary>
    /// Maintain the room temperature steadily at 8°C and prevent the room from freezing by heating operation when nobody is at home for long in severe winter (from http://www.gree.ca/en/features)
    /// </summary>
    public bool MaintainSteadyTemperature { get; init; }

    /// <summary>
    /// Not documented
    /// </summary>
    public int HeatCoolType { get; init; }

    /// <summary>
    /// Used to distinguish between two Fahrenheit values (see Setting the temperature using Fahrenheit section below)
    /// </summary>
    public int TemRec { get; init; }

    /// <summary>
    /// Energy saving mode
    /// </summary>
    public bool EnergySavingMode { get; init; }
}
