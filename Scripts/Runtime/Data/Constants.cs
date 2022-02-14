using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public enum DistanceUnits
    {
        Meters,
        Kilometers,
        AU
    }

    public enum VelocityUnits
    {
        MetersASecond,
        KilometersAHour,
        Lightspeed
    }

    public enum TimeUnits
    {
        Seconds,
        Minutes,
        Hours,
        Days,
        Years
    }

    public const float SpeedOfLight = 299792458; // In m/s
    public const float AstronomicalUnit = 149600000; // In KM

    public static float ConvertMetersASecondTo (VelocityUnits convertTo, float speedInMetersASecond)
    {
        switch (convertTo)
        {
            case VelocityUnits.MetersASecond:
                return speedInMetersASecond;
            case VelocityUnits.KilometersAHour:
                return speedInMetersASecond * 3.6f;
            case VelocityUnits.Lightspeed:
                return speedInMetersASecond / SpeedOfLight;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertKmHourTo(VelocityUnits convertTo, float speedInKmHour)
    {
        switch (convertTo)
        {
            case VelocityUnits.MetersASecond:
                return speedInKmHour / 3.6f;
            case VelocityUnits.KilometersAHour:
                return speedInKmHour;
            case VelocityUnits.Lightspeed:
                return (speedInKmHour * 3.6f) / SpeedOfLight;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertLightspeedTo(VelocityUnits convertTo, float speedInLightspeed)
    {
        switch (convertTo)
        {
            case VelocityUnits.MetersASecond:
                return speedInLightspeed / SpeedOfLight;
            case VelocityUnits.KilometersAHour:
                return (speedInLightspeed / SpeedOfLight) * 3.6f;
            case VelocityUnits.Lightspeed:
                return speedInLightspeed;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertMetersTo(DistanceUnits convertTo, float distanceInMeters)
    {
        switch (convertTo)
        {
            case DistanceUnits.Meters:
                return distanceInMeters;
            case DistanceUnits.Kilometers:
                return distanceInMeters / 1000f;
            case DistanceUnits.AU:
                return (distanceInMeters * 1000f) / AstronomicalUnit;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertKmTo (DistanceUnits convertTo, float distanceInKM)
    {
        switch (convertTo)
        {
            case DistanceUnits.Meters:
                return distanceInKM * 1000f;
            case DistanceUnits.Kilometers:
                return distanceInKM;
            case DistanceUnits.AU:
                return distanceInKM / AstronomicalUnit;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertAUTo(DistanceUnits convertTo, float distanceInAU)
    {
        switch (convertTo)
        {
            case DistanceUnits.Meters:
                return distanceInAU * 1000f * AstronomicalUnit;
            case DistanceUnits.Kilometers:
                return distanceInAU * AstronomicalUnit;
            case DistanceUnits.AU:
                return distanceInAU;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertSecondsTo(TimeUnits convertTo, float timeInSeconds)
    {
        switch (convertTo)
        {
            case TimeUnits.Seconds:
                return timeInSeconds;
            case TimeUnits.Minutes:
                return timeInSeconds / 60f;
            case TimeUnits.Hours:
                return timeInSeconds / 3600f;
            case TimeUnits.Days:
                return timeInSeconds / 86400f;
            case TimeUnits.Years:
                return timeInSeconds / 31536000f;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertMinutesTo(TimeUnits convertTo, float timeInMinutes)
    {
        switch (convertTo)
        {
            case TimeUnits.Seconds:
                return timeInMinutes * 60f;
            case TimeUnits.Minutes:
                return timeInMinutes;
            case TimeUnits.Hours:
                return timeInMinutes / 60f;
            case TimeUnits.Days:
                return timeInMinutes / 1440f;
            case TimeUnits.Years:
                return timeInMinutes / 525600f;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertHoursTo(TimeUnits convertTo, float timeInHours)
    {
        switch (convertTo)
        {
            case TimeUnits.Seconds:
                return timeInHours * 3600f;
            case TimeUnits.Minutes:
                return timeInHours * 60f;
            case TimeUnits.Hours:
                return timeInHours;
            case TimeUnits.Days:
                return timeInHours / 24f;
            case TimeUnits.Years:
                return timeInHours / 8760f;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertDaysTo(TimeUnits convertTo, float timeInDays)
    {
        switch (convertTo)
        {
            case TimeUnits.Seconds:
                return timeInDays * 86400f;
            case TimeUnits.Minutes:
                return timeInDays * 1440f;
            case TimeUnits.Hours:
                return timeInDays * 24f;
            case TimeUnits.Days:
                return timeInDays;
            case TimeUnits.Years:
                return timeInDays / 365f;
            default:
                break;
        }
        return 0;
    }

    public static float ConvertYearsTo(TimeUnits convertTo, float timeInYears)
    {
        switch (convertTo)
        {
            case TimeUnits.Seconds:
                return timeInYears * 31536000f;
            case TimeUnits.Minutes:
                return timeInYears * 525600f;
            case TimeUnits.Hours:
                return timeInYears * 8760f;
            case TimeUnits.Days:
                return timeInYears * 365f;
            case TimeUnits.Years:
                return timeInYears;
            default:
                break;
        }
        return 0;
    }
}
