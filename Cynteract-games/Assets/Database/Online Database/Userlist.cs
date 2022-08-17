using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.OnlineDatabase
{
    public class Userlist
    {
        public List<IDUser> users;

        public Userlist(List<IDUser> users)
        {
            this.users = users;
        }
    }
}
