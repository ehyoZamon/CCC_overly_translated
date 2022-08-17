using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Cynteract.Database
{
    public class UserPanel : MonoBehaviour
    {
        public Button deleteButton;
        public TextMeshProUGUI title;


        public void Init(string name, OnlineDatabaseConnection connection, Action onChanged)
        {
            this.name = name;
            title.text = name;
            deleteButton.onClick.AddListener(async () => { await connection.DeleteUser(this.name); onChanged(); });

        }
    }
}