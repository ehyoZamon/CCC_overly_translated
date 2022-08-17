using Cynteract.OnlineDatabase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
namespace Cynteract.Database {
    public class OnlineDatabaseConnection
    {
        private const string AuthServerUri = "https://account.cynteract.com";
        private const string ServerUri = "https://cloud.cynteract.com";
        //private const string AuthServerUri = "http://localhost:4000";
        //private const string ServerUri = "http://localhost:3000";
        static HttpClient client = new HttpClient();
        string accessToken, refreshToken;
        public IDUser user;
        public async Task<ServerResponse> TestConnection()
        {
            var response = await Get(ServerUri + "/");
            return response;
        }
        public async Task<ServerResponse> Get404()
        {
            var response = await Get(ServerUri + "/404");
            return response;
        }
        public async Task<ServerResponse> CheckPassword(string password)
        {
            var response = await Post(AuthServerUri + "/check-password", new { username = user.username, password = password }, false);
            return response;
        }

        public async Task<ServerResponse> TryLogin(string username, string password)
        {
            var response = await Post(AuthServerUri + "/login", new { username = username, password = password }, false);
            switch (response)
            {
                case DynamicJsonResponse content:
                    accessToken = content.content.accessToken;
                    refreshToken = content.content.refreshToken;
                    user = content.content.user.ToObject<IDUser>();
                    return new StatusResponse(HttpStatusCode.OK);
            }
            return response;
        }
        public async Task<ServerResponse> Logout()
        {
            var response = await ExecuteTokenedRequest(() => Delete(AuthServerUri + "/logout", new { token = refreshToken }));
            return response;
        }
        public async Task<ServerResponse> DeleteSelf()
        {
            var response = await ExecuteTokenedRequest(() => Delete(ServerUri + "/delete-self", new { }));
            return response;
        }
        private async Task<bool> RefreshToken()
        {
            var response = await Post(AuthServerUri + "/token", new { token = refreshToken }, false);
            switch (response)
            {
                case StatusResponse statusResponse:
                    return false;
                case DynamicJsonResponse content:
                    accessToken = content.content.accessToken;
                    return true;
            }
            return false;
        }

        public async Task<ServerResponse> GetTherapistName()
        {
            var path = ServerUri + "/patient/get-therapist";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<string>((string) content.content.therapist);
            }
            return response;
        }

