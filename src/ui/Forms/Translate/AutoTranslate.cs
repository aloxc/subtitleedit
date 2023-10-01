﻿using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Translate;
using Nikse.SubtitleEdit.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms.Translate
{
    public sealed partial class AutoTranslate : Form
    {
        public Subtitle TranslatedSubtitle { get; }
        private readonly Subtitle _subtitle;
        private readonly Encoding _encoding;
        private IAutoTranslator _autoTranslator;
        private List<IAutoTranslator> _autoTranslatorEngines;
        private int _translationProgressIndex = -1;
        private bool _translationProgressDirty = true;
        private bool _breakTranslation;
        private Process _processNllbServe;
        private Process _processNllbApi;
        private Process _processLibreTranslate;

        public AutoTranslate(Subtitle subtitle, Subtitle selectedLines, string title, Encoding encoding)
        {
            UiUtil.PreInitialize(this);
            InitializeComponent();
            UiUtil.FixFonts(this);

            Text = LanguageSettings.Current.GoogleTranslate.Title;
            buttonTranslate.Text = LanguageSettings.Current.GoogleTranslate.Translate;
            labelPleaseWait.Text = LanguageSettings.Current.GoogleTranslate.PleaseWait;
            buttonOK.Text = LanguageSettings.Current.General.Ok;
            buttonCancel.Text = LanguageSettings.Current.General.Cancel;
            subtitleListViewSource.InitializeLanguage(LanguageSettings.Current.General, Configuration.Settings);
            subtitleListViewTarget.InitializeLanguage(LanguageSettings.Current.General, Configuration.Settings);
            subtitleListViewSource.HideColumn(SubtitleListView.SubtitleColumn.CharactersPerSeconds);
            subtitleListViewSource.HideColumn(SubtitleListView.SubtitleColumn.WordsPerMinute);
            subtitleListViewTarget.HideColumn(SubtitleListView.SubtitleColumn.CharactersPerSeconds);
            subtitleListViewTarget.HideColumn(SubtitleListView.SubtitleColumn.WordsPerMinute);
            UiUtil.InitializeSubtitleFont(subtitleListViewSource);
            UiUtil.InitializeSubtitleFont(subtitleListViewTarget);
            subtitleListViewSource.HideColumn(SubtitleListView.SubtitleColumn.End);
            subtitleListViewSource.HideColumn(SubtitleListView.SubtitleColumn.Gap);
            subtitleListViewTarget.HideColumn(SubtitleListView.SubtitleColumn.End);
            subtitleListViewTarget.HideColumn(SubtitleListView.SubtitleColumn.Gap);
            subtitleListViewSource.AutoSizeColumns();
            subtitleListViewSource.AutoSizeColumns();
            UiUtil.FixLargeFonts(this, buttonOK);
            ActiveControl = buttonTranslate;

            if (!string.IsNullOrEmpty(title))
            {
                Text = title;
            }

            _subtitle = new Subtitle(subtitle);
            _encoding = encoding;

            InitializeAutoTranslatorEngines();

            nikseComboBoxUrl.UsePopupWindow = true;

            labelPleaseWait.Visible = false;
            progressBar1.Visible = false;

            if (selectedLines != null)
            {
                TranslatedSubtitle = new Subtitle(selectedLines);
                TranslatedSubtitle.Renumber();
                subtitleListViewTarget.Fill(TranslatedSubtitle);
            }
            else
            {
                TranslatedSubtitle = new Subtitle(_subtitle);
                foreach (var paragraph in TranslatedSubtitle.Paragraphs)
                {
                    paragraph.Text = string.Empty;
                }
            }

            subtitleListViewSource.Fill(_subtitle);
            AutoTranslate_Resize(null, null);
            UpdateTranslation();
        }

        private void InitializeAutoTranslatorEngines()
        {
            _autoTranslatorEngines = new List<IAutoTranslator>
            {
                new NoLanguageLeftBehindServe(),
                new NoLanguageLeftBehindApi(),
                new LibreTranslate(),
            };

            nikseComboBoxEngine.Items.Clear();
            nikseComboBoxEngine.Items.AddRange(_autoTranslatorEngines.Select(p => p.Name).ToArray<object>());

            if (!string.IsNullOrEmpty(Configuration.Settings.Tools.AutoTranslateLastName))
            {
                var lastEngine = _autoTranslatorEngines.FirstOrDefault(p => p.Name == Configuration.Settings.Tools.AutoTranslateLastName);
                if (lastEngine != null)
                {
                    _autoTranslator = lastEngine;
                    nikseComboBoxEngine.SelectedIndex = _autoTranslatorEngines.IndexOf(lastEngine);
                }
            }

            if (nikseComboBoxEngine.SelectedIndex < 0)
            {
                _autoTranslator = _autoTranslatorEngines[0];
                nikseComboBoxEngine.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Tools.AutoTranslateLastUrl))
            {
                nikseComboBoxUrl.SelectedText = Configuration.Settings.Tools.AutoTranslateLastUrl;
            }
        }

        private void SetAutoTranslatorEngine()
        {
            _autoTranslator = GetCurrentEngine();
            linkLabelPoweredBy.Text = string.Format(LanguageSettings.Current.GoogleTranslate.PoweredByX, _autoTranslator.Name);
            var engineType = _autoTranslator.GetType();

            if (engineType == typeof(NoLanguageLeftBehindServe))
            {
                FillUrls(new List<string>
                {
                    Configuration.Settings.Tools.AutoTranslateNllbServeUrl,
                    "http://127.0.0.1:6060/",
                    "http://192.168.8.127:6060/",
                });

                return;
            }

            if (engineType == typeof(NoLanguageLeftBehindApi))
            {
                FillUrls(new List<string>
                {
                    Configuration.Settings.Tools.AutoTranslateNllbApiUrl,
                    "http://localhost:7860/api/v2/",
                    "https://winstxnhdw-nllb-api.hf.space/api/v2/",
                });
                return;
            }

            if (engineType == typeof(LibreTranslate))
            {
                FillUrls(new List<string>
                {
                    Configuration.Settings.Tools.AutoTranslateLibreUrl,
                    "http://localhost:5000/",
                    "https://libretranslate.com/",
                });

                return;
            }

            throw new Exception($"Engine {_autoTranslator.Name} not handled!");
        }

        private void FillUrls(List<string> list)
        {
            nikseComboBoxUrl.Items.Clear();
            foreach (var url in list.Distinct())
            {
                if (!string.IsNullOrEmpty(url))
                {
                    nikseComboBoxUrl.Items.Add(url.TrimEnd('/') + "/");
                }
            }

            nikseComboBoxUrl.SelectedIndex = 0;
            nikseComboBoxUrl.Visible = true;
            labelUrl.Visible = true;
        }

        private void SetAutoTranslatorUrl(string url)
        {
            var engine = GetCurrentEngine();
            var engineType = engine.GetType();

            if (engineType == typeof(NoLanguageLeftBehindApi))
            {
                Configuration.Settings.Tools.AutoTranslateNllbApiUrl = url;
                return;
            }

            if (engineType == typeof(NoLanguageLeftBehindServe))
            {
                Configuration.Settings.Tools.AutoTranslateNllbServeUrl = url;
                return;
            }

            if (engineType == typeof(LibreTranslate))
            {
                Configuration.Settings.Tools.AutoTranslateLibreUrl = url;
                return;
            }

            throw new Exception($"Engine {engine.Name} not handled!");
        }

        private void SetupLanguageSettings()
        {
            FillComboWithLanguages(comboBoxSource, _autoTranslator.GetSupportedSourceLanguages());
            var sourceLanguageIsoCode = EvaluateDefaultSourceLanguageCode(_encoding, _subtitle);
            SelectLanguageCode(comboBoxSource, sourceLanguageIsoCode);

            FillComboWithLanguages(comboBoxTarget, _autoTranslator.GetSupportedTargetLanguages());
            var targetLanguageIsoCode = EvaluateDefaultTargetLanguageCode(sourceLanguageIsoCode);
            SelectLanguageCode(comboBoxTarget, targetLanguageIsoCode);
        }

        public static void SelectLanguageCode(NikseComboBox comboBox, string languageIsoCode)
        {
            var i = 0;
            var threeLetterLanguageCode = Iso639Dash2LanguageCode.GetThreeLetterCodeFromTwoLetterCode(languageIsoCode);
            foreach (TranslationPair item in comboBox.Items)
            {
                if (languageIsoCode.Length == 2 && item.Code == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
                else if (item.Code.StartsWith(threeLetterLanguageCode) || item.Code == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }

                i++;
            }

            if (comboBox.SelectedIndex < 0 && comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        public static void FillComboWithLanguages(NikseComboBox comboBox, IEnumerable<TranslationPair> languages)
        {
            comboBox.Items.Clear();
            foreach (var language in languages)
            {
                comboBox.Items.Add(language);
            }
        }

        public static string EvaluateDefaultSourceLanguageCode(Encoding encoding, Subtitle subtitle)
        {
            var defaultSourceLanguageCode = LanguageAutoDetect.AutoDetectGoogleLanguage(encoding); // Guess language via encoding
            if (string.IsNullOrEmpty(defaultSourceLanguageCode))
            {
                defaultSourceLanguageCode = LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle); // Guess language based on subtitle contents
            }

            return defaultSourceLanguageCode;
        }

        public static string EvaluateDefaultTargetLanguageCode(string defaultSourceLanguage)
        {
            var installedLanguages = new List<string>();
            foreach (InputLanguage language in InputLanguage.InstalledInputLanguages)
            {
                var iso639 = Iso639Dash2LanguageCode.GetTwoLetterCodeFromEnglishName(language.LayoutName);
                if (!string.IsNullOrEmpty(iso639) && !installedLanguages.Contains(iso639))
                {
                    installedLanguages.Add(iso639.ToLowerInvariant());
                }
            }

            var uiCultureTargetLanguage = Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage;
            if (uiCultureTargetLanguage == defaultSourceLanguage)
            {
                foreach (var s in Utilities.GetDictionaryLanguages())
                {
                    var temp = s.Replace("[", string.Empty).Replace("]", string.Empty);
                    if (temp.Length > 4)
                    {
                        temp = temp.Substring(temp.Length - 5, 2).ToLowerInvariant();
                        if (temp != defaultSourceLanguage && installedLanguages.Any(p => p.Contains(temp)))
                        {
                            uiCultureTargetLanguage = temp;
                            break;
                        }
                    }
                }
            }

            if (uiCultureTargetLanguage == defaultSourceLanguage)
            {
                foreach (var language in installedLanguages)
                {
                    if (language != defaultSourceLanguage)
                    {
                        uiCultureTargetLanguage = language;
                        break;
                    }
                }
            }

            if (uiCultureTargetLanguage == defaultSourceLanguage)
            {
                var name = CultureInfo.CurrentCulture.Name;
                if (name.Length > 2)
                {
                    name = name.Remove(0, name.Length - 2);
                }
                var iso = IsoCountryCodes.ThreeToTwoLetterLookup.FirstOrDefault(p => p.Value == name);
                if (!iso.Equals(default(KeyValuePair<string, string>)))
                {
                    var iso639 = Iso639Dash2LanguageCode.GetTwoLetterCodeFromThreeLetterCode(iso.Key);
                    if (!string.IsNullOrEmpty(iso639))
                    {
                        uiCultureTargetLanguage = iso639;
                    }
                }
            }

            // Set target language to something different than source language
            if (uiCultureTargetLanguage == defaultSourceLanguage && defaultSourceLanguage == "en")
            {
                uiCultureTargetLanguage = "es";
            }
            else if (uiCultureTargetLanguage == defaultSourceLanguage)
            {
                uiCultureTargetLanguage = "en";
            }

            return uiCultureTargetLanguage;
        }

        private void AutoTranslate_Resize(object sender, EventArgs e)
        {
            var width = (Width / 2) - (subtitleListViewSource.Left * 3) + 19;
            subtitleListViewSource.Width = width;
            subtitleListViewTarget.Width = width;

            var height = Height - (subtitleListViewSource.Top + buttonTranslate.Height + 60);
            subtitleListViewSource.Height = height;
            subtitleListViewTarget.Height = height;

            comboBoxSource.Left = subtitleListViewSource.Left + (subtitleListViewSource.Width - comboBoxSource.Width);
            labelSource.Left = comboBoxSource.Left - 5 - labelSource.Width;

            subtitleListViewTarget.Left = width + (subtitleListViewSource.Left * 2);
            subtitleListViewTarget.Width = Width - subtitleListViewTarget.Left - 32;
            labelTarget.Left = subtitleListViewTarget.Left;
            comboBoxTarget.Left = labelTarget.Left + labelTarget.Width + 5;
            buttonTranslate.Left = comboBoxTarget.Left + comboBoxTarget.Width + 9;
            labelPleaseWait.Left = buttonTranslate.Left + buttonTranslate.Width + 9;
            progressBar1.Left = labelPleaseWait.Left;
            progressBar1.Width = subtitleListViewTarget.Width - (progressBar1.Left - subtitleListViewTarget.Left);
        }

        private async void buttonTranslate_Click(object sender, EventArgs e)
        {
            if (buttonTranslate.Text == LanguageSettings.Current.General.Cancel)
            {
                buttonTranslate.Enabled = false;
                buttonOK.Enabled = true;
                buttonCancel.Enabled = true;
                _breakTranslation = true;
                Application.DoEvents();
                buttonOK.Refresh();
                return;
            }

            _autoTranslator = GetCurrentEngine();
            var engineType = _autoTranslator.GetType();
            if (_processNllbServe == null &&
                Configuration.Settings.Tools.AutoTranslateNllbServeAutoStart &&
                engineType == typeof(NoLanguageLeftBehindServe))
            {
                ShowInfo($"Starting {_autoTranslator.Name} web server...");
                _processNllbServe = StartNoLanguageLeftBehindServe();
                return;
            }

            if (_processNllbApi == null &&
                Configuration.Settings.Tools.AutoTranslateNllbApiAutoStart &&
                engineType == typeof(NoLanguageLeftBehindApi))
            {
                ShowInfo($"Starting {_autoTranslator.Name} web server...");
                _processNllbApi = StartNoLanguageLeftBehindApi();
                return;
            }

            if (_processLibreTranslate == null &&
                Configuration.Settings.Tools.AutoTranslateLibreAutoStart &&
                engineType == typeof(LibreTranslate))
            {
                ShowInfo($"Starting {_autoTranslator.Name} web server...");
                _processLibreTranslate = StartLibreTranslate();
                return;
            }

            buttonOK.Enabled = false;
            buttonCancel.Enabled = false;
            _breakTranslation = false;
            buttonTranslate.Text = LanguageSettings.Current.General.Cancel;

            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = TranslatedSubtitle.Paragraphs.Count;
            progressBar1.Visible = true;
            labelPleaseWait.Visible = true;

            _autoTranslator.Initialize();


            var timerUpdate = new Timer();
            timerUpdate.Interval = 1500;
            timerUpdate.Tick += TimerUpdate_Tick;
            timerUpdate.Start();
            var linesTranslate = 0;

            if (comboBoxSource.SelectedItem is TranslationPair source &&
                comboBoxTarget.SelectedItem is TranslationPair target)
            {
                try
                {


                    var start = subtitleListViewTarget.SelectedIndex >= 0 ? subtitleListViewTarget.SelectedIndex : 0;
                    var index = start;
                    while (index < _subtitle.Paragraphs.Count)
                    {
                        var p = _subtitle.Paragraphs[index];

                        var mergeCount = 0;
                        var allItalic = false;
                        var allBold = false;

                        var text = string.Empty;
                        if (MergeWithThreeNext(_subtitle, index, source.Code))
                        {
                            mergeCount = 3;
                            allItalic = HasAllLinesTag(_subtitle, index, mergeCount, "i");
                            allBold = HasAllLinesTag(_subtitle, index, mergeCount, "b");
                            text = MergeLines(_subtitle, index, mergeCount, allItalic, allBold);
                        }
                        else if (MergeWithTwoNext(_subtitle, index, source.Code))
                        {
                            mergeCount = 2;
                            allItalic = HasAllLinesTag(_subtitle, index, mergeCount, "i");
                            allBold = HasAllLinesTag(_subtitle, index, mergeCount, "b");
                            text = MergeLines(_subtitle, index, mergeCount, allItalic, allBold);
                        }
                        else if (MergeWithNext(_subtitle, index, source.Code))
                        {
                            mergeCount = 1;
                            allItalic = HasAllLinesTag(_subtitle, index, mergeCount, "i");
                            allBold = HasAllLinesTag(_subtitle, index, mergeCount, "b");
                            text = MergeLines(_subtitle, index, mergeCount, allItalic, allBold);
                        }

                        if (mergeCount > 0)
                        {
                            var mergedTranslation = await _autoTranslator.Translate(text, source.Code, target.Code);
                            var result = SplitResult(mergedTranslation.SplitToLines(), mergeCount, source.Code);
                            if (allItalic)
                            {
                                for (var k = 0; k < result.Count; k++)
                                {
                                    result[k] = "<i>" + result[k] + "</i>";
                                }
                            }

                            if (allBold)
                            {
                                for (var k = 0; k < result.Count; k++)
                                {
                                    result[k] = "<b>" + result[k] + "</b>";
                                }
                            }

                            if (result.Count == mergeCount + 1 && result.All(t => !string.IsNullOrEmpty(t)))
                            {
                                foreach (var line in result)
                                {
                                    TranslatedSubtitle.Paragraphs[index].Text = line;
                                    index++;
                                    linesTranslate++;
                                }

                                continue;
                            }
                        }

                        var translation = await _autoTranslator.Translate(p.Text, source.Code, target.Code);
                        translation = translation
                            .Replace("<br />", Environment.NewLine)
                            .Replace("<br/>", Environment.NewLine);
                        TranslatedSubtitle.Paragraphs[index].Text = Utilities.AutoBreakLine(translation);
                        linesTranslate++;

                        _translationProgressIndex = index;
                        _translationProgressDirty = true;
                        progressBar1.Value = index;
                        index++;

                        Application.DoEvents();
                        if (_breakTranslation)
                        {
                            break;
                        }
                    }

                }
                catch (Exception exception)
                {
                    SeLogger.Error(exception);
                    if (linesTranslate == 0)
                    {
                        if (engineType == typeof(NoLanguageLeftBehindApi) || engineType == typeof(NoLanguageLeftBehindServe))
                        {
                            var dr = MessageBox.Show($"Facebook NLLB via {_autoTranslator.Name} requires an API running locally!" + Environment.NewLine
                                                                                                                                  + Environment.NewLine
                                                                                                                                  + "Read more?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                UiUtil.ShowHelp("#translation");
                            }
                        }
                        else if (engineType == typeof(LibreTranslate))
                        {
                            var dr = MessageBox.Show($"{_autoTranslator.Name} requires an API running locally!" + Environment.NewLine
                                                                     + Environment.NewLine
                                                                     + "Read more?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                UiUtil.ShowHelp("#translation");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, MessageBoxIcon.Error);
                    }
                }
            }

            timerUpdate.Stop();

            progressBar1.Visible = false;
            labelPleaseWait.Visible = false;
            buttonOK.Enabled = true;
            buttonCancel.Enabled = true;
            _breakTranslation = false;
            buttonTranslate.Enabled = true;
            buttonTranslate.Text = LanguageSettings.Current.GoogleTranslate.Translate;

            timerUpdate.Dispose();
            _translationProgressDirty = true;

            UpdateTranslation();

            buttonOK.Focus();
        }

        private void ShowInfo(string s)
        {
            labelInfo.Left = subtitleListViewTarget.Left;
            labelInfo.Text = s;
            labelInfo.Visible = true;
            labelInfo.Refresh();
            TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(5000), () => labelInfo.Visible = false);
        }

        private Process StartNoLanguageLeftBehindServe()
        {
            var modelName = Configuration.Settings.Tools.AutoTranslateNllbServeModel;
            var arguments = string.IsNullOrEmpty(modelName)
                ? string.Empty
                : $"-mi {modelName}";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("nllb-serve", arguments)
                {
                    UseShellExecute = false,
                }
            };

            process.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
            process.StartInfo.EnvironmentVariables["PYTHONLEGACYWINDOWSSTDIO"] = "utf-8";
            process.Start();
            return process;
        }

        private Process StartNoLanguageLeftBehindApi()
        {
            var arguments = "docker run --rm -e SERVER_PORT=5000 -e APP_PORT=7860 -p 7860:7860 -v C:\\Windows\\Temp\\cache.bin:/home/user/.cache ghcr.io/winstxnhdw/nllb-api:main";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("docker", arguments)
                {
                    UseShellExecute = false,
                }
            };

            process.Start();
            return process;
        }

        private Process StartLibreTranslate()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("libretranslate", string.Empty)
                {
                    UseShellExecute = false,
                }
            };

            process.Start();
            return process;
        }

        private static List<string> SplitResult(List<string> result, int mergeCount, string language)
        {
            if (result.Count != 1)
            {
                return result;
            }

            if (mergeCount == 1)
            {
                var arr = Utilities.AutoBreakLine(result[0], 84, 1, language).SplitToLines();
                if (arr.Count == 1)
                {
                    arr = Utilities.AutoBreakLine(result[0], 42, 1, language).SplitToLines();
                }

                if (arr.Count == 1)
                {
                    arr = Utilities.AutoBreakLine(result[0], 22, 1, language).SplitToLines();
                }

                if (arr.Count == 2)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                    };
                }

                if (arr.Count == 1)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                    };
                }

                return result;
            }

            if (mergeCount == 2)
            {
                var arr = SplitHelper.SplitToXLines(3, result[0], 84).ToArray();

                if (arr.Length == 3)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[2], 42, language == "zh" ? 0 : 25, language),
                    };
                }

                if (arr.Length == 2)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                    };
                }

                if (arr.Length == 1)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                        string.Empty,
                    };
                }

                return result;
            }

            if (mergeCount == 3)
            {
                var arr = SplitHelper.SplitToXLines(4, result[0], 84).ToArray();

                if (arr.Length == 4)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[2], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[3], 42, language == "zh" ? 0 : 25, language),
                    };
                }

                if (arr.Length == 3)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[2], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                    };
                }

                if (arr.Length == 2)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        Utilities.AutoBreakLine(arr[1], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                        string.Empty,
                    };
                }

                if (arr.Length == 1)
                {
                    return new List<string>
                    {
                        Utilities.AutoBreakLine(arr[0], 42, language == "zh" ? 0 : 25, language),
                        string.Empty,
                        string.Empty,
                        string.Empty,
                    };
                }

                return result;
            }

            return result;
        }

        private static bool MergeWithNext(Subtitle subtitle, int i, string source)
        {
            if (i + 1 >= subtitle.Paragraphs.Count || source.ToLowerInvariant() == "zh" || source.ToLowerInvariant() == "ja")
            {
                return false;
            }

            var p = subtitle.Paragraphs[i];
            var text = HtmlUtil.RemoveHtmlTags(p.Text, true).TrimEnd('"');
            if (text.EndsWith(".", StringComparison.Ordinal) ||
                text.EndsWith("!", StringComparison.Ordinal) ||
                text.EndsWith("?", StringComparison.Ordinal))
            {
                return false;
            }

            var next = subtitle.Paragraphs[i + 1];
            return next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds < 500;
        }

        private static bool HasAllLinesTag(Subtitle subtitle, int i, int mergeCount, string tag)
        {
            for (var j = i; j < subtitle.Paragraphs.Count && j <= i + mergeCount; j++)
            {
                var text = subtitle.Paragraphs[j].Text.Trim();
                if (!text.StartsWith("<" + tag + ">") && !text.EndsWith("</" + tag + ">"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MergeWithTwoNext(Subtitle subtitle, int i, string source)
        {
            if (i + 2 >= subtitle.Paragraphs.Count || source.ToLowerInvariant() == "zh" || source.ToLowerInvariant() == "ja")
            {
                return false;
            }

            return MergeWithNext(subtitle, i, source) && MergeWithNext(subtitle, i + 1, source);
        }

        private static bool MergeWithThreeNext(Subtitle subtitle, int i, string source)
        {
            if (i + 3 >= subtitle.Paragraphs.Count || source.ToLowerInvariant() == "zh" || source.ToLowerInvariant() == "ja")
            {
                return false;
            }

            return MergeWithNext(subtitle, i, source) && MergeWithNext(subtitle, i + 1, source) && MergeWithNext(subtitle, i + 2, source);
        }

        private static string MergeLines(Subtitle subtitle, int i, int mergeCount, bool italic, bool bold)
        {
            var sb = new StringBuilder();
            for (var j = i; j < subtitle.Paragraphs.Count && j <= i + mergeCount; j++)
            {
                var text = subtitle.Paragraphs[j].Text.Trim();
                sb.AppendLine(RemoveAllLinesTag(text, italic, bold));
            }

            return Utilities.RemoveLineBreaks(sb.ToString());
        }

        private static string RemoveAllLinesTag(string text, bool allItalic, bool allBold)
        {
            if (allItalic)
            {
                text = text.Replace("<i>", string.Empty);
                text = text.Replace("</i>", string.Empty);
            }

            if (allBold)
            {
                text = text.Replace("<b>", string.Empty);
                text = text.Replace("</b>", string.Empty);
            }

            return text;
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateTranslation();
        }

        private void UpdateTranslation()
        {
            if (!_translationProgressDirty)
            {
                return;
            }

            subtitleListViewTarget.BeginUpdate();
            subtitleListViewTarget.Fill(TranslatedSubtitle);
            _translationProgressDirty = true;
            subtitleListViewTarget.SelectIndexAndEnsureVisible(_translationProgressIndex);
            subtitleListViewTarget.EndUpdate();
            subtitleListViewSource.SelectIndexAndEnsureVisible(_translationProgressIndex);
        }

        private void AutoTranslate_ResizeEnd(object sender, EventArgs e)
        {
            AutoTranslate_Resize(null, null);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var isEmpty = TranslatedSubtitle == null || TranslatedSubtitle.Paragraphs.All(p => string.IsNullOrEmpty(p.Text));
            DialogResult = isEmpty ? DialogResult.Cancel : DialogResult.OK;
        }

        private IAutoTranslator GetCurrentEngine()
        {
            return _autoTranslatorEngines.First(p => p.Name == nikseComboBoxEngine.Text);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void nikseComboBoxEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAutoTranslatorEngine();
            SetupLanguageSettings();
        }

        private void nikseComboBoxUrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAutoTranslatorUrl(nikseComboBoxUrl.Text);
        }

        private void linkLabelPoweredBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var engine = _autoTranslatorEngines.First(p => p.Name == nikseComboBoxEngine.Text);
            UiUtil.OpenUrl(engine.Url);
        }

        private void AutoTranslate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyData == UiUtil.HelpKeys)
            {
                UiUtil.ShowHelp("#translation");
                e.SuppressKeyPress = true;
            }
        }

        private void AutoTranslate_FormClosing(object sender, FormClosingEventArgs e)
        {
            var engine = GetCurrentEngine();
            Configuration.Settings.Tools.AutoTranslateLastName = engine.Name;
            Configuration.Settings.Tools.AutoTranslateLastUrl = nikseComboBoxUrl.Text;
        }
    }
}