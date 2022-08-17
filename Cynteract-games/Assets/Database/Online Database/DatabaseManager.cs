using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using Sirenix.OdinInspector;
using System.Threading;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using Cynteract.OnlineDatabase;

namespace Cynteract.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        OnlineDatabaseConnection onlineDatabaseConnection = new OnlineDatabaseConnection();
        LocalDatabaseConnection localDatabaseConnection = new LocalDatabaseConnection();

        CallbackList onConnectedCallbacks = new CallbackList();
        CallbackList onLoggedInCallbacks = new CallbackList();
        CallbackList onDisconnectedCallbacks = new CallbackList();

        Thread connectionThread;


        public CallbackList onDeprecatedVersionCallbacks = new CallbackList();
        public CallbackList onOldVersionCallbacks = new CallbackList();

        public static DatabaseManager instance;
        private string version;

        public IDUser User
        {
            get;
            private set;
        }
        public bool LoggedIn
        {
            get;
            private set;
        }



        [ShowInInspector, ReadOnly]
        public bool IsOnline
        {
            get; private set;
        }
        private void Awake()
        {
            instance = this;
            version=Application.version;
        }

        private void Update()
        {

        }
        [Button]
        public async void Get404() {
            await ExecuteOnlineRequest(() => onlineDatabaseConnection.Get404());
        }
        public async Task<bool> CheckPassword(string password)
        {
            try
            {
                HttpStatusCode status = await GetStatus(() => onlineDatabaseConnection.CheckPassword(password));
                return status == HttpStatusCode.OK;

            }
            catch 
            {
                OnDisconnected();
                return false;
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                HttpStatusCode status = await GetStatus(() => onlineDatabaseConnection.TestConnection());
                IsOnline = status==HttpStatusCode.Accepted|| status == HttpStatusCode.OK;
                if (IsOnline)
                {
                    OnConnected();
                }
                Debug.Log("Online: " + IsOnline);
            }
            catch (System.Exception)
            {
                IsOnline = false;
                Debug.Log("Online: " + IsOnline);
            }
            return IsOnline;
        }


        
        [Button]
        public async Task<LoginResponse> Login(string username, string password)
        {
            if (!IsOnline)
            {
                onConnectedCallbacks.Add(new Callback(async () => await Login(username, password), CallbackType.Once));
                return LoginLocally(username, password);
            }
            try
            {
                Debug.Log("Logging in");
                var result = await  GetStatus(() => onlineDatabaseConnection.TryLogin(username, password));
                var status = result;
                Debug.Log("Got result "+ status.ToString());
                switch (result)
                {
                    case HttpStatusCode.OK:
                        var user = onlineDatabaseConnection.user;
                        var localUser = new IDUser(user.id, username, password, user.email, user.role, user.settings);
                        localDatabaseConnection.InsertUser(localUser);
                        User = localDatabaseConnection.GetCurrentUser();
                        LoggedIn = true;
                        OnLoggedInOnline();
                        return LoginResponse.Success;
                    case HttpStatusCode.Forbidden:
                        return LoginResponse.WrongPassword;
                    case HttpStatusCode.Unauthorized:
                        return LoginResponse.UserUnknown;
                    case (HttpStatusCode)423:
                        localDatabaseConnection.SetFailedAttempts(username,1000);
                        var attempts=localDatabaseConnection.GetLoginAttempts(username);
                        return LoginResponse.ToManyFailedAttempts;
                    default:
                        Debug.LogError("UnexpectedStatusCode: " + status);
                        return LoginResponse.UnknownFail;
                }
            }
            catch (System.Exception e)
            {
                onConnectedCallbacks.Add(new Callback(async () => await Login(username, password), CallbackType.Once));
                OnDisconnected();

                return LoginLocally(username, password);

            }
        }
        public async Task<ServerResponse> GetMaxHighscore(string gameName)
        {
            if (!IsOnline) return new NotConnectedResponse();
            return await ExecuteOnlineRequest(()=> onlineDatabaseConnection.GetHighestScore(gameName));
        }
        public async Task<ServerResponse> GetRank(string gameName)
        {
            if (!IsOnline) return new NotConnectedResponse();
            return await ExecuteOnlineRequest(() => onlineDatabaseConnection.GetRank(gameName));
        }
        private async void OnLoggedInOnline()
        {
            await SyncSettings();
            await SyncTrainingsData();
            await SyncFeedback();
            onLoggedInCallbacks.CallCallbacks();
        }
        private void OnConnected()
        {
            onConnectedCallbacks.CallCallbacks();
        }
        private void OnDisconnected()
        {
            IsOnline = false;
            onDisconnectedCallbacks.CallCallbacks();
            StartTryingToConnect();
        }
        public void SubscribeOnConnected(Callback callback) 
        {
            onConnectedCallbacks.Add(callback);
        }
        public void SubscribeOnDisconnected(Callback callback)
        {
            onDisconnectedCallbacks.Add(callback);
        }
        private LoginResponse LoginLocally(string username, string password)
        {
            Debug.Log("Not Online Trying to login locally");
            IsOnline = false;
            LoginResponse localLoginResponse = localDatabaseConnection.Login(username, password);
            if (localLoginResponse == LoginResponse.Success)
            {
                User = localDatabaseConnection.GetCurrentUser();
            }
            return localLoginResponse;
        }

        public async Task<ServerResponse> AllowTherapistAccess(int therapistID)
        {
            try
            {
                return await onlineDatabaseConnection.AllowTherapistAccess(therapistID);
            }
            catch
            {
                OnDisconnected();
                return new NotConnectedResponse();
            }
        }
        public async Task<HttpStatusCode> RevokeTherapistAccess()
        {
            try
            {
                return await GetStatus(()=> onlineDatabaseConnection.RevokeTherapistAccess());
            }
            catch
            {
                OnDisconnected();
                return HttpStatusCode.NotFound;
            }
        }
        public async Task<bool> AllowedAccess()
        {
            try
            {
                return await GetContent<bool>(()=>  onlineDatabaseConnection.AllowedAccess());
            }
            catch(Exception ex)
            {
                OnDisconnected();
                return false;
            }
        }
        public async Task<string> GetTherapistName()
        {
            try
            {
                return await GetContent<string>(()=>onlineDatabaseConnection.GetTherapistName());
            }
            catch
            {
                OnDisconnected();
                return null;
            }
        }

        public async Task<bool> Logout()
        {
            if (!IsOnline) return true;
            try
            {
                return (await GetStatus(() => onlineDatabaseConnection.Logout()) == HttpStatusCode.NoContent);
            }
            catch
            {
                OnDisconnected();
                return true;
            }
        }
        public async Task<bool> DeleteSelf()
        {
            try
            {
                if ((await GetStatus(()=> onlineDatabaseConnection.DeleteSelf())==HttpStatusCode.NoContent))
                {
                    localDatabaseConnection.DeleteSelf();
                    await onlineDatabaseConnection.Logout();
                    return true;
                }
                return false;
            }
            catch
            {
                OnDisconnected();
                return false;
            }
        }
        public string GetEmail()
        {
            return localDatabaseConnection.GetCurrentUser().email;
        }
        public async Task<ServerResponse> SetEmail( string email)
        {
            if (!IsOnline)
            {
                return new NotConnectedResponse();
            }
            try
            {
                var setOnline = await ExecuteOnlineRequest(() => onlineDatabaseConnection.SetEmail(email));
                switch (setOnline)
                {
                    case StatusResponse status:
                        if (status.statusCode != HttpStatusCode.OK)
                        {
                            return setOnline;
                        }
                        localDatabaseConnection.SetEmail(email);
                        User = localDatabaseConnection.GetCurrentUser();
                        return status;
                    default:
                        throw new Exception("Not a status");
                }
            }
            catch
            {

                OnDisconnected();
                return new NotConnectedResponse();
            }

        }
        public async Task<ServerResponse> SetPassword(string password)
        {

            if (!IsOnline)
            {
                return new NotConnectedResponse();
            }
            try
            {
                var setOnline =  await ExecuteOnlineRequest(() => onlineDatabaseConnection.SetPassword(password));
                switch (setOnline)
                {
                    case StatusResponse status:
                        if ( status.statusCode!= HttpStatusCode.OK)
                        {
                            return setOnline;
                        }
                        localDatabaseConnection.SetPassword(password);
                        User = localDatabaseConnection.GetCurrentUser();
                        return status;
                    default:
                        throw new Exception("Not a status");
                }

            }
            catch 
            {
                OnDisconnected();
                return new NotConnectedResponse();
            }

        }

        public void SetConsent(bool v)
        {
            localDatabaseConnection.SetConsent(v);
        }
        public void ShowWelcomeScreen(bool v)
        {
            localDatabaseConnection.ShowWelcomeScreen(v);
        }
        private async void TryConnecting()
        {
            bool online = false;
            while (!online)
            {
                online = await TestConnection();
                Thread.Sleep(1000);
            }
        }
        [Button]
        public void Init()
        {
            LoggedIn = false;
            onConnectedCallbacks.Add(new Callback(async()=>await CheckVersion(), CallbackType.Once));
            StartTryingToConnect();
            localDatabaseConnection.InitAndCheckForOldDB();
        }

        private async Task CheckVersion()
        {
            var valid=await GetContent<bool>(() => onlineDatabaseConnection.CheckVersion(version));
            if (!valid)
            {
                onOldVersionCallbacks.CallCallbacks();
            }
        }

        public UserSettings GetDatabaseSettings()
        {
            return localDatabaseConnection.GetSettings();
        }
        public UserSettings GetSettings()
        {
            string settings = User.settings;
            if (settings == null) return new UserSettings();
            return JObject.Parse(settings).ToObject<UserSettings>();
        }
        public void SetSettings(UserSettings settings)
        {
            settings.lastChanged = DateTime.Now;
            localDatabaseConnection.SetSettings(settings);
            User = localDatabaseConnection.GetCurrentUser();
        }
        public async Task SyncSettings()
        {
            if (!IsOnline) return;
            try
            {
                var onlineSettings = await GetContent< UserSettings> (()=>onlineDatabaseConnection.GetSettings());
                var localSettings = localDatabaseConnection.GetSettings();

                if (onlineSettings.lastChanged == DateTime.MinValue)
                {
                    if (localSettings.lastChanged == DateTime.MinValue)
                    {
                        localSettings.lastChanged = DateTime.Now;
                        localDatabaseConnection.SetSettings(localSettings);
                    }
                    await ExecuteOnlineRequest(()=> onlineDatabaseConnection.SetSettings(localSettings));
                    User = localDatabaseConnection.GetCurrentUser();
                    return;
                }
                if (localSettings.lastChanged > onlineSettings.lastChanged)
                {
                    await ExecuteOnlineRequest(() => onlineDatabaseConnection.SetSettings(localSettings));
                }
                else
                {
                    localDatabaseConnection.SetSettings(onlineSettings);
                }
                User = localDatabaseConnection.GetCurrentUser();
            }
            catch
            {
                OnDisconnected();
            }

        }

        public Config GetOrAddConfig()
        {
            var config=localDatabaseConnection.GetConfig();
            if (config==null)
            {
                localDatabaseConnection.AddConfig();
            }
            return localDatabaseConnection.GetConfig();
             
        }

        public async Task SyncTrainingsData()
        {
            if (!IsOnline) return;
            try
            {
                long lastSync;
                Config config = GetOrAddConfig();

                lastSync = config.LastTrainingsDataSync;

                var onlineData = (await GetContent<List<OnlineTrainingsData>>(()=> onlineDatabaseConnection.GetStatisticsSince(lastSync))).Select(x => x.ToTrainingsData()).ToList();
                Debug.Log("Downloaded");
                var localData = localDatabaseConnection.GetStatisticsSince(lastSync);
                Debug.Log("Loaded Local data");
                await ExecuteOnlineRequest(()=>onlineDatabaseConnection.InsertStats(localData));
                Debug.Log("Uploaded");

                localDatabaseConnection.InsertTrainingsData(onlineData);
                localDatabaseConnection.SetLastTrainingsDataSync(DateTime.Now.Ticks);
            }
            catch
            {
                OnDisconnected();
            }
        }
        public async Task SyncFeedback()
        {
            if (!IsOnline) return;
            try
            {
                long lastSync;
                Config config = GetOrAddConfig();

                lastSync = config.LastFeedbackSync;
                Debug.Log($"Last feedback sync: {lastSync}");


                var onlineData = await GetContent<List<Feedback>>(() => onlineDatabaseConnection.GetFeedbacksSince(lastSync));
                Debug.Log("Downloaded feedback");
                var localData = localDatabaseConnection.GetFeedbackSince(lastSync);
                Debug.Log("Loaded Local feedback data");
                await ExecuteOnlineRequest(()=> onlineDatabaseConnection.InsertFeedbacks(localData));
                Debug.Log("Uploaded feedback");

                localDatabaseConnection.InsertFeedback(onlineData);
                localDatabaseConnection.SetFeedbackLastSync(DateTime.Now.Ticks);
            }
            catch
            {
                OnDisconnected();
            }
        }
        [Button]
        public List<TrainingsData> GetTrainingsData()
        {
            return localDatabaseConnection.GetStatistics();
        }
        public void AddTrainingsData(List<TrainingsData> trainingsData)
        {
            localDatabaseConnection.InsertTrainingsData(trainingsData);

        }
       
        public void AddTrainingsData(TrainingsData trainingsData)
        {
            localDatabaseConnection.InsertTrainingsData(trainingsData);
        }
        public List<Feedback> GetFeedback()
        {
            return localDatabaseConnection.GetFeedback();
        }
        public void InsertFeedback(Feedback feedback)
        {
            localDatabaseConnection.InsertFeedback(feedback);

        }
        private void StartTryingToConnect()
        {
            StopTryingToConnect();
            connectionThread = new Thread(TryConnecting);
            connectionThread.Start();
        }
        public async Task<ServerResponse> ResetPassword(string email)
        { 
            if (!IsOnline) return new NotConnectedResponse();
            try
            {
                return await ExecuteOnlineRequest(()=>onlineDatabaseConnection.ResetPassword(email));
                
            }
            catch
            {
                OnDisconnected();
                return new NotConnectedResponse();
            }
        }
        public async void DeleteSynchedTrainingsData()
        {
            Debug.Log("Deleting");
            await  onlineDatabaseConnection.DeleteStats();
            await onlineDatabaseConnection.DeleteFeedbacks();
            Debug.Log("Deleted Online");
            localDatabaseConnection.DeleteTrainingsdata();
            localDatabaseConnection.DeleteFeedback();

            Debug.Log("Deleted Offline");

        }

        [Button]
        public void StopTryingToConnect()
        {
            if (connectionThread != null)
            {
                connectionThread.Abort();
            }
        }
        private void OnDestroy()
        {
            Debug.Log("On Destroy");
            StopTryingToConnect();
            LogoutIfLoggedIn();
            localDatabaseConnection.Close();

        }

        private async void LogoutIfLoggedIn()
        {
            if (LoggedIn)
            {
                await Logout();
            }
        }
        private async Task<ServerResponse> ExecuteOnlineRequest(Func<Task<ServerResponse>> request)
        {
            try
            {
                var response = await request();
                switch (response)
                {
                    case StatusResponse statusResponse:
                        if (statusResponse.statusCode == HttpStatusCode.NotFound ||
                            statusResponse.statusCode == HttpStatusCode.Moved ||
                            statusResponse.statusCode == HttpStatusCode.MovedPermanently
                            )
                        {
                            onDeprecatedVersionCallbacks.CallCallbacks();
                            throw new Exception("Version likely deprecated");
                        }
                        break;
                    default:
                        break;
                }
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        private async Task<T> GetContent<T>(Func<Task<ServerResponse>> request)
        {
            var result =await ExecuteOnlineRequest(request);
            if (result is GenericResponse<T>)
            {
                return (result as GenericResponse<T>).content;
            }
            throw new Exception("Unexpected Response");
        }
        private async Task<HttpStatusCode> GetStatus(Func<Task<ServerResponse>> request)
        {
            var result = await ExecuteOnlineRequest(request);
            if (result is StatusResponse)
            {
                return (result as StatusResponse).statusCode;
            }
            throw new Exception("Unexpected Response");
        }
    }
}