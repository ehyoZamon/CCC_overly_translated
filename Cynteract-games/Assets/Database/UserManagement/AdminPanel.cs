using Cynteract.OnlineDatabase;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace Cynteract.Database
{
    public class AdminPanel : MonoBehaviour
    {
        OnlineDatabaseConnection connection = new OnlineDatabaseConnection();
        public TherapistPanel therapistPanelPrefab;
        public List<TherapistPanel> therapistPanels = new List<TherapistPanel>();
        public Transform therapistParent, overlayParent;

        public UserPanel userPanelPrefab;

        public List<UserPanel> userPanels = new List<UserPanel>();
        public Transform userParent;
        public Button createPatientButton;

        public Button createTherapistButton;
        public CreateUserPanel createUserPanelPrefab;


        public Button refreshButton;
        private void Awake()
        {
            createTherapistButton.onClick.AddListener(() =>
            {
                CreateUserPanel panel = Instantiate(createUserPanelPrefab, overlayParent);
                panel.Init("therapist", CreateUser);
            });
            createPatientButton.onClick.AddListener(() =>
            {
                CreateUserPanel panel = Instantiate(createUserPanelPrefab, overlayParent);
                panel.Init("patient", CreateUser);
            });
            refreshButton.onClick.AddListener(async () => await UpdateUsers());
        }
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
        [Button]
        private async void Init()
        {
            if (CandyCoded.env.env.TryParseEnvironmentVariable("STANDARD_ADMIN_USERNAME", out string username))
            {
                
            }
            else
            {
                throw new Exception("Environment key LOCAL_DATABASE_PASSWORD not found");
            }
            if (CandyCoded.env.env.TryParseEnvironmentVariable("STANDARD_ADMIN_PASSWORD", out string password))
            {

            }
            else
            {
                throw new Exception("Environment key LOCAL_DATABASE_PASSWORD not found");
            }
            var response= await connection.TryLogin(username, password);
            await UpdateUsers();
        }
        [Button]
        private async void GetTherapistPatients()
        {
            var connections = await connection.GetTherapistPatients() as GenericResponse<List<dynamic>>;
            connections.content.ForEach(Debug.Log);
        }
        [Button]
        private async void DeleteUnsafeAdmin()
        {
            await connection.DeleteUser("admin");
        }
        private async void UpdateOnChanged()
        {
            await UpdateUsers();
        }

        private async Task UpdateUsers()
        {
            ServerResponse serverResponse = await connection.GetAllUsers();
            var users = serverResponse as GenericResponse<List<IDUser>>;
            DrawUsers(users.content);
            var therapists = users.content.Where(x => x.Role == "therapist");
            DrawTherapists(therapists);
        }

        private void DrawUsers(List<IDUser> users)
        {
            foreach (var item in userPanels)
            {
                Destroy(item.gameObject);
            }
            userPanels = new List<UserPanel>();
            foreach (var item in users)
            {
                UserPanel user = Instantiate(userPanelPrefab, userParent);
                user.Init(item.username, connection, UpdateOnChanged);
                userPanels.Add(user);
            }

            RectTransform rectTransform = userParent.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, userPanels.Count * 60);

        }

        private void DrawTherapists(IEnumerable<IDUser> therapists)
        {
            foreach (var item in therapistPanels)
            {
                Destroy(item.gameObject);
            }
            therapistPanels = new List<TherapistPanel>();
            foreach (var item in therapists)
            {
                TherapistPanel therapist = Instantiate(therapistPanelPrefab, therapistParent);
                therapist.Init(item, connection, UpdateOnChanged);
                therapistPanels.Add(therapist);
            }
        }

        [Button]
        public async Task CreateUser(User user)
        {
            await connection.InsertUser(user);
            await Task.Delay(400);
            await UpdateUsers();
        }
        [Button]
        public void CreatePatient(IDUser therapist)
        {
            CreateUserPanel panel = Instantiate(createUserPanelPrefab, overlayParent);
            panel.Init("therapist", user => CreatePatient(therapist, user));
        }
        [Button]
        public async Task CreatePatient(IDUser therapist, User patient)
        {
            await connection.CreatePatient(therapist, patient);
            await UpdateUsers();
        }
        [Button]
        public async Task DeleteUser(string name)
        {
            await connection.DeleteUser(name);
            await UpdateUsers();
        }
    }
}