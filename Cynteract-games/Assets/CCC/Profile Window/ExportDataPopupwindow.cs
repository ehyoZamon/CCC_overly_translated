using Cynteract.Database;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class ExportDataPopupwindow : PopupWindow
    {
        public PasswordField oldPasswordField;
        public Button fhirButton, htmlButton, cancelButton;
        public Message message;
        Action<bool> callbackAction;


        protected override void OnInit()
        {
            message.Hide();
            fhirButton.onClick.AddListener(SaveAsFhir);
            htmlButton.onClick.AddListener(SaveAsHtml);
            cancelButton.onClick.AddListener(() => Close(false));
        }

        private async void SaveAsHtml()
        {
            if (await DatabaseManager.instance.CheckPassword(oldPasswordField.inputField.text))
            {
                FileBrowser.SetFilters(false, new FileBrowser.Filter("HTML Files", ".html"));
                string path = await GetPath("traingsdata.html");
                File.WriteAllText(path, CreateHtml());
                Close(true);
            }
            else
            {
                message.DisplayTranslated("The current password is wrong");
            }
        }

        private async void SaveAsFhir()
        {
            if (await DatabaseManager.instance.CheckPassword(oldPasswordField.inputField.text))
            {
                FileBrowser.SetFilters(false, new FileBrowser.Filter("Fhir Files", "fhir.json"));
                string path = await GetPath("traingsdata.fhir.json");
                File.WriteAllText(path, CreateFhir());
                Close(true);
            }
            else
            {
                message.DisplayTranslated("The current password is wrong");
            }
        }

        private string CreateFhir()
        {
            dynamic [] getTelekom()
            {
                if (DatabaseManager.instance.User.email!=null&& DatabaseManager.instance.User.email!="")
                {
                    return new[]
                            {
                                new
                                {
                                    system="email",
                                    value=DatabaseManager.instance.User.email??"",
                                    use="home"
                                }
                     };
                }
                return new dynamic[0];
            }
            var patient = new
            {
                resource = new
                {
                    resourceType = "Patient",
                    language = DatabaseManager.instance.GetSettings().language,
                    name = new[] {
                        new {
                            given = new[]
                                    {
                                DatabaseManager.instance.User.username
                            }
                        }
                    },

                    telecom = getTelekom()
                },
            };
            var data = DatabaseManager.instance.GetTrainingsData();
            var trainings = data.Select(training =>
            new
            {
                resourceType = "Bundle",
                timestamp = training.Start,
                entry = new dynamic[] {
                    new
                    {
                        resource =new {
                            resourceType="Observation",
                            code = new
                            {
                                text="Duration"
                            },

                            valuePeriod = new
                            {
                                start=training.Start,
                                end=training.Start+training.duration
                            }

                        },

                    },
                    new
                    {
                        resource = new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Game"
                            },
                            valueString=training.GameName
                        }
                    },
                    new
                    {
                        resource = new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Score"
                            },
                            valueDecimal=training.Score
                        }
                    },
                    new
                    {
                        resource = new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Hand Turns"
                            },
                            valueDecimal= training.movements.ContainsKey("Arm rotation")?training.movements["Arm rotation"]:0
                        }
                    },
                    new
                    {
                        resource = new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Hand Turns"
                            },
                            valueDecimal= training.movements.ContainsKey("Fist Grip")?training.movements["Fist Grip"]:0
                        }
                    },
                }
            });

            var trainingsBundle = new
            {
                resource = new
                {
                    resourceType = "Bundle",
                    entry = new[]
                    {
                        trainings
                    }
                }

            };
            var data1 = DatabaseManager.instance.GetFeedback();
            var feedbacks = data1.Select(feedback => new
            {
                resourceType = "Bundle",
                timestamp = feedback.timestamp,
                entry = new dynamic[]
                {
                    new
                    {
                        resource=new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Stars"
                            },
                            valueDecimal=feedback.stars
                        }
                    },
                    new
                    {
                        resource=new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Pain"
                            },
                            valueDecimal=feedback.pain
                        }
                    },
                    new
                    {
                        resource=new
                        {
                            resourceType="Observation",
                            code=new
                            {
                                text="Comments"
                            },
                            valueString=feedback.comments
                        }
                    }
                }
            });
            var feedbackBundle = new
            {
                resource = new
                {
                    resourceType = "Bundle",
                    entry = new[]
                    {
                        feedbacks
                    }
                }

            };
            var exportData = new
            {
                resourceType = "Bundle",
                timestamp = DateTime.Now,
                entry = new dynamic[]
                {
                    patient,
                    trainingsBundle,
                    feedbackBundle
                }
            };

            return JObject.FromObject(exportData).ToString();
        }

        public Task<string> GetPath(string initialFilename)
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
            FileBrowser.ShowSaveDialog(path => taskCompletionSource.SetResult(path.First()), () => taskCompletionSource.SetResult(null), FileBrowser.PickMode.Files, initialFilename: initialFilename);
            return taskCompletionSource.Task;
        }


        string CreateHtml()
        {
            string translate(string key)
            {
                return Lean.Localization.LeanLocalization.GetTranslationText(key);
            }
            var data = DatabaseManager.instance.GetTrainingsData();
            string trainingsData = "";
            foreach (var item in data)
            {
                trainingsData += $"<li>{translate("Start time")} : {item.Start}, {translate("Duration")}: {item.duration}, {translate("Game")}: {item.GameName},  {translate("Score")}: {item.Score}, {translate("Arm rotations")}: {(item.movements.ContainsKey("Arm rotation")?item.movements["Arm rotation"]:0)} {translate("Fist grips")}: {(item.movements.ContainsKey("Fist Grip") ? item.movements["Fist Grip"] : 0)}</li>\n";
            }
            var feedbacks = DatabaseManager.instance.GetFeedback();
            string feedbacksData = "";
            foreach (var item in feedbacks)
            {
                feedbacksData += $"<li>{translate("Date")}: {new DateTime(item.Timestamp)}, {(item.stars > 0 ? $"{translate("Stars")}: ({item.stars}/5)" :translate("No rating given")) }, {(item.pain > 0 ? $"{translate("Pain")}: ({item.pain}/5)" : translate("Pain not specified")) }, {translate("Comments")}: {item.comments} </li>\n";
            }

            string s =
                @$"
                <h1>{translate("Data of")} {DatabaseManager.instance.User.username}</h1>
                <p>{translate("Email")} {DatabaseManager.instance.User.email}</p>
                <p>{translate("Language")} {DatabaseManager.instance.GetSettings().language}</p>
                <h2>{translate("Trainings data")}</h2>
                <ul>
                  {trainingsData}
                </ul>
                <h2>Feedback</h2>
                <ul>
                  {feedbacksData}
                </ul>
                ";
            return s;
        }
        public void Close(bool value)
        {
            callbackAction(value);
            Close();
        }
        void CreateExcel(string path)
        {
            FileStream MyAddress = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            HSSFWorkbook MyWorkbook = new HSSFWorkbook();
            HSSFSheet Sheet01 = (HSSFSheet)MyWorkbook.CreateSheet("Trainingsdaten");
            var data = DatabaseManager.instance.GetTrainingsData();
            HSSFRow header = (HSSFRow)Sheet01.CreateRow(0);

            HSSFCell start = (HSSFCell)header.CreateCell(0);
            start.SetCellValue("Startzeit");

            HSSFCell duration = (HSSFCell)header.CreateCell(1);
            duration.SetCellValue("Dauer");

            HSSFCell game = (HSSFCell)header.CreateCell(2);
            game.SetCellValue("Spiel");

            HSSFCell points = (HSSFCell)header.CreateCell(3);
            points.SetCellValue("Punkte");

            HSSFCell armTurns = (HSSFCell)header.CreateCell(4);
            armTurns.SetCellValue("Armdrehungen");

            HSSFCell fistGrips = (HSSFCell)header.CreateCell(5);
            fistGrips.SetCellValue("Faustschlüsse");


            int i = 1;
            foreach (var item in data)
            {

                HSSFRow row = (HSSFRow)Sheet01.CreateRow(i);
                HSSFCell start1 = (HSSFCell)row.CreateCell(0);
                start1.SetCellValue(item.Start.ToString());

                HSSFCell duration1 = (HSSFCell)row.CreateCell(1);
                duration1.SetCellValue(item.duration.ToString());

                HSSFCell game1 = (HSSFCell)row.CreateCell(2);
                game1.SetCellValue(item.GameName);

                HSSFCell points1 = (HSSFCell)row.CreateCell(3);
                points1.SetCellValue(item.Score);

                HSSFCell armTurns1 = (HSSFCell)row.CreateCell(4);
                if (item.movements.ContainsKey("Arm rotation"))
                {
                    armTurns1.SetCellValue(item.movements["Arm rotation"]);
                }
                else
                {
                    armTurns1.SetCellValue(0);
                }
                HSSFCell fistGrips1 = (HSSFCell)row.CreateCell(5);
                if (item.movements.ContainsKey("Fist Grip"))
                {
                    fistGrips1.SetCellValue(item.movements["Fist Grip"]);
                }
                else
                {
                    fistGrips1.SetCellValue(0);
                }
                i++;
            }


            MyWorkbook.Write(MyAddress);

            MyWorkbook.Close();

        }

        protected override void OnClose()
        {
        }
        public void SubscribeOnClosed(Action<bool> callback)
        {
            this.callbackAction = callback;
        }
    }
}