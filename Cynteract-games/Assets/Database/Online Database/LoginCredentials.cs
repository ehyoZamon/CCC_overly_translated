using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.OnlineDatabase
{
    public class LoginCredentials:ServerResponse
    {
        public  string accessToken, refreshToken;
        public LoginCredentials(string accessToken, string refreshToken)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }
}

