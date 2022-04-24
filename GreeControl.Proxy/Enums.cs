namespace GreeControlProxy;
public enum SwingHorizontal
{
    Default = 0,
    FullRange = 1,
    FixedLeftMost = 2,
    FixedMiddleUp = 3,
    FixedMiddle = 4,
    FixedMiddleLow = 5,
    FixedRightMost = 6
}
public enum SwingVertical
{
    Default = 0,
    FullRange = 1,
    FixedUpMost = 2,
    FixedMiddleUp = 3,
    FixedMiddle = 4,
    FixedMiddleLow = 5,
    FixedLowest = 6,
    SwingDownMost = 7,
    SwingMiddleLow = 8,
    SwingMiddle = 9,
    SwingMiddleUp = 10,
    SwingUpMost = 11,
}
public enum State
{
    Off = 0,
    On = 1
}
public enum DeviceMode
{
    Auto = 0,
    Cool = 1,
    Dry = 2,
    Fan = 3,
    Heat = 4
}

public enum TemperatureUnit
{
    Celsius = 0,
    Farenheit = 1
}

public enum FanSpeed
{
    Auto = 0,
    Low = 1,
    MediumLow = 2,
    Medium = 3,
    MediumHigh = 4,
    High = 5
}
