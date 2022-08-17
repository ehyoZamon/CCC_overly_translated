using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.OnlineDatabase
{
    public class User
    {
        public string username, password, role;

        public User(string username, string password, string role)
        {
            this.username = username;
            this.password = password;
            this.role = role;
        }
    }
}