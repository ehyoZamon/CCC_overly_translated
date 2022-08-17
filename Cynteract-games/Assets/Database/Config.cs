using SqliteForUnity3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Config 
{
    [Unique]
    public int UserID
    {
        get; set;
    }
    public long LastTrainingsDataSync
    {
        get;  set;
    }
    public long LastFeedbackSync
    {
        get; set;
    }
    public bool Consented
    {
        get; set;
    }
    public bool ShowWelcomeScreen
    {
        get; set;
    }
    public Config()
    {

    }

    public Config(int userID, DateTime lastTrainingsDataSync, DateTime lastFeedbackSync, bool consented, bool showWelcomeScreen)
    {
        UserID = userID;
        LastTrainingsDataSync = lastTrainingsDataSync.Ticks;
        LastFeedbackSync = lastFeedbackSync.Ticks;
        Consented = consented;
        ShowWelcomeScreen = showWelcomeScreen;
    }
    public override string ToString()
    {
        return $"{{UserID: {UserID}, LastTrainingsDataSync: {LastTrainingsDataSync}, LastFeedbackSync: {LastFeedbackSync},Consented: {Consented} }}";
    }
}
