using Sirenix.OdinInspector;
using SqliteForUnity3D;
using System;

[Serializable]
public class IDUser
{
    public int id;
    public string username;
    public string password;
    public string email;
    public string role;
    public string settings;
    public IDUser(int id, string username, string password, string email, string role, string settings)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        Settings = settings;
    }
    public IDUser()
    {

    }
    [ShowInInspector]
    [PrimaryKey]
    public int Id { get { return id; } set { id = value; } }
    [ShowInInspector]
    [Unique]
    public string Username { get { return username; } set { username = value; } }
    [ShowInInspector]
    public string Password { get { return password; } set { password = value; } }
    [ShowInInspector]
    public string Email { get { return email; } set { email = value; } }
    [ShowInInspector]
    public string Role { get { return role; } set { role = value; } }
    [ShowInInspector]
    public string Settings { get { return settings; } set { settings = value; } }

}

