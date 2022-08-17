using Cynteract.OnlineDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Cynteract.Database
{
    public class TherapistPanel : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public Button deleteButton, createUserButton;
        public IDUser user;
        private OnlineDatabaseConnection connection;
        public UserPanel userPanelPrefab;
        public List<UserPanel> userPanels = new List<UserPanel>();
        public Transform userParent, overlayParent;
        public CreateUserPanel createUserPanelPrefab;
        public void SetName(string name)
        {
            title.text = "<b>Therapist</b> " + name;
        }
        public async void Init(IDUser user, OnlineDatabaseConnection connection, Action onChange)
        {
            this.user = user;
            this.connection = connection;
            SetName(user.Username+ " id: " +user.id);
            deleteButton.onClick.AddListener(async () => { await connection.DeleteUser(this.user.Username); onChange(); });
            createUserButton.onClick.AddListener(async () =>
            {
                CreateUserPanel panel = Instantiate(createUserPanelPrefab, overlayParent);
                panel.Init("patient", CreateUser);
            });

            await UpdateUsers();
        }

        public async Task CreatePatient(IDUser therapist, User patient)
        {
            await connection.CreatePatient(therapist, patient);
            await UpdateUsers();
        }
        private async void UpdateUsersOnChanged()
        {
            await UpdateUsers();
        }
        private async Task UpdateUsers()
        {
            var patients = await connection.GetPatients(this.user) as GenericResponse<List<IDUser>>;
            DrawPatients(patients.content);
        }

        private void DrawPatients(List<IDUser> patients)
        {
            foreach (var item in userPanels)
            {
                Destroy(item.gameObject);
            }
            userPanels = new List<UserPanel>();
            foreach (var item in patients)
            {
                UserPanel user = Instantiate(userPanelPrefab, userParent);
                user.Init(item.Username, connection, UpdateUsersOnChanged);
                userPanels.Add(user);
            }
        }
        public async Task DeleteUser(string user)
        {
            await connection.DeleteUser(user);
            await UpdateUsers();
        }
        public async Task CreateUser(User user)
        {
            await connection.CreatePatient(this.user, user);
            await UpdateUsers();
        }
    }
}