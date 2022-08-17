using SqliteForUnity3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginAttempts 
{
    [Unique]
    public string Username { get; set; }

    public int FailedAttempts { get; set; }
}
