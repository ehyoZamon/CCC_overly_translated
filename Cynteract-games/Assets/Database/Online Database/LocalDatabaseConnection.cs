using Cynteract.Database;
using Cynteract.OnlineDatabase;
using Newtonsoft.Json.Linq;
using SqliteForUnity3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDatabaseConnection
{

    public ISQLiteConnection UserDatabaseConnection { get; private set; }


    public int userID;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user">User sent by database but with unhashed password</param>
    public void InsertUser(IDUser user)
    {

            this.userID = user.Id;
        SetFailedAttempts(user.username, 0);
        try
        {
            var hashedPassword = SecurePasswordHasher.Hash(user.password);
            UserDatabaseConnection.Insert(new IDUser(userID, user.username, hashedPassword, user.email, user.role, user.settings));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            var potBlockingUser = GetUser(user.username);
            if (potBlockingUser != null)
            {
                //user already exists with different id 
                //because the server is responsible for ids, the local id must be changed
                UpdateID(userID, potBlockingUser, user.password);

            }
            else
            {
                UpdateUser(user);
            }
        }
    }
    private void UpdateID(int newId, /*for user*/ IDUser user, string password)
    {
        UserDatabaseConnection.Execute($"UPDATE IDUser SET Id=\"{newId}\" , Password = \"{SecurePasswordHasher.Hash(password)}\" WHERE Username=\"{user.username}\"");
        UserDatabaseConnection.Execute($"UPDATE TrainingsData SET UserID=\"{newId}\" WHERE UserID=\"{user.id}\"");
        UserDatabaseConnection.Execute($"UPDATE Config SET UserID=\"{newId}\" WHERE UserID=\"{user.id}\"");

    }
    private void UpdateUser(IDUser user)
    {
        UserDatabaseConnection.Execute(
            $"UPDATE IDUser SET " +
            $"Username=\"{user.username}\"," +
            $"Password = \"{SecurePasswordHasher.Hash(user.password)}\"," +
            $"Role = \"{user.role}\"," +
            $"Email = \"{user.email}\"" +
            $"WHERE Id={userID}");
    }

    public IDUser GetCurrentUser()
    {
        return GetUser(userID);
    }
    public IDUser GetUser(string username)
    {
        return UserDatabaseConnection.Query<IDUser>($"SELECT * FROM IDUser WHERE Username = \"{username}\"").FirstOrDefault();
    }
    public LoginAttempts GetLoginAttempts(string username)
    {
        return UserDatabaseConnection.Query<LoginAttempts>($"SELECT * FROM LoginAttempts WHERE Username = \"{username}\"").FirstOrDefault();
    }
    public void AddFailedAttempt(string username)
    {
        UserDatabaseConnection.Execute($"UPDATE LoginAttempts SET FailedAttempts=FailedAttempts+1 WHERE Username = \"{username}\"");
    }
    public void AddLoginAttempts(string username)
    {
        try
        {
            UserDatabaseConnection.Insert(new LoginAttempts() { Username = username, FailedAttempts = 0 });
        }
        catch 
        {
            //already added
        }
        
    }
    public void SetFailedAttempts(string username, int attempts)
    {
        UserDatabaseConnection.Execute($"UPDATE LoginAttempts SET FailedAttempts={attempts} WHERE Username = \"{username}\"");
    }
    public void SetEmail(string email)
    {
        UserDatabaseConnection.Execute($"UPDATE IDUser SET Email = \"{email}\" WHERE Id={userID}");
    }
    public void SetPassword(string password)
    {
        UserDatabaseConnection.Execute($"UPDATE IDUser SET Password = \"{SecurePasswordHasher.Hash(password)}\" WHERE Id={userID}");
    }
    public IDUser GetUser(int userID)
    {
        return UserDatabaseConnection.Query<IDUser>($"SELECT * FROM IDUser WHERE Id = {userID}").FirstOrDefault();
    }
    public UserSettings GetSettings()
    {
        string settings = GetCurrentUser().settings;
        if (settings == null) return new UserSettings();
        return JObject.Parse(settings).ToObject<UserSettings>();
    }
    public void SetSettings(UserSettings localSettings)
    {
        SetSettings(JObject.FromObject(localSettings).ToString());
    }
    public void SetSettings(string localSettings)
    {
        Debug.Log(localSettings);
        UserDatabaseConnection.Execute($"UPDATE IDUser SET Settings = '{localSettings}' WHERE Id={userID}");
    }
    public LoginResponse Login(string username, string password)
    {
        var user = GetUser(username);
        AddLoginAttempts(username);
        if (GetLoginAttempts(username).FailedAttempts>10)
        {
            return LoginResponse.ToManyFailedAttempts;
        }
        if (user==null)
        {
            return LoginResponse.UserUnknown;
        }
        if (username!=""&&password!=""&&SecurePasswordHasher.Verify(password, user.password))
        {
            SetFailedAttempts(username, 0);
            userID = user.id;
            return LoginResponse.Success;
        }
        AddFailedAttempt(username);
        return LoginResponse.WrongPassword;
    }
    public List<TrainingsData> GetStatistics()
    {
        return UserDatabaseConnection.Query<TrainingsData>($"SELECT * from TrainingsData WHERE UserID={userID}");
    }
    public List<TrainingsData> GetStatisticsSince(long dateTime)
    {
        return UserDatabaseConnection.Query<TrainingsData>($"SELECT * from TrainingsData WHERE UserID={userID}").Where(x => x.Start > new DateTime (dateTime)).ToList();
    }
    public void InsertTrainingsData(TrainingsData trainingsData)
    {
        trainingsData.UserID = userID;
        UserDatabaseConnection.Insert(trainingsData);
    }

    public void DeleteTrainingsdata()
    {
        UserDatabaseConnection.Execute($"DELETE FROM TrainingsData WHERE UserID={userID}");

    }

    public void InsertTrainingsData(List<TrainingsData> trainingsData)
    {
        trainingsData.ForEach(item => item.UserID = userID);
        UserDatabaseConnection.InsertAll(trainingsData);
    }

    public void DeleteSelf()
    {
        DeleteTrainingsdata();
        UserDatabaseConnection.Execute($"DELETE FROM IDUser WHERE Id={userID}");

    }

    public void ShowWelcomeScreen(bool v)
    {
        if (v)
        {

            UserDatabaseConnection.Execute($"UPDATE Config SET ShowWelcomeScreen = 1  WHERE UserID={userID}");
        }
        else
        {
            UserDatabaseConnection.Execute($"UPDATE Config SET ShowWelcomeScreen = 0  WHERE UserID={userID}");
        }
    }

    public void SetConsent(bool v)
    {
        if (v)
        {
            UserDatabaseConnection.Execute($"UPDATE Config SET Consented = 1  WHERE UserID={userID}");
        }
        else
        {
            UserDatabaseConnection.Execute($"UPDATE Config SET Consented = 0  WHERE UserID={userID}");
        }
    }

    public Config GetConfig()
    {
        return UserDatabaseConnection.Query<Config>($"SELECT * FROM Config  WHERE UserID={userID}").FirstOrDefault();
    }
    public void SetLastTrainingsDataSync(long syncDateTime)
    {
        UserDatabaseConnection.Execute($"UPDATE Config SET LastTrainingsDataSync = '{syncDateTime}' WHERE UserID={userID}");
    }
    public void SetFeedbackLastSync(long syncDateTime)
    {
        UserDatabaseConnection.Execute($"UPDATE Config SET LastFeedbackSync = '{syncDateTime}' WHERE UserID={userID}");
    }
    public void Close()
    {
        if (UserDatabaseConnection != null)
        {
            UserDatabaseConnection.Close();
        }
    }

    public void InitAndCheckForOldDB()
    {
        try
        {
            Init();
        }
        catch (Exception)
        {
            Close();
            string format = "dd_MM_yyyy_HH_mm_ss";
            System.IO.File.Move(Application.persistentDataPath + "/" + "UserDatabase", Application.persistentDataPath + "/" + "UserDatabase_OLD_"+DateTime.Now.ToString(format));
            Init();
        }

    }
    public void Init()
    {
        var factory = new ConnectionFactory();
        UserDatabaseConnection = factory.Create(Path.Combine(Application.persistentDataPath, "UserDatabase"));
        if (CandyCoded.env.env.TryParseEnvironmentVariable("LOCAL_DATABASE_PASSWORD", out string password))
        {
            UserDatabaseConnection.Key(password);
        }
        else
        {
            throw new Exception("Environment key LOCAL_DATABASE_PASSWORD not found");
        }
        
        UserDatabaseConnection.CreateTable<IDUser>();
        UserDatabaseConnection.CreateTable<TrainingsData>();
        UserDatabaseConnection.CreateTable<Config>();
        UserDatabaseConnection.CreateTable<LoginAttempts>();
        UserDatabaseConnection.CreateTable<Feedback>();
    }
    public void AddConfig()
    {
        UserDatabaseConnection.Insert(new Config(userID, DateTime.MinValue, DateTime.MinValue, false, true));
    }

    public List<Feedback> GetEntireFeedback()
    {
        return UserDatabaseConnection.Query<Feedback>($"SELECT * from Feedback");
    }
    public List<Feedback> GetFeedback()
    {
        return UserDatabaseConnection.Query<Feedback>($"SELECT * from Feedback WHERE UserID={userID}");

    }
    public List<Feedback> GetFeedbackSince(long dateTime)
    {
        return UserDatabaseConnection.Query<Feedback>($"SELECT * from Feedback WHERE UserID={userID}").Where(x =>  x.Timestamp > dateTime).ToList();
    }
    public void InsertFeedback(Feedback feedback)
    {
        feedback.UserID = userID;
        UserDatabaseConnection.Insert(feedback);
    }

    public void DeleteFeedback()
    {
        UserDatabaseConnection.Execute($"DELETE FROM Feedback WHERE UserID={userID}");

    }

    public void InsertFeedback(List<Feedback> feedback)
    {
        feedback.ForEach(item => item.UserID = userID);
        UserDatabaseConnection.InsertAll(feedback);
    }


}
