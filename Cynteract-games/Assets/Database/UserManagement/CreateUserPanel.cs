using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cynteract.OnlineDatabase;
using System.Threading.Tasks;

public class CreateUserPanel : MonoBehaviour
{
    public Button createButton, cancelButton;
    public TextMeshProUGUI title;
    public TMP_InputField usernameField, passwordField;
    public string role;
    public void Init(string role, Func<User, Task> addUser)
    {
        this.role = role;
        title.text = "Create " + role;
        createButton.onClick.AddListener(async() => {
            await addUser(new User(usernameField.text, passwordField.text, role));
            Destroy(gameObject);
        });
        cancelButton.onClick.AddListener(() => Destroy(gameObject));
    }
    
}
