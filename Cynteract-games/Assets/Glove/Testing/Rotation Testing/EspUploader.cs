using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using Cynteract.CGlove;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class EspUploader : MonoBehaviour
{
    public TextMeshProUGUI statusText, gloveText;
    public TextMeshProUGUI testButtonText;
    public string folderPath;
    Process process;
    public Button uploadButton, testButton;
    string output;
    private bool uploading, newUpload;

    // Start is called before the first frame update
    private void Awake()
    {
        uploadButton.onClick.AddListener(Upload);
        testButton.onClick.AddListener(SwitchToTestScene);
    }

    private void SwitchToTestScene()
    {
        SceneManager.LoadScene("RotationTestScene");
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

            if (!uploading)
            {

                UpdatePort();
            }
    }

    private async void UpdatePort()
    {
        var port = await GetPort();
        if (port=="")
        {
            gloveText.text = "No Glove Found";
            uploadButton.interactable = false;
            testButton.interactable = false;
            newUpload = false;

        }
        else
        {
            gloveText.text = $"Glove Found at port {port}";
            uploadButton.interactable = !uploading;
            if (newUpload)
            {
                if (!uploading)
                {
                    testButton.interactable = false;
                    testButtonText.text = "Please reconnect the Glove";
                }
            }
            else
            {
                testButton.interactable = !uploading;
            }
        }
        if (!newUpload)
        {
            testButtonText.text = "Testing";
        }
    }

    public async void Upload()
    {
        uploading = true;
        newUpload = true;
        uploadButton.interactable = false;
        testButton.gameObject.SetActive(false);
        uploadButton.gameObject.SetActive(false);
        testButton.interactable=false;

        folderPath = Application.dataPath;
        var logPath = folderPath + "\\log.txt";
        var path = folderPath + "\\test.bat";
        try
        {
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine("Path of .bat: " + path);
            }
            /*
            string port = await GetPort();
            if (port == "")
            {
                uploading = false;
                uploadButton.interactable = true;
                statusText.text = "";
                return;
            }
            string text = File.ReadAllText(path);
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine("Loaded .bat Text: " + text);
            }
            text = Regex.Replace(text, @"\bCOM\S*\b", port);
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine("Changed COM to: " + port);
            }
            File.WriteAllText(path, text);
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine("Replaced COM in .bat");
            }*/
            statusText.text = "Uploading...";
            await UploadFiles(folderPath, path);
            statusText.text = "Upload Finished";
            uploading = false;
            uploadButton.interactable = true;
            testButton.gameObject.SetActive(true);
            uploadButton.gameObject.SetActive(true);
        }
        catch (Exception e)
        {

            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine("\nEXCEPTION: " + e);
            }
        }



    }

    private static async Task<string> GetPort()
    {
        var scan = new Scan();
        var scanResult = await scan.ScanDevices();
        if (scanResult==null||scanResult.ids==null||scanResult.ids.Length==0)
        {
            return "";
        }
        var port = scanResult.ids[0];
        return port;
    }

    public Task<bool> UploadFiles(string folderPath, string path)
    {
        TaskCompletionSource<bool> taskCompletionSource=new TaskCompletionSource<bool>();
        Task.Factory.StartNew(() =>
        {
            var cmdPath = "";

            var parts=folderPath.Split('/');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i==0)
                {
                    parts[i] = $"{parts[i]}/";
                }
                else
                {
                    parts[i] = $"\"{parts[i]}\"/";
                }
                cmdPath += parts[i];
            }

            ExecuteCommand($"cd {cmdPath} &&" + $"{cmdPath}test.bat");
            taskCompletionSource.SetResult(true);
        });
        return taskCompletionSource.Task;
    }

     void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        var process = Process.Start(processInfo);
        string s="";
        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            s+=("output>>" + e.Data+"\n");
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            s += (("error>>" + e.Data+"\n"));
        process.BeginErrorReadLine();

        process.WaitForExit();
        s += $"Folderpath {folderPath}\n";
        s += $"Command {command}\n";
        try
        {

        File.WriteAllText(folderPath+"\\uploaderLog.txt", s);
        }
        catch (Exception e)
        {

            print(e.Message);
        }
        print("ExitCode:" + process.ExitCode);
        process.Close();
    }
}
