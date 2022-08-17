using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginResponse
{
    Success,
    WrongPassword,
    UserUnknown,
    ToManyFailedAttempts,
    UnknownFail
}
