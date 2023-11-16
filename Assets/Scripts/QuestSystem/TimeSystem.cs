using System;
using UnityEngine;

public class TimeSystem : SingletonMonobehaviour<TimeSystem>
{
    [SerializeField] Timestamp startingTime;

    [SerializeField] float gameSecondsPerRealSecond;
    
    float currentSecond;
    public int CurrentMinute { get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentDay { get; private set; }

    bool isTicking;

    public void StartGameTime() => isTicking = true;
    public void StopGameTime() => isTicking = false;

    // These will be overwritten if time has been previously saved.
    void OnEnable()
    {
        RestoreCurrentTime(startingTime);
    }

    void Update()
    {
        if (!isTicking) return;
        currentSecond += Time.deltaTime * gameSecondsPerRealSecond;

        if (currentSecond < 60) return;
            
        CurrentMinute++;
        currentSecond -= 60;
        EventManager.OnMinutesIncremented(CurrentMinute);

        if (CurrentMinute < 60) return;

        CurrentHour++;
        CurrentMinute -= 60;
        EventManager.OnHoursIncremented(CurrentHour);

        if (CurrentHour < 24) return;

        CurrentDay++;
        CurrentHour -= 24;
        EventManager.OnDaysIncremented(CurrentDay);
    }

    public Timestamp GetCurrentTimestamp => new Timestamp
    {
        seconds = currentSecond,
        minutes = CurrentMinute,
        hours = CurrentHour,
        days = CurrentDay
    };

    public void RestoreCurrentTime(object currentTimeObj)
    {
        if (currentTimeObj is not Timestamp currentTime)
        {
            Debug.LogError("Couldn't parse CurrentTime struct.");
            return;
        }
        
        currentSecond = currentTime.seconds;
        CurrentMinute = currentTime.minutes;
        CurrentHour = currentTime.hours;
        CurrentDay = currentTime.days;
        
        EventManager.OnMinutesIncremented(CurrentMinute);
    }
}

[Serializable]
public struct Timestamp
{
    public float seconds;
    public int minutes;
    public int hours;
    public int days;

    public string GetFormattedTimestampText() => $"Day {days}, {hours:00}:{minutes:00}";
}