        public async Task<ServerResponse> AllowedAccess()
        {
            var path = ServerUri + "/patient/get-allowed-access";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    GenericResponse<bool> genericResponse = new GenericResponse<bool>((bool)content.content.allowed);
                    return genericResponse;
            }
            return response;
        }

        public async Task<ServerResponse> GetAllUsers()
        {
            var path = ServerUri + "/admin/get-users";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<IDUser>>( (content.content.users as JArray).ToObject<List<IDUser>>());
            } 
            return response;
        }

        public async Task<ServerResponse> UpdateUser(IDUser user)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/update", new { user }));
            return response;
        }

        public async Task<ServerResponse> CreatePatient(IDUser therapist, User patient)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/admin/create-patient", new { therapist, patient }));
            return response;
        }
        public async Task<ServerResponse> GetPatients(IDUser therapist)
        {
            var path = ServerUri + "/admin/get-patients";
            var response = await ExecuteTokenedRequest(() => Post(path, therapist));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<IDUser>>((content.content.patients as JArray).ToObject<List<IDUser>>());
            }
            return response;
        }

        public async Task<ServerResponse> GetHighestScore(string gameName)
        {
            var path = ServerUri + $"/get-highest-score/{gameName}";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<int>(((int)content.content.score));
            }
            return response;
        }
        public async Task<ServerResponse> GetRank(string gameName)
        {
            var path = ServerUri + $"/patient/get-rank/{gameName}";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<int>(((int)content.content.rank));
            }
            return response;
        }

        public async Task<ServerResponse> GetTherapistPatients()
        {
            var path = ServerUri + "/admin/get-therapist-patients";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<dynamic>> ((content.content.patients as JArray).ToObject<List<dynamic>>());
            }
            return response;
        }
        public async Task<ServerResponse> InsertUser(User user)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/admin/create-user", user));
            return response;
        }
        public async Task<ServerResponse> DeleteUser(string name)
        {
            var response = await ExecuteTokenedRequest(() => Delete(ServerUri + "/admin/delete-user", new { username = name }));
            return response;
        }
        public async Task<ServerResponse> GetSelf()
        {
            var path = ServerUri + "/patient/get-self";
            Debug.Log(path);
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<IDUser>( content.content.user);
            }
            return response;
        }
        public async Task<ServerResponse> SetEmail(string email)
        {
            var response = await ExecuteTokenedRequest(() => Put(AuthServerUri + "/patient/set-email", new {email}));
            return response;
        }

        public async Task<ServerResponse> ResetPassword(string email)
        {
            var path = AuthServerUri + "/send-email";
            var response = await ExecuteTokenedRequest(() => Post(path,new { email}));
            return response;
        }

        public async Task<ServerResponse> SetPassword(string password)
        {
            var response = await ExecuteTokenedRequest(() => Put(AuthServerUri + "/patient/set-password", new { password }));
            return response;
        }
        public async Task<ServerResponse> SetSettings(string settings)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/set-settings", new { settings }));
            return response;
        }
        public async Task<ServerResponse> SetSettings(UserSettings settings)
        {
            return await SetSettings(JObject.FromObject(settings).ToString());
        }
        public async Task<ServerResponse> GetSettings()
        {
            var path = ServerUri + "/patient/get-settings";
            Debug.Log(path);
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {

                case DynamicJsonResponse content:
                    string settings = content.content.settings;
                    UserSettings userSettings;

                    if (settings!=null)
                    {
                        Debug.Log(settings);
                        JObject jObject = JObject.Parse(settings);
                        userSettings= jObject.ToObject<UserSettings>();
                    }
                    else
                    {
                        userSettings = new UserSettings();
                    }
                    return new GenericResponse<UserSettings>(userSettings);
            }
            return response;
        }
        public async Task<ServerResponse> GetStatistics()
        {
            var path = ServerUri + "/patient/get-stats";
            Debug.Log(path);
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<OnlineTrainingsData>>((content.content.stats as JArray).ToObject<List<OnlineTrainingsData>>());
            }
            return response;
        }
        public async Task<ServerResponse> GetStatisticsSince(long dateTime)
        {
            var path = ServerUri + "/patient/get-stats-since";
            var response = await ExecuteTokenedRequest(() => Post(path, new { timestamp = dateTime }));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<OnlineTrainingsData>>((content.content.stats as JArray).ToObject<List<OnlineTrainingsData>>());
            }
            return response;
        }
        public async Task<ServerResponse> InsertStats(List<TrainingsData> statistics)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/insert-stats", new { stats = statistics.Select(x => new OnlineTrainingsData(x)).ToArray() }));
            return response;
        }
        public async Task<ServerResponse> DeleteStats()
        {
            var response = await ExecuteTokenedRequest(() => Delete(ServerUri + "/patient/delete-stats", new { }));
            return response;
        }
        public async Task<ServerResponse> GetFeedbacks()
        {
            var path = ServerUri + "/patient/get-feedbacks";
            var response = await ExecuteTokenedRequest(() => Get(path));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<Feedback>>((content.content.feedbacks as JArray).ToObject<List<Feedback>>());
            }
            return response;
        }
        public async Task<ServerResponse> GetFeedbacksSince(long dateTime)
        {
            var path = ServerUri + "/patient/get-feedbacks-since";
            var response = await ExecuteTokenedRequest(() => Post(path, new { timestamp = dateTime }));
            switch (response)
            {
                case DynamicJsonResponse content:
                    return new GenericResponse<List<Feedback>> ((content.content.feedbacks as JArray).ToObject<List<Feedback>>());
            }
            return response;
        }

        public async Task<ServerResponse> CheckVersion(string version)
        {
            var path = ServerUri + "/check-version/"+version;
            var response = await Get(path,false);
            switch (response)
            {
                case DynamicJsonResponse content:
                    GenericResponse<bool> genericResponse = new GenericResponse<bool>((bool)content.content.valid);
                    return genericResponse;
            }
            return response;
        }

        public async Task<ServerResponse> InsertFeedbacks(List<Feedback> feedbacks)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/insert-feedbacks", new { feedbacks= feedbacks.ToArray() }));
            return response;
        }
        public async Task<ServerResponse> DeleteFeedbacks()
        {
            var response = await ExecuteTokenedRequest(() => Delete(ServerUri + "/patient/delete-feedbacks", new { }));
            return response;
        }
        public async Task<ServerResponse> AllowTherapistAccess(int therapistID)
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/allow-therapist-access-by-id", new { therapistID }));
            return response;
        }
        public async Task<ServerResponse> RevokeTherapistAccess()
        {
            var response = await ExecuteTokenedRequest(() => Put(ServerUri + "/patient/revoke-therapist-access",new { }));
            return response;
        }
        public async Task<ServerResponse> ExecuteTokenedRequest(Func<Task<ServerResponse>> request)
        {
            var response = await request();
            switch (response)
            {
                case StatusResponse statusResponse:
                    if (statusResponse.statusCode == HttpStatusCode.Forbidden)
                    {
                        if (await RefreshToken())
                        {
                            return await request();
                        }
                    }
                    return new StatusResponse(statusResponse.statusCode);
                case DynamicJsonResponse content:
                    return content;
            }
            return response;
        }
        public async Task<string> GetA()
        {
            var response = await Get(ServerUri + "/a",false);
            switch (response)
            {
                case DynamicJsonResponse dynamicJsonResponse:
                    return dynamicJsonResponse.content.a;
                default:
                    break;
            }
            return response.ToString();
        }
        public async Task<string> GetB()
        {
            var response = await Get(ServerUri + "/b", false);
            switch (response)
            {
                case DynamicJsonResponse dynamicJsonResponse:
                    return dynamicJsonResponse.content.b;
                default:
                    break;
            }
            return response.ToString();
        }
        public async Task<ServerResponse> Get(string path, bool includeToken = true)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, path))
            {
                if (includeToken)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new StatusResponse(response.StatusCode);
                }
                try
                {
                    dynamic content = await ToJObject(response.Content);
                    return new DynamicJsonResponse(content);
                }
                catch (Exception)
                {

                    return new StatusResponse(response.StatusCode);
                }

            }
        }
        public async Task<ServerResponse> Put(string path, object content, bool includeToken = true)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, path))
            {
                if (includeToken)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                requestMessage.Content = GetContent(content);
                var response = await client.SendAsync(requestMessage);
                Debug.Log(response);
                return new StatusResponse(response.StatusCode);
            }
        }

        public async Task<ServerResponse> Delete(string path, object content, bool includeToken = true)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, path))
            {
                if (includeToken)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                requestMessage.Content = GetContent(content);
                var response = await client.SendAsync(requestMessage);
                Debug.Log(response);
                return new StatusResponse(response.StatusCode);
            }
        }
        public async Task<ServerResponse> Post(string path, object content, bool includeToken = true)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, path))
            {
                if (includeToken)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                requestMessage.Content = GetContent(content);
                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new StatusResponse(response.StatusCode);
                }
                try
                {
                    dynamic recievedContent = await ToJObject(response.Content);
                    return new DynamicJsonResponse(recievedContent);
                }
                catch (Exception)
                {

                    return new StatusResponse(response.StatusCode);
                }
            }
        }

        private static StringContent GetContent(object content)
        {
            return new StringContent(JObject.FromObject(content).ToString(), System.Text.Encoding.UTF8, "application/json");
        }
        private static async Task<JObject> ToJObject(HttpContent content)
        {
            string jsonString = await content.ReadAsStringAsync();
            Debug.Log(jsonString);
            return JObject.Parse(jsonString);
        }
    }
}