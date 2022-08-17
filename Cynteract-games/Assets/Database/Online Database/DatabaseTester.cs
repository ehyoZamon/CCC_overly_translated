using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Database
{
    public class DatabaseTester : MonoBehaviour
    {
        public DatabaseManager databaseManager;
        [Button]
        public async void Login(string username, string password)
        {
            await databaseManager.Login(username, password);
        }
    }
}
