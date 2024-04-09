using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.AudioToText;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats;
using Nikse.SubtitleEdit.Core.ContainerFormats.MaterialExchangeFormat;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4.Boxes;
using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using Nikse.SubtitleEdit.Core.Enums;
using Nikse.SubtitleEdit.Core.Forms;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.NetflixQualityCheck;
using Nikse.SubtitleEdit.Core.SpellCheck;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Forms.Assa;
using Nikse.SubtitleEdit.Forms.AudioToText;
using Nikse.SubtitleEdit.Forms.FormatProperties;
using Nikse.SubtitleEdit.Forms.Networking;
using Nikse.SubtitleEdit.Forms.Ocr;
using Nikse.SubtitleEdit.Forms.Options;
using Nikse.SubtitleEdit.Forms.SeJobs;
using Nikse.SubtitleEdit.Forms.ShotChanges;
using Nikse.SubtitleEdit.Forms.Styles;
using Nikse.SubtitleEdit.Forms.Translate;
using Nikse.SubtitleEdit.Forms.VTT;
using Nikse.SubtitleEdit.Logic;
using Nikse.SubtitleEdit.Logic.CommandLineConvert;
using Nikse.SubtitleEdit.Logic.Networking;
using Nikse.SubtitleEdit.Logic.SeJob;
using Nikse.SubtitleEdit.Logic.VideoPlayers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheckForUpdatesHelper = Nikse.SubtitleEdit.Logic.CheckForUpdatesHelper;
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class Main : Form, IReloadSubtitle, IFindAndReplace
    {
        private class ComboBoxZoomItem
        {
            public string Text { get; set; }
            public double ZoomFactor { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public static string MainTextBox => nameof(textBoxListViewText);

        private Control ListView => splitContainerListViewAndText;
        private Control SourceView => textBoxSource;
        private bool InListView => !textBoxSource.Visible;
        private bool InSourceView => textBoxSource.Visible;
        private int MinGapBetweenLines => Configuration.Settings.General.MinimumMillisecondsBetweenLines;
        private bool _isOriginalActive;
        private bool IsOriginalEditable => _isOriginalActive && Configuration.Settings.General.AllowEditOfOriginalSubtitle;
        private bool IsVideoVisible => _layout != LayoutManager.LayoutNoVideo;

        private Subtitle _subtitle = new Subtitle();
        private Subtitle _subtitleOriginal = new Subtitle();
        private string _fileName;
        private string _subtitleOriginalFileName;
        private int _undoIndex = -1;
        private string _listViewTextUndoLast;
        private int _listViewTextUndoIndex = -1;
        private long _listViewTextTicks = -1;
        private string _listViewOriginalTextUndoLast;
        private long _listViewOriginalTextTicks = -1;
        private bool _listViewMouseDown;
        private long _sourceTextTicks = -1;

        private int _videoAudioTrackNumber = -1;

        public int VideoAudioTrackNumber
        {
            get => _videoAudioTrackNumber;
            set
            {
                if (_videoAudioTrackNumber != value)
                {
                    if (value >= 0 && _videoAudioTrackNumber != -1)
                    {
                        ReloadWaveform(_videoFileName, value);
                    }

                    _videoAudioTrackNumber = value;
                }
            }
        }

        private string _videoFileName;
        private bool VideoFileNameIsUrl => _videoFileName != null &&
                                          (_videoFileName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                           _videoFileName.StartsWith("https://", StringComparison.OrdinalIgnoreCase));

        private DateTime _fileDateTime;
        private string _title;
        private FindReplaceDialogHelper _findHelper;
        private FindDialog _findDialog;
        private ReplaceDialog _replaceDialog;
        private Form _dialog;
        private bool _sourceViewChange;
        private int _changeSubtitleHash = -1;
        private int _changeOriginalSubtitleHash = -1;
        private int _changeSubtitleTextHash = -1;
        private Timer _liveSpellCheckTimer;
        private int _subtitleListViewIndex = -1;
        private Paragraph _oldSelectedParagraph;
        private bool _converted;
        private bool _formatManuallyChanged;
        private SubtitleFormat _oldSubtitleFormat;
        private SubtitleFormat _currentSubtitleFormat;
        private List<int> _selectedIndices;
        private LanguageStructure.Main _language;
        private LanguageStructure.General _languageGeneral;
        private SpellCheck _spellCheckForm;
        private bool _loading = true;
        private bool _exitWhenLoaded;
        private bool _forceClose = false;
        private int _repeatCount = -1;
        private double _endSeconds = -1;
        private int _playSelectionIndex = -1;
        private int _playSelectionIndexLoopStart = -1;
        private double _endSecondsNewPosition = -1;
        private long _endSecondsNewPositionTicks;
        private const double EndDelay = 0.05;
        private int _autoContinueDelayCount = -1;
        private long _lastTextKeyDownTicks;
        private long _lastHistoryTicks;
        private long _lastWaveformMenuCloseTicks;
        private double? _audioWaveformRightClickSeconds;
        private readonly Timer _timerDoSyntaxColoring = new Timer();
        private Timer _timerAutoBackup;
        private readonly Timer _timerClearStatus = new Timer();
        private string _textAutoBackup;
        private string _textAutoBackupOriginal;
        private readonly List<string> _statusLog = new List<string>();
        private bool _disableShowStatus;
        private StatusLog _statusLogForm;
        private bool _makeHistoryPaused;
        private bool _saveAsCalled;
        private string _imageSubFileName;
        private readonly Timer _timerSlow = new Timer();
        private readonly ContextMenuStrip _contextMenuStripPlayRate;

        private CheckForUpdatesHelper _checkForUpdatesHelper;
        private Timer _timerCheckForUpdates;

        private NikseWebServiceSession _networkSession;
        private NetworkChat _networkChat;

        private ShowEarlierLater _showEarlierOrLater;
        private MeasurementConverter _measurementConverter;

        private bool _isVideoControlsUndocked;
        private VideoPlayerUndocked _videoPlayerUndocked;
        private WaveformUndocked _waveformUndocked;
        private VideoControlsUndocked _videoControlsUndocked;

        private GoogleOrMicrosoftTranslate _googleOrMicrosoftTranslate;

        private bool _cleanupHasRun;
        private bool _cancelWordSpellCheck = true;

        private bool IsLiveSpellCheckEnabled => Configuration.Settings.Tools.LiveSpellCheck &&
                                                Configuration.Settings.General.SubtitleTextBoxSyntaxColor;

        private bool _clearLastFind;
        private FindType _clearLastFindType = FindType.Normal;
        private string _clearLastFindText = string.Empty;
        private bool _videoLoadedGoToSubPosAndPause;
        private string _cutText = string.Empty;
        private Paragraph _mainCreateStartDownEndUpParagraph;
        private Paragraph _mainAdjustStartDownEndUpAndGoToNextParagraph;
        private int _lastDoNotPrompt = -1;
        private VideoInfo _videoInfo;
        private bool _splitDualSami;
        private bool _openFileDialogOn;
        private bool _resetVideo = true;
        private bool _doAutoBreakOnTextChanged = true;
        private readonly static object _syncUndo = new object();
        private string[] _dragAndDropFiles;
        private readonly Timer _dragAndDropTimer = new Timer(); // to prevent locking windows explorer
        private readonly Timer _dragAndDropVideoTimer = new Timer(); // to prevent locking windows explorer
        private long _labelNextTicks = -1;
        private bool _showBookmarkLabel = true;
        private ContextMenuStrip _bookmarkContextMenu;
        private readonly MainShortcuts _shortcuts = new MainShortcuts();
        private long _winLeftDownTicks = -1;
        private long _winRightDownTicks = -1;
        private FormWindowState _lastFormWindowState = FormWindowState.Normal;
        private readonly List<string> _filesToDelete = new List<string>();
        private bool _restorePreviewAfterSecondSubtitle;
        private ListBox _intellisenceList;
        private ListBox _intellisenceListOriginal;
        private bool _updateSelectedCountStatusBar;
        private VoskDictate _dictateForm;
        private object _dictateTextBox;
        private bool _hasCurrentVosk;
        private int _openSaveCounter;

        public bool IsMenuOpen { get; private set; }

        public string Title
        {
            get
            {
                if (_title == null)
                {
                    var versionInfo = Utilities.AssemblyVersion.Split('.');
                    _title = $"{_languageGeneral.Title} {versionInfo[0]}.{versionInfo[1]}.{versionInfo[2]}";
                }

                return _title;
            }
        }

        private void SetCurrentFormat(string formatName)
        {
            SetCurrentFormat(SubtitleFormat.FromName(formatName, new SubRip()));
        }

        private void SetCurrentFormat(SubtitleFormat format)
        {
            if (format.IsVobSubIndexFile)
            {
                SubtitleListview1.HideNonVobSubColumns();
            }
            
        }

        private static string GetArgumentAfterColon(IEnumerable<string> commandLineArguments, string requestedArgumentName)
        {
            foreach (var argument in commandLineArguments)
            {
                if (argument.StartsWith(requestedArgumentName, StringComparison.OrdinalIgnoreCase))
                {
                    if (requestedArgumentName.EndsWith(':'))
                    {
                        return argument.Substring(requestedArgumentName.Length);
                    }

                    return argument;
                }
            }

            return null;
        }

        public Main()
        {
            if (Configuration.IsRunningOnLinux)
            {
                NativeMethods.setlocale(NativeMethods.LC_NUMERIC, "C");
            }

            try
            {
                UiUtil.PreInitialize(this);
                InitializeComponent();


                Icon = Properties.Resources.SEIcon;


                _contextMenuStripPlayRate = new ContextMenuStrip();
                SetLanguage(Configuration.Settings.General.Language);
                Text = Title;

                SetFormatTo(Configuration.Settings.General.DefaultSubtitleFormat);


                // set up UI interfaces / injections
                YouTubeAnnotations.GetYouTubeAnnotationStyles = new UiGetYouTubeAnnotationStyles();
                Ebu.EbuUiHelper = new UiEbuSaveHelper();
                Pac.GetPacEncodingImplementation = new UiGetPacEncoding(this);
                RichTextToPlainText.NativeRtfTextConverter = new RtfTextConverterRichTextBox();

                UpdateRecentFilesUI();
                InitializeToolbarAndImages();

                if (Configuration.Settings.General.RightToLeftMode)
                {
                    SubtitleListview1.RightToLeft = RightToLeft.Yes;
                    SubtitleListview1.RightToLeftLayout = true;
                }

                if (Configuration.Settings.General.StartInSourceView)
                {
                    SwitchView(SourceView);
                }
                else
                {
                    SwitchView(ListView);
                }


                _timerClearStatus.Interval = Configuration.Settings.General.ClearStatusBarAfterSeconds * 1000;
                _timerClearStatus.Tick += TimerClearStatus_Tick;

                var commandLineArgs = Environment.GetCommandLineArgs();
                var fileName = string.Empty;
                int srcLineNumber = -1;
                if (commandLineArgs.Length > 1)
                {
                    // ConvertOrReturn() shall not return if a command line conversion has been requested
                    CommandLineConverter.ConvertOrReturn(Title, commandLineArgs);

                    fileName = commandLineArgs[1];

                    if (fileName.Equals("/batchconvertui", StringComparison.OrdinalIgnoreCase) || fileName.Equals("-batchconvertui", StringComparison.OrdinalIgnoreCase))
                    {
                        new BatchConvert(this.Icon).ShowDialog();
                        Opacity = 0;
                        Environment.Exit(0);
                    }

                    var sourceLineString = GetArgumentAfterColon(commandLineArgs, "/srcline:");
                    if (!int.TryParse(sourceLineString, out srcLineNumber))
                    {
                        srcLineNumber = -1;
                    }

                    _videoFileName = GetArgumentAfterColon(commandLineArgs, "/video:");
                }

                CheckAndGetNewlyDownloadedMpvDlls(string.Empty);

                if (fileName.Length > 0 && File.Exists(fileName))
                {
                    fileName = Path.GetFullPath(fileName);

                    if (srcLineNumber < 0)
                    {
                        if (!OpenFromRecentFiles(fileName))
                        {
                            OpenSubtitle(fileName, null, _videoFileName, VideoAudioTrackNumber, null, true);
                        }
                    }
                    else
                    {
                        OpenSubtitle(fileName, null, _videoFileName, VideoAudioTrackNumber, null, true);
                    }

                    if (srcLineNumber >= 0 && GetCurrentSubtitleFormat().GetType() == typeof(SubRip) && srcLineNumber < textBoxSource.Lines.Length)
                    {
                        int pos = 0;
                        for (int i = 0; i < srcLineNumber; i++)
                        {
                            pos += textBoxSource.Lines[i].Length;
                        }

                        if (pos + 35 < textBoxSource.TextLength)
                        {
                            pos += 35;
                        }

                        string s = textBoxSource.Text.Substring(0, pos);
                        int lastTimeCode = s.LastIndexOf(" --> ", StringComparison.Ordinal); // 00:02:26,407 --> 00:02:31,356
                        if (lastTimeCode > 14 && lastTimeCode + 16 >= s.Length)
                        {
                            s = s.Substring(0, lastTimeCode - 5);
                            lastTimeCode = s.LastIndexOf(" --> ", StringComparison.Ordinal);
                        }

                        if (lastTimeCode > 14 && lastTimeCode + 16 < s.Length)
                        {
                            string tc = s.Substring(lastTimeCode - 13, 30).Trim();
                            int index = 0;
                            foreach (var p in _subtitle.Paragraphs)
                            {
                                if (tc == p.StartTime + " --> " + p.EndTime)
                                {
                                    SubtitleListview1.SelectNone();
                                    SubtitleListview1.Items[0].Selected = false;
                                    SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
                                    break;
                                }

                                index++;
                            }
                        }
                    }
                }
                else if (Configuration.Settings.General.StartLoadLastFile && Configuration.Settings.RecentFiles.Files.Count > 0)
                {
                    fileName = Configuration.Settings.RecentFiles.Files[0].FileName;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName) && !OpenFromRecentFiles(fileName))
                    {
                        OpenSubtitle(fileName, null);
                    }
                }

                if (string.IsNullOrEmpty(_fileName))
                {
                    EnableOrDisableEditControls();
                }

               

                InitializeWaveformZoomDropdown();

                FixLargeFonts();

                if (Configuration.Settings.General.RightToLeftMode)
                {
                    ToolStripMenuItemRightToLeftModeClick(null, null);
                }


                ListViewHelper.RestoreListViewDisplayIndices(SubtitleListview1);
            }
            catch (Exception exception)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                SeLogger.Error(exception, "Main constructor");
            }
        }


        private void InitializeWaveformZoomDropdown()
        {
            for (double zoomCounter = AudioVisualizer.ZoomMinimum; zoomCounter <= AudioVisualizer.ZoomMaximum + (0.001); zoomCounter += 0.1)
            {
                int percent = (int)Math.Round(zoomCounter * 100);
                var item = new ComboBoxZoomItem { Text = percent + "%", ZoomFactor = zoomCounter };
            }
        }

        private void TimerClearStatus_Tick(object sender, EventArgs e)
        {
            ShowStatus(string.Empty);
        }

        private void ResetPlaySelection()
        {
            _endSeconds = -1;
            _playSelectionIndex = -1;
            _playSelectionIndexLoopStart = -1;
        }


        private void RemoveShotChange(int idx)
        {
           
        }

        private int _lastMultiMoveHash = -1;


        private void Main_Load(object sender, EventArgs e)
        {

            if (Configuration.Settings.General.StartRememberPositionAndSize &&
                !string.IsNullOrEmpty(Configuration.Settings.General.StartPosition))
            {
                var parts = Configuration.Settings.General.StartPosition.Split(';');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
                    {
                        if (x > -100 || y > -100)
                        {
                            Left = x;
                            Top = y;
                        }
                    }
                }

                if (Configuration.Settings.General.StartSize == "Maximized")
                {
                    CenterFormOnCurrentScreen();
                    WindowState = FormWindowState.Maximized;
                    return;
                }

                parts = Configuration.Settings.General.StartSize.Split(';');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
                    {
                        Width = x;
                        Height = y;
                    }
                }

                var ctrlScreen = Screen.FromControl(this);

                if (ctrlScreen.Bounds.Width < Width)
                {
                    Width = ctrlScreen.Bounds.Width;
                }

                if (ctrlScreen.Bounds.Height < Height)
                {
                    Height = ctrlScreen.Bounds.Height;
                }

                // Fix main window coordinate (Multi-Monitor issue)
                if ((ctrlScreen.Bounds.Right < Left) || (ctrlScreen.Bounds.Bottom < Top) ||
                    (ctrlScreen.Bounds.X > Right) || (ctrlScreen.Bounds.Y > Top))
                {
                    CenterToScreen();
                }
            }
            else
            {
                CenterFormOnCurrentScreen();
            }
        }

        private void InitializeLanguage()
        {
            

            // main controls
            SubtitleListview1.InitializeLanguage(_languageGeneral, Configuration.Settings);
            // waveform
            var languageWaveform = LanguageSettings.Current.Waveform;


            FormatLanguage.LineNumberXErrorReadingFromSourceLineY = LanguageSettings.Current.Main.LineNumberXErrorReadingFromSourceLineY;
            FormatLanguage.LineNumberXErrorReadingTimeCodeFromSourceLineY = LanguageSettings.Current.Main.LineNumberXErrorReadingTimeCodeFromSourceLineY;
            FormatLanguage.LineNumberXExpectedEmptyLine = LanguageSettings.Current.Main.LineNumberXExpectedEmptyLine;
            FormatLanguage.LineNumberXExpectedNumberFromSourceLineY = LanguageSettings.Current.Main.LineNumberXExpectedNumberFromSourceLineY;

            NetflixLanguage.GlyphCheckReport = LanguageSettings.Current.NetflixQualityCheck.GlyphCheckReport;
            NetflixLanguage.WhiteSpaceCheckReport = LanguageSettings.Current.NetflixQualityCheck.WhiteSpaceCheckReport;

            DvdSubtitleLanguage.Language.NotSpecified = LanguageSettings.Current.LanguageNames.NotSpecified;
            DvdSubtitleLanguage.Language.UnknownCodeX = LanguageSettings.Current.LanguageNames.UnknownCodeX;
            DvdSubtitleLanguage.Language.CultureName = LanguageSettings.Current.General.CultureName;
            DvdSubtitleLanguage.Language.LanguageNames = DvdSubtitleLanguages.GetLanguages();
            DvdSubtitleLanguage.Initialize();
        }

        private void SetFormatTo(string formatName)
        {
            SetFormatTo(SubtitleFormat.FromName(formatName, new SubRip()));
        }

        private void SetFormatTo(SubtitleFormat subtitleFormat)
        {
            var oldFormat = _currentSubtitleFormat;
            _currentSubtitleFormat = null;
            _currentSubtitleFormat = GetCurrentSubtitleFormat();
            MakeFormatChange(oldFormat, _currentSubtitleFormat);
        }

        private void MakeFormatChange(SubtitleFormat currentSubtitleFormat, SubtitleFormat oldFormat)
        {
            var format = currentSubtitleFormat;
            _converted = format != _oldSubtitleFormat;
            if (format == null)
            {
                format = new SubRip();
            }

            var formatType = format.GetType();

            _oldSubtitleFormat = oldFormat;
            var oldParagraphCount = _subtitle.Paragraphs.Count;
            if (_oldSubtitleFormat == null)
            {
                if (!_loading && _lastChangedToFormat != format.FriendlyName)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeConvertingToX, format.FriendlyName));
                }

                if (formatType == typeof(AdvancedSubStationAlpha))
                {
                    SetAssaResolutionWithChecks();
                }
            }
            else
            {
                if (!_makeHistoryPaused && _lastChangedToFormat != format.FriendlyName)
                {
                    _subtitle.MakeHistoryForUndo(string.Format(_language.BeforeConvertingToX, format.FriendlyName), _lastChangedToFormat, _fileDateTime, _subtitleOriginal, _subtitleOriginalFileName, _subtitleListViewIndex, textBoxListViewText.SelectionStart, textBoxListViewTextOriginal.SelectionStart);
                    _undoIndex++;
                    if (_undoIndex > Subtitle.MaximumHistoryItems)
                    {
                        _undoIndex--;
                    }
                }

                if (formatType == typeof(AdvancedSubStationAlpha) && _oldSubtitleFormat.GetType() == typeof(NetflixImsc11Japanese))
                {
                    var raw = NetflixImsc11JapaneseToAss.Convert(_subtitle, _videoInfo?.Width ?? 1280, _videoInfo?.Height ?? 720);
                    var s = new Subtitle();
                    new AdvancedSubStationAlpha().LoadSubtitle(s, raw.SplitToLines(), null);
                    _subtitle.Paragraphs.Clear();
                    _subtitle.Paragraphs.AddRange(s.Paragraphs);
                    _subtitle.Header = s.Header;
                    _subtitle.Footer = s.Footer;

                    SaveSubtitleListviewIndices();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();
                }
                else if (_oldSubtitleFormat?.GetType() != format.GetType())
                {
                    _oldSubtitleFormat.RemoveNativeFormatting(_subtitle, format);
                }

                if (formatType == typeof(AdvancedSubStationAlpha) && _oldSubtitleFormat.GetType() != typeof(NetflixImsc11Japanese))
                {
                    if (_oldSubtitleFormat?.GetType() == typeof(WebVTT) ||
                        _oldSubtitleFormat?.GetType() == typeof(WebVTTFileWithLineNumber))
                    {
                        _subtitle = WebVttToAssa.Convert(_subtitle, new SsaStyle(), _videoInfo?.Width ?? 0, _videoInfo?.Height ?? 0);
                    }

                    foreach (var p in _subtitle.Paragraphs)
                    {
                        p.Text = AdvancedSubStationAlpha.FormatText(p.Text);
                    }
                }

                _subtitle.Renumber();
                if (oldParagraphCount == _subtitle.Paragraphs.Count)
                {
                    SaveSubtitleListviewIndices();
                }

                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);

                if (oldParagraphCount == _subtitle.Paragraphs.Count)
                {
                    RestoreSubtitleListviewIndices();
                }

                if (_oldSubtitleFormat.HasStyleSupport)
                {
                    SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);
                }

                if (_networkSession == null)
                {
                    SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Network);
                }

                if (formatType == typeof(AdvancedSubStationAlpha))
                {
                    if (_oldSubtitleFormat.GetType() == typeof(SubStationAlpha))
                    {
                        if (_subtitle.Header != null && !_subtitle.Header.Contains("[V4+ Styles]"))
                        {
                            _subtitle.Header = AdvancedSubStationAlpha.GetHeaderAndStylesFromSubStationAlpha(_subtitle.Header);
                            foreach (var p in _subtitle.Paragraphs)
                            {
                                if (p.Extra != null)
                                {
                                    p.Extra = p.Extra.TrimStart('*');
                                }
                            }
                        }
                    }
                    else if (_oldSubtitleFormat.GetType() == typeof(AdvancedSubStationAlpha) && string.IsNullOrEmpty(_subtitle.Header))
                    {
                        _subtitle.Header = AdvancedSubStationAlpha.DefaultHeader;
                    }

                    SetAssaResolutionWithChecks();
                }
            }

            _lastChangedToFormat = format.FriendlyName;
            UpdateSourceView();
            if (_converted && _subtitle?.Paragraphs.Count > 0 && oldFormat != null)
            {
                ShowStatus(string.Format(_language.ConvertedToX, format.FriendlyName));
            }

            if (!string.IsNullOrEmpty(_fileName) && _oldSubtitleFormat != null)
            {
                if (_fileName.Contains('.'))
                {
                    _fileName = _fileName.Substring(0, _fileName.LastIndexOf('.')) + format.Extension;
                }
                else
                {
                    _fileName += format.Extension;
                }

                SetTitle();
            }

            if ((formatType == typeof(AdvancedSubStationAlpha) ||
                 formatType == typeof(SubStationAlpha) ||
                 formatType == typeof(CsvNuendo)) && (_subtitle.Paragraphs.Any(p => !string.IsNullOrEmpty(p.Actor)) ||
                                                      Configuration.Settings.Tools.ListViewShowColumnActor))
            {
                bool wasVisible = SubtitleListview1.ColumnIndexActor >= 0;
                if (formatType == typeof(CsvNuendo))
                {
                    SubtitleListview1.ShowActorColumn(LanguageSettings.Current.General.Character);
                }
                else
                {
                    SubtitleListview1.ShowActorColumn(LanguageSettings.Current.General.Actor);
                }

                if (!wasVisible)
                {
                    SaveSubtitleListviewIndices();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();
                }
            }
            else
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Actor);
            }

            if (formatType == typeof(TimedText10) && Configuration.Settings.Tools.ListViewShowColumnRegion)
            {
                SubtitleListview1.ShowRegionColumn(LanguageSettings.Current.General.Region);
            }
            else
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Region);
            }

            if (format.HasStyleSupport)
            {
                var styles = new List<string>();
                if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
                {
                    styles = AdvancedSubStationAlpha.GetStylesFromHeader(_subtitle.Header);
                    if (styles.Count == 0)
                    {
                        styles = AdvancedSubStationAlpha.GetStylesFromHeader(AdvancedSubStationAlpha.DefaultHeader);
                    }
                }
                else if (formatType == typeof(TimedText10) || formatType == typeof(ItunesTimedText) || formatType == typeof(TimedTextImsc11))
                {
                    styles = TimedText10.GetStylesFromHeader(_subtitle.Header);
                }
                else if (formatType == typeof(Sami) || formatType == typeof(SamiModern))
                {
                    styles = Sami.GetStylesFromHeader(_subtitle.Header);
                    if (string.IsNullOrEmpty(_subtitle.Header))
                    {
                        styles = Sami.GetStylesFromSubtitle(_subtitle);
                    }
                    else
                    {
                        styles = Sami.GetStylesFromHeader(_subtitle.Header);
                    }
                }
                else if (format.Name == "Nuendo")
                {
                    styles = GetNuendoStyles();
                }

                if (styles.Count > 0)
                {
                    foreach (var p in _subtitle.Paragraphs)
                    {
                        if (string.IsNullOrEmpty(p.Extra))
                        {
                            p.Extra = styles[0];
                        }
                    }
                }

                if (formatType == typeof(Sami) || formatType == typeof(SamiModern))
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.Class);
                }
                else if (formatType == typeof(TimedText10) || formatType == typeof(ItunesTimedText) || formatType == typeof(TimedTextImsc11))
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.StyleLanguage);
                }
                else if (format.Name == "Nuendo")
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.Character);
                }
                else
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.Style);
                }

                SaveSubtitleListviewIndices();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                RestoreSubtitleListviewIndices();
            }
            else
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);
            }

            ShowHideTextBasedFeatures(format);

            UpdateToolbarButtonsToCurrentFormat(currentSubtitleFormat);

            _oldSubtitleFormat = oldFormat;
        }

        private int FirstSelectedIndex => SubtitleListview1.SelectedIndices.Count == 0 ? -1 : SubtitleListview1.SelectedIndices[0];

        private int FirstVisibleIndex =>
            SubtitleListview1.Items.Count == 0 || SubtitleListview1.TopItem == null ? -1 : SubtitleListview1.TopItem.Index;

        private long _lastAutoSave;

        private void AutoSave(bool force = false)
        {
            if (!Configuration.Settings.General.AutoSave ||
                DateTime.UtcNow.Ticks - _lastAutoSave < 10000 * 3000 && !force) // only check for auto save evety 3 seconds
            {
                return;
            }

            if (force)
            {
                DoAutoSave();
            }
            else
            {
                Interlocked.Increment(ref _openSaveCounter);
                DoAutoSave();
                Interlocked.Decrement(ref _openSaveCounter);

            }
        }

        private void DoAutoSave()
        {
            _lastAutoSave = DateTime.UtcNow.Ticks + 1009000;
            var currentSubtitleHash = GetFastSubtitleHash();
            if (_changeSubtitleHash != currentSubtitleHash && _lastDoNotPrompt != currentSubtitleHash && _subtitle?.Paragraphs.Count > 0)
            {
                if (string.IsNullOrEmpty(_fileName) || _converted)
                {
                    return;
                }

                SaveSubtitle(GetCurrentSubtitleFormat(), false, true);
            }

            if (!string.IsNullOrEmpty(_subtitleOriginalFileName) && Configuration.Settings.General.AllowEditOfOriginalSubtitle)
            {
                SaveOriginalSubtitle(GetCurrentSubtitleFormat(), true);
            }

            _lastAutoSave = DateTime.UtcNow.Ticks;
        }

        private bool ContinueNewOrExit()
        {
            AutoSave(true);
            var currentSubtitleHash = GetFastSubtitleHash();
            if (_changeSubtitleHash != currentSubtitleHash && _lastDoNotPrompt != currentSubtitleHash && _subtitle?.Paragraphs.Count > 0)
            {
                string promptText = _language.SaveChangesToUntitled;
                if (!string.IsNullOrEmpty(_fileName))
                {
                    promptText = string.Format(_language.SaveChangesToX, _fileName);
                }

                var dr = MessageBox.Show(this, promptText, Title, MessageBoxButtons.YesNoCancel);

                if (dr == DialogResult.Cancel)
                {
                    return false;
                }

                if (dr == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(_fileName))
                    {
                        if (!string.IsNullOrEmpty(openFileDialog1.InitialDirectory))
                        {
                            saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
                        }

                        saveFileDialog1.Title = _language.SaveSubtitleAs;
                        if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            openFileDialog1.InitialDirectory = saveFileDialog1.InitialDirectory;
                            _fileName = saveFileDialog1.FileName;
                            SetTitle();
                            Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                            Configuration.Settings.Save();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (SaveSubtitle(GetCurrentSubtitleFormat()) != DialogResult.OK)
                    {
                        return false;
                    }
                }
            }

            return ContinueNewOrExitOriginal();
        }

        private bool ContinueNewOrExitOriginal()
        {
            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0 && _changeOriginalSubtitleHash != GetFastSubtitleOriginalHash())
            {
                string promptText = _language.SaveChangesToUntitledOriginal;
                if (!string.IsNullOrEmpty(_subtitleOriginalFileName))
                {
                    promptText = string.Format(_language.SaveChangesToOriginalX, _subtitleOriginalFileName);
                }

                var dr = MessageBox.Show(this, promptText, Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dr == DialogResult.Cancel)
                {
                    return false;
                }

                if (dr == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(_subtitleOriginalFileName))
                    {
                        if (!string.IsNullOrEmpty(openFileDialog1.InitialDirectory))
                        {
                            saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
                        }

                        saveFileDialog1.Title = _language.SaveOriginalSubtitleAs;
                        if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            openFileDialog1.InitialDirectory = saveFileDialog1.InitialDirectory;
                            _subtitleOriginalFileName = saveFileDialog1.FileName;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (SaveOriginalSubtitle(GetCurrentSubtitleFormat()) != DialogResult.OK)
                    {
                        return false;
                    }
                }
            }

            _lastDoNotPrompt = GetFastSubtitleHash();
            return true;
        }

        public void MakeHistoryForUndo(string description, bool resetTextUndo)
        {
            if (_makeHistoryPaused)
            {
                return;
            }

            if (resetTextUndo)
            {
                _listViewTextUndoLast = null;
                _listViewOriginalTextUndoLast = null;
            }

            if (_undoIndex == -1)
            {
                _subtitle.HistoryItems.Clear();
            }
            else
            {
                // remove items for redo
                while (_subtitle.HistoryItems.Count > _undoIndex + 1)
                {
                    _subtitle.HistoryItems.RemoveAt(_subtitle.HistoryItems.Count - 1);
                }
            }

            _subtitle.FileName = _fileName;
            _subtitle.MakeHistoryForUndo(description, GetCurrentSubtitleFormat().FriendlyName, _fileDateTime, _subtitleOriginal, _subtitleOriginalFileName, _subtitleListViewIndex, textBoxListViewText.SelectionStart, textBoxListViewTextOriginal.SelectionStart);
            _undoIndex++;

            if (_undoIndex > Subtitle.MaximumHistoryItems)
            {
                _undoIndex--;
            }
        }

        public void MakeHistoryForUndo(string description)
        {
            MakeHistoryForUndo(description, true);
        }

        /// <summary>
        /// Add undo history - but only if last entry is older than 500 ms
        /// </summary>
        /// <param name="description">Undo description</param>
        public void MakeHistoryForUndoOnlyIfNotRecent(string description)
        {
            if (_makeHistoryPaused)
            {
                return;
            }

            if ((DateTime.UtcNow.Ticks - _lastHistoryTicks) > 10000 * 500) // only if last change was longer ago than 500 milliseconds
            {
                MakeHistoryForUndo(description);
                _lastHistoryTicks = DateTime.UtcNow.Ticks;
            }
        }

        private bool IsSubtitleLoaded =>
            _subtitle != null && (_subtitle.Paragraphs.Count > 1 || (_subtitle.Paragraphs.Count == 1 && !string.IsNullOrWhiteSpace(_subtitle.Paragraphs[0].Text)));

        private bool OpenFromRecentFiles(string fileName)
        {
            var rfe = Configuration.Settings.RecentFiles.Files.Find(p => !string.IsNullOrEmpty(p.FileName) && p.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (rfe != null)
            {
                OpenRecentFile(rfe);
                GotoSubPosAndPause();
                SubtitleListview1.EndUpdate();
                SetRecentIndices(rfe);
                if (!string.IsNullOrEmpty(rfe.VideoFileName))
                {
                    var p = _subtitle.GetParagraphOrDefault(rfe.FirstSelectedIndex);
                }

                _openFileDialogOn = false;
                return true;
            }

            return false;
        }

        public double CurrentFrameRate
        {
            get { 
                return Configuration.Settings.General.DefaultFrameRate;
            }
        }

        private void OpenSubtitle(string fileName, Encoding encoding)
        {
            OpenSubtitle(fileName, encoding, null, -1, null);
        }

        private void ResetHistory()
        {
            _undoIndex = -1;
            _subtitle.HistoryItems.Clear();
        }

        private void OpenSubtitle(string fileName, Encoding encoding, string videoFileName, int audioTrack, string originalFileName)
        {
            OpenSubtitle(fileName, encoding, videoFileName, audioTrack, originalFileName, false);
        }

        private void OpenSubtitle(string fileName, Encoding encoding, string videoFileName, int audioTrack, string originalFileName, bool updateRecentFile)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show(string.Format(_language.FileNotFound, fileName));
                return;
            }

            if (FileUtil.IsFileLocked(fileName))
            {
                MessageBox.Show(string.Format(_language.FileLocked, fileName));
                return;
            }

            _lastAutoSave = DateTime.UtcNow.Ticks;
            bool videoFileLoaded = false;
            _formatManuallyChanged = false;
            var file = new FileInfo(fileName);
            var ext = file.Extension.ToLowerInvariant();

            // save last first visible index + first selected index from listview
            if (_fileName != null && updateRecentFile)
            {
                Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
            }

            Configuration.Settings.General.CurrentVideoOffsetInMs = 0;
            Configuration.Settings.General.CurrentVideoIsSmpte = false;

            openFileDialog1.InitialDirectory = file.DirectoryName;

            if (ext == ".idx")
            {
                var subFileName = fileName.Substring(0, fileName.Length - 3) + "sub";
                if (File.Exists(subFileName) && FileUtil.IsVobSub(subFileName))
                {
                    ext = ".sub";
                    fileName = subFileName;
                }
            }

            if (ext == ".sup")
            {
                if (FileUtil.IsBluRaySup(fileName))
                {
                    if (Configuration.Settings.Tools.BDOpenIn == "EDIT")
                    {
                        using (var form = new BinaryEdit.BinEdit(fileName, _loading))
                        {
                            form.ShowDialog(this);
                        }

                        if (_loading)
                        {
                            _exitWhenLoaded = _loading;
                            Opacity = 0;
                        }
                    }
                    else
                    {
                        ImportAndOcrBluRaySup(fileName, _loading);
                    }

                    return;
                }

                if (FileUtil.IsSpDvdSup(fileName))
                {
                    ImportAndOcrSpDvdSup(fileName, _loading);
                    return;
                }
            }

            if (FileUtil.IsManzanita(fileName))
            {
                var tsParser = new ManzanitaTransportStreamParser();
                tsParser.Parse(fileName);
                var subtitles = tsParser.GetDvbSup();
                if (subtitles.Count > 0)
                {
                    ImportSubtitleFromManzanitaTransportStream(fileName, subtitles);
                    return;
                }
            }

            if (file.Length > Subtitle.MaxFileSize)
            {
                // retry Blu-ray sup (file with wrong extension)
                if (FileUtil.IsBluRaySup(fileName))
                {
                    ImportAndOcrBluRaySup(fileName, _loading);
                    return;
                }

                // retry vobsub (file with wrong extension)
                if (IsVobSubFile(fileName, false))
                {
                    ImportAndOcrVobSubSubtitleNew(fileName, _loading);
                    return;
                }

                var text = string.Format(_language.FileXIsLargerThan10MB + Environment.NewLine + Environment.NewLine + _language.ContinueAnyway, fileName);
                if (MessageBox.Show(this, text, Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }

            if (_subtitle.HistoryItems.Count > 0 || _subtitle.Paragraphs.Count > 0)
            {
                MakeHistoryForUndo(string.Format(_language.BeforeLoadOf, Path.GetFileName(fileName)));
            }

            var subtitleHash = GetFastSubtitleHash();
            bool hasChanged = (_changeSubtitleHash != subtitleHash) && (_lastDoNotPrompt != subtitleHash);
            var newSubtitle = new Subtitle();
            SubtitleFormat format = newSubtitle.LoadSubtitle(fileName, out encoding, encoding);

            if (!hasChanged)
            {
                _changeSubtitleHash = GetFastSubtitleHash();
            }

            ShowHideTextBasedFeatures(format);

            bool justConverted = false;


            var encodingFromFile = encoding;
            if (format == null)
            {
                encodingFromFile = LanguageAutoDetect.GetEncodingFromFile(fileName);
            }

            if (format == null)
            {
                var f = new TimeCodesOnly1();
                var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                if (f.IsMine(list, fileName))
                {
                    f.LoadSubtitle(newSubtitle, list, fileName);
                    _oldSubtitleFormat = f;
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    justConverted = true;
                    format = GetCurrentSubtitleFormat();
                }
            }

            if (format == null)
            {
                var f = new TimeCodesOnly2();
                var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                if (f.IsMine(list, fileName))
                {
                    f.LoadSubtitle(newSubtitle, list, fileName);
                    _oldSubtitleFormat = f;
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    justConverted = true;
                    format = GetCurrentSubtitleFormat();
                }
            }

            if (format == null)
            {
                try
                {
                    var bdnXml = new BdnXml();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (bdnXml.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrBdnXml(fileName, bdnXml, list);
                        }

                        return;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (format == null)
            {
                try
                {
                    var rhozetHarmonicImage = new RhozetHarmonicImage();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (rhozetHarmonicImage.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, rhozetHarmonicImage, list);
                        }

                        return;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (format == null)
            {
                try
                {
                    var fcpImage = new FinalCutProImage();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (fcpImage.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, fcpImage, list);
                        }

                        return;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (format == null)
            {
                try
                {
                    var f = new DvdStudioProSpaceGraphic();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (f.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, f, list);
                        }

                        return;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (format == null)
            {
                try
                {
                    var imageFormat = new SpuImage();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (imageFormat.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, imageFormat, list);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null)
            {
                var arib = new AribB36();
                if (arib.IsMine(null, fileName))
                {
                    arib.LoadSubtitle(newSubtitle, null, fileName);
                    _oldSubtitleFormat = arib;
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    justConverted = true;
                    format = GetCurrentSubtitleFormat();
                }
            }

            
            if (format == null)
            {
                try
                {
                    var timedtextImage = new TimedTextImage();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (timedtextImage.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, timedtextImage, list);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null)
            {
                try
                {
                    var seImageHtmlIndex = new SeImageHtmlIndex();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (seImageHtmlIndex.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrDost(fileName, seImageHtmlIndex, list);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null || format.Name == Scenarist.NameOfFormat)
            {
                try
                {
                    var son = new Son();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (son.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrSon(fileName, son, list);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null || format.Name == SubRip.NameOfFormat)
            {
                if (newSubtitle.Paragraphs.Count > 1)
                {
                    int imageCount = 0;
                    foreach (var p in newSubtitle.Paragraphs)
                    {
                        string s = p.Text.ToLowerInvariant();
                        if (s.EndsWith(".bmp", StringComparison.Ordinal) || s.EndsWith(".png", StringComparison.Ordinal) || s.EndsWith(".jpg", StringComparison.Ordinal) || s.EndsWith(".tif", StringComparison.Ordinal))
                        {
                            imageCount++;
                        }
                    }

                    if (imageCount > 2 && imageCount >= newSubtitle.Paragraphs.Count - 2)
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrSrt(newSubtitle);
                        }

                        return;
                    }
                }
            }

            if (format == null)
            {
                try
                {
                    var satBoxPng = new SatBoxPng();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (satBoxPng.IsMine(list, fileName))
                    {
                        var subtitle = new Subtitle();
                        satBoxPng.LoadSubtitle(subtitle, list, fileName);
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrSrt(subtitle);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null || format.Name == Scenarist.NameOfFormat)
            {
                try
                {
                    var sst = new SonicScenaristBitmaps();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (sst.IsMine(list, fileName))
                    {
                        if (ContinueNewOrExit())
                        {
                            ImportAndOcrSst(fileName, sst, list);
                        }

                        return;
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null)
            {
                try
                {
                    var htmlSamiArray = new HtmlSamiArray();
                    var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                    if (htmlSamiArray.IsMine(list, fileName))
                    {
                        htmlSamiArray.LoadSubtitle(newSubtitle, list, fileName);
                        _oldSubtitleFormat = htmlSamiArray;
                        SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                        justConverted = true;
                        format = GetCurrentSubtitleFormat();
                    }
                }
                catch
                {
                    format = null;
                }
            }

            if (format == null)
            {
                foreach (var f in SubtitleFormat.GetBinaryFormats(false))
                {
                    if (f.IsMine(null, fileName))
                    {
                        f.LoadSubtitle(newSubtitle, null, fileName);
                        _oldSubtitleFormat = f;
                        SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                        justConverted = true;
                        format = GetCurrentSubtitleFormat();
                        break;
                    }
                }
            }

            
            if (format == null)
            {
                var lines = FileUtil.ReadAllTextShared(fileName, encodingFromFile).SplitToLines();
                foreach (var f in SubtitleFormat.GetTextOtherFormats())
                {
                    if (f.IsMine(lines, fileName))
                    {
                        f.LoadSubtitle(newSubtitle, lines, fileName);
                        _oldSubtitleFormat = f;
                        SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                        justConverted = true;
                        format = GetCurrentSubtitleFormat();
                        break;
                    }
                }
            }


            // retry vobsub (file with wrong extension)
            if (format == null && file.Length > 500 && IsVobSubFile(fileName, false))
            {
                ImportAndOcrVobSubSubtitleNew(fileName, _loading);
                return;
            }

            // retry Blu-ray (file with wrong extension)
            if (format == null && file.Length > 500 && FileUtil.IsBluRaySup(fileName))
            {
                ImportAndOcrBluRaySup(fileName, _loading);
                return;
            }

            // retry SP DVD (file with wrong extension)
            if (format == null && file.Length > 500 && FileUtil.IsSpDvdSup(fileName))
            {
                ImportAndOcrSpDvdSup(fileName, _loading);
                return;
            }

            // retry Matroska (file with wrong extension)
            if (format == null && !string.IsNullOrWhiteSpace(fileName))
            {
                using (var matroska = new MatroskaFile(fileName))
                {
                    if (matroska.IsValid)
                    {
                        var subtitleList = matroska.GetTracks(true);
                        if (subtitleList.Count > 0)
                        {
                            ImportSubtitleFromMatroskaFile(fileName);
                            return;
                        }
                    }
                }
            }


            // check for all binary zeroes (I've heard about this a few times... perhaps related to crashes?)
            if (format == null && FileUtil.IsSubtitleFileAllBinaryZeroes(fileName))
            {
                MessageBox.Show(_language.ErrorLoadBinaryZeroes);
                return;
            }

            if (format == null && file.Length < 100 * 1000000 && TransportStreamParser.IsDvbSup(fileName))
            {
                ImportSubtitleFromDvbSupFile(fileName);
                return;
            }

            if (format == null && file.Length < 1000000)
            {
                // check for valid timed text

                // Try to use a generic subtitle format parser (guessing subtitle format)
                try
                {
                    var enc = encodingFromFile;
                    var s = File.ReadAllText(fileName, enc);

                    // check for RTF file

                    var uknownFormatImporter = new UnknownFormatImporter { UseFrames = true };
                    var genericParseSubtitle = uknownFormatImporter.AutoGuessImport(s.SplitToLines());
                    if (genericParseSubtitle.Paragraphs.Count > 1)
                    {
                        newSubtitle = genericParseSubtitle;
                        SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                        justConverted = true;
                        format = GetCurrentSubtitleFormat();
                        ShowStatus("Guessed subtitle format via generic subtitle parser!");
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (format == null && file.Length < 1_000_000 && (FileUtil.IsPlainText(fileName) || new Tx3GTextOnly().IsMine(null, fileName)))
            {
                ImportPlainText(fileName);
                return;
            }

            if (format == null)
            {
                var fd = new FinalDraftTemplate2();
                var list = new List<string>(File.ReadAllLines(fileName, encodingFromFile));
                if (fd.IsMine(list, fileName))
                {
                    ImportPlainText(fileName);
                    return;
                }
            }

            _fileDateTime = File.GetLastWriteTime(fileName);

            if (format != null)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;

                RemoveOriginal(true, false);
                if (format.HasStyleSupport && format.GetType() == typeof(AdvancedSubStationAlpha))
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.Style);
                }

                new BookmarkPersistence(newSubtitle, fileName).Load();

                if (Configuration.Settings.General.RemoveBlankLinesWhenOpening)
                {
                    newSubtitle.RemoveEmptyLines();
                }

                if (Configuration.Settings.General.RemoveBadCharsWhenOpening)
                {
                    foreach (var p in newSubtitle.Paragraphs)
                    {
                        // Replace U+0456 (CYRILLIC SMALL LETTER BYELORUSSIAN-UKRAINIAN I) by U+0069 (LATIN SMALL LETTER I)
                        p.Text = p.Text.Replace("<і>", "<i>").Replace("</і>", "</i>");

                        // remove control characters (e.g. binary zero)
                        p.Text = p.Text.RemoveControlCharactersButWhiteSpace();
                    }
                }

                _subtitleListViewIndex = -1;
                Configuration.Settings.General.CurrentVideoOffsetInMs = 0;
                Configuration.Settings.General.CurrentVideoIsSmpte = false;

                if (_resetVideo && ModifierKeys != Keys.Shift)
                {
                    _videoFileName = null;
                    _videoInfo = null;
                    VideoAudioTrackNumber = -1;
                }

                var oldSaveFormat = Configuration.Settings.General.LastSaveAsFormat;
                _oldSubtitleFormat = format;
                SetCurrentFormat(format);
                Configuration.Settings.General.LastSaveAsFormat = oldSaveFormat;

                _subtitleOriginalFileName = null;
                if (LoadOriginalSubtitleFile(originalFileName))
                {
                    _subtitleOriginalFileName = originalFileName;
                }

                // Seungki begin
                _splitDualSami = false;
                if (Configuration.Settings.SubtitleSettings.SamiDisplayTwoClassesAsTwoSubtitles && format.GetType() == typeof(Sami) && Sami.GetStylesFromHeader(newSubtitle.Header).Count == 2)
                {
                    var classes = Sami.GetStylesFromHeader(newSubtitle.Header);
                    var s1 = new Subtitle(newSubtitle);
                    var s2 = new Subtitle(newSubtitle);
                    s1.Paragraphs.Clear();
                    s2.Paragraphs.Clear();
                    foreach (var p in newSubtitle.Paragraphs)
                    {
                        if (p.Extra != null && p.Extra.Equals(classes[0], StringComparison.OrdinalIgnoreCase))
                        {
                            s1.Paragraphs.Add(p);
                        }
                        else
                        {
                            s2.Paragraphs.Add(p);
                        }
                    }

                    if (s1.Paragraphs.Count == 0 || s2.Paragraphs.Count == 0)
                    {
                        return;
                    }

                    newSubtitle = s1;
                    _subtitleOriginal = s2;
                    _subtitleOriginalFileName = _fileName;
                    SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);
                    SubtitleListview1.ShowOriginalTextColumn(classes[1]);
                    _splitDualSami = true;
                }

                _subtitleListViewIndex = -1;
                SubtitleListview1.Fill(newSubtitle, _subtitleOriginal);
                _subtitle = newSubtitle;


                _findHelper = null;
                _spellCheckForm = null;

                if (IsVideoVisible)
                {
                    if (!Configuration.Settings.General.DisableVideoAutoLoading)
                    {
                        if (!string.IsNullOrEmpty(videoFileName) && File.Exists(videoFileName))
                        {
                            OpenVideo(videoFileName, audioTrack);
                        }
                        else if (!string.IsNullOrEmpty(fileName))
                        {
                            TryToFindAndOpenVideoFile(Utilities.GetPathAndFileNameWithoutExtension(fileName));
                        }

                        if (_videoFileName == null)
                        {
                            CloseVideoToolStripMenuItemClick(this, null);
                        }
                    }
                }


                videoFileLoaded = _videoFileName != null;

                VideoAudioTrackNumber = audioTrack;

                Configuration.Settings.RecentFiles.Add(fileName, videoFileName, audioTrack, originalFileName);
                UpdateRecentFilesUI();

                _fileName = fileName;
                SetTitle();
                ShowStatus(string.Format(_language.LoadedSubtitleX, _fileName));
                _sourceViewChange = false;

                _changeSubtitleHash = GetFastSubtitleHash();
                _converted = false;
                ResetHistory();
                SetListViewStateImages();
                SetUndockedWindowsTitle();

                if (justConverted)
                {
                    _converted = true;
                    ShowStatus(string.Format(_language.LoadedSubtitleX, _fileName) + " - " + string.Format(_language.ConvertedToX, format.FriendlyName));
                }

                var formatType = format.GetType();
                if (formatType == typeof(SubStationAlpha))
                {
                    string errors = AdvancedSubStationAlpha.CheckForErrors(_subtitle.Header);
                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show(this, errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    errors = (format as SubStationAlpha).Errors;
                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show(this, errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (formatType == typeof(AdvancedSubStationAlpha))
                {
                    string errors = AdvancedSubStationAlpha.CheckForErrors(_subtitle.Header);
                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show(this, errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    errors = (format as AdvancedSubStationAlpha).Errors;
                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show(this, errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (formatType == typeof(SubRip))
                {
                    string errors = (format as SubRip).Errors;
                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show(this, errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                

                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;

                _subtitleListViewIndex = -1;
                if (SubtitleListview1.Items.Count > 0)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(0);
                }
            }
            else
            {
                if (file.Length < 50)
                {
                    _findHelper = null;
                    _spellCheckForm = null;
                    _videoFileName = null;
                    _videoInfo = null;
                    VideoAudioTrackNumber = -1;

                    Configuration.Settings.RecentFiles.Add(fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                    Configuration.Settings.Save();
                    UpdateRecentFilesUI();
                    _fileName = fileName;
                    SetTitle();
                    ShowStatus(string.Format(_language.LoadedEmptyOrShort, _fileName));
                    _sourceViewChange = false;
                    _converted = false;

                    MessageBox.Show(_language.FileIsEmptyOrShort);
                }
                else
                {
                    if (ShowUnknownSubtitle(fileName, true))
                    {
                        ImportPlainText(fileName);
                    }

                    return;
                }
            }

            ResetShowEarlierOrLater();
            FixRightToLeftDependingOnLanguage();
        }

        private void LoadProfile(string profileName, SeJobModel seJob)
        {
            var profile = Configuration.Settings.General.Profiles.FirstOrDefault(p => p.Name == profileName);
            if (profile == null)
            {
                return;
            }

            var g = Configuration.Settings.General;
            g.CurrentProfile = profileName;
            g.MaxNumberOfLines = seJob.Rules.MaxNumberOfLines;
            g.SubtitleLineMaximumLength = seJob.Rules.SubtitleLineMaximumLength;
            g.SubtitleMaximumCharactersPerSeconds = (double)seJob.Rules.SubtitleMaximumCharactersPerSeconds;
            g.SubtitleMinimumDisplayMilliseconds = seJob.Rules.SubtitleMinimumDisplayMilliseconds;
            g.SubtitleMaximumDisplayMilliseconds = seJob.Rules.SubtitleMaximumDisplayMilliseconds;
            g.MinimumMillisecondsBetweenLines = seJob.Rules.MinimumMillisecondsBetweenLines;
            g.SubtitleMaximumWordsPerMinute = (double)seJob.Rules.SubtitleMaximumWordsPerMinute;
            g.SubtitleOptimalCharactersPerSeconds = (double)seJob.Rules.SubtitleOptimalCharactersPerSeconds;
        }

        private void ShowHideTextBasedFeatures(SubtitleFormat format)
        {
        }

        private void SetUndockedWindowsTitle()
        {
            string title = _languageGeneral.NoVideoLoaded;
            if (!string.IsNullOrEmpty(_videoFileName))
            {
                title = Path.GetFileNameWithoutExtension(_videoFileName);
            }

            if (_videoControlsUndocked != null && !_videoControlsUndocked.IsDisposed)
            {
                _videoControlsUndocked.Text = string.Format(_languageGeneral.ControlsWindowTitle, title);
            }

            if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed)
            {
                _videoPlayerUndocked.Text = string.Format(_languageGeneral.VideoWindowTitle, title);
            }

            if (_waveformUndocked != null && !_waveformUndocked.IsDisposed)
            {
                _waveformUndocked.Text = string.Format(_languageGeneral.AudioWindowTitle, title);
            }
        }

        private void ImportAndOcrBdnXml(string fileName, BdnXml bdnXml, List<string> list)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                var bdnSubtitle = new Subtitle();
                bdnXml.LoadSubtitle(bdnSubtitle, list, fileName);
                bdnSubtitle.FileName = fileName;
                formSubOcr.Initialize(bdnSubtitle, Configuration.Settings.VobSubOcr, false);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBdnXml);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(formSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                    _imageSubFileName = fileName;
                }
            }
        }

        private void ImportAndOcrSon(string fileName, Son format, List<string> list)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                var sub = new Subtitle();
                format.LoadSubtitle(sub, list, fileName);
                sub.FileName = fileName;
                formSubOcr.Initialize(sub, Configuration.Settings.VobSubOcr, true);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBdnXml);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(formSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                }
            }
        }

        private void ImportAndOcrDost(string fileName, SubtitleFormat format, List<string> list)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                var sub = new Subtitle();
                format.LoadSubtitle(sub, list, fileName);
                sub.FileName = fileName;
                formSubOcr.Initialize(sub, Configuration.Settings.VobSubOcr, false);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBdnXml);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(formSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                }
            }
        }

        private void ImportAndOcrSst(string fileName, SonicScenaristBitmaps format, List<string> list)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                var sub = new Subtitle();
                format.LoadSubtitle(sub, list, fileName);
                sub.FileName = fileName;
                formSubOcr.Initialize(sub, Configuration.Settings.VobSubOcr, true);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBdnXml);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(formSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                }
            }
        }

        private void ImportAndOcrSrt(Subtitle subtitle)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.Initialize(subtitle, Configuration.Settings.VobSubOcr, false);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBdnXml);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(formSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                }
            }
        }

        private bool ShowUnknownSubtitle(string fileName, bool allowImportPlainText = false)
        {
            using (var unknownSubtitle = new UnknownSubtitle())
            {
                unknownSubtitle.Initialize(Title, fileName, allowImportPlainText);
                unknownSubtitle.ShowDialog(this);
                return unknownSubtitle.ImportPlainText;
            }
        }

        private void UpdateRecentFilesUI()
        {
            var dropDownItems = new List<ToolStripMenuItem>();
            if (Configuration.Settings.General.ShowRecentFiles && Configuration.Settings.RecentFiles.Files.Count > 0)
            {
                var lowerFileNameList = new List<string>();
                foreach (var file in Configuration.Settings.RecentFiles.Files)
                {
                    if (!string.IsNullOrEmpty(file.OriginalFileName) && File.Exists(file.OriginalFileName))
                    {
                        dropDownItems.Add(new ToolStripMenuItem(file.FileName + " + " + file.OriginalFileName, null, ReopenSubtitleToolStripMenuItemClick) { Tag = file.FileName });
                    }
                    else
                    {
                        if (!lowerFileNameList.Contains(file.FileName.ToLowerInvariant()))
                        {
                            dropDownItems.Add(new ToolStripMenuItem(file.FileName, null, ReopenSubtitleToolStripMenuItemClick) { Tag = file.FileName });
                            lowerFileNameList.Add(file.FileName.ToLowerInvariant());
                        }
                    }
                }


                var tss = new ToolStripSeparator();
                UiUtil.FixFonts(tss);

                var clearHistoryMenuItem = new ToolStripMenuItem(LanguageSettings.Current.DvdSubRip.Clear);
                clearHistoryMenuItem.Click += (sender, args) =>
                {
                    Configuration.Settings.RecentFiles.Files.RemoveAll(entry => entry.FileName != _fileName || entry.FileName == _fileName && entry.OriginalFileName != _subtitleOriginalFileName);
                    UpdateRecentFilesUI();
                };
                UiUtil.FixFonts(clearHistoryMenuItem);
            }
            else
            {
                Configuration.Settings.RecentFiles.Files.Clear();
            }
        }

        private void ReopenSubtitleToolStripMenuItemClick(object sender, EventArgs e)
        {
            ReloadFromSourceView();
            var item = sender as ToolStripItem;

            if (ContinueNewOrExit())
            {
                if (!string.IsNullOrEmpty(_fileName) && !_converted)
                {
                    Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                }

                RecentFileEntry rfe = null;
                foreach (var file in Configuration.Settings.RecentFiles.Files.Where(p => !string.IsNullOrEmpty(p.OriginalFileName)))
                {
                    if ((file.FileName + " + " + file.OriginalFileName).Equals(item.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        rfe = file;
                        break;
                    }
                }

                if (rfe == null)
                {
                    foreach (var file in Configuration.Settings.RecentFiles.Files.Where(p => string.IsNullOrEmpty(p.OriginalFileName)))
                    {
                        if (file.FileName.Equals(item.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            rfe = file;
                            RemoveOriginal(true, false);
                            break;
                        }
                    }
                }

                CheckSecondSubtitleReset();
                SubtitleListview1.BeginUpdate();
                if (rfe == null)
                {
                    Interlocked.Increment(ref _openSaveCounter);
                    OpenSubtitle(item.Text, null);
                    Interlocked.Decrement(ref _openSaveCounter);
                }
                else
                {
                    Interlocked.Increment(ref _openSaveCounter);
                    OpenRecentFile(rfe);
                    Interlocked.Decrement(ref _openSaveCounter);
                }

                GotoSubPosAndPause();
                SetRecentIndices(rfe);
                SubtitleListview1.EndUpdate();
                if (rfe != null && !string.IsNullOrEmpty(rfe.VideoFileName))
                {
                    var p = _subtitle.GetParagraphOrDefault(rfe.FirstSelectedIndex);
                }
            }
        }

        private void OpenRecentFile(RecentFileEntry rfe)
        {
            OpenSubtitle(rfe.FileName, null, rfe.VideoFileName, rfe.AudioTrack, rfe.OriginalFileName, false);
            Configuration.Settings.General.CurrentVideoOffsetInMs = rfe.VideoOffsetInMs;
            if (rfe.VideoOffsetInMs != 0)
            {
                _subtitle.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(-Configuration.Settings.General.CurrentVideoOffsetInMs));
                _subtitleListViewIndex = -1;
                _changeSubtitleHash = GetFastSubtitleHash();
                if (IsOriginalEditable)
                {
                    _subtitleOriginal.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(-Configuration.Settings.General.CurrentVideoOffsetInMs));
                    _changeOriginalSubtitleHash = GetFastSubtitleOriginalHash();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }
                else
                {
                    SubtitleListview1.Fill(_subtitle);
                }
            }

            if (rfe.VideoIsSmpte)
            {

                    Configuration.Settings.General.CurrentVideoIsSmpte = true;
                
            }

            if (!Configuration.Settings.General.DisableVideoAutoLoading &&
                rfe.VideoFileName != null && rfe.VideoFileName.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                Configuration.IsRunningOnWindows)
            {
                var isMpvAvailable = LibMpvDynamic.IsInstalled;
                var isYouTubeDlInstalled = File.Exists(Path.Combine(Configuration.DataDirectory, "yt-dlp.exe"));

              
            }
        }

        private void GotoSubPosAndPause()
        {
            if (!string.IsNullOrEmpty(_videoFileName))
            {
                _videoLoadedGoToSubPosAndPause = true;
            }
            else
            {
            }
        }

        private void SetRecentIndices(RecentFileEntry rfe)
        {
            if (!Configuration.Settings.General.RememberSelectedLine)
            {
                return;
            }

            Application.DoEvents();
            if (rfe != null && !string.IsNullOrEmpty(rfe.FileName) &&
                rfe.FirstSelectedIndex >= 0 && rfe.FirstSelectedIndex < SubtitleListview1.Items.Count)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListView1SelectedIndexChange;
                SubtitleListview1.SelectIndexAndEnsureVisible(rfe.FirstSelectedIndex, true);
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();
            }
        }


        private DialogResult FileSaveAs(bool allowUsingDefaultOrLastSaveAsFormat)
        {
            var oldFormat = GetCurrentSubtitleFormat();
            SubtitleFormat currentFormat = null;
            if (allowUsingDefaultOrLastSaveAsFormat)
            {
                if (!string.IsNullOrEmpty(Configuration.Settings.General.DefaultSaveAsFormat))
                {
                    currentFormat = Utilities.GetSubtitleFormatByFriendlyName(Configuration.Settings.General.DefaultSaveAsFormat);
                }
                else if (!string.IsNullOrEmpty(Configuration.Settings.General.LastSaveAsFormat))
                {
                    currentFormat = Utilities.GetSubtitleFormatByFriendlyName(Configuration.Settings.General.LastSaveAsFormat);
                }
            }

            if (currentFormat == null)
            {
                currentFormat = GetCurrentSubtitleFormat();
            }


            var suffix = string.Empty;
            if (_subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible && !string.IsNullOrEmpty(Configuration.Settings.General.TranslationAutoSuffixDefault))
            {
                if (Configuration.Settings.General.TranslationAutoSuffixDefault.StartsWith('<'))
                {
                    var translationLangauge = LanguageAutoDetect.AutoDetectGoogleLanguageOrNull(_subtitle);
                    if (!string.IsNullOrEmpty(translationLangauge))
                    {
                        suffix = "." + translationLangauge;
                    }
                    else
                    {
                        var originalLanguage = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
                        suffix = "." + GenericTranslate.EvaluateDefaultTargetLanguageCode(originalLanguage);
                    }
                }
                else
                {
                    suffix = Configuration.Settings.General.TranslationAutoSuffixDefault;
                }
            }

            if (!string.IsNullOrWhiteSpace(_subtitleOriginalFileName) && Configuration.Settings.General.SaveAsUseFileNameFrom.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                var originalLanguage = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
                var fileNameNoExt = Utilities.GetFileNameWithoutExtension(_subtitleOriginalFileName);
                if (fileNameNoExt.EndsWith("." + originalLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    fileNameNoExt = fileNameNoExt.Substring(0, fileNameNoExt.Length - ("." + originalLanguage).Length);
                }

            }
            else if (!string.IsNullOrWhiteSpace(_subtitleOriginalFileName))
            {
                var originalLanguage = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
                var fileNameNoExt = Utilities.GetFileNameWithoutExtension(_subtitleOriginalFileName);
                if (fileNameNoExt.EndsWith("." + originalLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    fileNameNoExt = fileNameNoExt.Substring(0, fileNameNoExt.Length - ("." + originalLanguage).Length);
                }
            }
            else
            {
            }
            return result;
        }

        private DialogResult SaveSubtitle(SubtitleFormat format, bool useNewLineWithOnly0A = false, bool skipPrompts = false)
        {
            if (string.IsNullOrEmpty(_fileName) || _converted)
            {
                return FileSaveAs(_converted && !_formatManuallyChanged);
            }

            try
            {
                var sub = GetSaveSubtitle(_subtitle);

                if (format != null && !format.IsTextBased)
                {
                    if (format is Ebu ebu)
                    {
                        var header = new Ebu.EbuGeneralSubtitleInformation();
                        if (_subtitle != null && _subtitle.Header != null && (_subtitle.Header.Contains("STL2") || _subtitle.Header.Contains("STL3")))
                        {
                            header = Ebu.ReadHeader(Encoding.UTF8.GetBytes(_subtitle.Header));
                        }

                        if (ebu.Save(_fileName, sub, !_saveAsCalled, header))
                        {
                            _changeSubtitleHash = GetFastSubtitleHash();
                            Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                            Configuration.Settings.Save();
                        }
                    }

                    _oldSubtitleFormat = format;
                    _formatManuallyChanged = false;
                    return DialogResult.OK;
                }

                string allText = sub.ToText(format);

                // Seungki begin
                if (_splitDualSami && _subtitleOriginal?.Paragraphs.Count > 0)
                {
                    var s = new Subtitle(_subtitle);
                    foreach (var p in _subtitleOriginal.Paragraphs)
                    {
                        s.Paragraphs.Add(p);
                    }

                    allText = s.ToText(format);
                }
                // Seungki end

                bool isUnicode = currentEncoding.Equals(Encoding.Unicode) || currentEncoding.Equals(Encoding.UTF32) || currentEncoding.Equals(Encoding.GetEncoding(12001)) || currentEncoding.Equals(Encoding.UTF7) || currentEncoding.Equals(Encoding.UTF8);
                if (!isUnicode)
                {
                    if (!skipPrompts && currentEncoding.GetString(currentEncoding.GetBytes(allText)) != allText)
                    {
                        if (MessageBox.Show(string.Format(_language.UnicodeMusicSymbolsAnsiWarning), Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                        {
                            return DialogResult.No;
                        }
                    }

                    allText = allText.NormalizeUnicode(currentEncoding);
                }

                bool containsNegativeTime = false;
                var negativeTimeLines = new List<string>();
                foreach (var p in sub.Paragraphs)
                {
                    if (p.StartTime.TotalMilliseconds < 0 || p.EndTime.TotalMilliseconds < 0)
                    {
                        containsNegativeTime = true;
                        negativeTimeLines.Add(sub.Paragraphs.IndexOf(p).ToString(CultureInfo.InvariantCulture));
                        if (negativeTimeLines.Count > 10)
                        {
                            negativeTimeLines[negativeTimeLines.Count - 1] = negativeTimeLines[negativeTimeLines.Count - 1] + "...";
                            break;
                        }
                    }
                }

                if (containsNegativeTime && !skipPrompts)
                {
                    if (MessageBox.Show(_language.NegativeTimeWarning + Environment.NewLine +
                                        string.Format(LanguageSettings.Current.MultipleReplace.LinesFoundX, string.Join(", ", negativeTimeLines)),
                        Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    {
                        return DialogResult.No;
                    }
                }

                if (!skipPrompts && File.Exists(_fileName))
                {
                    var fileInfo = new FileInfo(_fileName);
                    var fileOnDisk = fileInfo.LastWriteTime;
                    if (_fileDateTime != fileOnDisk && _fileDateTime != new DateTime())
                    {
                        if (MessageBox.Show(string.Format(_language.OverwriteModifiedFile,
                                _fileName, fileOnDisk.ToShortDateString(), fileOnDisk.ToString("HH:mm:ss"),
                                Environment.NewLine, _fileDateTime.ToShortDateString(), _fileDateTime.ToString("HH:mm:ss")),
                            Title + " - " + _language.FileOnDiskModified, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                        {
                            return DialogResult.No;
                        }
                    }

                    if (fileInfo.IsReadOnly)
                    {
                        MessageBox.Show(string.Format(_language.FileXIsReadOnly, _fileName));
                        return DialogResult.No;
                    }
                }

                // force encoding
                var formatType = format.GetType();

                if (Configuration.Settings.General.ShowFormatRequiresUtf8Warning && !currentEncoding.Equals(Encoding.UTF8) &&
                    (formatType == typeof(DCinemaInterop) || formatType == typeof(DCinemaSmpte2007) ||
                     formatType == typeof(DCinemaSmpte2010) || formatType == typeof(DCinemaSmpte2014)))
                {
                    using (var form = new DialogDoNotShowAgain(Title, string.Format(_language.FormatXShouldUseUft8, GetCurrentSubtitleFormat().FriendlyName)))
                    {
                        form.ShowDialog(this);
                        Configuration.Settings.General.ShowFormatRequiresUtf8Warning = !form.DoNoDisplayAgain;
                    }
                }


                if (useNewLineWithOnly0A)
                {
                    allText = allText.Replace("\r\n", "\n");
                }

                if (formatType == typeof(ItunesTimedText) || formatType == typeof(ScenaristClosedCaptions) || formatType == typeof(ScenaristClosedCaptionsDropFrame))
                {
                    var outputEnc = new UTF8Encoding(false); // create encoding with no BOM
                    using (var file = new StreamWriter(_fileName, false, outputEnc)) // open file with encoding
                    {
                        file.Write(allText);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(allText))
                    {
                        MessageBox.Show(string.Format(_language.UnableToSaveSubtitleX, _fileName), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return DialogResult.Cancel;
                    }
                }

                Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                Configuration.Settings.Save();
                new BookmarkPersistence(_subtitle, _fileName).Save();
                _fileDateTime = File.GetLastWriteTime(_fileName);
                _oldSubtitleFormat = format;
                _formatManuallyChanged = false;
                ShowStatus(string.Format(_language.SavedSubtitleX, _fileName));
                if (formatType == typeof(NetflixTimedText))
                {
                    NetflixGlyphCheck(true);
                }

                _changeSubtitleHash = GetFastSubtitleHash();
                return DialogResult.OK;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return DialogResult.Cancel;
            }
            finally
            {
            }
        }

        private DialogResult SaveOriginalSubtitle(SubtitleFormat format, bool skipPrompts = false)
        {
            try
            {
                var subAlt = GetSaveSubtitle(_subtitleOriginal);

                bool containsNegativeTime = false;
                foreach (var p in subAlt.Paragraphs)
                {
                    if (p.StartTime.TotalMilliseconds < 0 || p.EndTime.TotalMilliseconds < 0)
                    {
                        containsNegativeTime = true;
                        break;
                    }
                }

                if (!skipPrompts && containsNegativeTime)
                {
                    if (MessageBox.Show(_language.NegativeTimeWarning, Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                    {
                        return DialogResult.No;
                    }
                }

                if (format != null && !format.IsTextBased)
                {
                    if (format is Ebu ebu)
                    {
                        if (ebu.Save(_subtitleOriginalFileName, subAlt))
                        {
                            Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                            Configuration.Settings.Save();
                            ShowStatus(string.Format(_language.SavedOriginalSubtitleX, _subtitleOriginalFileName));
                            _changeOriginalSubtitleHash = GetFastSubtitleOriginalHash();
                            return DialogResult.OK;
                        }

                        return DialogResult.No;
                    }

                    MessageBox.Show("Ups - save original does not support this format - please go to Github and create an issue!");
                }

                string allText = subAlt.ToText(format);
                ShowStatus(string.Format(_language.SavedOriginalSubtitleX, _subtitleOriginalFileName));
                _changeOriginalSubtitleHash = GetFastSubtitleOriginalHash();
                return DialogResult.OK;
            }
            catch
            {
                MessageBox.Show(string.Format(_language.UnableToSaveSubtitleX, _subtitleOriginalFileName));
                return DialogResult.Cancel;
            }
        }


        private void ResetSubtitle()
        {
            if (ModifierKeys != Keys.Shift)
            {
                _videoFileName = null;
                _videoInfo = null;
            }

            SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);


            _isOriginalActive = false;
            Configuration.Settings.General.CurrentVideoOffsetInMs = 0;
            Configuration.Settings.General.CurrentVideoIsSmpte = false;
            _subtitle = new Subtitle(_subtitle.HistoryItems);
            _changeOriginalSubtitleHash = -1;
            _changeSubtitleHash = -1;
            _changeSubtitleTextHash = -1;
            _subtitleOriginalFileName = null;
            SubtitleListview1.Items.Clear();
            _fileName = string.Empty;
            _fileDateTime = new DateTime();
            _oldSubtitleFormat = null;
            RemoveOriginal(true, false);
            _splitDualSami = false;
            _imageSubFileName = null;

            SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);

            var oldDisableShowStatus = _disableShowStatus;
            _disableShowStatus = true;
            _disableShowStatus = oldDisableShowStatus;


            _findHelper = null;
            _spellCheckForm = null;

            if (ModifierKeys != Keys.Shift)
            {
                _videoFileName = null;
                _videoInfo = null;
                VideoAudioTrackNumber = -1;
            }

            _sourceViewChange = false;
            EnableOrDisableEditControls();

            _listViewTextUndoLast = null;
            _listViewOriginalTextUndoLast = null;
            _listViewTextUndoIndex = -1;

            _changeSubtitleHash = GetFastSubtitleHash();
            _converted = false;

            SetTitle();
            SetUndockedWindowsTitle();
            ShowStatus(_language.New);

            ResetShowEarlierOrLater();

            // Set default RTL or LTR

            SetListViewStateImages();
        }

        private void ResetShowEarlierOrLater()
        {
            try
            {
                if (_showEarlierOrLater != null && !_showEarlierOrLater.IsDisposed)
                {
                    _showEarlierOrLater.ResetTotalAdjustment();
                }
            }
            catch
            {
                // form closing or alike
            }
        }

        private void FileNew()
        {
            if (ContinueNewOrExit())
            {
                if (Configuration.Settings.General.ShowRecentFiles && File.Exists(_fileName))
                {
                    Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                }

                if (IsSubtitleLoaded)
                {
                    MakeHistoryForUndo(_language.BeforeNew);
                }

                ResetSubtitle();
                CheckSecondSubtitleReset();
            }
        }

        private string _lastChangedToFormat;


        private static List<string> GetNuendoStyles()
        {
            if (!string.IsNullOrEmpty(Configuration.Settings.SubtitleSettings.NuendoCharacterListFile) && File.Exists(Configuration.Settings.SubtitleSettings.NuendoCharacterListFile))
            {
                return NuendoProperties.LoadCharacters(Configuration.Settings.SubtitleSettings.NuendoCharacterListFile);
            }

            return new List<string>();
        }

        private SubtitleFormat GetCurrentSubtitleFormat()
        {
            if (_currentSubtitleFormat == null)
            {
                _currentSubtitleFormat = Utilities.GetSubtitleFormatByFriendlyName(comboBoxSubtitleFormats.SelectedItem.ToString());
                MakeFormatChange(_currentSubtitleFormat, _oldSubtitleFormat);
            }

            return _currentSubtitleFormat;
        }

        private void ShowSource()
        {
            if (_subtitle != null && _subtitle.Paragraphs.Count > 0)
            {
                SubtitleFormat format = GetCurrentSubtitleFormat();
                if (format != null)
                {
                    return;
                }
            }
        }

        private void CheckAndGetNewlyDownloadedMpvDlls(string message)
        {
           

            var newMpvFiles = Directory.GetFiles(Configuration.DataDirectory, "*.dll.new-mpv");
            if (newMpvFiles.Length <= 0)
            {
                return;
            }

            foreach (string newDllFileName in newMpvFiles)
            {
                if (File.Exists(newDllFileName)) // dll was in use, so unload + copy new dll + load
                {
                    try
                    {
                        string targetFileName = newDllFileName.Replace(".dll.new-mpv", ".dll");
                        File.Copy(newDllFileName, targetFileName, true);
                        File.Delete(newDllFileName);
                        ShowStatus("libmpv updated");
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
            }
        }

        private static void TryLoadIcon(ToolStripItem button, string iconName)
        {
            button.BackColor = UiUtil.BackColor;

            var theme = Configuration.Settings.General.UseDarkTheme ? "DarkTheme" : "DefaultTheme";
            if (!string.IsNullOrEmpty(Configuration.Settings.General.ToolbarIconTheme) && !Configuration.Settings.General.ToolbarIconTheme.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                theme = Configuration.Settings.General.ToolbarIconTheme;
            }

            var themeFullPath = Path.Combine(Configuration.IconsDirectory, theme, iconName + ".png");
            if (File.Exists(themeFullPath))
            {
                button.Image?.Dispose();
                button.Image = new Bitmap(themeFullPath);
                return;
            }

            var fullPath = Path.Combine(Configuration.IconsDirectory, "DefaultTheme", iconName + ".png");
            if (File.Exists(fullPath))
            {
                button.Image?.Dispose();
                button.Image = new Bitmap(fullPath);
            }
        }

        private void InitializeToolbarAndImages()
        {
            UpdateToolbarButtonsToCurrentFormat(GetCurrentSubtitleFormat());
        }



        private void SaveAll(bool useOnly0AForNewLine = false)
        {
            if (_subtitle == null || _subtitle.Paragraphs.Count == 0)
            {
                ShowStatus(_language.CannotSaveEmptySubtitle);
                return;
            }

            ReloadFromSourceView();
            _disableShowStatus = true;
            _saveAsCalled = false;
            var result = SaveSubtitle(GetCurrentSubtitleFormat(), useOnly0AForNewLine);
            if (result != DialogResult.OK)
            {
                _disableShowStatus = false;
                return;
            }

            if (IsOriginalEditable && _subtitleOriginal.Paragraphs.Count > 0)
            {
                SaveOriginalToolStripMenuItemClick(null, null);
                _disableShowStatus = false;
                ShowStatus(string.Format(_language.SavedSubtitleX, Path.GetFileName(_fileName)) + " + " +
                           string.Format(_language.SavedOriginalSubtitleX, $"\"{_subtitleOriginalFileName}\""));
                return;
            }

            _disableShowStatus = false;
            ShowStatus(string.Format(_language.SavedSubtitleX, $"\"{_fileName}\""));

            if (Configuration.Settings.General.ShowNegativeDurationInfoOnSave)
            {
                var sb = new StringBuilder();
                for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
                {
                    var p = _subtitle.Paragraphs[index];
                    if (p.DurationTotalMilliseconds < 0 && !p.StartTime.IsMaxTime && !p.EndTime.IsMaxTime)
                    {
                        if (sb.Length < 20)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(", ");
                            }

                            sb.Append((index + 1).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            sb.Append("...");
                            break;
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    using (var form = new DialogDoNotShowAgain(Title, string.Format(_language.SubtitleContainsNegativeDurationsX, sb.ToString())))
                    {
                        form.ShowDialog(this);
                        Configuration.Settings.General.ShowNegativeDurationInfoOnSave = !form.DoNoDisplayAgain;
                    }
                }
            }
        }

        private void TextBoxSourceTextChanged(object sender, EventArgs e)
        {
            ShowSourceLineNumber();
            _sourceViewChange = true;
            labelStatus.Text = string.Empty;
            _sourceTextTicks = DateTime.UtcNow.Ticks;
        }

        private bool ShowProfileInStatusBar => Configuration.Settings.General.CurrentProfile != "Default";

        private void ShowSourceLineNumber()
        {
            if (InSourceView)
            {
                var profile = Configuration.Settings.General.CurrentProfile + "   ";
                if (!ShowProfileInStatusBar)
                {
                    profile = string.Empty;
                }

                toolStripSelected.Text = profile + string.Format(_language.LineNumberX, textBoxSource.GetLineFromCharIndex(textBoxSource.SelectionStart) + 1);
            }
        }


        private void Find()
        {
            string selectedText;
            if (InSourceView)
            {
                selectedText = textBoxSource.SelectedText;
            }
            else
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    selectedText = textBoxListViewTextOriginal.SelectedText;
                }
                else
                {
                    selectedText = textBoxListViewText.SelectedText;
                }
            }

            if (selectedText.Length == 0 && _findHelper != null)
            {
                if (_clearLastFind)
                {
                    _clearLastFind = false;
                    _findHelper.FindReplaceType.FindType = _clearLastFindType;
                    selectedText = _clearLastFindText;
                }
                else
                {
                    selectedText = _findHelper.FindText;
                }
            }

            var left = 0;
            var top = 0;
            if (_findDialog != null)
            {
                left = _findDialog.Left;
                top = _findDialog.Top;
            }

            _findDialog?.Dispose();
            _findDialog = new FindDialog(_subtitle, this);
            _findDialog.SetIcon(toolStripButtonFind.Image as Bitmap);
            _findDialog.Initialize(selectedText, _findHelper);

            if (left <= 0 || top <= 0)
            {
                left = Left + Width / 2 - _findDialog.Width / 2;
                top = Top + Height / 2 - _findDialog.Height / 2;
            }

            _findDialog.Left = left;
            _findDialog.Top = top;

            _findDialog.Show(this);
        }

        public void FindDialogClose()
        {
            if (_findHelper != null)
            {
                _findHelper.InProgress = false;
                _findHelper.MatchInOriginal = false;
                _findHelper.SelectedPosition = -1;
            }

            Focus();
        }

        public void FindDialogFindPrevious(string findText)
        {
            _findHelper = _findHelper ?? _findDialog.GetFindDialogHelper(_subtitleListViewIndex);
            _findHelper.FindText = findText;
            _findHelper.FindTextLength = findText.Length;
            FindPrevious();
        }

        public void FindDialogFind(string findText, ReplaceType findReplaceType, Regex regex)
        {
            _findHelper = _findHelper ?? _findDialog.GetFindDialogHelper(_subtitleListViewIndex);
            _findHelper.FindText = findText;
            _findHelper.FindTextLength = findText.Length;
            _findHelper.FindReplaceType = findReplaceType;
            if (findReplaceType.FindType == FindType.RegEx)
            {
                _findHelper.SetRegex(regex);
            }

            DialogFind(_findHelper);
        }



        private SETextBox GetFindReplaceTextBox()
        {
            return _findHelper.MatchInOriginal ? textBoxListViewTextOriginal : textBoxListViewText;
        }

        private void SelectListViewIndexAndEnsureVisible(int index)
        {
            SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
            SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
            SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            SubtitleListview1_SelectedIndexChanged(null, null);
        }

        private void FindPrevious()
        {
            if (_findHelper == null)
            {
                return;
            }

            _findHelper.InProgress = true;
            var tb = GetFindReplaceTextBox();
            if (InListView)
            {
                int selectedIndex = -1;
                if (SubtitleListview1.SelectedItems.Count > 0)
                {
                    selectedIndex = SubtitleListview1.SelectedItems[0].Index;
                }

                int textBoxStart = tb.SelectionStart;
                if (_findHelper.SelectedPosition - 1 == tb.SelectionStart && tb.SelectionLength > 0 ||
                    _findHelper.FindText.Equals(tb.SelectedText, StringComparison.OrdinalIgnoreCase))
                {
                    textBoxStart = tb.SelectionStart - 1;
                }

                if (_findHelper.FindPrevious(_subtitle, _subtitleOriginal, selectedIndex, textBoxStart, Configuration.Settings.General.AllowEditOfOriginalSubtitle))
                {
                    tb = GetFindReplaceTextBox();
                    SelectListViewIndexAndEnsureVisible(_findHelper.SelectedLineIndex);
                    ShowStatus(string.Format(_language.XFoundAtLineNumberY, _findHelper.FindText, _findHelper.SelectedLineIndex + 1));
                    tb.Focus();
                    tb.SelectionStart = _findHelper.SelectedPosition;
                    tb.SelectionLength = _findHelper.FindTextLength;
                    _findHelper.SelectedPosition--;
                }
                else
                {
                    ShowStatus(string.Format(_language.XNotFound, _findHelper.FindText));
                }
            }
            else if (InSourceView)
            {
                if (_findHelper.FindPrevious(textBoxSource.Text, textBoxSource.SelectionStart))
                {
                    textBoxSource.SelectionStart = _findHelper.SelectedLineIndex;
                    textBoxSource.SelectionLength = _findHelper.FindTextLength;
                    textBoxSource.ScrollToCaret();
                    ShowStatus(string.Format(_language.XFoundAtLineNumberY, _findHelper.FindText, textBoxSource.GetLineFromCharIndex(textBoxSource.SelectionStart)));
                }
                else
                {
                    ShowStatus(string.Format(_language.XNotFound, _findHelper.FindText));
                }
            }

            _findHelper.InProgress = false;
        }

        private void ReplaceSourceViewStart()
        {
            string selectedText = textBoxSource.SelectedText;
            if (selectedText.Length == 0 && _findHelper != null)
            {
                selectedText = _findHelper.FindText;
            }

            if (_replaceDialog == null || _replaceDialog.IsDisposed)
            {
                _replaceDialog = new ReplaceDialog(this);
                _replaceDialog.SetIcon(toolStripButtonReplace.Image as Bitmap);
                _findHelper = _findHelper ?? _replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);

                _replaceDialog.Left = Left + Width / 2 - _replaceDialog.Width / 2;
                _replaceDialog.Top = Left + Height / 2 - _replaceDialog.Height / 2;
            }

            _replaceDialog.Initialize(selectedText, _findHelper, IsOriginalEditable && SubtitleListview1.IsOriginalTextColumnVisible);
            if (!_replaceDialog.Visible)
            {
                _replaceDialog.Show(this);
            }
        }

        public void ReplaceDialogFind(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            _findHelper = findReplaceDialogHelper;

            if (_findHelper != null)
            {
                if (_findHelper.SelectedLineIndex != _subtitleListViewIndex)
                {
                    _findHelper.SelectedLineIndex = _subtitleListViewIndex;
                    _findHelper.SelectedPosition = -1;
                    _findHelper.MatchInOriginal = false;
                    _findHelper.ReplaceFromPosition = 0;
                }

                DialogFind(_findHelper);
                return;
            }

            DialogFind(_replaceDialog.GetFindDialogHelper(_subtitleListViewIndex));
        }

        public void DialogFind(FindReplaceDialogHelper findHelper)
        {
            _findHelper = findHelper;
            _findHelper.InProgress = true;
            if (!string.IsNullOrWhiteSpace(_findHelper.FindText))
            {
                if (Configuration.Settings.Tools.FindHistory.Count == 0 || Configuration.Settings.Tools.FindHistory[0] != _findHelper.FindText)
                {
                    Configuration.Settings.Tools.FindHistory.Insert(0, _findHelper.FindText);
                }
            }

            ShowStatus(string.Format(_language.SearchingForXFromLineY, _findHelper.FindText, _subtitleListViewIndex + 1));
            if (InListView)
            {
                var tb = GetFindReplaceTextBox();
                int startPos = tb.SelectedText.Length > 0 ? tb.SelectionStart + 1 : tb.SelectionStart;
                bool found = _findHelper.Find(_subtitle, _subtitleOriginal, _subtitleListViewIndex, startPos);
                // if we fail to find the text, we might want to start searching from the top of the file.
                if (!found && !(_subtitleListViewIndex == 0 && _findHelper.SelectedPosition <= 0))
                {
                    if (MessageBox.Show(_language.FindContinue, _language.FindContinueTitle, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        _findHelper.StartLineIndex = 0;
                        _findHelper.StartFindText = _findHelper.FindText;
                        _findHelper.SelectedLineIndex = 0;
                        _findHelper.MatchInOriginal = false;
                        _findHelper.SelectedPosition = -1;
                        textBoxListViewText.SelectionStart = 0;
                        textBoxListViewText.SelectionLength = 0;
                        textBoxListViewTextOriginal.SelectionStart = 0;
                        textBoxListViewTextOriginal.SelectionLength = 0;
                        found = _findHelper.Find(_subtitle, _subtitleOriginal, -1);
                    }
                }

                if (found)
                {
                    SelectListViewIndexAndEnsureVisible(_findHelper.SelectedLineIndex);
                    textBoxListViewText.SelectionStart = 0;
                    textBoxListViewText.SelectionLength = 0;
                    textBoxListViewTextOriginal.SelectionStart = 0;
                    textBoxListViewTextOriginal.SelectionLength = 0;
                    tb = GetFindReplaceTextBox();
                    tb.SelectionLength = 0;
                    tb.Focus();
                    tb.SelectionStart = _findHelper.SelectedPosition;
                    tb.SelectionLength = _findHelper.FindTextLength;
                    ShowStatus(string.Format(_language.XFoundAtLineNumberY, _findHelper.FindText, _findHelper.SelectedLineIndex + 1));
                    _findHelper.SelectedPosition++;
                }
                else
                {
                    ShowStatus(string.Format(_language.XNotFound, _findHelper.FindText));
                }
            }
            else if (InSourceView)
            {
                if (_findHelper.Find(textBoxSource, textBoxSource.SelectionStart))
                {
                    textBoxSource.SelectionStart = _findHelper.SelectedLineIndex;
                    textBoxSource.SelectionLength = _findHelper.FindTextLength;
                    textBoxSource.ScrollToCaret();
                    ShowStatus(string.Format(_language.XFoundAtLineNumberY, _findHelper.FindText, textBoxSource.GetLineFromCharIndex(textBoxSource.SelectionStart)));
                }
                else
                {
                    ShowStatus(string.Format(_language.XNotFound, _findHelper.FindText));
                }
            }
        }

        public void ReplaceDialogReplace(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            if (InListView)
            {
                ReplaceDialogReplaceListView();
            }
            else
            {
                ReplaceDialogReplaceSourceView();
            }
        }

        public void ReplaceDialogReplaceListView()
        {
            _findHelper.InProgress = true;
            var line = _findHelper.SelectedLineIndex;
            var pos = _findHelper.ReplaceFromPosition;
            var success = _findHelper.Success;
            var matchInOriginal = _findHelper.MatchInOriginal;
            _findHelper = _replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
            _findHelper.SelectedLineIndex = line;
            _findHelper.SelectedPosition = pos;
            _findHelper.Success = success;
            _findHelper.MatchInOriginal = matchInOriginal;
            _findHelper.InProgress = true;

            ShowStatus(string.Format(_language.SearchingForXFromLineY, _findHelper.FindText, _subtitleListViewIndex + 1));
            var tb = GetFindReplaceTextBox();
            string msg = string.Empty;
            if (_findHelper.FindReplaceType.FindType == FindType.RegEx)
            {
                if (_findHelper.Success)
                {
                    if (_findHelper.FindReplaceType.FindType == FindType.RegEx)
                    {
                        ReplaceViaRegularExpression(tb, _replaceDialog.ReplaceAll);
                    }
                    else
                    {
                        tb.SelectedText = _findHelper.ReplaceText;
                    }

                    msg = _language.OneReplacementMade + " ";
                }
            }
            else if (tb.SelectionLength == _findHelper.FindTextLength)
            {
                tb.SelectedText = _findHelper.ReplaceText;
                msg = _language.OneReplacementMade + " ";
                _findHelper.SelectedPosition += _findHelper.ReplaceText.Length;
            }

            if (_findHelper.FindNext(_subtitle, _subtitleOriginal, _findHelper.SelectedLineIndex, _findHelper.SelectedPosition, Configuration.Settings.General.AllowEditOfOriginalSubtitle))
            {
                SelectListViewIndexAndEnsureVisible(_findHelper.SelectedLineIndex);
                textBoxListViewText.SelectionStart = 0;
                textBoxListViewText.SelectionLength = 0;
                textBoxListViewTextOriginal.SelectionStart = 0;
                textBoxListViewTextOriginal.SelectionLength = 0;
                tb = GetFindReplaceTextBox();
                tb.Focus();
                tb.SelectionStart = _findHelper.SelectedPosition;
                tb.SelectionLength = _findHelper.FindTextLength;
                if (_findHelper.FindReplaceType.FindType != FindType.RegEx)
                {
                    _findHelper.SelectedPosition += _findHelper.ReplaceText.Length;
                }

                ShowStatus(string.Format(msg + _language.XFoundAtLineNumberY, _findHelper.FindText, _findHelper.SelectedLineIndex + 1));
            }
            else
            {
                ShowStatus(msg + string.Format(_language.XNotFound, _findHelper.FindText));

                // Prompt for start over
                if (!(_subtitleListViewIndex == 0 && _findHelper.SelectedPosition <= 0))
                {
                    if (MessageBox.Show(_language.FindContinue, _language.FindContinueTitle, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        SelectListViewIndexAndEnsureVisible(0);
                        _findHelper.StartLineIndex = 0;
                        _findHelper.StartFindText = _findHelper.FindText;
                        _findHelper.SelectedLineIndex = 0;
                        _findHelper.SelectedPosition = 0;
                        _findHelper.ReplaceFromPosition = 0;
                        if (_findHelper.FindNext(_subtitle, _subtitleOriginal, _findHelper.SelectedLineIndex, _findHelper.SelectedPosition, Configuration.Settings.General.AllowEditOfOriginalSubtitle))
                        {
                            SelectListViewIndexAndEnsureVisible(_findHelper.SelectedLineIndex);
                            tb = GetFindReplaceTextBox();
                            tb.Focus();
                            tb.SelectionStart = _findHelper.SelectedPosition;
                            tb.SelectionLength = _findHelper.FindTextLength;
                            _findHelper.SelectedPosition += _findHelper.ReplaceText.Length;
                            ShowStatus(string.Format(msg + _language.XFoundAtLineNumberY, _findHelper.FindText, _findHelper.SelectedLineIndex + 1));
                        }
                    }
                }
            }

            _findHelper.InProgress = false;
        }

        public void ReplaceDialogReplaceSourceView()
        {
            _findHelper = _replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
            ShowStatus(string.Format(_language.SearchingForXFromLineY, _findHelper.FindText, _subtitleListViewIndex + 1));

            int replaceCount = 0;
            var searchStringFound = false;
            int start = textBoxSource.SelectionStart;

            MakeHistoryForUndo(string.Format(_language.BeforeReplace, _findHelper.FindText));
            _makeHistoryPaused = true;
            if (start >= 0)
            {
                start--;
            }

            if (_findHelper.FindNext(textBoxSource.Text, start))
            {
                textBoxSource.SelectionStart = _findHelper.SelectedLineIndex;
                textBoxSource.SelectionLength = _findHelper.FindTextLength;
                if (!_replaceDialog.FindOnly)
                {
                    textBoxSource.SelectedText = _findHelper.ReplaceText;
                }

                textBoxSource.ScrollToCaret();

                replaceCount++;
                searchStringFound = true;

                if (!_replaceDialog.FindOnly)
                {
                    if (_findHelper.FindNext(textBoxSource.Text, start))
                    {
                        textBoxSource.SelectionStart = _findHelper.SelectedLineIndex;
                        textBoxSource.SelectionLength = _findHelper.FindTextLength;
                        textBoxSource.ScrollToCaret();
                    }

                    Replace();
                    return;
                }
            }

            if (_replaceDialog.FindOnly)
            {
                if (searchStringFound)
                {
                    ShowStatus(string.Format(_language.MatchFoundX, _findHelper.FindText));
                }
                else
                {
                    ShowStatus(string.Format(_language.NoMatchFoundX, _findHelper.FindText));
                }

                Replace();
                return;
            }

            ReloadFromSourceView();
            if (replaceCount == 0)
            {
                ShowStatus(_language.FoundNothingToReplace);
            }
            else
            {
                ShowStatus(string.Format(_language.ReplaceCountX, replaceCount));
            }
        }
        public void ReplaceDialogReplaceAll(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            _findHelper = _replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
            ShowStatus(string.Format(_language.SearchingForXFromLineY, _findHelper.FindText, _subtitleListViewIndex + 1));

            if (InListView)
            {
                ListViewReplaceAll(_replaceDialog);
            }
            else
            {
                SourceListReplaceAll(_replaceDialog, _findHelper);
            }
        }

        public void ReplaceDialogClose()
        {
            if (_makeHistoryPaused)
            {
                RestartHistory();
            }

            if (_findHelper != null)
            {
                _findHelper.InProgress = false;
                _findHelper.MatchInOriginal = false;
                _findHelper.SelectedPosition = -1;
            }

            Focus();
        }

        public bool GetAllowReplaceInOriginal()
        {
            return IsOriginalEditable;
        }

        public void ListViewReplaceAll(ReplaceDialog replaceDialog)
        {
            if (_findHelper == null)
            {
                _findHelper = replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
                _findHelper.InProgress = true;
            }
            else
            {
                var line = _findHelper.SelectedLineIndex;
                var pos = _findHelper.ReplaceFromPosition;
                var success = _findHelper.Success;
                var matchInOriginal = _findHelper.MatchInOriginal;
                _findHelper = replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
                _findHelper.SelectedLineIndex = line;
                _findHelper.SelectedPosition = pos;
                _findHelper.Success = success;
                _findHelper.MatchInOriginal = matchInOriginal;
                _findHelper.InProgress = true;
            }

            var isFirst = true;
            var replaceCount = 0;
            var searchStringFound = true;
            var stopAtIndex = int.MaxValue;
            var firstIndex = FirstSelectedIndex;
            var searchedFromTop = firstIndex == 0 && _findHelper.ReplaceFromPosition == 0;
            while (searchStringFound)
            {
                searchStringFound = false;
                if (isFirst)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeReplace, _findHelper.FindText));
                    isFirst = false;
                    _makeHistoryPaused = true;
                }

                if (replaceDialog.ReplaceAll)
                {
                    replaceCount = ReplaceAllHelper.ReplaceAll(_findHelper, _subtitle, _subtitleOriginal, Configuration.Settings.General.AllowEditOfOriginalSubtitle, stopAtIndex);
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();

                    string msgText = _language.ReplaceContinueNotFound;
                    if (replaceCount > 0)
                    {
                        msgText = string.Format(_language.ReplaceXContinue, replaceCount);
                    }

                    if (!searchedFromTop && MessageBox.Show(msgText, _language.ReplaceContinueTitle, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        stopAtIndex = firstIndex;
                        _findHelper.StartLineIndex = 0;
                        _findHelper.StartFindText = _findHelper.FindText;
                        _findHelper.SelectedLineIndex = 0;
                        _findHelper.MatchInOriginal = false;
                        _findHelper.SelectedPosition = -1;
                        textBoxListViewText.SelectionStart = 0;
                        textBoxListViewText.SelectionLength = 0;
                        textBoxListViewTextOriginal.SelectionStart = 0;
                        textBoxListViewTextOriginal.SelectionLength = 0;
                        replaceCount = ReplaceAllHelper.ReplaceAll(_findHelper, _subtitle, _subtitleOriginal, Configuration.Settings.General.AllowEditOfOriginalSubtitle, stopAtIndex);
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    }
                }

                UpdateSourceView();
                if (replaceCount == 0)
                {
                    ShowStatus(_language.FoundNothingToReplace);
                }
                else
                {
                    ShowStatus(string.Format(_language.ReplaceCountX, replaceCount));
                    SubtitleListview1.SyntaxColorAllLines(_subtitle);
                }
            }

            RestoreSubtitleListviewIndices();
            if (_makeHistoryPaused)
            {
                RestartHistory();
            }

            _findHelper.InProgress = false;
        }

        private void SourceListReplaceAll(ReplaceDialog replaceDialog, FindReplaceDialogHelper findHelper)
        {
            _makeHistoryPaused = true;

            if (_findHelper.FindReplaceType.FindType == FindType.RegEx)
            {
                SourceListReplaceAllRegEx(replaceDialog);
                return;
            }

            var replaceCount = 0;
            var searchStringFound = true;
            var start = textBoxSource.SelectionStart;
            var originalSelectionStart = textBoxSource.SelectionStart;
            var isFirst = true;
            var text = textBoxSource.Text;
            while (searchStringFound)
            {
                searchStringFound = false;
                if (isFirst)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeReplace, _findHelper.FindText));
                    isFirst = false;
                    _makeHistoryPaused = true;
                    if (start >= 0)
                    {
                        start--;
                    }
                }
                else
                {
                    start--;
                }

                if (_findHelper.FindNext(text, start))
                {
                    text = text.Remove(findHelper.SelectedLineIndex, findHelper.FindTextLength).Insert(findHelper.SelectedLineIndex, findHelper.ReplaceText);
                    start = findHelper.SelectedLineIndex + findHelper.FindTextLength;
                    replaceCount++;
                    searchStringFound = true;
                }
            }

            textBoxSource.Text = text;
            ReloadFromSourceView();

            if (originalSelectionStart < text.Length)
            {
                textBoxSource.SelectionStart = originalSelectionStart;
            }
            textBoxSource.SelectionLength = 0;

            if (replaceCount == 0)
            {
                ShowStatus(_language.FoundNothingToReplace);
            }
            else
            {
                ShowStatus(string.Format(_language.ReplaceCountX, replaceCount));
            }

            if (_makeHistoryPaused)
            {
                RestartHistory();
            }

            replaceDialog.Dispose();
        }

        private void SourceListReplaceAllRegEx(ReplaceDialog replaceDialog)
        {
            var start = textBoxSource.SelectionStart;
            var s = textBoxSource.Text;
            var r = new Regex(_findHelper.FindText, RegexOptions.Multiline);
            var matches = r.Matches(s, start);

            if (matches.Count > 0)
            {
                MakeHistoryForUndo(string.Format(_language.BeforeReplace, _findHelper.FindText));
            }

            var result = RegexUtils.ReplaceNewLineSafe(r, s, _findHelper.ReplaceText, int.MaxValue, start);

            // update UI
            textBoxSource.Text = result;
            ShowStatus(matches.Count == 0 ? _language.FoundNothingToReplace : string.Format(_language.ReplaceCountX, matches.Count));

            // replace again from beginning
            if (start > 1)
            {
                string msgText = _language.ReplaceContinueNotFound;
                if (matches.Count > 0)
                {
                    msgText = string.Format(_language.ReplaceXContinue, matches.Count);
                }

                if (MessageBox.Show(msgText, _language.ReplaceContinueTitle, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    s = result.Substring(0, start - 1);
                    var rest = result.Remove(0, start - 1);
                    if (s.EndsWith('\r') && rest.StartsWith('\n'))
                    { // don't split "\r\n"
                        s = s.Substring(0, s.Length - 1);
                        rest = '\r' + rest;
                    }

                    matches = r.Matches(s);
                    result = RegexUtils.ReplaceNewLineSafe(r, s, _findHelper.ReplaceText);

                    // update UI
                    textBoxSource.Text = result + rest;
                    ShowStatus(matches.Count == 0 ? _language.FoundNothingToReplace : string.Format(_language.ReplaceCountX, matches.Count));
                }
            }

            replaceDialog.Dispose();
            ReloadFromSourceView();
        }

        private void ReplaceListViewStart()
        {
            SaveSubtitleListviewIndices();
            string selectedText;
            if (textBoxListViewTextOriginal.Focused)
            {
                selectedText = textBoxListViewTextOriginal.SelectedText;
            }
            else
            {
                selectedText = textBoxListViewText.SelectedText;
            }

            if (selectedText.Length == 0 && _findHelper != null)
            {
                selectedText = _findHelper.FindText;
            }

            if (_replaceDialog == null || _replaceDialog.IsDisposed || _findHelper == null || !_replaceDialog.Visible)
            {
                _replaceDialog?.Dispose();
                _replaceDialog = new ReplaceDialog(this);
                _replaceDialog.Left = Left + Width / 2 - _replaceDialog.Width / 2;
                _replaceDialog.Top = Left + Height / 2 - _replaceDialog.Height / 2;
                _replaceDialog.SetIcon(toolStripButtonReplace.Image as Bitmap);
                _findHelper = _findHelper ?? _replaceDialog.GetFindDialogHelper(_subtitleListViewIndex);
                _findHelper.InProgress = true;
                int index = 0;

                if (SubtitleListview1.SelectedItems.Count > 0)
                {
                    index = SubtitleListview1.SelectedItems[0].Index;
                }

                _findHelper.SelectedLineIndex = index;
                if (textBoxListViewTextOriginal.Focused)
                {
                    _findHelper.SelectedPosition = textBoxListViewTextOriginal.SelectionStart;
                    _findHelper.ReplaceFromPosition = _findHelper.SelectedPosition;
                }
                else
                {
                    _findHelper.SelectedPosition = textBoxListViewText.SelectionStart;
                    _findHelper.ReplaceFromPosition = _findHelper.SelectedPosition;
                }
            }
            else
            {
                if (_findHelper != null)
                {
                    selectedText = _findHelper.FindText;
                    _findHelper.InProgress = true;
                }
            }

            _replaceDialog.Initialize(selectedText, _findHelper, IsOriginalEditable && SubtitleListview1.IsOriginalTextColumnVisible);
            if (!_replaceDialog.Visible)
            {
                _replaceDialog.Show(this);
            }

            _replaceDialog.Activate();
            _replaceDialog.Focus();
            var scr = Screen.FromControl(this);
            var x = _replaceDialog.Left + _replaceDialog.Width / 2;
            var y = _replaceDialog.Top + _replaceDialog.Height / 2;
            if (x < 0 || x > scr.Bounds.Right || y < 0 || y > scr.Bounds.Bottom)
            {
                _replaceDialog.Left = scr.Bounds.Left + scr.Bounds.Width / 2 - _replaceDialog.Width / 2;
                _replaceDialog.Top = scr.Bounds.Top + scr.Bounds.Height / 2 - _replaceDialog.Height / 2;
            }
        }

        private void ReplaceViaRegularExpression(SETextBox tb, bool replaceAll)
        {
            var r = new Regex(RegexUtils.FixNewLine(_findHelper.FindText), RegexOptions.Multiline);
            if (replaceAll)
            {
                string result = RegexUtils.ReplaceNewLineSafe(r, tb.Text, _findHelper.ReplaceText);
                if (result != tb.Text)
                {
                    tb.Text = result;
                }

                _findHelper.SelectedPosition = tb.Text.Length;
                _findHelper.ReplaceFromPosition = _findHelper.SelectedPosition;
            }
            else
            {
                string result = RegexUtils.ReplaceNewLineSafe(r, tb.Text, _findHelper.ReplaceText, 1, _findHelper.SelectedPosition);
                if (result != tb.Text)
                {
                    var match = r.Match(string.Join(Environment.NewLine, tb.Text.SplitToLines()));
                    if (match != null && match.Success && !_findHelper.FindText.StartsWith('^') && _findHelper.ReplaceText.Length > 0)
                    {
                        var add = Math.Abs(match.Length - _findHelper.ReplaceText.Length);
                        _findHelper.SelectedPosition += add;
                        _findHelper.ReplaceFromPosition = _findHelper.SelectedPosition;
                    }

                    tb.Text = result;
                }

                if (_findHelper.FindText.StartsWith('^'))
                {
                    _findHelper.SelectedPosition++;
                }
            }
        }

        private void Replace()
        {
            if (InSourceView)
            {
                ReplaceSourceViewStart();
            }
            else
            {
                ReplaceListViewStart();
            }
        }

        public void ShowStatus(string message, bool log = true, int clearAfterSeconds = 0, bool isError = false)
        {
            if (_disableShowStatus)
            {
                return;
            }

            _timerClearStatus.Stop();
            labelStatus.Text = message.Replace("&", "&&");
            statusStrip1.Refresh();
            if (!string.IsNullOrEmpty(message))
            {
                labelStatus.ForeColor = isError ? Color.Red : UiUtil.ForeColor;
                if (log)
                {
                    _timerClearStatus.Interval = clearAfterSeconds > 0 ? clearAfterSeconds : (Configuration.Settings.General.ClearStatusBarAfterSeconds * 1000);
                    _statusLog.Add(string.Format("{0:0000}-{1:00}-{2:00} {3}: {4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.ToLongTimeString(), message));
                }
                else
                {
                    _timerClearStatus.Interval = clearAfterSeconds > 0 ? (clearAfterSeconds * 1000) : 1500;
                }

                _timerClearStatus.Start();
            }
            else
            {
                _timerClearStatus.Stop();
            }

            ShowSourceLineNumber();
            ShowLineInformationListView();
        }

        private void ReloadFromSourceView()
        {
            if (_sourceViewChange)
            {
                SaveSubtitleListviewIndices();
                if (!string.IsNullOrWhiteSpace(textBoxSource.Text))
                {
                    var oldSubtitle = new Subtitle(_subtitle);
                    var format = GetCurrentSubtitleFormat();
                    var list = textBoxSource.Lines.ToList();
                    format = new Subtitle().ReloadLoadSubtitle(list, null, format);
                    if (format == null && !string.IsNullOrWhiteSpace(textBoxSource.Text))
                    {
                        MessageBox.Show(_language.UnableToParseSourceView);
                        return;
                    }

                    _sourceViewChange = false;
                    MakeHistoryForUndo(_language.BeforeChangesMadeInSourceView);
                    _subtitle.ReloadLoadSubtitle(list, null, format);

                    int index = 0;
                    foreach (string formatName in comboBoxSubtitleFormats.Items)
                    {
                        if (formatName == format.FriendlyName)
                        {
                            comboBoxSubtitleFormats.SelectedIndex = index;
                            break;
                        }

                        index++;
                    }

                    for (var i = 0; i < oldSubtitle.Paragraphs.Count; i++)
                    {
                        if (oldSubtitle.Paragraphs[i].Bookmark != null)
                        {
                            var newParagraph = _subtitle.GetFirstAlike(oldSubtitle.Paragraphs[i]);
                            if (newParagraph != null)
                            {
                                newParagraph.Bookmark = oldSubtitle.Paragraphs[i].Bookmark;
                            }
                        }
                    }

                    if (Configuration.Settings.General.CurrentVideoOffsetInMs != 0)
                    {
                        _subtitle.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(-Configuration.Settings.General.CurrentVideoOffsetInMs));
                    }

                    var formatType = format.GetType();
                    if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
                    {
                        string errors = AdvancedSubStationAlpha.CheckForErrors(_subtitle.Header);
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (formatType == typeof(SubRip))
                    {
                        string errors = (format as SubRip).Errors;
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (formatType == typeof(MicroDvd))
                    {
                        string errors = (format as MicroDvd).Errors;
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (formatType == typeof(DCinemaSmpte2007))
                    {
                        format.ToText(_subtitle, string.Empty);
                        string errors = (format as DCinemaSmpte2007).Errors;
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (formatType == typeof(DCinemaSmpte2010))
                    {
                        format.ToText(_subtitle, string.Empty);
                        string errors = (format as DCinemaSmpte2010).Errors;
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (formatType == typeof(DCinemaSmpte2014))
                    {
                        format.ToText(_subtitle, string.Empty);
                        string errors = (format as DCinemaSmpte2014).Errors;
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show(errors, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    _sourceViewChange = false;
                    MakeHistoryForUndo(_language.BeforeChangesMadeInSourceView);
                    _sourceViewChange = false;
                    _subtitle.Paragraphs.Clear();
                    EnableOrDisableEditControls();
                }

                _subtitleListViewIndex = -1;
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                RestoreSubtitleListviewIndices();
            }
        }

        private void RecalcCurrentDuration(bool onlyOptimal = false)
        {
            if (SubtitleListview1.SelectedItems.Count >= 1)
            {
                MakeHistoryForUndo(_language.BeforeDisplayTimeAdjustment);
                _makeHistoryPaused = true;
                var idx = SubtitleListview1.SelectedItems[0].Index;
                _subtitle.RecalculateDisplayTime(Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds, idx, Configuration.Settings.General.SubtitleOptimalCharactersPerSeconds, onlyOptimal: onlyOptimal);
                SetDurationInSeconds(_subtitle.Paragraphs[idx].DurationTotalSeconds);
                _makeHistoryPaused = false;
            }
        }

        private void RecalcCurrentDurationMin()
        {
            if (SubtitleListview1.SelectedItems.Count >= 1)
            {
                MakeHistoryForUndo(_language.BeforeDisplayTimeAdjustment);
                _makeHistoryPaused = true;
                var idx = SubtitleListview1.SelectedItems[0].Index;
                _subtitle.RecalculateDisplayTime(Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds, idx, Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds);
                SetDurationInSeconds(_subtitle.Paragraphs[idx].DurationTotalSeconds);
                _makeHistoryPaused = false;
            }
        }

        internal void ReloadFromSubtitle(Subtitle subtitle, string messageForUndo)
        {
            MakeHistoryForUndo(messageForUndo);
            var firstSelected = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
            _subtitle.Paragraphs.Clear();
            _subtitle.Paragraphs.AddRange(subtitle.Paragraphs);
            UpdateSourceView();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            _subtitleListViewIndex = -1;
            if (firstSelected != null)
            {
                var newSelected = _subtitle.GetNearestAlike(firstSelected);
                if (newSelected != null)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(_subtitle.GetIndex(newSelected), true);
                    return;
                }
            }

            SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
        }



        private void SplitToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!IsSubtitleLoaded)
            {
                DisplaySubtitleNotLoadedMessage();
                return;
            }

            ReloadFromSourceView();
            double lengthInSeconds = 0;
            if (mediaPlayer.VideoPlayer != null)
            {
                lengthInSeconds = mediaPlayer.Duration;
            }

            if (Configuration.Settings.Tools.SplitAdvanced)
            {
                using (var split = new Split())
                {
                    split.Initialize(_subtitle, _fileName, GetCurrentSubtitleFormat());
                    if (split.ShowDialog(this) == DialogResult.OK)
                    {
                        ShowStatus(_language.SubtitleSplitted);
                    }
                    else if (split.ShowBasic)
                    {
                        Configuration.Settings.Tools.SplitAdvanced = false;
                        SplitToolStripMenuItemClick(null, null);
                    }
                }
            }
            else
            {
                using (var splitSubtitle = new SplitSubtitle())
                {
                    splitSubtitle.Initialize(_subtitle, _fileName, GetCurrentSubtitleFormat(), GetCurrentEncoding(), lengthInSeconds);
                    if (splitSubtitle.ShowDialog(this) == DialogResult.OK)
                    {
                        ShowStatus(_language.SubtitleSplitted);
                    }
                    else if (splitSubtitle.ShowAdvanced)
                    {
                        Configuration.Settings.Tools.SplitAdvanced = true;
                        SplitToolStripMenuItemClick(null, null);
                    }
                }
            }
        }



        /// <summary>
        /// Undo or Redo
        /// </summary>
        /// <param name="undo">True equals undo, false triggers redo</param>
        private void UndoToIndex(bool undo)
        {
            if (_networkSession != null)
            {
                return;
            }

            lock (_syncUndo)
            {
                if (!undo && _undoIndex >= _subtitle.HistoryItems.Count - 1)
                {
                    return;
                }

                if (undo && (_subtitle == null || !_subtitle.CanUndo || _undoIndex < 0))
                {
                    return;
                }

                // Add latest changes if any (also stop changes from being added while redoing/undoing)
                timerTextUndo.Stop();
                timerOriginalTextUndo.Stop();
                _listViewTextTicks = 0;
                _listViewOriginalTextTicks = 0;
                TimerTextUndoTick(null, null);
                TimerOriginalTextUndoTick(null, null);

                try
                {
                    var undoLineIndex = -1;
                    int selectedIndex = FirstSelectedIndex;
                    if (undo)
                    {
                        _subtitle.HistoryItems[_undoIndex].RedoParagraphs = new List<Paragraph>();
                        _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal = new List<Paragraph>();
                        undoLineIndex = _subtitle.HistoryItems[_undoIndex].LineIndex;

                        foreach (var p in _subtitle.Paragraphs)
                        {
                            _subtitle.HistoryItems[_undoIndex].RedoParagraphs.Add(new Paragraph(p));
                        }

                        if (IsOriginalEditable)
                        {
                            foreach (var p in _subtitleOriginal.Paragraphs)
                            {
                                _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal.Add(new Paragraph(p));
                            }
                        }

                        _subtitle.HistoryItems[_undoIndex].RedoFileName = _fileName;
                        _subtitle.HistoryItems[_undoIndex].RedoFileModified = _fileDateTime;
                        _subtitle.HistoryItems[_undoIndex].RedoOriginalFileName = _subtitleOriginalFileName;

                        if (selectedIndex >= 0)
                        {
                            _subtitle.HistoryItems[_undoIndex].RedoParagraphs[selectedIndex].Text =
                                textBoxListViewText.Text;
                            if (IsOriginalEditable &&
                                selectedIndex < _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal.Count)
                            {
                                _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal[selectedIndex].Text =
                                    textBoxListViewTextOriginal.Text;
                            }

                            _subtitle.HistoryItems[_undoIndex].RedoLineIndex = selectedIndex;
                            _subtitle.HistoryItems[_undoIndex].RedoLinePosition = textBoxListViewText.SelectionStart;
                            _subtitle.HistoryItems[_undoIndex].RedoLinePositionOriginal = textBoxListViewTextOriginal.SelectionStart;
                        }
                        else
                        {
                            _subtitle.HistoryItems[_undoIndex].RedoLineIndex = -1;
                            _subtitle.HistoryItems[_undoIndex].RedoLinePosition = -1;
                        }
                    }
                    else
                    {
                        _undoIndex++;
                    }

                    var text = _subtitle.HistoryItems[_undoIndex].Description;

                    _subtitleListViewIndex = -1;
                    textBoxListViewText.Text = string.Empty;
                    textBoxListViewTextOriginal.Text = string.Empty;
                    string oldFileName = _fileName;
                    DateTime oldFileDateTime = _fileDateTime;

                    string oldAlternameFileName = _subtitleOriginalFileName;
                    _fileName = _subtitle.UndoHistory(_undoIndex, out var subtitleFormatFriendlyName, out _fileDateTime, out _subtitleOriginal, out _subtitleOriginalFileName);
                    if (string.IsNullOrEmpty(oldAlternameFileName) && !string.IsNullOrEmpty(_subtitleOriginalFileName))
                    {
                        SubtitleListview1.ShowOriginalTextColumn(_languageGeneral.OriginalText);
                        SubtitleListview1.AutoSizeAllColumns(this);
                    }
                    else if (SubtitleListview1.IsOriginalTextColumnVisible && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count == 0)
                    {
                        RemoveOriginal(true, false);
                    }

                    if (!undo)
                    {
                        // TODO: Sometimes redo paragraphs can be null - how?
                        if (_subtitle.HistoryItems[_undoIndex].RedoParagraphs != null)
                        {
                            _subtitle.Paragraphs.Clear();
                            foreach (var p in _subtitle.HistoryItems[_undoIndex].RedoParagraphs)
                            {
                                _subtitle.Paragraphs.Add(new Paragraph(p));
                            }

                            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null)
                            {
                                _subtitleOriginal.Paragraphs.Clear();
                                foreach (var p in _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal)
                                {
                                    _subtitleOriginal.Paragraphs.Add(new Paragraph(p));
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Undo failed at undo index: " + _undoIndex);
                        }

                        _subtitle.HistoryItems[_undoIndex].RedoParagraphs = null;
                        _subtitle.HistoryItems[_undoIndex].RedoParagraphsOriginal = null;
                        if (SubtitleListview1.IsOriginalTextColumnVisible && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count == 0)
                        {
                            RemoveOriginal(true, false);
                        }
                    }

                    if (oldFileName == null || oldFileName.Equals(_fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        _fileDateTime = oldFileDateTime; // undo will not give overwrite-newer-file warning
                    }

                    if (GetCurrentSubtitleFormat().FriendlyName != subtitleFormatFriendlyName)
                    {
                        var oldPaused = _makeHistoryPaused;
                        _makeHistoryPaused = true;
                        SetCurrentFormat(subtitleFormatFriendlyName);
                        _makeHistoryPaused = oldPaused;
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);

                    if (undoLineIndex >= 0 && undoLineIndex < _subtitle.Paragraphs.Count)
                    {
                        selectedIndex = undoLineIndex;
                    }

                    if (selectedIndex >= _subtitle.Paragraphs.Count)
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(_subtitle.Paragraphs.Count - 1, true);
                    }
                    else if (selectedIndex >= 0 && selectedIndex < _subtitle.Paragraphs.Count)
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(selectedIndex, true);
                    }
                    else
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    }

                    audioVisualizer.Invalidate();
                    if (undo)
                    {
                        if (_subtitle.HistoryItems[_undoIndex].LineIndex == FirstSelectedIndex)
                        {
                            textBoxListViewText.SelectionStart = _subtitle.HistoryItems[_undoIndex].LinePosition;
                            if (_subtitleOriginal != null)
                            {
                                textBoxListViewTextOriginal.SelectionStart =
                                    _subtitle.HistoryItems[_undoIndex].LinePositionOriginal;
                            }
                        }

                        ShowStatus(_language.UndoPerformed + ": " + text.Replace(Environment.NewLine, "  "));
                        _undoIndex--;
                    }
                    else
                    {
                        if (_subtitle.HistoryItems[_undoIndex].RedoLineIndex >= 0 &&
                            _subtitle.HistoryItems[_undoIndex].RedoLineIndex == FirstSelectedIndex)
                        {
                            textBoxListViewText.SelectionStart = _subtitle.HistoryItems[_undoIndex].RedoLinePosition;
                        }

                        if (_subtitleOriginal != null && _subtitle.HistoryItems[_undoIndex].RedoLineIndex >= 0 &&
                            _subtitle.HistoryItems[_undoIndex].RedoLineIndex == FirstSelectedIndex)
                        {
                            textBoxListViewTextOriginal.SelectionStart = _subtitle.HistoryItems[_undoIndex].RedoLinePositionOriginal;
                        }

                        _fileName = _subtitle.HistoryItems[_undoIndex].RedoFileName;
                        _subtitleOriginalFileName = _subtitle.HistoryItems[_undoIndex].RedoOriginalFileName;
                        ShowStatus(_language.RedoPerformed);
                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }

                timerTextUndo.Start();
                timerOriginalTextUndo.Start();
                SetTitle();
                SetListViewStateImages();
                EnableOrDisableEditControls();
            }
        }

        private void LiveSpellCheckTimer_Tick(object sender, EventArgs e)
        {
            _liveSpellCheckTimer.Stop();
            InitializeLiveSpellChcek();
            _liveSpellCheckTimer.Start();
        }

        private void InitializeLiveSpellChcek()
        {
            if (IsSubtitleLoaded)
            {
                var hash = _subtitle.GetFastHashCodeTextOnly();
                if (!textBoxListViewText.IsSpellCheckerInitialized && textBoxListViewText.IsDictionaryDownloaded)
                {
                    textBoxListViewText.InitializeLiveSpellCheck(_subtitle, FirstSelectedIndex);
                }
                else if (_changeSubtitleTextHash != hash)
                {
                    textBoxListViewText.CheckForLanguageChange(_subtitle);
                    _changeSubtitleTextHash = hash;
                }

                if (textBoxListViewText.LanguageChanged)
                {
                    if (textBoxListViewText.IsDictionaryDownloaded)
                    {
                        ShowStatus(string.Format(LanguageSettings.Current.SpellCheck.LiveSpellCheckLanguage, textBoxListViewText.CurrentLanguage), true);
                    }
                    else
                    {
                        ShowStatus(string.Format(LanguageSettings.Current.SpellCheck.NoDictionaryForLiveSpellCheck, textBoxListViewText.CurrentLanguage), true);
                    }

                    textBoxListViewText.LanguageChanged = false;
                }
            }
            else if (textBoxListViewText.IsSpellCheckerInitialized)
            {
                textBoxListViewText.DisposeHunspellAndDictionaries();
                textBoxListViewText.IsDictionaryDownloaded = true;
            }
        }

        public void ChangeWholeTextMainPart(ref int noOfChangedWords, ref bool firstChange, int i, Paragraph p)
        {
            SubtitleListview1.SetText(i, p.Text);
            noOfChangedWords++;
            if (firstChange)
            {
                MakeHistoryForUndo(_language.BeforeSpellCheck);
                firstChange = false;
            }

            UpdateSourceView();
            RefreshSelectedParagraph();
        }

        public void DeleteLine()
        {
            MakeHistoryForUndo(LanguageSettings.Current.Main.OneLineDeleted);
            DeleteSelectedLines();
        }

        public void FocusParagraph(int index)
        {
            if (InSourceView)
            {
                SwitchView(ListView);
            }
            else if (InListView)
            {
                SelectListViewIndexAndEnsureVisible(index);
            }
        }

        private void RefreshSelectedParagraph()
        {
            if (!InListView)
            {
                return;
            }

            var idx = FirstSelectedIndex;
            if (idx == -1 && _subtitle?.Paragraphs?.Count > 0)
            {
                idx = 0;
            }

            var p = _subtitle.GetParagraphOrDefault(idx);
            _subtitleListViewIndex = -1;
            SubtitleListview1_SelectedIndexChanged(null, null);
            if (p != null)
            {
                SubtitleListview1.SetStartTimeAndDuration(idx, p, _subtitle.GetParagraphOrDefault(idx + 1), _subtitle.GetParagraphOrDefault(idx - 1));
            }
        }

        private void RefreshSelectedParagraphs()
        {
            foreach (var index in SubtitleListview1.GetSelectedIndices())
            {
                var p = _subtitle.GetParagraphOrDefault(index);
                if (p != null)
                {
                    SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                }
            }

            _subtitleListViewIndex = -1;
            SubtitleListview1_SelectedIndexChanged(null, null);
        }

        private int GetPositionFromWordIndex(string text, int wordIndex)
        {
            var sb = new StringBuilder();
            int index = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (SpellCheckWordLists.SplitChars.Contains(text[i]))
                {
                    if (sb.Length > 0)
                    {
                        index++;
                        if (index == wordIndex)
                        {
                            int pos = i - sb.Length;
                            if (pos > 0)
                            {
                                pos--;
                            }

                            if (pos >= 0)
                            {
                                return pos;
                            }
                        }
                    }

                    sb.Clear();
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            if (sb.Length > 0)
            {
                index++;
                if (index == wordIndex)
                {
                    int pos = text.Length - 1 - sb.Length;
                    if (pos >= 0)
                    {
                        return pos;
                    }
                }
            }

            return 0;
        }

        public void CorrectWord(string changeWord, Paragraph p, string oldWord, ref bool firstChange, int wordIndex)
        {
            if (oldWord == changeWord)
            {
                return;
            }

            if (firstChange)
            {
                MakeHistoryForUndo(_language.BeforeSpellCheck);
                firstChange = false;
            }

            int startIndex = p.Text.IndexOf(oldWord, StringComparison.Ordinal);
            if (wordIndex >= 0)
            {
                startIndex = p.Text.IndexOf(oldWord, GetPositionFromWordIndex(p.Text, wordIndex), StringComparison.Ordinal);
            }

            while (startIndex >= 0 && startIndex < p.Text.Length && p.Text.Substring(startIndex).Contains(oldWord))
            {
                bool startOk = startIndex == 0 ||
                               "«»“” <>-—+/'\"[](){}¿¡….,;:!?%&$£\r\n؛،؟\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u00A0\u200B\uFEFF".Contains(p.Text[startIndex - 1]) ||
                               char.IsPunctuation(p.Text[startIndex - 1]) ||
                               startIndex == p.Text.Length - oldWord.Length;
                if (startOk)
                {
                    int end = startIndex + oldWord.Length;
                    if (end == p.Text.Length ||
                        "«»“” ,.!?:;'()<>\"-—+/[]{}%&$£…\r\n؛،؟\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u00A0\u200B\uFEFF".Contains(p.Text[end]) ||
                        char.IsPunctuation(p.Text[end]))
                    {
                        var endOk = true;

                        if (changeWord.EndsWith('\'') && end < p.Text.Length && p.Text[end] == '\'')
                        {
                            endOk = false;
                        }

                        if (endOk)
                        {
                            var lengthBefore = p.Text.Length;
                            p.Text = p.Text.Remove(startIndex, oldWord.Length).Insert(startIndex, changeWord);
                            var lengthAfter = p.Text.Length;
                            if (lengthAfter > lengthBefore)
                            {
                                startIndex += (lengthAfter - lengthBefore);
                            }
                        }
                    }
                }

                if (startIndex + 2 >= p.Text.Length)
                {
                    startIndex = -1;
                }
                else
                {
                    startIndex = p.Text.IndexOf(oldWord, startIndex + 2, StringComparison.Ordinal);
                }

                // stop if using index
                if (wordIndex >= 0)
                {
                    startIndex = -1;
                }
            }

            ShowStatus(string.Format(_language.SpellCheckChangedXToY, oldWord, changeWord));
            SubtitleListview1.SetText(_subtitle.GetIndex(p), p.Text);
            UpdateSourceView();
            RefreshSelectedParagraph();
        }

        private void ExtractAudioSelectedLines()
        {
            if (!RequireFfmpegOk())
            {
                return;
            }

            var audioClips = GetAudioClips();
            UiUtil.OpenFolder(Path.GetDirectoryName(audioClips[0].AudioFileName));
        }

        private void AudioToTextVoskSelectedLines()
        {
            if (!RequireFfmpegOk())
            {
                return;
            }

            var audioClips = GetAudioClips();

            if (audioClips.Count == 1 && audioClips[0].Paragraph.DurationTotalMilliseconds > 10_000)
            {
                using (var form = new VoskAudioToText(audioClips[0].AudioFileName, _videoAudioTrackNumber, this))
                {
                    var result = form.ShowDialog(this);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    if (form.TranscribedSubtitle.Paragraphs.Count == 0)
                    {
                        MessageBox.Show(LanguageSettings.Current.AudioToText.NoTextFound);
                        return;
                    }

                    _subtitle.Paragraphs.RemoveAll(p => p.Id == audioClips[0].Paragraph.Id);

                    foreach (var p in form.TranscribedSubtitle.Paragraphs)
                    {
                        p.Adjust(1, audioClips[0].Paragraph.StartTime.TotalSeconds);
                        _subtitle.InsertParagraphInCorrectTimeOrder(p);
                    }

                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();
                    RefreshSelectedParagraph();
                }
            }

            using (var form = new VoskAudioToTextSelectedLines(audioClips, this))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, string.Format(LanguageSettings.Current.Main.Menu.Video.VideoAudioToTextX, "Vosk/Kaldi")));
                    SubtitleListview1.BeginUpdate();
                    foreach (var ac in audioClips)
                    {
                        var p = _subtitle.Paragraphs.FirstOrDefault(pa => pa.Id == ac.Paragraph.Id);
                        if (p != null)
                        {
                            p.Text = ac.Paragraph.Text;
                            var idx = _subtitle.Paragraphs.IndexOf(p);
                            SubtitleListview1.SetText(idx, p.Text);
                        }
                    }

                    SubtitleListview1.EndUpdate();
                    RefreshSelectedParagraph();
                }
            }
        }

        private void AudioToTextWhisperSelectedLines()
        {
            if (!RequireFfmpegOk())
            {
                return;
            }

            var audioClips = GetAudioClips();
            if (audioClips.Count == 1 && audioClips[0].Paragraph.DurationTotalMilliseconds > 8_000)
            {
                var s = new Subtitle();
                s.Paragraphs.Add(audioClips[0].Paragraph);

                using (var form = new WhisperAudioToText(audioClips[0].AudioFileName, s, _videoAudioTrackNumber, this, null))
                {
                    var result = form.ShowDialog(this);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    if (form.TranscribedSubtitle.Paragraphs.Count == 0)
                    {
                        MessageBox.Show(LanguageSettings.Current.AudioToText.NoTextFound);
                        return;
                    }

                    _subtitle.Paragraphs.RemoveAll(p => p.Id == audioClips[0].Paragraph.Id);

                    foreach (var p in form.TranscribedSubtitle.Paragraphs)
                    {
                        p.Adjust(1, audioClips[0].Paragraph.StartTime.TotalSeconds);
                        _subtitle.InsertParagraphInCorrectTimeOrder(p);
                    }

                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();
                    RefreshSelectedParagraph();
                }

                return;
            }

            using (var form = new WhisperAudioToTextSelectedLines(audioClips, this))
            {
                CheckWhisperCpp();
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, string.Format(LanguageSettings.Current.Main.Menu.Video.VideoAudioToTextX, "Whisper")));
                    SubtitleListview1.BeginUpdate();
                    foreach (var ac in audioClips)
                    {
                        var p = _subtitle.Paragraphs.FirstOrDefault(pa => pa.Id == ac.Paragraph.Id);
                        if (p != null)
                        {
                            p.Text = ac.Paragraph.Text;
                            var idx = _subtitle.Paragraphs.IndexOf(p);
                            SubtitleListview1.SetText(idx, p.Text);
                        }
                    }

                    SubtitleListview1.EndUpdate();
                    RefreshSelectedParagraph();
                }
            }
        }

        private List<AudioClipsGet.AudioClip> GetAudioClips()
        {
            var selectedParagraphs = new List<Paragraph>();
            foreach (var index in SubtitleListview1.GetSelectedIndices())
            {
                selectedParagraphs.Add(new Paragraph(_subtitle.Paragraphs[index], false));
            }

            using (var form = new AudioClipsGet(selectedParagraphs, _videoFileName, _videoAudioTrackNumber))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    return form.AudioClips;
                }
            }

            return null;
        }

        private void SetActor(string actor)
        {
            if (!string.IsNullOrEmpty(actor))
            {
                MakeHistoryForUndo(LanguageSettings.Current.Main.Menu.ContextMenu.SetActor + ": " + actor);

                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    _subtitle.Paragraphs[index].Actor = actor;
                    SubtitleListview1.SetTimeAndText(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1));
                }
            }
        }

        private void WebVTTSetVoice(string voice)
        {
            if (!string.IsNullOrEmpty(voice))
            {
                MakeHistoryForUndo("Set voice: " + voice);
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    _subtitle.Paragraphs[index].Text = WebVTT.RemoveTag("v", _subtitle.Paragraphs[index].Text);
                    _subtitle.Paragraphs[index].Text = string.Format("<v {0}>{1}", voice, _subtitle.Paragraphs[index].Text);
                    SubtitleListview1.SetText(index, _subtitle.Paragraphs[index].Text);
                }

                RefreshSelectedParagraph();
            }
        }

        private void SetNewActor(object sender, EventArgs e)
        {
            using (var form = new TextPrompt(LanguageSettings.Current.Main.Menu.ContextMenu.NewActor.Replace("...", string.Empty), LanguageSettings.Current.General.Actor, string.Empty))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    string actor = form.InputText;
                    if (!string.IsNullOrEmpty(actor))
                    {
                        foreach (int index in SubtitleListview1.SelectedIndices)
                        {
                            _subtitle.Paragraphs[index].Actor = actor;
                            SubtitleListview1.SetTimeAndText(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1));
                        }
                    }
                }
            }
        }

        private void WebVTTSetNewVoiceTextBox(object sender, EventArgs e)
        {
            using (var form = new TextPrompt(LanguageSettings.Current.WebVttNewVoice.Title, LanguageSettings.Current.WebVttNewVoice.VoiceName, string.Empty))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    string voice = form.InputText;
                    if (!string.IsNullOrEmpty(voice))
                    {
                        var tb = GetFocusedTextBox();

                        if (tb.SelectionLength > 0)
                        {
                            string s = tb.SelectedText;
                            s = WebVTT.RemoveTag("v", s);
                            if (tb.SelectedText == tb.Text)
                            {
                                s = string.Format("<v {0}>{1}", voice, s);
                            }
                            else
                            {
                                s = string.Format("<v {0}>{1}</v>", voice, s);
                            }

                            tb.SelectedText = s;
                        }
                    }
                }
            }
        }
        private void ListViewToggleTag(string tag)
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 0)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(string.Format(_language.BeforeAddingTagX, tag));

                var indices = new List<int>();
                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    indices.Add(item.Index);
                }

                var first = true;
                var toggleOn = true;

                SubtitleListview1.BeginUpdate();
                var isAssa = IsAssa();
                foreach (int i in indices)
                {
                    var p = _subtitle.GetParagraphOrDefault(i);
                    if (p != null)
                    {
                        if (first)
                        {
                            toggleOn = !HtmlUtil.IsTagOn(p.Text, tag, true, isAssa);
                            first = false;
                        }

                        if (IsOriginalEditable)
                        {
                            var original = Utilities.GetOriginalParagraph(i, p, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                if (toggleOn)
                                {
                                    original.Text = HtmlUtil.TagOn(original.Text, tag, true, isAssa);
                                }
                                else
                                {
                                    original.Text = HtmlUtil.TagOff(original.Text, tag, true, isAssa);
                                }

                                SubtitleListview1.SetOriginalText(i, original.Text);
                            }
                        }

                        if (toggleOn)
                        {
                            p.Text = HtmlUtil.TagOn(p.Text, tag, true, isAssa);
                        }
                        else
                        {
                            p.Text = HtmlUtil.TagOff(p.Text, tag, true, isAssa);
                        }

                        SubtitleListview1.SetText(i, p.Text);
                    }
                }

                SubtitleListview1.EndUpdate();

                ShowStatus(string.Format(_language.TagXAdded, isAssa ? tag : "<" + tag + ">"));
                UpdateSourceView();
                RefreshSelectedParagraph();
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            }
        }
        private void ToolStripMenuItemDeleteClick(object sender, EventArgs e)
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 0)
            {
                string statusText;
                string historyText;
                string askText;

                if (SubtitleListview1.SelectedItems.Count > 1)
                {
                    statusText = string.Format(_language.XLinesDeleted, SubtitleListview1.SelectedItems.Count);
                    historyText = string.Format(_language.BeforeDeletingXLines, SubtitleListview1.SelectedItems.Count);
                    askText = string.Format(_language.DeleteXLinesPrompt, SubtitleListview1.SelectedItems.Count);
                }
                else
                {
                    statusText = _language.OneLineDeleted;
                    historyText = _language.BeforeDeletingOneLine;
                    askText = _language.DeleteOneLinePrompt;
                }

                if (Configuration.Settings.General.PromptDeleteLines && MessageBox.Show(askText, Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    _cutText = string.Empty;
                    return;
                }

                if (!string.IsNullOrEmpty(_cutText))
                {
                    ClipboardSetText(_cutText);
                    _cutText = string.Empty;
                }

                MakeHistoryForUndo(historyText);
                DeleteSelectedLines();

                EnableOrDisableEditControls();

                ShowStatus(statusText);
                UpdateSourceView();
            }
        }
        private void EnableOrDisableEditControls()
        {
            if (_subtitle.Paragraphs.Count == 0)
            {
                _subtitleListViewIndex = -1;
                ShowHideBookmark(new Paragraph());

                var focused = FindFocusedControl(this);
                if (focused?.Name == MainTextBox)
                {
                    SubtitleListview1.Focus();
                }
            }
        }

        public static Control FindFocusedControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }

            return control;
        }

        private void DeleteSelectedLines()
        {
            _subtitleListViewIndex = -1;
            textBoxListViewText.Text = string.Empty;
            textBoxListViewTextOriginal.Text = string.Empty;
            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                var originalIndices = new List<int>();
                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    var p = _subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        var original = Utilities.GetOriginalParagraph(item.Index, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            originalIndices.Add(_subtitleOriginal.GetIndex(original));
                        }
                    }
                }

                originalIndices.Reverse();
                foreach (int i in originalIndices)
                {
                    if (i < _subtitleOriginal.Paragraphs.Count)
                    {
                        _subtitleOriginal.Paragraphs.RemoveAt(i);
                    }
                }

                _subtitleOriginal.Renumber();
            }

            var indices = new List<int>();
            foreach (ListViewItem item in SubtitleListview1.SelectedItems)
            {
                indices.Add(item.Index);
            }

            int firstIndex = SubtitleListview1.SelectedItems[0].Index;

            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(indices, 0, null);
            }
            else
            {
                indices.Reverse();
                foreach (int i in indices)
                {
                    _subtitle.Paragraphs.RemoveAt(i);
                }

                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                if (SubtitleListview1.FirstVisibleIndex == 0)
                {
                    SubtitleListview1.FirstVisibleIndex = -1;
                }

                if (SubtitleListview1.Items.Count > firstIndex)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(firstIndex, true);
                }
                else if (SubtitleListview1.Items.Count > 0)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(SubtitleListview1.Items.Count - 1, true);
                }
            }

            EnableOrDisableEditControls();
            SetListViewStateImages();
        }

        private void InsertBefore()
        {
            MakeHistoryForUndo(_language.BeforeInsertLine);

            int firstSelectedIndex = 0;
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                firstSelectedIndex = SubtitleListview1.SelectedItems[0].Index;
            }

            int addMilliseconds = MinGapBetweenLines + 1;
            if (addMilliseconds < 1)
            {
                addMilliseconds = 1;
            }

            var newParagraph = new Paragraph();

            SetStyleForNewParagraph(newParagraph, firstSelectedIndex);

            var prev = _subtitle.GetParagraphOrDefault(firstSelectedIndex - 1);
            var next = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
            if (prev != null && next != null)
            {
                newParagraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - addMilliseconds;
                newParagraph.StartTime.TotalMilliseconds = newParagraph.EndTime.TotalMilliseconds - 2000;
                if (newParagraph.StartTime.TotalMilliseconds <= prev.EndTime.TotalMilliseconds)
                {
                    newParagraph.StartTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds + 1;
                }

                if (newParagraph.DurationTotalMilliseconds < 100)
                {
                    newParagraph.EndTime.TotalMilliseconds += 100;
                }

                if (next.StartTime.IsMaxTime && prev.EndTime.IsMaxTime)
                {
                    newParagraph.StartTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                }
                else if (next.StartTime.TotalMilliseconds == 0 && prev.EndTime.TotalMilliseconds == 0)
                {
                    newParagraph.StartTime.TotalMilliseconds = 0;
                    newParagraph.EndTime.TotalMilliseconds = 0;
                }
                else if (prev.StartTime.TotalMilliseconds == next.StartTime.TotalMilliseconds &&
                         prev.EndTime.TotalMilliseconds == next.EndTime.TotalMilliseconds)
                {
                    newParagraph.StartTime.TotalMilliseconds = prev.StartTime.TotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds;
                }
            }
            else if (prev != null)
            {
                newParagraph.StartTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds + addMilliseconds;
                newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds + Configuration.Settings.General.NewEmptyDefaultMs;
                if (newParagraph.StartTime.TotalMilliseconds > newParagraph.EndTime.TotalMilliseconds)
                {
                    newParagraph.StartTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds + 1;
                }
            }
            else if (next != null)
            {
                newParagraph.StartTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - (2000 + MinGapBetweenLines);
                newParagraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;

                if (next.StartTime.IsMaxTime)
                {
                    newParagraph.StartTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                }
                else if (next.StartTime.TotalMilliseconds == 0 && next.EndTime.TotalMilliseconds == 0)
                {
                    newParagraph.StartTime.TotalMilliseconds = 0;
                    newParagraph.EndTime.TotalMilliseconds = 0;
                }
            }
            else
            {
                newParagraph.StartTime.TotalMilliseconds = 1000;
                newParagraph.EndTime.TotalMilliseconds = 3000;
                if (newParagraph.DurationTotalMilliseconds < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                {
                    newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds +
                                                             Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
                }
            }

            if (newParagraph.Duration.TotalMilliseconds < 100)
            {
                newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
            }

            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(new List<int>(), firstSelectedIndex, newParagraph);
            }
            else
            {
                _subtitle.Paragraphs.Insert(firstSelectedIndex, newParagraph);
                _subtitleListViewIndex = -1;
                _subtitle.Renumber();

                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible)
                {
                    var currentOriginal = Utilities.GetOriginalParagraph(firstSelectedIndex, _subtitle.Paragraphs[firstSelectedIndex], _subtitleOriginal.Paragraphs);
                    if (currentOriginal != null)
                    {
                        _subtitleOriginal.Paragraphs.Insert(_subtitleOriginal.Paragraphs.IndexOf(currentOriginal), new Paragraph(newParagraph));
                    }
                    else
                    {
                        _subtitleOriginal.InsertParagraphInCorrectTimeOrder(new Paragraph(newParagraph));
                    }

                    _subtitleOriginal.Renumber();
                }

                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            }

            SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
            UpdateSourceView();
            ShowStatus(_language.LineInserted);
        }

        private void InsertAfter(string text, bool goToNewLine)
        {
            MakeHistoryForUndo(_language.BeforeInsertLine);

            int firstSelectedIndex = 0;
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                firstSelectedIndex = SubtitleListview1.SelectedItems[0].Index + 1;
            }

            var newParagraph = new Paragraph { Text = text };

            SetStyleForNewParagraph(newParagraph, firstSelectedIndex);

            var prev = _subtitle.GetParagraphOrDefault(firstSelectedIndex - 1);
            var next = _subtitle.GetParagraphOrDefault(firstSelectedIndex);

            var addMilliseconds = MinGapBetweenLines;
            if (addMilliseconds < 1)
            {
                addMilliseconds = 0;
            }

            if (prev != null)
            {
                newParagraph.StartTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds + addMilliseconds;
                newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds + Configuration.Settings.General.NewEmptyDefaultMs;
                if (next != null && newParagraph.EndTime.TotalMilliseconds > next.StartTime.TotalMilliseconds)
                {
                    newParagraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                }

                if (newParagraph.StartTime.TotalMilliseconds > newParagraph.EndTime.TotalMilliseconds)
                {
                    newParagraph.StartTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds + addMilliseconds;
                }

                if (next != null && next.StartTime.IsMaxTime && prev.EndTime.IsMaxTime)
                {
                    newParagraph.StartTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                }
                else if (next != null && next.StartTime.TotalMilliseconds == 0 && prev.EndTime.TotalMilliseconds == 0)
                {
                    newParagraph.StartTime.TotalMilliseconds = 0;
                    newParagraph.EndTime.TotalMilliseconds = 0;
                }
                else if (next == null && prev.EndTime.IsMaxTime)
                {
                    newParagraph.StartTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                }
                else if (next == null && prev.EndTime.TotalMilliseconds == 0)
                {
                    newParagraph.StartTime.TotalMilliseconds = 0;
                    newParagraph.EndTime.TotalMilliseconds = 0;
                }
                else if (next != null &&
                         prev.StartTime.TotalMilliseconds == next.StartTime.TotalMilliseconds &&
                         prev.EndTime.TotalMilliseconds == next.EndTime.TotalMilliseconds)
                {
                    newParagraph.StartTime.TotalMilliseconds = prev.StartTime.TotalMilliseconds;
                    newParagraph.EndTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds;
                }
            }
            else if (next != null)
            {
                newParagraph.StartTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 2000;
                newParagraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - addMilliseconds;
            }
            else
            {
                newParagraph.StartTime.TotalMilliseconds = 1000;
                newParagraph.EndTime.TotalMilliseconds = 3000;
                if (newParagraph.DurationTotalMilliseconds < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                {
                    newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds +
                                                             Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
                }
            }

            if (newParagraph.Duration.TotalMilliseconds < 100)
            {
                newParagraph.EndTime.TotalMilliseconds = newParagraph.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
            }

            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                var currentOriginal = Utilities.GetOriginalParagraph(firstSelectedIndex - 1, _subtitle.Paragraphs[firstSelectedIndex - 1], _subtitleOriginal.Paragraphs);
                if (currentOriginal != null)
                {
                    _subtitleOriginal.Paragraphs.Insert(_subtitleOriginal.Paragraphs.IndexOf(currentOriginal) + 1, new Paragraph(newParagraph));
                }
                else
                {
                    _subtitleOriginal.InsertParagraphInCorrectTimeOrder(new Paragraph(newParagraph));
                }

                _subtitleOriginal.Renumber();
            }

            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(new List<int>(), firstSelectedIndex, newParagraph);
            }
            else
            {
                _subtitle.Paragraphs.Insert(firstSelectedIndex, newParagraph);
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            }

            if (goToNewLine)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
                UpdateSourceView();
                ShowStatus(_language.LineInserted);
            }
        }

        private void SetStyleForNewParagraph(Paragraph newParagraph, int nearestIndex)
        {
            var format = GetCurrentSubtitleFormat();
            bool useExtraForStyle = format.HasStyleSupport;
            var formatType = format.GetType();
            var styles = new List<string>();
            if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
            {
                styles = AdvancedSubStationAlpha.GetStylesFromHeader(_subtitle.Header);
            }
            else if (formatType == typeof(TimedText10) || formatType == typeof(ItunesTimedText) || formatType == typeof(TimedTextImsc11))
            {
                styles = TimedText10.GetStylesFromHeader(_subtitle.Header);
            }
            else if (formatType == typeof(Sami) || formatType == typeof(SamiModern))
            {
                styles = Sami.GetStylesFromHeader(_subtitle.Header);
            }

            string style = "Default";
            if (styles.Count > 0)
            {
                style = styles[0];
            }

            if (useExtraForStyle)
            {
                newParagraph.Extra = style;
                if (format.GetType() == typeof(TimedText10) || format.GetType() == typeof(ItunesTimedText) || formatType == typeof(TimedTextImsc11))
                {
                    if (styles.Count > 0)
                    {
                        newParagraph.Style = style;
                    }

                    var c = _subtitle.GetParagraphOrDefault(nearestIndex);
                    if (c != null)
                    {
                        newParagraph.Style = c.Style;
                        newParagraph.Region = c.Region;
                        newParagraph.Language = c.Language;
                    }

                    newParagraph.Extra = TimedText10.SetExtra(newParagraph);
                }
                else if (format.GetType() == typeof(AdvancedSubStationAlpha))
                {
                    var c = _subtitle.GetParagraphOrDefault(nearestIndex);
                    if (c != null)
                    {
                        newParagraph.Extra = c.Extra;
                    }
                }
            }
        }
        private void SubtitleListView1SelectedIndexChange()
        {
            StopAutoDuration();

            if (_listViewMouseDown)
            {
                return;
            }

            ShowLineInformationListView();
            if (_subtitle?.Paragraphs.Count > 0)
            {
                int firstSelectedIndex = 0;
                if (SubtitleListview1.SelectedIndices.Count > 0)
                {
                    firstSelectedIndex = SubtitleListview1.SelectedIndices[0];
                }

                if (_subtitleListViewIndex >= 0)
                {
                    if (_subtitleListViewIndex == firstSelectedIndex)
                    {
                        return;
                    }

                    bool showSource = false;

                    var last = _subtitle?.GetParagraphOrDefault(_subtitleListViewIndex);
                    if (last != null && textBoxListViewText.Text != last.Text)
                    {
                        last.Text = textBoxListViewText.Text.TrimEnd();
                        SubtitleListview1.SetText(_subtitleListViewIndex, last.Text);
                        showSource = true;
                    }

                    var startTime = timeUpDownStartTime.TimeCode;
                    if (startTime != null && last != null && _subtitle != null &&
                        Math.Abs(last.StartTime.TotalMilliseconds - startTime.TotalMilliseconds) > 0.5)
                    {
                        var dur = last.DurationTotalMilliseconds;
                        last.StartTime.TotalMilliseconds = startTime.TotalMilliseconds;
                        last.EndTime.TotalMilliseconds = startTime.TotalMilliseconds + dur;
                        SubtitleListview1.SetStartTimeAndDuration(_subtitleListViewIndex, last, _subtitle.GetParagraphOrDefault(_subtitleListViewIndex + 1), _subtitle.GetParagraphOrDefault(_subtitleListViewIndex - 1));
                        showSource = true;
                    }

                    var duration = GetDurationInMilliseconds();
                    if (last != null && duration > 0 && duration < 100000 && Math.Abs(duration - last.DurationTotalMilliseconds) > 0.5 && _subtitle != null)
                    {
                        last.EndTime.TotalMilliseconds = last.StartTime.TotalMilliseconds + duration;
                        SubtitleListview1.SetDuration(_subtitleListViewIndex, last, _subtitle.GetParagraphOrDefault(_subtitleListViewIndex + 1));
                        showSource = true;
                    }

                    if (showSource)
                    {
                        UpdateSourceView();
                    }
                }

                var p = _subtitle?.GetParagraphOrDefault(firstSelectedIndex);
                if (p != null)
                {
                    if (IsLiveSpellCheckEnabled)
                    {
                        textBoxListViewText.CurrentLineIndex = firstSelectedIndex;
                        textBoxListViewText.IsSpellCheckRequested = true;
                    }

                    InitializeListViewEditBox(p);
                    _subtitleListViewIndex = firstSelectedIndex;
                    _oldSelectedParagraph = new Paragraph(p);
                    UpdateListViewTextInfo(labelTextLineLengths, labelSingleLine, labelSingleLinePixels, labelTextLineTotal, labelCharactersPerSecond, p, textBoxListViewText);
                    FixVerticalScrollBars(textBoxListViewText);

                    if (_isOriginalActive)
                    {
                        InitializeListViewEditBoxOriginal(p, firstSelectedIndex);
                        labelOriginalCharactersPerSecond.Left = textBoxListViewTextOriginal.Left + (textBoxListViewTextOriginal.Width - labelOriginalCharactersPerSecond.Width);
                        labelTextOriginalLineTotal.Left = textBoxListViewTextOriginal.Left + (textBoxListViewTextOriginal.Width - labelTextOriginalLineTotal.Width);
                    }
                }
            }
        }
        private void ShowLineInformationListView()
        {
            if (InListView)
            {
                _updateSelectedCountStatusBar = true;
            }
        }

        private void UpdateListViewTextCharactersPerSeconds(Label charsPerSecond, Paragraph paragraph)
        {
            if (paragraph.DurationTotalSeconds > 0)
            {
                double charactersPerSecond = Utilities.GetCharactersPerSecond(paragraph);
                if (charactersPerSecond > Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds &&
                    Configuration.Settings.Tools.ListViewSyntaxColorDurationSmall)
                {
                    charsPerSecond.ForeColor = Color.Red;
                }
                else
                {
                    charsPerSecond.ForeColor = UiUtil.ForeColor;
                }

                charsPerSecond.Text = string.Format(_language.CharactersPerSecond, charactersPerSecond);
            }
            else
            {
                if (Configuration.Settings.Tools.ListViewSyntaxColorDurationSmall)
                {
                    charsPerSecond.ForeColor = UiUtil.ForeColor;
                }
                else
                {
                    charsPerSecond.ForeColor = Color.Red;
                }

                charsPerSecond.Text = string.Format(_language.CharactersPerSecond, _languageGeneral.NotAvailable);
            }
        }
        private void UpdateListViewTextInfo(Label lineLengths, Label singleLine, Label singleLinePixels, Label lineTotal, Label charactersPerSecond, Paragraph paragraph, SETextBox textBox)
        {
            if (paragraph == null)
            {
                return;
            }

            bool textBoxHasFocus = textBox.Focused;
            string text = paragraph.Text;
            lineLengths.Text = _languageGeneral.SingleLineLengths.Trim();
            singleLine.Left = lineLengths.Left + lineLengths.Width - 3;
            singleLinePixels.Left = lineLengths.Left + lineLengths.Width + 50;
            text = HtmlUtil.RemoveHtmlTags(text, true);
            text = NetflixImsc11Japanese.RemoveTags(text);
            UiUtil.GetLineLengths(singleLine, text);

            if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
            {
                UiUtil.GetLinePixelWidths(singleLinePixels, text);
                labelSingleLinePixels.Visible = !(textBoxListViewText.Width / 3 < labelTextLineLengths.Width);
            }
            else
            {
                labelSingleLinePixels.Visible = false;
            }

            buttonSplitLine.Visible = false;

            var s = text.Replace(Environment.NewLine, " ");
            var len = text.CountCharacters(false);

            int numberOfLines = Utilities.GetNumberOfLines(text.Trim());
            int maxLines = int.MaxValue;
            if (Configuration.Settings.Tools.ListViewSyntaxMoreThanXLines)
            {
                maxLines = Configuration.Settings.General.MaxNumberOfLines;
            }

            var splitLines = text.SplitToLines();
            if (numberOfLines <= maxLines)
            {
                if (len <= Configuration.Settings.General.SubtitleLineMaximumLength * Math.Max(numberOfLines, 2) &&
                    splitLines.Count == 2 && splitLines[0].StartsWith('-') && splitLines[1].StartsWith('-') &&
                    (splitLines[0].CountCharacters(false) > Configuration.Settings.General.SubtitleLineMaximumLength ||
                     splitLines[1].CountCharacters(false) > Configuration.Settings.General.SubtitleLineMaximumLength))
                {
                    if (buttonUnBreak.Visible)
                    {
                        if (!textBoxHasFocus)
                        {
                            if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
                            {
                                var totalLengthPixels = TextWidth.CalcPixelWidth(text.RemoveChar('\r', '\n'));
                                lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, string.Format("{0}     {1}", len, totalLengthPixels));
                            }
                            else
                            {
                                lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, len);
                            }
                        }

                        buttonSplitLine.Visible = true;
                    }
                }
                else if (len <= Configuration.Settings.General.SubtitleLineMaximumLength * Math.Max(numberOfLines, 2))
                {
                    lineTotal.ForeColor = UiUtil.ForeColor;
                    if (!textBoxHasFocus)
                    {
                        if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
                        {
                            var totalLengthPixels = TextWidth.CalcPixelWidth(text.RemoveChar('\r', '\n'));
                            lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, string.Format("{0}     {1}", len, totalLengthPixels));
                        }
                        else
                        {
                            lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, len);
                        }
                    }
                }
                else
                {
                    lineTotal.ForeColor = Color.Red;
                    if (!textBoxHasFocus)
                    {
                        if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
                        {
                            var totalLengthPixels = TextWidth.CalcPixelWidth(text.RemoveChar('\r', '\n'));
                            lineTotal.Text = string.Format(_languageGeneral.TotalLengthXSplitLine, string.Format("{0}     {1}", len, totalLengthPixels));
                        }
                        else
                        {
                            lineTotal.Text = string.Format(_languageGeneral.TotalLengthXSplitLine, len);
                        }
                    }

                    if (buttonUnBreak.Visible)
                    {
                        if (!textBoxHasFocus)
                        {
                            if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
                            {
                                var totalLengthPixels = TextWidth.CalcPixelWidth(text.RemoveChar('\r', '\n'));
                                lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, string.Format("{0}     {1}", len, totalLengthPixels));
                            }
                            else
                            {
                                lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, len);
                            }
                        }

                        var lang = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle, 50);
                        var abl = Utilities.AutoBreakLine(s, lang).SplitToLines();
                        if (abl.Count > maxLines || abl.Any(li => li.CountCharacters(false) > Configuration.Settings.General.SubtitleLineMaximumLength))
                        {
                            buttonSplitLine.Visible = buttonAutoBreak.Visible;
                        }
                    }
                }
            }
            else
            {
                if (!textBoxHasFocus)
                {
                    if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
                    {
                        var totalLengthPixels = TextWidth.CalcPixelWidth(text.RemoveChar('\r', '\n'));
                        lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, string.Format("{0}     {1}", len, totalLengthPixels));
                    }
                    else
                    {
                        lineTotal.Text = string.Format(_languageGeneral.TotalLengthX, len);
                    }
                }

                var lang = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle, 50);
                var abl = Utilities.AutoBreakLine(s, lang).SplitToLines();
                if (abl.Count > maxLines || abl.Any(li => li.CountCharacters(false) > Configuration.Settings.General.SubtitleLineMaximumLength) &&
                    !textBoxListViewTextOriginal.Visible)
                {
                    buttonSplitLine.Visible = buttonAutoBreak.Visible;
                }
            }

            UpdateListViewTextCharactersPerSeconds(charactersPerSecond, paragraph);
            charactersPerSecond.Left = textBox.Left + (textBox.Width - labelCharactersPerSecond.Width);
            lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
        }

        private void ButtonNextClick(object sender, EventArgs e)
        {
            MoveNextPrevious(0);
        }

        private void ButtonPreviousClick(object sender, EventArgs e)
        {
            MoveNextPrevious(1);
        }

        private void MoveNextPrevious(int firstSelectedIndex)
        {
            if (_subtitle.Paragraphs.Count == 0)
            {
                return;
            }

            SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
            var temp = firstSelectedIndex;
            if (SubtitleListview1.SelectedIndices.Count > 0)
            {
                firstSelectedIndex = SubtitleListview1.SelectedIndices[0];
            }

            firstSelectedIndex = temp == 0 ? firstSelectedIndex + 1 : firstSelectedIndex - 1;
            var p = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
            if (p != null)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
            }

            SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            SubtitleListview1_SelectedIndexChanged(null, null);
        }

        private void FixVerticalScrollBars(SETextBox tb)
        {
            if (!Configuration.Settings.General.SubtitleTextBoxAutoVerticalScrollBars || !tb.Visible)
            {
                return;
            }

            var noOfNewLines = Utilities.GetNumberOfLines(tb.Text.TrimEnd());
            try
            {
                if (noOfNewLines <= 2 && tb.Text.Length <= 70 && tb.TextBoxFont.Size < 15 && tb.Width > 300)
                {
                    tb.ScrollBars = RichTextBoxScrollBars.None;
                }
                else if (noOfNewLines > 20 || tb.Text.Length > 999)
                {
                    tb.ScrollBars = RichTextBoxScrollBars.Vertical;
                }
                else
                {
                    var calculatedHeight = TextRenderer.MeasureText(
                        tb.Text,
                        tb.TextBoxFont,
                        new Size(tb.Width, 1000),
                        TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
                    BeginInvoke(new Action(() =>
                    {
                        tb.ScrollBars = calculatedHeight > tb.Height ? RichTextBoxScrollBars.Vertical : RichTextBoxScrollBars.None;
                    }));
                }
            }
            catch
            {
                // ignored
            }
        }

        private void TextBoxListViewTextTextChanged(object sender, EventArgs e)
        {
            var idx = _subtitleListViewIndex;
            var p = _subtitle.GetParagraphOrDefault(idx);
            if (p == null)
            {
                return;
            }

            // Writing when text is selected gives a double event + some trouble (typed letter disappears or a crash happens).
            // This tries to fix this - changing scrollbars is bad during this double event!?
            // Also check https://stackoverflow.com/questions/28331672/c-sharp-textchanged-event-fires-twice-in-a-multiline-textbox
            if (textBoxListViewText.Text.Length == 0)
            {
                p.Text = string.Empty;
                UpdateListViewTextInfo(labelTextLineLengths, labelSingleLine, labelSingleLinePixels, labelTextLineTotal, labelCharactersPerSecond, _subtitle.Paragraphs[idx], textBoxListViewText);
                SubtitleListview1.SetText(idx, string.Empty);
                _listViewTextUndoIndex = idx;
                labelStatus.Text = string.Empty;
                StartUpdateListSyntaxColoring();
                return;
            }

            textBoxListViewText.TextChanged -= TextBoxListViewTextTextChanged;
            if (_doAutoBreakOnTextChanged)
            {
                UiUtil.CheckAutoWrap(textBoxListViewText, new KeyEventArgs(Keys.None), Utilities.GetNumberOfLines(textBoxListViewText.Text));
            }

            // update _subtitle + listview
            string text = textBoxListViewText.Text.TrimEnd();
            if (text.ContainsNonStandardNewLines())
            {
                var lines = text.SplitToLines();
                text = string.Join(Environment.NewLine, lines);
                textBoxListViewText.Text = text;
            }

            if (p != _subtitle.GetParagraphOrDefault(idx))
            {
                textBoxListViewText.TextChanged += TextBoxListViewTextTextChanged;
                return;
            }

            p.Text = text;
            labelStatus.Text = string.Empty;
            UpdateListViewTextInfo(labelTextLineLengths, labelSingleLine, labelSingleLinePixels, labelTextLineTotal, labelCharactersPerSecond, _subtitle.Paragraphs[idx], textBoxListViewText);
            SubtitleListview1.SetText(idx, text);

            _listViewTextUndoIndex = _subtitleListViewIndex;

            StartUpdateListSyntaxColoring();
            FixVerticalScrollBars(textBoxListViewText);
            textBoxListViewText.TextChanged += TextBoxListViewTextTextChanged;
        }

        private void TextBoxListViewTextOriginalTextChanged(object sender, EventArgs e)
        {
            if (_subtitleListViewIndex >= 0)
            {
                var p = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
                if (p == null)
                {
                    return;
                }

                var original = Utilities.GetOriginalParagraph(_subtitleListViewIndex, p, _subtitleOriginal.Paragraphs);
                if (original != null)
                {
                    if (textBoxListViewTextOriginal.Text.Length == 0)
                    {
                        UpdateListViewTextInfo(labelTextOriginalLineLengths, labelOriginalSingleLine, labelOriginalSingleLinePixels, labelTextOriginalLineTotal, labelOriginalCharactersPerSecond, original, textBoxListViewTextOriginal);
                        SubtitleListview1.SetOriginalText(_subtitleListViewIndex, string.Empty);
                        _listViewTextUndoIndex = _subtitleListViewIndex;
                        labelStatus.Text = string.Empty;
                        StartUpdateListSyntaxColoring();
                        return;
                    }

                    int numberOfNewLines = Utilities.GetNumberOfLines(textBoxListViewTextOriginal.Text);
                    UiUtil.CheckAutoWrap(textBoxListViewTextOriginal, new KeyEventArgs(Keys.None), numberOfNewLines);

                    // update _subtitle + listview
                    string text = textBoxListViewTextOriginal.Text.TrimEnd();
                    if (text.ContainsNonStandardNewLines())
                    {
                        var lines = text.SplitToLines();
                        text = string.Join(Environment.NewLine, lines);
                        textBoxListViewTextOriginal.Text = text;
                    }

                    original.Text = text;
                    UpdateListViewTextInfo(labelTextOriginalLineLengths, labelOriginalSingleLine, labelOriginalSingleLinePixels, labelTextOriginalLineTotal, labelOriginalCharactersPerSecond, original, textBoxListViewTextOriginal);
                    SubtitleListview1.SetOriginalText(_subtitleListViewIndex, text);
                    _listViewTextUndoIndex = _subtitleListViewIndex;
                }

                labelStatus.Text = string.Empty;

                StartUpdateListSyntaxColoring();
                FixVerticalScrollBars(textBoxListViewTextOriginal);
            }
        }

        private void NumericUpDownLayer_ValueChanged(object sender, EventArgs e)
        {
            var p = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
            if (p == null)
            {
                return;
            }

            var isAssa = GetCurrentSubtitleFormat().GetType() == typeof(AdvancedSubStationAlpha);
            if (isAssa)
            {
                var layer = (int)numericUpDownLayer.Value;
                MakeHistoryForUndo(string.Format(_language.BeforeX, $"{_language.Menu.ContextMenu.SetLayer}: {layer}"));
                p.Layer = layer;
            }
        }

        public Point GetPositionInForm(Control ctrl)
        {
            Point p = ctrl.Location;
            Control parent = ctrl.Parent;
            while (!(parent is Form))
            {
                p.Offset(parent.Location.X, parent.Location.Y);
                parent = parent.Parent;
            }

            return p;
        }


        private void MoveFirstWordInNextUp(SETextBox tb)
        {
            int firstIndex = FirstSelectedIndex;
            if (firstIndex >= 0)
            {
                var p = _subtitle.GetParagraphOrDefault(firstIndex);
                var next = _subtitle.GetParagraphOrDefault(firstIndex + 1);

                if (tb == textBoxListViewTextOriginal)
                {
                    p = Utilities.GetOriginalParagraph(firstIndex, p, _subtitleOriginal.Paragraphs);
                    next = Utilities.GetOriginalParagraph(firstIndex + 1, next, _subtitleOriginal.Paragraphs);
                }

                if (p != null && next != null)
                {
                    var moveUpDown = new MoveWordUpDown(p.Text, next.Text);
                    moveUpDown.MoveWordUp();
                    if (moveUpDown.S1 != p.Text && moveUpDown.S2 != next.Text)
                    {
                        MakeHistoryForUndo(_language.BeforeLineUpdatedInListView);
                        p.Text = moveUpDown.S1;
                        next.Text = moveUpDown.S2;
                        if (tb == textBoxListViewTextOriginal)
                        {
                            SubtitleListview1.SetOriginalText(firstIndex, p.Text);
                            SubtitleListview1.SetOriginalText(firstIndex + 1, next.Text);
                        }
                        else
                        {
                            SubtitleListview1.SetText(firstIndex, p.Text);
                            SubtitleListview1.SetText(firstIndex + 1, next.Text);
                        }

                        var selectionStart = textBoxListViewText.SelectionStart;
                        tb.Text = p.Text;
                        if (selectionStart >= 0)
                        {
                            tb.SelectionStart = selectionStart;
                        }
                    }
                }
            }
        }

        private void MoveLastWordDown(SETextBox tb)
        {
            int firstIndex = FirstSelectedIndex;
            if (firstIndex >= 0)
            {
                var p = _subtitle.GetParagraphOrDefault(firstIndex);
                var next = _subtitle.GetParagraphOrDefault(firstIndex + 1);

                if (tb == textBoxListViewTextOriginal)
                {
                    p = Utilities.GetOriginalParagraph(firstIndex, p, _subtitleOriginal.Paragraphs);
                    next = Utilities.GetOriginalParagraph(firstIndex + 1, next, _subtitleOriginal.Paragraphs);
                }

                if (p != null && next != null)
                {
                    var moveUpDown = new MoveWordUpDown(p.Text, next.Text);
                    moveUpDown.MoveWordDown();
                    if (moveUpDown.S1 != p.Text && moveUpDown.S2 != next.Text)
                    {
                        MakeHistoryForUndo(_language.BeforeLineUpdatedInListView);
                        p.Text = moveUpDown.S1;
                        next.Text = moveUpDown.S2;
                        if (tb == textBoxListViewTextOriginal)
                        {
                            SubtitleListview1.SetOriginalText(firstIndex, p.Text);
                            SubtitleListview1.SetOriginalText(firstIndex + 1, next.Text);
                        }
                        else
                        {
                            SubtitleListview1.SetText(firstIndex, p.Text);
                            SubtitleListview1.SetText(firstIndex + 1, next.Text);
                        }

                        var selectionStart = textBoxListViewText.SelectionStart;
                        tb.Text = p.Text;
                        if (selectionStart >= 0)
                        {
                            tb.SelectionStart = selectionStart;
                        }
                    }
                }
            }
        }

        private void MakeAutoDuration()
        {
            int i = _subtitleListViewIndex;
            var p = _subtitle.GetParagraphOrDefault(i);
            if (p == null)
            {
                return;
            }

            double duration = Utilities.GetOptimalDisplayMilliseconds(textBoxListViewText.Text);
            var next = _subtitle.GetParagraphOrDefault(i + 1);
            if (next != null && p.StartTime.TotalMilliseconds + duration + MinGapBetweenLines > next.StartTime.TotalMilliseconds)
            {
                duration = next.StartTime.TotalMilliseconds - p.StartTime.TotalMilliseconds - MinGapBetweenLines;
                if (duration < 400)
                {
                    return;
                }
            }

            SetDurationInSeconds(duration / TimeCode.BaseUnit);

            p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
            SubtitleListview1.SetDuration(i, p, _subtitle.GetParagraphOrDefault(i + 1));
        }

        private void SplitSelectedParagraph(double? splitSeconds, int? textIndex, bool autoBreak = false)
        {
            var maxSingleLineLength = Configuration.Settings.General.SubtitleLineMaximumLength;
            var language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
            int? originalTextIndex = null;
            if (textBoxListViewTextOriginal.Focused)
            {
                originalTextIndex = textIndex;
                textIndex = null;
            }

            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedIndices.Count > 0)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(_language.BeforeSplitLine);

                int firstSelectedIndex = SubtitleListview1.SelectedIndices[0];

                var currentParagraph = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
                var newParagraph = new Paragraph(currentParagraph) { NewSection = false };

                currentParagraph.Text = currentParagraph.Text.Replace("< /i>", "</i>");
                currentParagraph.Text = currentParagraph.Text.Replace("< i>", "<i>");

                string oldText = currentParagraph.Text;
                var lines = currentParagraph.Text.SplitToLines();
                if (textIndex != null)
                {
                    string a = oldText.Substring(0, textIndex.Value).Trim();
                    string b = oldText.Substring(textIndex.Value).Trim();

                    if (oldText.TrimStart().StartsWith("<i>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</i>", StringComparison.Ordinal) &&
                        Utilities.CountTagInText(oldText, "<i>") == 1 && Utilities.CountTagInText(oldText, "</i>") == 1)
                    {
                        a += "</i>";
                        b = "<i>" + b;
                    }
                    else if (oldText.TrimStart().StartsWith("<b>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</b>", StringComparison.Ordinal) &&
                             Utilities.CountTagInText(oldText, "<b>") == 1 && Utilities.CountTagInText(oldText, "</b>") == 1)
                    {
                        a += "</b>";
                        b = "<b>" + b;
                    }

                    string aTrimmed = HtmlUtil.RemoveHtmlTags(a).TrimEnd('"').TrimEnd().TrimEnd('\'').TrimEnd();
                    if (Configuration.Settings.General.SplitRemovesDashes && (aTrimmed.EndsWith('.') || aTrimmed.EndsWith('!') || aTrimmed.EndsWith('?') || aTrimmed.EndsWith('…') || aTrimmed.EndsWith('؟')))
                    {
                        a = DialogSplitMerge.RemoveStartDash(a);
                        b = DialogSplitMerge.RemoveStartDash(b);
                    }

                    currentParagraph.Text = a.SplitToLines().Any(line => line.Length > maxSingleLineLength) ? Utilities.AutoBreakLine(a, language) : a;
                    newParagraph.Text = b.SplitToLines().Any(line => line.Length > maxSingleLineLength) ? Utilities.AutoBreakLine(b, language) : b;
                }
                else
                {
                    var l0 = string.Empty;
                    if (lines.Count > 0)
                    {
                        l0 = HtmlUtil.RemoveHtmlTags(lines[0], true).Trim().TrimEnd('"', '\'').TrimEnd();
                    }

                    if (lines.Count == 2 && (l0.EndsWith('.') || l0.EndsWith('!') || l0.EndsWith('?') || l0.EndsWith('…') || l0.EndsWith('؟')))
                    {
                        currentParagraph.Text = Utilities.AutoBreakLine(lines[0], language);
                        newParagraph.Text = Utilities.AutoBreakLine(lines[1], language);
                        if (currentParagraph.Text.StartsWith("<i>", StringComparison.Ordinal) && !currentParagraph.Text.Contains("</i>") &&
                            newParagraph.Text.EndsWith("</i>", StringComparison.Ordinal) && !newParagraph.Text.Contains("<i>"))
                        {
                            currentParagraph.Text += "</i>";
                            newParagraph.Text = "<i>" + newParagraph.Text;
                        }

                        if (currentParagraph.Text.StartsWith("<b>", StringComparison.Ordinal) && !currentParagraph.Text.Contains("</b>") &&
                            newParagraph.Text.EndsWith("</b>", StringComparison.Ordinal) && !newParagraph.Text.Contains("<b>"))
                        {
                            currentParagraph.Text += "</b>";
                            newParagraph.Text = "<b>" + newParagraph.Text;
                        }

                        if (Configuration.Settings.General.SplitRemovesDashes)
                        {
                            currentParagraph.Text = DialogSplitMerge.RemoveStartDash(currentParagraph.Text);
                            newParagraph.Text = DialogSplitMerge.RemoveStartDash(newParagraph.Text);
                        }
                    }
                    else
                    {
                        string s = currentParagraph.Text;
                        var arr = HtmlUtil.RemoveHtmlTags(s, true).SplitToLines();
                        if (arr.Count == 1 || arr.Count == 2 && (arr[0].Length > Configuration.Settings.General.SubtitleLineMaximumLength || arr[1].Length > Configuration.Settings.General.SubtitleLineMaximumLength))
                        {
                            if (arr.Count == 2 && arr[0].StartsWith('-') && arr[1].StartsWith('-'))
                            {
                                if (lines[0].StartsWith("<i>-", StringComparison.Ordinal))
                                {
                                    lines[0] = "<i>" + lines[0].Remove(0, 4).TrimStart();
                                }

                                lines[0] = lines[0].TrimStart('-').TrimStart();
                                lines[1] = lines[1].TrimStart('-').TrimStart();
                                s = lines[0] + Environment.NewLine + lines[1];
                            }
                            else
                            {
                                s = Utilities.AutoBreakLine(currentParagraph.Text, 5, Configuration.Settings.General.MergeLinesShorterThan, language);
                            }
                        }

                        lines = s.SplitToLines();
                        if (lines.Count == 1)
                        {
                            s = Utilities.AutoBreakLine(currentParagraph.Text, 3, 20, language);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 1)
                        {
                            s = Utilities.AutoBreakLine(currentParagraph.Text, 3, 18, language);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 1)
                        {
                            s = Utilities.AutoBreakLine(currentParagraph.Text, 3, 15, language);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 2)
                        {
                            if (Utilities.CountTagInText(s, "<i>") == 1 && lines[0].Contains("<i>", StringComparison.Ordinal) && lines[1].Contains("</i>", StringComparison.Ordinal))
                            {
                                lines[0] += "</i>";
                                lines[1] = "<i>" + lines[1];
                            }

                            currentParagraph.Text = Utilities.AutoBreakLine(lines[0], language);
                            newParagraph.Text = Utilities.AutoBreakLine(lines[1], language);
                        }
                        else if (lines.Count == 1)
                        {
                            currentParagraph.Text = Utilities.AutoBreakLine(lines[0], language);
                            newParagraph.Text = string.Empty;
                        }
                        else if (lines.Count == 3)
                        {
                            currentParagraph.Text = Utilities.AutoBreakLine(lines[0] + Environment.NewLine + lines[1], language);
                            newParagraph.Text = lines[2];
                        }
                        else if (lines.Count > 3)
                        {
                            var half = lines.Count / 2;
                            var sb1 = new StringBuilder();
                            for (int i = 0; i < half; i++)
                            {
                                sb1.AppendLine(lines[i]);
                            }

                            currentParagraph.Text = Utilities.AutoBreakLine(sb1.ToString(), language);
                            sb1 = new StringBuilder();
                            for (int i = half; i < lines.Count; i++)
                            {
                                sb1.AppendLine(lines[i]);
                            }

                            newParagraph.Text = Utilities.AutoBreakLine(sb1.ToString(), language);
                        }

                        if (currentParagraph.Text.Contains("<i>", StringComparison.Ordinal) && !currentParagraph.Text.Contains("</i>", StringComparison.Ordinal) &&
                            newParagraph.Text.Contains("</i>", StringComparison.Ordinal) && !newParagraph.Text.Contains("<i>", StringComparison.Ordinal))
                        {
                            currentParagraph.Text += "</i>";
                            newParagraph.Text = "<i>" + newParagraph.Text;
                        }

                        if (currentParagraph.Text.Contains("<b>", StringComparison.Ordinal) && !currentParagraph.Text.Contains("</b>", StringComparison.Ordinal) &&
                            newParagraph.Text.Contains("</b>", StringComparison.Ordinal) && !newParagraph.Text.Contains("<b>"))
                        {
                            currentParagraph.Text += "</b>";
                            newParagraph.Text = "<b>" + newParagraph.Text;
                        }

                        if (currentParagraph.Text.StartsWith("<i>-", StringComparison.Ordinal) && (currentParagraph.Text.EndsWith(".</i>", StringComparison.Ordinal) || currentParagraph.Text.EndsWith("!</i>", StringComparison.Ordinal)) &&
                            newParagraph.Text.StartsWith("<i>-", StringComparison.Ordinal) && (newParagraph.Text.EndsWith(".</i>", StringComparison.Ordinal) || newParagraph.Text.EndsWith("!</i>", StringComparison.Ordinal)))
                        {
                            currentParagraph.Text = currentParagraph.Text.Remove(3, 1);
                            newParagraph.Text = newParagraph.Text.Remove(3, 1);
                        }
                    }
                }

                if (currentParagraph.Text.StartsWith("<i> ", StringComparison.Ordinal))
                {
                    currentParagraph.Text = currentParagraph.Text.Remove(3, 1);
                }

                if (newParagraph.Text.StartsWith("<i> ", StringComparison.Ordinal))
                {
                    newParagraph.Text = newParagraph.Text.Remove(3, 1);
                }

                var continuationStyle = Configuration.Settings.General.ContinuationStyle;
                if (continuationStyle != ContinuationStyle.None)
                {
                    if (language == "ar")
                    {
                        currentParagraph.Text = ContinuationUtilities.ConvertToForArabic(currentParagraph.Text);
                        newParagraph.Text = ContinuationUtilities.ConvertToForArabic(newParagraph.Text);
                    }

                    var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                    if (ContinuationUtilities.ShouldAddSuffix(currentParagraph.Text, continuationProfile))
                    {
                        currentParagraph.Text = ContinuationUtilities.AddSuffixIfNeeded(currentParagraph.Text, continuationProfile, false);
                        newParagraph.Text = ContinuationUtilities.AddPrefixIfNeeded(newParagraph.Text, continuationProfile, false);
                    }

                    if (language == "ar")
                    {
                        currentParagraph.Text = ContinuationUtilities.ConvertBackForArabic(currentParagraph.Text);
                        newParagraph.Text = ContinuationUtilities.ConvertBackForArabic(newParagraph.Text);
                    }
                }

                FixSplitItalicTagAndAssa(currentParagraph, newParagraph);
                FixSplitFontTag(currentParagraph, newParagraph);
                FixSplitBoxTag(currentParagraph, newParagraph);
                SetSplitTime(splitSeconds, currentParagraph, newParagraph, oldText);

                if (autoBreak)
                {
                    currentParagraph.Text = Utilities.AutoBreakLine(currentParagraph.Text);
                    newParagraph.Text = Utilities.AutoBreakLine(newParagraph.Text);
                }

                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    var originalCurrent = Utilities.GetOriginalParagraph(firstSelectedIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                    if (originalCurrent != null)
                    {
                        string languageOriginal = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);

                        originalCurrent.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                        var originalNew = new Paragraph(newParagraph) { NewSection = false };

                        lines = originalCurrent.Text.SplitToLines();

                        var l0Trimmed = string.Empty;
                        if (lines.Count > 0)
                        {
                            l0Trimmed = HtmlUtil.RemoveHtmlTags(lines[0]).TrimEnd('\'').TrimEnd('"');
                        }

                        oldText = originalCurrent.Text;
                        if (originalTextIndex != null)
                        {
                            var firstPart = oldText.Substring(0, originalTextIndex.Value).Trim();
                            var secondPart = oldText.Substring(originalTextIndex.Value).Trim();
                            originalCurrent.Text = firstPart.SplitToLines().Any(line => line.Length > maxSingleLineLength) ? Utilities.AutoBreakLine(firstPart, language) : firstPart;
                            originalNew.Text = secondPart.SplitToLines().Any(line => line.Length > maxSingleLineLength) ? Utilities.AutoBreakLine(secondPart, language) : secondPart;
                            if (originalCurrent.Text.Contains("<i>", StringComparison.Ordinal) && !originalCurrent.Text.Contains("</i>", StringComparison.Ordinal) &&
                                originalNew.Text.Contains("</i>", StringComparison.Ordinal) && !originalNew.Text.Contains("<i>", StringComparison.Ordinal))
                            {
                                if (originalCurrent.Text.StartsWith("<i>-", StringComparison.Ordinal) && (originalCurrent.Text.EndsWith(".", StringComparison.Ordinal) || originalCurrent.Text.EndsWith("?", StringComparison.Ordinal) ||
                                                                                                          originalCurrent.Text.EndsWith("!", StringComparison.Ordinal) || originalCurrent.Text.EndsWith("؟", StringComparison.Ordinal)) && originalNew.Text.StartsWith("-", StringComparison.Ordinal))
                                {
                                    originalCurrent.Text = "<i>" + originalCurrent.Text.Remove(0, 4).Trim();
                                    originalNew.Text = originalNew.Text.TrimStart('-').Trim();
                                }

                                originalCurrent.Text += "</i>";
                                originalNew.Text = "<i>" + originalNew.Text;
                            }

                            if (originalCurrent.Text.Contains("<b>", StringComparison.Ordinal) && !originalCurrent.Text.Contains("</b>") &&
                                originalNew.Text.Contains("</b>", StringComparison.Ordinal) && !originalNew.Text.Contains("<b>"))
                            {
                                originalCurrent.Text += "</b>";
                                originalNew.Text = "<b>" + originalNew.Text;
                            }

                            if (Configuration.Settings.General.SplitRemovesDashes && (l0Trimmed.EndsWith('.') || l0Trimmed.EndsWith('!') || l0Trimmed.EndsWith('?') || l0Trimmed.EndsWith('…') || l0Trimmed.EndsWith('؟')))
                            {
                                originalCurrent.Text = DialogSplitMerge.RemoveStartDash(originalCurrent.Text);
                                originalNew.Text = DialogSplitMerge.RemoveStartDash(originalNew.Text);
                            }

                            lines.Clear();
                        }
                        else if (lines.Count == 2 && (l0Trimmed.EndsWith('.') || l0Trimmed.EndsWith('!') || l0Trimmed.EndsWith('?') || l0Trimmed.EndsWith('…') || l0Trimmed.EndsWith('؟')))
                        {
                            string a = lines[0].Trim();
                            string b = lines[1].Trim();
                            if (oldText.TrimStart().StartsWith("<i>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</i>", StringComparison.Ordinal) &&
                                Utilities.CountTagInText(oldText, "<i>") == 1 && Utilities.CountTagInText(oldText, "</i>") == 1)
                            {
                                a += "</i>";
                                b = "<i>" + b;
                            }

                            if (oldText.TrimStart().StartsWith("<b>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</b>", StringComparison.Ordinal) &&
                                Utilities.CountTagInText(oldText, "<b>") == 1 && Utilities.CountTagInText(oldText, "</b>") == 1)
                            {
                                a += "</b>";
                                b = "<b>" + b;
                            }

                            if (Configuration.Settings.General.SplitRemovesDashes)
                            {
                                a = DialogSplitMerge.RemoveStartDash(a);
                                b = DialogSplitMerge.RemoveStartDash(b);
                            }

                            lines[0] = a;
                            lines[1] = b;
                            originalCurrent.Text = Utilities.AutoBreakLine(a);
                            originalNew.Text = Utilities.AutoBreakLine(b);
                        }
                        else if (Utilities.GetNumberOfLines(originalCurrent.Text) == 2)
                        {
                            lines = originalCurrent.Text.SplitToLines();
                        }
                        else
                        {
                            string s = Utilities.AutoBreakLine(originalCurrent.Text, 5, Configuration.Settings.General.MergeLinesShorterThan, languageOriginal);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 1)
                        {
                            string s = Utilities.AutoBreakLine(lines[0], 3, 20, languageOriginal);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 1)
                        {
                            string s = Utilities.AutoBreakLine(lines[0], 3, 18, languageOriginal);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 1)
                        {
                            string s = Utilities.AutoBreakLine(lines[0], 3, 15, languageOriginal);
                            lines = s.SplitToLines();
                        }

                        if (lines.Count == 2)
                        {
                            string a = lines[0].Trim();
                            string b = lines[1].Trim();
                            if (oldText.TrimStart().StartsWith("<i>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</i>", StringComparison.Ordinal) &&
                                Utilities.CountTagInText(oldText, "<i>") == 1 && Utilities.CountTagInText(oldText, "</i>") == 1)
                            {
                                a += "</i>";
                                b = "<i>" + b;
                            }

                            if (oldText.TrimStart().StartsWith("<b>", StringComparison.Ordinal) && oldText.TrimEnd().EndsWith("</b>", StringComparison.Ordinal) &&
                                Utilities.CountTagInText(oldText, "<b>") == 1 && Utilities.CountTagInText(oldText, "</b>") == 1)
                            {
                                a += "</b>";
                                b = "<b>" + b;
                            }

                            if (Configuration.Settings.General.SplitRemovesDashes && (l0Trimmed.EndsWith('.') || l0Trimmed.EndsWith('!') || l0Trimmed.EndsWith('?') || l0Trimmed.EndsWith('…') || l0Trimmed.EndsWith('؟')))
                            {
                                a = DialogSplitMerge.RemoveStartDash(a);
                                b = DialogSplitMerge.RemoveStartDash(b);
                            }

                            lines[0] = a;
                            lines[1] = b;

                            originalCurrent.Text = Utilities.AutoBreakLine(lines[0]);
                            originalNew.Text = Utilities.AutoBreakLine(lines[1]);
                        }
                        else if (lines.Count == 1)
                        {
                            originalNew.Text = string.Empty;
                        }

                        if (originalCurrent != null && originalNew != null)
                        {
                            if (originalCurrent.Text.StartsWith("<i> ", StringComparison.Ordinal))
                            {
                                originalCurrent.Text = originalCurrent.Text.Remove(3, 1);
                            }

                            if (originalNew.Text.StartsWith("<i> ", StringComparison.Ordinal))
                            {
                                originalCurrent.Text = originalCurrent.Text.Remove(3, 1);
                            }

                            if (continuationStyle != ContinuationStyle.None)
                            {
                                if (languageOriginal == "ar")
                                {
                                    originalCurrent.Text = ContinuationUtilities.ConvertToForArabic(originalCurrent.Text);
                                    originalNew.Text = ContinuationUtilities.ConvertToForArabic(originalNew.Text);
                                }

                                var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                                if (ContinuationUtilities.ShouldAddSuffix(originalCurrent.Text, continuationProfile))
                                {
                                    originalCurrent.Text = ContinuationUtilities.AddSuffixIfNeeded(originalCurrent.Text, continuationProfile, false);
                                    originalNew.Text = ContinuationUtilities.AddPrefixIfNeeded(originalNew.Text, continuationProfile, false);
                                }

                                if (languageOriginal == "ar")
                                {
                                    originalCurrent.Text = ContinuationUtilities.ConvertBackForArabic(originalCurrent.Text);
                                    originalNew.Text = ContinuationUtilities.ConvertBackForArabic(originalNew.Text);
                                }
                            }
                        }

                        _subtitleOriginal.InsertParagraphInCorrectTimeOrder(originalNew);
                        _subtitleOriginal.Renumber();
                        FixSplitItalicTagAndAssa(originalCurrent, originalNew);
                        FixSplitFontTag(originalCurrent, originalNew);
                        FixSplitBoxTag(originalCurrent, originalNew);

                        if (autoBreak)
                        {
                            originalCurrent.Text = Utilities.AutoBreakLine(originalCurrent.Text);
                            originalNew.Text = Utilities.AutoBreakLine(originalNew.Text);
                        }
                    }
                }

                if (_networkSession != null)
                {
                    _networkSession.TimerStop();
                    SetDurationInSeconds(currentParagraph.DurationTotalSeconds);
                    _networkSession.UpdateLine(_subtitle.GetIndex(currentParagraph), currentParagraph);
                    NetworkGetSendUpdates(new List<int>(), firstSelectedIndex + 1, newParagraph);
                }
                else
                {
                    _subtitle.Paragraphs.Insert(firstSelectedIndex + 1, newParagraph);
                    _subtitle.Renumber();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }

                var index = firstSelectedIndex;
                if (Configuration.Settings.General.SplitBehavior == 0 || !mediaPlayer.IsPaused)
                {
                    index++;
                }

                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
                UpdateSourceView();
                ShowStatus(_language.LineSplitted);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();

                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(25), () =>
                {
                    _lastTextKeyDownTicks = -1; // faster refresh
                });
            }
        }
        private void FixSplitItalicTagAndAssa(Paragraph currentParagraph, Paragraph nextParagraph)
        {
            if (currentParagraph == null || nextParagraph == null)
            {
                return;
            }

            var startIdx = currentParagraph.Text.LastIndexOf("<i>", StringComparison.OrdinalIgnoreCase);

            string pre;
            if (startIdx >= 0 &&
                !currentParagraph.Text.Contains("</i>", StringComparison.OrdinalIgnoreCase) &&
                nextParagraph.Text.Contains("</i>", StringComparison.OrdinalIgnoreCase))
            {
                var endIdx = currentParagraph.Text.IndexOf('>', startIdx);
                if (endIdx >= 0)
                {
                    var fontTag = currentParagraph.Text.Substring(startIdx, endIdx - startIdx + 1);
                    pre = string.Empty;
                    if (currentParagraph.Text.StartsWith('{') && currentParagraph.Text.IndexOf('}') > 0)
                    {
                        var i = currentParagraph.Text.IndexOf('}');
                        pre = currentParagraph.Text.Substring(0, i + 1);
                        currentParagraph.Text = currentParagraph.Text.Remove(0, i + 1);
                    }

                    currentParagraph.Text = pre + currentParagraph.Text + "</i>";
                    nextParagraph.Text = pre + fontTag + nextParagraph.Text;
                }
            }
            else if (currentParagraph.Text.StartsWith("{\\", StringComparison.Ordinal))
            {
                var endIdx = currentParagraph.Text.IndexOf('}', 2);
                if (endIdx > 2)
                {
                    pre = currentParagraph.Text.Substring(0, endIdx + 1);
                    nextParagraph.Text = pre + nextParagraph.Text;
                }
            }
            else if (currentParagraph.Text.Contains("{\\i1}", StringComparison.Ordinal) &&
                     !currentParagraph.Text.Contains("{\\i0}", StringComparison.Ordinal) &&
                     nextParagraph.Text.Contains("{\\i0}", StringComparison.Ordinal))
            {
                currentParagraph.Text += "{\\i0}";
                nextParagraph.Text = "{\\i1}" + nextParagraph.Text;
            }
        }

        private void FixSplitBoxTag(Paragraph currentParagraph, Paragraph nextParagraph)
        {
            if (currentParagraph == null || nextParagraph == null)
            {
                return;
            }

            var startIdx = currentParagraph.Text.LastIndexOf("<box>", StringComparison.OrdinalIgnoreCase);

            string pre;
            if (startIdx >= 0 &&
                !currentParagraph.Text.Contains("</box>", StringComparison.OrdinalIgnoreCase) &&
                nextParagraph.Text.Contains("</box>", StringComparison.OrdinalIgnoreCase))
            {
                var endIdx = currentParagraph.Text.IndexOf('>', startIdx);
                if (endIdx >= 0)
                {
                    var fontTag = currentParagraph.Text.Substring(startIdx, endIdx - startIdx + 1);
                    pre = string.Empty;
                    if (currentParagraph.Text.StartsWith('{') && currentParagraph.Text.IndexOf('}') > 0)
                    {
                        var i = currentParagraph.Text.IndexOf('}');
                        pre = currentParagraph.Text.Substring(0, i + 1);
                        currentParagraph.Text = currentParagraph.Text.Remove(0, i + 1);
                    }

                    currentParagraph.Text = pre + currentParagraph.Text + "</box>";
                    nextParagraph.Text = pre + fontTag + nextParagraph.Text;
                }
            }
        }

        private void FixSplitFontTag(Paragraph currentParagraph, Paragraph nextParagraph)
        {
            if (currentParagraph == null || nextParagraph == null)
            {
                return;
            }

            var startIdx = currentParagraph.Text.LastIndexOf("<font ", StringComparison.OrdinalIgnoreCase);
            if (startIdx >= 0 &&
                !currentParagraph.Text.Contains("</font>", StringComparison.OrdinalIgnoreCase) &&
                nextParagraph.Text.Contains("</font>", StringComparison.OrdinalIgnoreCase))
            {
                var endIdx = currentParagraph.Text.IndexOf('>', startIdx);
                if (endIdx >= 0)
                {
                    var fontTag = currentParagraph.Text.Substring(startIdx, endIdx - startIdx + 1);
                    var pre = string.Empty;
                    if (currentParagraph.Text.StartsWith('{') && currentParagraph.Text.IndexOf('}') > 0)
                    {
                        var i = currentParagraph.Text.IndexOf('}');
                        pre = currentParagraph.Text.Substring(0, i + 1);
                        currentParagraph.Text = currentParagraph.Text.Remove(0, i + 1);
                    }

                    currentParagraph.Text = pre + currentParagraph.Text + "</font>";
                    nextParagraph.Text = pre + fontTag + nextParagraph.Text;
                }
            }
        }

        private void SetSplitTime(double? splitSeconds, Paragraph currentParagraph, Paragraph newParagraph, string oldText)
        {
            double middle = currentParagraph.StartTime.TotalMilliseconds + (currentParagraph.DurationTotalMilliseconds / 2);
            if (!string.IsNullOrWhiteSpace(HtmlUtil.RemoveHtmlTags(oldText)))
            {
                var lineOneTextNoHtml = HtmlUtil.RemoveHtmlTags(currentParagraph.Text, true).Replace(Environment.NewLine, string.Empty);
                var lineTwoTextNoHtml = HtmlUtil.RemoveHtmlTags(newParagraph.Text, true).Replace(Environment.NewLine, string.Empty);
                if (Math.Abs(lineOneTextNoHtml.Length - lineTwoTextNoHtml.Length) > 2)
                {
                    // give more time to the paragraph with most text
                    var oldTextNoHtml = HtmlUtil.RemoveHtmlTags(oldText, true).Replace(Environment.NewLine, string.Empty);
                    var startFactor = (double)lineOneTextNoHtml.Length / oldTextNoHtml.Length;
                    if (startFactor < 0.25)
                    {
                        startFactor = 0.25;
                    }

                    if (startFactor > 0.75)
                    {
                        startFactor = 0.75;
                    }

                    middle = currentParagraph.StartTime.TotalMilliseconds + (currentParagraph.DurationTotalMilliseconds * startFactor);
                }
            }

            if (currentParagraph.StartTime.IsMaxTime && currentParagraph.EndTime.IsMaxTime)
            {
                newParagraph.StartTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
                newParagraph.EndTime.TotalMilliseconds = TimeCode.MaxTimeTotalMilliseconds;
            }
            else if (currentParagraph.DurationTotalMilliseconds <= 1)
            {
                newParagraph.StartTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                newParagraph.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
            }
            else
            {
                if (splitSeconds.HasValue && splitSeconds.Value > (currentParagraph.StartTime.TotalSeconds + 0.2) && splitSeconds.Value < (currentParagraph.EndTime.TotalSeconds - 0.2))
                {
                    middle = splitSeconds.Value * TimeCode.BaseUnit;
                }

                newParagraph.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                currentParagraph.EndTime.TotalMilliseconds = middle;
                newParagraph.StartTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds + 1;
                if (MinGapBetweenLines > 0)
                {
                    if (splitSeconds == null || Configuration.Settings.General.SplitBehavior == 1)
                    {
                        // SE decides split point (not user), so split gap time evenly
                        var halfGap = (int)Math.Round(MinGapBetweenLines / 2.0);
                        currentParagraph.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds - halfGap;
                    }
                    else if (Configuration.Settings.General.SplitBehavior == 0)
                    {
                        currentParagraph.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds - MinGapBetweenLines;
                    }

                    newParagraph.StartTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds + MinGapBetweenLines;
                }
            }
        }

        private void MergeSelectedLinesBilingual(int[] selectedIndices)
        {
            if (_subtitle.Paragraphs.Count > 0 && selectedIndices.Length > 1)
            {
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();
                var deleteIndices = new List<int>();
                bool first = true;
                int firstIndex = 0;
                double durationMilliseconds = 0;
                int next = 0;
                foreach (var index in selectedIndices)
                {
                    if (first)
                    {
                        firstIndex = index;
                        next = index + 1;
                        first = !first;
                    }
                    else
                    {
                        deleteIndices.Add(index);
                        if (next != index)
                        {
                            return;
                        }

                        next++;
                    }

                    var p = _subtitle.GetParagraphOrDefault(index);
                    if (p == null)
                    {
                        return;
                    }

                    var arr = p.Text.Trim().SplitToLines();
                    if (arr.Count > 0)
                    {
                        var mid = arr.Count / 2;
                        for (var i = 0; i < arr.Count; i++)
                        {
                            var l = arr[i];
                            if (i < mid)
                            {
                                sb1.Append(l).Append(' ');
                            }
                            else
                            {
                                sb2.Append(l).Append(' ');
                            }
                        }
                    }

                    durationMilliseconds += p.DurationTotalMilliseconds;
                }

                if (sb1.Length > 150 || sb2.Length > 150)
                {
                    return;
                }

                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(_language.BeforeMergeLines);

                var currentParagraph = _subtitle.Paragraphs[firstIndex];
                string text1 = sb1.ToString().Trim();
                string text2 = sb2.ToString().Trim();

                currentParagraph.Text = (text1 + Environment.NewLine + text2).Trim();

                //display time
                currentParagraph.EndTime.TotalMilliseconds = currentParagraph.StartTime.TotalMilliseconds + durationMilliseconds;

                var nextParagraph = _subtitle.GetParagraphOrDefault(next);
                if (nextParagraph != null && currentParagraph.EndTime.TotalMilliseconds > nextParagraph.StartTime.TotalMilliseconds && currentParagraph.StartTime.TotalMilliseconds < nextParagraph.StartTime.TotalMilliseconds)
                {
                    currentParagraph.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
                }

                if (_networkSession != null)
                {
                    _networkSession.TimerStop();
                    _networkSession.UpdateLine(firstIndex, currentParagraph);
                    NetworkGetSendUpdates(deleteIndices, 0, null);
                }
                else
                {
                    for (var i = deleteIndices.Count - 1; i >= 0; i--)
                    {
                        _subtitle.Paragraphs.RemoveAt(deleteIndices[i]);
                    }

                    _subtitle.Renumber();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }

                UpdateSourceView();
                ShowStatus(_language.LinesMerged);
                SubtitleListview1.SelectIndexAndEnsureVisible(firstIndex, true);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();
            }
        }

        private void MergeSelectedLines(BreakMode breakMode = BreakMode.Normal)
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 1)
            {
                var sb = new StringBuilder();
                var deleteIndices = new List<int>();
                bool first = true;
                int firstIndex = 0;
                double endMilliseconds = 0;
                int next = 0;
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    if (first)
                    {
                        firstIndex = index;
                        next = index + 1;
                        first = !first;
                    }
                    else
                    {
                        deleteIndices.Add(index);
                        if (next != index)
                        {
                            return;
                        }

                        next++;
                    }

                    var continuationStyle = Configuration.Settings.General.ContinuationStyle;
                    if (continuationStyle != ContinuationStyle.None)
                    {
                        var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                        if (next < firstIndex + SubtitleListview1.SelectedIndices.Count)
                        {
                            var mergeResult = ContinuationUtilities.MergeHelper(_subtitle.Paragraphs[index].Text, _subtitle.Paragraphs[index + 1].Text, continuationProfile, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle));
                            _subtitle.Paragraphs[index].Text = mergeResult.Item1;
                            _subtitle.Paragraphs[index + 1].Text = mergeResult.Item2;
                        }
                    }

                    var addText = _subtitle.Paragraphs[index].Text;

                    if (firstIndex != index)
                    {
                        addText = RemoveAssStartAlignmentTag(addText);
                    }

                    if (breakMode == BreakMode.UnbreakNoSpace)
                    {
                        sb.Append(addText);
                    }
                    else
                    {
                        sb.AppendLine(addText);
                    }

                    endMilliseconds = _subtitle.Paragraphs[index].EndTime.TotalMilliseconds;
                }

                if (HtmlUtil.RemoveHtmlTags(sb.ToString(), true).Length > 200)
                {
                    return;
                }

                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(_language.BeforeMergeLines);

                var currentParagraph = _subtitle.Paragraphs[firstIndex];
                string text = sb.ToString();
                text = HtmlUtil.FixInvalidItalicTags(text);
                text = FixAssaTagsAfterMerge(text);
                text = ChangeAllLinesTagsToSingleTag(text, "i");
                text = ChangeAllLinesTagsToSingleTag(text, "b");
                text = ChangeAllLinesTagsToSingleTag(text, "u");
                if (breakMode == BreakMode.Unbreak)
                {
                    text = Utilities.UnbreakLine(text);
                }
                else if (breakMode == BreakMode.UnbreakNoSpace)
                {
                    text = text.Replace(" " + Environment.NewLine + " ", string.Empty)
                        .Replace(Environment.NewLine + " ", string.Empty)
                        .Replace(" " + Environment.NewLine, string.Empty)
                        .Replace(Environment.NewLine, string.Empty);
                }
                else
                {
                    text = Utilities.AutoBreakLine(text, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle));
                }

                currentParagraph.Text = text;

                //display time
                currentParagraph.EndTime.TotalMilliseconds = endMilliseconds;

                var nextParagraph = _subtitle.GetParagraphOrDefault(next);
                if (nextParagraph != null && currentParagraph.EndTime.TotalMilliseconds > nextParagraph.StartTime.TotalMilliseconds && currentParagraph.StartTime.TotalMilliseconds < nextParagraph.StartTime.TotalMilliseconds)
                {
                    currentParagraph.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
                }

                // original subtitle
                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(firstIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        var originalTexts = new StringBuilder();
                        originalTexts.Append(original.Text.TrimEnd());
                        if (breakMode != BreakMode.UnbreakNoSpace)
                        {
                            originalTexts.Append(" ");
                        }

                        for (int i = 0; i < deleteIndices.Count; i++)
                        {
                            var originalNext = Utilities.GetOriginalParagraph(deleteIndices[i], _subtitle.Paragraphs[deleteIndices[i]], _subtitleOriginal.Paragraphs);
                            if (originalNext != null)
                            {
                                if (breakMode == BreakMode.UnbreakNoSpace)
                                {
                                    originalTexts.Append(originalNext.Text.Trim());
                                }
                                else
                                {
                                    originalTexts.Append(originalNext.Text).Append(' ');
                                }
                            }

                        }

                        for (int i = deleteIndices.Count - 1; i >= 0; i--)
                        {
                            var originalNext = Utilities.GetOriginalParagraph(deleteIndices[i], _subtitle.Paragraphs[deleteIndices[i]], _subtitleOriginal.Paragraphs);
                            if (originalNext != null)
                            {
                                _subtitleOriginal.Paragraphs.Remove(originalNext);
                            }
                        }

                        original.Text = originalTexts.ToString().Replace("  ", " ");
                        original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "i");
                        original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "b");
                        original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "u");

                        if (breakMode == BreakMode.Unbreak)
                        {
                            original.Text = Utilities.UnbreakLine(original.Text);
                        }
                        else if (breakMode == BreakMode.UnbreakNoSpace)
                        {
                            original.Text = original.Text.Replace(" " + Environment.NewLine + " ", string.Empty)
                                .Replace(Environment.NewLine + " ", string.Empty)
                                .Replace(" " + Environment.NewLine, string.Empty)
                                .Replace(Environment.NewLine, string.Empty);
                        }
                        else
                        {
                            original.Text = Utilities.AutoBreakLine(original.Text, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal));
                        }

                        original.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                        _subtitleOriginal.Renumber();
                    }
                }

                if (_networkSession != null)
                {
                    _networkSession.TimerStop();
                    _networkSession.UpdateLine(firstIndex, currentParagraph);
                    NetworkGetSendUpdates(deleteIndices, 0, null);
                }
                else
                {
                    for (int i = deleteIndices.Count - 1; i >= 0; i--)
                    {
                        _subtitle.Paragraphs.RemoveAt(deleteIndices[i]);
                    }

                    _subtitle.Renumber();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }

                UpdateSourceView();
                ShowStatus(_language.LinesMerged);
                SubtitleListview1.SelectIndexAndEnsureVisible(firstIndex, true);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();
            }
        }

        private static string FixAssaTagsAfterMerge(string text)
        {
            return text
                .Replace("{\\i0}{\\i1}", "")
                .Replace("{\\i0} {\\i1}", " ")
                .Replace($"{{\\i0}}{Environment.NewLine}{{\\i1}}", Environment.NewLine);
        }

        private static string ChangeAllLinesTagsToSingleTag(string text, string tag)
        {
            if (!text.Contains("<" + tag + ">"))
            {
                return text;
            }

            foreach (var line in text.SplitToLines())
            {
                if (!line.TrimStart().StartsWith("<" + tag + ">", StringComparison.Ordinal) || !line.TrimEnd().EndsWith("</" + tag + ">", StringComparison.Ordinal))
                {
                    return text;
                }
            }

            return "<" + tag + ">" + HtmlUtil.RemoveOpenCloseTags(text, tag).Trim() + "</" + tag + ">";
        }
        private void MergeAfterToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 0)
            {
                if (SubtitleListview1.SelectedItems.Count > 2)
                {
                    MergeSelectedLines();
                    return;
                }

                MergeWithLineAfter(false);
            }
        }

        public enum BreakMode
        {
            AutoBreak,
            Normal,
            Unbreak,
            UnbreakNoSpace
        }

        private void MergeWithLineAfter(bool insertDash, BreakMode breakMode = BreakMode.Normal)
        {
            var dialogHelper = new DialogSplitMerge { DialogStyle = Configuration.Settings.General.DialogStyle };
            int firstSelectedIndex = SubtitleListview1.SelectedItems[0].Index;

            var currentParagraph = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
            var nextParagraph = _subtitle.GetParagraphOrDefault(firstSelectedIndex + 1);

            if (nextParagraph != null && currentParagraph != null)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(_language.BeforeMergeLines);

                var continuationStyle = Configuration.Settings.General.ContinuationStyle;
                if (continuationStyle != ContinuationStyle.None && !insertDash)
                {
                    var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                    var mergeResult = ContinuationUtilities.MergeHelper(currentParagraph.Text, nextParagraph.Text, continuationProfile, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle));
                    currentParagraph.Text = mergeResult.Item1;
                    nextParagraph.Text = mergeResult.Item2;
                }

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(firstSelectedIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                    var originalNext = Utilities.GetOriginalParagraph(firstSelectedIndex + 1, nextParagraph, _subtitleOriginal.Paragraphs);

                    if (original != null && originalNext != null)
                    {
                        if (continuationStyle != ContinuationStyle.None && !insertDash)
                        {
                            var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                            var mergeResult = ContinuationUtilities.MergeHelper(original.Text, originalNext.Text, continuationProfile, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal));
                            original.Text = mergeResult.Item1;
                            originalNext.Text = mergeResult.Item2;
                        }
                    }

                    if (originalNext != null)
                    {
                        if (original == null)
                        {
                            originalNext.StartTime.TotalMilliseconds = currentParagraph.StartTime.TotalMilliseconds;
                            originalNext.EndTime.TotalMilliseconds = nextParagraph.EndTime.TotalMilliseconds;
                        }
                        else
                        {
                            if (insertDash && !string.IsNullOrEmpty(original.Text) && !string.IsNullOrEmpty(originalNext.Text))
                            {
                                string s = Utilities.UnbreakLine(original.Text);
                                original.Text = dialogHelper.InsertStartDash(s, 0);

                                s = Utilities.UnbreakLine(originalNext.Text);
                                original.Text += Environment.NewLine + dialogHelper.InsertStartDash(s, 1);

                                original.Text = original.Text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine).TrimEnd();
                            }
                            else
                            {
                                string old1 = original.Text;
                                string old2 = originalNext.Text;

                                if (breakMode == BreakMode.Unbreak)
                                {
                                    original.Text = old1.Replace(Environment.NewLine, " ");
                                    original.Text += Environment.NewLine + old2.Replace(Environment.NewLine, " ");
                                    original.Text = Utilities.UnbreakLine(original.Text);
                                }
                                else if (breakMode == BreakMode.UnbreakNoSpace)
                                {
                                    original.Text = old1.TrimEnd() + old2.TrimStart();
                                }
                                else
                                {
                                    original.Text = old1.Replace(Environment.NewLine, " ");
                                    original.Text += Environment.NewLine + old2.Replace(Environment.NewLine, " ");

                                    if (old1.Contains(Environment.NewLine) || old2.Contains(Environment.NewLine) ||
                                        old1.Length > Configuration.Settings.General.SubtitleLineMaximumLength || old2.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                                    {
                                        original.Text = Utilities.AutoBreakLine(original.Text, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal));
                                    }
                                }

                                original.Text = FixAssaTagsAfterMerge(original.Text);
                                original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "i");
                                original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "b");
                                original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "u");
                                original.Text = ChangeAllLinesTagsToSingleTag(original.Text, "box");

                                if (string.IsNullOrWhiteSpace(old1))
                                {
                                    original.Text = original.Text.TrimStart();
                                }

                                if (string.IsNullOrWhiteSpace(old2))
                                {
                                    original.Text = original.Text.TrimEnd();
                                }
                            }

                            original.EndTime = originalNext.EndTime;
                            _subtitleOriginal.Paragraphs.Remove(originalNext);
                        }

                        _subtitleOriginal.Renumber();
                    }
                }

                if (insertDash && !string.IsNullOrEmpty(currentParagraph.Text) && !string.IsNullOrEmpty(nextParagraph.Text))
                {
                    string s = Utilities.UnbreakLine(currentParagraph.Text);
                    currentParagraph.Text = dialogHelper.InsertStartDash(s, 0);

                    s = Utilities.UnbreakLine(RemoveAssStartAlignmentTag(nextParagraph.Text));
                    currentParagraph.Text += Environment.NewLine + dialogHelper.InsertStartDash(s, 1);

                    currentParagraph.Text = currentParagraph.Text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine).TrimEnd();
                }
                else
                {
                    string old1 = currentParagraph.Text;
                    string old2 = nextParagraph.Text;
                    if (breakMode == BreakMode.Unbreak)
                    {
                        currentParagraph.Text = currentParagraph.Text.Replace(Environment.NewLine, " ");
                        currentParagraph.Text += Environment.NewLine + nextParagraph.Text.Replace(Environment.NewLine, " ");
                        currentParagraph.Text = Utilities.UnbreakLine(RemoveAssStartAlignmentTag(currentParagraph.Text));
                    }
                    else if (breakMode == BreakMode.UnbreakNoSpace)
                    {
                        currentParagraph.Text = currentParagraph.Text.TrimEnd() + RemoveAssStartAlignmentTag(nextParagraph.Text).TrimStart();
                    }
                    else if (breakMode == BreakMode.AutoBreak)
                    {
                        currentParagraph.Text = currentParagraph.Text.Replace(Environment.NewLine, " ");
                        currentParagraph.Text += Environment.NewLine + RemoveAssStartAlignmentTag(nextParagraph.Text).Replace(Environment.NewLine, " ");
                        var language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
                        currentParagraph.Text = Utilities.AutoBreakLine(currentParagraph.Text, language);
                    }
                    else
                    {
                        currentParagraph.Text = (currentParagraph.Text.Trim() + Environment.NewLine +
                                                 RemoveAssStartAlignmentTag(nextParagraph.Text).Trim()).Trim();
                    }

                    currentParagraph.Text = FixAssaTagsAfterMerge(currentParagraph.Text);
                    currentParagraph.Text = ChangeAllLinesTagsToSingleTag(currentParagraph.Text, "i");
                    currentParagraph.Text = ChangeAllLinesTagsToSingleTag(currentParagraph.Text, "b");
                    currentParagraph.Text = ChangeAllLinesTagsToSingleTag(currentParagraph.Text, "u");
                    currentParagraph.Text = ChangeAllLinesTagsToSingleTag(currentParagraph.Text, "box");

                    if (old1.Contains(Environment.NewLine) || old2.Contains(Environment.NewLine) ||
                        old1.Length > Configuration.Settings.General.SubtitleLineMaximumLength || old2.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                    {
                        currentParagraph.Text = Utilities.AutoBreakLine(currentParagraph.Text, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle));
                    }

                    if (string.IsNullOrWhiteSpace(old1) && old2 != null)
                    {
                        currentParagraph.Text = old2.Trim();
                    }

                    if (string.IsNullOrWhiteSpace(old2) && old1 != null)
                    {
                        currentParagraph.Text = old1.Trim();
                    }
                }

                currentParagraph.EndTime.TotalMilliseconds = nextParagraph.EndTime.TotalMilliseconds;

                if (_networkSession != null)
                {
                    _networkSession.TimerStop();
                    SetDurationInSeconds(currentParagraph.DurationTotalSeconds);
                    _networkSession.UpdateLine(_subtitle.GetIndex(currentParagraph), currentParagraph);
                    var deleteIndices = new List<int> { _subtitle.GetIndex(nextParagraph) };
                    NetworkGetSendUpdates(deleteIndices, 0, null);
                }
                else
                {
                    _subtitle.Paragraphs.Remove(nextParagraph);
                    _subtitle.Renumber();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }

                UpdateSourceView();
                ShowStatus(_language.LinesMerged);
                SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();
                SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
            }
        }

        private static string RemoveAssStartAlignmentTag(string text)
        {
            var s = text.TrimStart();
            if (s.StartsWith("{\\an", StringComparison.Ordinal) && s.Length > 5 && s[5] == '}')
            {
                s = s.Remove(0, 6);
            }

            return s;
        }

        private void UpdateStartTimeInfo(TimeCode startTime, int index)
        {
            if (_subtitle.Paragraphs.Count > 0 && _subtitleListViewIndex >= 0 && startTime != null)
            {
                UpdateOverlapErrors(startTime, index);

                // update _subtitle + listview
                var p = _subtitle.Paragraphs[index];
                p.EndTime.TotalMilliseconds += (startTime.TotalMilliseconds - p.StartTime.TotalMilliseconds);
                p.StartTime = startTime;
                SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                StartUpdateListSyntaxColoring();
            }
        }

        private void StartUpdateListSyntaxColoring()
        {
            if (!_timerDoSyntaxColoring.Enabled)
            {
                _timerDoSyntaxColoring.Start();
            }
        }

        private void UpdateListSyntaxColoring()
        {
            if (_loading || _subtitle == null || _subtitleListViewIndex < 0 || _subtitleListViewIndex >= _subtitle.Paragraphs.Count)
            {
                return;
            }

            SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, _subtitleListViewIndex, _subtitle.Paragraphs[_subtitleListViewIndex]);
            var idx = _subtitleListViewIndex + 1;
            var p = _subtitle.GetParagraphOrDefault(idx);
            if (p != null)
            {
                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, idx, p);
            }

            idx = _subtitleListViewIndex - 1;
            p = _subtitle.GetParagraphOrDefault(idx);
            if (p != null)
            {
                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, idx, p);
            }
        }

        private void UpdateOverlapErrors(TimeCode startTime, int index)
        {
            string startTimeWarning = string.Empty;
            string durationWarning = string.Empty;
            if (_subtitle.Paragraphs.Count > 0 && _subtitleListViewIndex >= 0 && startTime != null)
            {
                var prevParagraph = _subtitle.GetParagraphOrDefault(index - 1);
                if (prevParagraph != null && !prevParagraph.EndTime.IsMaxTime && prevParagraph.EndTime.TotalMilliseconds > startTime.TotalMilliseconds && Configuration.Settings.Tools.ListViewSyntaxColorOverlap)
                {
                    startTimeWarning = string.Format(_languageGeneral.OverlapPreviousLineX, prevParagraph.EndTime.TotalSeconds - startTime.TotalSeconds);
                }

                var nextParagraph = _subtitle.GetParagraphOrDefault(index + 1);
                if (nextParagraph != null)
                {
                    double durationMilliSeconds = GetDurationInMilliseconds();
                    if (startTime.TotalMilliseconds + durationMilliSeconds > nextParagraph.StartTime.TotalMilliseconds &&
                        Configuration.Settings.Tools.ListViewSyntaxColorOverlap &&
                        !startTime.IsMaxTime)
                    {
                        durationWarning = string.Format(_languageGeneral.OverlapX, ((startTime.TotalMilliseconds + durationMilliSeconds) - nextParagraph.StartTime.TotalMilliseconds) / TimeCode.BaseUnit);
                    }

                    if (startTimeWarning.Length == 0 &&
                        startTime.TotalMilliseconds > nextParagraph.StartTime.TotalMilliseconds &&
                        Configuration.Settings.Tools.ListViewSyntaxColorOverlap &&
                        !startTime.IsMaxTime)
                    {
                        double di = (startTime.TotalMilliseconds - nextParagraph.StartTime.TotalMilliseconds) / TimeCode.BaseUnit;
                        startTimeWarning = string.Format(_languageGeneral.OverlapNextX, di);
                    }
                    else if (numericUpDownDuration.Value < 0)
                    {
                        durationWarning = _languageGeneral.Negative;
                    }
                }
            }

            if (!string.IsNullOrEmpty(startTimeWarning) && !string.IsNullOrEmpty(durationWarning))
            {
                labelStartTimeWarning.TextAlign = ContentAlignment.TopLeft;
                labelStartTimeWarning.Text = _languageGeneral.OverlapStartAndEnd;
                ShowStatus(startTimeWarning + "  " + durationWarning, false, 4, true);
            }
            else if (!string.IsNullOrEmpty(startTimeWarning))
            {
                labelStartTimeWarning.TextAlign = ContentAlignment.TopLeft;
                labelStartTimeWarning.Text = startTimeWarning;
            }
            else if (!string.IsNullOrEmpty(durationWarning))
            {
                labelStartTimeWarning.TextAlign = ContentAlignment.TopRight;
                labelStartTimeWarning.Text = durationWarning;
            }
            else
            {
                labelStartTimeWarning.Text = string.Empty;
            }
        }

        private double _durationMsInitialValue = 0;
        private bool _durationIsDirty = false;

        private double GetDurationInMilliseconds()
        {
            if (!_durationIsDirty)
            {
                return _durationMsInitialValue;
            }

            if (Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                var seconds = (int)numericUpDownDuration.Value;
                var frames = (int)Math.Round((Convert.ToDouble(numericUpDownDuration.Value) % 1.0 * 100.0));
                return seconds * TimeCode.BaseUnit + frames * (TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate);
            }

            return ((double)numericUpDownDuration.Value * TimeCode.BaseUnit);
        }

        private bool _skipDurationChangedEvent = false;

        private void SetDurationInSeconds(double seconds)
        {
            _durationIsDirty = false;
            _durationMsInitialValue = seconds * 1000.0;
            if (Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                var wholeSeconds = (int)seconds;
                var frames = SubtitleFormat.MillisecondsToFrames(seconds % 1.0 * TimeCode.BaseUnit);
                var extraSeconds = (int)(frames / Configuration.Settings.General.CurrentFrameRate);
                var restFrames = (int)(frames % Configuration.Settings.General.CurrentFrameRate);
                var v = (decimal)(wholeSeconds + extraSeconds + restFrames / 100.0);
                if (v >= numericUpDownDuration.Minimum && v <= numericUpDownDuration.Maximum)
                {
                    _skipDurationChangedEvent = true;
                    numericUpDownDuration.Value = (decimal)(wholeSeconds + extraSeconds + restFrames / 100.0);
                    _skipDurationChangedEvent = false;

                    int firstSelectedIndex = FirstSelectedIndex;
                    var currentParagraph = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
                    if (currentParagraph != null)
                    {
                        UpdateOverlapErrors(timeUpDownStartTime.TimeCode, firstSelectedIndex);
                        UpdateListViewTextCharactersPerSeconds(labelCharactersPerSecond, currentParagraph);

                        if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                        {
                            var original = Utilities.GetOriginalParagraph(firstSelectedIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                original.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                                UpdateListViewTextCharactersPerSeconds(labelOriginalCharactersPerSecond, original);
                            }
                        }

                        SubtitleListview1.SetDuration(firstSelectedIndex, currentParagraph, _subtitle.GetParagraphOrDefault(firstSelectedIndex + 1));
                        StartUpdateListSyntaxColoring();
                    }
                }
            }
            else
            {
                var d = (decimal)seconds;
                if (d > numericUpDownDuration.Maximum)
                {
                    numericUpDownDuration.Value = numericUpDownDuration.Maximum;
                }
                else if (d < numericUpDownDuration.Minimum)
                {
                    numericUpDownDuration.Value = numericUpDownDuration.Minimum;
                }
                else if (numericUpDownDuration.Value != d)
                {
                    numericUpDownDuration.Value = d;
                }
            }
        }

        private void NumericUpDownDurationValueChanged(object sender, EventArgs e)
        {
            if (_skipDurationChangedEvent)
            {
                return;
            }

            _durationIsDirty = true;
            if (_subtitle.Paragraphs.Count > 0 && _subtitleListViewIndex >= 0)
            {
                labelStatus.Text = string.Empty;
                int firstSelectedIndex = _subtitleListViewIndex;
                var currentParagraph = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
                if (currentParagraph != null)
                {
                    // update _subtitle + listview
                    var oldDuration = currentParagraph.Duration.ToString();
                    var temp = new Paragraph(currentParagraph);

                    if (Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                    {
                        var seconds = (int)numericUpDownDuration.Value;
                        var frames = Convert.ToInt32((numericUpDownDuration.Value - seconds) * 100);
                        if (frames > Math.Round(Configuration.Settings.General.CurrentFrameRate) - 1)
                        {
                            numericUpDownDuration.ValueChanged -= NumericUpDownDurationValueChanged;
                            if (frames >= 99)
                            {
                                numericUpDownDuration.Value = (decimal)(seconds + (Math.Round((Configuration.Settings.General.CurrentFrameRate - 1)) / 100.0));
                            }
                            else
                            {
                                numericUpDownDuration.Value = seconds + 1;
                            }

                            numericUpDownDuration.ValueChanged += NumericUpDownDurationValueChanged;
                        }
                    }

                    temp.EndTime.TotalMilliseconds = currentParagraph.StartTime.TotalMilliseconds + GetDurationInMilliseconds();

                    MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.DisplayTimeAdjustedX, "#" + currentParagraph.Number + ": " + oldDuration + " -> " + temp.Duration));

                    currentParagraph.EndTime.TotalMilliseconds = temp.EndTime.TotalMilliseconds;
                    SubtitleListview1.SetDuration(firstSelectedIndex, currentParagraph, _subtitle.GetParagraphOrDefault(firstSelectedIndex + 1));

                    UpdateOverlapErrors(timeUpDownStartTime.TimeCode, firstSelectedIndex);
                    UpdateListViewTextCharactersPerSeconds(labelCharactersPerSecond, currentParagraph);

                    if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                    {
                        var original = Utilities.GetOriginalParagraph(firstSelectedIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            original.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                            UpdateListViewTextCharactersPerSeconds(labelOriginalCharactersPerSecond, original);
                        }
                    }

                    StartUpdateListSyntaxColoring();
                }

                StartUpdateListSyntaxColoring();
            }
        }

        private void InitializeListViewEditBoxOriginal(Paragraph p, int firstSelectedIndex)
        {
            if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                var original = Utilities.GetOriginalParagraph(firstSelectedIndex, p, _subtitleOriginal.Paragraphs);
                if (original == null)
                {
                    textBoxListViewTextOriginal.Enabled = false;
                    textBoxListViewTextOriginal.Text = string.Empty;
                    labelOriginalCharactersPerSecond.Text = string.Empty;
                }
                else
                {
                    textBoxListViewTextOriginal.Enabled = Configuration.Settings.General.AllowEditOfOriginalSubtitle;
                    textBoxListViewTextOriginal.BackColor = textBoxListViewTextOriginal.Focused ? SystemColors.Highlight : SystemColors.WindowFrame;
                    textBoxListViewTextOriginal.TextChanged -= TextBoxListViewTextOriginalTextChanged;
                    textBoxListViewTextOriginal.Text = original.Text;
                    textBoxListViewTextOriginal.TextChanged += TextBoxListViewTextOriginalTextChanged;
                    UpdateListViewTextCharactersPerSeconds(labelOriginalCharactersPerSecond, original);
                    _listViewOriginalTextUndoLast = original.Text;
                }
            }
        }

        private void InitializeListViewEditBox(Paragraph p)
        {
            textBoxListViewText.TextChanged -= TextBoxListViewTextTextChanged;
            textBoxListViewText.Text = p.Text;
            textBoxListViewText.TextChanged += TextBoxListViewTextTextChanged;
            _listViewTextUndoLast = p.Text;

            var format = GetCurrentSubtitleFormat();
            bool isAssa = format.GetType() == typeof(AdvancedSubStationAlpha);
            numericUpDownLayer.Visible = isAssa;
            labelLayer.Visible = isAssa;
            if (isAssa)
            {
                labelLayer.Text = LanguageSettings.Current.General.Layer;
                numericUpDownLayer.Left = labelLayer.Right + 5;
                numericUpDownLayer.ValueChanged -= NumericUpDownLayer_ValueChanged;
                numericUpDownLayer.Value = p.Layer;
                numericUpDownLayer.ValueChanged += NumericUpDownLayer_ValueChanged;
            }

            timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
            timeUpDownStartTime.TimeCode = p.StartTime;
            timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;

            numericUpDownDuration.ValueChanged -= NumericUpDownDurationValueChanged;
            if (p.DurationTotalSeconds > (double)numericUpDownDuration.Maximum)
            {
                SetDurationInSeconds((double)numericUpDownDuration.Maximum);
            }
            else
            {
                SetDurationInSeconds(p.DurationTotalSeconds);
            }

            numericUpDownDuration.ValueChanged += NumericUpDownDurationValueChanged;

            UpdateOverlapErrors(timeUpDownStartTime.TimeCode, _subtitle.Paragraphs.IndexOf(p));
            UpdateListViewTextCharactersPerSeconds(labelCharactersPerSecond, p);
            if (_subtitle != null && _subtitle.Paragraphs.Count > 0)
            {
                textBoxListViewText.Enabled = true;
                textBoxListViewText.BackColor = textBoxListViewText.Focused ? SystemColors.Highlight : SystemColors.WindowFrame;
                EnableOrDisableEditControls();
            }

            StartUpdateListSyntaxColoring();
            ShowHideBookmark(p);
        }

        private void InitializeListViewEditBoxTimeOnly(Paragraph p)
        {
            timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
            timeUpDownStartTime.TimeCode = p.StartTime;
            timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;

            numericUpDownDuration.ValueChanged -= NumericUpDownDurationValueChanged;
            if (p.DurationTotalSeconds > (double)numericUpDownDuration.Maximum)
            {
                SetDurationInSeconds((double)numericUpDownDuration.Maximum);
            }
            else
            {
                SetDurationInSeconds(p.DurationTotalSeconds);
            }

            numericUpDownDuration.ValueChanged += NumericUpDownDurationValueChanged;

            UpdateOverlapErrors(timeUpDownStartTime.TimeCode, _subtitle.Paragraphs.IndexOf(p));
            StartUpdateListSyntaxColoring();
        }

        private void MaskedTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_subtitleListViewIndex >= 0 && SubtitleListview1.Items.Count > 0)
            {
                MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.StartTimeAdjustedX, "#" + (_subtitleListViewIndex + 1) + ": " + timeUpDownStartTime.TimeCode));

                var oldParagraph = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
                if (oldParagraph != null)
                {
                    oldParagraph = new Paragraph(oldParagraph, false);
                }

                labelStatus.Text = string.Empty;
                UpdateStartTimeInfo(timeUpDownStartTime.TimeCode, _subtitleListViewIndex);
                UpdateOriginalTimeCodes(oldParagraph);
            }
        }

        private void UpdateOriginalTimeCodes(Paragraph currentPargraphBeforeChange, Paragraph p2Before = null)
        {
            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                Paragraph p1 = null;
                Paragraph o1 = null;
                if (currentPargraphBeforeChange != null)
                {
                    p1 = _subtitle.GetParagraphOrDefaultById(currentPargraphBeforeChange.Id);
                    if (p1 != null)
                    {
                        o1 = Utilities.GetOriginalParagraph(_subtitle.Paragraphs.IndexOf(p1), currentPargraphBeforeChange, _subtitleOriginal.Paragraphs);
                    }
                }

                Paragraph p2 = null;
                Paragraph o2 = null;
                if (p2Before != null)
                {
                    p2 = _subtitle.GetParagraphOrDefaultById(p2Before.Id);
                    if (p2 != null)
                    {
                        o2 = Utilities.GetOriginalParagraph(_subtitle.Paragraphs.IndexOf(p2), p2Before, _subtitleOriginal.Paragraphs);
                    }
                }

                if (o1 != null)
                {
                    o1.StartTime.TotalMilliseconds = p1.StartTime.TotalMilliseconds;
                    o1.EndTime.TotalMilliseconds = p1.EndTime.TotalMilliseconds;
                }

                if (o2 != null)
                {
                    o2.StartTime.TotalMilliseconds = p2.StartTime.TotalMilliseconds;
                    o2.EndTime.TotalMilliseconds = p2.EndTime.TotalMilliseconds;
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClose)
            {
                return;
            }

            _lastDoNotPrompt = -1;
            ReloadFromSourceView();
            if (!ContinueNewOrExit())
            {
                e.Cancel = true;
                return;
            }

            if (_networkSession != null)
            {
                try
                {
                    _networkSession.TimerStop();
                    _networkSession.Leave();
                }
                catch
                {
                    // ignore
                }
            }

            _timerSlow.Stop();
            if (Configuration.Settings.General.StartRememberPositionAndSize && WindowState != FormWindowState.Minimized)
            {
                Configuration.Settings.General.StartPosition = Left + ";" + Top;
                if (WindowState == FormWindowState.Maximized)
                {
                    Configuration.Settings.General.StartSize = "Maximized";
                }
                else
                {
                    Configuration.Settings.General.StartSize = Width + ";" + Height;
                }

                Configuration.Settings.General.LayoutSizes = LayoutManager.SaveLayout();
            }

            Configuration.Settings.General.AutoRepeatOn = checkBoxAutoRepeatOn.Checked;
            if (int.TryParse(comboBoxAutoRepeat.Text, out var autoRepeat))
            {
                Configuration.Settings.General.AutoRepeatCount = autoRepeat;
            }

            Configuration.Settings.General.AutoContinueOn = checkBoxAutoContinue.Checked;
            Configuration.Settings.General.AutoContinueDelay = comboBoxAutoContinue.SelectedIndex;
            Configuration.Settings.General.SyncListViewWithVideoWhilePlaying = checkBoxSyncListViewWithVideoWhilePlaying.Checked;
            Configuration.Settings.General.ShowWaveform = audioVisualizer.ShowWaveform;
            Configuration.Settings.General.ShowSpectrogram = audioVisualizer.ShowSpectrogram;
            Configuration.Settings.General.LayoutNumber = _layout;
            if (Configuration.Settings.General.ShowRecentFiles)
            {
                if (!string.IsNullOrEmpty(_fileName))
                {
                    Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                }
                else if (Configuration.Settings.RecentFiles.Files.Count > 0)
                {
                    Configuration.Settings.RecentFiles.Add(null, null, 1, null);
                }
            }

            if (SubtitleListview1.StateImageList?.Images.Count > 0)
            {
                Configuration.Settings.General.ListViewNumberWidth = SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Width - 18;
            }
            else
            {
                Configuration.Settings.General.ListViewNumberWidth = SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Width;
            }

            SaveUndockedPositions();
            ListViewHelper.SaveListViewState(SubtitleListview1, _subtitle);
            CheckSecondSubtitleReset();
            Configuration.Settings.Save();

            if (mediaPlayer.VideoPlayer != null)
            {
                mediaPlayer.PauseAndDisposePlayer();
            }

            foreach (var fileToDelete in _filesToDelete)
            {
                try
                {
                    File.Delete(fileToDelete);
                }
                catch
                {
                    // ignore
                }
            }

            _dictateForm?.Dispose();

            if (!e.Cancel)
            {
                e.Cancel = true; // Hack as FormClosing will crash if any Forms are created here (e.g. a msgbox). 
                _forceClose = true;
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(10), () => Application.Exit());
            }
        }

        private void SaveUndockedPositions()
        {
            if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed)
            {
                Configuration.Settings.General.UndockedVideoFullscreen = _videoPlayerUndocked.IsFullscreen;
                Configuration.Settings.General.UndockedVideoPosition = _videoPlayerUndocked.Left + @";" + _videoPlayerUndocked.Top + @";" + _videoPlayerUndocked.Width + @";" + _videoPlayerUndocked.Height;
            }

            if (_waveformUndocked != null && !_waveformUndocked.IsDisposed)
            {
                Configuration.Settings.General.UndockedWaveformPosition = _waveformUndocked.Left + @";" + _waveformUndocked.Top + @";" + _waveformUndocked.Width + @";" + _waveformUndocked.Height;
            }

            if (_videoControlsUndocked != null && !_videoControlsUndocked.IsDisposed)
            {
                Configuration.Settings.General.UndockedVideoControlsPosition = _videoControlsUndocked.Left + @";" + _videoControlsUndocked.Top + @";" + _videoControlsUndocked.Width + @";" + _videoControlsUndocked.Height;
            }
        }

        private void BreakUnbreakTextBox(bool unbreak, SETextBox tb, bool removeNewLineOnly = false)
        {
            var textCaretPos = tb.SelectionStart;
            var startText = tb.Text.Substring(0, textCaretPos);
            var numberOfNewLines = Utilities.CountTagInText(startText, Environment.NewLine);
            if (unbreak)
            {
                textCaretPos -= numberOfNewLines;
                if (removeNewLineOnly)
                {
                    tb.Text = tb.Text.Replace(Environment.NewLine, string.Empty);
                }
                else
                {
                    tb.Text = Utilities.UnbreakLine(tb.Text);
                }
            }
            else
            {
                int i = 0;
                string s;
                bool useLanguage = false;
                var language = "en";
                if (Configuration.Settings.Tools.UseNoLineBreakAfter && tb == textBoxListViewText)
                {
                    language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
                    useLanguage = true;
                }
                else if (Configuration.Settings.Tools.UseNoLineBreakAfter && tb == textBoxListViewTextOriginal)
                {
                    language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
                    useLanguage = true;
                }

                if (useLanguage)
                {
                    s = Utilities.AutoBreakLine(tb.Text, language);
                }
                else
                {
                    s = Utilities.AutoBreakLine(tb.Text);
                }

                while (i < textCaretPos && i < s.Length)
                {
                    var ch = s[i];
                    if (ch == '\n')
                    {
                        textCaretPos++;
                    }

                    i++;
                }

                textCaretPos -= numberOfNewLines;
                tb.Text = s;
            }

            tb.SelectionStart = textCaretPos;
        }

        private void ButtonUnBreakClick(object sender, EventArgs e)
        {
            Unbreak();
        }

        private void Unbreak(bool removeNewLineOnly = false)
        {
            _doAutoBreakOnTextChanged = false;

            var textCaretPos = textBoxListViewText.SelectionStart;
            var startText = textBoxListViewText.Text.Substring(0, textCaretPos);
            var numberOfNewLines = Utilities.CountTagInText(startText, Environment.NewLine);
            textCaretPos -= numberOfNewLines;
            bool historyAdded = false;

            if (SubtitleListview1.SelectedItems.Count > 1)
            {
                SubtitleListview1.BeginUpdate();
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    var p = _subtitle.GetParagraphOrDefault(index);
                    var oldText = p.Text;
                    var newText = p.Text;
                    if (removeNewLineOnly)
                    {
                        newText = newText.Replace(Environment.NewLine, string.Empty);
                    }
                    else
                    {
                        newText = Utilities.UnbreakLine(newText);
                    }

                    if (oldText != newText)
                    {
                        if (!historyAdded)
                        {
                            historyAdded = true;
                            MakeHistoryForUndo(_language.BeforeRemoveLineBreaksInSelectedLines);
                        }

                        p.Text = newText;
                        SubtitleListview1.SetText(index, p.Text);
                        SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, index, p);
                    }

                    if (_subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible && Configuration.Settings.General.AllowEditOfOriginalSubtitle)
                    {
                        var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            oldText = original.Text;
                            newText = original.Text;
                            if (removeNewLineOnly)
                            {
                                newText = newText.Replace(Environment.NewLine, string.Empty);
                            }
                            else
                            {
                                newText = Utilities.UnbreakLine(newText);
                            }

                            if (oldText != newText)
                            {
                                if (!historyAdded)
                                {
                                    historyAdded = true;
                                    MakeHistoryForUndo(_language.BeforeRemoveLineBreaksInSelectedLines);
                                }

                                original.Text = newText;
                                SubtitleListview1.SetOriginalText(index, original.Text);
                            }
                        }
                    }
                }

                SubtitleListview1.EndUpdate();
                RefreshSelectedParagraph();
            }
            else
            {
                var fixedText = removeNewLineOnly ? textBoxListViewText.Text.Replace(Environment.NewLine, string.Empty) : Utilities.UnbreakLine(textBoxListViewText.Text);
                var makeHistory = textBoxListViewText.Text != fixedText;
                if (IsOriginalEditable)
                {
                    var originalFixedText = removeNewLineOnly ? textBoxListViewText.Text.Replace(Environment.NewLine, string.Empty) : Utilities.UnbreakLine(textBoxListViewTextOriginal.Text);
                    if (!makeHistory)
                    {
                        makeHistory = textBoxListViewTextOriginal.Text != originalFixedText;
                    }

                    if (makeHistory)
                    {
                        MakeHistoryForUndo(_language.BeforeRemoveLineBreaksInSelectedLines);
                        textBoxListViewText.Text = fixedText;
                    }

                    textBoxListViewTextOriginal.Text = originalFixedText;
                }
                else if (makeHistory)
                {
                    MakeHistoryForUndo(_language.BeforeRemoveLineBreaksInSelectedLines);
                    textBoxListViewText.Text = fixedText;
                }

                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, _subtitleListViewIndex, _subtitle.GetParagraphOrDefault(_subtitleListViewIndex));
            }

            _doAutoBreakOnTextChanged = true;
            textBoxListViewText.SelectionStart = textCaretPos;
        }


        private void AutoBreak()
        {
            string language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
            string languageOriginal = string.Empty;
            if (_subtitleOriginal != null)
            {
                languageOriginal = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
            }

            var textCaretPos = textBoxListViewText.SelectionStart;

            if (SubtitleListview1.SelectedItems.Count > 1)
            {
                bool historyAdded = false;
                SubtitleListview1.BeginUpdate();
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    var p = _subtitle.GetParagraphOrDefault(index);
                    if (p != null)
                    {
                        var oldText = p.Text;
                        var newText = Utilities.AutoBreakLine(p.Text, language);
                        if (oldText != newText)
                        {
                            if (!historyAdded)
                            {
                                historyAdded = true;
                                MakeHistoryForUndo(_language.Controls.AutoBreak.RemoveChar('&'));
                            }

                            p.Text = newText;
                            SubtitleListview1.SetText(index, p.Text);
                        }

                        if (_subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible && Configuration.Settings.General.AllowEditOfOriginalSubtitle)
                        {
                            var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                oldText = original.Text;
                                newText = Utilities.AutoBreakLine(original.Text, language);
                                if (oldText != newText)
                                {
                                    if (!historyAdded)
                                    {
                                        historyAdded = true;
                                        MakeHistoryForUndo(_language.Controls.AutoBreak.RemoveChar('&'));
                                    }

                                    original.Text = newText;
                                    SubtitleListview1.SetOriginalText(index, original.Text);
                                }
                            }
                        }

                        SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, index, p);
                    }
                }

                SubtitleListview1.EndUpdate();
                RefreshSelectedParagraph();
            }
            else
            {
                var fixedText = Utilities.AutoBreakLine(textBoxListViewText.Text, language);
                var makeHistory = textBoxListViewText.Text != fixedText;
                if (IsOriginalEditable)
                {
                    var originalFixedText = Utilities.AutoBreakLine(textBoxListViewTextOriginal.Text, languageOriginal);
                    if (!makeHistory)
                    {
                        makeHistory = textBoxListViewTextOriginal.Text != originalFixedText;
                    }

                    if (makeHistory)
                    {
                        MakeHistoryForUndo(_language.Controls.AutoBreak.RemoveChar('&'));
                        textBoxListViewText.Text = fixedText;
                    }

                    textBoxListViewTextOriginal.Text = originalFixedText;
                }
                else if (makeHistory)
                {
                    MakeHistoryForUndo(_language.Controls.AutoBreak.RemoveChar('&'));
                    textBoxListViewText.Text = fixedText;
                }
            }

            var s = textBoxListViewText.Text;
            var startText = s.Substring(0, Math.Min(textCaretPos, s.Length));
            var numberOfNewLines = Utilities.CountTagInText(startText, Environment.NewLine);
            textCaretPos += numberOfNewLines;
            if (s.Length > textCaretPos && '\n' == s[textCaretPos])
            {
                textCaretPos--;
            }

            if (textCaretPos > 0)
            {
                textBoxListViewText.SelectionStart = textCaretPos;
            }
        }

        private void SwitchView(Control view)
        {
            if (InSourceView)
            {
                var currentFormat = GetCurrentSubtitleFormat();
                if (currentFormat != null && currentFormat.IsTextBased)
                {
                    var newFormat = new Subtitle().ReloadLoadSubtitle(textBoxSource.Lines.ToList(), null, currentFormat);
                    if (newFormat == null && !string.IsNullOrWhiteSpace(textBoxSource.Text))
                    {
                        MessageBox.Show(_language.UnableToParseSourceView);
                        return;
                    }
                }
            }

            if (view == ListView)
            {
                textBoxSource.Visible = false;
            }
            else
            {
                textBoxSource.Parent.Controls.Remove(textBoxSource);
                SubtitleListview1.Parent.Parent.Parent.Controls.Add(textBoxSource);
                textBoxSource.Dock = DockStyle.Fill;
                textBoxSource.BringToFront();
                textBoxSource.Visible = true;
            }

            toolStripButtonSourceView.Checked = InSourceView;
        }

        private void SetColor(string color, bool selectedText = false, bool allowRemove = true)
        {
            var format = GetCurrentSubtitleFormat();
            var isAssa = format.GetType() == typeof(AdvancedSubStationAlpha);
            var isWebVtt = format.Name == WebVTT.NameOfFormat || format.Name == WebVTTFileWithLineNumber.NameOfFormat;
            var c = HtmlUtil.GetColorFromString(color);

            if (selectedText)
            {
                SetSelectedTextColor(color);
            }
            else
            {
                var webVttStyles = new List<WebVttStyle>();
                if (isWebVtt)
                {
                    webVttStyles = WebVttHelper.GetStyles(_subtitle.Header);
                }

                MakeHistoryForUndo(_language.BeforeSettingColor);
                var remove = allowRemove;
                var removeOriginal = allowRemove;

                var assaColor = string.Empty;
                if (isAssa)
                {
                    assaColor = AdvancedSubStationAlpha.GetSsaColorStringForEvent(c);
                }

                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    var p = _subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        if (isAssa)
                        {
                            if (!p.Text.Contains(assaColor, StringComparison.OrdinalIgnoreCase))
                            {
                                remove = false;
                                break;
                            }
                        }
                        else if (isWebVtt)
                        {
                            var removeFound = false;
                            foreach (var style in webVttStyles)
                            {
                                if (style.Color == c && p.Text.Contains("." + style.Name))
                                {
                                    removeFound = true;
                                }
                            }

                            if (!removeFound)
                            {
                                remove = false;
                                break;
                            }
                        }
                        else
                        {
                            var s = Utilities.RemoveSsaTags(p.Text);
                            if (!s.StartsWith("<font ", StringComparison.OrdinalIgnoreCase) || !s.Contains(color, StringComparison.OrdinalIgnoreCase))
                            {
                                remove = false;
                                break;
                            }

                            if (assaColor.Length > 0 && !s.Contains(assaColor))
                            {
                                remove = false;
                                break;
                            }
                        }
                    }
                }

                if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                    {
                        var p = _subtitle.GetParagraphOrDefault(item.Index);
                        if (p != null)
                        {
                            var original = Utilities.GetOriginalParagraph(item.Index, p, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                var s = Utilities.RemoveSsaTags(original.Text);
                                if (!s.StartsWith("<font ", StringComparison.OrdinalIgnoreCase) || !s.Contains(color, StringComparison.OrdinalIgnoreCase))
                                {
                                    removeOriginal = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    var p = _subtitle.GetParagraphOrDefault(item.Index);
                    if (p != null)
                    {
                        if (remove)
                        {
                            if (isAssa)
                            {
                                p.Text = HtmlUtil.RemoveAssaColor(p.Text);
                            }
                            else if (isWebVtt)
                            {
                                p.Text = WebVttHelper.RemoveColorTag(p.Text, c, webVttStyles);
                            }
                            else
                            {
                                p.Text = HtmlUtil.RemoveOpenCloseTags(p.Text, HtmlUtil.TagFont);
                            }
                        }
                        else
                        {
                            SetParagraphFontColor(_subtitle, p, color, isAssa, isWebVtt, webVttStyles);
                        }

                        SubtitleListview1.SetText(item.Index, p.Text);

                        if (IsOriginalEditable && SubtitleListview1.IsOriginalTextColumnVisible)
                        {
                            var original = Utilities.GetOriginalParagraph(item.Index, p, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                if (removeOriginal)
                                {
                                    if (isWebVtt)
                                    {
                                        original.Text = WebVttHelper.RemoveColorTag(original.Text, c, webVttStyles);
                                    }
                                    else
                                    {
                                        original.Text = HtmlUtil.RemoveOpenCloseTags(original.Text, HtmlUtil.TagFont);
                                    }
                                }
                                else
                                {
                                    SetParagraphFontColor(_subtitleOriginal, original, color);
                                }

                                SubtitleListview1.SetOriginalText(item.Index, original.Text);
                            }
                        }
                    }
                }

                RefreshSelectedParagraph();
            }
        }

        private void SetSelectedTextColor(string color)
        {
            var tb = GetFocusedTextBox();
            bool allSelected;
            string text = tb.SelectedText;
            if (string.IsNullOrEmpty(text) && tb.Text.Length > 0)
            {
                text = tb.Text;
                tb.SelectAll();
                allSelected = true;
            }
            else
            {
                allSelected = tb.SelectionLength == tb.Text.Length;
            }

            int selectionStart = tb.SelectionStart;

            var format = GetCurrentSubtitleFormat();

            if (IsAssa())
            {
                var c = HtmlUtil.GetColorFromString(color);
                var assaColor = AdvancedSubStationAlpha.GetSsaColorStringForEvent(c);
                if (allSelected)
                {
                    text = $"{{\\c{assaColor}&}}{text}";
                }
                else
                {
                    var p = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
                    if (p != null)
                    {
                        var style = AdvancedSubStationAlpha.GetSsaStyle(p.Extra, _subtitle.Header);
                        text = $"{{\\{assaColor}&}}{text}{{\\{AdvancedSubStationAlpha.GetSsaColorStringForEvent(style.Primary)}&}}";
                    }
                }

                tb.SelectedText = text;
                tb.SelectionStart = selectionStart;
                tb.SelectionLength = text.Length;

                return;
            }
            else if (format.Name == WebVTT.NameOfFormat || format.Name == WebVTTFileWithLineNumber.NameOfFormat)
            {
                var c = HtmlUtil.GetColorFromString(color);
                WebVttStyle styleWithColor = WebVttHelper.GetOnlyColorStyle(c, _subtitle.Header);
                if (styleWithColor == null)
                {
                    styleWithColor = WebVttHelper.AddStyleFromColor(c);
                    _subtitle.Header = WebVttHelper.AddStyleToHeader(_subtitle.Header, styleWithColor);
                }

                if (text.StartsWith("<c.", StringComparison.Ordinal))
                {
                    var indexOfEndTag = text.IndexOf('>');
                    if (indexOfEndTag > 0)
                    {
                        text = text.Insert(indexOfEndTag, "." + styleWithColor.Name.RemoveChar('.'));
                    }
                }
                else
                {
                    text = "<c." + styleWithColor.Name.RemoveChar('.') + ">" + text + "</c>";
                }

                text = WebVttHelper.RemoveUnusedColorStylesFromText(text, _subtitle.Header);

                tb.SelectedText = text;
                tb.SelectionStart = selectionStart;
                tb.SelectionLength = text.Length;

                return;
            }

            bool done = false;
            string pre = string.Empty;
            if (selectionStart == 0 && text.StartsWith("{\\", StringComparison.Ordinal) && text.IndexOf('}') >= 0)
            {
                int endIndex = text.IndexOf('}') + 1;
                pre = text.Substring(0, endIndex);
                text = text.Remove(0, endIndex);
            }

            string s = text;
            if (s.StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
            {
                if (s.EndsWith("</font>", StringComparison.Ordinal) &&
                    s.StartsWith($"<font color=\"{color}\">", StringComparison.Ordinal))
                {
                    var start = $"<font color=\"{color}\">";
                    text = s.Substring(start.Length, s.Length - start.Length - "</font>".Length);
                    done = true;
                }
                else
                {
                    int end = s.IndexOf('>');
                    if (end > 0)
                    {
                        string f = s.Substring(0, end);
                        if (f.Contains(" face=", StringComparison.OrdinalIgnoreCase) && !f.Contains(" color=", StringComparison.OrdinalIgnoreCase))
                        {
                            var start = s.IndexOf(" face=", StringComparison.OrdinalIgnoreCase);
                            s = s.Insert(start, string.Format(" color=\"{0}\"", color));
                            text = s;
                            done = true;
                        }
                        else if (f.Contains(" color=", StringComparison.OrdinalIgnoreCase))
                        {
                            int colorStart = f.IndexOf(" color=", StringComparison.OrdinalIgnoreCase);
                            if (s.IndexOf('"', colorStart + " color=".Length + 1) > 0)
                            {
                                end = s.IndexOf('"', colorStart + " color=".Length + 1);
                            }

                            s = s.Substring(0, colorStart) + string.Format(" color=\"{0}", color) + s.Substring(end);
                            text = s;
                            done = true;
                        }
                    }
                }
            }

            if (!done)
            {
                text = $"{pre}<font color=\"{color}\">{text}</font>";
            }
            else
            {
                text = pre + text;
            }

            tb.SelectedText = text;
            tb.SelectionStart = selectionStart;
            tb.SelectionLength = text.Length;
        }

        private void SetParagraphFontColor(Subtitle subtitle, Paragraph p, string color, bool isAssa = false, bool isWebVtt = false, List<WebVttStyle> webVttStyles = null)
        {
            if (p == null)
            {
                return;
            }

            if (isAssa)
            {
                try
                {
                    var c = HtmlUtil.GetColorFromString(color);
                    p.Text = HtmlUtil.RemoveAssaColor(p.Text);
                    p.Text = "{\\" + AdvancedSubStationAlpha.GetSsaColorStringForEvent(c) + "&}" + p.Text;
                }
                catch
                {
                    // ignore
                }

                return;
            }

            if (isWebVtt)
            {
                try
                {
                    var c = HtmlUtil.GetColorFromString(color);
                    var existingStyle = WebVttHelper.GetOnlyColorStyle(c, _subtitle.Header);
                    if (existingStyle != null)
                    {
                        p.Text = WebVttHelper.AddStyleToText(p.Text, existingStyle, WebVttHelper.GetStyles(_subtitle.Header));
                        p.Text = WebVttHelper.RemoveUnusedColorStylesFromText(p.Text, subtitle.Header);
                    }
                    else
                    {
                        var styleWithColor = WebVttHelper.AddStyleFromColor(c);
                        subtitle.Header = WebVttHelper.AddStyleToHeader(_subtitle.Header, styleWithColor);
                        p.Text = WebVttHelper.AddStyleToText(p.Text, styleWithColor, WebVttHelper.GetStyles(_subtitle.Header));
                        p.Text = WebVttHelper.RemoveUnusedColorStylesFromText(p.Text, subtitle.Header);
                    }
                }
                catch
                {
                    // ignore
                }

                return;
            }

            string pre = string.Empty;
            if (p.Text.StartsWith("{\\", StringComparison.Ordinal) && p.Text.IndexOf('}') >= 0)
            {
                int endIndex = p.Text.IndexOf('}') + 1;
                pre = p.Text.Substring(0, endIndex);
                p.Text = p.Text.Remove(0, endIndex);
            }

            string s = p.Text;
            if (s.StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
            {
                int end = s.IndexOf('>');
                if (end > 0)
                {
                    string f = s.Substring(0, end);

                    if (f.Contains(" face=", StringComparison.OrdinalIgnoreCase) && !f.Contains(" color=", StringComparison.OrdinalIgnoreCase))
                    {
                        var start = s.IndexOf(" face=", StringComparison.OrdinalIgnoreCase);
                        s = s.Insert(start, string.Format(" color=\"{0}\"", color));
                        p.Text = pre + s;
                        return;
                    }

                    var colorStart = f.IndexOf(" color=", StringComparison.OrdinalIgnoreCase);
                    if (colorStart >= 0)
                    {
                        if (s.IndexOf('"', colorStart + 8) > 0)
                        {
                            end = s.IndexOf('"', colorStart + 8);
                        }

                        s = s.Substring(0, colorStart) + string.Format(" color=\"{0}", color) + s.Substring(end);
                        p.Text = pre + s;
                        return;
                    }
                }
            }

            p.Text = $"{pre}<font color=\"{color}\">{p.Text}</font>";
        }

        private void ImportSubtitleFromMatroskaFile(string fileName)
        {
            using (var matroska = new MatroskaFile(fileName))
            {
                if (matroska.IsValid)
                {
                    var subtitleList = matroska.GetTracks(true);
                    if (subtitleList.Count == 0)
                    {
                        MessageBox.Show(_language.NoSubtitlesFound);
                    }
                    else if (ContinueNewOrExit())
                    {
                        if (subtitleList.Count > 1)
                        {
                            using (var subtitleChooser = new MatroskaSubtitleChooser("mkv"))
                            {
                                subtitleChooser.Initialize(subtitleList);
                                if (_loading)
                                {
                                    subtitleChooser.Icon = (Icon)this.Icon.Clone();
                                    subtitleChooser.ShowInTaskbar = true;
                                    subtitleChooser.ShowIcon = true;
                                }

                                if (subtitleChooser.ShowDialog(this) == DialogResult.OK)
                                {
                                    if (LoadMatroskaSubtitle(subtitleList[subtitleChooser.SelectedIndex], matroska, false) &&
                                        (Path.GetExtension(matroska.Path).Equals(".mkv", StringComparison.OrdinalIgnoreCase) ||
                                         Path.GetExtension(matroska.Path).Equals(".mks", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        if (!Configuration.Settings.General.DisableVideoAutoLoading)
                                        {
                                            matroska.Dispose();
                                            OpenVideo(matroska.Path);
                                        }
                                    }
                                    else
                                    {
                                        _exitWhenLoaded = _loading;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var ext = Path.GetExtension(matroska.Path).ToLowerInvariant();
                            if (LoadMatroskaSubtitle(subtitleList[0], matroska, false) &&
                                (ext == ".mkv" || ext == ".mks"))
                            {
                                if (!Configuration.Settings.General.DisableVideoAutoLoading)
                                {
                                    matroska.Dispose();
                                    if (ext == ".mkv")
                                    {
                                        OpenVideo(matroska.Path);
                                    }
                                    else
                                    {
                                        TryToFindAndOpenVideoFile(Path.Combine(Path.GetDirectoryName(matroska.Path), Path.GetFileNameWithoutExtension(matroska.Path)));
                                    }
                                }
                            }
                            else
                            {
                                _exitWhenLoaded = _loading;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(_language.NotAValidMatroskaFileX, fileName));
                }
            }
        }

        private int _lastProgressPercent = -1;

        private void UpdateProgress(long position, long total, string statusMessage)
        {
            var percent = (int)Math.Round(position * 100.0 / total);
            if (percent == _lastProgressPercent)
            {
                return;
            }

            ShowStatus(string.Format("{0}, {1:0}%", statusMessage, _lastProgressPercent));
            statusStrip1.Refresh();
            TaskbarList.SetProgressValue(Handle, percent, 100);
            if (DateTime.UtcNow.Ticks % 10 == 0)
            {
                Application.DoEvents();
            }

            _lastProgressPercent = percent;
        }

        private void MatroskaProgress(long position, long total)
        {
            UpdateProgress(position, total, _language.ParsingMatroskaFile);
        }

        private bool LoadMatroskaSubtitle(MatroskaTrackInfo matroskaSubtitleInfo, MatroskaFile matroska, bool batchMode)
        {
            if (matroskaSubtitleInfo.CodecId.Equals("S_VOBSUB", StringComparison.OrdinalIgnoreCase))
            {
                if (batchMode)
                {
                    return false;
                }

                return LoadVobSubFromMatroska(matroskaSubtitleInfo, matroska);
            }

            if (matroskaSubtitleInfo.CodecId.Equals("S_HDMV/PGS", StringComparison.OrdinalIgnoreCase))
            {
                if (batchMode)
                {
                    return false;
                }

                return LoadBluRaySubFromMatroska(matroskaSubtitleInfo, matroska);
            }

            if (matroskaSubtitleInfo.CodecId.Equals("S_HDMV/TEXTST", StringComparison.OrdinalIgnoreCase))
            {
                if (batchMode)
                {
                    return false;
                }

                return LoadTextSTFromMatroska(matroskaSubtitleInfo, matroska, batchMode);
            }

            if (matroskaSubtitleInfo.CodecId.Equals("S_DVBSUB", StringComparison.OrdinalIgnoreCase))
            {
                if (batchMode)
                {
                    return false;
                }

                return LoadDvbFromMatroska(matroskaSubtitleInfo, matroska, batchMode);
            }

            ShowStatus(_language.ParsingMatroskaFile);
            Refresh();
            Cursor.Current = Cursors.WaitCursor;
            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, MatroskaProgress);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);
            Cursor.Current = Cursors.Default;

            MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
            _subtitleListViewIndex = -1;
            if (!batchMode)
            {
                ResetSubtitle();
            }

            _subtitle.Paragraphs.Clear();

            var format = Utilities.LoadMatroskaTextSubtitle(matroskaSubtitleInfo, matroska, sub, _subtitle);

            if (matroskaSubtitleInfo.GetCodecPrivate().Contains("[script info]", StringComparison.OrdinalIgnoreCase))
            {
                if (_networkSession == null)
                {
                    SubtitleListview1.ShowExtraColumn(_languageGeneral.Style);
                }
            }
            else if (_networkSession == null)
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);
            }

            SetCurrentFormat(format);
            _oldSubtitleFormat = format;
            ShowStatus(_language.SubtitleImportedFromMatroskaFile);
            _subtitle.Renumber();
            if (matroska.Path.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) || matroska.Path.EndsWith(".mks", StringComparison.OrdinalIgnoreCase))
            {
                _fileName = matroska.Path.Remove(matroska.Path.Length - 4) + format.Extension;
            }

            SetTitle();
            _fileDateTime = new DateTime();
            _converted = true;

            if (batchMode)
            {
                return true;
            }

            UpdateSourceView();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            _subtitleListViewIndex = -1;
            SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
            return true;
        }

        private bool LoadTextSTFromMatroska(MatroskaTrackInfo matroskaSubtitleInfo, MatroskaFile matroska, bool batchMode)
        {
            ShowStatus(_language.ParsingMatroskaFile);
            Refresh();
            Cursor.Current = Cursors.WaitCursor;
            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, MatroskaProgress);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);
            Cursor.Current = Cursors.Default;

            MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
            _subtitleListViewIndex = -1;
            if (!batchMode)
            {
                ResetSubtitle();
            }

            _subtitle.Paragraphs.Clear();

            Utilities.LoadMatroskaTextSubtitle(matroskaSubtitleInfo, matroska, sub, _subtitle);
            Utilities.ParseMatroskaTextSt(matroskaSubtitleInfo, sub, _subtitle);

            if (_networkSession == null)
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Extra);
            }

            SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
            ShowStatus(_language.SubtitleImportedFromMatroskaFile);
            _subtitle.Renumber();
            if (matroska.Path.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) || matroska.Path.EndsWith(".mks", StringComparison.OrdinalIgnoreCase))
            {
                _fileName = matroska.Path.Remove(matroska.Path.Length - 4) + GetCurrentSubtitleFormat().Extension;
            }

            SetTitle();
            _fileDateTime = new DateTime();
            _converted = true;
            if (batchMode)
            {
                return true;
            }

            UpdateSourceView();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            _subtitleListViewIndex = -1;
            SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
            return true;
        }

        private bool LoadDvbFromMatroska(MatroskaTrackInfo matroskaSubtitleInfo, MatroskaFile matroska, bool batchMode)
        {
            ShowStatus(_language.ParsingMatroskaFile);
            Refresh();
            Cursor.Current = Cursors.WaitCursor;
            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, MatroskaProgress);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);
            Cursor.Current = Cursors.Default;

            MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
            _subtitleListViewIndex = -1;
            if (!batchMode)
            {
                ResetSubtitle();
            }

            _subtitle.Paragraphs.Clear();
            var subtitleImages = new List<DvbSubPes>();
            var subtitle = new Subtitle();
            Utilities.LoadMatroskaTextSubtitle(matroskaSubtitleInfo, matroska, sub, _subtitle);
            for (int index = 0; index < sub.Count; index++)
            {
                try
                {
                    var msub = sub[index];
                    DvbSubPes pes = null;
                    var data = msub.GetData(matroskaSubtitleInfo);
                    if (data != null && data.Length > 9 && data[0] == 15 && data[1] >= SubtitleSegment.PageCompositionSegment && data[1] <= SubtitleSegment.DisplayDefinitionSegment) // sync byte + segment id
                    {
                        var buffer = new byte[data.Length + 3];
                        Buffer.BlockCopy(data, 0, buffer, 2, data.Length);
                        buffer[0] = 32;
                        buffer[1] = 0;
                        buffer[buffer.Length - 1] = 255;
                        pes = new DvbSubPes(0, buffer);
                    }
                    else if (VobSubParser.IsMpeg2PackHeader(data))
                    {
                        pes = new DvbSubPes(data, Mpeg2Header.Length);
                    }
                    else if (VobSubParser.IsPrivateStream1(data, 0))
                    {
                        pes = new DvbSubPes(data, 0);
                    }
                    else if (data.Length > 9 && data[0] == 32 && data[1] == 0 && data[2] == 14 && data[3] == 16)
                    {
                        pes = new DvbSubPes(0, data);
                    }

                    if (pes == null && subtitle.Paragraphs.Count > 0)
                    {
                        var last = subtitle.Paragraphs[subtitle.Paragraphs.Count - 1];
                        if (last.DurationTotalMilliseconds < 100)
                        {
                            last.EndTime.TotalMilliseconds = msub.Start;
                            if (last.DurationTotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                            {
                                last.EndTime.TotalMilliseconds = last.StartTime.TotalMilliseconds + 3000;
                            }
                        }
                    }

                    if (pes != null && pes.PageCompositions != null && pes.PageCompositions.Any(p => p.Regions.Count > 0))
                    {
                        subtitleImages.Add(pes);
                        subtitle.Paragraphs.Add(new Paragraph(string.Empty, msub.Start, msub.End));
                    }
                }
                catch
                {
                    // continue
                }
            }

            if (subtitleImages.Count == 0)
            {
                return false;
            }

            for (int index = 0; index < subtitle.Paragraphs.Count; index++)
            {
                var p = subtitle.Paragraphs[index];
                if (p.DurationTotalMilliseconds < 200)
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + 3000;
                }

                var next = subtitle.GetParagraphOrDefault(index + 1);
                if (next != null && next.StartTime.TotalMilliseconds < p.EndTime.TotalMilliseconds)
                {
                    p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                }
            }

            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.Initialize(subtitle, subtitleImages, Configuration.Settings.VobSubOcr); // TODO: language???
                if (_loading)
                {
                    formSubOcr.Icon = (Icon)Icon.Clone();
                    formSubOcr.ShowInTaskbar = true;
                    formSubOcr.ShowIcon = true;
                }

                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    ResetSubtitle();
                    _subtitle.Paragraphs.Clear();
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    RefreshSelectedParagraph();
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(matroska.Path) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();

                    Configuration.Settings.Save();
                    return true;
                }
            }

            return false;
        }
        private bool LoadVobSubFromMatroska(MatroskaTrackInfo matroskaSubtitleInfo, MatroskaFile matroska)
        {
            if (matroskaSubtitleInfo.ContentEncodingType == 1)
            {
                MessageBox.Show(_language.NoSupportEncryptedVobSub);
            }

            ShowStatus(_language.ParsingMatroskaFile);
            Refresh();
            Cursor.Current = Cursors.WaitCursor;
            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, MatroskaProgress);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);
            Cursor.Current = Cursors.Default;

            MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
            _subtitleListViewIndex = -1;
            _subtitle.Paragraphs.Clear();

            List<VobSubMergedPack> mergedVobSubPacks = new List<VobSubMergedPack>();
            var idx = new Core.VobSub.Idx(matroskaSubtitleInfo.GetCodecPrivate().SplitToLines());
            foreach (var p in sub)
            {
                mergedVobSubPacks.Add(new VobSubMergedPack(p.GetData(matroskaSubtitleInfo), TimeSpan.FromMilliseconds(p.Start), 32, null));
                if (mergedVobSubPacks.Count > 0)
                {
                    mergedVobSubPacks[mergedVobSubPacks.Count - 1].EndTime = TimeSpan.FromMilliseconds(p.End);
                }

                // fix overlapping (some versions of Handbrake makes overlapping time codes - thx Hawke)
                if (mergedVobSubPacks.Count > 1 && mergedVobSubPacks[mergedVobSubPacks.Count - 2].EndTime > mergedVobSubPacks[mergedVobSubPacks.Count - 1].StartTime)
                {
                    mergedVobSubPacks[mergedVobSubPacks.Count - 2].EndTime = TimeSpan.FromMilliseconds(mergedVobSubPacks[mergedVobSubPacks.Count - 1].StartTime.TotalMilliseconds - 1);
                }
            }

            // Remove bad packs
            for (int i = mergedVobSubPacks.Count - 1; i >= 0; i--)
            {
                if (mergedVobSubPacks[i].SubPicture.SubPictureDateSize <= 2)
                {
                    mergedVobSubPacks.RemoveAt(i);
                }
                else if (mergedVobSubPacks[i].SubPicture.SubPictureDateSize <= 67 && mergedVobSubPacks[i].SubPicture.Delay.TotalMilliseconds < 35)
                {
                    mergedVobSubPacks.RemoveAt(i);
                }
            }

            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.Initialize(mergedVobSubPacks, idx.Palette, Configuration.Settings.VobSubOcr, null); // TODO: language???
                if (_loading)
                {
                    formSubOcr.Icon = (Icon)Icon.Clone();
                    formSubOcr.ShowInTaskbar = true;
                    formSubOcr.ShowIcon = true;
                }

                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    ResetSubtitle();
                    _subtitle.Paragraphs.Clear();
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    RefreshSelectedParagraph();
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(matroska.Path) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();

                    Configuration.Settings.Save();
                    return true;
                }
            }

            return false;
        }

        private bool LoadBluRaySubFromMatroska(MatroskaTrackInfo matroskaSubtitleInfo, MatroskaFile matroska)
        {
            if (matroskaSubtitleInfo.ContentEncodingType == 1)
            {
                MessageBox.Show(_language.NoSupportEncryptedVobSub);
            }

            ShowStatus(_language.ParsingMatroskaFile);
            Refresh();
            Cursor.Current = Cursors.WaitCursor;
            var sub = matroska.GetSubtitle(matroskaSubtitleInfo.TrackNumber, MatroskaProgress);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);
            Cursor.Current = Cursors.Default;

            int noOfErrors = 0;
            string lastError = string.Empty;
            MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
            _subtitleListViewIndex = -1;
            _subtitle.Paragraphs.Clear();
            var subtitles = new List<BluRaySupParser.PcsData>();
            var log = new StringBuilder();
            var clusterStream = new MemoryStream();
            var lastPalettes = new Dictionary<int, List<PaletteInfo>>();
            var lastBitmapObjects = new Dictionary<int, List<BluRaySupParser.OdsData>>();
            foreach (var p in sub)
            {
                byte[] buffer = p.GetData(matroskaSubtitleInfo);
                if (buffer != null && buffer.Length > 2)
                {
                    clusterStream.Write(buffer, 0, buffer.Length);
                    if (ContainsBluRayStartSegment(buffer))
                    {
                        if (subtitles.Count > 0 && subtitles[subtitles.Count - 1].StartTime == subtitles[subtitles.Count - 1].EndTime)
                        {
                            subtitles[subtitles.Count - 1].EndTime = (long)((p.Start - 1) * 90.0);
                        }

                        clusterStream.Position = 0;
                        var list = BluRaySupParser.ParseBluRaySup(clusterStream, log, true, lastPalettes, lastBitmapObjects);
                        foreach (var sup in list)
                        {
                            sup.StartTime = (long)((p.Start - 1) * 90.0);
                            sup.EndTime = (long)((p.End - 1) * 90.0);
                            subtitles.Add(sup);

                            // fix overlapping
                            if (subtitles.Count > 1 && sub[subtitles.Count - 2].End > sub[subtitles.Count - 1].Start)
                            {
                                subtitles[subtitles.Count - 2].EndTime = subtitles[subtitles.Count - 1].StartTime - 1;
                            }
                        }

                        clusterStream = new MemoryStream();
                    }
                }
                else if (subtitles.Count > 0)
                {
                    var lastSub = subtitles[subtitles.Count - 1];
                    if (lastSub.StartTime == lastSub.EndTime)
                    {
                        lastSub.EndTime = (long)((p.Start - 1) * 90.0);
                        if (lastSub.EndTime - lastSub.StartTime > 1000000)
                        {
                            lastSub.EndTime = lastSub.StartTime;
                        }
                    }
                }
            }

            if (noOfErrors > 0)
            {
                MessageBox.Show(string.Format("{0} error(s) occurred during extraction of bdsup\r\n\r\n{1}", noOfErrors, lastError));
            }

            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, matroska.Path);
                if (_loading)
                {
                    formSubOcr.Icon = (Icon)Icon.Clone();
                    formSubOcr.ShowInTaskbar = true;
                    formSubOcr.ShowIcon = true;
                }

                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingDvdSubtitle);

                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    RefreshSelectedParagraph();

                    _fileName = string.Empty;
                    if (!string.IsNullOrEmpty(matroska.Path))
                    {
                        _fileName = Path.GetFileNameWithoutExtension(matroska.Path) + GetCurrentSubtitleFormat().Extension;
                    }

                    _converted = true;
                    SetTitle();

                    Configuration.Settings.Save();
                    return true;
                }
            }

            return false;
        }

        private bool ContainsBluRayStartSegment(byte[] buffer)
        {
            const int epochStart = 0x80;
            var position = 0;
            while (position + 3 <= buffer.Length)
            {
                var segmentType = buffer[position];
                if (segmentType == epochStart)
                {
                    return true;
                }

                int length = BluRaySupParser.BigEndianInt16(buffer, position + 1) + 3;
                position += length;
            }

            return false;
        }

        private void ImportSubtitleFromDvbSupFile(string fileName)
        {
            using (var formSubOcr = new VobSubOcr())
            {
                string language = null;
                var programMapTableParser = new ProgramMapTableParser();
                programMapTableParser.Parse(fileName); // get languages
                if (programMapTableParser.GetSubtitlePacketIds().Count > 0)
                {
                    language = programMapTableParser.GetSubtitleLanguageTwoLetter(programMapTableParser.GetSubtitlePacketIds().First());
                }

                var subtitles = TransportStreamParser.GetDvbSup(fileName);
                formSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName, language);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingDvdSubtitle);

                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = string.Empty;
                    SetTitle();

                    Configuration.Settings.Save();
                }
            }
        }

        private bool ImportSubtitleFromTransportStream(string fileName)
        {
            ShowStatus(_language.ParsingTransportStream);
            Refresh();
            var tsParser = new TransportStreamParser();
            tsParser.Parse(fileName, (pos, total) => UpdateProgress(pos, total, _language.ParsingTransportStreamFile));
            ShowStatus(string.Empty);
            TaskbarList.SetProgressState(Handle, TaskbarButtonProgressFlags.NoProgress);

            if (tsParser.SubtitlePacketIds.Count == 0 && tsParser.TeletextSubtitlesLookup.Count == 0)
            {
                MessageBox.Show(_language.NoSubtitlesFound);
                _exitWhenLoaded = _loading;
                return false;
            }

            if (tsParser.SubtitlePacketIds.Count == 0 && tsParser.TeletextSubtitlesLookup.Count == 1 && tsParser.TeletextSubtitlesLookup.First().Value.Count == 1)
            {
                _subtitle = new Subtitle(tsParser.TeletextSubtitlesLookup.First().Value.First().Value);
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle);
                SubtitleListview1.SelectIndexAndEnsureVisible(0);
                if (!Configuration.Settings.General.DisableVideoAutoLoading)
                {
                    OpenVideo(fileName);
                }

                _fileName = Path.GetFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                _converted = true;
                SetTitle();
                return true;
            }

            int packetId;
            if (tsParser.SubtitlePacketIds.Count + tsParser.TeletextSubtitlesLookup.Sum(p => p.Value.Count) > 1)
            {
                using (var subChooser = new TransportStreamSubtitleChooser())
                {
                    subChooser.Initialize(tsParser, fileName);
                    if (subChooser.ShowDialog(this) == DialogResult.Cancel)
                    {
                        return false;
                    }

                    if (subChooser.IsTeletext)
                    {
                        new SubRip().LoadSubtitle(_subtitle, subChooser.Srt.SplitToLines(), null);
                        _subtitle.Renumber();
                        SubtitleListview1.Fill(_subtitle);
                        SubtitleListview1.SelectIndexAndEnsureVisible(0);
                        if (!Configuration.Settings.General.DisableVideoAutoLoading)
                        {
                            OpenVideo(fileName);
                        }

                        _fileName = Path.GetFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                        _converted = true;
                        SetTitle();
                        return true;
                    }

                    packetId = tsParser.SubtitlePacketIds[subChooser.SelectedIndex];
                }
            }
            else
            {
                packetId = tsParser.SubtitlePacketIds[0];
            }


            var subtitles = tsParser.GetDvbSubtitles(packetId);
            using (var formSubOcr = new VobSubOcr())
            {
                string language = null;
                var programMapTableParser = new ProgramMapTableParser();
                programMapTableParser.Parse(fileName); // get languages
                if (programMapTableParser.GetSubtitlePacketIds().Count > 0)
                {
                    language = programMapTableParser.GetSubtitleLanguage(packetId);
                }

                formSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName, language);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingDvdSubtitle);

                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = string.Empty;
                    if (!string.IsNullOrEmpty(formSubOcr.FileName))
                    {
                        var currentFormat = GetCurrentSubtitleFormat();
                        _fileName = Utilities.GetPathAndFileNameWithoutExtension(formSubOcr.FileName) + currentFormat.Extension;
                        if (!Configuration.Settings.General.DisableVideoAutoLoading)
                        {
                            OpenVideo(fileName);
                        }

                        _converted = true;
                    }

                    SetTitle();
                    Configuration.Settings.Save();
                    return true;
                }
            }

            _exitWhenLoaded = _loading;
            return false;
        }

        private bool ImportSubtitleFromManzanitaTransportStream(string fileName, List<TransportStreamSubtitle> subtitles)
        {
            ShowStatus(_language.ParsingTransportStream);
            Refresh();

            using (var formSubOcr = new VobSubOcr())
            {
                string language = null;
                formSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName, language);
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingDvdSubtitle);

                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = string.Empty;
                    if (!string.IsNullOrEmpty(formSubOcr.FileName))
                    {
                        var currentFormat = GetCurrentSubtitleFormat();
                        _fileName = Utilities.GetPathAndFileNameWithoutExtension(formSubOcr.FileName) + currentFormat.Extension;
                        if (!Configuration.Settings.General.DisableVideoAutoLoading)
                        {
                            OpenVideo(fileName);
                        }

                        _converted = true;
                    }

                    SetTitle();
                    Configuration.Settings.Save();
                    return true;
                }
            }

            _exitWhenLoaded = _loading;
            return false;
        }

        private bool ImportSubtitleFromMp4(string fileName)
        {
            var mp4Parser = new MP4Parser(fileName);
            var mp4SubtitleTracks = mp4Parser.GetSubtitleTracks();
            if (mp4SubtitleTracks.Count == 0)
            {
                if (mp4Parser.VttcSubtitle?.Paragraphs.Count > 0)
                {
                    MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                    _subtitleListViewIndex = -1;
                    FileNew();
                    _subtitle = mp4Parser.VttcSubtitle;
                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();
                    return true;
                }

                if (mp4Parser.TrunCea608Subtitle?.Paragraphs.Count > 0)
                {
                    MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                    _subtitleListViewIndex = -1;
                    FileNew();
                    _subtitle = mp4Parser.TrunCea608Subtitle;
                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();
                    return true;
                }

                MessageBox.Show(_language.NoSubtitlesFound);
                return false;
            }
            else if (mp4SubtitleTracks.Count == 1)
            {
                LoadMp4Subtitle(fileName, mp4SubtitleTracks[0]);
                return true;
            }
            else
            {
                using (var subtitleChooser = new MatroskaSubtitleChooser("mp4"))
                {
                    subtitleChooser.Initialize(mp4SubtitleTracks);
                    if (subtitleChooser.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadMp4Subtitle(fileName, mp4SubtitleTracks[subtitleChooser.SelectedIndex]);
                        return true;
                    }
                }

                return false;
            }
        }

        private bool ImportSubtitleFromDivX(string fileName)
        {
            var count = 0;
            var list = new List<XSub>();
            using (var f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var searchBuffer = new byte[2048];
                long pos = 0;
                long length = f.Length - 50;
                while (pos < length)
                {
                    f.Position = pos;
                    int readCount = f.Read(searchBuffer, 0, searchBuffer.Length);
                    for (int i = 0; i < readCount; i++)
                    {
                        if (searchBuffer[i] != 0x5b || (i + 4 < readCount && (searchBuffer[i + 1] < 0x30 || searchBuffer[i + 1] > 0x39 || searchBuffer[i + 3] != 0x3a)))
                        {
                            continue;
                        }

                        f.Position = pos + i + 1;

                        var buffer = new byte[26];
                        f.Read(buffer, 0, buffer.Length);

                        if (buffer[2] == 0x3a && // :
                            buffer[5] == 0x3a && // :
                            buffer[8] == 0x2e && // .
                            buffer[12] == 0x2d && // -
                            buffer[15] == 0x3a && // :
                            buffer[18] == 0x3a && // :
                            buffer[21] == 0x2e && // .
                            buffer[25] == 0x5d) // ]
                        { // subtitle time code
                            string timeCode = Encoding.ASCII.GetString(buffer, 0, 25);

                            f.Read(buffer, 0, 2);
                            int width = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int height = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int x = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int y = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int xEnd = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int yEnd = BitConverter.ToUInt16(buffer, 0);
                            f.Read(buffer, 0, 2);
                            int RleLength = BitConverter.ToUInt16(buffer, 0);

                            var colorBuffer = new byte[4 * 3]; // four colors with rgb (3 bytes)
                            f.Read(colorBuffer, 0, colorBuffer.Length);

                            buffer = new byte[RleLength];
                            int bytesRead = f.Read(buffer, 0, buffer.Length);

                            if (width > 0 && height > 0 && bytesRead == buffer.Length)
                            {
                                var xSub = new XSub(timeCode, width, height, colorBuffer, buffer);
                                list.Add(xSub);
                                count++;
                            }
                        }
                    }

                    pos += searchBuffer.Length;
                }
            }

            if (count == 0)
            {
                return false;
            }

            using (var formSubOcr = new VobSubOcr())
            {
                formSubOcr.Initialize(list, Configuration.Settings.VobSubOcr, fileName); // TODO: language???
                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                    _subtitleListViewIndex = -1;
                    FileNew();
                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();

                    Configuration.Settings.Save();
                    if (!Configuration.Settings.General.DisableVideoAutoLoading)
                    {
                        OpenVideo(fileName);
                    }
                }
            }

            return true;
        }


        private void LoadMp4Subtitle(string fileName, Trak mp4SubtitleTrack)
        {
            if (mp4SubtitleTrack.Mdia.IsVobSubSubtitle)
            {
                var subPicturesWithTimeCodes = new List<VobSubOcr.SubPicturesWithSeparateTimeCodes>();
                var paragraphs = mp4SubtitleTrack.Mdia.Minf.Stbl.GetParagraphs();
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    if (mp4SubtitleTrack.Mdia.Minf.Stbl.SubPictures.Count > i)
                    {
                        var start = paragraphs[i].StartTime.TimeSpan;
                        var end = paragraphs[i].EndTime.TimeSpan;
                        subPicturesWithTimeCodes.Add(new VobSubOcr.SubPicturesWithSeparateTimeCodes(mp4SubtitleTrack.Mdia.Minf.Stbl.SubPictures[i], start, end));
                    }
                }

                using (var formSubOcr = new VobSubOcr())
                {
                    formSubOcr.Initialize(subPicturesWithTimeCodes, Configuration.Settings.VobSubOcr, fileName); // TODO: language???
                    if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                    {
                        MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                        _subtitleListViewIndex = -1;
                        FileNew();
                        foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                        {
                            _subtitle.Paragraphs.Add(p);
                        }

                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        _subtitleListViewIndex = -1;
                        SubtitleListview1.FirstVisibleIndex = -1;
                        SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                        _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                        _converted = true;
                        SetTitle();

                        Configuration.Settings.Save();
                    }
                }
            }
            else
            {
                MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                _subtitleListViewIndex = -1;
                FileNew();

                _subtitle.Paragraphs.AddRange(mp4SubtitleTrack.Mdia.Minf.Stbl.GetParagraphs());

                
                ShowStatus(_language.SubtitleImportedFromMatroskaFile);
                _subtitle.Renumber();
                if (fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                    fileName.EndsWith(".m4v", StringComparison.OrdinalIgnoreCase))
                {
                    _fileName = fileName.Substring(0, fileName.Length - 4) + GetCurrentSubtitleFormat().Extension;
                }

                SetTitle();
                _fileDateTime = new DateTime();
                _converted = true;
                UpdateSourceView();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
            }
        }

        private void SubtitleListview1_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void SubtitleListview1_DragDrop(object sender, DragEventArgs e)
        {
            _dragAndDropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (_dragAndDropFiles.Length == 1)
            {
                _dragAndDropTimer.Start();
            }
            else
            {
                MessageBox.Show(_language.DropOnlyOneFile);
            }
        }

        private void DoSubtitleListview1Drop(object sender, EventArgs e)
        {
            _dragAndDropTimer.Stop();

            if (!ContinueNewOrExit())
            {
                return;
            }

            Interlocked.Increment(ref _openSaveCounter);

            string fileName = _dragAndDropFiles[0];
            var file = new FileInfo(fileName);

            // Do not allow directory drop
            if (FileUtil.IsDirectory(fileName))
            {
                MessageBox.Show(_language.ErrorDirectoryDropNotAllowed, file.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dirName = Path.GetDirectoryName(fileName);
            saveFileDialog1.InitialDirectory = dirName;
            openFileDialog1.InitialDirectory = dirName;
            var ext = file.Extension.ToLowerInvariant();

            if (ext == ".mkv" || ext == ".mks")
            {
                using (var matroska = new MatroskaFile(fileName))
                {
                    if (matroska.IsValid)
                    {
                        var subtitleList = matroska.GetTracks(true);
                        if (subtitleList.Count == 0)
                        {
                            MessageBox.Show(_language.NoSubtitlesFound);
                        }
                        else if (subtitleList.Count > 1)
                        {
                            using (var subtitleChooser = new MatroskaSubtitleChooser("mkv"))
                            {
                                subtitleChooser.Initialize(subtitleList);
                                if (subtitleChooser.ShowDialog(this) == DialogResult.OK)
                                {
                                    ResetSubtitle();
                                    if (LoadMatroskaSubtitle(subtitleList[subtitleChooser.SelectedIndex], matroska, false) &&
                                        (ext.Equals(".mkv", StringComparison.Ordinal) || ext.Equals(".mks", StringComparison.Ordinal)) &&
                                        !Configuration.Settings.General.DisableVideoAutoLoading)
                                    {
                                        OpenVideo(fileName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ResetSubtitle();
                            if (LoadMatroskaSubtitle(subtitleList[0], matroska, false) &&
                                (ext.Equals(".mkv", StringComparison.Ordinal) || ext.Equals(".mks", StringComparison.Ordinal)) &&
                                !Configuration.Settings.General.DisableVideoAutoLoading)
                            {
                                OpenVideo(fileName);
                            }
                        }

                        return;
                    }
                }
            }

            if (ext == ".ismt" || ext == ".mp4" || ext == ".m4v" || ext == ".mov" || ext == ".3gp" || ext == ".cmaf" || ext == ".m4s")
            {
                var mp4Parser = new MP4Parser(fileName);
                var mp4SubtitleTracks = mp4Parser.GetSubtitleTracks();
                if (mp4SubtitleTracks.Count > 0)
                {
                    ImportSubtitleFromMp4(fileName);
                    if (_subtitle.Paragraphs.Count > 0)
                    {
                        return;
                    }
                }

                var f = new IsmtDfxp();
                if (f.IsMine(null, fileName))
                {
                    f.LoadSubtitle(_subtitle, null, fileName);
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                    SetTitle();
                    ShowStatus(string.Format(_language.LoadedSubtitleX, _fileName));
                    _sourceViewChange = false;
                    _changeSubtitleHash = GetFastSubtitleHash();
                    ResetHistory();
                    SetUndockedWindowsTitle();
                    _converted = true;
                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    return;
                }

                if (mp4Parser.VttcSubtitle?.Paragraphs.Count > 0)
                {
                    MakeHistoryForUndo(_language.BeforeImportFromMatroskaFile);
                    _subtitleListViewIndex = -1;
                    FileNew();
                    _subtitle = mp4Parser.VttcSubtitle;
                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    _fileName = Utilities.GetPathAndFileNameWithoutExtension(fileName) + GetCurrentSubtitleFormat().Extension;
                    _converted = true;
                    SetTitle();
                    return;
                }

                MessageBox.Show(_language.NoSubtitlesFound);
                return;
            }
            else if (ext == ".vob" || ext == ".ifo")
            {
                ImportDvdSubtitle(fileName);
                return;
            }
            else if (ext == ".idx")
            {
                var subFileName = fileName.Substring(0, fileName.Length - 3) + "sub";
                if (File.Exists(subFileName) && FileUtil.IsVobSub(subFileName))
                {
                    ImportAndOcrVobSubSubtitleNew(subFileName, _loading);
                    return;
                }
            }

            comboBoxEncoding.BeginUpdate();
            comboBoxSubtitleFormats.BeginUpdate();

            if (file.Length < Subtitle.MaxFileSize)
            {
                if (!OpenFromRecentFiles(fileName))
                {
                    OpenSubtitle(fileName, null);
                }
            }
            else if (file.Length < 150000000 && ext == ".sub" && IsVobSubFile(fileName, true)) // max 150 mb
            {
                OpenSubtitle(fileName, null);
            }
            else if (file.Length < 250000000 && ext == ".sup" && FileUtil.IsBluRaySup(fileName)) // max 250 mb
            {
                OpenSubtitle(fileName, null);
            }
            else if ((ext == ".ts" || ext == ".tsv" || ext == ".tts" || ext == ".rec" || ext == ".mpg" || ext == ".mpeg") && FileUtil.IsTransportStream(fileName))
            {
                OpenSubtitle(fileName, null);
            }
            else if ((ext == ".m2ts" || ext == ".ts" || ext == ".tts" || ext == ".mts") && FileUtil.IsM2TransportStream(fileName))
            {
                OpenSubtitle(fileName, null);
            }
            else if (ext == ".divx" || ext == ".avi")
            {
                OpenSubtitle(fileName, null);
            }
            else
            {
                MessageBox.Show(string.Format(_language.DropSubtitleFileXNotAccepted, fileName));
            }

            comboBoxSubtitleFormats.EndUpdate();
            comboBoxEncoding.EndUpdate();

            Interlocked.Decrement(ref _openSaveCounter);
        }

      
        private void ChangeCasing(bool onlySelectedLines)
        {
            if (!IsSubtitleLoaded)
            {
                DisplaySubtitleNotLoadedMessage();
                return;
            }

            SaveSubtitleListviewIndices();
            using (var changeCasing = new ChangeCasing())
            {
                if (onlySelectedLines)
                {
                    changeCasing.Text += " - " + _language.SelectedLines;
                }

                ReloadFromSourceView();
                if (changeCasing.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeChangeCasing);

                    Cursor.Current = Cursors.WaitCursor;
                    var selectedLines = new Subtitle();
                    var selectedIndices = new List<int>(SubtitleListview1.GetSelectedIndices());
                    if (onlySelectedLines)
                    {
                        foreach (int index in selectedIndices)
                        {
                            selectedLines.Paragraphs.Add(new Paragraph(_subtitle.Paragraphs[index]));
                        }
                    }
                    else
                    {
                        foreach (var p in _subtitle.Paragraphs)
                        {
                            selectedLines.Paragraphs.Add(new Paragraph(p));
                        }
                    }

                    bool saveChangeCaseChanges = true;
                    var casingNamesLinesChanged = 0;

                    if (changeCasing.ChangeNamesToo && changeCasing.OnlyAllUpper)
                    {
                        selectedIndices = new List<int>();
                        var allUpperSubtitle = new Subtitle();
                        var sub = onlySelectedLines ? selectedLines : _subtitle;
                        for (var index = 0; index < sub.Paragraphs.Count; index++)
                        {
                            var p = sub.Paragraphs[index];
                            var noTags = HtmlUtil.RemoveHtmlTags(p.Text, true);
                            if (noTags == noTags.ToUpperInvariant())
                            {
                                allUpperSubtitle.Paragraphs.Add(p);
                                selectedIndices.Add(index);
                            }
                        }

                        selectedLines = allUpperSubtitle;
                        onlySelectedLines = true;
                    }

                    changeCasing.FixCasing(selectedLines, LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle));
                    if (changeCasing.ChangeNamesToo)
                    {
                        using (var changeCasingNames = new ChangeCasingNames())
                        {
                            changeCasingNames.Initialize(selectedLines);
                            if (changeCasingNames.ShowDialog(this) == DialogResult.OK)
                            {
                                changeCasingNames.FixCasing();
                                casingNamesLinesChanged = changeCasingNames.LinesChanged;

                                if (changeCasing.LinesChanged == 0)
                                {
                                    ShowStatus(string.Format(_language.CasingCompleteMessageOnlyNames, casingNamesLinesChanged, _subtitle.Paragraphs.Count));
                                }
                                else
                                {
                                    ShowStatus(string.Format(_language.CasingCompleteMessage, changeCasing.LinesChanged, _subtitle.Paragraphs.Count, casingNamesLinesChanged));
                                }
                            }
                            else
                            {
                                saveChangeCaseChanges = false;
                            }
                        }
                    }
                    else
                    {
                        ShowStatus(string.Format(_language.CasingCompleteMessageNoNames, changeCasing.LinesChanged, _subtitle.Paragraphs.Count));
                    }

                    if (saveChangeCaseChanges)
                    {
                        if (onlySelectedLines)
                        {
                            int i = 0;
                            foreach (int index in selectedIndices)
                            {
                                _subtitle.Paragraphs[index].Text = selectedLines.Paragraphs[i].Text;
                                i++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
                            {
                                _subtitle.Paragraphs[i].Text = selectedLines.Paragraphs[i].Text;
                            }
                        }

                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        if (changeCasing.LinesChanged > 0 || casingNamesLinesChanged > 0)
                        {
                            _subtitleListViewIndex = -1;
                            RestoreSubtitleListviewIndices();
                            UpdateSourceView();
                        }
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }


        private bool IsVobSubFile(string subFileName, bool verbose)
        {
            try
            {
                if (FileUtil.IsVobSub(subFileName))
                {
                    if (!verbose)
                    {
                        return true;
                    }

                    var idxFileName = Utilities.GetPathAndFileNameWithoutExtension(subFileName) + ".idx";
                    if (File.Exists(idxFileName))
                    {
                        return true;
                    }

                    var dr = MessageBox.Show(string.Format(_language.IdxFileNotFoundWarning, idxFileName), _title, MessageBoxButtons.YesNoCancel);
                    return dr == DialogResult.Yes;
                }

                if (verbose)
                {
                    MessageBox.Show(string.Format(_language.InvalidVobSubHeader, subFileName));
                }
            }
            catch (Exception ex)
            {
                if (verbose)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return false;
        }

        private void ImportAndOcrSpDvdSup(string fileName, bool showInTaskbar)
        {
            var spList = new List<SpHeader>();

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[SpHeader.SpHeaderLength];
                int bytesRead = fs.Read(buffer, 0, buffer.Length);
                var header = new SpHeader(buffer);

                while (header.Identifier == "SP" && bytesRead > 0 && header.NextBlockPosition > 4)
                {
                    buffer = new byte[header.NextBlockPosition];
                    bytesRead = fs.Read(buffer, 0, buffer.Length);
                    if (bytesRead == buffer.Length)
                    {
                        header.AddPicture(buffer);
                        spList.Add(header);
                    }

                    buffer = new byte[SpHeader.SpHeaderLength];
                    bytesRead = fs.Read(buffer, 0, buffer.Length);
                    while (bytesRead == buffer.Length && Encoding.ASCII.GetString(buffer, 0, 2) != "SP")
                    {
                        fs.Seek(fs.Position - buffer.Length + 1, SeekOrigin.Begin);
                        bytesRead = fs.Read(buffer, 0, buffer.Length);
                    }

                    header = new SpHeader(buffer);
                }
            }

            using (var vobSubOcr = new VobSubOcr())
            {
                if (showInTaskbar)
                {
                    vobSubOcr.Icon = (Icon)this.Icon.Clone();
                    vobSubOcr.ShowInTaskbar = true;
                    vobSubOcr.ShowIcon = true;
                }

                vobSubOcr.Initialize(fileName, null, Configuration.Settings.VobSubOcr, spList);
                if (vobSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingVobSubFile);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in vobSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(vobSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;

                    Configuration.Settings.Save();
                }
                else
                {
                    _exitWhenLoaded = _loading;
                }
            }
        }

        private void ImportAndOcrVobSubSubtitleNew(string fileName, bool showInTaskbar)
        {
            if (!IsVobSubFile(fileName, true))
            {
                return;
            }

            using (var vobSubOcr = new VobSubOcr())
            {
                if (showInTaskbar)
                {
                    vobSubOcr.Icon = (Icon)Icon.Clone();
                    vobSubOcr.ShowInTaskbar = true;
                    vobSubOcr.ShowIcon = true;
                }

                if (vobSubOcr.Initialize(fileName, Configuration.Settings.VobSubOcr, this) && vobSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingVobSubFile);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in vobSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.FirstVisibleIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                    _fileName = Path.ChangeExtension(vobSubOcr.FileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;

                    _imageSubFileName = vobSubOcr.FileName;

                    Configuration.Settings.Save();
                }
                else
                {
                    _exitWhenLoaded = _loading;
                }
            }
        }

        private void SaveSubtitleListviewIndices()
        {
            _selectedIndices = new List<int>();
            foreach (int index in SubtitleListview1.SelectedIndices)
            {
                _selectedIndices.Add(index);
            }
        }

        private void RestoreSubtitleListviewIndices()
        {
            _subtitleListViewIndex = -1;
            if (_selectedIndices != null)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                SubtitleListview1.SelectNone();
                int i = 0;
                foreach (int index in _selectedIndices)
                {
                    if (index >= 0 && index < SubtitleListview1.Items.Count)
                    {
                        SubtitleListview1.Items[index].Selected = true;
                        if (i == 0)
                        {
                            SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                            SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
                            SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                        }
                    }

                    i++;
                }

                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            }
        }

        internal void MainKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LWin)
            {
                _winLeftDownTicks = DateTime.UtcNow.Ticks;
            }

            if (e.KeyCode == Keys.RWin)
            {
                _winRightDownTicks = DateTime.UtcNow.Ticks;
            }

            if ((DateTime.UtcNow.Ticks - _winLeftDownTicks) <= 10000 * 999 || (DateTime.UtcNow.Ticks - _winRightDownTicks) <= 10000 * 999) // less than 999 ms
            {
                // if it's less than one second since Win key was pressed we ignore key (not perfect...)
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Modifiers == Keys.Alt && e.KeyCode == (Keys.RButton | Keys.ShiftKey))
            {
                return;
            }

            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.ShiftKey)
            {
                return;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
                return;
            }

            if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.ShiftKey)
            {
                return;
            }

            if (comboBoxSubtitleFormats.Focused && comboBoxSubtitleFormats.DroppedDown)
            {
                return;
            }

            var fc = UiUtil.FindFocusedControl(this);
            if (fc != null && (e.Modifiers == Keys.None || e.Modifiers == Keys.Shift))
            {
                var typeName = fc.GetType().Name;

                // do not check for shortcuts if text is being entered and a textbox is focused
                var textBoxTypes = new List<string> { "AdvancedTextBox", "SimpleTextBox", "SETextBox", "TextBox", "RichTextBox" };
                if (textBoxTypes.Contains(typeName) &&
                    ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) ||
                     (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.OemBackslash) ||
                      e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 ||
                      e.KeyCode == Keys.Multiply ||
                      e.KeyCode == Keys.Add ||
                      e.KeyCode == Keys.Subtract ||
                      e.KeyCode == Keys.Divide ||
                      e.KeyValue >= 48 && e.KeyValue <= 57) &&
                    !Configuration.Settings.General.AllowLetterShortcutsInTextBox)
                {
                    return;
                }

                // do not check for shortcuts if a number is being entered and a time box is focused
                if (typeName == "UpDownEdit" &&
                    (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 || e.KeyValue >= 48 && e.KeyValue <= 57))
                {
                    return;
                }
            }

            if (e.KeyCode == Keys.Escape && !_cancelWordSpellCheck)
            {
                _cancelWordSpellCheck = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformVerticalZoom)
            {
                audioVisualizer.VerticalZoomFactor *= 1.1;
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformVerticalZoomOut)
            {
                audioVisualizer.VerticalZoomFactor /= 1.1;
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformZoomIn)
            {
                audioVisualizer.ZoomIn();
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformZoomOut)
            {
                audioVisualizer.ZoomOut();
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformSplit)
            {
                var pos = mediaPlayer.CurrentPosition;
                var paragraph = _subtitle.GetFirstParagraphOrDefaultByTime(pos * TimeCode.BaseUnit);
                if (paragraph != null &&
                    pos * TimeCode.BaseUnit + 100 > paragraph.StartTime.TotalMilliseconds &&
                    pos * TimeCode.BaseUnit - 100 < paragraph.EndTime.TotalMilliseconds)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(paragraph);
                    SplitSelectedParagraph(pos, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainToggleVideoControls)
            {
                Configuration.Settings.General.ShowVideoControls = !Configuration.Settings.General.ShowVideoControls;
                ToggleVideoControlsOnOff(Configuration.Settings.General.ShowVideoControls);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoPlayFirstSelected && !string.IsNullOrEmpty(_videoFileName))
            {
                PlayFirstSelectedSubtitle();
            }
            else if (audioVisualizer.Visible && (e.KeyData == _shortcuts.WaveformPlaySelection || e.KeyData == _shortcuts.WaveformPlaySelectionEnd))
            {
                WaveformPlaySelection(nearEnd: e.KeyData == _shortcuts.WaveformPlaySelectionEnd);
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformSearchSilenceForward)
            {
                if (audioVisualizer.WavePeaks != null)
                {
                    audioVisualizer.FindDataBelowThreshold(Configuration.Settings.VideoControls.WaveformSeeksSilenceMaxVolume, Configuration.Settings.VideoControls.WaveformSeeksSilenceDurationSeconds);
                }

                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Visible && e.KeyData == _shortcuts.WaveformSearchSilenceBack)
            {
                if (audioVisualizer.WavePeaks != null)
                {
                    audioVisualizer.FindDataBelowThresholdBack(Configuration.Settings.VideoControls.WaveformSeeksSilenceMaxVolume, Configuration.Settings.VideoControls.WaveformSeeksSilenceDurationSeconds);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainInsertAfter == e.KeyData && InListView)
            {
                InsertAfter(string.Empty, true);
                e.SuppressKeyPress = true;
                textBoxListViewText.Focus();
            }
            else if (_shortcuts.MainInsertBefore == e.KeyData && InListView)
            {
                InsertBefore();
                e.SuppressKeyPress = true;
                textBoxListViewText.Focus();
            }
            else if (_shortcuts.MainMergeDialog == e.KeyData && InListView)
            {
                MergeDialogs();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainMergeDialogWithNext == e.KeyData && InListView)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx >= 0 && _subtitle.Paragraphs.Count > idx + 1)
                    {
                        SelectListViewIndexAndEnsureVisible(idx);
                        MergeWithLineAfter(true);
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainMergeDialogWithPrevious == e.KeyData && InListView)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx > 0)
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx - 1, true);
                        MergeWithLineAfter(true);
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewToggleDashes == e.KeyData && InListView)
            {
                if (textBoxListViewText.Focused)
                {
                    ToggleDashesTextBox(textBoxListViewText);
                }
                else if (textBoxListViewTextOriginal.Focused)
                {
                    ToggleDashesTextBox(textBoxListViewTextOriginal);
                }
                else
                {
                    ToggleDashes();
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewToggleQuotes == e.KeyData && InListView)
            {
                if (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused)
                {
                    SurroundWithTag("\"", "\"", selectedTextOnly: true);
                }
                else
                {
                    SurroundWithTag("\"", "\"");
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewToggleHiTags == e.KeyData && InListView)
            {
                var tags = Configuration.Settings.General.TagsInToggleHiTags.Split(';');
                string openingTag;
                string closingTag;
                switch (tags.Length)
                {
                    case 1:
                        openingTag = tags[0];
                        closingTag = openingTag;
                        break;
                    case 2:
                        openingTag = tags[0];
                        closingTag = tags[1];
                        break;
                    default:
                        openingTag = "[";
                        closingTag = "]";
                        break;
                }

                if (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused)
                {
                    SurroundWithTag(openingTag, closingTag, true);
                }
                else
                {
                    SurroundWithTag(openingTag, closingTag);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewToggleCustomTags == e.KeyData && InListView)
            {
                var tags = Configuration.Settings.General.TagsInToggleCustomTags.Split('Æ');
                string openingTag;
                string closingTag;
                switch (tags.Length)
                {
                    case 1:
                        openingTag = tags[0];
                        closingTag = openingTag;
                        break;
                    case 2:
                        openingTag = tags[0];
                        closingTag = tags[1];
                        break;
                    default:
                        openingTag = string.Empty;
                        closingTag = string.Empty;
                        break;
                }

                if (!string.IsNullOrEmpty(openingTag) || !string.IsNullOrEmpty(closingTag))
                {
                    if (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused)
                    {
                        SurroundWithTag(openingTag, closingTag, true);
                    }
                    else
                    {
                        SurroundWithTag(openingTag, closingTag);
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTextBoxSelectionToggleCasing == e.KeyData)
            {
                e.SuppressKeyPress = true;
                if (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused)
                {
                    var tb = GetFocusedTextBox();
                    if (tb.SelectionLength > 0)
                    {
                        var start = tb.SelectionStart;
                        var length = tb.SelectionLength;
                        var text = tb.SelectedText.ToggleCasing(GetCurrentSubtitleFormat());
                        tb.SelectedText = text;
                        tb.SelectionStart = start;
                        tb.SelectionLength = length;
                    }
                }
                else
                {
                    ToggleCasingListView();
                }
            }
            else if (!toolStripMenuItemRtlUnicodeControlChars.Visible && _shortcuts.MainEditFixRTLViaUnicodeChars == e.KeyData && InListView)
            {
                ToolStripMenuItemRtlUnicodeControlCharsClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (!toolStripMenuItemRemoveUnicodeControlChars.Visible && _shortcuts.MainEditRemoveRTLUnicodeChars == e.KeyData && InListView)
            {
                ToolStripMenuItemRemoveUnicodeControlCharsClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (!toolStripMenuItemReverseRightToLeftStartEnd.Visible && _shortcuts.MainEditReverseStartAndEndingForRtl == e.KeyData && InListView)
            {
                ReverseStartAndEndingForRtl();
                e.SuppressKeyPress = true;
            }
            else if (toolStripMenuItemUndo.ShortcutKeys == e.KeyData) // undo
            {
                ToolStripMenuItemUndoClick(sender, e);
                e.SuppressKeyPress = true;
            }
            else if (toolStripMenuItemRedo.ShortcutKeys == e.KeyData) // redo
            {
                ToolStripMenuItemRedoClick(sender, e);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToNextSubtitlePlayTranslate == e.KeyData)
            {
                if (AutoRepeatContinueOn || AutoRepeatOn)
                {
                    PlayNext();
                }
                else
                {
                    ButtonNextClick(null, null);
                }
            }
            else if (_shortcuts.MainGeneralGoToPrevSubtitlePlayTranslate == e.KeyData)
            {
                if (AutoRepeatContinueOn || AutoRepeatOn)
                {
                    PlayPrevious();
                }
                else
                {
                    ButtonPreviousClick(null, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToNextSubtitle == e.KeyData)
            {
                MoveNextPrevious(0);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToNextSubtitleCursorAtEnd == e.KeyData)
            {
                MoveNextPrevious(0);
                textBoxListViewText.SelectionStart = textBoxListViewText.Text.Length;
                textBoxListViewText.SelectionLength = 0;
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToPrevSubtitle == e.KeyData)
            {
                ButtonPreviousClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToStartOfCurrentSubtitle == e.KeyData)
            {
                GotoSubPositionAndPause();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToEndOfCurrentSubtitle == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count == 1 && mediaPlayer.VideoPlayer != null)
                {
                    mediaPlayer.CurrentPosition = _subtitle.Paragraphs[SubtitleListview1.SelectedItems[0].Index].EndTime.TotalSeconds;
                    e.SuppressKeyPress = true;
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGoToPreviousSubtitleAndFocusVideo == e.KeyData)
            {
                int newIndex = _subtitleListViewIndex - 1;
                if (newIndex >= 0)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    GotoSubtitleIndex(newIndex);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGoToNextSubtitleAndFocusVideo == e.KeyData)
            {
                int newIndex = _subtitleListViewIndex + 1;
                if (newIndex < _subtitle.Paragraphs.Count)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    GotoSubtitleIndex(newIndex);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGoToPreviousSubtitleAndFocusWaveform == e.KeyData)
            {
                int newIndex = _subtitleListViewIndex - 1;
                if (newIndex >= 0)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    GotoSubtitleIndex(newIndex);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGoToNextSubtitleAndFocusWaveform == e.KeyData)
            {
                int newIndex = _subtitleListViewIndex + 1;
                if (newIndex < _subtitle.Paragraphs.Count)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    audioVisualizer.Focus();
                    GotoSubtitleIndex(newIndex);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGoToNextSubtitleAndPlay == e.KeyData && mediaPlayer != null)
            {
                int newIndex = _subtitleListViewIndex + 1;
                if (newIndex < _subtitle.Paragraphs.Count)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    GotoSubtitleIndex(newIndex);
                    var p = _subtitle.GetParagraphOrDefault(newIndex);
                    if (p != null)
                    {
                        ResetPlaySelection();
                        _endSeconds = p.EndTime.TotalSeconds;
                    }

                    e.SuppressKeyPress = true;
                }
            }
            else if (_shortcuts.MainGoToPrevSubtitleAndPlay == e.KeyData && mediaPlayer != null)
            {
                int newIndex = _subtitleListViewIndex - 1;
                if (newIndex >= 0)
                {
                    _subtitleListViewIndex = -1;
                    SelectListViewIndexAndEnsureVisible(newIndex);
                    _subtitleListViewIndex = newIndex;
                    GotoSubtitleIndex(newIndex);
                    var p = _subtitle.GetParagraphOrDefault(newIndex);
                    if (p != null)
                    {
                        ResetPlaySelection();
                        _endSeconds = p.EndTime.TotalSeconds;
                    }

                    e.SuppressKeyPress = true;
                }
            }
            else if (_shortcuts.MainTextBoxAutoBreak == e.KeyData && InListView)
            {
                    AutoBreak();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTextBoxUnbreak == e.KeyData && InListView)
            {
                if (textBoxListViewText.Focused)
                {
                    BreakUnbreakTextBox(true, textBoxListViewText);
                }
                else if (textBoxListViewTextOriginal.Focused)
                {
                    BreakUnbreakTextBox(true, textBoxListViewTextOriginal);
                }
                else
                {
                    Unbreak();
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTextBoxUnbreakNoSpace == e.KeyData && InListView)
            {
                if (textBoxListViewText.Focused)
                {
                    BreakUnbreakTextBox(true, textBoxListViewText, true);
                }
                else if (textBoxListViewTextOriginal.Focused)
                {
                    BreakUnbreakTextBox(true, textBoxListViewTextOriginal, true);
                }
                else
                {
                    Unbreak(true);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralToggleBookmarks == e.KeyData)
            {
                ToggleBookmarks(false, this);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralFocusTextBox == e.KeyData)
            {
                textBoxListViewText.Focus();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralEditBookmark == e.KeyData)
            {
                var p = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
                if (p?.Bookmark != null)
                {
                    LabelBookmarkDoubleClick(null, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralClearBookmarks == e.KeyData)
            {
                ClearBookmarks();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToBookmark == e.KeyData)
            {
                e.SuppressKeyPress = true;
                BeginInvoke(new Action(() => GoToBookmark()));
            }
            else if (_shortcuts.MainGeneralGoToPreviousBookmark == e.KeyData)
            {
                GoToPrevoiusBookmark();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToNextBookmark == e.KeyData)
            {
                GoToNextBookmark();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralChooseProfile == e.KeyData)
            {
                e.SuppressKeyPress = true;
                BeginInvoke(new Action(() => ChooseProfile()));
            }
            else if (_shortcuts.MainGeneralOpenDataFolder == e.KeyData)
            {
                UiUtil.OpenFolder(Configuration.DataDirectory);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralDuplicateLine == e.KeyData && SubtitleListview1.SelectedItems.Count == 1)
            {
                DuplicateLine();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralToggleView == e.KeyData)
            {
                if (InListView)
                {
                    SwitchView(SourceView);
                }
                else
                {
                    SwitchView(ListView);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetNewActor == e.KeyData)
            {
                var formatType = GetCurrentSubtitleFormat().GetType();
                if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
                {
                    SetNewActor(null, null);
                }
                else if (formatType == typeof(WebVTT) || formatType == typeof(WebVTTFileWithLineNumber))
                {
                    WebVTTSetNewVoiceTextBox(null, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor1 == e.KeyData)
            {
                SetActorVoice(0);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor2 == e.KeyData)
            {
                SetActorVoice(1);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor3 == e.KeyData)
            {
                SetActorVoice(2);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor4 == e.KeyData)
            {
                SetActorVoice(3);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor5 == e.KeyData)
            {
                SetActorVoice(4);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor6 == e.KeyData)
            {
                SetActorVoice(5);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor7 == e.KeyData)
            {
                SetActorVoice(6);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainListViewSetActor8 == e.KeyData)
            {
                SetActorVoice(7);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralToggleMode == e.KeyData && Configuration.Settings.General.ShowVideoControls)
            {
                var nextModeIndex = tabControlModes.SelectedIndex + 1;
                if (nextModeIndex == tabControlModes.TabCount)
                {
                    nextModeIndex = 0;
                }

                tabControlModes.SelectedIndex = nextModeIndex;
                tabControlModes.Focus();

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralTogglePreviewOnVideo == e.KeyData)
            {
                Configuration.Settings.General.MpvHandlesPreviewText = !Configuration.Settings.General.MpvHandlesPreviewText;
                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    if (!Configuration.Settings.General.MpvHandlesPreviewText)
                    {
                        libMpv.RemoveSubtitle();
                    }

                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralFileSaveAll == e.KeyData)
            {
                SaveAll();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralSetAssaResolution == e.KeyData)
            {
                SetAssaResolution(_subtitle);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralTakeAutoBackupNow == e.KeyData)
            {
                var saveFormat = GetCurrentSubtitleFormat();
                if (!saveFormat.IsTextBased)
                {
                    saveFormat = new SubRip();
                }

                if (_subtitle != null)
                {
                    if (RestoreAutoBackup.SaveAutoBackup(_subtitle, saveFormat, _subtitle.ToText(saveFormat)))
                    {
                        ShowStatus(_language.AutoBackupSaved);
                    }
                }

                if (_subtitleOriginalFileName != null && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    RestoreAutoBackup.SaveAutoBackup(_subtitleOriginal, saveFormat, _subtitleOriginal.ToText(saveFormat));
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainToggleFocus == e.KeyData)
            {
                if (InListView)
                {
                    if (SubtitleListview1.Focused)
                    {
                        textBoxListViewText.Focus();
                    }
                    else
                    {
                        SubtitleListview1.Focus();
                    }
                }
                else if (InSourceView)
                {
                    if (!textBoxSource.Focused)
                    {
                        textBoxSource.Focus();
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainToggleFocusWaveform == e.KeyData)
            {
                if (audioVisualizer.CanFocus)
                {
                    if (InListView)
                    {
                        if (SubtitleListview1.Focused || textBoxListViewText.Focused)
                        {
                            audioVisualizer.Focus();
                        }
                        else
                        {
                            SubtitleListview1.Focus();
                        }
                    }
                    else if (InSourceView)
                    {
                        if (textBoxSource.Focused)
                        {
                            audioVisualizer.Focus();
                        }
                        else
                        {
                            textBoxSource.Focus();
                        }
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainToggleFocusWaveformTextBox == e.KeyData)
            {
                if (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused)
                {
                    audioVisualizer.Focus();
                }
                else if (audioVisualizer.Focused)
                {
                    textBoxListViewText.Focus();
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToFirstSelectedLine == e.KeyData) //Locate first selected line in subtitle listview
            {
                if (SubtitleListview1.SelectedItems.Count > 0)
                {
                    SubtitleListview1.SelectedItems[0].EnsureVisible();
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralGoToFirstEmptyLine == e.KeyData) //Go to first empty line - if any
            {
                GoToFirstEmptyLine();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralMergeSelectedLines == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count >= 1)
                {
                    e.SuppressKeyPress = true;
                    if (SubtitleListview1.SelectedItems.Count == 2)
                    {
                        MergeAfterToolStripMenuItemClick(null, null);
                    }
                    else
                    {
                        MergeSelectedLines();
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeSelectedLinesAndAutoBreak == e.KeyData)
            {
                e.SuppressKeyPress = true;
                if (SubtitleListview1.SelectedItems.Count == 2)
                {
                    MergeWithLineAfter(false, BreakMode.AutoBreak);
                }
                else
                {
                    MergeSelectedLines(BreakMode.AutoBreak);
                }
            }
            else if (_shortcuts.MainGeneralMergeSelectedLinesAndUnbreak == e.KeyData)
            {
                e.SuppressKeyPress = true;
                if (SubtitleListview1.SelectedItems.Count == 2)
                {
                    MergeWithLineAfter(false, BreakMode.Unbreak);
                }
                else
                {
                    MergeSelectedLines(BreakMode.Unbreak);
                }
            }
            else if (_shortcuts.MainGeneralMergeSelectedLinesAndUnbreakNoSpace == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count >= 1)
                {
                    e.SuppressKeyPress = true;
                    if (SubtitleListview1.SelectedItems.Count == 2)
                    {
                        MergeWithLineAfter(false, BreakMode.UnbreakNoSpace);
                    }
                    else
                    {
                        MergeSelectedLines(BreakMode.UnbreakNoSpace);
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeSelectedLinesBilingual == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count >= 1 && SubtitleListview1.SelectedItems.Count < 10)
                {
                    e.SuppressKeyPress = true;
                    MergeSelectedLinesBilingual(SubtitleListview1.GetSelectedIndices());
                }
            }
            else if (_shortcuts.MainGeneralMergeWithPreviousBilingual == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 1 && SubtitleListview1.SelectedItems.Count >= 1)
                {
                    e.SuppressKeyPress = true;
                    MergeSelectedLinesBilingual(new int[] { SubtitleListview1.SelectedItems[0].Index - 1, SubtitleListview1.SelectedItems[0].Index });
                }
            }
            else if (_shortcuts.MainGeneralMergeWithNextBilingual == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 1 && SubtitleListview1.SelectedItems.Count >= 1)
                {
                    e.SuppressKeyPress = true;
                    MergeSelectedLinesBilingual(new int[] { SubtitleListview1.SelectedItems[0].Index, SubtitleListview1.SelectedItems[0].Index + 1 });
                }
            }
            else if (_shortcuts.MainGeneralMergeSelectedLinesOnlyFirstText == e.KeyData)
            {
                if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count >= 1)
                {
                    e.SuppressKeyPress = true;
                    MergeSelectedLinesOnlyFirstText();
                }
            }
            else if (_shortcuts.MainGeneralMergeWithNext == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx >= 0 && _subtitle.Paragraphs.Count > idx + 1)
                    {
                        SelectListViewIndexAndEnsureVisible(idx);
                        MergeAfterToolStripMenuItemClick(null, null);
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeWithPrevious == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx > 0)
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx - 1, true);
                        MergeAfterToolStripMenuItemClick(null, null);
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeWithNextAndUnbreak == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx >= 0 && _subtitle.Paragraphs.Count > idx + 1)
                    {
                        MakeHistoryForUndo(_language.BeforeMergeLines);
                        _makeHistoryPaused = true;
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx, true);
                        MergeAfterToolStripMenuItemClick(null, null);
                        ButtonUnBreakClick(null, null);
                        _makeHistoryPaused = false;
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeWithPreviousAndUnbreak == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx > 0)
                    {
                        MakeHistoryForUndo(_language.BeforeMergeLines);
                        _makeHistoryPaused = true;
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx - 1, true);
                        MergeAfterToolStripMenuItemClick(null, null);
                        ButtonUnBreakClick(null, null);
                        _makeHistoryPaused = false;
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeWithNextAndBreak == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx >= 0 && _subtitle.Paragraphs.Count > idx + 1)
                    {
                        MakeHistoryForUndo(_language.BeforeMergeLines);
                        _makeHistoryPaused = true;
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx, true);
                        MergeAfterToolStripMenuItemClick(null, null);
                        AutoBreak();
                        _makeHistoryPaused = false;
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralMergeWithPreviousAndBreak == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count >= 1)
                {
                    var idx = SubtitleListview1.SelectedItems[0].Index;
                    if (idx > 0)
                    {
                        MakeHistoryForUndo(_language.BeforeMergeLines);
                        _makeHistoryPaused = true;
                        SubtitleListview1.SelectIndexAndEnsureVisible(idx - 1, true);
                        MergeAfterToolStripMenuItemClick(null, null);
                        AutoBreak();
                        _makeHistoryPaused = false;
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (_shortcuts.MainGeneralToggleTranslationMode == e.KeyData)
            { // toggle translator mode
                EditToolStripMenuItemDropDownOpening(null, null);
                ToolStripMenuItemTranslationModeClick(null, null);
            }
            else if (e.KeyData == _shortcuts.VideoPlayPauseToggle)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    if (_shortcuts.VideoPlayPauseToggle == Keys.Space &&
                        (textBoxListViewText.Focused || textBoxListViewTextOriginal.Focused || textBoxSearchWord.Focused))
                    {
                        return;
                    }

                    ResetPlaySelection();
                    e.SuppressKeyPress = true;
                    TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(1), () => mediaPlayer.TogglePlayPause());
                }
            }
            else if (e.KeyData == _shortcuts.VideoPlay150Speed)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    if (mediaPlayer.IsPaused)
                    {
                        SetPlayRateAndPlay(150);
                    }
                    else if (mediaPlayer.VideoPlayer.PlayRate != 1.5)
                    {
                        SetPlayRateAndPlay(150, false);
                    }
                    else
                    {
                        mediaPlayer.Pause();
                    }

                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyData == _shortcuts.VideoPlay200Speed)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    if (mediaPlayer.IsPaused)
                    {
                        SetPlayRateAndPlay(200);
                    }
                    else if (mediaPlayer.VideoPlayer.PlayRate != 2.0)
                    {
                        SetPlayRateAndPlay(200, false);
                    }
                    else
                    {
                        mediaPlayer.Pause();
                    }

                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyData == _shortcuts.VideoPause)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    ResetPlaySelection();
                    mediaPlayer.Pause();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyData == _shortcuts.VideoStop)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    mediaPlayer.Stop();
                    e.SuppressKeyPress = true;
                }
            }
            else if (_shortcuts.MainVideoPlayFromJustBefore == e.KeyData)
            {
                ButtonBeforeTextClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideoPlayFromBeginning == e.KeyData)
            {
                mediaPlayer.Stop();
                mediaPlayer.Play();
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
            {
                if (!textBoxListViewText.Focused && !textBoxListViewTextOriginal.Focused && !textBoxSource.Focused && mediaPlayer.VideoPlayer != null)
                {
                    if (audioVisualizer.Focused || mediaPlayer.Focused || SubtitleListview1.Focused)
                    {
                        ResetPlaySelection();
                        mediaPlayer.TogglePlayPause();
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.D1)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    var p = _subtitle.GetParagraphOrDefault(SubtitleListview1.SelectedItems[0].Index);
                    if (p != null)
                    {
                        mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.D2)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    var p = _subtitle.GetParagraphOrDefault(SubtitleListview1.SelectedItems[0].Index);
                    if (p != null)
                    {
                        mediaPlayer.CurrentPosition = p.EndTime.TotalSeconds;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.D3)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    int index = SubtitleListview1.SelectedItems[0].Index - 1;
                    var p = _subtitle.GetParagraphOrDefault(index);
                    if (p != null)
                    {
                        SelectListViewIndexAndEnsureVisible(index);
                        mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.D4)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    int index = SubtitleListview1.SelectedItems[0].Index + 1;
                    var p = _subtitle.GetParagraphOrDefault(index);
                    if (p != null)
                    {
                        SelectListViewIndexAndEnsureVisible(index);
                        mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideoToggleStartEndCurrent == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    mediaPlayer.Pause();
                    var p = _subtitle.GetParagraphOrDefault(SubtitleListview1.SelectedItems[0].Index);
                    if (p != null)
                    {
                        if (Math.Abs(mediaPlayer.CurrentPosition - p.StartTime.TotalSeconds) < 0.1)
                        {
                            mediaPlayer.CurrentPosition = p.EndTime.TotalSeconds;
                        }
                        else
                        {
                            mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                        }
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideoPlaySelectedLines == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    PlaySelectedLines(false);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideoLoopSelectedLines == e.KeyData)
            {
                if (SubtitleListview1.SelectedItems.Count > 0 && _subtitle != null && mediaPlayer.VideoPlayer != null)
                {
                    PlaySelectedLines(true);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideoGoToStartCurrent == e.KeyData)
            {
                GotoSubPositionAndPause();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideo3000MsLeft == e.KeyData)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    GoBackSeconds(3);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainVideo3000MsRight == e.KeyData)
            {
                if (mediaPlayer.VideoPlayer != null)
                {
                    GoBackSeconds(-3);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == (Keys.Control | Keys.Alt | Keys.Shift) && e.KeyCode == Keys.W) // watermark
            {
                if (enc != Encoding.UTF8 && enc != Encoding.UTF32 && enc != Encoding.Unicode && enc != Encoding.UTF7)
                {
                    MessageBox.Show(LanguageSettings.Current.Watermark.ErrorUnicodeEncodingOnly);
                }
                else
                {
                    using (var watermarkForm = new Watermark())
                    {
                        MakeHistoryForUndo(LanguageSettings.Current.Watermark.BeforeWatermark);
                        watermarkForm.Initialize(_subtitle, FirstSelectedIndex);
                        if (watermarkForm.ShowDialog(this) == DialogResult.OK)
                        {
                            watermarkForm.AddOrRemove(_subtitle);
                            RefreshSelectedParagraph();
                        }
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == (Keys.Control | Keys.Alt | Keys.Shift) && e.KeyCode == Keys.F) // Toggle HHMMSSFF / HHMMSSMMM
            {
                Configuration.Settings.General.UseTimeFormatHHMMSSFF = !Configuration.Settings.General.UseTimeFormatHHMMSSFF;
                RefreshTimeCodeMode();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralSwitchTranslationAndOriginal == e.KeyData &&
                     _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0 && _networkSession == null)
            { // switch original/current
                int firstIndex = FirstSelectedIndex;
                if (firstIndex == -1)
                {
                    firstIndex = _subtitleListViewIndex;
                }

                double firstMs = -1;
                if (firstIndex >= 0)
                {
                    firstMs = _subtitle.Paragraphs[firstIndex].StartTime.TotalMilliseconds;
                }

                (_subtitle, _subtitleOriginal) = (_subtitleOriginal, _subtitle);
                (_fileName, _subtitleOriginalFileName) = (_subtitleOriginalFileName, _fileName);
                (_changeSubtitleHash, _changeOriginalSubtitleHash) = (_changeOriginalSubtitleHash, _changeSubtitleHash);

                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);

                _subtitleListViewIndex = -1;
                if (firstIndex == -1 && _subtitle?.Paragraphs?.Count > 0)
                {
                    firstIndex = 0;
                }

                SubtitleListview1.SelectIndexAndEnsureVisible(firstIndex, true);

                SetTitle();
                _fileDateTime = new DateTime();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralSwitchTranslationAndOriginalTextBoxes == e.KeyData &&
                     _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            { // switch original/current text boxes
                Configuration.Settings.General.TextAndOrigianlTextBoxesSwitched = !Configuration.Settings.General.TextAndOrigianlTextBoxesSwitched;
                MainResize();

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose == e.KeyData)
            {
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(25), () => ToolStripButtonLayoutChooseClick(null, null));
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose1 == e.KeyData)
            {
                SetLayout(0, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose2 == e.KeyData)
            {
                SetLayout(1, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose3 == e.KeyData)
            {
                SetLayout(2, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose4 == e.KeyData)
            {
                SetLayout(3, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose5 == e.KeyData)
            {
                SetLayout(4, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose6 == e.KeyData)
            {
                SetLayout(5, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose7 == e.KeyData)
            {
                SetLayout(6, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose8 == e.KeyData)
            {
                SetLayout(7, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose9 == e.KeyData)
            {
                SetLayout(8, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose10 == e.KeyData)
            {
                SetLayout(9, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose11 == e.KeyData)
            {
                SetLayout(10, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralLayoutChoose12 == e.KeyData)
            {
                SetLayout(11, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainGeneralMergeTranslationAndOriginal == e.KeyData) // Merge translation and original
            {
                if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0 && _networkSession == null)
                {
                    if (ContinueNewOrExit())
                    {
                        var subtitle = new Subtitle();
                        var fr = CurrentFrameRate;
                        var format = GetCurrentSubtitleFormat();
                        var videoFileName = _videoFileName;
                        foreach (var p in _subtitle.Paragraphs)
                        {
                            var newP = new Paragraph(p);
                            var original = Utilities.GetOriginalParagraph(_subtitle.GetIndex(p), p, _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                newP.Text = (newP.Text.TrimEnd() + Environment.NewLine + original.Text.TrimStart()).Trim();
                            }

                            subtitle.Paragraphs.Add(newP);
                        }

                        RemoveOriginal(true, true);
                        FileNew();
                        SetCurrentFormat(format);
                        toolStripComboBoxFrameRate.Text = fr.ToString();
                        _subtitle = subtitle;
                        _subtitleListViewIndex = -1;
                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                        if (!string.IsNullOrEmpty(videoFileName))
                        {
                            OpenVideo(videoFileName);
                        }

                        e.SuppressKeyPress = true;
                    }
                }
            }
            else if (e.KeyData == _shortcuts.ToggleVideoDockUndock)
            {
                if (_isVideoControlsUndocked)
                {
                    RedockVideoControlsToolStripMenuItemClick(null, null);
                }
                else
                {
                    UndockVideoControlsToolStripMenuItemClick(null, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && e.KeyData == _shortcuts.MainVideoFocusSetVideoPosition)
            {
                if (tabControlModes.SelectedTab == tabPageAdjust)
                {
                    timeUpDownVideoPositionAdjust.Focus();
                }
                else if (tabControlModes.SelectedTab == tabPageCreate)
                {
                    timeUpDownVideoPosition.Focus();
                }

                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && e.KeyData == _shortcuts.Video1FrameLeft)
            {
                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    libMpv.GetPreviousFrame();
                }
                else
                {
                    MoveVideoSeconds(-1.0 / Configuration.Settings.General.CurrentFrameRate);
                }

                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && e.KeyData == _shortcuts.Video1FrameRight)
            {
                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    libMpv.GetNextFrame();
                }
                else
                {
                    MoveVideoSeconds(1.0 / Configuration.Settings.General.CurrentFrameRate);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.Video1FrameLeftWithPlay)
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.Video1FrameRightWithPlay)
            {

                e.SuppressKeyPress = true;
            }
           
            else if (e.KeyData == _shortcuts.MainVideoFullscreen) // fullscreen
            {
                GoFullscreen(false);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoSlower)
            {
                e.SuppressKeyPress = true;
                for (var index = 0; index < _contextMenuStripPlayRate.Items.Count; index++)
                {
                    var item = (ToolStripMenuItem)_contextMenuStripPlayRate.Items[index];
                    if (item.Checked && index > 0)
                    {
                        SetPlayRate(_contextMenuStripPlayRate.Items[index - 1], null);
                        return;
                    }
                }
            }
            else if (e.KeyData == _shortcuts.MainVideoFaster)
            {
                e.SuppressKeyPress = true;
                for (var index = 0; index < _contextMenuStripPlayRate.Items.Count; index++)
                {
                    var item = (ToolStripMenuItem)_contextMenuStripPlayRate.Items[index];
                    if (item.Checked && index + 1 < _contextMenuStripPlayRate.Items.Count)
                    {
                        SetPlayRate(_contextMenuStripPlayRate.Items[index + 1], null);
                        return;
                    }
                }
            }
            else if (e.KeyData == _shortcuts.MainVideoSpeedToggle)
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoReset)
            {
                e.SuppressKeyPress = true;
                if (audioVisualizer != null)
                {
                    audioVisualizer.ZoomFactor = 1.0;
                    audioVisualizer.VerticalZoomFactor = 1.0;
                    InitializeWaveformZoomDropdown();
                }

               
            }
            else if (audioVisualizer.Focused && audioVisualizer.NewSelectionParagraph != null && e.KeyData == _shortcuts.WaveformAddTextAtHere)
            {
                AddParagraphHereToolStripMenuItemClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Focused && audioVisualizer.NewSelectionParagraph != null && e.KeyData == _shortcuts.WaveformAddTextAtHereFromClipboard)
            {
                AddParagraphAndPasteToolStripMenuItem_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.Focused && audioVisualizer.NewSelectionParagraph != null && e.KeyData == _shortcuts.WaveformSetParagraphAsNewSelection)
            {
                ToolStripMenuItemSetParagraphAsSelectionClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoGoToPrevSubtitle)
            {
                GoToPreviousSubtitle(mediaPlayer.CurrentPosition * TimeCode.BaseUnit);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoGoToNextSubtitle)
            {
                GoToNextSubtitle(mediaPlayer.CurrentPosition * TimeCode.BaseUnit);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoGoToPrevTimeCode)
            {
                GoToNearestTimeCode(mediaPlayer.CurrentPosition * TimeCode.BaseUnit, false);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoGoToNextTimeCode)
            {
                GoToNearestTimeCode(mediaPlayer.CurrentPosition * TimeCode.BaseUnit, true);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.VideoSelectNextSubtitle)
            {
                var cp = mediaPlayer.CurrentPosition * TimeCode.BaseUnit;
                foreach (var p in _subtitle.Paragraphs)
                {
                    if (p.StartTime.TotalMilliseconds > cp)
                    {
                        SubtitleListview1.SelectNone();
                        SubtitleListview1.Items[_subtitle.Paragraphs.IndexOf(p)].Selected = true;
                        SubtitleListview1.Items[_subtitle.Paragraphs.IndexOf(p)].Focused = true;
                        break;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.Chapters?.Length > 0 && e.KeyData == _shortcuts.VideoGoToPrevChapter)
            {
                var cp = mediaPlayer.CurrentPosition - 0.01;
                foreach (var chapter in mediaPlayer.Chapters.Reverse<MatroskaChapter>())
                {
                    if (chapter.StartTime < cp)
                    {
                        mediaPlayer.CurrentPosition = chapter.StartTime;
                        break;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.Chapters?.Length > 0 && e.KeyData == _shortcuts.VideoGoToNextChapter)
            {
                var cp = mediaPlayer.CurrentPosition + 0.01;
                foreach (var chapter in mediaPlayer.Chapters)
                {
                    if (chapter.StartTime > cp)
                    {
                        mediaPlayer.CurrentPosition = chapter.StartTime;
                        break;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.ShotChanges != null && e.KeyData == _shortcuts.WaveformGoToPreviousShotChange)
            {
                var cp = mediaPlayer.CurrentPosition - 0.01;
                foreach (var shotChange in audioVisualizer.ShotChanges.Reverse<double>())
                {
                    if (shotChange < cp)
                    {
                        mediaPlayer.CurrentPosition = shotChange;
                        break;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.ShotChanges != null && e.KeyData == _shortcuts.WaveformGoToNextShotChange)
            {
                var cp = mediaPlayer.CurrentPosition + 0.01;
                foreach (var shotChange in audioVisualizer.ShotChanges)
                {
                    if (shotChange > cp)
                    {
                        mediaPlayer.CurrentPosition = shotChange;
                        break;
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.ShotChanges != null && mediaPlayer.IsPaused && e.KeyData == _shortcuts.WaveformToggleShotChange)
            {
                var cp = mediaPlayer.CurrentPosition;
                var idx = audioVisualizer.GetShotChangeIndex(cp);
                if (idx >= 0)
                {
                    RemoveShotChange(idx);
                    if (audioVisualizer.ShotChanges.Count == 0)
                    {
                        ShotChangeHelper.DeleteShotChanges(_videoFileName);
                    }
                }
                else
                { // add shot change
                    var list = audioVisualizer.ShotChanges.Where(p => p > 0).ToList();
                    list.Add(cp);
                    list.Sort();
                    audioVisualizer.ShotChanges = list;
                    ShotChangeHelper.SaveShotChanges(_videoFileName, list);
                }

                e.SuppressKeyPress = true;
            }
            else if (audioVisualizer.ShotChanges != null && mediaPlayer.IsPaused && e.KeyData == _shortcuts.WaveformGuessStart)
            {
                AutoGuessStartTime(_subtitleListViewIndex);
                e.SuppressKeyPress = true;
            }
            else if (!string.IsNullOrEmpty(_videoFileName) && mediaPlayer.IsPaused && e.KeyData == _shortcuts.WaveformAudioToTextVosk)
            {
                e.SuppressKeyPress = true;
                AudioToTextVoskSelectedLines();
            }
            else if (!string.IsNullOrEmpty(_videoFileName) && mediaPlayer.IsPaused && e.KeyData == _shortcuts.WaveformAudioToTextWhisper)
            {
                e.SuppressKeyPress = true;
                AudioToTextWhisperSelectedLines();
            }
            else if (audioVisualizer.Focused && e.KeyCode == Keys.Delete)
            {
                ToolStripMenuItemDeleteClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainToolsAutoDuration == e.KeyData)
            {
                MakeAutoDuration();
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == (Keys.Control | Keys.Alt | Keys.Shift) && e.KeyCode == Keys.I)
            {
                using (var form = new ImportUnknownFormat(string.Empty))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        if (form.ImportedSubitle?.Paragraphs.Count > 0)
                        {
                            _subtitle = form.ImportedSubitle;
                            _fileName = null;
                            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                            SetTitle();
                        }
                    }
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainTextBoxMoveLastWordDown)
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    MoveLastWordDown(textBoxListViewTextOriginal);
                }
                else
                {
                    MoveLastWordDown(textBoxListViewText);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainTextBoxMoveFirstWordFromNextUp)
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    MoveFirstWordInNextUp(textBoxListViewTextOriginal);
                }
                else
                {
                    MoveFirstWordInNextUp(textBoxListViewText);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainTextBoxMoveLastWordDownCurrent)
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    MoveWordUpDownInCurrent(true, textBoxListViewTextOriginal);
                }
                else
                {
                    MoveWordUpDownInCurrent(true, textBoxListViewText);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainTextBoxMoveFirstWordUpCurrent)
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    MoveWordUpDownInCurrent(false, textBoxListViewTextOriginal);
                }
                else
                {
                    MoveWordUpDownInCurrent(false, textBoxListViewText);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainTextBoxMoveFromCursorToNextAndGoToNext)
            {
                if (textBoxListViewTextOriginal.Focused)
                {
                    MoveTextFromCursorToNext(textBoxListViewTextOriginal);
                }
                else
                {
                    MoveTextFromCursorToNext(textBoxListViewText);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAutoCalcCurrentDuration == e.KeyData)
            {
                RecalcCurrentDuration();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAutoCalcCurrentDurationByOptimalReadingSpeed == e.KeyData)
            {
                RecalcCurrentDuration(true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAutoCalcCurrentDurationByMinReadingSpeed == e.KeyData)
            {
                RecalcCurrentDurationMin();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F3 && e.Modifiers == Keys.Shift)
            {
                FindPrevious();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendCurrentSubtitle == e.KeyData)
            {
                ExtendCurrentSubtitle();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToNextSubtitle == e.KeyData)
            {
                ExtendSelectedLinesToNextLine();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToPreviousSubtitle == e.KeyData)
            {
                ExtendSelectedLinesToPreviousLine();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToNextSubtitleMinusChainingGap == e.KeyData)
            {
                ExtendSelectedLinesToNextLine(true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToPreviousSubtitleMinusChainingGap == e.KeyData)
            {
                ExtendSelectedLinesToPreviousLine(true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendPreviousLineEndToCurrentStart == e.KeyData)
            {
                ExtendPreviousEndToCurrentStart();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendNextLineStartToCurrentEnd == e.KeyData)
            {
                ExtendNextStartToCurrentEnd();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSnapStartToNextShotChange == e.KeyData)
            {
                SnapSelectedLinesStartToNextShotChange();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSnapEndToPreviousShotChange == e.KeyData)
            {
                SnapSelectedLinesEndToPreviousShotChange();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToNextShotChange == e.KeyData)
            {
                ExtendSelectedLinesToNextShotChange();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustExtendToPreviousShotChange == e.KeyData)
            {
                ExtendSelectedLinesToPreviousShotChange();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainSetInCueToClosestShotChangeLeftGreenZone == e.KeyData)
            {
                SetCueToClosestShotChangeGreenZone(true, true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainSetInCueToClosestShotChangeRightGreenZone == e.KeyData)
            {
                SetCueToClosestShotChangeGreenZone(true, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainSetOutCueToClosestShotChangeLeftGreenZone == e.KeyData)
            {
                SetCueToClosestShotChangeGreenZone(false, true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainSetOutCueToClosestShotChangeRightGreenZone == e.KeyData)
            {
                SetCueToClosestShotChangeGreenZone(false, false);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainListViewGoToNextError)
            {
                GoToNextSyntaxError();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainListViewRemoveBlankLines)
            {
                if (_subtitle != null && _subtitle.Paragraphs.Any(p => string.IsNullOrWhiteSpace(p.Text)))
                {
                    ShowStatus(LanguageSettings.Current.Settings.RemoveBlankLines);
                    MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.RemoveBlankLines));
                    SaveSubtitleListviewIndices();
                    _subtitle.RemoveEmptyLines();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();
                    RefreshSelectedParagraph();
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainWaveformAdd)
            {
                if (audioVisualizer.WavePeaks == null)
                {
                    AudioWaveform_Click(null, null);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoAudioToTextVosk)
            {
                VideoaudioToTextToolStripMenuItemClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoAudioToTextWhisper)
            {
                AudioToTextWhisperTolStripMenuItemClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoAudioExtractSelectedLines)
            {
                e.SuppressKeyPress = true;
                ExtractAudioSelectedLines();
            }
            else if (e.KeyData == _shortcuts.MainVideoToggleBrightness)
            {
                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    ShowStatus(string.Format("Brightness: {0}", libMpv.ToggleBrightness()), false);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainVideoToggleContrast)
            {
                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    ShowStatus(string.Format("Contrast: {0}", libMpv.ToggleContrast()), false);
                }

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == _shortcuts.MainCheckFixTimingViaShotChanges)
            {
                ShowCheckFixTimingViaShotChanges();
            }


            // TABS: Create / adjust / translate

            // create
            else if (_shortcuts.MainCreateInsertSubAtVideoPos == e.KeyData)
            {
                ButtonInsertNewTextClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainCreateInsertSubAtVideoPosNoTextBoxFocus == e.KeyData)
            {
                var p = InsertNewTextAtVideoPosition(false);
                p.Text = string.Empty;
                SubtitleListview1.SetText(_subtitle.GetIndex(p), p.Text);
                textBoxListViewText.Text = p.Text;
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainCreateInsertSubAtVideoPosMax == e.KeyData)
            {
                var p = InsertNewTextAtVideoPosition(true);
                p.Text = string.Empty;
                SubtitleListview1.SetText(_subtitle.GetIndex(p), p.Text);
                textBoxListViewText.Text = p.Text;
                e.SuppressKeyPress = true;
            }
            else if (tabControlModes.SelectedTab == tabPageCreate && e.Modifiers == Keys.Alt && e.KeyCode == Keys.F9)
            {
                StopAutoDuration();
                SetEndTime();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainCreateSetStart == e.KeyData)
            {
                ButtonSetStartTimeClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainCreateSetEnd == e.KeyData)
            {
                StopAutoDuration();
                SetEndTime();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustVideoSetStartForAppropriateLine == e.KeyData && mediaPlayer.VideoPlayer != null)
            {
                VideoSetStartForAppropriateLine(mediaPlayer.CurrentPosition);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustVideoSetEndForAppropriateLine == e.KeyData && mediaPlayer.VideoPlayer != null)
            {
                VideoSetEndForAppropriateLine(mediaPlayer.CurrentPosition);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSetEndAndPause == e.KeyData)
            {
                StopAutoDuration();
                mediaPlayer.Pause();
                SetEndTime();
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainCreateSetEndAddNewAndGoToNew == e.KeyData)
            {
                StopAutoDuration();
                e.SuppressKeyPress = true;

                if (SubtitleListview1.SelectedItems.Count == 1)
                {
                    double videoPositionMs = mediaPlayer.CurrentPosition * 1000.0;
                    if (!mediaPlayer.IsPaused)
                    {
                        videoPositionMs -= Configuration.Settings.General.SetStartEndHumanDelay / 1000.0;
                    }

                    int index = SubtitleListview1.SelectedItems[0].Index;
                    var p = _subtitle.Paragraphs[index];
                    var videoTimeCode = new TimeCode(videoPositionMs);
                    var duration = videoTimeCode.TotalMilliseconds - p.StartTime.TotalMilliseconds - MinGapBetweenLines;
                    if (duration > 0.01 &&
                        duration <= 60_000)
                    {
                        MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + _subtitle.Paragraphs[index].Number + " " + _subtitle.Paragraphs[index].Text));
                        var newEndTime = new TimeCode(videoTimeCode.TotalMilliseconds - MinGapBetweenLines);
                        p.EndTime = newEndTime;

                        SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                        SetDurationInSeconds(_subtitle.Paragraphs[index].DurationTotalSeconds);
                        var newP = InsertNewParagraphAtPosition(newEndTime.TotalMilliseconds + MinGapBetweenLines, false);
                        if (audioVisualizer.WavePeaks != null && newP.EndTime.TotalSeconds >= audioVisualizer.EndPositionSeconds - 0.1)
                        {
                            audioVisualizer.StartPositionSeconds = Math.Max(0, newP.StartTime.TotalSeconds - 0.1);
                        }

                        UpdateSourceView();
                        StartAutoDuration();
                    }

                    textBoxListViewText.Focus();
                }
            }
            else if (_shortcuts.MainCreateStartDownEndUp == e.KeyData)
            {
                if (_mainCreateStartDownEndUpParagraph == null)
                {
                    _mainCreateStartDownEndUpParagraph = InsertNewTextAtVideoPosition(false);
                }

                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSelected100MsForward == e.KeyData)
            {
                ShowEarlierOrLater(100, SelectionChoice.SelectionOnly);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSelected100MsBack == e.KeyData)
            {
                ShowEarlierOrLater(-100, SelectionChoice.SelectionOnly);
                e.SuppressKeyPress = true;
            }


            // adjust
            else if (_shortcuts.MainAdjustSelected100MsForward == e.KeyData)
            {
                ShowEarlierOrLater(100, SelectionChoice.SelectionOnly);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustSelected100MsBack == e.KeyData)
            {
                ShowEarlierOrLater(-100, SelectionChoice.SelectionOnly);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustAdjustStartXMsBack == e.KeyData)
            {
                MoveStartCurrent(-Configuration.Settings.Tools.MoveStartEndMs, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustAdjustStartXMsForward == e.KeyData)
            {
                MoveStartCurrent(Configuration.Settings.Tools.MoveStartEndMs, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustAdjustEndXMsBack == e.KeyData)
            {
                MoveEndCurrent(-Configuration.Settings.Tools.MoveStartEndMs, false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustAdjustEndXMsForward == e.KeyData)
            {
                MoveEndCurrent(Configuration.Settings.Tools.MoveStartEndMs, false);
                e.SuppressKeyPress = true;
            }

            else if (_shortcuts.MainAdjustMoveStartOneFrameBack == e.KeyData)
            {
                MoveStartCurrent(-(int)Math.Round(1000.0 / CurrentFrameRate), false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveStartOneFrameForward == e.KeyData)
            {
                MoveStartCurrent((int)Math.Round(1000.0 / CurrentFrameRate), false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveEndOneFrameBack == e.KeyData)
            {
                MoveEndCurrent(-(int)Math.Round(1000.0 / CurrentFrameRate), false);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveEndOneFrameForward == e.KeyData)
            {
                MoveEndCurrent((int)Math.Round(1000.0 / CurrentFrameRate), false);
                e.SuppressKeyPress = true;
            }

            else if (_shortcuts.MainAdjustMoveStartOneFrameBackKeepGapPrev == e.KeyData)
            {
                MoveStartCurrent(-(int)Math.Round(1000.0 / CurrentFrameRate), true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveStartOneFrameForwardKeepGapPrev == e.KeyData)
            {
                MoveStartCurrent((int)Math.Round(1000.0 / CurrentFrameRate), true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveEndOneFrameBackKeepGapNext == e.KeyData)
            {
                MoveEndCurrent(-(int)Math.Round(1000.0 / CurrentFrameRate), true);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainAdjustMoveEndOneFrameForwardKeepGapNext == e.KeyData)
            {
                MoveEndCurrent((int)Math.Round(1000.0 / CurrentFrameRate), true);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && (_shortcuts.MainAdjustSetStartAndOffsetTheRest == e.KeyData || _shortcuts.MainAdjustSetStartAndOffsetTheRest2 == e.KeyData))
            {
                ButtonSetStartAndOffsetRestClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartAndOffsetTheWholeSubtitle == e.KeyData)
            {
                SetStartAndOffsetTheWholeSubtitle();
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndAndOffsetTheRest == e.KeyData)
            {
                SetEndAndOffsetTheRest(false);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext == e.KeyData)
            {
                SetEndAndOffsetTheRest(true);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndAndGotoNext == e.KeyData)
            {
                ShowNextSubtitleLabel();
                ButtonSetEndAndGoToNextClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartKeepDuration == e.KeyData)
            {
                SetStartTime(true, mediaPlayer.CurrentPosition);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustInsertViaEndAutoStart == e.KeyData)
            {
                SetCurrentViaEndPositionAndGotoNext(FirstSelectedIndex, false);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustInsertViaEndAutoStartAndGoToNext == e.KeyData)
            {
                ShowNextSubtitleLabel();
                SetCurrentViaEndPositionAndGotoNext(FirstSelectedIndex, true);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndMinusGapAndStartNextHere == e.KeyData)
            {
                ShowNextSubtitleLabel();
                SetEndMinusGapAndStartNextHere(FirstSelectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndAndStartOfNextPlusGap == e.KeyData)
            {
                MainAdjustSetEndAndStartOfNextPlusGap(FirstSelectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartAutoDurationAndGoToNext == e.KeyData)
            {
                SetCurrentStartAutoDurationAndGotoNext(FirstSelectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetEndNextStartAndGoToNext == e.KeyData)
            {
                ShowNextSubtitleLabel();
                SetCurrentEndNextStartAndGoToNext(FirstSelectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustStartDownEndUpAndGoToNext == e.KeyData && _mainAdjustStartDownEndUpAndGoToNextParagraph == null)
            {
                ShowNextSubtitleLabel();
                _mainAdjustStartDownEndUpAndGoToNextParagraph = _subtitle.GetParagraphOrDefault(FirstSelectedIndex);
                SetStartTime(true, mediaPlayer.CurrentPosition);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartAndEndOfPrevious == e.KeyData)
            {
                var pos = mediaPlayer.CurrentPosition;
                SetStartAndEndOfPrevious(pos, false);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartAndEndOfPreviousAndGoToNext == e.KeyData)
            {
                var pos = mediaPlayer.CurrentPosition;
                SetStartAndEndOfPrevious(pos, true);
                e.SuppressKeyPress = true;
            }
            else if (mediaPlayer.VideoPlayer != null && _shortcuts.MainAdjustSetStartAndGotoNext == e.KeyData)
            {
                var pos = mediaPlayer.CurrentPosition;
                SetStartTime(false, pos);

                var index = _subtitleListViewIndex;
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
                if (mediaPlayer.IsPaused && index + 1 < _subtitle.Paragraphs.Count)
                {
                    mediaPlayer.CurrentPosition = _subtitle.Paragraphs[index + 1].StartTime.TotalSeconds;
                }
                e.SuppressKeyPress = true;
            }

            // translate
            else if (_shortcuts.MainTranslateGoogleIt == e.KeyData)
            {
                ButtonGoogleItClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateGoogleTranslateIt == e.KeyData)
            {
                ButtonGoogleTranslateItClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateCustomSearch1 == e.KeyData)
            {
                RunCustomSearch(Configuration.Settings.VideoControls.CustomSearchUrl1);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateCustomSearch2 == e.KeyData)
            {
                RunCustomSearch(Configuration.Settings.VideoControls.CustomSearchUrl2);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateCustomSearch3 == e.KeyData)
            {
                RunCustomSearch(Configuration.Settings.VideoControls.CustomSearchUrl3);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateCustomSearch4 == e.KeyData)
            {
                RunCustomSearch(Configuration.Settings.VideoControls.CustomSearchUrl4);
                e.SuppressKeyPress = true;
            }
            else if (_shortcuts.MainTranslateCustomSearch5 == e.KeyData)
            {
                RunCustomSearch(Configuration.Settings.VideoControls.CustomSearchUrl5);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == (Keys.Alt | Keys.Shift | Keys.Control) && e.KeyCode == Keys.T)
            {
                using (var form = new TextToSpeech(_subtitle, _videoFileName, _videoInfo))
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                }

                e.SuppressKeyPress = true;
            }


            // put new entries above tabs

            //if (e.Modifiers == (Keys.Alt | Keys.Shift | Keys.Control) && e.KeyCode == Keys.something)
            //{
            //    new WordSplitDictionaryGenerator().ShowDialog(this);
            //}
        }

        private int _layout = 0;

        private void SetActorVoice(int index)
        {
            var formatType = GetCurrentSubtitleFormat().GetType();
            if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
            {
                var actors = new List<string>();
                foreach (var p in _subtitle.Paragraphs)
                {
                    if (!string.IsNullOrEmpty(p.Actor) && !actors.Contains(p.Actor))
                    {
                        actors.Add(p.Actor);
                    }

                    actors.Sort();
                }

                if (index >= 0 && index < actors.Count)
                {
                    SetActor(actors[index]);
                }
            }
            else if (formatType == typeof(WebVTT) || formatType == typeof(WebVTTFileWithLineNumber))
            {
                var voices = WebVTT.GetVoices(_subtitle).OrderBy(p => p).ToList();
                if (index >= 0 && index < voices.Count)
                {
                    WebVTTSetVoice(voices[index]);
                }
            }
        }

        private void ToggleCasingListView()
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 0)
            {
                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.MainTextBoxSelectionToggleCasing));

                var indices = new List<int>();
                string first = null;

                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    indices.Add(item.Index);
                }


                var format = GetCurrentSubtitleFormat();
                SubtitleListview1.BeginUpdate();
                foreach (int i in indices)
                {
                    if (first == null)
                    {
                        first = _subtitle.Paragraphs[i].Text;
                    }

                    _subtitle.Paragraphs[i].Text = _subtitle.Paragraphs[i].Text.ToggleCasing(format, first);
                    SubtitleListview1.SetText(i, _subtitle.Paragraphs[i].Text);

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(i, _subtitle.Paragraphs[i], _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            original.Text = original.Text.ToggleCasing(format, first);
                            SubtitleListview1.SetOriginalText(i, original.Text);
                        }
                    }
                }
                SubtitleListview1.EndUpdate();

                UpdateSourceView();
                RefreshSelectedParagraph();
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            }
        }

        private void ShowCheckFixTimingViaShotChanges()
        {
            using (var form = new AdjustTimingViaShotChanges(_subtitle, _videoFileName, audioVisualizer.WavePeaks, audioVisualizer.ShotChanges))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }
        }


        private void ToggleVideoControlsOnOff(bool on)
        {
            if (_isVideoControlsUndocked)
            {
                return;
            }

            groupBoxVideo.SuspendLayout();
            tabControlModes.Visible = on;
            var left = 5;
            if (on)
            {
                left = tabControlModes.Width + 10;
            }

            if (panelVideoPlayer.Parent == groupBoxVideo)
            {
                panelVideoPlayer.Left = left;
                panelVideoPlayer.Width = groupBoxVideo.Width - (panelVideoPlayer.Left + 10);
            }

            audioVisualizer.Left = left;
            audioVisualizer.Width = groupBoxVideo.Width - (audioVisualizer.Left + 10);
            checkBoxSyncListViewWithVideoWhilePlaying.Left = left;
            panelWaveformControls.Left = left;
            trackBarWaveformPosition.Left = left + panelWaveformControls.Width;
            trackBarWaveformPosition.Width = audioVisualizer.Left + audioVisualizer.Width - trackBarWaveformPosition.Left + 5;
            groupBoxVideo.ResumeLayout();
            audioVisualizer.Invalidate();
        }

        private void SetStartAndEndOfPrevious(double positionInSeconds, bool goToNext)
        {
            int index = SubtitleListview1.SelectedItems[0].Index;
            var current = _subtitle.GetParagraphOrDefault(index);
            if (SubtitleListview1.SelectedItems.Count != 1 || current == null)
            {
                return;
            }

            if (positionInSeconds > current.EndTime.TotalSeconds - Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds / 1000)
            {
                return;
            }

            // previous sub
            var p = _subtitle.GetParagraphOrDefault(index - 1);
            if (p == null || p.StartTime.TotalMilliseconds < p.StartTime.TotalMilliseconds - 9000)
            {
                SetStartTime(false, positionInSeconds);
                return;
            }

            if (positionInSeconds < p.StartTime.TotalSeconds + Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds / 1000)
            {
                return;
            }

            MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + p.Number + " " + p.Text));
            var oldCurrent = new Paragraph(current, false);
            current.StartTime.TotalMilliseconds = positionInSeconds * 1_000.0;
            UpdateOriginalTimeCodes(oldCurrent);
            if (oldCurrent.EndTime.IsMaxTime)
            {
                current.EndTime.TotalMilliseconds = current.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
                UpdateOriginalTimeCodes(oldCurrent);
            }

            var oldParagraph = new Paragraph(p, false);
            p.EndTime.TotalMilliseconds = positionInSeconds * TimeCode.BaseUnit - MinGapBetweenLines;
            if (oldParagraph.StartTime.IsMaxTime)
            {
                p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(p.Text);
            }

            UpdateOriginalTimeCodes(oldParagraph);
            SubtitleListview1.SetStartTimeAndDuration(index, current, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
            SubtitleListview1.SetStartTimeAndDuration(index - 1, p, current, _subtitle.GetParagraphOrDefault(index - 2));
            UpdateSourceView();
            var next = _subtitle.GetParagraphOrDefault(index - 1);
            if (goToNext && next != null)
            {
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
            }
            else
            {
                p = _subtitle.GetParagraphOrDefault(index);
                InitializeListViewEditBox(p);
            }
        }

        private void ExtendCurrentSubtitle()
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                var idx = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.Paragraphs[idx];
                var next = _subtitle.GetParagraphOrDefault(idx + 1);
                if (next == null || next.StartTime.TotalMilliseconds > p.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds + MinGapBetweenLines)
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
                }
                else
                {
                    p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                }

                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendCurrentSubtitle));

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        var originalNext = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) + 1);
                        if (originalNext == null || originalNext.StartTime.TotalMilliseconds > original.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds + MinGapBetweenLines)
                        {
                            original.EndTime.TotalMilliseconds = original.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
                        }
                        else
                        {
                            original.EndTime.TotalMilliseconds = originalNext.StartTime.TotalMilliseconds - MinGapBetweenLines;
                        }
                    }
                }

                RefreshSelectedParagraph();
            }
        }

        private void ExtendSelectedLinesToNextLine(bool minusChainingGap = false)
        {
            double GetNextStartTimeMinusChainingGap(Paragraph next)
            {
                if (ShotChangeHelper.IsCueOnShotChange(audioVisualizer.ShotChanges, next.StartTime, true))
                {
                    if (Configuration.Settings.BeautifyTimeCodes.Profile.ChainingInCueOnShotUseZones)
                    {
                        return next.StartTime.TotalMilliseconds - SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.ChainingInCueOnShotLeftGreenZone);
                    }
                    else
                    {
                        return next.StartTime.TotalMilliseconds - Configuration.Settings.BeautifyTimeCodes.Profile.ChainingInCueOnShotMaxGap;
                    }
                }
                else
                {
                    if (Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralUseZones)
                    {
                        return next.StartTime.TotalMilliseconds - SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralLeftGreenZone);
                    }
                    else
                    {
                        return next.StartTime.TotalMilliseconds - Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralMaxGap;
                    }
                }
            }

            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                var next = _subtitle.GetParagraphOrDefault(idx + 1);
                if (next != null)
                {
                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, minusChainingGap ? LanguageSettings.Current.Settings.AdjustExtendToNextSubtitleMinusChainingGap : LanguageSettings.Current.Settings.AdjustExtendToNextSubtitle));
                        historyAdded = true;
                    }

                    if (minusChainingGap)
                    {
                        p.EndTime.TotalMilliseconds = GetNextStartTimeMinusChainingGap(next);
                    }
                    else
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                    }

                    if (p.DurationTotalMilliseconds < 0)
                    {
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds;
                    }
                }

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        var originalNext = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) + 1);
                        if (originalNext != null)
                        {
                            if (!historyAdded)
                            {
                                MakeHistoryForUndo(string.Format(_language.BeforeX, minusChainingGap ? LanguageSettings.Current.Settings.AdjustExtendToNextSubtitleMinusChainingGap : LanguageSettings.Current.Settings.AdjustExtendToNextSubtitle));
                                historyAdded = true;
                            }

                            if (minusChainingGap)
                            {
                                original.EndTime.TotalMilliseconds = GetNextStartTimeMinusChainingGap(originalNext);
                            }
                            else
                            {
                                original.EndTime.TotalMilliseconds = originalNext.StartTime.TotalMilliseconds - MinGapBetweenLines;
                            }

                            if (original.DurationTotalMilliseconds < 0)
                            {
                                original.EndTime.TotalMilliseconds = original.StartTime.TotalMilliseconds;
                            }
                        }
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void ExtendSelectedLinesToPreviousLine(bool minusChainingGap = false)
        {
            double GetPreviousEndTimePlusChainingGap(Paragraph previous)
            {
                if (ShotChangeHelper.IsCueOnShotChange(audioVisualizer.ShotChanges, previous.EndTime, false))
                {
                    if (Configuration.Settings.BeautifyTimeCodes.Profile.ChainingOutCueOnShotUseZones)
                    {
                        return previous.EndTime.TotalMilliseconds + SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.ChainingOutCueOnShotRightGreenZone);
                    }
                    else
                    {
                        return previous.EndTime.TotalMilliseconds + Configuration.Settings.BeautifyTimeCodes.Profile.ChainingOutCueOnShotMaxGap;
                    }
                }
                else
                {
                    if (Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralUseZones)
                    {
                        return previous.EndTime.TotalMilliseconds + SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralLeftGreenZone);
                    }
                    else
                    {
                        return previous.EndTime.TotalMilliseconds + Configuration.Settings.BeautifyTimeCodes.Profile.ChainingGeneralMaxGap;
                    }
                }
            }

            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                var previous = _subtitle.GetParagraphOrDefault(idx - 1);
                if (previous != null)
                {
                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            var originalPrevious = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) - 1);
                            if (originalPrevious != null)
                            {
                                if (!historyAdded)
                                {
                                    MakeHistoryForUndo(string.Format(_language.BeforeX, minusChainingGap ? LanguageSettings.Current.Settings.AdjustExtendToPreviousSubtitleMinusChainingGap : LanguageSettings.Current.Settings.AdjustExtendToPreviousSubtitle));
                                    historyAdded = true;
                                }

                                if (minusChainingGap)
                                {
                                    original.StartTime.TotalMilliseconds = GetPreviousEndTimePlusChainingGap(originalPrevious);
                                }
                                else
                                {
                                    original.StartTime.TotalMilliseconds = originalPrevious.EndTime.TotalMilliseconds + MinGapBetweenLines;
                                }

                                if (original.DurationTotalMilliseconds < 0)
                                {
                                    original.StartTime.TotalMilliseconds = original.EndTime.TotalMilliseconds;
                                }
                            }
                        }
                    }

                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, minusChainingGap ? LanguageSettings.Current.Settings.AdjustExtendToPreviousSubtitleMinusChainingGap : LanguageSettings.Current.Settings.AdjustExtendToPreviousSubtitle));
                        historyAdded = true;
                    }

                    if (minusChainingGap)
                    {
                        p.StartTime.TotalMilliseconds = GetPreviousEndTimePlusChainingGap(previous);
                    }
                    else
                    {
                        p.StartTime.TotalMilliseconds = previous.EndTime.TotalMilliseconds + MinGapBetweenLines;
                    }

                    if (p.DurationTotalMilliseconds < 0)
                    {
                        p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds;
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void ExtendPreviousEndToCurrentStart()
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                var idx = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.Paragraphs[idx];
                var previous = _subtitle.GetParagraphOrDefault(idx - 1);
                if (previous is null)
                {
                    return;
                }

                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendPreviousLineEndToCurrentStart));
                previous.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds - MinGapBetweenLines;

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        var originalPrevious = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) - 1);
                        if (originalPrevious != null)
                        {
                            originalPrevious.EndTime.TotalMilliseconds = original.StartTime.TotalMilliseconds - MinGapBetweenLines;
                        }
                    }
                }

                SubtitleListview1.SetStartTimeAndDuration(idx - 1, previous, p, _subtitle.GetParagraphOrDefault(idx - 2));
                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, idx - 1, previous);
            }
        }

        private void ExtendNextStartToCurrentEnd()
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                var idx = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.Paragraphs[idx];
                var next = _subtitle.GetParagraphOrDefault(idx + 1);
                if (next is null)
                {
                    return;
                }

                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendNextLineStartToCurrentEnd));
                next.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds + MinGapBetweenLines;

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        var originalNext = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) + 1);
                        if (originalNext != null)
                        {
                            originalNext.StartTime.TotalMilliseconds = original.EndTime.TotalMilliseconds + MinGapBetweenLines;
                        }
                    }
                }

                SubtitleListview1.SetStartTimeAndDuration(idx + 1, next, _subtitle.GetParagraphOrDefault(idx + 2), p);
                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, idx + 1, next);
            }
        }

        private void SnapSelectedLinesStartToNextShotChange()
        {
            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                var nextShotChange = ShotChangeHelper.GetNextShotChangeInMs(audioVisualizer.ShotChanges, p.StartTime);
                if (nextShotChange != null)
                {
                    var newStartTime = nextShotChange.Value + TimeCodesBeautifierUtils.GetInCuesGapMs();

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            if (!historyAdded)
                            {
                                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustSnapStartToNextShotChange));
                                historyAdded = true;
                            }

                            if (newStartTime < p.EndTime.TotalMilliseconds)
                            {
                                original.StartTime.TotalMilliseconds = newStartTime;
                            }
                        }
                    }

                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustSnapStartToNextShotChange));
                        historyAdded = true;
                    }

                    if (newStartTime < p.EndTime.TotalMilliseconds)
                    {
                        p.StartTime.TotalMilliseconds = newStartTime;
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void SnapSelectedLinesEndToPreviousShotChange()
        {
            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                var previousShotChange = ShotChangeHelper.GetPreviousShotChangeInMs(audioVisualizer.ShotChanges, p.EndTime);
                if (previousShotChange != null)
                {
                    var newEndTime = previousShotChange.Value - TimeCodesBeautifierUtils.GetOutCuesGapMs();

                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustSnapEndToPreviousShotChange));
                        historyAdded = true;
                    }

                    if (newEndTime > p.StartTime.TotalMilliseconds)
                    {
                        p.EndTime.TotalMilliseconds = newEndTime;
                    }

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            if (!historyAdded)
                            {
                                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustSnapEndToPreviousShotChange));
                                historyAdded = true;
                            }

                            if (newEndTime > p.StartTime.TotalMilliseconds)
                            {
                                original.EndTime.TotalMilliseconds = newEndTime;
                            }
                        }
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void ExtendSelectedLinesToNextShotChange()
        {
            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                if (audioVisualizer.ShotChanges.Count > 0)
                {
                    var next = _subtitle.GetParagraphOrDefault(idx + 1);
                    double nearestShotChangeWithGap = ShotChangeHelper.GetNextShotChangeMinusGapInMs(audioVisualizer.ShotChanges, p.EndTime) ?? double.MaxValue;
                    double nearestStartTimeWithGap = next != null ? next.StartTime.TotalMilliseconds - MinGapBetweenLines : Double.MaxValue;

                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendToNextShotChange));
                        historyAdded = true;
                    }

                    var newEndTime = Math.Min(nearestShotChangeWithGap, nearestStartTimeWithGap);
                    if (newEndTime <= _videoInfo.TotalMilliseconds)
                    {
                        p.EndTime.TotalMilliseconds = newEndTime;
                    }

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            var originalNext = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) + 1);
                            double nearestOriginalStartTimeWithGap = originalNext != null ? originalNext.StartTime.TotalMilliseconds - MinGapBetweenLines : Double.MaxValue;

                            if (!historyAdded)
                            {
                                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendToNextShotChange));
                                historyAdded = true;
                            }

                            var originalNewEndTime = Math.Min(nearestShotChangeWithGap, nearestStartTimeWithGap);
                            if (originalNewEndTime <= _videoInfo.TotalMilliseconds)
                            {
                                original.EndTime.TotalMilliseconds = originalNewEndTime;
                            }
                        }
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void ExtendSelectedLinesToPreviousShotChange(bool withGap = false)
        {
            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];
                if (audioVisualizer.ShotChanges.Count > 0)
                {
                    var previous = _subtitle.GetParagraphOrDefault(idx - 1);
                    double nearestShotChangeWithGap = ShotChangeHelper.GetPreviousShotChangePlusGapInMs(audioVisualizer.ShotChanges, p.StartTime) ?? double.MinValue;
                    double nearestEndTimeWithGap = previous != null ? previous.EndTime.TotalMilliseconds + MinGapBetweenLines : -9999;

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            var originalPrevious = _subtitleOriginal.GetParagraphOrDefault(_subtitleOriginal.GetIndex(original) - 1);
                            double nearestOriginalEndTimeWithGap = originalPrevious != null ? originalPrevious.EndTime.TotalMilliseconds + MinGapBetweenLines : -9999;

                            if (!historyAdded)
                            {
                                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendToPreviousShotChange));
                                historyAdded = true;
                            }

                            var originalNewStartTime = Math.Max(nearestShotChangeWithGap, nearestOriginalEndTimeWithGap);
                            if (originalNewStartTime >= 0)
                            {
                                original.StartTime.TotalMilliseconds = originalNewStartTime;
                            }
                        }
                    }

                    if (!historyAdded)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.AdjustExtendToPreviousShotChange));
                        historyAdded = true;
                    }

                    var newStartTime = Math.Max(nearestShotChangeWithGap, nearestEndTimeWithGap);
                    if (newStartTime >= 0)
                    {
                        p.StartTime.TotalMilliseconds = newStartTime;
                    }
                }

                RefreshSelectedParagraphs();
            }
        }

        private void SetCueToClosestShotChangeGreenZone(bool isInCue, bool isLeft)
        {
            void MakeHistory()
            {
                if (isInCue && isLeft)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.SetInCueToClosestShotChangeLeftGreenZone));
                }
                else if (isInCue && !isLeft)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.SetInCueToClosestShotChangeRightGreenZone));
                }
                else if (!isInCue && isLeft)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.SetOutCueToClosestShotChangeLeftGreenZone));
                }
                else if (!isInCue && !isLeft)
                {
                    MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.SetOutCueToClosestShotChangeRightGreenZone));
                }
            }

            void SetCueToClosestShotChangeGreenZone(Paragraph p, Subtitle sub)
            {
                if (isInCue)
                {
                    var closestShotChange = ShotChangeHelper.GetClosestShotChange(audioVisualizer.ShotChanges, p.StartTime);
                    if (closestShotChange != null)
                    {
                        var newInCue = isLeft ? (closestShotChange.Value * 1000) - SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.InCuesLeftGreenZone)
                            : (closestShotChange.Value * 1000) + SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.InCuesRightGreenZone);

                        double newStart = 0;
                        double newPreviousEnd = 0;

                        if (newInCue >= 0 && newInCue < p.EndTime.TotalMilliseconds)
                        {
                            newStart = newInCue;
                        }
                        else
                        {
                            return;
                        }

                        var previous = sub.GetParagraphOrDefault(sub.GetIndex(p) - 1);
                        if (previous != null)
                        {
                            if (isLeft)
                            {
                                // Push previous subtitle away if overlap
                                newPreviousEnd = Math.Min(previous.EndTime.TotalMilliseconds, newStart - Configuration.Settings.General.MinimumMillisecondsBetweenLines);
                            }
                            else
                            {
                                // Push previous subtitle away if overlap, until green zone edge
                                newPreviousEnd = Math.Min(newInCue, previous.EndTime.TotalMilliseconds);
                                newStart = Math.Max(newInCue, newPreviousEnd + Configuration.Settings.General.MinimumMillisecondsBetweenLines);
                            }

                            // Prevent invalid durations
                            if (newPreviousEnd - previous.StartTime.TotalMilliseconds > 0 && p.EndTime.TotalMilliseconds - newStart > 0)
                            {
                                p.StartTime.TotalMilliseconds = newStart;
                                previous.EndTime.TotalMilliseconds = newPreviousEnd;
                            }
                        }
                        else
                        {
                            p.StartTime.TotalMilliseconds = newStart;
                        }
                    }
                }
                else
                {
                    var closestShotChange = ShotChangeHelper.GetClosestShotChange(audioVisualizer.ShotChanges, p.EndTime);
                    if (closestShotChange != null)
                    {
                        var newOutCue = isLeft ? (closestShotChange.Value * 1000) - SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.OutCuesLeftGreenZone)
                            : (closestShotChange.Value * 1000) + SubtitleFormat.FramesToMilliseconds(Configuration.Settings.BeautifyTimeCodes.Profile.OutCuesRightGreenZone);

                        double newEnd = 0;
                        double newNextStart = 0;

                        if (newOutCue > p.StartTime.TotalMilliseconds)
                        {
                            newEnd = newOutCue;
                        }
                        else
                        {
                            return;
                        }

                        var next = sub.GetParagraphOrDefault(sub.GetIndex(p) + 1);
                        if (next != null)
                        {
                            if (!isLeft)
                            {
                                // Push next subtitle away if overlap
                                newNextStart = Math.Max(next.StartTime.TotalMilliseconds, newEnd + Configuration.Settings.General.MinimumMillisecondsBetweenLines);
                            }
                            else
                            {
                                // Push next subtitle away if overlap, until green zone edge
                                newNextStart = Math.Max(next.StartTime.TotalMilliseconds, newOutCue);
                                newEnd = Math.Min(newNextStart - Configuration.Settings.General.MinimumMillisecondsBetweenLines, newOutCue);
                            }

                            // Prevent invalid durations
                            if (next.EndTime.TotalMilliseconds - newNextStart > 0 && newEnd - p.StartTime.TotalMilliseconds > 0)
                            {
                                p.EndTime.TotalMilliseconds = newEnd;
                                next.StartTime.TotalMilliseconds = newNextStart;
                            }
                        }
                        else
                        {
                            p.EndTime.TotalMilliseconds = newEnd;
                        }
                    }
                }
            }

            var historyAdded = false;
            foreach (ListViewItem selectedItem in SubtitleListview1.SelectedItems)
            {
                var idx = selectedItem.Index;
                var p = _subtitle.Paragraphs[idx];

                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        if (!historyAdded)
                        {
                            MakeHistory();
                            historyAdded = true;
                        }

                        SetCueToClosestShotChangeGreenZone(original, _subtitleOriginal);
                    }
                }

                if (!historyAdded)
                {
                    MakeHistory();
                    historyAdded = true;
                }

                SetCueToClosestShotChangeGreenZone(p, _subtitle);

                RefreshSelectedParagraphs();
            }
        }

        private void GoToPreviousSubtitle(double currentPosition)
        {
            var found = false;
            foreach (var p in _subtitle.Paragraphs)
            {
                if (p.StartTime.TotalMilliseconds > currentPosition - 1)
                {
                    var prev = _subtitle.GetParagraphOrDefault(_subtitle.Paragraphs.IndexOf(p) - 1);
                    if (prev == null)
                    {
                        break;
                    }

                    mediaPlayer.CurrentPosition = prev.StartTime.TotalSeconds;
                    SelectListViewIndexAndEnsureVisible(_subtitle.Paragraphs.IndexOf(prev));
                    if (audioVisualizer.WavePeaks != null && p.StartTime.TotalSeconds > audioVisualizer.EndPositionSeconds + 0.2)
                    {
                        audioVisualizer.StartPositionSeconds = mediaPlayer.CurrentPosition - 0.2;
                    }

                    found = true;
                    break;
                }
            }

            if (!found && _subtitle.Paragraphs.Count > 0 && _subtitle.Paragraphs[_subtitle.Paragraphs.Count - 1].StartTime.TotalMilliseconds < currentPosition)
            {
                var p = _subtitle.Paragraphs[_subtitle.Paragraphs.Count - 1];
                mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                SelectListViewIndexAndEnsureVisible(_subtitle.Paragraphs.Count - 1);
                if (audioVisualizer.WavePeaks != null && p.StartTime.TotalSeconds > audioVisualizer.EndPositionSeconds + 0.2)
                {
                    audioVisualizer.StartPositionSeconds = mediaPlayer.CurrentPosition - 0.2;
                }
            }
        }

        private void GoToNextSubtitle(double currentPosition)
        {
            foreach (var p in _subtitle.Paragraphs)
            {
                if (p.StartTime.TotalMilliseconds > currentPosition + 0.01)
                {
                    mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                    SelectListViewIndexAndEnsureVisible(_subtitle.Paragraphs.IndexOf(p));
                    if (audioVisualizer.WavePeaks != null && p.StartTime.TotalSeconds > audioVisualizer.EndPositionSeconds + 0.2)
                    {
                        audioVisualizer.StartPositionSeconds = mediaPlayer.CurrentPosition - 0.2;
                    }

                    break;
                }
            }
        }

        private void GoToNearestTimeCode(double currentPosition, bool forward)
        {
            var paragraphsStart = forward
                ? _subtitle.Paragraphs.Where(p => p.StartTime.TotalMilliseconds > currentPosition + 1)
                : _subtitle.Paragraphs.Where(p => p.StartTime.TotalMilliseconds < currentPosition - 1);

            var paragraphsEnd = forward
                ? _subtitle.Paragraphs.Where(p => p.EndTime.TotalMilliseconds > currentPosition + 1)
                : _subtitle.Paragraphs.Where(p => p.EndTime.TotalMilliseconds < currentPosition - 1);

            var closestStart = paragraphsStart
                .Select(p => new { Paragraph = p, Distance = Math.Abs(p.StartTime.TotalMilliseconds - currentPosition) })
                .OrderBy(p => p.Distance)
                .FirstOrDefault();

            var closestEnd = paragraphsEnd
                .Select(p => new { Paragraph = p, Distance = Math.Abs(p.EndTime.TotalMilliseconds - currentPosition) })
                .OrderBy(p => p.Distance)
                .FirstOrDefault();

            Paragraph found = null;
            double foundSeconds = 0d;

            if (closestStart != null && (closestEnd == null || Math.Abs(closestStart.Paragraph.StartTime.TotalMilliseconds - currentPosition) <
                Math.Abs(closestEnd.Paragraph.EndTime.TotalMilliseconds - currentPosition)))
            {
                if (closestStart.Paragraph.StartTime.IsMaxTime)
                {
                    return;
                }

                found = closestStart.Paragraph;
                foundSeconds = closestStart.Paragraph.StartTime.TotalSeconds;
            }
            else if (closestEnd != null && (closestStart == null || Math.Abs(closestStart.Paragraph.StartTime.TotalMilliseconds - currentPosition) >
                         Math.Abs(closestEnd.Paragraph.EndTime.TotalMilliseconds - currentPosition)))
            {
                if (closestEnd.Paragraph.EndTime.IsMaxTime)
                {
                    return;
                }

                found = closestEnd.Paragraph;
                foundSeconds = closestEnd.Paragraph.EndTime.TotalSeconds;
            }

            if (found == null)
            {
                return;
            }

            mediaPlayer.CurrentPosition = foundSeconds;
            SelectListViewIndexAndEnsureVisible(_subtitle.Paragraphs.IndexOf(found));

            Application.DoEvents();
            if (audioVisualizer.WavePeaks != null && foundSeconds < audioVisualizer.StartPositionSeconds + 0.15)
            {
                audioVisualizer.StartPositionSeconds -= 0.15;
            }
        }

        private void AutoGuessStartTime(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p == null)
            {
                return;
            }

            var silenceLengthInSeconds = 0.08;
            var lowPercent = audioVisualizer.FindLowPercentage(p.StartTime.TotalSeconds - 0.3, p.StartTime.TotalSeconds + 0.1);
            var highPercent = audioVisualizer.FindHighPercentage(p.StartTime.TotalSeconds - 0.3, p.StartTime.TotalSeconds + 0.4);
            var add = 5.0;
            if (highPercent > 40)
            {
                add = 8;
            }
            else if (highPercent < 5)
            {
                add = highPercent - lowPercent - 0.3;
            }

            for (var startVolume = lowPercent + add; startVolume < 14; startVolume += 0.3)
            {
                var pos = audioVisualizer.FindDataBelowThresholdBackForStart(startVolume, silenceLengthInSeconds, p.StartTime.TotalSeconds);
                var pos2 = audioVisualizer.FindDataBelowThresholdBackForStart(startVolume + 0.3, silenceLengthInSeconds, p.StartTime.TotalSeconds);
                if (pos >= 0 && pos > p.StartTime.TotalSeconds - 1)
                {
                    if (pos2 > pos && pos2 >= 0 && pos2 > p.StartTime.TotalSeconds - 1)
                    {
                        pos = pos2;
                    }

                    var newStartTimeMs = pos * TimeCode.BaseUnit;
                    var prev = _subtitle.GetParagraphOrDefault(index - 1);
                    if (prev != null && prev.EndTime.TotalMilliseconds + MinGapBetweenLines >= newStartTimeMs)
                    {
                        newStartTimeMs = prev.EndTime.TotalMilliseconds + MinGapBetweenLines;
                        if (newStartTimeMs >= p.StartTime.TotalMilliseconds)
                        {
                            break; // cannot move start time
                        }
                    }

                    // check for shot changes
                    if (audioVisualizer.ShotChanges != null)
                    {
                        var matchingShotChanges = audioVisualizer.ShotChanges
                            .Where(sc => sc > p.StartTime.TotalSeconds - 0.3 && sc < p.StartTime.TotalSeconds + 0.2)
                            .OrderBy(sc => Math.Abs(sc - p.StartTime.TotalSeconds));
                        if (matchingShotChanges.Any())
                        {
                            newStartTimeMs = matchingShotChanges.First() * TimeCode.BaseUnit;
                        }
                    }

                    if (Math.Abs(p.StartTime.TotalMilliseconds - newStartTimeMs) < 10)
                    {
                        break; // diff too small
                    }

                    var newEndTimeMs = p.EndTime.TotalMilliseconds;
                    if (newStartTimeMs > p.StartTime.TotalMilliseconds)
                    {
                        var temp = new Paragraph(p);
                        temp.StartTime.TotalMilliseconds = newStartTimeMs;
                        if (temp.DurationTotalMilliseconds < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds ||
                            Utilities.GetCharactersPerSecond(temp) > Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds)
                        {
                            var next = _subtitle.GetParagraphOrDefault(index + 1);
                            if (next == null ||
                                next.StartTime.TotalMilliseconds > newStartTimeMs + p.DurationTotalMilliseconds + MinGapBetweenLines)
                            {
                                newEndTimeMs = newStartTimeMs + p.DurationTotalMilliseconds;
                            }
                        }
                    }

                    MakeHistoryForUndo(string.Format(LanguageSettings.Current.Main.BeforeX, LanguageSettings.Current.Settings.WaveformGuessStart));

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            original.StartTime.TotalMilliseconds = newStartTimeMs;
                            original.EndTime.TotalMilliseconds = newEndTimeMs;
                        }
                    }

                    p.StartTime.TotalMilliseconds = newStartTimeMs;
                    p.EndTime.TotalMilliseconds = newEndTimeMs;
                    RefreshSelectedParagraph();
                    SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                    break;
                }
            }
        }

        private void GoToBookmark()
        {
            using (var form = new BookmarksGoTo(_subtitle))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    SelectListViewIndexAndEnsureVisible(form.BookmarkIndex);
                    if (mediaPlayer.VideoPlayer != null)
                    {
                        mediaPlayer.CurrentPosition = _subtitle.Paragraphs[form.BookmarkIndex].StartTime.TotalSeconds;
                    }
                }
            }
        }

        private void GoToPrevoiusBookmark()
        {
            int idx = FirstSelectedIndex - 1;
            try
            {
                for (int i = idx; i >= 0; i--)
                {
                    var p = _subtitle.Paragraphs[i];
                    if (p.Bookmark != null)
                    {
                        SelectListViewIndexAndEnsureVisible(i);
                        if (mediaPlayer.VideoPlayer != null)
                        {
                            mediaPlayer.CurrentPosition = _subtitle.Paragraphs[i].StartTime.TotalSeconds;
                        }

                        return;
                    }
                }
            }
            catch
            {
            }
        }

        private void GoToNextBookmark()
        {
            int idx = FirstSelectedIndex + 1;
            try
            {
                for (int i = idx; i < _subtitle.Paragraphs.Count; i++)
                {
                    var p = _subtitle.Paragraphs[i];
                    if (p.Bookmark != null)
                    {
                        SelectListViewIndexAndEnsureVisible(i);
                        if (mediaPlayer.VideoPlayer != null)
                        {
                            mediaPlayer.CurrentPosition = _subtitle.Paragraphs[i].StartTime.TotalSeconds;
                        }

                        return;
                    }
                }
            }
            catch
            {
            }
        }

        public void ToggleBookmarks(bool setText, Form parentForm)
        {
            bool first = true;
            string newValue = null;
            if (setText)
            {
                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.ToggleBookmarksWithComment));
            }
            else
            {
                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.ToggleBookmarks));
            }

            foreach (int index in SubtitleListview1.SelectedIndices)
            {
                var p = _subtitle.Paragraphs[index];
                if (first)
                {
                    if (p.Bookmark == null)
                    {
                        if (setText)
                        {
                            using (var form = new BookmarkAdd(p))
                            {
                                var result = form.ShowDialog(parentForm);
                                if (result != DialogResult.OK)
                                {
                                    return;
                                }

                                newValue = form.Comment;
                            }
                        }
                        else
                        {
                            newValue = string.Empty;
                        }
                    }
                    else
                    {
                        newValue = null;
                    }

                    first = false;
                }

                p.Bookmark = newValue;
                SubtitleListview1.ShowState(index, p);
                ShowHideBookmark(p);
            }

            SetListViewStateImages();
            new BookmarkPersistence(_subtitle, _fileName).Save();
        }

        private void SetListViewStateImages()
        {
            var oldStateImageList = SubtitleListview1.StateImageList;
            SubtitleListview1.StateImageList = _subtitle != null && _subtitle.Paragraphs.Any(p => p.Bookmark != null) ? imageListBookmarks : null;
            if (SubtitleListview1.StateImageList == null)
            {
                SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Text = LanguageSettings.Current.General.NumberSymbol;
            }
            else
            {
                SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Text = "    " + LanguageSettings.Current.General.NumberSymbol;
            }

            if (oldStateImageList == SubtitleListview1.StateImageList)
            {
                return;
            }

            if (!_loading)
            {
                if (SubtitleListview1.StateImageList == null)
                {
                    SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Width = Configuration.Settings.General.ListViewNumberWidth - 18;
                }
                else
                {
                    SubtitleListview1.Columns[SubtitleListview1.ColumnIndexNumber].Width = Configuration.Settings.General.ListViewNumberWidth + 18;
                }
            }

            SubtitleListview1.SubtitleListViewLastColumnFill(null, null);
        }

        private void ClearBookmarks()
        {
            MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Settings.ClearBookmarks));
            for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
            {
                var paragraph = _subtitle.Paragraphs[index];
                if (paragraph.Bookmark != null)
                {
                    paragraph.Bookmark = null;
                    SubtitleListview1.ShowState(index, paragraph);
                }
            }

            var p = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex);
            if (p != null)
            {
                ShowHideBookmark(p);
            }

            SetListViewStateImages();
            new BookmarkPersistence(_subtitle, _fileName).Save();
        }

        private void MoveTextFromCursorToNext(SETextBox tb)
        {
            int firstIndex = FirstSelectedIndex;
            if (firstIndex < 0)
            {
                return;
            }

            var p = _subtitle.GetParagraphOrDefault(firstIndex);
            var next = _subtitle.GetParagraphOrDefault(firstIndex + 1);
            if (tb == textBoxListViewTextOriginal)
            {
                p = Utilities.GetOriginalParagraph(firstIndex, p, _subtitleOriginal.Paragraphs);
                next = Utilities.GetOriginalParagraph(firstIndex + 1, next, _subtitleOriginal.Paragraphs);
            }

            if (p == null)
            {
                return;
            }

            MakeHistoryForUndo(_language.BeforeLineUpdatedInListView);

            if (next == null)
            {
                _makeHistoryPaused = true;
                InsertAfter(string.Empty, false);
                _makeHistoryPaused = false;
                next = _subtitle.GetParagraphOrDefault(firstIndex + 1);
                if (tb == textBoxListViewTextOriginal)
                {
                    next = Utilities.GetOriginalParagraph(firstIndex + 1, next, _subtitleOriginal.Paragraphs);
                }
            }

            var text1 = string.Empty;
            var text2 = p.Text;

            if (tb.SelectionStart > 0)
            {
                text1 = p.Text.Substring(0, Math.Min(p.Text.Length, tb.SelectionStart)).Trim();
                text2 = p.Text.Remove(0, Math.Min(p.Text.Length, tb.SelectionStart)).Trim();
            }

            p.Text = text1;
            next.Text = (text2 + Environment.NewLine + next.Text.Trim()).Trim();
            tb.Text = p.Text;
            if (tb == textBoxListViewTextOriginal)
            {
                SubtitleListview1.SetOriginalText(firstIndex, p.Text);
                SubtitleListview1.SetOriginalText(firstIndex + 1, next.Text);
            }
            else
            {
                SubtitleListview1.SetText(firstIndex, p.Text);
                SubtitleListview1.SetText(firstIndex + 1, next.Text);
            }

            if (firstIndex + 1 < _subtitle.Paragraphs.Count)
            {
                _subtitleListViewIndex = -1;
                var index = firstIndex + 1;
                SelectListViewIndexAndEnsureVisible(index);
                if (mediaPlayer.IsPaused && index < _subtitle.Paragraphs.Count)
                {
                    mediaPlayer.CurrentPosition = _subtitle.Paragraphs[index].StartTime.TotalSeconds;
                }
            }
        }

        private void MoveWordUpDownInCurrent(bool down, SETextBox tb)
        {
            int firstIndex = FirstSelectedIndex;
            if (firstIndex < 0)
            {
                return;
            }

            var p = _subtitle.GetParagraphOrDefault(firstIndex);
            if (tb == textBoxListViewTextOriginal)
            {
                p = Utilities.GetOriginalParagraph(firstIndex, p, _subtitleOriginal.Paragraphs);
            }

            if (p == null)
            {
                return;
            }

            var lines = p.Text.SplitToLines();
            if (lines.Count == 1)
            {
                lines.Add(string.Empty);
            }

            if (lines.Count != 2)
            {
                return;
            }

            var line1Words = lines[0].Split(' ').ToList();
            var line2Words = lines[1].Split(' ').ToList();
            if (down)
            {
                if (line1Words.Count > 0)
                {
                    line2Words.Insert(0, line1Words[line1Words.Count - 1]);
                    line1Words.RemoveAt(line1Words.Count - 1);
                }
            }
            else // up
            {
                if (line2Words.Count > 0)
                {
                    line1Words.Add(line2Words[0]);
                    line2Words.RemoveAt(0);
                }
            }

            var newText = (string.Join(" ", line1Words.ToArray()).Trim() + Environment.NewLine +
                           string.Join(" ", line2Words.ToArray()).Trim()).Trim();
            if (newText != p.Text)
            {
                var oldText = p.Text;
                MakeHistoryForUndo(_language.BeforeLineUpdatedInListView);
                var textCaretPos = textBoxListViewText.SelectionStart;
                p.Text = newText;
                if (tb == textBoxListViewTextOriginal)
                {
                    SubtitleListview1.SetOriginalText(firstIndex, p.Text);
                }
                else
                {
                    SubtitleListview1.SetText(firstIndex, p.Text);
                }

                tb.Text = p.Text;

                // keep cursor position
                KeepCursorMoveWordUpdown(down, newText, oldText, textCaretPos);
            }
        }

        private void KeepCursorMoveWordUpdown(bool down, string newText, string oldText, int textCaretPos)
        {
            if (textCaretPos > textBoxListViewText.Text.Length)
            {
                // set cursor at end of textbox
                textBoxListViewText.SelectionStart = textCaretPos;
                int end = textBoxListViewText.Text.Length;
                textBoxListViewText.SelectionStart = end;
                textBoxListViewText.SelectionLength = 0;
                return;
            }

            int indexOfNewLine = newText.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            int oldIndexOfNewLine = oldText.IndexOf(Environment.NewLine, StringComparison.Ordinal);

            if (down)
            {
                if (indexOfNewLine == -1 && oldIndexOfNewLine > 0 && textCaretPos > oldIndexOfNewLine)
                {
                    textCaretPos--;
                }
                else if (textCaretPos > indexOfNewLine && textCaretPos > oldIndexOfNewLine && oldIndexOfNewLine >= 0 ||
                         textCaretPos < indexOfNewLine && (oldIndexOfNewLine == -1 || textCaretPos < oldIndexOfNewLine) ||
                         textCaretPos < oldIndexOfNewLine && indexOfNewLine == -1)
                {
                }
                else
                {
                    textCaretPos++;
                }
            }
            else // up
            {
                if (textCaretPos <= oldIndexOfNewLine || textCaretPos > oldIndexOfNewLine && textCaretPos > indexOfNewLine && indexOfNewLine >= 0)
                {
                }
                else
                {
                    textCaretPos--;
                }
            }

            if (textBoxListViewText.Text.Length > textCaretPos && '\n' == textBoxListViewText.Text[textCaretPos])
            {
                textCaretPos--;
            }

            if (textCaretPos >= 0)
            {
                textBoxListViewText.SelectionStart = textCaretPos;
                textBoxListViewText.SelectionStart = textCaretPos;
                textBoxListViewText.SelectionStart = textCaretPos;
            }
        }

        private void MoveStartCurrent(int ms, bool keepGapPrevIfClose)
        {
            StopAutoDuration();
            var i = _subtitleListViewIndex;
            if (i < 0 || i >= _subtitle.Paragraphs.Count || ms == 0)
            {
                return;
            }

            var p = _subtitle.GetParagraphOrDefault(i);
            if (p == null)
            {
                return;
            }

            // snap to shot change
            if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && audioVisualizer?.ShotChanges?.Count > 0)
            {
                var seconds = (p.StartTime.TotalMilliseconds + ms) / 1000.0;
                var closest = audioVisualizer.ShotChanges.OrderBy(sc => Math.Abs(seconds - sc)).First() * 1000.0;
                if (Math.Abs(p.StartTime.TotalMilliseconds + ms - closest) < CurrentFrameRate * 0.9)
                {
                    ms = (int)Math.Round(closest - p.StartTime.TotalMilliseconds);
                }
            }

            var prevGap = 0.0;
            var prev = _subtitle.GetParagraphOrDefault(i - 1);
            var isClose = false;
            if (keepGapPrevIfClose && prev != null)
            {
                if (prev.EndTime.TotalMilliseconds <= p.StartTime.TotalMilliseconds && prev.EndTime.TotalMilliseconds + MinGapBetweenLines >= p.StartTime.TotalMilliseconds)
                {
                    isClose = true;
                    prevGap = p.StartTime.TotalMilliseconds - prev.EndTime.TotalMilliseconds;
                    if (ms < 0 && prev.DurationTotalMilliseconds + ms < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                    {
                        return;
                    }
                }
            }

            if (ms > 0)
            {
                if (p.StartTime.TotalMilliseconds + ms + Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds > p.EndTime.TotalMilliseconds)
                {
                    return; // do not allow duration smaller than min duration in ms
                }

                p.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + ms;
            }
            else
            {
                if (p.DurationTotalMilliseconds + Math.Abs(ms) > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    return;
                }

                if (p.StartTime.TotalMilliseconds + ms < 0)
                {
                    return;
                }

                if (prev == null || keepGapPrevIfClose && isClose || p.StartTime.TotalMilliseconds - (Math.Abs(ms) + MinGapBetweenLines) > prev.EndTime.TotalMilliseconds)
                {
                    p.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + ms;
                }
                else
                {
                    var newStartMs = prev.EndTime.TotalMilliseconds + MinGapBetweenLines;
                    if (newStartMs < p.StartTime.TotalMilliseconds)
                    {
                        p.StartTime.TotalMilliseconds = newStartMs;
                    }
                }
            }

            SubtitleListview1.SetStartTimeAndDuration(i, p, _subtitle.GetParagraphOrDefault(i + 1), _subtitle.GetParagraphOrDefault(i - 1));
            timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
            timeUpDownStartTime.TimeCode = p.StartTime;
            timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
            SetDurationInSeconds(p.DurationTotalSeconds);

            if (keepGapPrevIfClose && isClose && prev != null)
            {
                prev.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds - prevGap;
                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(_subtitle.GetIndex(prev), prev, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        original.EndTime.TotalMilliseconds = prev.EndTime.TotalMilliseconds;
                    }
                }

                SubtitleListview1.SetStartTimeAndDuration(i - 1, prev, p, _subtitle.GetParagraphOrDefault(i - 2));
            }

            if (IsOriginalEditable)
            {
                var original = Utilities.GetOriginalParagraph(_subtitle.GetIndex(p), p, _subtitleOriginal.Paragraphs);
                if (original != null)
                {
                    original.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds;
                    original.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds;
                }
            }

            SubtitleListview1.SyntaxColorLineBackground(_subtitle.Paragraphs, i, p);
            UpdateSourceView();
            audioVisualizer.Invalidate();
        }

        private void MoveEndCurrent(int ms, bool keepGapNextIfClose)
        {
            StopAutoDuration();
            var i = _subtitleListViewIndex;
            if (i < 0 || i >= _subtitle.Paragraphs.Count || ms == 0)
            {
                return;
            }

            var p = _subtitle.GetParagraphOrDefault(i);
            if (p == null)
            {
                return;
            }

            // snap to shot change
            if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && audioVisualizer?.ShotChanges?.Count > 0)
            {
                var seconds = (p.EndTime.TotalMilliseconds + ms) / 1000.0;
                var closest = audioVisualizer.ShotChanges.OrderBy(sc => Math.Abs(seconds - sc)).First() * 1000.0;
                if (Math.Abs(p.EndTime.TotalMilliseconds + ms - closest) < CurrentFrameRate * 0.9)
                {
                    ms = (int)Math.Round(closest - p.EndTime.TotalMilliseconds);
                }
            }

            var nextGap = 0.0;
            var next = _subtitle.GetParagraphOrDefault(i + 1);
            var isClose = false;
            if (keepGapNextIfClose && next != null)
            {
                if (p.EndTime.TotalMilliseconds <= next.StartTime.TotalMilliseconds && p.EndTime.TotalMilliseconds + MinGapBetweenLines >= next.StartTime.TotalMilliseconds)
                {
                    isClose = true;
                    nextGap = next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds;
                    if (ms > 0 && next.DurationTotalMilliseconds + ms < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                    {
                        return;
                    }
                }
            }

            if (ms > 0)
            {
                if (p.DurationTotalMilliseconds + ms > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    return;
                }

                if (next == null || keepGapNextIfClose && isClose || p.EndTime.TotalMilliseconds + ms + MinGapBetweenLines < next.StartTime.TotalMilliseconds)
                {
                    p.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds + ms;
                }
                else
                {
                    var newEndMs = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                    if (newEndMs > p.EndTime.TotalMilliseconds)
                    {
                        p.EndTime.TotalMilliseconds = newEndMs;
                    }
                }
            }
            else
            {
                if (p.EndTime.TotalMilliseconds + ms - Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds < p.StartTime.TotalMilliseconds)
                {
                    return; // do not allow duration smaller than min duration in ms
                }

                p.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds + ms;
            }

            SubtitleListview1.SetStartTimeAndDuration(i, p, _subtitle.GetParagraphOrDefault(i + 1), _subtitle.GetParagraphOrDefault(i - 1));
            timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
            timeUpDownStartTime.TimeCode = p.StartTime;
            timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
            SetDurationInSeconds(p.DurationTotalSeconds);

            if (keepGapNextIfClose && isClose && next != null)
            {
                next.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds + nextGap;
                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(_subtitle.GetIndex(next), next, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        original.StartTime.TotalMilliseconds = next.StartTime.TotalMilliseconds;
                    }
                }

                SubtitleListview1.SetStartTimeAndDuration(i + 1, next, _subtitle.GetParagraphOrDefault(i + 2), p);
            }

            if (IsOriginalEditable)
            {
                var original = Utilities.GetOriginalParagraph(_subtitle.GetIndex(p), p, _subtitleOriginal.Paragraphs);
                if (original != null)
                {
                    original.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds;
                    original.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds;
                }
            }

            SubtitleListview1.SyntaxColorLineBackground(_subtitle.Paragraphs, i, p);
            UpdateSourceView();
            audioVisualizer.Invalidate();
        }

        private void ShowNextSubtitleLabel()
        {
            if (audioVisualizer.Visible && audioVisualizer.WavePeaks != null && audioVisualizer.Width > 300 && _subtitleListViewIndex >= 0)
            {
                var next = _subtitle.GetParagraphOrDefault(_subtitleListViewIndex + 1);
                if (next != null && !string.IsNullOrEmpty(next.Text))
                {
                    labelNextWord.Top = audioVisualizer.Top;
                    labelNextWord.Text = string.Format(_language.NextX, HtmlUtil.RemoveHtmlTags(next.Text, true).Replace(Environment.NewLine, " "));
                    labelNextWord.Left = audioVisualizer.Width / 2 - labelNextWord.Width / 2 + audioVisualizer.Left;
                    labelNextWord.Visible = true;
                    _labelNextTicks = DateTime.UtcNow.Ticks;
                }
                else
                {
                    labelNextWord.Visible = false;
                }
            }
            else
            {
                labelNextWord.Visible = false;
            }
        }

        private void MergeSelectedLinesOnlyFirstText()
        {
            if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 1)
            {
                var deleteIndices = new List<int>();
                bool first = true;
                int firstIndex = 0;
                int next = 0;
                string text = string.Empty;
                double endTime = 0;
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    if (first)
                    {
                        firstIndex = index;
                        next = index + 1;
                    }
                    else
                    {
                        deleteIndices.Add(index);
                        if (next != index)
                        {
                            return;
                        }

                        next++;
                    }

                    first = false;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = _subtitle.Paragraphs[index].Text.Trim();
                    }

                    endTime = _subtitle.Paragraphs[index].EndTime.TotalMilliseconds;
                }

                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(_language.BeforeMergeLines);

                var currentParagraph = _subtitle.Paragraphs[firstIndex];
                currentParagraph.Text = text;
                currentParagraph.EndTime.TotalMilliseconds = endTime;

                var nextParagraph = _subtitle.GetParagraphOrDefault(next);
                if (nextParagraph != null && currentParagraph.EndTime.TotalMilliseconds > nextParagraph.StartTime.TotalMilliseconds && currentParagraph.StartTime.TotalMilliseconds < nextParagraph.StartTime.TotalMilliseconds)
                {
                    currentParagraph.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
                }

                // original subtitle
                if (IsOriginalEditable)
                {
                    var original = Utilities.GetOriginalParagraph(firstIndex, currentParagraph, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        string originalText = string.Empty;
                        for (int i = 0; i < deleteIndices.Count; i++)
                        {
                            var originalNext = Utilities.GetOriginalParagraph(deleteIndices[i], _subtitle.Paragraphs[deleteIndices[i]], _subtitleOriginal.Paragraphs);
                            if (originalNext != null && string.IsNullOrEmpty(originalText))
                            {
                                originalText = originalNext.Text;
                            }
                        }

                        for (int i = deleteIndices.Count - 1; i >= 0; i--)
                        {
                            var originalNext = Utilities.GetOriginalParagraph(deleteIndices[i], _subtitle.Paragraphs[deleteIndices[i]], _subtitleOriginal.Paragraphs);
                            if (originalNext != null)
                            {
                                _subtitleOriginal.Paragraphs.Remove(originalNext);
                            }
                        }

                        original.Text = originalText;
                        original.EndTime.TotalMilliseconds = currentParagraph.EndTime.TotalMilliseconds;
                        _subtitleOriginal.Renumber();
                    }
                }

                if (_networkSession != null)
                {
                    _networkSession.TimerStop();
                    _networkSession.UpdateLine(firstIndex, currentParagraph);
                    NetworkGetSendUpdates(deleteIndices, 0, null);
                }
                else
                {
                    for (int i = deleteIndices.Count - 1; i >= 0; i--)
                    {
                        _subtitle.Paragraphs.RemoveAt(deleteIndices[i]);
                    }

                    _subtitle.Renumber();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                }

                UpdateSourceView();
                ShowStatus(_language.LinesMerged);
                SubtitleListview1.SelectIndexAndEnsureVisible(firstIndex, true);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                RefreshSelectedParagraph();
            }
        }

        private void GoToFirstEmptyLine()
        {
            var index = FirstSelectedIndex + 1;
            for (; index < _subtitle.Paragraphs.Count; index++)
            {
                if (string.IsNullOrWhiteSpace(_subtitle.Paragraphs[index].Text))
                {
                    SelectListViewIndexAndEnsureVisible(index);
                    if (mediaPlayer.VideoPlayer != null)
                    {
                        mediaPlayer.CurrentPosition = _subtitle.Paragraphs[index].StartTime.TotalSeconds;
                    }

                    return;
                }
            }
        }

        private void PlayFirstSelectedSubtitle()
        {
            if (_subtitleListViewIndex >= 0 && mediaPlayer.VideoPlayer != null)
            {
                GotoSubtitleIndex(_subtitleListViewIndex);
                var paragraph = _subtitle.Paragraphs[_subtitleListViewIndex];
                double startSeconds = paragraph.StartTime.TotalSeconds;
                ResetPlaySelection();
                _endSeconds = paragraph.EndTime.TotalSeconds;
            }
        }

        private void MoveVideoSeconds(double seconds)
        {
            var oldPosition = mediaPlayer.CurrentPosition;
            var newPosition = oldPosition + seconds;
            if (newPosition < 0)
            {
                newPosition = 0;
            }

            if (mediaPlayer.IsPaused && Configuration.Settings.General.MoveVideo100Or500MsPlaySmallSample)
            {
                mediaPlayer.CurrentPosition = newPosition;
                mediaPlayer.Play();
                Thread.Sleep(99);
                mediaPlayer.Stop();
            }

            mediaPlayer.CurrentPosition = newPosition;
        }

        private void RunCustomSearch(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                RunTranslateSearch((text) =>
                {
                    url = string.Format(url, Utilities.UrlEncode(text));
                    UiUtil.OpenUrl(url);
                });
            }
        }

        private void GoFullscreen(bool force)
        {
            if (_videoPlayerUndocked != null && Configuration.Settings.General.Undocked)
            {
                _videoPlayerUndocked.WindowState = FormWindowState.Maximized;
                return;
            }

            if (mediaPlayer.VideoPlayer == null)
            {
                return;
            }

            _textHeightResize = splitContainerListViewAndText.Height - splitContainerListViewAndText.SplitterDistance;
            _textHeightResizeIgnoreUpdate = DateTime.UtcNow.Ticks;
            mediaPlayer.ShowFullScreenControls();
            bool setRedockOnFullscreenEnd = false;

            if (_videoPlayerUndocked == null || _videoPlayerUndocked.IsDisposed)
            {
                Configuration.Settings.General.Undocked = true;
                UnDockVideoPlayer();
                setRedockOnFullscreenEnd = true;
            }

            if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed)
            {
                _videoPlayerUndocked.Show(this);
                _videoPlayerUndocked.Focus();
                _videoPlayerUndocked.GoFullscreen();
                if (setRedockOnFullscreenEnd)
                {
                    _videoPlayerUndocked.RedockOnFullscreenEnd = true;
                }
            }
        }

        private void RefreshTimeCodeMode()
        {
            if (Configuration.Settings.General.UseTimeFormatHHMMSSFF)
            {
                numericUpDownDuration.DecimalPlaces = 2;
                numericUpDownDuration.Increment = (decimal)(0.01);

                toolStripSeparatorFrameRate.Visible = true;
                toolStripLabelFrameRate.Visible = true;
                toolStripComboBoxFrameRate.Visible = true;
                toolStripButtonGetFrameRate.Visible = true;
            }
            else
            {
                numericUpDownDuration.DecimalPlaces = 3;
                numericUpDownDuration.Increment = (decimal)0.1;

                toolStripSeparatorFrameRate.Visible = Configuration.Settings.General.ShowFrameRate;
                toolStripLabelFrameRate.Visible = Configuration.Settings.General.ShowFrameRate;
                toolStripComboBoxFrameRate.Visible = Configuration.Settings.General.ShowFrameRate;
                toolStripButtonGetFrameRate.Visible = Configuration.Settings.General.ShowFrameRate;
            }

            SaveSubtitleListviewIndices();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            RestoreSubtitleListviewIndices();
            RefreshSelectedParagraph();
        }

        private void ReverseStartAndEndingForRtl()
        {
            MakeHistoryForUndo(toolStripMenuItemReverseRightToLeftStartEnd.Text);
            int selectedIndex = FirstSelectedIndex;
            foreach (int index in SubtitleListview1.SelectedIndices)
            {
                var p = _subtitle.Paragraphs[index];
                p.Text = Utilities.ReverseStartAndEndingForRightToLeft(p.Text);
                SubtitleListview1.SetText(index, p.Text);
                if (index == selectedIndex)
                {
                    textBoxListViewText.Text = p.Text;
                }
            }
        }

        private void MergeDialogs()
        {
            if (SubtitleListview1.SelectedItems.Count == 1 || SubtitleListview1.SelectedItems.Count == 2 && SubtitleListview1.SelectedIndices[0] + 1 == SubtitleListview1.SelectedIndices[1])
            {
                MergeWithLineAfter(true);
            }
        }

        private void ToggleDashes()
        {
            var index = FirstSelectedIndex;
            if (index >= 0)
            {
                var hasStartDash = false;
                var p = _subtitle.Paragraphs[index];
                var lines = p.Text.SplitToLines();
                foreach (var line in lines)
                {
                    var trimmed = HtmlUtil.RemoveHtmlTags(line, true).TrimStart();
                    if (trimmed.StartsWith('-'))
                    {
                        hasStartDash = true;
                        break;
                    }
                }

                if (!hasStartDash && _subtitleOriginal != null && textBoxListViewTextOriginal.Visible)
                {
                    var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        lines = original.Text.SplitToLines();
                        foreach (var line in lines)
                        {
                            var trimmed = HtmlUtil.RemoveHtmlTags(line, true).TrimStart();
                            if (trimmed.StartsWith('-'))
                            {
                                hasStartDash = true;
                                break;
                            }
                        }
                    }
                }

                MakeHistoryForUndo(_language.BeforeToggleDialogDashes);
                if (hasStartDash)
                {
                    RemoveDashes();
                }
                else
                {
                    AddDashes();
                }
            }
        }

        private void ToggleDashesTextBox(SETextBox tb)
        {
            var hasStartDash = false;
            var lines = tb.Text.TrimEnd().SplitToLines();
            foreach (var line in lines)
            {
                var trimmed = HtmlUtil.RemoveHtmlTags(line, true).TrimStart();
                if (trimmed.StartsWith('-'))
                {
                    hasStartDash = true;
                    break;
                }
            }

            MakeHistoryForUndo(_language.BeforeToggleDialogDashes);
            var sb = new StringBuilder();
            if (hasStartDash)
            {
                // remove dashes
                foreach (var line in lines)
                {
                    var pre = string.Empty;
                    var s = Utilities.SplitStartTags(line, ref pre);
                    sb.Append(pre).AppendLine(s.TrimStart('-').TrimStart());
                }

                tb.Text = sb.ToString().Trim();
            }
            else
            {
                // add dashes
                if (CouldBeDialog(lines))
                {
                    foreach (var line in lines)
                    {
                        var pre = string.Empty;
                        var s = Utilities.SplitStartTags(line, ref pre);
                        sb.Append(pre).Append("- ").AppendLine(s);
                    }
                }
                else
                {
                    sb.Append(tb.Text);
                }

                var text = sb.ToString().Trim();
                var dialogHelper = new DialogSplitMerge { DialogStyle = Configuration.Settings.General.DialogStyle, SkipLineEndingCheck = true };
                text = dialogHelper.FixDashesAndSpaces(text);
                tb.Text = text;
            }
        }

        private void AddDashes()
        {
            var dialogHelper = new DialogSplitMerge { DialogStyle = Configuration.Settings.General.DialogStyle, SkipLineEndingCheck = true };
            foreach (int index in SubtitleListview1.SelectedIndices)
            {
                var p = _subtitle.Paragraphs[index];
                var lines = p.Text.SplitToLines();
                var sb = new StringBuilder();
                if (CouldBeDialog(lines))
                {
                    foreach (var line in lines)
                    {
                        var pre = string.Empty;
                        var s = Utilities.SplitStartTags(line, ref pre);
                        sb.Append(pre).Append("- ").AppendLine(s);
                    }
                }
                else
                {
                    sb.Append(p.Text);
                }

                var text = sb.ToString().Trim();
                text = dialogHelper.FixDashesAndSpaces(text);
                _subtitle.Paragraphs[index].Text = text;
                SubtitleListview1.SetText(index, text);
                if (index == _subtitleListViewIndex)
                {
                    textBoxListViewText.Text = text;
                }

                if (_subtitleOriginal != null && textBoxListViewTextOriginal.Visible)
                {
                    var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        lines = original.Text.SplitToLines();
                        sb = new StringBuilder();
                        if (CouldBeDialog(lines))
                        {
                            foreach (var line in lines)
                            {
                                var pre = string.Empty;
                                var s = Utilities.SplitStartTags(line, ref pre);
                                if (!line.StartsWith('-'))
                                {
                                    sb.Append(pre).Append("- ").AppendLine(s);
                                }
                                else
                                {
                                    sb.Append(pre).AppendLine(s);
                                }
                            }
                        }
                        else
                        {
                            sb.Append(original.Text);
                        }

                        text = sb.ToString().Trim();
                        text = dialogHelper.FixDashesAndSpaces(text);
                        _subtitleOriginal.Paragraphs[index].Text = text;
                        SubtitleListview1.SetOriginalText(index, text);
                        if (index == _subtitleListViewIndex)
                        {
                            textBoxListViewTextOriginal.Text = text;
                        }
                    }
                }
            }
        }

        private static bool CouldBeDialog(List<string> lines)
        {
            return lines.Count >= 2 && lines.Count <= 3;
        }

        private void RemoveDashes()
        {
            foreach (int index in SubtitleListview1.SelectedIndices)
            {
                var p = _subtitle.Paragraphs[index];
                var lines = p.Text.SplitToLines();
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var pre = string.Empty;
                    var s = Utilities.SplitStartTags(line, ref pre);
                    sb.Append(pre).AppendLine(s.TrimStart('-').TrimStart());
                }

                string text = sb.ToString().Trim();
                _subtitle.Paragraphs[index].Text = text;
                SubtitleListview1.SetText(index, text);
                if (index == _subtitleListViewIndex)
                {
                    textBoxListViewText.Text = text;
                }

                if (_subtitleOriginal != null && textBoxListViewTextOriginal.Visible)
                {
                    var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        lines = original.Text.SplitToLines();
                        sb = new StringBuilder();
                        foreach (var line in lines)
                        {
                            var pre = string.Empty;
                            var s = Utilities.SplitStartTags(line, ref pre);
                            sb.Append(pre).AppendLine(s.TrimStart('-').TrimStart());
                        }

                        text = sb.ToString().Trim();
                        _subtitleOriginal.Paragraphs[index].Text = text;
                        SubtitleListview1.SetOriginalText(index, text);
                        if (index == _subtitleListViewIndex)
                        {
                            textBoxListViewTextOriginal.Text = text;
                        }
                    }
                }
            }
        }

        private void SetTitle()
        {
            var text = "Untitled";
            string separator = " + ";
            if (!string.IsNullOrEmpty(_fileName))
            {
                text = Configuration.Settings.General.TitleBarFullFileName ? _fileName : Path.GetFileName(_fileName);
            }

            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                text += separator;

                if (!string.IsNullOrEmpty(_subtitleOriginalFileName))
                {
                    text += Configuration.Settings.General.TitleBarFullFileName ? _subtitleOriginalFileName : Path.GetFileName(_subtitleOriginalFileName);
                }
                else
                {
                    text += _language.New;
                }
            }

            Text = text + " - " + Title;
        }

        private void ClipboardSetText(string text)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(text);
                    return;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }

            MessageBox.Show("Unable to set clipboard text - some other application might have locked the clipboard.");
        }




        private void SetAlignment(string tag, bool selectedLines)
        {
            if (selectedLines)
            {
                var indices = SubtitleListview1.GetSelectedIndices();
                if (indices.Length == 0)
                {
                    return;
                }

                SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                MakeHistoryForUndo(string.Format(_language.BeforeAddingTagX, tag));

                bool first = true;
                SubtitleListview1.BeginUpdate();
                foreach (int i in indices)
                {
                    if (first)
                    {
                        if (_subtitle.Paragraphs[i].Text.StartsWith(tag, StringComparison.Ordinal))
                        {
                            tag = string.Empty;
                        }

                        if (_subtitle.Paragraphs[i].Text.StartsWith(tag.Replace("}", "\\"), StringComparison.Ordinal))
                        {
                            tag = string.Empty;
                        }

                        first = false;
                    }

                    if (IsOriginalEditable && SubtitleListview1.IsOriginalTextColumnVisible)
                    {
                        var original = Utilities.GetOriginalParagraph(i, _subtitle.Paragraphs[i], _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            original.Text = SetAlignTag(original.Text, tag);
                            SubtitleListview1.SetOriginalText(i, original.Text);
                        }
                    }

                    _subtitle.Paragraphs[i].Text = SetAlignTag(_subtitle.Paragraphs[i].Text, tag);
                    SubtitleListview1.SetText(i, _subtitle.Paragraphs[i].Text);
                }

                SubtitleListview1.EndUpdate();

                ShowStatus(string.Format(_language.TagXAdded, tag));
                UpdateSourceView();
                RefreshSelectedParagraph();
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            }
            else
            {
                var tb = GetFocusedTextBox();
                var pos = tb.SelectionStart;
                int oldLength = tb.Text.Length;
                bool atEnd = pos == oldLength;
                tb.Text = SetAlignTag(tb.Text, tag);
                if (atEnd)
                {
                    tb.SelectionStart = tb.Text.Length;
                }
                else if (pos == 0)
                {
                    tb.SelectionStart = 0;
                }
                else if (oldLength == tb.Text.Length)
                {
                    tb.SelectionStart = pos;
                }
                else if (pos + 5 <= tb.Text.Length)
                {
                    tb.SelectionStart = pos + 5;
                }
            }
        }

        private void GoToNextSyntaxError()
        {
            int idx = FirstSelectedIndex + 1;
            try
            {
                for (int i = idx; i < _subtitle.Paragraphs.Count; i++)
                {
                    var item = SubtitleListview1.Items[i];
                    if (item.SubItems[SubtitleListview1.ColumnIndexDuration].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor ||
                        item.SubItems[SubtitleListview1.ColumnIndexText].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor ||
                        item.SubItems[SubtitleListview1.ColumnIndexStart].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor ||
                        (SubtitleListview1.ColumnIndexCps >= 0 && item.SubItems[SubtitleListview1.ColumnIndexCps].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor) ||
                        (SubtitleListview1.ColumnIndexWpm >= 0 && item.SubItems[SubtitleListview1.ColumnIndexWpm].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor) ||
                        (SubtitleListview1.ColumnIndexGap >= 0 && item.SubItems[SubtitleListview1.ColumnIndexGap].BackColor == Configuration.Settings.Tools.ListViewSyntaxErrorColor))
                    {
                        SelectListViewIndexAndEnsureVisible(i);
                        if (mediaPlayer.VideoPlayer != null)
                        {
                            mediaPlayer.CurrentPosition = _subtitle.Paragraphs[i].StartTime.TotalSeconds;
                        }

                        return;
                    }
                }
            }
            catch
            {
            }
        }


        private void RestartHistory()
        {
            _listViewTextUndoLast = null;
            _listViewTextUndoIndex = -1;
            _listViewTextTicks = -1;
            _listViewOriginalTextUndoLast = null;
            _listViewOriginalTextTicks = -1;
            _undoIndex = _subtitle.HistoryItems.Count - 1;
            _makeHistoryPaused = false;
        }


        private void CenterFormOnCurrentScreen()
        {
            var screen = Screen.FromControl(this);
            Left = screen.Bounds.X + ((screen.Bounds.Width - Width) / 2);
            Top = screen.Bounds.Y + ((screen.Bounds.Height - Height) / 2);
        }

        private void SortSubtitle(SubtitleSortCriteria subtitleSortCriteria, string description)
        {
            var firstSelectedIndex = 0;
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                firstSelectedIndex = SubtitleListview1.SelectedItems[0].Index;
            }

            _subtitleListViewIndex = -1;
            MakeHistoryForUndo(string.Format(_language.BeforeSortX, description));
            _subtitle.Sort(subtitleSortCriteria);
            if (descendingToolStripMenuItem.Checked)
            {
                _subtitle.Paragraphs.Reverse();
            }

            UpdateSourceView();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
            ShowStatus(string.Format(_language.SortedByX, description));
        }

        private void SetLanguage(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = "en-US";
            }

            if (cultureName != "en-US")
            {
                try
                {
                    LanguageSettings.Current = Language.Load(Path.Combine(Configuration.BaseDirectory, "Languages", cultureName + ".xml"));
                }
                catch (Exception ex)
                {
                    var cap = "Language file load error";
                    var msg = "Could not load language file " + cultureName + ".xml" +
                              "\n\nError Message:\n" + ex.Message +
                              "\n\nStack Trace:\n" + ex.StackTrace;
                    MessageBox.Show(this, msg, cap);
                    cultureName = "en-US";
                }
            }

            if (cultureName == "en-US")
            {
                LanguageSettings.Current = new Language(); // default is en-US
            }

            Configuration.Settings.General.Language = cultureName;
            _languageGeneral = LanguageSettings.Current.General;
            _language = LanguageSettings.Current.Main;
            InitializeLanguage();
        }

       

        private void ImportDvdSubtitle(string fileName)
        {
            using (var formSubRip = new DvdSubRip(Handle, fileName))
            {
                if (formSubRip.ShowDialog(this) == DialogResult.OK)
                {
                    using (var showSubtitles = new DvdSubRipChooseLanguage())
                    {
                        showSubtitles.Initialize(formSubRip.MergedVobSubPacks, formSubRip.Palette, formSubRip.Languages, formSubRip.SelectedLanguage);
                        if (formSubRip.Languages.Count == 1 || showSubtitles.ShowDialog(this) == DialogResult.OK)
                        {
                            using (var formSubOcr = new VobSubOcr())
                            {
                                var subs = formSubRip.MergedVobSubPacks;
                                if (showSubtitles.SelectedVobSubMergedPacks != null)
                                {
                                    subs = showSubtitles.SelectedVobSubMergedPacks;
                                }

                                formSubOcr.Initialize(subs, formSubRip.Palette, Configuration.Settings.VobSubOcr, formSubRip.SelectedLanguage);
                                if (formSubOcr.ShowDialog(this) == DialogResult.OK)
                                {
                                    MakeHistoryForUndo(_language.BeforeImportingDvdSubtitle);
                                    FileNew();
                                    _subtitle.Paragraphs.Clear();
                                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                                    foreach (var p in formSubOcr.SubtitleFromOcr.Paragraphs)
                                    {
                                        _subtitle.Paragraphs.Add(p);
                                    }

                                    UpdateSourceView();
                                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                                    _subtitleListViewIndex = -1;
                                    SubtitleListview1.FirstVisibleIndex = -1;
                                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);

                                    _fileName = string.Empty;
                                    SetTitle();

                                    Configuration.Settings.Save();
                                }
                            }
                        }
                    }
                }
            }
        }

       


        private bool IsUnicode
        {
            get
            {
                return enc == Encoding.UTF8 || enc == Encoding.Unicode || enc == Encoding.UTF7 || enc == Encoding.UTF32 || enc == Encoding.BigEndianUnicode;
            }
        }

        private void EditToolStripMenuItemDropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItemRemoveUnicodeControlChars.Visible = IsUnicode;
            toolStripMenuItemRtlUnicodeControlChars.Visible = IsUnicode;
            if (!IsUnicode || _subtitleListViewIndex == -1)
            {
                toolStripMenuItemInsertUnicodeCharacter.Visible = false;
                toolStripSeparatorInsertUnicodeCharacter.Visible = false;
            }
            else
            {
                if (toolStripMenuItemInsertUnicodeCharacter.DropDownItems.Count == 0)
                {
                    foreach (var s in Configuration.Settings.Tools.UnicodeSymbolsToInsert.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        toolStripMenuItemInsertUnicodeCharacter.DropDownItems.Add(s, null, InsertUnicodeGlyphAllowMultiLine);
                    }

                    UiUtil.FixFonts(toolStripMenuItemInsertUnicodeCharacter);
                }

                toolStripMenuItemInsertUnicodeCharacter.Visible = toolStripMenuItemInsertUnicodeCharacter.DropDownItems.Count > 0;
                toolStripSeparatorInsertUnicodeCharacter.Visible = toolStripMenuItemInsertUnicodeCharacter.DropDownItems.Count > 0;
            }

            lock (_syncUndo)
            {
                toolStripMenuItemUndo.Enabled = _subtitle != null && _subtitle.CanUndo && _undoIndex >= 0;
                toolStripMenuItemRedo.Enabled = _subtitle != null && _subtitle.CanUndo && _undoIndex < _subtitle.HistoryItems.Count - 1;
            }

            showHistoryforUndoToolStripMenuItem.Enabled = _subtitle != null && _subtitle.CanUndo;
            toolStripMenuItemShowOriginalInPreview.Visible = SubtitleListview1.IsOriginalTextColumnVisible;

            if (_networkSession != null)
            {
                toolStripMenuItemUndo.Enabled = false;
                toolStripMenuItemRedo.Enabled = false;
                showHistoryforUndoToolStripMenuItem.Enabled = false;
            }
        }

        private void InsertUnicodeGlyph(object sender, EventArgs e)
        {
            if (sender is ToolStripItem item)
            {
                PasteIntoActiveTextBox(item.Text);
            }
        }

        private void InsertUnicodeGlyphAllowMultiLine(object sender, EventArgs e)
        {
            if (sender is ToolStripItem item)
            {
                PasteIntoActiveTextBox(item.Text, true);
            }
        }



        private void ImportPlainText(string fileName)
        {
            using (var importText = new ImportText(fileName, _subtitle, this))
            {
                if (importText.ShowDialog(this) == DialogResult.OK)
                {
                    if (ContinueNewOrExit())
                    {
                        MakeHistoryForUndo(_language.BeforeImportText);

                        ResetSubtitle();
                        if (!Configuration.Settings.General.DisableVideoAutoLoading)
                        {
                            TryToFindAndOpenVideoFile(Utilities.GetPathAndFileNameWithoutExtension(importText.VideoFileName ?? fileName));
                        }

                        _fileName = Path.GetFileNameWithoutExtension(importText.VideoFileName);
                        _converted = true;
                        SetTitle();

                        _subtitleListViewIndex = -1;
                        if (importText.Format != null)
                        {
                            SetCurrentFormat(importText.Format);
                        }

                        _subtitle = new Subtitle(importText.FixedSubtitle.Paragraphs, _subtitle.HistoryItems);
                        ShowStatus(_language.TextImported);
                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    }
                }
            }
        }


        private void ToolStripMenuItemTranslationModeClick(object sender, EventArgs e)
        {
            if (_subtitle == null || _subtitle.Paragraphs.Count == 0)
            {
                return;
            }

            if (SubtitleListview1.IsOriginalTextColumnVisible)
            {
                RemoveOriginal(true, true);
            }
            else
            {
                OpenOriginalSubtitle();
                SetTitle();
            }
        }

        private void OpenOriginalSubtitle()
        {
            if (ContinueNewOrExitOriginal())
            {
                SaveSubtitleListviewIndices();
                openFileDialog1.Title = _languageGeneral.OpenOriginalSubtitleFile;
                openFileDialog1.FileName = string.Empty;
                openFileDialog1.Filter = UiUtil.SubtitleExtensionFilter.Value;
                if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                if (!LoadOriginalSubtitleFile(openFileDialog1.FileName))
                {
                    return;
                }

                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                RestoreSubtitleListviewIndices();

                saveOriginalToolStripMenuItem.Enabled = true;
                saveOriginalAstoolStripMenuItem.Enabled = true;
                removeOriginalToolStripMenuItem.Enabled = true;
                removeTranslationToolStripMenuItem.Enabled = true;

                Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                Configuration.Settings.Save();
                UpdateRecentFilesUI();
                MainResize();
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                RefreshSelectedParagraph();
            }
        }

        private bool LoadOriginalSubtitleFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            var file = new FileInfo(fileName);

            if (file.Extension.Equals(".sub", StringComparison.OrdinalIgnoreCase) && IsVobSubFile(fileName, false))
            {
                return false;
            }

            if (file.Length > 1024 * 1024 * 10) // max 10 mb
            {
                var text = string.Format(_language.FileXIsLargerThan10MB + Environment.NewLine + Environment.NewLine + _language.ContinueAnyway, fileName);
                if (MessageBox.Show(this, text, Title, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return false;
                }
            }

            _subtitleOriginal = new Subtitle();
            _subtitleOriginalFileName = fileName;
            SubtitleFormat format = _subtitleOriginal.LoadSubtitle(fileName, out _, null);

            if (format == null)
            {
                foreach (var binaryFormat in SubtitleFormat.GetBinaryFormats(false))
                {
                    if (binaryFormat.IsMine(null, fileName))
                    {
                        binaryFormat.LoadSubtitle(_subtitleOriginal, null, fileName);
                        format = binaryFormat;
                        break;
                    }
                }
            }

            if (format == null)
            {
                var lines = FileUtil.ReadAllTextShared(fileName, LanguageAutoDetect.GetEncodingFromFile(fileName)).SplitToLines();
                foreach (var f in SubtitleFormat.GetTextOtherFormats())
                {
                    if (f.IsMine(lines, fileName))
                    {
                        f.LoadSubtitle(_subtitleOriginal, lines, fileName);
                        format = f;
                        break;
                    }
                }
            }

            if (format == null)
            {
                return false;
            }

            saveOriginalToolStripMenuItem.Enabled = true;
            saveOriginalAstoolStripMenuItem.Enabled = true;
            removeOriginalToolStripMenuItem.Enabled = true;
            removeTranslationToolStripMenuItem.Enabled = true;

            SetAssaResolution(_subtitleOriginal);
            SetupOriginalEdit();
            FixRightToLeftDependingOnLanguage();
            return true;
        }

        private void SetupOriginalEdit()
        {
            _isOriginalActive = true;

            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal.Paragraphs.Count > 0)
            {
                InsertMissingParagraphs(_subtitle, _subtitleOriginal);
                InsertMissingParagraphs(_subtitleOriginal, _subtitle);
            }

            buttonUnBreak.Visible = false;
            buttonAutoBreak.Visible = false;
            buttonSplitLine.Visible = false;

            textBoxListViewText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            textBoxListViewText.Width = (groupBoxEdit.Width - (textBoxListViewText.Left + 10)) / 2;
            textBoxListViewTextOriginal.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            textBoxListViewTextOriginal.Left = textBoxListViewText.Left + textBoxListViewText.Width + 3;
            textBoxListViewTextOriginal.Width = textBoxListViewText.Width;
            textBoxListViewTextOriginal.Visible = true;
            textBoxListViewTextOriginal.Enabled = Configuration.Settings.General.AllowEditOfOriginalSubtitle;
            labelOriginalText.Text = _languageGeneral.OriginalText;
            labelOriginalText.Visible = true;
            labelOriginalCharactersPerSecond.Visible = true;
            labelTextOriginalLineLengths.Visible = true;
            labelOriginalSingleLine.Visible = true;
            labelOriginalSingleLinePixels.Visible = true;
            labelTextOriginalLineTotal.Visible = true;

            labelCharactersPerSecond.Left = textBoxListViewText.Left + (textBoxListViewText.Width - labelCharactersPerSecond.Width);
            labelTextLineTotal.Left = textBoxListViewText.Left + (textBoxListViewText.Width - labelTextLineTotal.Width);
            Main_Resize(null, null);
            _changeOriginalSubtitleHash = GetFastSubtitleOriginalHash();

            SetTitle();

            SubtitleListview1.ShowOriginalTextColumn(_languageGeneral.OriginalText);
            SubtitleListview1.AutoSizeAllColumns(this);
            MainResize();
        }

        private static void InsertMissingParagraphs(Subtitle masterSubtitle, Subtitle insertIntoSubtitle)
        {
            int index = 0;
            foreach (var p in masterSubtitle.Paragraphs)
            {
                var insertParagraph = Utilities.GetOriginalParagraph(index, p, insertIntoSubtitle.Paragraphs);
                if (insertParagraph == null)
                {
                    insertParagraph = new Paragraph(p) { Text = string.Empty };
                    if (p.StartTime.IsMaxTime)
                    {
                        insertIntoSubtitle.Paragraphs.Add(new Paragraph(p, true) { Text = string.Empty });
                    }
                    else
                    {
                        insertIntoSubtitle.InsertParagraphInCorrectTimeOrder(insertParagraph);
                    }
                }

                index++;
            }

            insertIntoSubtitle.Renumber();
        }

        private void FixFfmpegWrongPath()
        {
            try
            {
                if (!Configuration.IsRunningOnWindows || (!string.IsNullOrWhiteSpace(Configuration.Settings.General.FFmpegLocation) && File.Exists(Configuration.Settings.General.FFmpegLocation)))
                {
                    return;
                }

                var defaultLocation = Path.Combine(Configuration.DataDirectory, "ffmpeg", "ffmpeg.exe");
                if (File.Exists(defaultLocation))
                {
                    Configuration.Settings.General.FFmpegLocation = defaultLocation;
                    return;
                }

                defaultLocation = Path.Combine(Configuration.DataDirectory, "ffmpeg.exe");
                if (File.Exists(defaultLocation))
                {
                    Configuration.Settings.General.FFmpegLocation = defaultLocation;
                }
            }
            catch
            {
                // ignore
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OpenVideo(string fileName)
        {
            OpenVideo(fileName, VideoAudioTrackNumber);
        }

        private void OpenVideo(string fileName, int audioTrack)
        {
            if (Configuration.Settings.InitialLoad && LibMpvDynamic.IsInstalled)
            {
                Configuration.Settings.General.VideoPlayer = "MPV";
                Configuration.Settings.InitialLoad = false;
            }

            if (!_resetVideo)
            {
                return;
            }

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            if (_loading)
            {
                _videoFileName = fileName;
                VideoAudioTrackNumber = audioTrack;
                return;
            }

            var fi = new FileInfo(fileName);
            if (fi.Length < 1000)
            {
                return;
            }

            CheckSecondSubtitleReset();
            Cursor = Cursors.WaitCursor;
            _videoFileName = fileName;
            ResetPlaySelection();

            _videoInfo = UiUtil.GetVideoInfo(fileName);

            string oldVideoPlayer = Configuration.Settings.General.VideoPlayer;
            var ok = UiUtil.InitializeVideoPlayerAndContainer(fileName, _videoInfo, mediaPlayer, VideoLoaded, VideoEnded);
            if (!ok && oldVideoPlayer != Configuration.Settings.General.VideoPlayer)
            {
                CloseVideoToolStripMenuItemClick(null, null);
                _videoFileName = fileName;
                _videoInfo = UiUtil.GetVideoInfo(fileName);
                UiUtil.InitializeVideoPlayerAndContainer(fileName, _videoInfo, mediaPlayer, VideoLoaded, VideoEnded);
            }

            if (!(mediaPlayer.VideoPlayer is LibMpvDynamic))
            {
                mediaPlayer.Volume = 0;
            }
            mediaPlayer.ShowFullscreenButton = Configuration.Settings.General.VideoPlayerShowFullscreenButton;
            mediaPlayer.OnButtonClicked -= MediaPlayer_OnButtonClicked;
            mediaPlayer.OnButtonClicked += MediaPlayer_OnButtonClicked;
            closeVideoToolStripMenuItem.Enabled = true;
            toolStripMenuItemOpenKeepVideo.Enabled = true;

            if (_videoInfo == null)
            {
                return;
            }

            labelVideoInfo.Text = Path.GetFileName(fileName) + " ";
            if (_videoInfo.Width > 0 && _videoInfo.Height > 0)
            {
                labelVideoInfo.Text += _videoInfo.Width + "x" + _videoInfo.Height + " ";
            }
            if (_videoInfo.VideoCodec != null && Configuration.Settings.VideoControls.WaveformLabelShowCodec)
            {
                labelVideoInfo.Text += _videoInfo.VideoCodec.Trim() + " ";
            }
            if (_videoInfo.FramesPerSecond > 0)
            {
                labelVideoInfo.Text += string.Format("{0:0.0##}", _videoInfo.FramesPerSecond);
            }

            if (audioTrack > 0 && mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
            {
                try
                {
                    var audioTracks = libMpv.AudioTracks;
                    if (audioTracks.Count <= 1)
                    {
                        _videoAudioTrackNumber = -1;
                        audioTrack = 0;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(fileName, audioTrack);
            var spectrogramFolder = WavePeakGenerator.SpectrogramDrawer.GetSpectrogramFolder(fileName, audioTrack);
            if (File.Exists(peakWaveFileName))
            {
                audioVisualizer.ZoomFactor = 1.0;
                audioVisualizer.VerticalZoomFactor = 1.0;
                SelectZoomTextInComboBox();
                audioVisualizer.WavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                audioVisualizer.SetSpectrogram(SpectrogramData.FromDisk(spectrogramFolder));
                audioVisualizer.ShotChanges = ShotChangeHelper.FromDisk(_videoFileName);
                SetWaveformPosition(0, 0, 0);
                timerWaveform.Start();

                if (smpteTimeModedropFrameToolStripMenuItem.Checked)
                {
                    audioVisualizer.UseSmpteDropFrameTime();
                }
            }
            else
            {
                audioVisualizer.WavePeaks = null;
                audioVisualizer.SetSpectrogram(null);
                audioVisualizer.ShotChanges = new List<double>();
                audioVisualizer.Chapters = Array.Empty<MatroskaChapter>();

                if (Configuration.Settings.General.WaveformAutoGenWhenOpeningVideo)
                {
                    string targetFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
                    Process process;
                    try
                    {
                        process = AddWaveform.GetCommandLineProcess(fileName, -1, targetFile, Configuration.Settings.General.VlcWaveTranscodeSettings, out var encoderName);
                        TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(25), () => ShowStatus(_language.GeneratingWaveformInBackground, true, 10_000));
                        var bw = new BackgroundWorker();
                        bw.DoWork += (sender, args) =>
                        {
                            var p = (Process)args.Argument;
                            process.Start();
                            while (!process.HasExited)
                            {
                                Application.DoEvents();
                            }

                            // check for delay in matroska files
                            var delayInMilliseconds = 0;
                            var audioTrackNames = new List<string>();
                            var mkvAudioTrackNumbers = new Dictionary<int, int>();
                            if (fileName.ToLowerInvariant().EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    using (var matroska = new MatroskaFile(fileName))
                                    {
                                        if (matroska.IsValid)
                                        {
                                            foreach (var track in matroska.GetTracks())
                                            {
                                                if (track.IsAudio)
                                                {
                                                    if (track.CodecId != null && track.Language != null)
                                                    {
                                                        audioTrackNames.Add("#" + track.TrackNumber + ": " + track.CodecId.Replace("\0", string.Empty) + " - " + track.Language.Replace("\0", string.Empty));
                                                    }
                                                    else
                                                    {
                                                        audioTrackNames.Add("#" + track.TrackNumber);
                                                    }

                                                    mkvAudioTrackNumbers.Add(mkvAudioTrackNumbers.Count, track.TrackNumber);
                                                }
                                            }
                                            if (mkvAudioTrackNumbers.Count > 0)
                                            {
                                                delayInMilliseconds = (int)matroska.GetAudioTrackDelayMilliseconds(mkvAudioTrackNumbers[0]);
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    SeLogger.Error(exception, $"Error getting delay from mkv: {fileName}");
                                }
                            }

                            if (File.Exists(targetFile))
                            {
                                using (var waveFile = new WavePeakGenerator(targetFile))
                                {
                                    if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                                    {
                                        waveFile.GeneratePeaks(delayInMilliseconds, WavePeakGenerator.GetPeakWaveFileName(fileName));
                                        if (Configuration.Settings.VideoControls.GenerateSpectrogram)
                                        {
                                            waveFile.GenerateSpectrogram(delayInMilliseconds, WavePeakGenerator.SpectrogramDrawer.GetSpectrogramFolder(fileName));
                                        }
                                    }
                                }
                            }
                        };
                        bw.RunWorkerCompleted += (sender, args) =>
                        {
                            ShowStatus(string.Empty, false);
                            if (string.IsNullOrEmpty(_videoFileName) || !File.Exists(_videoFileName))
                            {
                                return;
                            }

                            var newPeakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(_videoFileName);
                            if (File.Exists(peakWaveFileName))
                            {
                                if (peakWaveFileName == newPeakWaveFileName)
                                {
                                    audioVisualizer.ZoomFactor = 1.0;
                                    audioVisualizer.VerticalZoomFactor = 1.0;
                                    SelectZoomTextInComboBox();
                                    audioVisualizer.WavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                                    audioVisualizer.SetSpectrogram(SpectrogramData.FromDisk(spectrogramFolder));
                                    audioVisualizer.ShotChanges = ShotChangeHelper.FromDisk(_videoFileName);
                                    SetWaveformPosition(0, 0, 0);
                                    timerWaveform.Start();

                                    if (smpteTimeModedropFrameToolStripMenuItem.Checked)
                                    {
                                        audioVisualizer.UseSmpteDropFrameTime();
                                    }
                                }
                            }
                            else
                            {
                                var hasAudioTracks = HasAudioTracks(_videoFileName);
                                if (!hasAudioTracks)
                                {
                                    AddEmptyWaveform();
                                    return;
                                }

                                ShowStatus("Waveform load failed - install ffmpeg in Options - Settings - Waveform");
                            }
                        };
                        bw.RunWorkerAsync(process);

                        audioVisualizer.WaveformNotLoadedText = _language.GeneratingWaveformInBackground;
                        audioVisualizer.Invalidate();
                    }
                    catch (DllNotFoundException)
                    {
                        //TODO: display message
                        ShowStatus(string.Empty);
                    }
                }
            }

            Cursor = Cursors.Default;
            SetUndockedWindowsTitle();
            SetAssaResolutionWithChecks();
        }

        private bool HasAudioTracks(string videoFileName)
        {
            try
            {
                if (FileUtil.IsMatroskaFile(videoFileName))
                {
                    var matroska = new MatroskaFile(videoFileName);
                    if (matroska.IsValid)
                    {
                        return matroska.GetTracks().Any(p => p.IsAudio);
                    }
                }

                if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                {
                    return libMpv.AudioTracks.Count > 0;
                }

                var info = FfmpegMediaInfo.Parse(videoFileName);
                return info.Tracks.Any(p => p.TrackType == FfmpegTrackType.Audio);
            }
            catch
            {
                return true;
            }
        }

        private void SetAssaResolutionWithChecks()
        {
            if (Configuration.Settings.SubtitleSettings.AssaResolutionAutoNew && IsAssa() && _videoInfo?.Height > 0)
            {
                if (string.IsNullOrEmpty(_subtitle?.Header))
                {
                    _subtitle.Header = AdvancedSubStationAlpha.DefaultHeader;
                }

                var oldPlayResX = AdvancedSubStationAlpha.GetTagValueFromHeader("PlayResX", "[Script Info]", _subtitle.Header);
                var oldPlayResY = AdvancedSubStationAlpha.GetTagValueFromHeader("PlayResY", "[Script Info]", _subtitle.Header);

                if (oldPlayResX == _videoInfo.Width.ToString(CultureInfo.InvariantCulture) &&
                    oldPlayResY == _videoInfo.Height.ToString(CultureInfo.InvariantCulture))
                {
                    // all good - correct resolution
                }
                else if (oldPlayResX == null || oldPlayResY == null || _subtitle.Paragraphs.Count == 0)
                {
                    SetAssaResolution(_subtitle);
                    var styles = AdvancedSubStationAlpha.GetSsaStylesFromHeader(_subtitle.Header);
                    foreach (var style in styles)
                    {
                        if (style.FontSize <= 25)
                        {
                            const int defaultAssaHeight = 288;
                            style.FontSize = AssaResampler.Resample(defaultAssaHeight, _videoInfo.Height, style.FontSize);
                        }
                    }

                    _subtitle.Header = AdvancedSubStationAlpha.GetHeaderAndStylesFromAdvancedSubStationAlpha(_subtitle.Header, styles);
                }
                else if (Configuration.Settings.SubtitleSettings.AssaResolutionPromptChange)
                {
                    if (_subtitle.Paragraphs.Count == 0 && int.TryParse(oldPlayResX, out var sourceWidth) && int.TryParse(oldPlayResX, out var sourceHeight))
                    {
                        var styles = AdvancedSubStationAlpha.GetSsaStylesFromHeader(_subtitle.Header);
                        foreach (var style in styles)
                        {
                            style.FontSize = AssaResampler.Resample(sourceHeight, _videoInfo.Height, style.FontSize);

                            style.OutlineWidth = AssaResampler.Resample(sourceHeight, _videoInfo.Height, style.OutlineWidth);
                            style.ShadowWidth = AssaResampler.Resample(sourceHeight, _videoInfo.Height, style.ShadowWidth);
                            style.Spacing = AssaResampler.Resample(sourceWidth, _videoInfo.Width, style.Spacing);
                        }
                    }
                    else
                    {
                        ShowAssaResolutionChanger(true);
                    }
                }
            }
        }

        private void MediaPlayer_OnButtonClicked(object sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Name == "_pictureBoxFullscreenOver")
            {
                if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed && _videoPlayerUndocked.IsFullscreen)
                {
                    _videoPlayerUndocked.NoFullscreen();
                }
                else
                {
                    GoFullscreen(false);
                }
            }
        }

        private void SetWaveformPosition(double startPositionSeconds, double currentVideoPositionSeconds, int subtitleIndex)
        {
            if (_subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible && Configuration.Settings.General.ShowOriginalAsPreviewIfAvailable)
            {
                int index = -1;
                if (SubtitleListview1.SelectedIndices.Count > 0 && _subtitle.Paragraphs.Count > 0)
                {
                    int i = SubtitleListview1.SelectedIndices[0];
                    var p = Utilities.GetOriginalParagraph(i, _subtitle.Paragraphs[i], _subtitleOriginal.Paragraphs);
                    index = _subtitleOriginal.GetIndex(p);
                }

                audioVisualizer.SetPosition(startPositionSeconds, _subtitleOriginal, currentVideoPositionSeconds, index, SubtitleListview1.SelectedIndices);
            }
            else
            {
                audioVisualizer.SetPosition(startPositionSeconds, _subtitle, currentVideoPositionSeconds, subtitleIndex, SubtitleListview1.SelectedIndices);
            }
        }

        private void VideoLoaded(object sender, EventArgs e)
        {
            if (_loading)
            {
                Application.DoEvents();
            }

            mediaPlayer.Volume = Configuration.Settings.General.VideoPlayerDefaultVolume;

            trackBarWaveformPosition.Maximum = (int)mediaPlayer.Duration;

            if (_videoLoadedGoToSubPosAndPause)
            {
                Application.DoEvents();
                _videoLoadedGoToSubPosAndPause = false;
                GotoSubPositionAndPause();
            }

            mediaPlayer.Pause();
            mediaPlayer.UpdatePlayerName();

            // Keep current play rate
            for (var index = 0; index < _contextMenuStripPlayRate.Items.Count; index++)
            {
                var item = (ToolStripMenuItem)_contextMenuStripPlayRate.Items[index];
                if (item.Checked)
                {
                    SetPlayRate(item, true);
                    break;
                }
            }

            if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv && !Configuration.Settings.General.MpvHandlesPreviewText)
            {
                libMpv?.RemoveSubtitle();
            }

            if (trackBarWaveformPosition.Maximum <= 0)
            {
                trackBarWaveformPosition.Maximum = (int)mediaPlayer.Duration;
            }

            textBoxSource.SelectionLength = 0;

            if (!_loading &&
                Configuration.Settings.General.AutoSetVideoSmpteForTtml &&
                !Configuration.Settings.General.CurrentVideoIsSmpte &&
                _subtitle.Header != null &&
                _subtitle.Header.Contains("frameRateMultiplier=\"1000 1001\"", StringComparison.OrdinalIgnoreCase) &&
                _subtitle.Header.Contains("timeBase=\"smpte\"", StringComparison.OrdinalIgnoreCase) &&
                _videoInfo != null &&
                !double.IsNaN(_videoInfo.FramesPerSecond) &&
                 ((decimal)_videoInfo.FramesPerSecond) % 1 != 0m &&
                (_currentSubtitleFormat?.Name == TimedText10.NameOfFormat ||
                _currentSubtitleFormat?.Name == NetflixTimedText.NameOfFormat ||
                _currentSubtitleFormat?.Name == ItunesTimedText.NameOfFormat))
            {
                if (!Configuration.Settings.General.AutoSetVideoSmpteForTtmlPrompt)
                {
                    SmpteTimeModedropFrameToolStripMenuItem_Click(null, null);
                    ShowStatus(_language.Menu.Video.SmptTimeMode);
                    return;
                }

                using (var form = new TimedTextSmpteTiming())
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        Configuration.Settings.General.AutoSetVideoSmpteForTtml = !form.Never;
                        return;
                    }

                    Configuration.Settings.General.AutoSetVideoSmpteForTtmlPrompt = !form.Always;
                    SmpteTimeModedropFrameToolStripMenuItem_Click(null, null);
                    ShowStatus(_language.Menu.Video.SmptTimeMode);
                }
            }

            if (VideoFileNameIsUrl)
            {
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(2000), () => LoadVideoInfoAfterVideoFromUrlLoad());
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(5000), () => LoadVideoInfoAfterVideoFromUrlLoad());
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(10000), () => LoadVideoInfoAfterVideoFromUrlLoad());
            }

            if (VideoAudioTrackNumber > 0)
            {
                if (mediaPlayer.VideoPlayer is LibVlcDynamic libVlc)
                {
                    libVlc.AudioTrackNumber = VideoAudioTrackNumber;
                }
                else if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv2)
                {
                    libMpv2.AudioTrackNumber = VideoAudioTrackNumber;
                }
            }

            if (!VideoFileNameIsUrl &&
                Configuration.Settings.General.FFmpegUseCenterChannelOnly &&
                Configuration.Settings.General.MpvHandlesPreviewText &&
                mediaPlayer.VideoPlayer is LibMpvDynamic libMpv2a &&
                FfmpegMediaInfo.Parse(_videoFileName).HasFrontCenterAudio(VideoAudioTrackNumber))
            {
                libMpv2a.SetAudioChannelFrontCenter(); // front center
                mediaPlayer.UsingFrontCenterAudioChannelOnly = true;
            }
        }

        private void LoadVideoInfoAfterVideoFromUrlLoad()
        {
            if (VideoFileNameIsUrl && _videoInfo == null && mediaPlayer.VideoPlayer is LibMpvDynamic libMpv && libMpv.Duration > 0)
            {
                _videoInfo = new VideoInfo()
                {
                    Width = libMpv.VideoWidth,
                    Height = libMpv.VideoHeight,
                    TotalSeconds = libMpv.Duration,
                    TotalMilliseconds = libMpv.Duration * 1000.0,
                    FramesPerSecond = libMpv.VideoTotalFrames,
                    TotalFrames = libMpv.VideoFps,
                    Success = true,
                };

                if (Configuration.Settings.General.WaveformAutoGenWhenOpeningVideo)
                {
                    AddEmptyWaveform();
                }
            }
        }

        private void VideoEnded(object sender, EventArgs e)
        {
        }

        private bool TryToFindAndOpenVideoFile(string fileNameNoExtension)
        {
            if (string.IsNullOrEmpty(fileNameNoExtension))
            {
                return false;
            }

            string movieFileName = null;

            foreach (var extension in Utilities.VideoFileExtensions.Concat(Utilities.AudioFileExtensions))
            {
                var fileName = fileNameNoExtension + extension;
                if (File.Exists(fileName))
                {
                    bool skipLoad = false;
                    if (extension == ".m2ts" && new FileInfo(fileName).Length < 2000000)
                    {
                        var textSt = new TextST();
                        skipLoad = textSt.IsMine(null, fileName); // don't load TextST files as video/audio file
                    }

                    if (!skipLoad)
                    {
                        movieFileName = fileName;
                        break;
                    }
                }
            }

            if (movieFileName != null)
            {
                OpenVideo(movieFileName);
                return true;
            }
            else
            {
                var index = fileNameNoExtension.LastIndexOf('.');
                if (index > 0 && TryToFindAndOpenVideoFile(fileNameNoExtension.Remove(index)))
                {
                    return true;
                }

                index = fileNameNoExtension.LastIndexOf('_');
                if (index > 0 && TryToFindAndOpenVideoFile(fileNameNoExtension.Remove(index)))
                {
                    return true;
                }
            }

            return false;
        }

        internal void GoBackSeconds(double seconds)
        {
        }

        private void ShowVideoPlayer()
        {
            int textHeight = splitContainerListViewAndText.Height - splitContainerListViewAndText.SplitterDistance;
            if (_isVideoControlsUndocked)
            {
                ShowHideUndockedVideoControls();
            }
            else
            {
                tabControlModes.Visible = Configuration.Settings.General.ShowVideoControls;
                var left = 5;
                if (Configuration.Settings.General.ShowVideoControls)
                {
                    left = tabControlModes.Left + tabControlModes.Width + 5;
                }
                splitContainerMain.Panel2Collapsed = false;
                if (IsVideoVisible)
                {
                    if (audioVisualizer.Visible)
                    {
                        audioVisualizer.Left = left;
                    }
                    else
                    {
                        panelVideoPlayer.Left = left;
                    }
                }
                else if (audioVisualizer.Visible)
                {
                    audioVisualizer.Left = left;
                }

                checkBoxSyncListViewWithVideoWhilePlaying.Left = left;
                audioVisualizer.Width = groupBoxVideo.Width - (audioVisualizer.Left + 10);
                panelWaveformControls.Left = audioVisualizer.Left;
                trackBarWaveformPosition.Left = panelWaveformControls.Left + panelWaveformControls.Width + 5;
                trackBarWaveformPosition.Width = audioVisualizer.Left + audioVisualizer.Width - trackBarWaveformPosition.Left + 5;
            }

            if (mediaPlayer.VideoPlayer == null && !string.IsNullOrEmpty(_fileName) && string.IsNullOrEmpty(_videoFileName) && !Configuration.Settings.General.DisableVideoAutoLoading)
            {
                TryToFindAndOpenVideoFile(Utilities.GetPathAndFileNameWithoutExtension(_fileName));
            }

            Main_Resize(null, null);

            if (!_isVideoControlsUndocked)
            {
                try
                {
                    splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - textHeight;
                }
                catch
                {
                    // ignore
                }
            }
        }

        private void ShowHideUndockedVideoControls()
        {
            if (_videoPlayerUndocked == null || _videoPlayerUndocked.IsDisposed)
            {
                UnDockVideoPlayer();
            }

            _videoPlayerUndocked.Visible = false;
            if (IsVideoVisible)
            {
                _videoPlayerUndocked.Show(this);
                if (_videoPlayerUndocked.WindowState == FormWindowState.Minimized)
                {
                    _videoPlayerUndocked.WindowState = FormWindowState.Normal;
                }
            }

            if (_waveformUndocked == null || _waveformUndocked.IsDisposed)
            {
                UnDockWaveform();
            }

            _waveformUndocked.Visible = false;
            if (IsVideoVisible)
            {
                _waveformUndocked.Show(this);
                if (_waveformUndocked.WindowState == FormWindowState.Minimized)
                {
                    _waveformUndocked.WindowState = FormWindowState.Normal;
                }
            }

            if (IsVideoVisible)
            {
                if (_videoControlsUndocked == null || _videoControlsUndocked.IsDisposed)
                {
                    UnDockVideoButtons();
                }

                _videoControlsUndocked.Visible = false;
                _videoControlsUndocked.Show(this);
            }
            else
            {
                if (_videoControlsUndocked != null && !_videoControlsUndocked.IsDisposed)
                {
                    _videoControlsUndocked.Visible = false;
                }
            }
        }

        
        private void FixLargeFonts()
        {
            using (var graphics = CreateGraphics())
            {
                var textSize = graphics.MeasureString(buttonPlayPrevious.Text, Font);
                if (textSize.Height > buttonPlayPrevious.Height - 4)
                {
                    int newButtonHeight = 23;
                    UiUtil.SetButtonHeight(this, newButtonHeight, -4);

                    // List view
                    SubtitleListview1.InitializeTimestampColumnWidths(this);
                    const int adjustUp = 8;
                    SubtitleListview1.Height -= adjustUp;
                    groupBoxEdit.Top -= adjustUp;
                    groupBoxEdit.Height += adjustUp;
                    numericUpDownDuration.Left = timeUpDownStartTime.Left + timeUpDownStartTime.Width;
                    numericUpDownDuration.Width += 5;
                    labelDuration.Left = numericUpDownDuration.Left;

                    // Video controls - Create
                    timeUpDownVideoPosition.Left = labelVideoPosition.Left + labelVideoPosition.Width;
                    int buttonWidth = labelVideoPosition.Width + timeUpDownVideoPosition.Width;
                    buttonInsertNewText.Width = buttonWidth;
                    buttonBeforeText.Width = buttonWidth;
                    buttonGotoSub.Width = buttonWidth;
                    buttonSetStartTime.Width = buttonWidth;
                    buttonSetEnd.Width = buttonWidth;
                    int FKeyLeft = buttonInsertNewText.Left + buttonInsertNewText.Width;
                    labelCreateF9.Left = FKeyLeft;
                    labelCreateF10.Left = FKeyLeft;
                    labelCreateF11.Left = FKeyLeft;
                    labelCreateF12.Left = FKeyLeft;
                    buttonForward1.Left = buttonInsertNewText.Left + buttonInsertNewText.Width - buttonForward1.Width;
                    numericUpDownSec1.Width = buttonInsertNewText.Width - (numericUpDownSec1.Left + buttonForward1.Width);
                    buttonForward2.Left = buttonInsertNewText.Left + buttonInsertNewText.Width - buttonForward2.Width;
                    numericUpDownSec2.Width = buttonInsertNewText.Width - (numericUpDownSec2.Left + buttonForward2.Width);

                    // Video controls - Adjust
                    timeUpDownVideoPositionAdjust.Left = labelVideoPosition2.Left + labelVideoPosition2.Width;
                    buttonSetStartAndOffsetRest.Width = buttonWidth;
                    buttonSetEndAndGoToNext.Width = buttonWidth;
                    buttonAdjustSetStartTime.Width = buttonWidth;
                    buttonAdjustSetEndTime.Width = buttonWidth;
                    buttonAdjustPlayBefore.Width = buttonWidth;
                    buttonAdjustGoToPosAndPause.Width = buttonWidth;
                    labelAdjustF9.Left = FKeyLeft;
                    labelAdjustF10.Left = FKeyLeft;
                    labelAdjustF11.Left = FKeyLeft;
                    labelAdjustF12.Left = FKeyLeft;
                    buttonAdjustSecForward1.Left = buttonInsertNewText.Left + buttonInsertNewText.Width - buttonAdjustSecForward1.Width;
                    numericUpDownSecAdjust1.Width = buttonInsertNewText.Width - (numericUpDownSecAdjust2.Left + buttonAdjustSecForward1.Width);
                    buttonAdjustSecForward2.Left = buttonInsertNewText.Left + buttonInsertNewText.Width - buttonAdjustSecForward2.Width;
                    numericUpDownSecAdjust2.Width = buttonInsertNewText.Width - (numericUpDownSecAdjust2.Left + buttonAdjustSecForward2.Width);

                    TabControlModes_SelectedIndexChanged(null, null);
                }
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            if (_textHeightResize >= 1)
            {
                try
                {
                    splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - _textHeightResize;
                }
                catch
                {
                    // ignore
                }
            }

            _textHeightResizeIgnoreUpdate = DateTime.UtcNow.Ticks;
            SubtitleListview1.AutoSizeAllColumns(this);

            if (WindowState == FormWindowState.Maximized ||
                WindowState == FormWindowState.Normal && _lastFormWindowState == FormWindowState.Maximized)
            {
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(25), () =>
                {
                    MainResize();
                    if (_textHeightResize >= 1)
                    {
                        try
                        {
                            splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - _textHeightResize;
                        }
                        catch
                        {
                            // ignore
                        }
                    }

                    _lastFormWindowState = WindowState;
                });
            }

            panelVideoPlayer.Invalidate();
        }

        private int _textHeightResize = -1;
        private long _textHeightResizeIgnoreUpdate = 0;

       

        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            _textHeightResizeIgnoreUpdate = 0;
            if (_loading)
            {
                return;
            }

            SuspendLayout();
            MainResize();

            if (_textHeightResize >= 1)
            {
                try
                {
                    splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - _textHeightResize;
                }
                catch
                {
                    // ignore
                }
            }

            // Due to strange bug in listview when maximizing
            SaveSubtitleListviewIndices();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            RestoreSubtitleListviewIndices();
            SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex, true);

            ResumeLayout();
        }

        private void MainResize()
        {
            if (_loading)
            {
                return;
            }

            var tbText = textBoxListViewText;
            var tbOriginal = textBoxListViewTextOriginal;
            int firstLeft = numericUpDownDuration.Right + 9;

            var lbText = labelText;
            var lbTextOriginal = labelOriginalText;

            var lbSingleLine = labelTextLineLengths;
            var lbSingleLineOriginal = labelTextOriginalLineLengths;

            var lbTotal = labelTextLineTotal;
            var lbTotalOriginal = labelTextOriginalLineTotal;

            var lbCps = labelCharactersPerSecond;
            var lbCpsOriginal = labelOriginalCharactersPerSecond;

            tbText.Left = firstLeft;
            tbOriginal.Left = firstLeft;
            lbText.Left = firstLeft;
            lbTextOriginal.Left = firstLeft;
            tbText.Width = groupBoxEdit.Width - (tbText.Left + 10 + (groupBoxEdit.Width - buttonUnBreak.Left));

            bool switchTextBoxes = Configuration.Settings.General.RightToLeftMode && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0;
            if (Configuration.Settings.General.TextAndOrigianlTextBoxesSwitched)
            {
                if (SubtitleListview1.ColumnIndexText < SubtitleListview1.ColumnIndexTextOriginal)
                {
                    SubtitleListview1.SwapTextAndOriginalText(_subtitle, _subtitleOriginal);
                }

                switchTextBoxes = !switchTextBoxes;
            }
            else
            {
                if (SubtitleListview1.ColumnIndexText > SubtitleListview1.ColumnIndexTextOriginal)
                {
                    SubtitleListview1.SwapTextAndOriginalText(_subtitle, _subtitleOriginal);
                }
            }

            if (switchTextBoxes)
            {
                tbText = textBoxListViewTextOriginal;
                tbOriginal = textBoxListViewText;

                lbText = labelOriginalText;
                lbTextOriginal = labelText;

                lbSingleLine = labelTextOriginalLineLengths;
                lbSingleLineOriginal = labelTextLineLengths;

                lbTotal = labelTextOriginalLineTotal;
                lbTotalOriginal = labelTextLineTotal;

                lbCps = labelOriginalCharactersPerSecond;
                lbCpsOriginal = labelCharactersPerSecond;
            }
            else
            {
                labelTextLineLengths.Left = firstLeft;
            }

            tbText.Left = firstLeft;
            lbText.Left = firstLeft;
            lbSingleLine.Left = firstLeft;

            if (_isOriginalActive)
            {
                tbText.Width = (groupBoxEdit.Width - (tbText.Left + 10)) / 2;
                tbOriginal.Left = tbText.Left + tbText.Width + 3;
                lbTextOriginal.Left = tbOriginal.Left;

                tbOriginal.Width = tbText.Width;

                labelOriginalCharactersPerSecond.Left = tbOriginal.Left + (tbOriginal.Width - labelOriginalCharactersPerSecond.Width);
                lbSingleLineOriginal.Left = tbOriginal.Left;
                labelOriginalSingleLine.Left = labelTextOriginalLineLengths.Left + labelTextOriginalLineLengths.Width;
                labelOriginalSingleLinePixels.Left = labelOriginalSingleLine.Left + labelOriginalSingleLine.Width + 10;
                lbTotalOriginal.Left = tbOriginal.Left + (tbOriginal.Width - lbTotalOriginal.Width);
                if (textBoxListViewText.Width / 2.1 < labelTextLineLengths.Width)
                {
                    lbTotalOriginal.Visible = false;
                }
                else
                {
                    lbTotalOriginal.Visible = true;
                }

                if (textBoxListViewText.Width / 3 < labelTextLineLengths.Width)
                {
                    labelOriginalSingleLinePixels.Visible = false;
                }
                else
                {
                    labelOriginalSingleLinePixels.Visible = Configuration.Settings.Tools.ListViewSyntaxColorWideLines;
                }
            }

            lbCpsOriginal.Top = lbCps.Top;
            lbCps.Left = tbText.Left + (tbText.Width - lbCps.Width);
            lbTotal.Left = tbText.Left + (tbText.Width - lbTotal.Width);
            SubtitleListview1.AutoSizeAllColumns(this);

            if (textBoxListViewText.Width / 2.1 < labelTextLineLengths.Width)
            {
                lbTotal.Visible = false;
            }
            else
            {
                lbTotal.Visible = true;
            }

            if (textBoxListViewText.Width / 3 < labelTextLineLengths.Width)
            {
                labelSingleLinePixels.Visible = false;
            }
            else
            {
                labelSingleLinePixels.Visible = Configuration.Settings.Tools.ListViewSyntaxColorWideLines;
            }

            FixRightToLeftDependingOnLanguage();

            if (tabControlModes.Visible)
            {
                tabControlModes.Height = tabControlModes.Parent.Height - 2 - tabControlModes.Top;
            }

            tbText.Height = groupBoxEdit.Height - tbText.Top - 32;
            tbOriginal.Height = tbText.Height;

            labelVideoInfo.Left = checkBoxSyncListViewWithVideoWhilePlaying.Right;
            labelVideoInfo.Width = labelVideoInfo.Parent.Width - labelVideoInfo.Left - 10;
        }

        private void FixRightToLeftDependingOnLanguage()
        {
            if (Configuration.Settings.General.RightToLeftMode)
            {
                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    if (LanguageAutoDetect.CouldBeRightToLeftLanguage(_subtitleOriginal))
                    {
                        textBoxListViewTextOriginal.RightToLeft = RightToLeft.Yes;
                    }
                    else
                    {
                        textBoxListViewTextOriginal.RightToLeft = RightToLeft.No;
                    }
                }

                if (LanguageAutoDetect.CouldBeRightToLeftLanguage(_subtitle))
                {
                    textBoxListViewText.RightToLeft = RightToLeft.Yes;
                    textBoxSource.RightToLeft = RightToLeft.Yes;
                }
                else
                {

                    textBoxListViewText.RightToLeft = RightToLeft.No;
                    textBoxSource.RightToLeft = RightToLeft.No;
                }
            }
            else
            {
                textBoxListViewText.RightToLeft = RightToLeft.No;
                textBoxSource.RightToLeft = RightToLeft.No;

                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    textBoxListViewTextOriginal.RightToLeft = RightToLeft.No;
                }
            }
        }

        private void PlayCurrent()
        {
            if (_subtitleListViewIndex >= 0)
            {
                GotoSubtitleIndex(_subtitleListViewIndex);
                textBoxListViewText.Focus();
                ReadyAutoRepeat();
                PlayPart(_subtitle.Paragraphs[_subtitleListViewIndex]);
            }
        }

        private void ReadyAutoRepeat()
        {
            if (checkBoxAutoRepeatOn.Checked)
            {
                _repeatCount = int.Parse(comboBoxAutoRepeat.Text);
            }
            else
            {
                _repeatCount = -1;
            }

            if (mediaPlayer.VideoPlayer != null)
            {
                labelStatus.Text = _language.VideoControls.Playing;
            }
        }

        private void PlayNext()
        {
            int newIndex = _subtitleListViewIndex + 1;
            if (newIndex < _subtitle.Paragraphs.Count)
            {
                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    item.Selected = false;
                }

                SubtitleListview1.Items[newIndex].Selected = true;
                SubtitleListview1.Items[newIndex].EnsureVisible();
                SubtitleListview1.Items[newIndex].Focused = true;
                _subtitleListViewIndex = newIndex;
                GotoSubtitleIndex(newIndex);
                PlayCurrent();
            }
        }

        private void PlayPrevious()
        {
            if (_subtitleListViewIndex > 0)
            {
                int newIndex = _subtitleListViewIndex - 1;
                foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                {
                    item.Selected = false;
                }

                SubtitleListview1.Items[newIndex].Selected = true;
                SubtitleListview1.Items[newIndex].EnsureVisible();
                SubtitleListview1.Items[newIndex].Focused = true;
                GotoSubtitleIndex(newIndex);
                _subtitleListViewIndex = newIndex;
                PlayCurrent();
            }
        }

        private void GotoSubtitleIndex(int index)
        {

        }

        private void PlayPart(Paragraph paragraph)
        {
           
        }

        private void PlaySelectedLines(bool loop)
        {
            var p = _subtitle.GetParagraphOrDefault(SubtitleListview1.SelectedItems[0].Index);
            if (p != null)
            {
                _endSeconds = p.EndTime.TotalSeconds;
                _playSelectionIndex = _subtitle.GetIndex(p);

                if (loop)
                {
                    _playSelectionIndexLoopStart = SubtitleListview1.SelectedItems[0].Index;
                }
                else
                {
                    _playSelectionIndexLoopStart = -1;
                }
            }
        }

        private void ButtonSetStartTimeClick(object sender, EventArgs e)
        {
            SetStartTime(false, mediaPlayer.CurrentPosition);
        }

        private void SetStartTime(bool adjustEndTime, double videoPosition)
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
                int index = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.Paragraphs[index];
                var oldParagraph = new Paragraph(p, false);
                if (oldParagraph.StartTime.IsMaxTime || oldParagraph.EndTime.IsMaxTime)
                {
                    adjustEndTime = true;
                }

                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + p.Number + " " + p.Text));

                timeUpDownStartTime.TimeCode = TimeCode.FromSeconds(videoPosition);

                var duration = p.DurationTotalMilliseconds;

                p.StartTime.TotalMilliseconds = videoPosition * TimeCode.BaseUnit;
                if (adjustEndTime)
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration;
                }

                if (oldParagraph.StartTime.IsMaxTime)
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
                }

                SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                timeUpDownStartTime.TimeCode = p.StartTime;
                timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;

                if (!adjustEndTime)
                {
                    SetDurationInSeconds(p.DurationTotalSeconds);
                }

                UpdateOriginalTimeCodes(oldParagraph);
                UpdateSourceView();
                RefreshSelectedParagraph();
            }
        }

        private void VideoSetStartForAppropriateLine(double videoPosition)
        {
            var p = _subtitle.Paragraphs.LastOrDefault(paragraph => videoPosition > paragraph.StartTime.TotalSeconds);
            if (p != null)
            {
                var index = _subtitle.Paragraphs.IndexOf(p);
                if (videoPosition < p.EndTime.TotalSeconds)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
                    SetStartTime(false, videoPosition);
                }
                else
                {
                    var next = _subtitle.GetParagraphOrDefault(index + 1);
                    if (next != null)
                    {
                        SubtitleListview1.SelectIndexAndEnsureVisible(_subtitle.Paragraphs.IndexOf(next), true);
                        SetStartTime(false, videoPosition);
                    }
                }
            }
            else if (_subtitle.Paragraphs.Count > 0)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                SetStartTime(false, videoPosition);
            }
        }

       

        private void SetEndTime()
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                double videoPosition = mediaPlayer.CurrentPosition;
                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                int index = SubtitleListview1.SelectedItems[0].Index;
                MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + _subtitle.Paragraphs[index].Number + " " + _subtitle.Paragraphs[index].Text));

                if (_subtitle.Paragraphs[index].StartTime.IsMaxTime)
                {
                    timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
                    _subtitle.Paragraphs[index].EndTime.TotalSeconds = videoPosition;
                    _subtitle.Paragraphs[index].StartTime.TotalMilliseconds = _subtitle.Paragraphs[index].EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(_subtitle.Paragraphs[index].Text);
                    if (_subtitle.Paragraphs[index].StartTime.TotalMilliseconds < 0)
                    {
                        _subtitle.Paragraphs[index].StartTime.TotalMilliseconds = 0;
                    }

                    timeUpDownStartTime.TimeCode = _subtitle.Paragraphs[index].StartTime;
                    SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                    timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
                }
                else
                {
                    _subtitle.Paragraphs[index].EndTime = TimeCode.FromSeconds(videoPosition);
                }

                SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                SetDurationInSeconds(_subtitle.Paragraphs[index].DurationTotalSeconds);
                UpdateSourceView();
            }
        }

        private void VideoSetEndForAppropriateLine(double videoPosition)
        {
            var p = _subtitle.Paragraphs.LastOrDefault(paragraph => videoPosition > paragraph.StartTime.TotalSeconds);
            if (p != null)
            {
                var index = _subtitle.Paragraphs.IndexOf(p);
                SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
                SetEndTime();
            }
        }

        private void ButtonInsertNewTextClick(object sender, EventArgs e)
        {
            mediaPlayer.Pause();

            var newParagraph = InsertNewTextAtVideoPosition(false);

            if (!InSourceView)
            {
                textBoxListViewText.Focus();
                if (Configuration.Settings.General.NewEmptyUseAutoDuration)
                {
                    timerAutoDuration.Start();
                }
            }

            ShowStatus(string.Format(_language.VideoControls.NewTextInsertAtX, newParagraph.StartTime.ToShortString()));
        }

        private Paragraph InsertNewTextAtVideoPosition(bool maxDuration)
        {
            // current movie Position
            double videoPositionInMilliseconds = mediaPlayer.CurrentPosition * TimeCode.BaseUnit;
            if (!mediaPlayer.IsPaused && videoPositionInMilliseconds > Configuration.Settings.General.SetStartEndHumanDelay)
            {
                videoPositionInMilliseconds -= Configuration.Settings.General.SetStartEndHumanDelay;
            }

            var tc = new TimeCode(videoPositionInMilliseconds);

            MakeHistoryForUndo(_language.BeforeInsertSubtitleAtVideoPosition + "  " + tc);
            return InsertNewParagraphAtPosition(videoPositionInMilliseconds, maxDuration);
        }

        private Paragraph InsertNewParagraphAtPosition(double positionInMilliseconds, bool maxDuration)
        {
            // find index where to insert
            int index = 0;
            foreach (var p in _subtitle.Paragraphs)
            {
                if (p.StartTime.TotalMilliseconds > positionInMilliseconds)
                {
                    break;
                }

                index++;
            }

            // prevent overlap
            var endTotalMilliseconds = positionInMilliseconds + Configuration.Settings.General.NewEmptyDefaultMs;
            if (maxDuration && mediaPlayer.VideoPlayer != null)
            {
                endTotalMilliseconds = mediaPlayer.Duration * 1000.0;
            }

            var next = _subtitle.GetParagraphOrDefault(index);
            if (next != null)
            {
                if (endTotalMilliseconds > next.StartTime.TotalMilliseconds - MinGapBetweenLines)
                {
                    endTotalMilliseconds = next.StartTime.TotalMilliseconds - MinGapBetweenLines;
                }
            }

            // create and insert
            var newParagraph = new Paragraph(string.Empty, positionInMilliseconds, endTotalMilliseconds);
            SetStyleForNewParagraph(newParagraph, index);
            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(new List<int>(), index, newParagraph);
            }
            else
            {
                _subtitle.Paragraphs.Insert(index, newParagraph);

                // check if original is available - and insert new paragraph in the original too
                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    _subtitleOriginal.InsertParagraphInCorrectTimeOrder(new Paragraph(newParagraph));
                    _subtitleOriginal.Renumber();
                }

                _subtitleListViewIndex = -1;
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            }

            SubtitleListview1.SelectIndexAndEnsureVisible(index, true);
            UpdateSourceView();
            return newParagraph;
        }

        private void ButtonBeforeTextClick(object sender, EventArgs e)
        {
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                int index = SubtitleListview1.SelectedItems[0].Index;

                mediaPlayer.Pause();
                double pos = _subtitle.Paragraphs[index].StartTime.TotalSeconds;
                if (pos > 1)
                {
                    mediaPlayer.CurrentPosition = (_subtitle.Paragraphs[index].StartTime.TotalSeconds) - 0.5;
                }
                else
                {
                    mediaPlayer.CurrentPosition = _subtitle.Paragraphs[index].StartTime.TotalSeconds;
                }

                mediaPlayer.Play();
            }
        }

        private void GotoSubPositionAndPause()
        {
            GotoSubPositionAndPause(0);
        }

        private void GotoSubPositionAndPause(double adjustSeconds)
        {
            if (mediaPlayer.VideoPlayer is null)
            {
                return;
            }

            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                int index = SubtitleListview1.SelectedItems[0].Index;
                if (index == -1 || index >= _subtitle.Paragraphs.Count)
                {
                    return;
                }

                var p = _subtitle.Paragraphs[index];
                mediaPlayer.Pause();
                if (p.StartTime.IsMaxTime)
                {
                    return;
                }

                double newPos = p.StartTime.TotalSeconds + adjustSeconds;
                if (newPos < 0)
                {
                    newPos = 0;
                }

                mediaPlayer.CurrentPosition = newPos;

                double startPos = mediaPlayer.CurrentPosition - 1;
                if (startPos < 0)
                {
                    startPos = 0;
                }

                SetWaveformPosition(startPos, mediaPlayer.CurrentPosition, index);
            }
        }


        private void ButtonOpenVideoClick(object sender, EventArgs e)
        {
            OpenVideoDialog();
        }

        private bool OpenVideoDialog()
        {
            if (string.IsNullOrEmpty(openFileDialog1.InitialDirectory) && !string.IsNullOrEmpty(_fileName))
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(_fileName);
            }

            openFileDialog1.Title = _languageGeneral.OpenVideoFileTitle;
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = UiUtil.GetVideoFileFilter(true);

            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName == _videoFileName)
                {
                    return false;
                }

                openFileDialog1.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
                if (!IsVideoVisible)
                {
                    _layout = 0;
                    SetLayout(_layout, false);
                }

                OpenVideo(openFileDialog1.FileName);
                return true;
            }

            return false;
        }

        private void ToolStripButtonLayoutChooseClick(object sender, EventArgs e)
        {
            using (var form = new LayoutPicker(_layout, Configuration.Settings.General.ShowVideoControls))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _layout = form.GetLayout();
                SetLayout(_layout, false);

                RefreshSelectedParagraph();
                if (Configuration.Settings.General.ShowVideoControls != form.ShowVideoControls)
                {
                    Configuration.Settings.General.ShowVideoControls = form.ShowVideoControls;
                    ToggleVideoControlsOnOff(form.ShowVideoControls);
                }
            }
        }

        private void SetLayout(int layout, bool undock)
        {
            if (!undock && _isVideoControlsUndocked)
            {
                RedockVideoControlsToolStripMenuItemClick(null, null);
            }

            var isLarge = _subtitle.Paragraphs.Count > 1000;
            if (isLarge)
            {
                SubtitleListview1.Items.Clear(); // for performance
            }

            var oldLayout = LayoutManager.LastLayout;
            LayoutManager.SetLayout(layout, this, panelVideoPlayer, SubtitleListview1, groupBoxVideo, groupBoxEdit, SplitContainerListViewAndTextSplitterMoved);

            if (isLarge)
            {
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal); // for performance
            }

            MainResize();

            if (!string.IsNullOrEmpty(_fileName) &&
                string.IsNullOrEmpty(_videoFileName) &&
                !Configuration.Settings.General.DisableVideoAutoLoading &&
                mediaPlayer.Visible)
            {
                TryToFindAndOpenVideoFile(Utilities.GetPathAndFileNameWithoutExtension(_fileName));
            }

            if (_subtitleListViewIndex >= 0)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex, true);
                if (layout != LayoutManager.LayoutNoVideo && oldLayout == LayoutManager.LayoutNoVideo)
                {
                    GotoSubPosAndPause();
                }
            }
        }

        public void ShowEarlierOrLater(double adjustMilliseconds, SelectionChoice selection)
        {
            var tc = new TimeCode(adjustMilliseconds);
            MakeHistoryForUndo(_language.BeforeShowSelectedLinesEarlierLater + ": " + tc);
            if (adjustMilliseconds < 0)
            {
                if (selection == SelectionChoice.AllLines)
                {
                    ShowStatus(string.Format(_language.ShowAllLinesXSecondsLinesEarlier, adjustMilliseconds / -TimeCode.BaseUnit));
                }
                else if (selection == SelectionChoice.SelectionOnly)
                {
                    ShowStatus(string.Format(_language.ShowSelectedLinesXSecondsLinesEarlier, adjustMilliseconds / -TimeCode.BaseUnit));
                }
                else if (selection == SelectionChoice.SelectionAndForward)
                {
                    ShowStatus(string.Format(_language.ShowSelectionAndForwardXSecondsLinesEarlier, adjustMilliseconds / -TimeCode.BaseUnit));
                }
            }
            else
            {
                if (selection == SelectionChoice.AllLines)
                {
                    ShowStatus(string.Format(_language.ShowAllLinesXSecondsLinesLater, adjustMilliseconds / TimeCode.BaseUnit));
                }
                else if (selection == SelectionChoice.SelectionOnly)
                {
                    ShowStatus(string.Format(_language.ShowSelectedLinesXSecondsLinesLater, adjustMilliseconds / TimeCode.BaseUnit));
                }
                else if (selection == SelectionChoice.SelectionAndForward)
                {
                    ShowStatus(string.Format(_language.ShowSelectionAndForwardXSecondsLinesLater, adjustMilliseconds / TimeCode.BaseUnit));
                }
            }

            int startFrom = 0;
            if (selection == SelectionChoice.SelectionAndForward)
            {
                if (SubtitleListview1.SelectedItems.Count > 0)
                {
                    startFrom = SubtitleListview1.SelectedItems[0].Index;
                }
                else
                {
                    startFrom = _subtitle.Paragraphs.Count;
                }
            }

            // don't overlap previous/next
            if (selection == SelectionChoice.SelectionOnly && SubtitleListview1.SelectedItems.Count == 1 &&
                !Configuration.Settings.VideoControls.WaveformAllowOverlap &&
                GetCurrentSubtitleFormat().GetType() != typeof(AdvancedSubStationAlpha))
            {
                var current = _subtitle.GetParagraphOrDefault(FirstSelectedIndex);
                if (current != null)
                {
                    if (adjustMilliseconds >= 0)
                    {
                        var next = _subtitle.GetParagraphOrDefault(FirstSelectedIndex + 1);
                        if (next != null && current.EndTime.TotalMilliseconds + MinGapBetweenLines > next.StartTime.TotalMilliseconds - adjustMilliseconds)
                        {
                            var newAdjustMs = next.StartTime.TotalMilliseconds - MinGapBetweenLines - current.EndTime.TotalMilliseconds;
                            if (newAdjustMs > 0)
                            {
                                adjustMilliseconds = newAdjustMs;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        var prev = _subtitle.GetParagraphOrDefault(FirstSelectedIndex - 1);
                        if (prev != null && current.StartTime.TotalMilliseconds - MinGapBetweenLines + adjustMilliseconds < prev.EndTime.TotalMilliseconds)
                        {
                            var newAdjustMs = prev.EndTime.TotalMilliseconds + MinGapBetweenLines - current.StartTime.TotalMilliseconds;
                            if (newAdjustMs < 0)
                            {
                                adjustMilliseconds = newAdjustMs;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }

            for (int i = startFrom; i < _subtitle.Paragraphs.Count; i++)
            {
                switch (selection)
                {
                    case SelectionChoice.SelectionOnly:
                        if (SubtitleListview1.Items[i].Selected)
                        {
                            ShowEarlierOrLaterParagraph(adjustMilliseconds, i);
                        }

                        break;
                    case SelectionChoice.AllLines:
                    case SelectionChoice.SelectionAndForward:
                        ShowEarlierOrLaterParagraph(adjustMilliseconds, i);
                        break;
                }
            }

            lock (_updateShowEarlierLock)
            {
                _updateShowEarlier = true;
            }

            RefreshSelectedParagraph();
            UpdateSourceView();
            UpdateListSyntaxColoring();
        }

        private void ShowEarlierOrLaterParagraph(double adjustMilliseconds, int i)
        {
            var p = _subtitle.GetParagraphOrDefault(i);
            if (p != null && !p.StartTime.IsMaxTime)
            {
                if (_subtitleOriginal != null)
                {
                    var original = Utilities.GetOriginalParagraph(i, p, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        original.StartTime.TotalMilliseconds += adjustMilliseconds;
                        original.EndTime.TotalMilliseconds += adjustMilliseconds;
                    }
                }

                p.StartTime.TotalMilliseconds += adjustMilliseconds;
                p.EndTime.TotalMilliseconds += adjustMilliseconds;
            }
        }

        private void UpdateSourceView()
        {
            if (!InSourceView)
            {
                return;
            }

            var textBoxSourceFocused = textBoxSource.Focused;
            var caretPosition = textBoxSource.SelectionStart;
            ShowSource();
            textBoxSource.SelectionStart = caretPosition;
            textBoxSource.ScrollToCaret();
            if (textBoxSourceFocused)
            {
                textBoxSource.Focus();
            }
        }

        private void StopAutoDuration()
        {
            timerAutoDuration.Stop();
            labelAutoDuration.Visible = false;
        }

        private void StartAutoDuration()
        {
            timerAutoDuration.Start();
            labelAutoDuration.Visible = true;
        }

        private void TextBoxListViewTextMouseMove(object sender, MouseEventArgs e)
        {
            if ((AutoRepeatContinueOn || AutoRepeatOn) && !textBoxSearchWord.Focused && textBoxListViewText.Focused)
            {
                string selectedText = textBoxListViewText.SelectedText;
                if (!string.IsNullOrEmpty(selectedText))
                {
                    selectedText = selectedText.Trim();
                    selectedText = selectedText.TrimEnd('.', ',', '!', '?');
                    selectedText = selectedText.TrimEnd();
                    if (!string.IsNullOrEmpty(selectedText) && selectedText != textBoxSearchWord.Text)
                    {
                        textBoxSearchWord.Text = HtmlUtil.RemoveHtmlTags(selectedText);
                    }
                }
            }
        }

        public void RunTranslateSearch(Action<string> act)
        {
            string text;
            if (!string.IsNullOrWhiteSpace(textBoxSearchWord.Text) &&
                !textBoxListViewText.Focused &&
                !textBoxListViewTextOriginal.Focused)
            {
                text = textBoxSearchWord.Text;
            }
            else
            {
                var tb = GetFocusedTextBox();
                if (tb.SelectionLength == 0)
                {
                    text = tb.Text;
                }
                else
                {
                    text = tb.SelectedText;
                }
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                act(text);
            }
        }

        private void ButtonGoogleItClick(object sender, EventArgs e)
        {
            RunTranslateSearch((text) => { UiUtil.OpenUrl("https://www.google.com/search?q=" + Utilities.UrlEncode(text)); });
        }

        private void ButtonGoogleTranslateItClick(object sender, EventArgs e)
        {
            RunTranslateSearch((text) =>
            {
                string languageId = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
                UiUtil.OpenUrl("https://translate.google.com/#auto|" + languageId + "|" + Utilities.UrlEncode(text));
            });
        }

        private void ButtonSetStartAndOffsetRestClick(object sender, EventArgs e)
        {
            SetStartAndOffsetTheRest(mediaPlayer.CurrentPosition);
        }

        private void SetStartAndOffsetTheRest(double videoPosition)
        {
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                bool oldSync = checkBoxSyncListViewWithVideoWhilePlaying.Checked;
                checkBoxSyncListViewWithVideoWhilePlaying.Checked = false;

                timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
                int index = SubtitleListview1.SelectedItems[0].Index;
                int lastLineNumber = SubtitleListview1.SelectedItems.Count == 1 ? SubtitleListview1.Items.Count : index + SubtitleListview1.SelectedItems.Count;
                var oldP = new Paragraph(_subtitle.Paragraphs[index]);
                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                var tc = TimeCode.FromSeconds(videoPosition);
                timeUpDownStartTime.TimeCode = tc;

                MakeHistoryForUndo(_language.BeforeSetStartTimeAndOffsetTheRest + @"  " + oldP.Number + @" - " + tc);

                double offset = oldP.StartTime.TotalMilliseconds - tc.TotalMilliseconds;

                if (oldP.StartTime.IsMaxTime)
                {
                    _subtitle.Paragraphs[index].StartTime.TotalSeconds = videoPosition;
                    _subtitle.Paragraphs[index].EndTime.TotalMilliseconds = _subtitle.Paragraphs[index].StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(_subtitle.Paragraphs[index].Text);
                    SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                    checkBoxSyncListViewWithVideoWhilePlaying.Checked = oldSync;
                    timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
                    RefreshSelectedParagraph();
                    return;
                }

                _subtitle.Paragraphs[index].StartTime = new TimeCode(_subtitle.Paragraphs[index].StartTime.TotalMilliseconds - offset);
                _subtitle.Paragraphs[index].EndTime = new TimeCode(_subtitle.Paragraphs[index].EndTime.TotalMilliseconds - offset);

                SubtitleListview1.BeginUpdate();
                for (int i = index + 1; i < lastLineNumber; i++)
                {
                    if (!_subtitle.Paragraphs[i].StartTime.IsMaxTime)
                    {
                        _subtitle.Paragraphs[i].StartTime = new TimeCode(_subtitle.Paragraphs[i].StartTime.TotalMilliseconds - offset);
                        _subtitle.Paragraphs[i].EndTime = new TimeCode(_subtitle.Paragraphs[i].EndTime.TotalMilliseconds - offset);

                        SubtitleListview1.SetStartTimeAndEndTimeSameDuration(i, _subtitle.Paragraphs[i]);
                    }
                }

                SubtitleListview1.SetStartTimeAndDuration(index - 1, _subtitle.GetParagraphOrDefault(index - 1), _subtitle.GetParagraphOrDefault(index), _subtitle.GetParagraphOrDefault(index - 2));
                SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, index - 1, _subtitle.GetParagraphOrDefault(index - 1));
                SubtitleListview1.EndUpdate();
                UpdateSourceView();

                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    var original = Utilities.GetOriginalParagraph(index, oldP, _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        index = _subtitleOriginal.GetIndex(original);
                        for (int i = index; i < _subtitleOriginal.Paragraphs.Count; i++)
                        {
                            if (!_subtitleOriginal.Paragraphs[i].StartTime.IsMaxTime)
                            {
                                _subtitleOriginal.Paragraphs[i].StartTime = new TimeCode(_subtitleOriginal.Paragraphs[i].StartTime.TotalMilliseconds - offset);
                                _subtitleOriginal.Paragraphs[i].EndTime = new TimeCode(_subtitleOriginal.Paragraphs[i].EndTime.TotalMilliseconds - offset);
                            }
                        }
                    }
                }

                checkBoxSyncListViewWithVideoWhilePlaying.Checked = oldSync;
                timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
                RefreshSelectedParagraph();
            }
        }

        private void SetStartAndOffsetTheWholeSubtitle()
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                var idx = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.GetParagraphOrDefault(idx);
                if (p is null)
                {
                    return;
                }

                var videoPosition = mediaPlayer.CurrentPosition;
                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                var offset = TimeCode.FromSeconds(videoPosition).TotalMilliseconds - p.StartTime.TotalMilliseconds;
                ShowEarlierOrLater(offset, SelectionChoice.AllLines);
            }
        }

        private void SetEndAndOffsetTheRest(bool goToNext)
        {
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                bool oldSync = checkBoxSyncListViewWithVideoWhilePlaying.Checked;
                checkBoxSyncListViewWithVideoWhilePlaying.Checked = false;

                int index = SubtitleListview1.SelectedItems[0].Index;
                int lastLineNumber = SubtitleListview1.SelectedItems.Count == 1 ? SubtitleListview1.Items.Count : index + SubtitleListview1.SelectedItems.Count;
                double videoPosition = mediaPlayer.CurrentPosition;
                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                var tc = TimeCode.FromSeconds(videoPosition);

                double offset = tc.TotalMilliseconds - _subtitle.Paragraphs[index].EndTime.TotalMilliseconds;
                if (_subtitle.Paragraphs[index].StartTime.TotalMilliseconds + 100 > tc.TotalMilliseconds)
                {
                    return;
                }

                MakeHistoryForUndo(_language.BeforeSetEndTimeAndOffsetTheRest + @"  " + _subtitle.Paragraphs[index].Number + @" - " + tc);

                numericUpDownDuration.ValueChanged -= NumericUpDownDurationValueChanged;
                _subtitle.Paragraphs[index].EndTime.TotalSeconds = videoPosition;
                SubtitleListview1.SetDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1));
                checkBoxSyncListViewWithVideoWhilePlaying.Checked = oldSync;
                numericUpDownDuration.Value = (decimal)_subtitle.Paragraphs[index].DurationTotalSeconds;
                numericUpDownDuration.ValueChanged += NumericUpDownDurationValueChanged;
                RefreshSelectedParagraph();

                SubtitleListview1.BeginUpdate();
                for (int i = index + 1; i < lastLineNumber; i++)
                {
                    if (!_subtitle.Paragraphs[i].StartTime.IsMaxTime)
                    {
                        _subtitle.Paragraphs[i].StartTime = new TimeCode(_subtitle.Paragraphs[i].StartTime.TotalMilliseconds + offset);
                        _subtitle.Paragraphs[i].EndTime = new TimeCode(_subtitle.Paragraphs[i].EndTime.TotalMilliseconds + offset);
                        SubtitleListview1.SetStartTimeAndEndTimeSameDuration(i, _subtitle.Paragraphs[i]);
                    }
                }
                SubtitleListview1.EndUpdate();

                if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    var original = Utilities.GetOriginalParagraph(index, _subtitle.Paragraphs[index], _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        index = _subtitleOriginal.GetIndex(original);
                        for (int i = index; i < lastLineNumber; i++)
                        {
                            if (!_subtitleOriginal.Paragraphs[i].StartTime.IsMaxTime)
                            {
                                _subtitleOriginal.Paragraphs[i].StartTime = new TimeCode(_subtitleOriginal.Paragraphs[i].StartTime.TotalMilliseconds + offset);
                                _subtitleOriginal.Paragraphs[i].EndTime = new TimeCode(_subtitleOriginal.Paragraphs[i].EndTime.TotalMilliseconds + offset);
                            }
                        }
                    }
                }

                checkBoxSyncListViewWithVideoWhilePlaying.Checked = oldSync;

                if (goToNext)
                {
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
                    if (mediaPlayer.IsPaused && index + 1 < _subtitle.Paragraphs.Count)
                    {
                        mediaPlayer.CurrentPosition = _subtitle.Paragraphs[index + 1].StartTime.TotalSeconds;
                    }
                }
            }
        }

        private void ButtonSetEndAndGoToNextClick(object sender, EventArgs e)
        {
            if (SubtitleListview1.SelectedItems.Count == 1)
            {
                int index = SubtitleListview1.SelectedItems[0].Index;
                double videoPosition = mediaPlayer.CurrentPosition;
                var temp = new Paragraph(_subtitle.Paragraphs[index]);
                if (!mediaPlayer.IsPaused)
                {
                    videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
                }

                if (videoPosition < temp.StartTime.TotalSeconds + 0.025)
                {
                    return;
                }

                string oldDuration = _subtitle.Paragraphs[index].Duration.ToString();
                temp.EndTime.TotalMilliseconds = TimeCode.FromSeconds(videoPosition).TotalMilliseconds;
                MakeHistoryForUndo(string.Format(_language.DisplayTimeAdjustedX, "#" + _subtitle.Paragraphs[index].Number + ": " + oldDuration + " -> " + temp.Duration));
                _makeHistoryPaused = true;

                if (_subtitle.Paragraphs[index].StartTime.IsMaxTime)
                {
                    timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
                    _subtitle.Paragraphs[index].EndTime.TotalSeconds = videoPosition;
                    _subtitle.Paragraphs[index].StartTime.TotalMilliseconds = _subtitle.Paragraphs[index].EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(_subtitle.Paragraphs[index].Text);
                    if (_subtitle.Paragraphs[index].StartTime.TotalMilliseconds < 0)
                    {
                        _subtitle.Paragraphs[index].StartTime.TotalMilliseconds = 0;
                    }

                    timeUpDownStartTime.TimeCode = _subtitle.Paragraphs[index].StartTime;
                    SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                    timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
                }
                else
                {
                    _subtitle.Paragraphs[index].EndTime = TimeCode.FromSeconds(videoPosition);
                }

                SubtitleListview1.SetDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1));
                SetDurationInSeconds(_subtitle.Paragraphs[index].DurationTotalSeconds);

                if (index + 1 < _subtitle.Paragraphs.Count)
                {
                    SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
                }

                _makeHistoryPaused = false;
                UpdateSourceView();
            }
        }

        private void ButtonAdjustSecBackClick(object sender, EventArgs e)
        {
            GoBackSeconds((double)numericUpDownSecAdjust1.Value);
        }

        private void ButtonAdjustSecForwardClick(object sender, EventArgs e)
        {
            GoBackSeconds(-(double)numericUpDownSecAdjust1.Value);
        }

        private void StartOrStopAutoBackup()
        {
            _timerAutoBackup?.Dispose();
            if (Configuration.Settings.General.AutoBackupSeconds > 0)
            {
                _timerAutoBackup = new Timer();
                _timerAutoBackup.Tick += TimerAutoBackupTick;
                _timerAutoBackup.Interval = 1000 * Configuration.Settings.General.AutoBackupSeconds; // take backup every x second if changes were made
                _timerAutoBackup.Start();
            }
        }


        private void PictureBoxRecord_Paint(object sender, PaintEventArgs e)
        {
            if (_dictateForm != null && VoskDictate.RecordingOn)
            {
                var len = pictureBoxRecord.Height - (int)Math.Round(VoskDictate.RecordingVolumePercent * pictureBoxRecord.Height / 100.0);
                using (var pen = new Pen(Color.DodgerBlue, 5))
                {
                    e.Graphics.DrawLine(pen, pictureBoxRecord.Width - 6, pictureBoxRecord.Height - 1, pictureBoxRecord.Width - 6, len);
                    e.Graphics.DrawLine(pen, 4, pictureBoxRecord.Height - 1, 4, len);
                }
            }
        }

        private void TrackBarWaveformPosition_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true; //disable default mouse wheel

            var delta = e.Delta;
            if (!Configuration.Settings.VideoControls.WaveformMouseWheelScrollUpIsForward)
            {
                delta = -delta;
            }

            if (delta > 0)
            {
                if (trackBarWaveformPosition.Value < trackBarWaveformPosition.Maximum)
                {
                    trackBarWaveformPosition.Value++;
                }
            }
            else
            {
                if (trackBarWaveformPosition.Value > trackBarWaveformPosition.Minimum)
                {
                    trackBarWaveformPosition.Value--;
                }
            }
        }

        
        private bool _updateShowEarlier;
        private readonly object _updateShowEarlierLock = new object();

        string _lastTranslationDebugError = string.Empty;
        
        private void InitializePlayRateDropDown()
        {
            var foreColor = UiUtil.ForeColor;
            var backColor = UiUtil.BackColor;
            _contextMenuStripPlayRate.Items.Clear();
            var items = new List<ToolStripMenuItem>(28);
            for (int i = 30; i <= 300; i += 10)
            {
                items.Add(new ToolStripMenuItem(i + "%", null, SetPlayRate, i.ToString()) { Checked = i == 100, BackColor = backColor, ForeColor = foreColor });
            }
            _contextMenuStripPlayRate.Items.AddRange(items.ToArray());
        }


        private void SetPlayRateAndPlay(int playRate, bool play = true)
        {
            SetPlayRate(_contextMenuStripPlayRate.Items[playRate.ToString()], false, true);
            if (play)
            {
                mediaPlayer.Play();
            }
        }

        private void SetPlayRate(object sender, EventArgs e)
        {
            SetPlayRate(sender, false);
        }

        private void SetPlayRate(object sender, bool skipStatusMessage, bool playedWithCustomSpeed = false)
        {
            if (!(sender is ToolStripMenuItem playRateDropDownItem) || mediaPlayer == null || mediaPlayer.VideoPlayer == null)
            {
                return;
            }

            foreach (ToolStripMenuItem item in _contextMenuStripPlayRate.Items)
            {
                item.Checked = false;
            }

            var percentText = playRateDropDownItem.Text.TrimEnd('%');
            var factor = double.Parse(percentText) / 100.0;
            if (!skipStatusMessage)
            {
                ShowStatus(string.Format(_language.SetPlayRateX, percentText));
            }

            if (!playedWithCustomSpeed)
            {
                playRateDropDownItem.Checked = true;
                if (Math.Abs(factor - 1) < 0.01)
                {
                    toolStripSplitButtonPlayRate.Checked = false;
                }
                else
                {
                    toolStripSplitButtonPlayRate.Checked = true;
                }
            }

            try
            {
                mediaPlayer.VideoPlayer.PlayRate = factor;
                mediaPlayer.PlayedWithCustomSpeed = playedWithCustomSpeed;
            }
            catch
            {
                if (Configuration.Settings.General.VideoPlayer != "MPV")
                {
                    using (var form = new SettingsMpv())
                    {
                        if (form.ShowDialog(this) != DialogResult.OK)
                        {
                            return;
                        }

                        Configuration.Settings.General.VideoPlayer = "MPV";
                    }
                }
            }
        }

        private void SetShortcuts()
        {
            _shortcuts.SetShortcuts();
            newToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileNew);
            openToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileOpen);
            toolStripMenuItemOpenKeepVideo.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileOpenKeepVideo);
            saveToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileSave);
            saveOriginalToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileSaveOriginal);
            saveOriginalAstoolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileSaveOriginalAs);
            saveAsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileSaveAs);
            openOriginalToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileOpenOriginal);
            removeOriginalToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileCloseOriginal);
            removeTranslationToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileCloseTranslation);
            toolStripMenuItemOpenContainingFolder.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.OpenContainingFolder);
            toolStripMenuItemCompare.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileCompare);
            toolStripMenuItemVerifyCompleteness.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileVerifyCompleteness);
            toolStripMenuItemImportText.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileImportPlainText);
            toolStripMenuItemImportBluraySupFileForEdit.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileImportBdSupForEdit);
            toolStripMenuItemImportTimeCodes.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileImportTimeCodes);
            toolStripMenuItemExportEBUSTL.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExportEbu);
            toolStripMenuItemExportPACScreenElectronics.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExportPac);
            toolStripMenuItemExportBluraySup.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExportBdSup);
            toolStripMenuItemExportEdlClipName.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExportEdlClip);
            toolStripMenuItemExportPlainText.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExportPlainText);
            exitToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainFileExit);

            toolStripMenuItemUndo.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditUndo);
            toolStripMenuItemRedo.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditRedo);
            findToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditFind);
            findNextToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditFindNext);
            replaceToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditReplace);
            multipleReplaceToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditMultipleReplace);
            gotoLineNumberToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditGoToLineNumber);
            toolStripMenuItemRightToLeftMode.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditRightToLeft);
            toolStripMenuItemShowOriginalInPreview.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews);
            toolStripMenuItemInverseSelection.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditInverseSelection);
            toolStripMenuItemModifySelection.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditModifySelection);

            adjustDisplayTimeToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsAdjustDuration);
            toolStripMenuItemApplyDurationLimits.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsAdjustDurationLimits);
            fixToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsFixCommonErrors);
            toolStripMenuItemAutoMergeShortLines.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMergeShortLines);
            toolStripMenuItemMergeDuplicateText.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMergeDuplicateText);
            toolStripMenuItemMergeLinesWithSameTimeCodes.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMergeSameTimeCodes);
            toolStripMenuItemMakeEmptyFromCurrent.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMakeEmptyFromCurrent);
            toolStripMenuItemAutoSplitLongLines.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsSplitLongLines);
            toolStripMenuItemSubtitlesBridgeGaps.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsDurationsBridgeGap);
            setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMinimumDisplayTimeBetweenParagraphs);
            startNumberingFromToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsRenumber);
            removeTextForHearImpairedToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsRemoveTextForHI);
            convertColorsToDialogToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsConvertColorsToDialog);
            ChangeCasingToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsChangeCasing);
            toolStripMenuItemShowOriginalInPreview.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews);
            toolStripMenuItemBatchConvert.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsBatchConvert);
            toolStripMenuItemMeasurementConverter.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsMeasurementConverter);
            splitToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsSplit);
            appendTextVisuallyToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsAppend);
            joinSubtitlesToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsJoin);
            toolStripMenuItemAssStyles.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainToolsStyleManager);
            listErrorsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewListErrors);

            openVideoToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainVideoOpen);
            closeVideoToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainVideoClose);
            toolStripMenuItemListShotChanges.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.WaveformListShotChanges);
            toolStripMenuItemBookmark.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralToggleBookmarksWithText);
            toolStripMenuItemGoToSourceView.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralToggleView);
            toolStripMenuItemEmptyGoToSourceView.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralToggleView);
            toolStripMenuItemGoToListView.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralToggleView);
            sortNumberToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByNumber);
            sortStartTimeToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByStartTime);
            sortEndTimeToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByEndTime);
            sortDisplayTimeToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByDuration);
            sortByGapToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByGap);
            sortTextAlphabeticallytoolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByText);
            sortTextMaxLineLengthToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortBySingleLineMaxLen);
            sortTextTotalLengthToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByTextTotalLength);
            textCharssecToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByCps);
            textWordsPerMinutewpmToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByWpm);
            sortTextNumberOfLinesToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByNumberOfLines);
            actorToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByActor);
            styleToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSortByStyle);

            spellCheckToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSpellCheck);
            findDoubleWordsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSpellCheckFindDoubleWords);
            addWordToNameListToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSpellCheckAddWordToNames);

            toolStripMenuItemAdjustAllTimes.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSynchronizationAdjustTimes);
            visualSyncToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSynchronizationVisualSync);
            toolStripMenuItemPointSync.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSynchronizationPointSync);
            pointSyncViaOtherSubtitleToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSynchronizationPointSyncViaFile);
            toolStripMenuItemChangeFrameRate2.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainSynchronizationChangeFrameRate);
            italicToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewItalic);
            italicToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewItalic);
            removeAllFormattingsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainRemoveFormatting);
            normalToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainRemoveFormatting);
            boldToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewBold);
            boldToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewBold);
            underlineToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewUnderline);
            underlineToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewUnderline);
            boxToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewBox);
            boxToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewBox);
            splitLineToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewSplit);
            toolStripMenuItemSurroundWithMusicSymbols.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewToggleMusicSymbols);
            toolStripMenuItemAlignment.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewAlignment);
            copyOriginalTextToCurrentToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent);
            columnDeleteTextOnlyToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnDeleteText);
            toolStripMenuItemColumnDeleteText.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnDeleteTextAndShiftUp);
            ShiftTextCellsDownToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnInsertText);
            toolStripMenuItemPasteSpecial.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnPaste);
            toolStripMenuItemRuby.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainTextBoxSelectionToRuby);
            moveTextUpToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnTextUp);
            moveTextDownToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColumnTextDown);
            toolStripMenuItemReverseRightToLeftStartEnd.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEditReverseStartAndEndingForRTL);
            autotranslateNLLBToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainTranslateAuto);
            genericTranslateToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainTranslateAutoSelectedLines);
            applyCustomStylesToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralApplyAssaOverrideTags);
            setPositionToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralSetAssaPosition);
            colorPickerToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralColorPicker);
            toolStripMenuItemAutoBreakLines.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainAutoBalanceSelectedLines);
            toolStripMenuItemEvenlyDistributeLines.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainEvenlyDistributeSelectedLines);
            generateBackgroundBoxToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralSetAssaBgBox);
            colorToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColorChoose);
            colorToolStripMenuItem1.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainListViewColorChoose);

            audioVisualizer.InsertAtVideoPositionShortcut = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainWaveformInsertAtCurrentPosition);
            audioVisualizer.Move100MsLeft = UiUtil.GetKeys(Configuration.Settings.Shortcuts.Waveform100MsLeft);
            audioVisualizer.Move100MsRight = UiUtil.GetKeys(Configuration.Settings.Shortcuts.Waveform100MsRight);
            audioVisualizer.MoveOneSecondLeft = UiUtil.GetKeys(Configuration.Settings.Shortcuts.Waveform1000MsLeft);
            audioVisualizer.MoveOneSecondRight = UiUtil.GetKeys(Configuration.Settings.Shortcuts.Waveform1000MsRight);

            UiUtil.HelpKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.GeneralHelp);
            helpToolStripMenuItem1.ShortcutKeys = UiUtil.HelpKeys;


            // shortcut hints
            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest2) && Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest2.Length < 5)
            {
                labelAdjustF9.Text = Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest2;
            }
            else if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest) && Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest.Length < 5)
            {
                labelAdjustF9.Text = Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest;
            }
            else
            {
                labelAdjustF9.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext) && Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext.Length < 5)
            {
                labelAdjustF10.Text = Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext;
            }
            else
            {
                labelAdjustF10.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainCreateSetStart) && Configuration.Settings.Shortcuts.MainCreateSetStart.Length < 5)
            {
                labelAdjustF11.Text = Configuration.Settings.Shortcuts.MainCreateSetStart;
                labelCreateF11.Text = Configuration.Settings.Shortcuts.MainCreateSetStart;
            }
            else
            {
                labelAdjustF11.Text = string.Empty;
                labelCreateF11.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainCreateSetEnd) && Configuration.Settings.Shortcuts.MainCreateSetEnd.Length < 5)
            {
                labelAdjustF12.Text = Configuration.Settings.Shortcuts.MainCreateSetEnd;
                labelCreateF12.Text = Configuration.Settings.Shortcuts.MainCreateSetEnd;
            }
            else
            {
                labelAdjustF12.Text = string.Empty;
                labelCreateF12.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos) && Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos.Length < 5)
            {
                labelCreateF9.Text = Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos;
            }
            else
            {
                labelCreateF9.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(Configuration.Settings.Shortcuts.MainVideoPlayFromJustBefore) && Configuration.Settings.Shortcuts.MainVideoPlayFromJustBefore.Length < 5)
            {
                labelCreateF10.Text = Configuration.Settings.Shortcuts.MainVideoPlayFromJustBefore;
            }
            else
            {
                labelCreateF10.Text = string.Empty;
            }
        }

        public static object GetPropertiesAndDoAction(string pluginFileName, out string name, out string text, out decimal version, out string description, out string actionType, out string shortcut, out System.Reflection.MethodInfo mi)
        {
            name = null;
            text = null;
            version = 0;
            description = null;
            actionType = null;
            shortcut = null;
            mi = null;
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(File.ReadAllBytes(pluginFileName));
            }
            catch (Exception exception)
            {
                SeLogger.Error(exception);

                try
                {
                    assembly = Assembly.Load(pluginFileName);
                }
                catch (Exception e)
                {
                    SeLogger.Error(e);
                }

                return null;
            }

            // note: *objectName must not include path or file extension
            string objectName = Path.GetFileNameWithoutExtension(pluginFileName);
            if (assembly != null)
            {
                Type pluginType = assembly.GetType("Nikse.SubtitleEdit.PluginLogic." + objectName);
                if (pluginType == null)
                {
                    return null;
                }

                object pluginObject = Activator.CreateInstance(pluginType);

                // IPlugin
                var t = pluginType.GetInterface("IPlugin");
                if (t == null)
                {
                    return null;
                }

                PropertyInfo pi = t.GetProperty("Name");
                if (pi != null)
                {
                    name = (string)pi.GetValue(pluginObject, null);
                }

                pi = t.GetProperty("Text");
                if (pi != null)
                {
                    text = (string)pi.GetValue(pluginObject, null);
                }

                pi = t.GetProperty("Description");
                if (pi != null)
                {
                    description = (string)pi.GetValue(pluginObject, null);
                }

                pi = t.GetProperty("Version");
                if (pi != null)
                {
                    version = Convert.ToDecimal(pi.GetValue(pluginObject, null));
                }

                pi = t.GetProperty("ActionType");
                if (pi != null)
                {
                    actionType = (string)pi.GetValue(pluginObject, null);
                }

                mi = t.GetMethod("DoAction");

                pi = t.GetProperty("Shortcut");
                if (pi != null)
                {
                    shortcut = (string)pi.GetValue(pluginObject, null);
                }

                return pluginObject;
            }

            return null;
        }

        private void LoadPlugins()
        {
            var path = Configuration.PluginsDirectory.TrimEnd(Path.DirectorySeparatorChar);
            if (!Directory.Exists(path))
            {
                return;
            }

            UiUtil.CleanUpMenuItemPlugin(fileToolStripMenuItem);
            UiUtil.CleanUpMenuItemPlugin(toolsToolStripMenuItem);
            UiUtil.CleanUpMenuItemPlugin(toolStripMenuItemSpellCheckMain);
            UiUtil.CleanUpMenuItemPlugin(toolStripMenuItemSynchronization);
            UiUtil.CleanUpMenuItemPlugin(toolStripMenuItemAutoTranslate);
            UiUtil.CleanUpMenuItemPlugin(toolStripMenuItemTranslateSelected);
            UiUtil.CleanUpMenuItemPlugin(toolStripMenuItemAssaTools);

            var fileMenuItems = new List<ToolStripMenuItem>();
            var toolsMenuItems = new List<ToolStripMenuItem>();
            var translateMenuItems = new List<ToolStripMenuItem>();
            var translateSelectedLinesMenuItems = new List<ToolStripMenuItem>();
            var syncMenuItems = new List<ToolStripMenuItem>();
            var spellCheckMenuItems = new List<ToolStripMenuItem>();
            var assaToolMenuItems = new List<ToolStripMenuItem>();

            foreach (var pluginFileName in Configuration.GetPlugins())
            {
                try
                {
                    GetPropertiesAndDoAction(pluginFileName, out var name, out var text, out var version, out var description, out var actionType, out var shortcut, out var mi);
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(actionType) && mi != null)
                    {
                        var item = new ToolStripMenuItem { Text = text, Tag = pluginFileName };
                        UiUtil.FixFonts(item);

                        if (!string.IsNullOrEmpty(shortcut))
                        {
                            item.ShortcutKeys = UiUtil.GetKeys(shortcut);
                        }

                        var shortcutCustom = Configuration.Settings.Shortcuts.PluginShortcuts.FirstOrDefault(p => p.Name == name);
                        if (shortcutCustom != null)
                        {
                            item.ShortcutKeys = UiUtil.GetKeys(shortcutCustom.Shortcut);
                        }

                        if (actionType.Equals("File", StringComparison.OrdinalIgnoreCase))
                        {
                            AddSeparator(fileMenuItems.Count, fileToolStripMenuItem, 2);
                            item.Click += PluginToolClick;
                            fileMenuItems.Add(item);
                        }
                        else if (actionType.Equals("Tool", StringComparison.OrdinalIgnoreCase))
                        {
                            AddSeparator(toolsMenuItems.Count, toolsToolStripMenuItem);
                            item.Click += PluginToolClick;
                            toolsMenuItems.Add(item);
                        }
                        else if (actionType.Equals("Sync", StringComparison.OrdinalIgnoreCase))
                        {
                            AddSeparator(syncMenuItems.Count, toolStripMenuItemSynchronization);
                            item.Click += PluginToolClick;
                            syncMenuItems.Add(item);
                        }
                        else if (actionType.Equals("Translate", StringComparison.OrdinalIgnoreCase))
                        {
                            AddSeparator(translateMenuItems.Count, toolStripMenuItemAutoTranslate);
                            item.Click += PluginClickTranslate;
                            translateMenuItems.Add(item);

                            // selected lines
                            item = new ToolStripMenuItem
                            {
                                Text = text,
                                Tag = pluginFileName
                            };
                            UiUtil.FixFonts(item);
                            AddSeparator(translateMenuItems.Count - 1, toolStripMenuItemTranslateSelected);
                            item.Click += PluginClickTranslateSelectedLines;
                            translateSelectedLinesMenuItems.Add(item);
                        }
                        else if (actionType.Equals("SpellCheck", StringComparison.OrdinalIgnoreCase))
                        {
                            AddSeparator(spellCheckMenuItems.Count, toolStripMenuItemSpellCheckMain);
                            item.Click += PluginClickNoFormatChange;
                            spellCheckMenuItems.Add(item);
                        }
                        else if (actionType.Equals("AssaTool", StringComparison.OrdinalIgnoreCase))
                        {
                            item.Click += CallPluginAssa;
                            assaToolMenuItems.Add(item);
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format(_language.ErrorLoadingPluginXErrorY, pluginFileName, exception.Message));
                }
            }

            foreach (var fileMenuItem in fileMenuItems)
            {
                fileToolStripMenuItem.DropDownItems.Insert(fileToolStripMenuItem.DropDownItems.Count - 2, fileMenuItem);
            }

            toolsToolStripMenuItem.DropDownItems.AddRange(toolsMenuItems.OrderBy(p => p.Text).ToArray());
            toolStripMenuItemAutoTranslate.DropDownItems.AddRange(translateMenuItems.OrderBy(p => p.Text).ToArray());
            toolStripMenuItemTranslateSelected.DropDownItems.AddRange(translateSelectedLinesMenuItems.OrderBy(p => p.Text).ToArray());
            toolStripMenuItemSynchronization.DropDownItems.AddRange(syncMenuItems.OrderBy(p => p.Text).ToArray());
            toolStripMenuItemSpellCheckMain.DropDownItems.AddRange(spellCheckMenuItems.OrderBy(p => p.Text).ToArray());
            toolStripMenuItemAssaTools.DropDownItems.AddRange(assaToolMenuItems.OrderBy(p => p.Text).ToArray());
        }

        private void AddSeparator(int pluginCount, ToolStripMenuItem parent, int? relativeOffset = null)
        {
            if (pluginCount == 0)
            {
                var tss = new ToolStripSeparator();
                if (relativeOffset == null)
                {
                    if (parent.DropDownItems.Count > 0 && parent.DropDownItems[parent.DropDownItems.Count - 1].GetType() == typeof(ToolStripSeparator))
                    {
                        return; // don't app separator after separator
                    }

                    parent.DropDownItems.Add(tss);
                }
                else
                {
                    if (parent.DropDownItems.Count - relativeOffset.Value >= 0 &&
                        relativeOffset.Value < parent.DropDownItems.Count &&
                        parent.DropDownItems.Count > 0 &&
                        parent.DropDownItems[parent.DropDownItems.Count - relativeOffset.Value].GetType() == typeof(ToolStripSeparator))
                    {
                        return; // don't app separator after separator
                    }

                    parent.DropDownItems.Insert(parent.DropDownItems.Count - relativeOffset.Value, tss);
                }

                UiUtil.FixFonts(tss);
            }
        }

        private void PluginToolClick(object sender, EventArgs e)
        {
            CallPlugin(sender, true, false);
        }

        private void PluginClickNoFormatChange(object sender, EventArgs e)
        {
            CallPlugin(sender, false, false);
        }

        private void PluginClickTranslate(object sender, EventArgs e)
        {
            CallPlugin(sender, false, true);
        }

        private void PluginClickTranslateSelectedLines(object sender, EventArgs e)
        {
            CallPluginTranslateSelectedLines(sender);
        }

        private void CallPlugin(object sender, bool allowChangeFormat, bool translate)
        {
            try
            {
                var item = (ToolStripItem)sender;
                var pluginObject = GetPropertiesAndDoAction(item.Tag.ToString(), out var name, out var text, out var version, out var description, out var actionType, out var shortcut, out var mi);
                if (mi == null)
                {
                    return;
                }

                string rawText = null;
                var format = GetCurrentSubtitleFormat();
                if (format != null)
                {
                    rawText = _subtitle.ToText(format);
                }

                string pluginResult = (string)mi.Invoke(pluginObject,
                    new object[]
                    {
                        this,
                        _subtitle.ToText(new SubRip()),
                        Configuration.Settings.General.CurrentFrameRate,
                        Configuration.Settings.General.ListViewLineSeparatorString,
                        _fileName,
                        _videoFileName,
                        rawText
                    });

                if (!string.IsNullOrEmpty(pluginResult) && pluginResult.Length > 10 && text != pluginResult)
                {
                    var lines = new List<string>(pluginResult.SplitToLines());

                    MakeHistoryForUndo(string.Format(_language.BeforeRunningPluginXVersionY, name, version));

                    var s = new Subtitle();
                    SubtitleFormat newFormat = null;
                    foreach (var subtitleFormat in SubtitleFormat.AllSubtitleFormats)
                    {
                        if (subtitleFormat.IsMine(lines, null))
                        {
                            subtitleFormat.LoadSubtitle(s, lines, null);
                            newFormat = subtitleFormat;
                            break;
                        }
                    }

                    if (translate)
                    {
                        _subtitleOriginal = new Subtitle(_subtitle);
                        _subtitleOriginalFileName = _fileName;

                        var language = LanguageAutoDetect.AutoDetectGoogleLanguageOrNull(s);
                        if (language != null && !string.IsNullOrEmpty(_fileName))
                        {
                            _fileName = Utilities.GetPathAndFileNameWithoutExtension(_fileName);
                            var oldLang = "." + LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitleOriginal);
                            if (oldLang.Length == 3 && _fileName.EndsWith(oldLang, StringComparison.OrdinalIgnoreCase))
                            {
                                _fileName = _fileName.Remove(_fileName.Length - 3);
                            }

                            _fileName += "." + language + GetCurrentSubtitleFormat().Extension;
                        }
                        else
                        {
                            _fileName = null;
                        }

                        _subtitle.Paragraphs.Clear();
                        foreach (var p in s.Paragraphs)
                        {
                            _subtitle.Paragraphs.Add(new Paragraph(p));
                        }

                        ShowStatus(_language.SubtitleTranslated);
                        UpdateSourceView();
                        SubtitleListview1.ShowOriginalTextColumn(_languageGeneral.OriginalText);
                        SubtitleListview1.AutoSizeAllColumns(this);
                        SetupOriginalEdit();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        ResetHistory();
                        RestoreSubtitleListviewIndices();
                        _converted = true;
                        SetTitle();
                        return;
                    }

                    if (newFormat != null)
                    {
                        if (allowChangeFormat && newFormat.GetType() == typeof(SubRip) && IsOnlyTextChanged(_subtitle, s))
                        {
                            allowChangeFormat = false;
                        }

                        if (!allowChangeFormat && IsOnlyTextChanged(_subtitle, s))
                        {
                            for (int k = 0; k < s.Paragraphs.Count; k++)
                            {
                                _subtitle.Paragraphs[k].Text = s.Paragraphs[k].Text;
                            }
                        }
                        else
                        {
                            _subtitle.Paragraphs.Clear();
                            _subtitle.Header = s.Header;
                            _subtitle.Footer = s.Footer;
                            foreach (var p in s.Paragraphs)
                            {
                                _subtitle.Paragraphs.Add(p);
                            }
                        }

                        if (allowChangeFormat)
                        {
                            SetCurrentFormat(newFormat);
                        }

                        SaveSubtitleListviewIndices();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        RestoreSubtitleListviewIndices();
                        UpdateSourceView();

                        ShowStatus(string.Format(_language.PluginXExecuted, name));
                    }
                    else
                    {
                        MessageBox.Show(_language.UnableToReadPluginResult);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                if (exception.InnerException != null)
                {
                    MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.InnerException.StackTrace);
                }
            }
        }

        private void CallPluginTranslateSelectedLines(object sender)
        {
            try
            {
                var item = (ToolStripItem)sender;
                var pluginObject = GetPropertiesAndDoAction(item.Tag.ToString(), out var name, out var text, out var version, out var description, out var actionType, out var shortcut, out var mi);
                if (mi == null)
                {
                    return;
                }

                SaveSubtitleListviewIndices();
                var selectedLines = new Subtitle();
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    var p = _subtitle.Paragraphs[index];
                    if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                    {
                        var original = Utilities.GetOriginalParagraph(index, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            p = original;
                        }
                        else
                        {
                            p = new Paragraph(string.Empty, p.StartTime.TotalMilliseconds, p.EndTime.TotalMilliseconds);
                        }
                    }

                    selectedLines.Paragraphs.Add(p);
                }

                string rawText = null;
                SubtitleFormat format = GetCurrentSubtitleFormat();
                if (format != null)
                {
                    rawText = selectedLines.ToText(format);
                }

                string pluginResult = (string)mi.Invoke(pluginObject,
                    new object[]
                    {
                        this,
                        selectedLines.ToText(new SubRip()),
                        Configuration.Settings.General.CurrentFrameRate,
                        Configuration.Settings.General.ListViewLineSeparatorString,
                        _fileName,
                        _videoFileName,
                        rawText
                    });

                if (!string.IsNullOrEmpty(pluginResult) && pluginResult.Length > 10 && text != pluginResult)
                {
                    var lines = new List<string>(pluginResult.SplitToLines());
                    MakeHistoryForUndo(string.Format(_language.BeforeRunningPluginXVersionY, name, version));
                    var s = new Subtitle();
                    var f = new SubRip();
                    if (f.IsMine(lines, null))
                    {
                        f.LoadSubtitle(s, lines, null);

                        // we only update selected lines
                        foreach (int index in SubtitleListview1.SelectedIndices)
                        {
                            var currentP = _subtitle.Paragraphs[index];
                            var translatedP = s.Paragraphs.FirstOrDefault(p => Math.Abs(p.StartTime.TotalMilliseconds - currentP.StartTime.TotalMilliseconds) < 0.001 &&
                                                                               Math.Abs(p.EndTime.TotalMilliseconds - currentP.EndTime.TotalMilliseconds) < 0.001);
                            if (translatedP != null)
                            {
                                currentP.Text = translatedP.Text;
                            }
                        }

                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        RestoreSubtitleListviewIndices();
                        ShowStatus(string.Format(_language.PluginXExecuted, name));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                if (exception.InnerException != null)
                {
                    MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.InnerException.StackTrace);
                }
            }
        }

        private void CallPluginAssa(object sender, EventArgs e)
        {
            try
            {
                var item = (ToolStripItem)sender;
                var pluginObject = GetPropertiesAndDoAction(item.Tag.ToString(), out var name, out var text, out var version, out var description, out var actionType, out var shortcut, out var mi);
                if (mi == null)
                {
                    return;
                }

                SaveSubtitleListviewIndices();
                var beforeParagraphCount = _subtitle.Paragraphs.Count;

                // Add "SelectedLines" and "VideoFilePositionMs" to [Script Info] section
                var selectedLines = SubtitleListview1.GetSelectedIndices();
                var sub = new Subtitle(_subtitle);
                SubtitleFormat format = new AdvancedSubStationAlpha();
                var selectedIndicesText = string.Join(",", selectedLines);
                if (string.IsNullOrEmpty(sub.Header))
                {
                    sub.Header = AdvancedSubStationAlpha.DefaultHeader;
                }

                sub.Header = AdvancedSubStationAlpha.AddTagToHeader("SelectedLines", $"SelectedLines: {selectedIndicesText}", "[Script Info]", sub.Header);
                if (!string.IsNullOrEmpty(_videoFileName))
                {
                    var ms = (int)Math.Round(mediaPlayer.CurrentPosition * 1000.0);
                    sub.Header = AdvancedSubStationAlpha.AddTagToHeader("VideoFilePositionMs", $"VideoFilePositionMs: {ms}", "[Script Info]", sub.Header);
                }

                var rawText = sub.ToText(format);

                string pluginResult = (string)mi.Invoke(pluginObject,
                    new object[]
                    {
                        this,
                        sub.ToText(new SubRip()),
                        Configuration.Settings.General.CurrentFrameRate,
                        Configuration.Settings.General.ListViewLineSeparatorString,
                        _fileName,
                        _videoFileName,
                        rawText
                    });

                if (!string.IsNullOrEmpty(pluginResult) && pluginResult.Length > 10 && text != pluginResult)
                {
                    var lines = new List<string>(pluginResult.SplitToLines());
                    MakeHistoryForUndo(string.Format(_language.BeforeRunningPluginXVersionY, name, version));
                    var s = new Subtitle();
                    var f = new AdvancedSubStationAlpha();
                    if (f.IsMine(lines, null))
                    {
                        f.LoadSubtitle(s, lines, null);

                        _subtitle.Paragraphs.Clear();
                        _subtitle.Paragraphs.AddRange(s.Paragraphs);
                        _subtitle.Header = AdvancedSubStationAlpha.RemoveTagFromHeader("SelectedLines", "[Script Info]", s.Header);
                        _subtitle.Header = AdvancedSubStationAlpha.RemoveTagFromHeader("VideoFilePositionMs", "[Script Info]", s.Header);
                        _subtitle.Footer = s.Footer;

                        UpdateSourceView();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        _subtitleListViewIndex = -1;

                        if (beforeParagraphCount == _subtitle.Paragraphs.Count)
                        {
                            RestoreSubtitleListviewIndices();
                        }
                        else if (_selectedIndices.Count > 0)
                        {
                            SubtitleListview1.SelectIndexAndEnsureVisible(_selectedIndices.First());
                        }
                        else
                        {
                            SubtitleListview1.SelectIndexAndEnsureVisible(0);
                        }

                        ShowStatus(string.Format(_language.PluginXExecuted, name));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                if (exception.InnerException != null)
                {
                    MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.InnerException.StackTrace);
                }
            }
        }

        private bool IsOnlyTextChanged(Subtitle s1, Subtitle s2)
        {
            if (s1.Paragraphs.Count != s2.Paragraphs.Count)
            {
                return false;
            }

            for (int i = 0; i < s1.Paragraphs.Count; i++)
            {
                var p1 = s1.Paragraphs[i];
                var p2 = s2.Paragraphs[i];
                if (Math.Abs(p1.StartTime.TotalMilliseconds - p2.StartTime.TotalMilliseconds) > 0.01)
                {
                    return false;
                }

                if (Math.Abs(p1.EndTime.TotalMilliseconds - p2.EndTime.TotalMilliseconds) > 0.01)
                {
                    return false;
                }
            }

            return true;
        }

        private string _lastWrittenAutoBackup = string.Empty;
        private void TimerAutoBackupTick(object sender, EventArgs e)
        {
            if (_openSaveCounter > 0)
            {
                return;
            }

            string currentText = string.Empty;
            if (_subtitle != null && _subtitle.Paragraphs.Count > 0)
            {
                var saveFormat = GetCurrentSubtitleFormat();
                if (!saveFormat.IsTextBased)
                {
                    saveFormat = new SubRip();
                }

                currentText = _subtitle.ToText(saveFormat);
                if (_textAutoBackup == null)
                {
                    _textAutoBackup = currentText;
                }

                if ((Configuration.Settings.General.AutoSave ||
                     !string.IsNullOrEmpty(_textAutoBackup) && currentText.Trim() != _textAutoBackup.Trim() && !string.IsNullOrWhiteSpace(currentText)) &&
                    _lastWrittenAutoBackup != currentText)
                {
                    RestoreAutoBackup.SaveAutoBackup(_subtitle, saveFormat, currentText);
                    _lastWrittenAutoBackup = currentText;

                    if (!_cleanupHasRun)
                    {
                        // let the cleanup process be handled by worker thread
                        Task.Factory.StartNew(() => { RestoreAutoBackup.CleanAutoBackupFolder(Configuration.AutoBackupDirectory, Configuration.Settings.General.AutoBackupDeleteAfterMonths); });
                        _cleanupHasRun = true;
                    }
                }
            }

            _textAutoBackup = currentText;

            if (_subtitleOriginalFileName != null && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                var saveFormat = GetCurrentSubtitleFormat();
                if (!saveFormat.IsTextBased)
                {
                    saveFormat = new SubRip();
                }

                string currentTextOriginal = _subtitleOriginal.ToText(saveFormat);
                if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    if (_textAutoBackupOriginal == null)
                    {
                        _textAutoBackupOriginal = currentTextOriginal;
                    }

                    if (Configuration.Settings.General.AutoSave ||
                        !string.IsNullOrEmpty(_textAutoBackupOriginal) && currentTextOriginal.Trim() != _textAutoBackupOriginal.Trim() && !string.IsNullOrWhiteSpace(currentTextOriginal))
                    {
                        RestoreAutoBackup.SaveAutoBackup(_subtitleOriginal, saveFormat, currentTextOriginal);
                    }
                }

                _textAutoBackupOriginal = currentTextOriginal;
            }
        }

       
        private void ButtonAdjustSecBack2Click(object sender, EventArgs e)
        {
            GoBackSeconds((double)numericUpDownSecAdjust2.Value);
        }

        private void ButtonAdjustSecForward2Click(object sender, EventArgs e)
        {
            GoBackSeconds(-(double)numericUpDownSecAdjust2.Value);
        }

        private void AudioWaveform_Click(object sender, EventArgs e)
        {
            if (audioVisualizer.WaveformNotLoadedText == _language.GeneratingWaveformInBackground)
            {
                return;
            }

            if (audioVisualizer.WavePeaks == null)
            {
                if (VideoFileNameIsUrl)
                {
                    AddEmptyWaveform();
                    return;
                }

                FixFfmpegWrongPath();

                if (string.IsNullOrEmpty(_videoFileName) || !File.Exists(_videoFileName))
                {
                    if (!OpenVideoDialog())
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(_videoFileName))
                    {
                        return;
                    }

                    if (audioVisualizer.WavePeaks != null && File.Exists(WavePeakGenerator.GetPeakWaveFileName(_videoFileName)))
                    {
                        return; // waveform already exists and is loaded
                    }
                }

                mediaPlayer.Pause();
                using (var addWaveform = new AddWaveform())
                {
                    var isVlc = mediaPlayer.VideoPlayer is LibVlcDynamic;
                    var oldVideoFileName = _videoFileName;
                    if (isVlc)
                    {
                        CloseVideoToolStripMenuItemClick(sender, e);
                    }

                    var videoAudioTrackNumber = VideoAudioTrackNumber;
                    if (isVlc && VideoAudioTrackNumber != -1)
                    {
                        videoAudioTrackNumber -= 1;
                    }

                    var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(oldVideoFileName, videoAudioTrackNumber);
                    var spectrogramFolder = WavePeakGenerator.SpectrogramDrawer.GetSpectrogramFolder(oldVideoFileName, videoAudioTrackNumber);

                    if (WavePeakGenerator.IsFileValidForVisualizer(oldVideoFileName))
                    {
                        addWaveform.InitializeViaWaveFile(oldVideoFileName, peakWaveFileName, spectrogramFolder);
                    }
                    else
                    {
                        addWaveform.Initialize(oldVideoFileName, peakWaveFileName, spectrogramFolder, videoAudioTrackNumber);
                    }

                    var dialogResult = addWaveform.ShowDialog();

                    if (isVlc)
                    {
                        OpenVideo(oldVideoFileName);
                    }

                    if (dialogResult == DialogResult.OK)
                    {
                        audioVisualizer.ZoomFactor = 1.0;
                        audioVisualizer.VerticalZoomFactor = 1.0;
                        SelectZoomTextInComboBox();
                        audioVisualizer.WavePeaks = addWaveform.Peaks;
                        if (smpteTimeModedropFrameToolStripMenuItem.Checked)
                        {
                            audioVisualizer.UseSmpteDropFrameTime();
                        }

                        audioVisualizer.SetSpectrogram(addWaveform.Spectrogram);
                        timerWaveform.Start();
                    }

                    if (videoAudioTrackNumber != addWaveform.AudioTrackNumber)
                    {
                        if (mediaPlayer.VideoPlayer is LibVlcDynamic libVlc)
                        {
                            libVlc.AudioTrackNumber = addWaveform.AudioTrackNumber + 1;
                            VideoAudioTrackNumber = addWaveform.AudioTrackNumber + 1;
                        }
                        else if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
                        {
                            libMpv.AudioTrackNumber = addWaveform.AudioTrackNumber;
                            VideoAudioTrackNumber = addWaveform.AudioTrackNumber;
                        }
                    }
                }

                if (mediaPlayer.Chapters?.Length > 0)
                {
                    audioVisualizer.Chapters = mediaPlayer.Chapters;
                }
            }
        }

        private void AddEmptyWaveform()
        {
            if (mediaPlayer.Duration > 0)
            {
                var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(_videoFileName);
                audioVisualizer.ZoomFactor = 1.0;
                audioVisualizer.VerticalZoomFactor = 1.0;
                SelectZoomTextInComboBox();
                audioVisualizer.WavePeaks = WavePeakGenerator.GenerateEmptyPeaks(peakWaveFileName, (int)mediaPlayer.Duration);
                if (smpteTimeModedropFrameToolStripMenuItem.Checked)
                {
                    audioVisualizer.UseSmpteDropFrameTime();
                }

                timerWaveform.Start();
            }
        }

        private void ReloadWaveform(string fileName, int audioTrackNumber)
        {
            if (audioVisualizer.WavePeaks != null)
            {
                audioVisualizer.WavePeaks = null;
                audioVisualizer.SetSpectrogram(null);
                audioVisualizer.ShotChanges = new List<double>();
                audioVisualizer.Chapters = Array.Empty<MatroskaChapter>();
            }

            if (mediaPlayer.VideoPlayer is LibVlcDynamic && audioTrackNumber != -1)
            {
                audioTrackNumber -= 1;
            }

            if (VideoFileNameIsUrl)
            {
                if (mediaPlayer.Duration > 0)
                {
                    ShowStatus(LanguageSettings.Current.AddWaveform.GeneratingPeakFile);
                    var peakWaveFileName1 = WavePeakGenerator.GetPeakWaveFileName(_videoFileName);
                    audioVisualizer.ZoomFactor = 1.0;
                    audioVisualizer.VerticalZoomFactor = 1.0;
                    SelectZoomTextInComboBox();
                    audioVisualizer.WavePeaks = WavePeakGenerator.GenerateEmptyPeaks(peakWaveFileName1, (int)mediaPlayer.Duration);
                    if (smpteTimeModedropFrameToolStripMenuItem.Checked)
                    {
                        audioVisualizer.UseSmpteDropFrameTime();
                    }

                    timerWaveform.Start();
                }

                return;
            }

            if (!File.Exists(_videoFileName))
            {
                return;
            }

            var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(fileName, audioTrackNumber);
            var spectrogramFolder = WavePeakGenerator.SpectrogramDrawer.GetSpectrogramFolder(_videoFileName, audioTrackNumber);

            if (File.Exists(peakWaveFileName))
            {
                audioVisualizer.ZoomFactor = 1.0;
                audioVisualizer.VerticalZoomFactor = 1.0;
                SelectZoomTextInComboBox();
                audioVisualizer.WavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                audioVisualizer.SetSpectrogram(SpectrogramData.FromDisk(spectrogramFolder));
                audioVisualizer.ShotChanges = ShotChangeHelper.FromDisk(_videoFileName);
                SetWaveformPosition(0, 0, 0);
                timerWaveform.Start();
            }

            if (smpteTimeModedropFrameToolStripMenuItem.Checked)
            {
                audioVisualizer.UseSmpteDropFrameTime();
            }
        }

        private void AddParagraphHereToolStripMenuItemClick(object sender, EventArgs e)
        {
            audioVisualizer.ClearSelection();
            var newParagraph = new Paragraph(audioVisualizer.NewSelectionParagraph);

            mediaPlayer.Pause();

            // find index where to insert
            int index = 0;
            for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
            {
                var p = _subtitle.Paragraphs[i];
                if (p.StartTime.TotalMilliseconds > newParagraph.StartTime.TotalMilliseconds &&
                    (!p.StartTime.IsMaxTime || !HasSmallerStartTimes(_subtitle, i + 1, newParagraph.StartTime.TotalMilliseconds)))
                {
                    break;
                }

                index++;
            }

            SetStyleForNewParagraph(newParagraph, index);

            MakeHistoryForUndo(_language.BeforeInsertLine);

            // create and insert
            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(new List<int>(), index, newParagraph);
            }
            else
            {
                _subtitle.Paragraphs.Insert(index, newParagraph);

                if (_subtitleOriginal != null && SubtitleListview1.IsOriginalTextColumnVisible && Configuration.Settings.General.AllowEditOfOriginalSubtitle)
                {
                    _subtitleOriginal.InsertParagraphInCorrectTimeOrder(new Paragraph(newParagraph));
                    _subtitleOriginal.Renumber();
                }

                _subtitleListViewIndex = -1;
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            }

            SubtitleListview1.SelectIndexAndEnsureVisible(index, true);

            textBoxListViewText.Focus();
            audioVisualizer.NewSelectionParagraph = null;
            UpdateSourceView();

            ShowStatus(string.Format(_language.VideoControls.NewTextInsertAtX, newParagraph.StartTime.ToShortString()));
            audioVisualizer.Invalidate();
        }

        private static bool HasSmallerStartTimes(Subtitle subtitle, int startIndex, double startMs)
        {
            for (int i = startIndex; i < subtitle.Paragraphs.Count; i++)
            {
                var p = subtitle.Paragraphs[i];
                if (startMs > p.StartTime.TotalMilliseconds && !p.StartTime.IsMaxTime)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddParagraphAndPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddParagraphHereToolStripMenuItemClick(sender, e);
            if (InSourceView)
            {
                var idx = FirstSelectedIndex;
                if (FirstSelectedIndex >= 0)
                {
                    _subtitle.Paragraphs[idx].Text = Clipboard.GetText();
                }

                UpdateSourceView();
            }
            else
            {
                textBoxListViewText.Text = Clipboard.GetText();
            }
        }


        private void WaveformPlaySelection(bool nearEnd = false)
        {
            if (mediaPlayer.VideoPlayer != null)
            {
                var p =
                    audioVisualizer.NewSelectionParagraph ??
                    audioVisualizer.SelectedParagraph;

                if (p != null)
                {
                    double startSeconds = p.StartTime.TotalSeconds;
                    _endSeconds = p.EndTime.TotalSeconds;
                    _playSelectionIndex = _subtitle.GetIndex(p);
                    _playSelectionIndexLoopStart = -1;

                    if (nearEnd)
                    {
                        startSeconds = Math.Max(startSeconds, _endSeconds - 1.0);
                    }
                }
            }
        }

        private void ToolStripComboBoxWaveformSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBoxWaveform.SelectedItem is ComboBoxZoomItem item)
                {
                    audioVisualizer.ZoomFactor = item.ZoomFactor;
                }
            }
            catch
            {
                // ignore
            }
        }

        private void SelectZoomTextInComboBox()
        {
            int i = 0;
            foreach (ComboBoxZoomItem item in toolStripComboBoxWaveform.Items)
            {
                if (Math.Abs(audioVisualizer.ZoomFactor - item.ZoomFactor) < 0.001)
                {
                    toolStripComboBoxWaveform.SelectedIndex = i;
                    return;
                }

                i++;
            }
        }


        private void ImportAndOcrBluRaySup(string fileName, bool showInTaskbar)
        {
            var log = new StringBuilder();
            var subtitles = BluRaySupParser.ParseBluRaySup(fileName, log);
            if (subtitles.Count == 0)
            {
                string msg = _language.BlurayNotSubtitlesFound + Environment.NewLine + Environment.NewLine + log.ToString();
                if (msg.Length > 800)
                {
                    msg = msg.Substring(0, 800);
                }

                MessageBox.Show(msg.Trim() + "...");
                return;
            }

            using (var vobSubOcr = new VobSubOcr())
            {
                if (showInTaskbar)
                {
                    vobSubOcr.Icon = (Icon)Icon.Clone();
                    vobSubOcr.ShowInTaskbar = true;
                    vobSubOcr.ShowIcon = true;
                }

                vobSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName);
                vobSubOcr.FileName = Path.GetFileName(fileName);
                if (vobSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBluRaySupFile);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in vobSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    RefreshSelectedParagraph();
                    _fileName = Path.ChangeExtension(fileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;
                    _imageSubFileName = fileName;

                    Configuration.Settings.Save();
                }
                else
                {
                    _exitWhenLoaded = _loading;
                }
            }
        }

        private void ImportAndInlineBase64(Subtitle subtitle, bool showInTaskbar, string fileName)
        {
            using (var vobSubOcr = new VobSubOcr())
            {
                if (showInTaskbar)
                {
                    vobSubOcr.Icon = (Icon)Icon.Clone();
                    vobSubOcr.ShowInTaskbar = true;
                    vobSubOcr.ShowIcon = true;
                }

                IList<IBinaryParagraphWithPosition> list = new List<IBinaryParagraphWithPosition>();
                foreach (var p in subtitle.Paragraphs)
                {
                    var x = new TimedTextBase64Image.Base64PngImage()
                    {
                        Text = p.Text,
                        StartTimeCode = p.StartTime,
                        EndTimeCode = p.EndTime,
                    };
                    using (var bitmap = x.GetBitmap())
                    {
                        var nikseBmp = new NikseBitmap(bitmap);
                        var nonTransparentHeight = nikseBmp.GetNonTransparentHeight();
                        if (nonTransparentHeight > 1)
                        {
                            list.Add(x);
                        }
                    }
                }

                vobSubOcr.Initialize(list, Configuration.Settings.VobSubOcr, fileName, "");
                vobSubOcr.FileName = Path.GetFileName(fileName);
                if (vobSubOcr.ShowDialog(this) == DialogResult.OK)
                {
                    MakeHistoryForUndo(_language.BeforeImportingBluRaySupFile);
                    FileNew();
                    _subtitle.Paragraphs.Clear();
                    SetCurrentFormat(Configuration.Settings.General.DefaultSubtitleFormat);
                    foreach (var p in vobSubOcr.SubtitleFromOcr.Paragraphs)
                    {
                        _subtitle.Paragraphs.Add(p);
                    }

                    UpdateSourceView();
                    _subtitleListViewIndex = -1;
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    SubtitleListview1.SelectIndexAndEnsureVisible(0, true);
                    RefreshSelectedParagraph();
                    _fileName = Path.ChangeExtension(fileName, GetCurrentSubtitleFormat().Extension);
                    SetTitle();
                    _converted = true;

                    Configuration.Settings.Save();
                }
                else
                {
                    _exitWhenLoaded = _loading;
                }
            }
        }

      
        private SETextBox GetFocusedTextBox()
        {
            if (!textBoxListViewTextOriginal.Visible)
            {
                return textBoxListViewText;
            }

            return textBoxListViewTextOriginal.Focused ? textBoxListViewTextOriginal : textBoxListViewText;
        }

        private void TextBoxListViewToggleTag(string tag)
        {
            var tb = GetFocusedTextBox();

            string text;
            int selectionStart = tb.SelectionStart;
            var isAssa = IsAssa();

            // No text selected.
            if (tb.SelectedText.Length == 0)
            {
                text = tb.Text;

                // Split lines (split a subtitle into its lines).
                var lines = text.SplitToLines();

                // Get current line index (the line where the cursor is current located).
                int numberOfNewLines = 0;
                for (int i = 0; i < tb.SelectionStart && i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        numberOfNewLines++;
                    }
                }

                int selectedLineIdx = numberOfNewLines; // Do not use 'GetLineFromCharIndex' as it also counts when lines are wrapped

                // Get line from index.
                string selectedLine = lines[selectedLineIdx];

                // Test if line at where cursor is current at is a dialog.
                bool isDialog = selectedLine.StartsWith('-') ||
                                selectedLine.StartsWith("<" + tag + ">-", StringComparison.OrdinalIgnoreCase);

                // Will be used keep cursor in its previous location after toggle/untoggle.
                int textLen = text.Length;

                // 1st set the cursor position to zero.
                tb.SelectionStart = 0;

                // If is dialog, only toggle/Untoggle line where caret/cursor is current at.
                if (isDialog)
                {
                    lines[selectedLineIdx] = HtmlUtil.ToggleTag(selectedLine, tag, false, isAssa);
                    text = string.Join(Environment.NewLine, lines);
                }
                else
                {
                    text = HtmlUtil.ToggleTag(text, tag, false, isAssa);
                }

                tb.Text = text;
                // Note: Math.Max will prevent blowing if caret is at the begining and tag was untoggled.
                tb.SelectionStart = textLen > text.Length ? Math.Max(selectionStart - 3, 0) : selectionStart + 3;
            }
            else
            {
                string post = string.Empty;
                string pre = string.Empty;
                // There is text selected
                text = tb.SelectedText;
                while (text.EndsWith(' ') || text.EndsWith(Environment.NewLine, StringComparison.Ordinal) || text.StartsWith(' ') || text.StartsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    if (text.EndsWith(' '))
                    {
                        post += " ";
                        text = text.Remove(text.Length - 1);
                    }

                    if (text.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                    {
                        post += Environment.NewLine;
                        text = text.Remove(text.Length - 2);
                    }

                    if (text.StartsWith(' '))
                    {
                        pre += " ";
                        text = text.Remove(0, 1);
                    }

                    if (text.StartsWith(Environment.NewLine, StringComparison.Ordinal))
                    {
                        pre += Environment.NewLine;
                        text = text.Remove(0, 2);
                    }
                }

                text = HtmlUtil.ToggleTag(text, tag, false, isAssa);
                // Update text and maintain selection.
                if (pre.Length > 0)
                {
                    text = pre + text;
                    selectionStart += pre.Length;
                }

                if (post.Length > 0)
                {
                    text += post;
                }

                tb.SelectedText = text;
                tb.SelectionStart = selectionStart;
                tb.SelectionLength = text.Length;
            }
        }

        private bool IsAssa()
        {
            return GetCurrentSubtitleFormat().GetType() == typeof(AdvancedSubStationAlpha);
        }

        private void EnableDisableControlsNotWorkingInNetworkMode(bool enabled)
        {
            //Top menu
            newToolStripMenuItem.Enabled = enabled;
            openToolStripMenuItem.Enabled = enabled;
            reopenToolStripMenuItem.Enabled = enabled;
            toolStripMenuItemOpenContainingFolder.Enabled = enabled;
            toolStripMenuItemCompare.Enabled = enabled;
            toolStripMenuItemVerifyCompleteness.Enabled = enabled;
            toolStripMenuItemImportFromVideo.Enabled = enabled;
            toolStripMenuItemImportDvdSubtitles.Enabled = enabled;
            toolStripMenuItemImportSubIdx.Enabled = enabled;
            toolStripMenuItemImportBluRaySup.Enabled = enabled;
            toolStripMenuItemImportManualAnsi.Enabled = enabled;
            toolStripMenuItemImportText.Enabled = enabled;
            toolStripMenuItemImportTimeCodes.Enabled = enabled;

            showHistoryforUndoToolStripMenuItem.Enabled = enabled;
            multipleReplaceToolStripMenuItem.Enabled = enabled;

            toolsToolStripMenuItem.Enabled = enabled;

            toolStripMenuItemSynchronization.Enabled = enabled;

            toolStripMenuItemAutoTranslate.Enabled = enabled;

            //Toolbar
            toolStripButtonFileNew.Enabled = enabled;
            toolStripButtonFileOpen.Enabled = enabled;
            toolStripMenuItemOpenKeepVideo.Enabled = enabled;
            toolStripMenuItemRestoreAutoBackup.Enabled = enabled;
            toolStripButtonVisualSync.Enabled = enabled;

            // textbox source
            textBoxSource.ReadOnly = !enabled;
        }

        private void NetworkGetSendUpdates(List<int> deleteIndices, int insertIndex, Paragraph insertParagraph)
        {
            _networkSession.TimerStop();

            bool doReFill = false;
            bool updateListViewStatus = false;
            SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
            string message = string.Empty;

            int numberOfLines = 0;
            List<SeNetworkService.SeUpdate> updates = null;
            int numberOfRetries = 10;
            while (numberOfRetries > 0)
            {
                numberOfRetries--;
                try
                {
                    updates = _networkSession.GetUpdates(out message, out numberOfLines);
                    numberOfRetries = 0;
                }
                catch (Exception exception)
                {
                    if (numberOfRetries <= 0)
                    {
                        if (exception.InnerException != null)
                        {
                            MessageBox.Show(string.Format(_language.NetworkUnableToConnectToServer, exception.InnerException.Message + Environment.NewLine + exception.InnerException.StackTrace));
                        }
                        else
                        {
                            MessageBox.Show(string.Format(_language.NetworkUnableToConnectToServer, exception.Message + Environment.NewLine + exception.StackTrace));
                        }

                        _networkSession.TimerStop();
                        if (_networkChat != null && !_networkChat.IsDisposed)
                        {
                            _networkChat.Close();
                            _networkChat = null;
                        }

                        _networkSession = null;
                        EnableDisableControlsNotWorkingInNetworkMode(true);
                        toolStripStatusNetworking.Visible = false;
                        SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Network);
                        _networkChat = null;
                        return;
                    }

                    Application.DoEvents();
                    Thread.Sleep(250);
                }
            }

            int currentSelectedIndex = -1;
            if (SubtitleListview1.SelectedItems.Count > 0)
            {
                currentSelectedIndex = SubtitleListview1.SelectedItems[0].Index;
            }

            int oldCurrentSelectedIndex = currentSelectedIndex;
            if (message == "OK")
            {
                foreach (var update in updates)
                {
                    if (!string.IsNullOrEmpty(update.Text))
                    {
                        if (!update.Text.Contains(Environment.NewLine))
                        {
                            update.Text = update.Text.Replace("\n", Environment.NewLine);
                        }

                        update.Text = WebUtility.HtmlDecode(update.Text).Replace("<br />", Environment.NewLine);
                    }

                    if (update.User.Ip != _networkSession.CurrentUser.Ip || update.User.UserName != _networkSession.CurrentUser.UserName)
                    {
                        if (update.Action == "USR")
                        {
                            _networkSession.Users.Add(update.User);
                            if (_networkChat != null && !_networkChat.IsDisposed)
                            {
                                _networkChat.AddUser(update.User);
                            }

                            _networkSession.AppendToLog(string.Format(_language.NetworkNewUser, update.User.UserName, update.User.Ip));
                        }
                        else if (update.Action == "MSG")
                        {
                            _networkSession.ChatLog.Add(new NikseWebServiceSession.ChatEntry { User = update.User, Message = update.Text });
                            if (_networkChat == null || _networkChat.IsDisposed)
                            {
                                _networkChat = new NetworkChat();
                                _networkChat.Initialize(_networkSession);
                                _networkChat.Show(this);
                            }
                            else
                            {
                                _networkChat.AddChatMessage(update.User, update.Text);
                            }

                            if (!string.IsNullOrEmpty(Configuration.Settings.NetworkSettings.NewMessageSound) && File.Exists(Configuration.Settings.NetworkSettings.NewMessageSound))
                            {
                                try
                                {
                                    using (var soundPlayer = new System.Media.SoundPlayer(Configuration.Settings.NetworkSettings.NewMessageSound))
                                    {
                                        soundPlayer.Play();
                                    }
                                }
                                catch
                                {
                                }
                            }

                            _networkSession.AppendToLog(string.Format(_language.NetworkMessage, update.User.UserName, update.User.Ip, update.Text));
                        }
                        else if (update.Action == "DEL")
                        {
                            doReFill = true;
                            _subtitle.Paragraphs.RemoveAt(update.Index);
                            _networkSession.LastSubtitle?.Paragraphs.RemoveAt(update.Index);

                            _networkSession.AppendToLog(string.Format(_language.NetworkDelete, update.User.UserName, update.User.Ip, update.Index));
                            _networkSession.AdjustUpdateLogToDelete(update.Index);

                            if (deleteIndices.Count > 0)
                            {
                                for (int i = deleteIndices.Count - 1; i >= 0; i--)
                                {
                                    int index = deleteIndices[i];
                                    if (index == update.Index)
                                    {
                                        deleteIndices.RemoveAt(i);
                                    }
                                    else if (index > update.Index)
                                    {
                                        deleteIndices[i] = index - 1;
                                    }
                                }
                            }

                            if (insertIndex > update.Index)
                            {
                                insertIndex--;
                            }

                            if (currentSelectedIndex >= 0 && currentSelectedIndex > update.Index)
                            {
                                currentSelectedIndex--;
                            }
                        }
                        else if (update.Action == "INS")
                        {
                            doReFill = true;
                            var p = new Paragraph(update.Text, update.StartMilliseconds, update.EndMilliseconds);
                            _subtitle.Paragraphs.Insert(update.Index, p);
                            _networkSession.LastSubtitle?.Paragraphs.Insert(update.Index, new Paragraph(p));

                            _networkSession.AppendToLog(string.Format(_language.NetworkInsert, update.User.UserName, update.User.Ip, update.Index, UiUtil.GetListViewTextFromString(update.Text)));
                            _networkSession.AddToWsUserLog(update.User, update.Index, update.Action, false);
                            updateListViewStatus = true;
                            _networkSession.AdjustUpdateLogToInsert(update.Index);

                            if (deleteIndices.Count > 0)
                            {
                                for (int i = deleteIndices.Count - 1; i >= 0; i--)
                                {
                                    int index = deleteIndices[i];
                                    if (index > update.Index)
                                    {
                                        deleteIndices[i] = index + 1;
                                    }
                                }
                            }

                            if (insertIndex > update.Index)
                            {
                                insertIndex++;
                            }

                            if (currentSelectedIndex >= 0 && currentSelectedIndex > update.Index)
                            {
                                currentSelectedIndex++;
                            }
                        }
                        else if (update.Action == "UPD")
                        {
                            updateListViewStatus = true;
                            var p = _subtitle.GetParagraphOrDefault(update.Index);
                            if (p != null)
                            {
                                p.StartTime.TotalMilliseconds = update.StartMilliseconds;
                                p.EndTime.TotalMilliseconds = update.EndMilliseconds;
                                p.Text = update.Text;
                                SubtitleListview1.SetTimeAndText(update.Index, p, _subtitle.GetParagraphOrDefault(update.Index + 1));
                                _networkSession.AppendToLog(string.Format(_language.NetworkUpdate, update.User.UserName, update.User.Ip, update.Index, UiUtil.GetListViewTextFromString(update.Text)));
                                _networkSession.AddToWsUserLog(update.User, update.Index, update.Action, true);
                                updateListViewStatus = true;
                            }

                            if (_networkSession.LastSubtitle != null)
                            {
                                p = _networkSession.LastSubtitle.GetParagraphOrDefault(update.Index);
                                if (p != null)
                                {
                                    p.StartTime.TotalMilliseconds = update.StartMilliseconds;
                                    p.EndTime.TotalMilliseconds = update.EndMilliseconds;
                                    p.Text = update.Text;
                                }
                            }
                        }
                        else if (update.Action == "BYE")
                        {
                            if (_networkChat != null && !_networkChat.IsDisposed)
                            {
                                _networkChat.RemoveUser(update.User);
                            }

                            SeNetworkService.SeUser removeUser = null;
                            foreach (var user in _networkSession.Users)
                            {
                                if (user.UserName == update.User.UserName)
                                {
                                    removeUser = user;
                                    break;
                                }
                            }

                            if (removeUser != null)
                            {
                                _networkSession.Users.Remove(removeUser);
                            }

                            _networkSession.AppendToLog(string.Format(_language.NetworkByeUser, update.User.UserName, update.User.Ip));
                        }
                        else
                        {
                            _networkSession.AppendToLog("UNKNOWN ACTION: " + update.Action + " by " + update.User.UserName + " (" + update.User.Ip + ")");
                        }
                    }
                }

                if (numberOfLines != _subtitle.Paragraphs.Count)
                {
                    _subtitle = _networkSession.ReloadSubtitle();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    UpdateListviewWithUserLogEntries();
                    _networkSession.LastSubtitle = new Subtitle(_subtitle);
                    _oldSelectedParagraph = null;
                    SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                    _networkSession.TimerStart();
                    RefreshSelectedParagraph();
                    return;
                }

                if (deleteIndices.Count > 0)
                {
                    deleteIndices.Sort();
                    deleteIndices.Reverse();
                    foreach (int i in deleteIndices)
                    {
                        _subtitle.Paragraphs.RemoveAt(i);
                        if (_networkSession.LastSubtitle != null && i < _networkSession.LastSubtitle.Paragraphs.Count)
                        {
                            _networkSession.LastSubtitle.Paragraphs.RemoveAt(i);
                        }
                    }

                    _networkSession.DeleteLines(deleteIndices);
                    doReFill = true;
                }

                if (insertIndex >= 0 && insertParagraph != null)
                {
                    _subtitle.Paragraphs.Insert(insertIndex, insertParagraph);
                    if (_networkSession.LastSubtitle != null && insertIndex < _networkSession.LastSubtitle.Paragraphs.Count)
                    {
                        _networkSession.LastSubtitle.Paragraphs.Insert(insertIndex, insertParagraph);
                    }

                    _networkSession.InsertLine(insertIndex, insertParagraph);
                    doReFill = true;
                }

                _networkSession.CheckForAndSubmitUpdates(); // updates only (no inserts/deletes)
            }
            else
            {
                if (message == "Session not found!")
                {
                    message = _networkSession.Restart();
                    if (message == "Reload")
                    {
                        _subtitle = _networkSession.ReloadSubtitle();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        UpdateListviewWithUserLogEntries();
                        _networkSession.LastSubtitle = new Subtitle(_subtitle);
                        _oldSelectedParagraph = null;
                        SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                        _networkSession.TimerStart();
                        RefreshSelectedParagraph();
                        return;
                    }

                    if (message == "OK")
                    {
                        _networkSession.TimerStart();
                        RefreshSelectedParagraph();
                        return;
                    }
                }
                else if (message == "User not found!")
                {
                    message = _networkSession.ReJoin();
                    if (message == "Reload")
                    {
                        _subtitle = _networkSession.ReloadSubtitle();
                        SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                        UpdateListviewWithUserLogEntries();
                        _networkSession.LastSubtitle = new Subtitle(_subtitle);
                        _oldSelectedParagraph = null;
                        SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                        _networkSession.TimerStart();
                        RefreshSelectedParagraph();
                        return;
                    }
                }

                MessageBox.Show(message);
                LeaveSessionToolStripMenuItemClick(null, null);
                SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                return;
            }

            if (doReFill)
            {
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                UpdateListviewWithUserLogEntries();

                if (oldCurrentSelectedIndex != currentSelectedIndex)
                {
                    _oldSelectedParagraph = null;
                    _subtitleListViewIndex = currentSelectedIndex;
                    SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex);
                }
                else if (_oldSelectedParagraph != null)
                {
                    var p = _subtitle.GetFirstAlike(_oldSelectedParagraph);
                    if (p == null)
                    {
                        var tmp = new Paragraph(_oldSelectedParagraph) { Text = textBoxListViewText.Text };
                        p = _subtitle.GetFirstAlike(tmp);
                    }

                    if (p == null)
                    {
                        int idx = oldCurrentSelectedIndex;
                        if (idx >= _subtitle.Paragraphs.Count)
                        {
                            idx = _subtitle.Paragraphs.Count - 1;
                        }

                        if (idx >= 0 && idx < _subtitle.Paragraphs.Count)
                        {
                            SubtitleListview1.SelectIndexAndEnsureVisible(idx);
                            _listViewTextUndoIndex = -1;
                            SubtitleListView1SelectedIndexChange();
                            textBoxListViewText.Text = _subtitle.Paragraphs[idx].Text;
                        }
                    }
                    else
                    {
                        _subtitleListViewIndex = _subtitle.GetIndex(p);
                        SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex);
                        _listViewTextUndoIndex = -1;
                        SubtitleListView1SelectedIndexChange();
                    }
                }
            }
            else if (updateListViewStatus)
            {
                UpdateListviewWithUserLogEntries();
            }

            _networkSession.LastSubtitle = new Subtitle(_subtitle);
            SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
            _networkSession.TimerStart();
        }

        private void UpdateListviewWithUserLogEntries()
        {
            SubtitleListview1.BeginUpdate();
            foreach (UpdateLogEntry entry in _networkSession.UpdateLog)
            {
                SubtitleListview1.SetNetworkText(entry.Index, entry.ToString(), Utilities.GetColorFromUserName(entry.UserName));
            }

            SubtitleListview1.EndUpdate();
        }

        private void LeaveSessionToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                _networkSession.Leave();
            }

            if (_networkChat != null && !_networkChat.IsDisposed)
            {
                _networkChat.Close();
                _networkChat = null;
            }

            _networkSession = null;
            EnableDisableControlsNotWorkingInNetworkMode(true);
            toolStripStatusNetworking.Visible = false;
            SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.Network);
            _networkChat = null;
        }




        private Control _videoPlayerUndockParent;

        private void UnDockVideoPlayer()
        {
            bool firstUndock = _videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed;

            _videoPlayerUndocked = new VideoPlayerUndocked(this, mediaPlayer);

            if (firstUndock)
            {
                Configuration.Settings.General.UndockedVideoPosition = _videoPlayerUndocked.Left + @";" + _videoPlayerUndocked.Top + @";" + _videoPlayerUndocked.Width + @";" + _videoPlayerUndocked.Height;
            }

            Control control = mediaPlayer;
            _videoPlayerUndockParent = control.Parent;
            if (control.Parent != null)
            {
                control.Parent.Controls.Remove(control);
            }

            if (control != null)
            {
                control.Top = 0;
                control.Left = 0;
                control.Width = _videoPlayerUndocked.PanelContainer.Width;
                control.Height = _videoPlayerUndocked.PanelContainer.Height;
                _videoPlayerUndocked.PanelContainer.Dock = DockStyle.Fill;
                _videoPlayerUndocked.PanelContainer.Controls.Add(control);
                control.Dock = DockStyle.Fill;
            }
        }

        public void ReDockVideoPlayer(Control control)
        {
            if (_videoPlayerUndockParent != null)
            {
                _videoPlayerUndockParent.Controls.Add(control);
                control.Dock = DockStyle.Fill;
            }

            mediaPlayer.FontSizeFactor = 1.0F;
            mediaPlayer.SetSubtitleFont();
            mediaPlayer.SubtitleText = string.Empty;
        }

        private Control _waveformUndockParent;

        private void UnDockWaveform()
        {
            _waveformUndocked = new WaveformUndocked(this);

            var control = audioVisualizer;
            _waveformUndockParent = control.Parent;
            if (_waveformUndockParent != null)
            {
                _waveformUndockParent.Controls.Remove(control);
            }

            control.Top = 0;
            control.Left = 0;
            control.Width = _waveformUndocked.PanelContainer.Width;
            control.Height = _waveformUndocked.PanelContainer.Height - panelWaveformControls.Height;
            _waveformUndocked.PanelContainer.Controls.Add(control);

            var control2 = (Control)panelWaveformControls;
            groupBoxVideo.Controls.Remove(control2);
            control2.Top = control.Height;
            control2.Left = 0;
            _waveformUndocked.PanelContainer.Controls.Add(control2);

            var control3 = (Control)trackBarWaveformPosition;
            groupBoxVideo.Controls.Remove(control3);
            control3.Top = control.Height;
            control3.Left = control2.Width + 2;
            control3.Width = _waveformUndocked.PanelContainer.Width - control3.Left;
            _waveformUndocked.PanelContainer.Controls.Add(control3);
        }

        public void ReDockWaveform(Control waveform, Control buttons, Control trackBar)
        {
            groupBoxVideo.Controls.Add(waveform);
            waveform.Top = 30;
            waveform.Height = groupBoxVideo.Height - (waveform.Top + buttons.Height + 10);

            groupBoxVideo.Controls.Add(buttons);
            buttons.Top = waveform.Top + waveform.Height + 5;

            groupBoxVideo.Controls.Add(trackBar);
            trackBar.Top = buttons.Top;
        }

        private void UnDockVideoButtons()
        {
            _videoControlsUndocked = new VideoControlsUndocked(this);
            var control = tabControlModes;
            groupBoxVideo.Controls.Remove(control);
            control.Top = 25;
            control.Left = 0;
            control.Height = _videoControlsUndocked.PanelContainer.Height - 4;
            _videoControlsUndocked.PanelContainer.Controls.Add(control);

            groupBoxVideo.Controls.Remove(checkBoxSyncListViewWithVideoWhilePlaying);
            _videoControlsUndocked.PanelContainer.Controls.Add(checkBoxSyncListViewWithVideoWhilePlaying);
            checkBoxSyncListViewWithVideoWhilePlaying.Top = 5;
            checkBoxSyncListViewWithVideoWhilePlaying.Left = 5;

            splitContainerMain.Panel2Collapsed = true;
            splitContainer1.Panel2Collapsed = true;
        }

        public void ReDockVideoButtons(Control videoButtons, Control checkBoxSyncSubWithVideo)
        {
            groupBoxVideo.Controls.Add(videoButtons);
            videoButtons.Top = 12;
            videoButtons.Left = 5;

            groupBoxVideo.Controls.Add(checkBoxSyncSubWithVideo);
            checkBoxSyncSubWithVideo.Top = 11;
            checkBoxSyncSubWithVideo.Left = videoButtons.Left + videoButtons.Width + 5;
        }

        private void UndockVideoControlsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Configuration.Settings.General.Undocked)
            {
                return;
            }

            Configuration.Settings.General.Undocked = true;
            var top = Math.Max(Top, 0);
            var left = Math.Max(Left, 0);
            UnDockVideoPlayer();
            try
            {
                splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - 109;
            }
            catch
            {
                // ignore
            }

            if (IsVideoVisible)
            {
                _videoPlayerUndocked.Show(this);
                if (_videoPlayerUndocked.Top < 0 || _videoPlayerUndocked.Left < 0)
                {
                    _videoPlayerUndocked.WindowState = FormWindowState.Normal;
                    _videoPlayerUndocked.Top = top + 40;
                    _videoPlayerUndocked.Left = Math.Abs(left - 20);
                    _videoPlayerUndocked.Width = 600;
                    _videoPlayerUndocked.Height = 400;
                }
            }

            UnDockWaveform();
            if (IsVideoVisible)
            {
                _waveformUndocked.Show(this);
                if (_waveformUndocked.Top < 0 || _waveformUndocked.Left < 0)
                {
                    _waveformUndocked.WindowState = FormWindowState.Normal;
                    _waveformUndocked.Top = top + 60;
                    _waveformUndocked.Left = Math.Abs(left - 15);
                    _waveformUndocked.Width = 600;
                    _waveformUndocked.Height = 200;
                }
            }

            UnDockVideoButtons();
            _videoControlsUndocked.Show(this);
            if (_videoControlsUndocked.Top < 0 || _videoControlsUndocked.Left < 0)
            {
                _videoControlsUndocked.WindowState = FormWindowState.Normal;
                _videoControlsUndocked.Top = top + 40;
                _videoControlsUndocked.Left = Math.Abs(left - 10);
                _videoControlsUndocked.Width = tabControlModes.Width + 20;
                _videoControlsUndocked.Height = tabControlModes.Height + 65;
            }

            _isVideoControlsUndocked = true;
            SetUndockedWindowsTitle();

            undockVideoControlsToolStripMenuItem.Visible = false;
            redockVideoControlsToolStripMenuItem.Visible = true;

            TabControlModes_SelectedIndexChanged(null, null);
            _videoControlsUndocked.Refresh();

            SetLayout(LayoutManager.LayoutNoVideo, true);
        }

        public void RedockVideoControlsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!Configuration.Settings.General.Undocked)
            {
                return;
            }

            _textHeightResizeIgnoreUpdate = DateTime.UtcNow.Ticks;
            mediaPlayer.ShowNonFullScreenControls();

            SaveUndockedPositions();

            Configuration.Settings.General.Undocked = false;

            if (_videoControlsUndocked != null && !_videoControlsUndocked.IsDisposed)
            {
                var control = _videoControlsUndocked.PanelContainer.Controls[0];
                control.Dock = DockStyle.None;
                var controlCheckBox = _videoControlsUndocked.PanelContainer.Controls[1];
                _videoControlsUndocked.PanelContainer.Controls.Clear();
                ReDockVideoButtons(control, controlCheckBox);
                _videoControlsUndocked.Close();
                _videoControlsUndocked = null;
            }

            if (_waveformUndocked != null && !_waveformUndocked.IsDisposed)
            {
                var controlWaveform = _waveformUndocked.PanelContainer.Controls[0];
                var controlButtons = _waveformUndocked.PanelContainer.Controls[1];
                var controlTrackBar = _waveformUndocked.PanelContainer.Controls[2];
                _waveformUndocked.PanelContainer.Controls.Clear();
                ReDockWaveform(controlWaveform, controlButtons, controlTrackBar);
                _waveformUndocked.Close();
                _waveformUndocked = null;
            }

            if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed)
            {
                var control = mediaPlayer;
                if (mediaPlayer.Parent != null)
                {
                    mediaPlayer.Parent.Controls.Remove(control);
                }

                ReDockVideoPlayer(control);
                _videoPlayerUndocked.Close();
                _videoPlayerUndocked = null;
                mediaPlayer.ShowFullscreenButton = Configuration.Settings.General.VideoPlayerShowFullscreenButton;
            }

            _isVideoControlsUndocked = false;
            _videoPlayerUndocked = null;
            _waveformUndocked = null;
            _videoControlsUndocked = null;
            ShowVideoPlayer();
            SetLayout(_layout, true);
            mediaPlayer.Invalidate();
            Refresh();

            undockVideoControlsToolStripMenuItem.Visible = true;
            redockVideoControlsToolStripMenuItem.Visible = false;
            SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex, true);

            _textHeightResizeIgnoreUpdate = 0;
            Main_ResizeEnd(null, null);
        }

        private void CloseVideoToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (mediaPlayer.VideoPlayer != null)
            {
                mediaPlayer.PauseAndDisposePlayer();
            }

            mediaPlayer.SetPlayerName(string.Empty);
            mediaPlayer.ResetTimeLabel();
            mediaPlayer.VideoPlayer = null;
            mediaPlayer.CurrentPosition = 0;
            _videoFileName = null;
            _videoInfo = null;
            VideoAudioTrackNumber = -1;
            labelVideoInfo.Text = _languageGeneral.NoVideoLoaded;
            audioVisualizer.WavePeaks = null;
            audioVisualizer.SetSpectrogram(null);
            audioVisualizer.ShotChanges = new List<double>();
            audioVisualizer.Chapters = Array.Empty<MatroskaChapter>();
            trackBarWaveformPosition.Value = 0;
            timeUpDownVideoPositionAdjust.TimeCode = new TimeCode();
            timeUpDownVideoPositionAdjust.Enabled = false;
            timeUpDownVideoPosition.TimeCode = new TimeCode();
            timeUpDownVideoPosition.Enabled = false;
            closeVideoToolStripMenuItem.Enabled = false;
            CheckSecondSubtitleReset();
        }

        private void ToolStripMenuItemVideoDropDownOpening(object sender, EventArgs e)
        {
            if (_isVideoControlsUndocked)
            {
                redockVideoControlsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls);
                undockVideoControlsToolStripMenuItem.ShortcutKeys = Keys.None;
            }
            else
            {
                undockVideoControlsToolStripMenuItem.ShortcutKeys = UiUtil.GetKeys(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls);
                redockVideoControlsToolStripMenuItem.ShortcutKeys = Keys.None;
            }

            closeVideoToolStripMenuItem.Enabled = !string.IsNullOrEmpty(_videoFileName);
            setVideoOffsetToolStripMenuItem.Enabled = !string.IsNullOrEmpty(_videoFileName);
            smpteTimeModedropFrameToolStripMenuItem.Enabled = !string.IsNullOrEmpty(_videoFileName);
            if (!string.IsNullOrEmpty(_videoFileName))
            {
                if (Configuration.Settings.General.CurrentVideoOffsetInMs != 0)
                {
                    setVideoOffsetToolStripMenuItem.Text = string.Format("{0} [{1}]", _language.Menu.Video.SetVideoOffset, new TimeCode(Configuration.Settings.General.CurrentVideoOffsetInMs).ToShortDisplayString());
                }
                else
                {
                    setVideoOffsetToolStripMenuItem.Text = _language.Menu.Video.SetVideoOffset;
                }

                smpteTimeModedropFrameToolStripMenuItem.Checked = Configuration.Settings.General.CurrentVideoIsSmpte;
            }

            toolStripMenuItemSetAudioTrack.Visible = false;
            openSecondSubtitleToolStripMenuItem.Visible = false;
            if (mediaPlayer.VideoPlayer is LibVlcDynamic libVlc)
            {
                try
                {
                    openSecondSubtitleToolStripMenuItem.Visible = true;
                    var audioTracks = libVlc.GetAudioTracks();
                    VideoAudioTrackNumber = libVlc.AudioTrackNumber;
                    if (audioTracks.Count > 1)
                    {
                        toolStripMenuItemSetAudioTrack.DropDownItems.Clear();
                        for (int i = 0; i < audioTracks.Count; i++)
                        {
                            var at = audioTracks[i];
                            toolStripMenuItemSetAudioTrack.DropDownItems.Add(string.IsNullOrWhiteSpace(at.Value) ? at.Key.ToString(CultureInfo.InvariantCulture) : at.Value, null, ChooseAudioTrack);
                            toolStripMenuItemSetAudioTrack.DropDownItems[toolStripMenuItemSetAudioTrack.DropDownItems.Count - 1].Tag = at.Key.ToString(CultureInfo.InvariantCulture);
                            if (at.Key == VideoAudioTrackNumber)
                            {
                                ((ToolStripMenuItem)toolStripMenuItemSetAudioTrack.DropDownItems[toolStripMenuItemSetAudioTrack.DropDownItems.Count - 1]).Checked = true;
                            }
                        }

                        toolStripMenuItemSetAudioTrack.Visible = true;
                        UiUtil.FixFonts(toolStripMenuItemSetAudioTrack);
                    }
                }
                catch (Exception exception)
                {
                    openSecondSubtitleToolStripMenuItem.Visible = false;
                    toolStripMenuItemSetAudioTrack.Visible = false;
                    SeLogger.Error(exception, "VideoDropDownOpening failed getting audio tracks from vlc");
                }
            }
            else if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
            {
                try
                {
                    openSecondSubtitleToolStripMenuItem.Visible = true;
                    var audioTracks = libMpv.AudioTracks;
                    VideoAudioTrackNumber = libMpv.AudioTrackNumber;
                    if (audioTracks.Count > 1)
                    {
                        toolStripMenuItemSetAudioTrack.DropDownItems.Clear();
                        for (int i = 0; i < audioTracks.Count; i++)
                        {
                            var at = audioTracks[i];
                            var trackText = string.IsNullOrWhiteSpace(at.Value) ? at.Key.ToString(CultureInfo.InvariantCulture) : "Track " + at.Key + " - " + char.ToUpper(at.Value[0]) + at.Value.Substring(1);
                            toolStripMenuItemSetAudioTrack.DropDownItems.Add(trackText, null, ChooseAudioTrack);
                            toolStripMenuItemSetAudioTrack.DropDownItems[toolStripMenuItemSetAudioTrack.DropDownItems.Count - 1].Tag = at.Key.ToString(CultureInfo.InvariantCulture);
                            if (i == VideoAudioTrackNumber)
                            {
                                ((ToolStripMenuItem)toolStripMenuItemSetAudioTrack.DropDownItems[toolStripMenuItemSetAudioTrack.DropDownItems.Count - 1]).Checked = true;
                            }
                        }

                        toolStripMenuItemSetAudioTrack.Visible = true;
                        UiUtil.FixFonts(toolStripMenuItemSetAudioTrack);
                    }
                }
                catch (Exception exception)
                {
                    toolStripMenuItemSetAudioTrack.Visible = false;
                    openSecondSubtitleToolStripMenuItem.Visible = false;
                    toolStripMenuItemSetAudioTrack.Visible = false;
                    SeLogger.Error(exception, "VideoDropDownOpening failed getting audio tracks from mpv");
                }
            }

            if (mediaPlayer.VideoPlayer != null && audioVisualizer.WavePeaks != null && audioVisualizer.WavePeaks.Peaks.Count > 0)
            {
                toolStripMenuItemImportShotChanges.Visible = true;
                toolStripMenuItemListShotChanges.Visible = audioVisualizer.ShotChanges.Count > 0;
            }
            else
            {
                toolStripMenuItemImportShotChanges.Visible = false;
                toolStripMenuItemListShotChanges.Visible = false;
            }

            if (mediaPlayer.VideoPlayer != null && _videoFileName != null && _videoFileName.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
            {
                toolStripMenuItemImportChapters.Visible = true;
            }
            else
            {
                toolStripMenuItemImportChapters.Visible = false;
            }
        }

        private void ChooseAudioTrack(object sender, EventArgs e)
        {
            if (mediaPlayer.VideoPlayer is LibVlcDynamic libVlc)
            {
                var item = sender as ToolStripItem;
                var number = int.Parse(item.Tag.ToString());
                libVlc.AudioTrackNumber = number;
                VideoAudioTrackNumber = number;
            }
            else if (mediaPlayer.VideoPlayer is LibMpvDynamic libMpv)
            {
                var item = sender as ToolStripItem;
                var number = int.Parse(item.Tag.ToString());
                number--;
                libMpv.AudioTrackNumber = number;
                VideoAudioTrackNumber = number;
            }
        }

        private void textBoxListViewTextOriginal_TextChanged(object sender, EventArgs e)
        {
            if (_subtitleOriginal == null || _subtitleOriginal.Paragraphs.Count < 1)
            {
                return;
            }

            if (_subtitleListViewIndex >= 0)
            {
                var original = Utilities.GetOriginalParagraph(_subtitleListViewIndex, _subtitle.Paragraphs[_subtitleListViewIndex], _subtitleOriginal.Paragraphs);
                if (original != null)
                {
                    string text = textBoxListViewTextOriginal.Text.TrimEnd();

                    // update _subtitle + listview
                    original.Text = text;
                    UpdateListViewTextInfo(labelTextOriginalLineLengths, labelOriginalSingleLine, labelOriginalSingleLinePixels, labelTextOriginalLineTotal, labelOriginalCharactersPerSecond, original, textBoxListViewTextOriginal);
                    SubtitleListview1.SetOriginalText(_subtitleListViewIndex, text);
                    FixVerticalScrollBars(textBoxListViewTextOriginal);
                }
            }
        }

       

        private void SaveOriginalAstoolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_subtitleOriginal == null || _subtitleOriginal.Paragraphs.Count == 0)
            {
                return;
            }

            SubtitleFormat currentFormat = GetCurrentSubtitleFormat() ?? new SubRip();

            UiUtil.SetSaveDialogFilter(saveFileDialog1, currentFormat);

            saveFileDialog1.Title = _language.SaveOriginalSubtitleAs;
            saveFileDialog1.DefaultExt = "*" + currentFormat.Extension;
            saveFileDialog1.AddExtension = true;

            if (!string.IsNullOrEmpty(_videoFileName))
            {
                saveFileDialog1.FileName = Utilities.GetPathAndFileNameWithoutExtension(_videoFileName);
            }
            else if (!string.IsNullOrEmpty(_subtitleOriginalFileName))
            {
                saveFileDialog1.FileName = Utilities.GetPathAndFileNameWithoutExtension(_subtitleOriginalFileName);
            }
            else
            {
                saveFileDialog1.FileName = string.Empty;
            }

            if (!string.IsNullOrEmpty(openFileDialog1.InitialDirectory))
            {
                saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
            }

            DialogResult result = saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _subtitleOriginalFileName = saveFileDialog1.FileName;
                SaveOriginalSubtitle(currentFormat);
                SetTitle();
                if (_fileName != null)
                {
                    Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                }
            }
        }

        private void SaveOriginalToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_subtitleOriginalFileName))
            {
                SaveOriginalAstoolStripMenuItemClick(null, null);
                return;
            }

            try
            {
                SaveOriginalSubtitle(GetCurrentSubtitleFormat());
            }
            catch
            {
                MessageBox.Show(string.Format(_language.UnableToSaveSubtitleX, _subtitleOriginalFileName));
            }
        }

        private void RemoveOriginal(bool removeFromListView, bool updateRecentFiles)
        {
            _isOriginalActive = false;
            if (removeFromListView)
            {
                SubtitleListview1.HideColumn(SubtitleListView.SubtitleColumn.TextOriginal);
                SubtitleListview1.AutoSizeAllColumns(this);
                _subtitleOriginal = new Subtitle();
                _subtitleOriginalFileName = null;

                if (_fileName != null && updateRecentFiles)
                {
                    Configuration.Settings.RecentFiles.Add(_fileName, FirstVisibleIndex, FirstSelectedIndex, _videoFileName, VideoAudioTrackNumber, _subtitleOriginalFileName, Configuration.Settings.General.CurrentVideoOffsetInMs, Configuration.Settings.General.CurrentVideoIsSmpte);
                    Configuration.Settings.Save();
                    UpdateRecentFilesUI();
                }
            }

            buttonUnBreak.Visible = true;
            buttonAutoBreak.Visible = true;
            textBoxListViewTextOriginal.Visible = false;
            labelOriginalText.Visible = false;
            labelOriginalCharactersPerSecond.Visible = false;
            labelTextOriginalLineLengths.Visible = false;
            labelOriginalSingleLine.Visible = false;
            labelOriginalSingleLinePixels.Visible = false;
            labelTextOriginalLineTotal.Visible = false;
            textBoxListViewText.Width = (groupBoxEdit.Width - (textBoxListViewText.Left + 8 + buttonUnBreak.Width));
            textBoxListViewText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            labelTextLineTotal.Left = 236;
            labelTextOriginalLineTotal.Left = 236;

            saveOriginalToolStripMenuItem.Enabled = false;
            saveOriginalAstoolStripMenuItem.Enabled = false;
            removeOriginalToolStripMenuItem.Enabled = false;

            MainResize();
            SetTitle();
            SubtitleListview1.SelectIndexAndEnsureVisible(_subtitleListViewIndex, true);
        }

        
        public void SetEndMinusGapAndStartNextHere(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p == null)
            {
                return;
            }

            if (mediaPlayer.VideoPlayer == null || string.IsNullOrEmpty(_videoFileName))
            {
                MessageBox.Show(_languageGeneral.NoVideoLoaded);
                return;
            }

            var oldParagraph = new Paragraph(p, false);

            double totalMillisecondsEnd = mediaPlayer.CurrentPosition * TimeCode.BaseUnit - MinGapBetweenLines;
            var newDurationMs = totalMillisecondsEnd - p.StartTime.TotalMilliseconds;
            if (!p.StartTime.IsMaxTime &&
                newDurationMs < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds &&
                newDurationMs > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
            {
                return;
            }

            var tc = new TimeCode(totalMillisecondsEnd);
            MakeHistoryForUndo(_language.BeforeSetEndAndVideoPosition + "  " + tc);
            _makeHistoryPaused = true;

            if (p.StartTime.IsMaxTime)
            {
                p.EndTime.TotalMilliseconds = totalMillisecondsEnd;
                p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(p.Text);
            }
            else
            {
                p.EndTime.TotalMilliseconds = totalMillisecondsEnd;
            }

            timeUpDownStartTime.TimeCode = p.StartTime;
            var durationInSeconds = (decimal)p.DurationTotalSeconds;
            if (durationInSeconds >= numericUpDownDuration.Minimum && durationInSeconds <= numericUpDownDuration.Maximum)
            {
                SetDurationInSeconds((double)durationInSeconds);
            }

            RestartHistory();

            var next = _subtitle.GetParagraphOrDefault(index + 1);
            Paragraph oldNextParagraph = null;
            if (next != null)
            {
                oldNextParagraph = new Paragraph(next, false);
                next.StartTime.TotalMilliseconds = totalMillisecondsEnd + MinGapBetweenLines;
            }

            UpdateOriginalTimeCodes(oldParagraph, oldNextParagraph);
            SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
            SubtitleListview1.SetStartTimeAndDuration(index - 1, _subtitle.GetParagraphOrDefault(index - 1), _subtitle.GetParagraphOrDefault(index), _subtitle.GetParagraphOrDefault(index - 2));
            SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.GetParagraphOrDefault(index), _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
            SubtitleListview1.SetStartTimeAndDuration(index + 1, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index + 2), _subtitle.GetParagraphOrDefault(index));
            RefreshSelectedParagraph();
            ShowStatus(string.Format(_language.VideoControls.AdjustedViaEndTime, p.StartTime.ToShortString()));
            audioVisualizer.Invalidate();
            UpdateSourceView();
        }

        public void MainAdjustSetEndAndStartOfNextPlusGap(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p == null)
            {
                return;
            }

            if (mediaPlayer.VideoPlayer == null || string.IsNullOrEmpty(_videoFileName))
            {
                MessageBox.Show(_languageGeneral.NoVideoLoaded);
                return;
            }

            double totalMillisecondsEnd = mediaPlayer.CurrentPosition * TimeCode.BaseUnit;
            var oldParagraph = new Paragraph(p, false);
            var newDurationMs = totalMillisecondsEnd - p.StartTime.TotalMilliseconds;
            if (!p.StartTime.IsMaxTime &&
                newDurationMs < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds &&
                newDurationMs > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
            {
                return;
            }

            var tc = new TimeCode(totalMillisecondsEnd);
            MakeHistoryForUndo(_language.BeforeSetEndAndVideoPosition + "  " + tc);
            _makeHistoryPaused = true;

            if (p.StartTime.IsMaxTime)
            {
                p.EndTime.TotalMilliseconds = totalMillisecondsEnd;
                p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(p.Text);
            }
            else
            {
                p.EndTime.TotalMilliseconds = totalMillisecondsEnd;
            }

            timeUpDownStartTime.TimeCode = p.StartTime;
            var durationInSeconds = (decimal)p.DurationTotalSeconds;
            if (durationInSeconds >= numericUpDownDuration.Minimum && durationInSeconds <= numericUpDownDuration.Maximum)
            {
                SetDurationInSeconds((double)durationInSeconds);
            }

            var next = _subtitle.GetParagraphOrDefault(index + 1);
            Paragraph oldNextParagraph = null;
            if (next != null && (next.StartTime.IsMaxTime || next.StartTime.TotalMilliseconds - totalMillisecondsEnd < 5_000))
            {
                oldNextParagraph = new Paragraph(next, false);
                next.StartTime.TotalMilliseconds = totalMillisecondsEnd + MinGapBetweenLines;

                if (next.StartTime.IsMaxTime)
                {
                    next.EndTime.TotalMilliseconds = totalMillisecondsEnd + MinGapBetweenLines + Configuration.Settings.General.NewEmptyDefaultMs;
                }
            }

            RestartHistory();

            UpdateOriginalTimeCodes(oldParagraph, oldNextParagraph);
            SubtitleListview1.SetStartTimeAndDuration(index - 1, _subtitle.GetParagraphOrDefault(index - 1), _subtitle.GetParagraphOrDefault(index), _subtitle.GetParagraphOrDefault(index - 2));
            SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.GetParagraphOrDefault(index), _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
            SubtitleListview1.SetStartTimeAndDuration(index + 1, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index + 2), _subtitle.GetParagraphOrDefault(index));
            RefreshSelectedParagraph();
            ShowStatus(string.Format(_language.VideoControls.AdjustedViaEndTime, p.StartTime.ToShortString()));
            audioVisualizer.Invalidate();
            UpdateSourceView();
        }

        public void SetCurrentViaEndPositionAndGotoNext(int index, bool goToNext)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p == null)
            {
                return;
            }

            if (mediaPlayer.VideoPlayer == null || string.IsNullOrEmpty(_videoFileName))
            {
                MessageBox.Show(_languageGeneral.NoVideoLoaded);
                return;
            }

            var oldParagraph = new Paragraph(p, false);

            //if (autoDuration)
            //{
            //    // TODO: auto duration
            //    // TODO: Search for start via wave file (must only be minor adjustment)
            //}

            // current movie Position
            double durationTotalMilliseconds = p.DurationTotalMilliseconds;
            double totalMillisecondsEnd = mediaPlayer.CurrentPosition * TimeCode.BaseUnit;

            var tc = new TimeCode(totalMillisecondsEnd - durationTotalMilliseconds);
            MakeHistoryForUndo(_language.BeforeSetEndAndVideoPosition + "  " + tc);
            _makeHistoryPaused = true;

            if (p.StartTime.IsMaxTime)
            {
                p.EndTime.TotalSeconds = mediaPlayer.CurrentPosition;
                p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(p.Text);
            }
            else
            {
                p.StartTime.TotalMilliseconds = totalMillisecondsEnd - durationTotalMilliseconds;
                p.EndTime.TotalMilliseconds = totalMillisecondsEnd;
            }

            timeUpDownStartTime.TimeCode = p.StartTime;
            var durationInSeconds = (decimal)(p.DurationTotalSeconds);
            if (durationInSeconds >= numericUpDownDuration.Minimum && durationInSeconds <= numericUpDownDuration.Maximum)
            {
                SetDurationInSeconds((double)durationInSeconds);
            }

            UpdateOriginalTimeCodes(oldParagraph);
            RestartHistory();

            if (goToNext)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
            }

            ShowStatus(string.Format(_language.VideoControls.AdjustedViaEndTime, p.StartTime.ToShortString()));
            audioVisualizer.Invalidate();
            UpdateSourceView();
        }

        public void SetCurrentStartAutoDurationAndGotoNext(int index)
        {
            var prev = _subtitle.GetParagraphOrDefault(index - 1);
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p == null)
            {
                return;
            }

            double videoPosition = mediaPlayer.CurrentPosition;
            if (prev != null && Math.Abs(prev.StartTime.TotalSeconds - videoPosition) < 0.3)
            {
                ShowStatus("Subtitle already here");
                return;
            }

            ShowNextSubtitleLabel();

            if (mediaPlayer.VideoPlayer == null || string.IsNullOrEmpty(_videoFileName))
            {
                MessageBox.Show(_languageGeneral.NoVideoLoaded);
                return;
            }

            MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + p.Number + " " + p.Text));

            timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
            var oldParagraph = new Paragraph(_subtitle.Paragraphs[index], false);
            if (!mediaPlayer.IsPaused)
            {
                videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
            }

            timeUpDownStartTime.TimeCode = TimeCode.FromSeconds(videoPosition);

            double duration = Utilities.GetOptimalDisplayMilliseconds(textBoxListViewText.Text);

            _subtitle.Paragraphs[index].StartTime.TotalMilliseconds = TimeSpan.FromSeconds(videoPosition).TotalMilliseconds;
            if (prev != null && prev.EndTime.TotalMilliseconds > _subtitle.Paragraphs[index].StartTime.TotalMilliseconds)
            {
                int minDiff = MinGapBetweenLines + 1;
                if (minDiff < 1)
                {
                    minDiff = 1;
                }

                prev.EndTime.TotalMilliseconds = _subtitle.Paragraphs[index].StartTime.TotalMilliseconds - minDiff;
            }

            _subtitle.Paragraphs[index].EndTime.TotalMilliseconds = _subtitle.Paragraphs[index].StartTime.TotalMilliseconds + duration;
            SubtitleListview1.SetStartTimeAndDuration(index, _subtitle.Paragraphs[index], _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
            timeUpDownStartTime.TimeCode = _subtitle.Paragraphs[index].StartTime;
            timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
            UpdateOriginalTimeCodes(oldParagraph);
            _subtitleListViewIndex = -1;
            SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
            audioVisualizer.Invalidate();
            UpdateSourceView();
        }

        public void SetCurrentEndNextStartAndGoToNext(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            var next = _subtitle.GetParagraphOrDefault(index + 1);
            if (p == null)
            {
                return;
            }

            if (mediaPlayer.VideoPlayer == null || string.IsNullOrEmpty(_videoFileName))
            {
                MessageBox.Show(_languageGeneral.NoVideoLoaded);
                return;
            }

            MakeHistoryForUndoOnlyIfNotRecent(string.Format(_language.VideoControls.BeforeChangingTimeInWaveformX, "#" + p.Number + " " + p.Text));
            var p1 = new Paragraph(p, false);
            Paragraph p2 = null;
            if (next != null)
            {
                p2 = new Paragraph(next, false);
            }

            double videoPosition = mediaPlayer.CurrentPosition;
            if (!mediaPlayer.IsPaused)
            {
                videoPosition -= Configuration.Settings.General.SetStartEndHumanDelay / TimeCode.BaseUnit;
            }

            p.EndTime = TimeCode.FromSeconds(videoPosition);
            if (p.StartTime.IsMaxTime)
            {
                timeUpDownStartTime.MaskedTextBox.TextChanged -= MaskedTextBoxTextChanged;
                p.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds - Utilities.GetOptimalDisplayMilliseconds(p.Text);
                if (p.StartTime.TotalMilliseconds < 0)
                {
                    p.StartTime.TotalMilliseconds = 0;
                }

                timeUpDownStartTime.TimeCode = p.StartTime;
                SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));
                timeUpDownStartTime.MaskedTextBox.TextChanged += MaskedTextBoxTextChanged;
            }

            if (p.DurationTotalSeconds < 0)
            {
                p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
            }

            SubtitleListview1.SetStartTimeAndDuration(index, p, _subtitle.GetParagraphOrDefault(index + 1), _subtitle.GetParagraphOrDefault(index - 1));

            SetDurationInSeconds(_subtitle.Paragraphs[index].DurationTotalSeconds + 0.001);
            if (next != null)
            {
                int addMilliseconds = MinGapBetweenLines;
                if (addMilliseconds < 1 || addMilliseconds > 500)
                {
                    addMilliseconds = 1;
                }

                var oldDuration = next.DurationTotalMilliseconds;
                if (next.StartTime.IsMaxTime || next.EndTime.IsMaxTime)
                {
                    oldDuration = Utilities.GetOptimalDisplayMilliseconds(p.Text);
                }

                next.StartTime.TotalMilliseconds = p.EndTime.TotalMilliseconds + addMilliseconds;
                next.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds + oldDuration;
                SubtitleListview1.SetStartTimeAndDuration(index + 1, next, _subtitle.GetParagraphOrDefault(index + 2), _subtitle.GetParagraphOrDefault(index));
                SubtitleListview1.SelectIndexAndEnsureVisible(index + 1, true);
            }

            UpdateOriginalTimeCodes(p1, p2);
            audioVisualizer.Invalidate();
            UpdateSourceView();
        }


        private void TimerTextUndoTick(object sender, EventArgs e)
        {
            if (_subtitle == null || _subtitle.Paragraphs.Count == 0 || _listViewTextTicks == -1 || !CanFocus)
            {
                return;
            }

            // progress check
            ShowTranslationProgress();

            // text undo
            int index = _listViewTextUndoIndex;
            if (index == -1)
            {
                index = _subtitleListViewIndex;
            }

            if (index < 0 || index >= _subtitle.Paragraphs.Count)
            {
                return;
            }

            if ((DateTime.UtcNow.Ticks - _listViewTextTicks) > 10000 * 700) // only if last typed char was entered > 700 milliseconds
            {
                var p = _subtitle.GetParagraphOrDefault(index);
                if (p == null)
                {
                    return;
                }

                string newText = p.Text.TrimEnd();
                string oldText = _listViewTextUndoLast;
                if (oldText == null)
                {
                    return;
                }

                if (_listViewTextUndoLast != newText)
                {
                    MakeHistoryForUndo(_languageGeneral.Text + ": " + _listViewTextUndoLast.TrimEnd() + " -> " + newText, false);

                    int hidx = _subtitle.HistoryItems.Count - 1;
                    if (hidx >= 0 && hidx < _subtitle.HistoryItems.Count)
                    {
                        var historyParagraph = _subtitle.HistoryItems[hidx].Subtitle.GetParagraphOrDefault(index);
                        if (historyParagraph != null)
                        {
                            historyParagraph.Text = _listViewTextUndoLast;
                        }
                    }

                    _listViewTextUndoLast = newText;
                    _listViewTextUndoIndex = -1;
                }
            }
        }

        private void ShowTranslationProgress()
        {
            if (Configuration.Settings.General.ShowProgress)
            {
                if (_subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
                {
                    int numberOfNonBlankLines = 0;
                    foreach (var paragraph in _subtitle.Paragraphs)
                    {
                        if (!string.IsNullOrWhiteSpace(paragraph.Text))
                        {
                            numberOfNonBlankLines++;
                        }
                    }

                    int percent = (int)Math.Round(numberOfNonBlankLines * 100.0 / _subtitle.Paragraphs.Count);
                    toolStripStatusLabelProgress.Text = string.Format("{0}% completed", percent);
                    if (!toolStripStatusLabelProgress.Visible)
                    {
                        toolStripStatusLabelProgress.Visible = true;
                    }
                }
                else if (_subtitle.Paragraphs.Count > 0 && !string.IsNullOrWhiteSpace(_videoFileName) && mediaPlayer != null && mediaPlayer.VideoPlayer != null && mediaPlayer.VideoPlayer.Duration > 0)
                {
                    var last = _subtitle.Paragraphs.LastOrDefault();
                    if (last != null && !last.StartTime.IsMaxTime)
                    {
                        var subtitleEndSeconds = last.EndTime.TotalSeconds;
                        var videoEndSeconds = mediaPlayer.VideoPlayer.Duration;
                        int percent = (int)Math.Round(subtitleEndSeconds * 100.0 / videoEndSeconds);
                        toolStripStatusLabelProgress.Text = string.Format("{0}% completed", percent);
                        if (!toolStripStatusLabelProgress.Visible)
                        {
                            toolStripStatusLabelProgress.Visible = true;
                        }
                    }
                }
                else
                {
                    toolStripStatusLabelProgress.Visible = false;
                }
            }
        }

        private void TimerOriginalTextUndoTick(object sender, EventArgs e)
        {
            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                int index = _listViewTextUndoIndex;
                if (index == -1)
                {
                    index = _subtitleListViewIndex;
                }

                if (_listViewOriginalTextTicks == -1 || !CanFocus || _subtitleOriginal == null || _subtitleOriginal.Paragraphs.Count == 0 || index < 0 || index >= _subtitleOriginal.Paragraphs.Count)
                {
                    return;
                }

                if ((DateTime.UtcNow.Ticks - _listViewOriginalTextTicks) > 10000 * 700) // only if last typed char was entered > 700 milliseconds
                {
                    var original = Utilities.GetOriginalParagraph(index, _subtitle.Paragraphs[index], _subtitleOriginal.Paragraphs);
                    if (original != null)
                    {
                        index = _subtitleOriginal.Paragraphs.IndexOf(original);
                    }
                    else
                    {
                        return;
                    }

                    string newText = _subtitleOriginal.Paragraphs[index].Text.TrimEnd();
                    string oldText = _listViewOriginalTextUndoLast;
                    if (oldText == null || index < 0)
                    {
                        return;
                    }

                    if (_listViewOriginalTextUndoLast != newText && _subtitle.HistoryItems.Count > 0 &&
                        index < _subtitle.HistoryItems[_subtitle.HistoryItems.Count - 1].OriginalSubtitle.Paragraphs.Count)
                    {
                        MakeHistoryForUndo(_languageGeneral.Text + ": " + _listViewOriginalTextUndoLast.TrimEnd() + " -> " + newText, false);
                        _subtitle.HistoryItems[_subtitle.HistoryItems.Count - 1].OriginalSubtitle.Paragraphs[index].Text = _listViewOriginalTextUndoLast;

                        _listViewOriginalTextUndoLast = newText;
                        _listViewTextUndoIndex = -1;
                    }
                }
            }
        }

        private void UpdatePositionAndTotalLength(Label lineTotal, SETextBox textBox)
        {
            var text = textBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                lineTotal.Text = string.Empty;
                return;
            }

            int extraNewLineLength = Environment.NewLine.Length - 1;

            int lineBreakPos = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            int pos = textBox.SelectionStart;
            var textNoHtml = HtmlUtil.RemoveHtmlTags(text, true);
            var s = textNoHtml.Replace(Environment.NewLine, string.Empty); // we don't count new line in total length... correct?
            var totalLength = s.CountCharacters(false);
            string totalL;

            if (Configuration.Settings.Tools.ListViewSyntaxColorWideLines)
            {
                var totalLengthPixels = TextWidth.CalcPixelWidth(textNoHtml.RemoveChar('\r', '\n'));
                totalL = "     " + string.Format(_languageGeneral.TotalLengthX, string.Format("{0}     {1}", totalLength, totalLengthPixels));
            }
            else
            {
                totalL = "     " + string.Format(_languageGeneral.TotalLengthX, totalLength);
            }

            if (textBox.SelectionLength > 0)
            {
                var len = textBox.SelectedText.CountCharacters(false);
                if (len > 0)
                {
                    lineTotal.Text = textBox.SelectionLength.ToString(CultureInfo.InvariantCulture) + totalL;
                    lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
                    return;
                }
            }

            if (lineBreakPos < 0 || pos <= lineBreakPos)
            {
                lineTotal.Text = "1," + (pos + 1) + totalL;
                lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
                return;
            }

            int secondLineBreakPos = text.IndexOf(Environment.NewLine, lineBreakPos + 1, StringComparison.Ordinal);
            if (secondLineBreakPos < 0 || pos <= secondLineBreakPos + extraNewLineLength)
            {
                lineTotal.Text = "2," + (pos - (lineBreakPos + extraNewLineLength)) + totalL;
                lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
                return;
            }

            int thirdLineBreakPos = text.IndexOf(Environment.NewLine, secondLineBreakPos + 1, StringComparison.Ordinal);
            if (thirdLineBreakPos < 0 || pos < thirdLineBreakPos + (extraNewLineLength * 2))
            {
                lineTotal.Text = "3," + (pos - (secondLineBreakPos + extraNewLineLength)) + totalL;
                lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
                return;
            }

            int forthLineBreakPos = text.IndexOf(Environment.NewLine, thirdLineBreakPos + 1, StringComparison.Ordinal);
            if (forthLineBreakPos < 0 || pos < forthLineBreakPos + (extraNewLineLength * 3))
            {
                lineTotal.Text = "4," + (pos - (thirdLineBreakPos + extraNewLineLength)) + totalL;
                lineTotal.Left = textBox.Left + (textBox.Width - lineTotal.Width);
                return;
            }

            lineTotal.Text = string.Empty;
        }


        public void GotoNextSubPosFromVideoPos()
        {
            if (mediaPlayer.VideoPlayer != null && _subtitle != null)
            {
                double ms = Math.Round(mediaPlayer.CurrentPosition * TimeCode.BaseUnit, MidpointRounding.AwayFromZero);
                foreach (var p in _subtitle.Paragraphs)
                {
                    if (p.EndTime.TotalMilliseconds > ms && p.StartTime.TotalMilliseconds < ms)
                    {
                        // current sub
                    }
                    else if (p.DurationTotalSeconds < 10 && p.StartTime.TotalMilliseconds > ms)
                    {
                        mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                        SubtitleListview1.SelectIndexAndEnsureVisible(_subtitle.GetIndex(p), true);
                        return;
                    }
                }
            }
        }

        public void GotoPrevSubPosFromvideoPos()
        {
            if (mediaPlayer.VideoPlayer != null && _subtitle != null)
            {
                double ms = Math.Round(mediaPlayer.CurrentPosition * TimeCode.BaseUnit, MidpointRounding.AwayFromZero);
                int i = _subtitle.Paragraphs.Count - 1;
                while (i >= 0)
                {
                    var p = _subtitle.Paragraphs[i];
                    if (p.EndTime.TotalMilliseconds > ms && p.StartTime.TotalMilliseconds < ms)
                    {
                        // current sub
                    }
                    else if (p.DurationTotalSeconds < 10 && p.StartTime.TotalMilliseconds < ms)
                    {
                        mediaPlayer.CurrentPosition = p.StartTime.TotalSeconds;
                        SubtitleListview1.SelectIndexAndEnsureVisible(_subtitle.GetIndex(p), true);
                        return;
                    }

                    i--;
                }
            }
        }


        private void SurroundWithTag(string tag, string endTag = "", bool selectedTextOnly = false)
        {
            if (selectedTextOnly)
            {
                var tb = GetFocusedTextBox();
                var text = tb.SelectedText;
                if (string.IsNullOrEmpty(text) && tb.Text.Length > 0)
                {
                    text = tb.Text;
                    tb.SelectAll();
                }

                int selectionStart = tb.SelectionStart;
                text = Utilities.ToggleSymbols(tag, text, endTag, out var _);
                tb.SelectedText = text;
                tb.SelectionStart = selectionStart;
                tb.SelectionLength = text.Length;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tag) && string.IsNullOrEmpty(endTag))
                {
                    return;
                }

                if (_subtitle.Paragraphs.Count > 0 && SubtitleListview1.SelectedItems.Count > 0)
                {
                    SubtitleListview1.SelectedIndexChanged -= SubtitleListview1_SelectedIndexChanged;
                    MakeHistoryForUndo(string.Format(_language.BeforeAddingTagX, tag));

                    var indices = new List<int>();
                    foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                    {
                        indices.Add(item.Index);
                    }

                    SubtitleListview1.BeginUpdate();
                    var first = true;
                    var addTags = true;
                    foreach (int i in indices)
                    {
                        if (first)
                        {
                            _subtitle.Paragraphs[i].Text = Utilities.ToggleSymbols(tag, _subtitle.Paragraphs[i].Text, endTag, out var added);
                            addTags = added;
                            first = false;
                        }


                        if (addTags)
                        {
                            _subtitle.Paragraphs[i].Text = Utilities.AddSymbols(tag, _subtitle.Paragraphs[i].Text, endTag);
                        }
                        else
                        {
                            _subtitle.Paragraphs[i].Text = Utilities.RemoveSymbols(tag, _subtitle.Paragraphs[i].Text, endTag);
                        }

                        SubtitleListview1.SetText(i, _subtitle.Paragraphs[i].Text);

                        if (IsOriginalEditable)
                        {
                            var original = Utilities.GetOriginalParagraph(i, _subtitle.Paragraphs[i], _subtitleOriginal.Paragraphs);
                            if (original != null)
                            {
                                if (addTags)
                                {
                                    original.Text = Utilities.AddSymbols(tag, original.Text, endTag);
                                }
                                else
                                {
                                    original.Text = Utilities.RemoveSymbols(tag, original.Text, endTag);
                                }

                                SubtitleListview1.SetOriginalText(i, original.Text);
                            }
                        }
                    }

                    SubtitleListview1.EndUpdate();

                    ShowStatus(string.Format(_language.TagXAdded, tag));
                    UpdateSourceView();
                    RefreshSelectedParagraph();
                    SubtitleListview1.SelectedIndexChanged += SubtitleListview1_SelectedIndexChanged;
                }
            }
        }

        private void ToolStripMenuItemRightToLeftModeClick(object sender, EventArgs e)
        {
            var focusedItem = SubtitleListview1.FocusedItem;
            toolStripMenuItemRightToLeftMode.Checked = !toolStripMenuItemRightToLeftMode.Checked;
            if (!toolStripMenuItemRightToLeftMode.Checked)
            {
                RightToLeft = RightToLeft.No;
                SubtitleListview1.RightToLeft = RightToLeft.No;
                SubtitleListview1.RightToLeftLayout = false;
                textBoxSource.RightToLeft = RightToLeft.No;
                mediaPlayer.TextRightToLeft = RightToLeft.No;
                textBoxSearchWord.RightToLeft = RightToLeft.No;
                Configuration.Settings.General.RightToLeftMode = false;
            }
            else
            {
                //RightToLeft = RightToLeft.Yes; - is this better? TimeUpDown custom control needs to support RTL before enabling this
                SubtitleListview1.RightToLeft = RightToLeft.Yes;
                SubtitleListview1.RightToLeftLayout = true;
                textBoxSource.RightToLeft = RightToLeft.Yes;
                mediaPlayer.TextRightToLeft = RightToLeft.Yes;
                textBoxSearchWord.RightToLeft = RightToLeft.Yes;
                Configuration.Settings.General.RightToLeftMode = true;
            }

            MainResize();
            TextBoxListViewTextTextChanged(null, null);
            textBoxListViewTextOriginal_TextChanged(null, null);
            if (focusedItem != null)
            {
                SubtitleListview1.SelectIndexAndEnsureVisible(focusedItem.Index, true);
            }
        }

        
        public void ApplySsaStyles(StylesForm styles)
        {
            if (_subtitle.Header != styles.Header)
            {
                MakeHistoryForUndo(styles.Text);
            }

            _subtitle.Header = styles.Header;
            var styleList = AdvancedSubStationAlpha.GetStylesFromHeader(_subtitle.Header);
            if ((styles as SubStationAlphaStyles).RenameActions.Count > 0)
            {
                foreach (var renameAction in (styles as SubStationAlphaStyles).RenameActions)
                {
                    for (var i = 0; i < _subtitle.Paragraphs.Count; i++)
                    {
                        var p = _subtitle.Paragraphs[i];
                        if (p.Extra == renameAction.OldName)
                        {
                            p.Extra = renameAction.NewName;
                        }
                    }
                }

                CleanRemovedStyles(styleList, false);
                SaveSubtitleListviewIndices();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                RestoreSubtitleListviewIndices();
            }
            else
            {
                CleanRemovedStyles(styleList, true);
            }
        }

        public void ApplyAssaStyles(AssaStyles styles)
        {
            if (_subtitle.Header != styles.Header)
            {
                MakeHistoryForUndo(styles.Text);
            }

            _subtitle.Header = styles.Header;
            var styleList = AdvancedSubStationAlpha.GetStylesFromHeader(_subtitle.Header);
            if (styles.RenameActions.Count > 0)
            {
                foreach (var renameAction in styles.RenameActions)
                {
                    for (var i = 0; i < _subtitle.Paragraphs.Count; i++)
                    {
                        var p = _subtitle.Paragraphs[i];
                        if (p.Extra == renameAction.OldName)
                        {
                            p.Extra = renameAction.NewName;
                        }
                    }
                }

                CleanRemovedStyles(styleList, false);
                SaveSubtitleListviewIndices();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                RestoreSubtitleListviewIndices();
            }
            else
            {
                CleanRemovedStyles(styleList, true);
            }
        }

        private void CleanRemovedStyles(List<string> styleList, bool updateListView)
        {
            var largeUpdate = false;
            if (updateListView)
            {
                if (_subtitle.Paragraphs.Count > 1000)
                {
                    largeUpdate = true;
                }
                else
                {
                    SubtitleListview1.BeginUpdate();
                }
            }

            for (var i = 0; i < _subtitle.Paragraphs.Count; i++)
            {
                var p = _subtitle.Paragraphs[i];
                if (p.Extra == null || !styleList.Any(s => s.Equals(p.Extra == "*Default" ? "Default" : p.Extra, StringComparison.OrdinalIgnoreCase)))
                {
                    p.Extra = styleList[0];

                    if (updateListView && !largeUpdate)
                    {
                        SubtitleListview1.SetExtraText(i, p.Extra, SubtitleListview1.ForeColor);
                    }
                }
            }

            if (updateListView)
            {
                if (largeUpdate)
                {
                    SaveSubtitleListviewIndices();
                    SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                    RestoreSubtitleListviewIndices();

                }
                else
                {
                    SubtitleListview1.EndUpdate();
                }
            }
        }

        private static string SetAlignTag(string s, string tag)
        {
            var text = HtmlUtil.RemoveAssAlignmentTags(s);
            if (text.StartsWith("{\\", StringComparison.Ordinal) && text.Contains('}'))
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    return text.Insert(1, "\\" + tag.TrimStart('{').TrimStart('\\').TrimEnd('}'));
                }

                return text;
            }
            else
            {
                return string.Format(@"{0}{1}", tag, text);
            }
        }

        
        private void LabelStatusClick(object sender, EventArgs e)
        {
            if (_statusLog.Count == 0)
            {
                return;
            }

            if (_statusLogForm == null || _statusLogForm.IsDisposed)
            {
                _statusLogForm = new StatusLog(_statusLog);
                _statusLogForm.Show(this);
            }
            else
            {
                _statusLogForm.Show();
            }
        }


        private void ToolStripMenuItemUndoClick(object sender, EventArgs e)
        {
            UndoToIndex(true);
        }

        private void ToolStripMenuItemRedoClick(object sender, EventArgs e)
        {
            UndoToIndex(false);
        }

        
        private Subtitle GetSaveSubtitle(Subtitle subtitle)
        {
            var sub = new Subtitle(subtitle);
            if (string.IsNullOrEmpty(sub.FileName))
            {
                sub.FileName = "Untitled";
            }

            if (Configuration.Settings.General.CurrentVideoOffsetInMs != 0)
            {
                sub.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(Configuration.Settings.General.CurrentVideoOffsetInMs));
            }

            return sub;
        }


        private void PasteIntoActiveTextBox(string s, bool allowMultiLine = false)
        {
            if (InSourceView)
            {
                textBoxSource.SelectedText = s;
            }
            else
            {
                if (textBoxListViewTextOriginal.Visible && textBoxListViewTextOriginal.Enabled && textBoxListViewTextOriginal.Focused)
                {
                    if (!string.IsNullOrEmpty(textBoxListViewTextOriginal.SelectedText))
                    {
                        textBoxListViewTextOriginal.SelectedText = s;
                    }
                    else
                    {
                        var selectionStart = textBoxListViewTextOriginal.SelectionStart;
                        textBoxListViewTextOriginal.Text = textBoxListViewTextOriginal.Text.Insert(textBoxListViewTextOriginal.SelectionStart, s);
                        textBoxListViewTextOriginal.SelectionStart = selectionStart + s.Length;
                    }
                }
                else
                {
                    if (SubtitleListview1.SelectedItems.Count > 1 && !textBoxListViewText.Focused && allowMultiLine)
                    {
                        foreach (ListViewItem item in SubtitleListview1.SelectedItems)
                        {
                            var p = _subtitle.GetParagraphOrDefault(item.Index);
                            if (p == null)
                            {
                                continue;
                            }

                            p.Text = s + " " + p.Text;
                            SubtitleListview1.SetText(item.Index, p.Text);
                        }

                        RefreshSelectedParagraph();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(textBoxListViewText.SelectedText))
                        {
                            textBoxListViewText.SelectedText = s;
                        }
                        else
                        {
                            var selectionStart = textBoxListViewText.SelectionStart;
                            textBoxListViewText.Text = textBoxListViewText.Text.Insert(textBoxListViewText.SelectionStart, s);
                            textBoxListViewText.SelectionStart = selectionStart + s.Length;
                        }
                    }

                    UpdateSourceView();
                }
            }
        }

        private void ToolStripMenuItemRtlUnicodeControlCharsClick(object sender, EventArgs e)
        {
            if (IsUnicode)
            {
                MakeHistoryForUndo(toolStripMenuItemRtlUnicodeControlChars.Text);
                int selectedIndex = FirstSelectedIndex;
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    var p = _subtitle.Paragraphs[index];
                    p.Text = Utilities.FixRtlViaUnicodeChars(p.Text);
                    SubtitleListview1.SetText(index, p.Text);
                    if (index == selectedIndex)
                    {
                        textBoxListViewText.Text = p.Text;
                    }
                }

                RefreshSelectedParagraph();
            }
        }

        private void ToolStripMenuItemRemoveUnicodeControlCharsClick(object sender, EventArgs e)
        {
            if (IsUnicode)
            {
                MakeHistoryForUndo(toolStripMenuItemRemoveUnicodeControlChars.Text);
                int selectedIndex = FirstSelectedIndex;
                foreach (int index in SubtitleListview1.SelectedIndices)
                {
                    var p = _subtitle.Paragraphs[index];
                    p.Text = Utilities.RemoveUnicodeControlChars(p.Text);
                    SubtitleListview1.SetText(index, p.Text);
                    if (index == selectedIndex)
                    {
                        textBoxListViewText.Text = p.Text;
                    }
                }

                RefreshSelectedParagraph();
            }
        }

        internal Subtitle UndoFromSpellCheck(Subtitle subtitle)
        {
            var idx = FirstSelectedIndex;
            for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
            {
                if (_subtitle.Paragraphs[i].Text != subtitle.Paragraphs[i].Text)
                {
                    _subtitle.Paragraphs[i].Text = subtitle.Paragraphs[i].Text;
                    SubtitleListview1.SetText(i, _subtitle.Paragraphs[i].Text);
                }

                if (idx == i)
                {
                    SubtitleListview1.SetText(idx, _subtitle.Paragraphs[idx].Text);
                }
            }

            RefreshSelectedParagraph();
            return _subtitle;
        }

        private void DisplaySubtitleNotLoadedMessage()
        {
            MessageBox.Show(this, _language.NoSubtitleLoaded, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ChooseProfile()
        {
            using (var form = new ProfileChoose(Configuration.Settings.General.Profiles, Configuration.Settings.General.CurrentProfile))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    SubtitleListview1.BeginUpdate();
                    for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
                    {
                        SubtitleListview1.SyntaxColorLine(_subtitle.Paragraphs, i, _subtitle.Paragraphs[i]);
                    }

                    SubtitleListview1.EndUpdate();
                    if (_subtitleListViewIndex >= 0)
                    {
                        UpdateListViewTextInfo(labelTextLineLengths, labelSingleLine, labelSingleLinePixels, labelTextLineTotal, labelCharactersPerSecond, _subtitle.Paragraphs[_subtitleListViewIndex], textBoxListViewText);
                    }

                    ShowLineInformationListView();
                    ShowSourceLineNumber();
                }
            }
        }

        private void DuplicateLine()
        {
            if (SubtitleListview1.SelectedItems.Count != 1)
            {
                return;
            }

            var firstSelectedIndex = SubtitleListview1.SelectedItems[0].Index;
            MakeHistoryForUndo(_language.BeforeInsertLine);
            var newParagraph = new Paragraph();
            SetStyleForNewParagraph(newParagraph, firstSelectedIndex);
            var cur = _subtitle.GetParagraphOrDefault(firstSelectedIndex);
            newParagraph.StartTime.TotalMilliseconds = cur.StartTime.TotalMilliseconds;
            newParagraph.EndTime.TotalMilliseconds = cur.EndTime.TotalMilliseconds;
            newParagraph.Text = cur.Text;

            if (Configuration.Settings.General.AllowEditOfOriginalSubtitle && _subtitleOriginal != null && _subtitleOriginal.Paragraphs.Count > 0)
            {
                var currentOriginal = Utilities.GetOriginalParagraph(firstSelectedIndex, _subtitle.Paragraphs[firstSelectedIndex], _subtitleOriginal.Paragraphs);
                if (currentOriginal != null)
                {
                    _subtitleOriginal.Paragraphs.Insert(_subtitleOriginal.Paragraphs.IndexOf(currentOriginal) + 1, new Paragraph(currentOriginal));
                }
                else
                {
                    _subtitleOriginal.InsertParagraphInCorrectTimeOrder(new Paragraph(newParagraph));
                }

                _subtitleOriginal.Renumber();
            }

            if (_networkSession != null)
            {
                _networkSession.TimerStop();
                NetworkGetSendUpdates(new List<int>(), firstSelectedIndex, newParagraph);
            }
            else
            {
                _subtitle.Paragraphs.Insert(firstSelectedIndex, newParagraph);
                _subtitle.Renumber();
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            }

            SubtitleListview1.SelectIndexAndEnsureVisible(firstSelectedIndex, true);
            UpdateSourceView();
            ShowStatus(_language.LineInserted);
        }

        private void UpdateToolbarButtonsToCurrentFormat(SubtitleFormat currentSubtitleFormat)
        {
            if (currentSubtitleFormat == null)
            {
                return;
            }

            var formatType = currentSubtitleFormat.GetType();

            var netflixIconOn = formatType == typeof(TimedText10) || formatType == typeof(NetflixTimedText) || formatType == typeof(NetflixImsc11Japanese) || formatType == typeof(Ebu);
            toolStripButtonNetflixQualityCheck.Visible = netflixIconOn && Configuration.Settings.General.ShowToolbarNetflixGlyphCheck;

            var assFormatOn = formatType == typeof(AdvancedSubStationAlpha);
            toolStripButtonAssStyleManager.Visible = assFormatOn;
            toolStripButtonAssStyleManager.ToolTipText = LanguageSettings.Current.SubStationAlphaStyles.Title;
            toolStripButtonAssProperties.Visible = assFormatOn;
            toolStripButtonAssaDraw.Visible = assFormatOn && File.Exists(Path.Combine(Configuration.PluginsDirectory, "AssaDraw.dll"));
            toolStripButtonAssAttachments.Visible = assFormatOn;

            toolStripMenuItemWebVttStyle.Visible = false;

            if (formatType == typeof(SubStationAlpha))
            {
                toolStripButtonAssStyleManager.Visible = true;
                toolStripButtonAssProperties.Visible = true;
                toolStripButtonAssAttachments.Visible = true;
            }

            if (formatType == typeof(AdvancedSubStationAlpha) || formatType == typeof(SubStationAlpha))
            {
                TryLoadIcon(toolStripButtonAssStyleManager, "AssaStyle");
            }

            toolStripButtonXProperties.Visible = formatType == typeof(ItunesTimedText);
            if (toolStripButtonXProperties.Visible)
            {
                toolStripButtonXProperties.ToolTipText = string.Format(_language.Menu.File.FormatXProperties, _currentSubtitleFormat?.Name);
                toolStripButtonXProperties.Image = Properties.Resources.itt;
                TryLoadIcon(toolStripButtonXProperties, "IttProperties");
            }

            if (formatType == typeof(WebVTT) || formatType == typeof(WebVTTFileWithLineNumber))
            {
                toolStripButtonXProperties.Visible = true;
                toolStripButtonXProperties.ToolTipText = string.Format(_language.Menu.File.FormatXProperties, new WebVTT().Name);
                toolStripButtonXProperties.Image = Properties.Resources.webvtt;
                TryLoadIcon(toolStripButtonXProperties, "WebVttProperties");

                toolStripButtonAssStyleManager.Visible = true;
                toolStripButtonAssStyleManager.ToolTipText = string.Format(LanguageSettings.Current.WebVttStyleManager.Title, new WebVTT().Name);
                toolStripButtonAssStyleManager.Image = Properties.Resources.webvtt;
                TryLoadIcon(toolStripButtonAssStyleManager, "WebVttStyle");

                toolStripMenuItemWebVttStyle.Visible = true;
            }

            if (formatType == typeof(Ebu))
            {
                toolStripButtonXProperties.Visible = true;
                toolStripButtonXProperties.ToolTipText = string.Format(_language.Menu.File.FormatXProperties, new Ebu().Name);
                toolStripButtonXProperties.Image = Properties.Resources.ebu;
                TryLoadIcon(toolStripButtonXProperties, "EbuProperties");
            }
        }

        private void NetflixGlyphCheck(bool isSaving)
        {
            ReloadFromSourceView();

            string fileName = string.IsNullOrEmpty(_fileName) ? "UntitledSubtitle" : Path.GetFileNameWithoutExtension(_fileName);
            string language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);

            var netflixController = new NetflixQualityController { Language = language, VideoFileName = _videoFileName };
            if (!string.IsNullOrEmpty(_videoFileName) && _videoInfo != null && _videoInfo.FramesPerSecond > 20)
            {
                netflixController.FrameRate = _videoInfo.FramesPerSecond;
            }
            else if (!string.IsNullOrEmpty(_videoFileName) && CurrentFrameRate != 23.976 && CurrentFrameRate != 24)
            {
                netflixController.FrameRate = CurrentFrameRate;
            }

            netflixController.RunChecks(_subtitle);

            if (netflixController.Records.Count > 0)
            {
                string reportPath = Path.GetTempPath() + fileName + "_NetflixQualityCheck.csv";
                netflixController.SaveCsv(reportPath);
                if (!isSaving)
                {
                    using (var form = new NetflixFixErrors(_subtitle, GetCurrentSubtitleFormat(), _fileName, _videoFileName, netflixController.FrameRate))
                    {
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            // Do nothing for now
                        }
                    }
                }
                else
                {
                    ShowStatus(string.Format(_language.SavedSubtitleX, $"\"{_fileName}\"") + " - " +
                               string.Format(LanguageSettings.Current.NetflixQualityCheck.FoundXIssues, netflixController.Records.Count));
                }
            }
            else if (!isSaving)
            {
                MessageBox.Show("Netflix Quality Check found no issues.", "Netflix Quality Check");
            }
        }

        private void OpenVideoFromUrl(string url)
        {
            Cursor = Cursors.WaitCursor;
            _videoFileName = url;

            Directory.SetCurrentDirectory(Configuration.DataDirectory);
            ResetPlaySelection();
            Cursor = Cursors.Default;
            SetUndockedWindowsTitle();
        }

        private void SmpteTimeModedropFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            smpteTimeModedropFrameToolStripMenuItem.Checked = !smpteTimeModedropFrameToolStripMenuItem.Checked;
            Configuration.Settings.General.CurrentVideoIsSmpte = smpteTimeModedropFrameToolStripMenuItem.Checked;
            if (audioVisualizer.WavePeaks != null)
            {
                ReloadWaveform(_videoFileName, VideoAudioTrackNumber);
            }
        }

        private void ShowHideBookmark(Paragraph p)
        {
            if (!string.IsNullOrWhiteSpace(p.Bookmark))
            {
                pictureBoxBookmark.Show();
                if (_showBookmarkLabel)
                {
                    panelBookmark.Show();
                    using (var graphics = CreateGraphics())
                    {
                        var textSize = graphics.MeasureString(p.Bookmark, Font);
                        labelBookmark.Text = p.Bookmark;
                        panelBookmark.Left = pictureBoxBookmark.Left;
                        panelBookmark.Top = pictureBoxBookmark.Top + pictureBoxBookmark.Height + 9;
                        panelBookmark.Width = (int)textSize.Width + 20;
                        panelBookmark.Height = (int)textSize.Height + 20;
                        panelBookmark.Show();
                    }
                }
                else
                {
                    panelBookmark.Hide();
                }
            }
            else if (p.Bookmark != null)
            {
                pictureBoxBookmark.Show();
                panelBookmark.Hide();
            }
            else if (panelBookmark.Visible || pictureBoxBookmark.Visible || _loading)
            {
                panelBookmark.Hide();
                pictureBoxBookmark.Hide();
            }
        }


        public void RemoveBookmark(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p != null)
            {
                MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Main.Menu.ContextMenu.RemoveBookmark));
                p.Bookmark = null;
                SubtitleListview1.ShowState(_subtitleListViewIndex, p);
                ShowHideBookmark(p);
                SetListViewStateImages();
                new BookmarkPersistence(_subtitle, _fileName).Save();
            }
        }

        private void LabelBookmarkDoubleClick(object sender, EventArgs e)
        {
            EditBookmark(_subtitleListViewIndex, this);
        }

        public void EditBookmark(int index, Form parentForm)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p != null)
            {
                using (var form = new BookmarkAdd(p))
                {
                    var result = form.ShowDialog(parentForm);
                    if (result == DialogResult.OK)
                    {
                        MakeHistoryForUndo(string.Format(_language.BeforeX, LanguageSettings.Current.Main.Menu.ContextMenu.EditBookmark));
                        p.Bookmark = form.Comment;
                        SubtitleListview1.ShowState(_subtitleListViewIndex, p);
                        ShowHideBookmark(p);
                        SetListViewStateImages();
                        new BookmarkPersistence(_subtitle, _fileName).Save();
                    }
                }
            }
        }


        private void RunActionOnAllParagraphs(Func<Paragraph, string> action, string historyMessage)
        {
            if (_subtitle.Paragraphs.Count <= 0 || SubtitleListview1.SelectedItems.Count <= 0)
            {
                return;
            }

            int linesUpdated = 0;
            var selectedIndices = SubtitleListview1.GetSelectedIndices();
            for (int i = selectedIndices.Length - 1; i >= 0; i--)
            {
                int idx = selectedIndices[i];
                var p = _subtitle.GetParagraphOrDefault(idx);
                if (p != null)
                {
                    var newText = action.Invoke(p);
                    if (newText != p.Text)
                    {
                        if (linesUpdated == 0)
                        {
                            MakeHistoryForUndo(historyMessage);
                        }

                        if (newText.IsOnlyControlCharactersOrWhiteSpace())
                        {
                            _subtitle.Paragraphs.RemoveAt(idx);
                        }
                        else
                        {
                            p.Text = newText;
                        }

                        linesUpdated++;
                    }

                    if (IsOriginalEditable)
                    {
                        var original = Utilities.GetOriginalParagraph(idx, p, _subtitleOriginal.Paragraphs);
                        if (original != null)
                        {
                            newText = action.Invoke(original);
                            if (newText != original.Text)
                            {
                                if (linesUpdated == 0)
                                {
                                    MakeHistoryForUndo(historyMessage);
                                }

                                if (newText.IsOnlyControlCharactersOrWhiteSpace())
                                {
                                    _subtitleOriginal.Paragraphs.RemoveAt(idx);
                                }
                                else
                                {
                                    original.Text = newText;
                                }

                                linesUpdated++;
                            }
                        }
                    }
                }
            }

            if (linesUpdated == 0)
            {
                return; // nothing changed
            }

            SaveSubtitleListviewIndices();
            _subtitle.Renumber();
            _subtitleOriginal?.Renumber();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            UpdateSourceView();
            RestoreSubtitleListviewIndices();
            ShowStatus(string.Format(_language.LinesUpdatedX, linesUpdated));
        }

        private void RemoveAllFormattingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            RunActionOnAllParagraphs((p) =>
            {
                var s = p.Text;
                if (!s.Contains("(♪", StringComparison.Ordinal) && !s.Contains("(♫", StringComparison.Ordinal) && !s.Contains("[♪", StringComparison.Ordinal) && !s.Contains("[♫", StringComparison.Ordinal) &&
                    !s.Contains("♪)", StringComparison.Ordinal) && !s.Contains("♫)", StringComparison.Ordinal) && !s.Contains("♪]", StringComparison.Ordinal) && !s.Contains("♫]", StringComparison.Ordinal))
                {
                    s = p.Text.Replace("♪", string.Empty).Replace("♫", string.Empty);
                }

                s = NetflixImsc11Japanese.RemoveTags(s);
                return HtmlUtil.RemoveHtmlTags(s, true).Trim();
            }, string.Format(_language.BeforeX, _language.Menu.ContextMenu.RemoveFormattingAll));
        }


        private void ToolStripMenuItemSetParagraphAsSelectionClick(object sender, EventArgs e)
        {
            if (SubtitleListview1.SelectedItems.Count == 1 && audioVisualizer != null && audioVisualizer.NewSelectionParagraph != null)
            {
                var idx = SubtitleListview1.SelectedItems[0].Index;
                var p = _subtitle.Paragraphs[idx];
                ButtonSetEndAndGoToNextClick(null, null);
                p.StartTime.TotalMilliseconds = audioVisualizer.NewSelectionParagraph.StartTime.TotalMilliseconds;
                p.EndTime.TotalMilliseconds = audioVisualizer.NewSelectionParagraph.EndTime.TotalMilliseconds;
                SubtitleListview1.SetStartTimeAndDuration(idx, p, _subtitle.GetParagraphOrDefault(idx - 1), _subtitle.GetParagraphOrDefault(idx + 1));
                mediaPlayer.CurrentPosition = audioVisualizer.NewSelectionParagraph.EndTime.TotalSeconds + MinGapBetweenLines / 1000.0;
                audioVisualizer.NewSelectionParagraph = null;
            }
        }

        private void CheckSecondSubtitleReset()
        {
            if (!_restorePreviewAfterSecondSubtitle)
            {
                return;
            }

            Configuration.Settings.General.MpvHandlesPreviewText = true;
            mediaPlayer.SubtitleText = string.Empty;
            _restorePreviewAfterSecondSubtitle = false;
        }

        private void SplitContainerListViewAndTextSplitterMoved(object sender, SplitterEventArgs e)
        {
            if (Configuration.Settings.General.SubtitleTextBoxMaxHeight < splitContainerListViewAndText.Panel2MinSize &&
                Configuration.Settings.General.SubtitleTextBoxMaxHeight > 1000)
            {
                return;
            }

            if (splitContainerListViewAndText.Panel2.Height > Configuration.Settings.General.SubtitleTextBoxMaxHeight)
            {
                try
                {
                    splitContainerListViewAndText.SplitterDistance = splitContainerListViewAndText.Height - Configuration.Settings.General.SubtitleTextBoxMaxHeight;
                }
                catch
                {
                    // ignore
                }
            }

            var diff = DateTime.UtcNow.Ticks - _textHeightResizeIgnoreUpdate;
            if (diff > 10_000 * 750 && // 750 ms
                WindowState == _lastFormWindowState)
            {
                _textHeightResize = splitContainerListViewAndText.Height - splitContainerListViewAndText.SplitterDistance;
            }

            MainResize();
        }

        public bool ProcessCmdKeyFromChildForm(ref Message msg, Keys keyData)
        {
            Message messageCopy = msg;
            messageCopy.HWnd = Handle;

            return ProcessCmdKey(ref messageCopy, keyData);
        }

        private int GetFastSubtitleHash()
        {
            return _subtitle.GetFastHashCode(_fileName + GetCurrentEncoding().BodyName);
        }

        private int GetFastSubtitleOriginalHash()
        {
            return _subtitleOriginal.GetFastHashCode(_subtitleOriginalFileName + GetCurrentEncoding().BodyName);
        }


        private void SetAssaResolution(Subtitle subtitle)
        {
            if (!IsAssa() || string.IsNullOrEmpty(_videoFileName) || _videoInfo.Width == 0 || _videoInfo.Height == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(subtitle.Header))
            {
                subtitle.Header = AdvancedSubStationAlpha.DefaultHeader;
            }

            ShowStatus($"{LanguageSettings.Current.Settings.SetAssaResolution}  {_videoInfo.Width.ToString(CultureInfo.InvariantCulture)}x{_videoInfo.Height.ToString(CultureInfo.InvariantCulture)}");
            subtitle.Header = AdvancedSubStationAlpha.AddTagToHeader("PlayResX", "PlayResX: " + _videoInfo.Width.ToString(CultureInfo.InvariantCulture), "[Script Info]", subtitle.Header);
            subtitle.Header = AdvancedSubStationAlpha.AddTagToHeader("PlayResY", "PlayResY: " + _videoInfo.Height.ToString(CultureInfo.InvariantCulture), "[Script Info]", subtitle.Header);
        }


        private void ShowAssaResolutionChanger(bool showNeverButton)
        {
            if (GetCurrentSubtitleFormat().GetType() != typeof(AdvancedSubStationAlpha))
            {
                return;
            }

            using (var form = new ResolutionResampler(_subtitle, _videoFileName, _videoInfo, showNeverButton))
            {
                var result = form.ShowDialog(this);
                if (result != DialogResult.OK)
                {
                    return;
                }

                var idx = FirstSelectedIndex;
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisibleFaster(idx);
            }
        }



        private bool RequireFfmpegOk()
        {
            FixFfmpegWrongPath();

            if (Configuration.IsRunningOnWindows && (string.IsNullOrWhiteSpace(Configuration.Settings.General.FFmpegLocation) || !File.Exists(Configuration.Settings.General.FFmpegLocation)))
            {
                if (MessageBox.Show(string.Format(LanguageSettings.Current.Settings.DownloadX, "FFmpeg"), "Subtitle Edit", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return false;
                }

                using (var form = new DownloadFfmpeg("FFmpeg"))
                {
                    if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.FFmpegPath))
                    {
                        Configuration.Settings.General.FFmpegLocation = form.FFmpegPath;
                        Configuration.Settings.General.UseFFmpegForWaveExtraction = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool RequireWhisperCpp()
        {
            if (!Configuration.IsRunningOnWindows)
            {
                return true;
            }

            var fullPath = Path.Combine(Configuration.DataDirectory, "Whisper", "Cpp", "main.exe");
            if (!File.Exists(fullPath) || WhisperDownload.IsOld(fullPath, WhisperChoice.Cpp))
            {
                if (MessageBox.Show(string.Format(LanguageSettings.Current.Settings.DownloadX, "Whisper.cpp"), "Subtitle Edit", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return false;
                }

                using (var form = new WhisperDownload(WhisperChoice.Cpp))
                {
                    if (form.ShowDialog(this) == DialogResult.OK && File.Exists(fullPath))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void VideoaudioToTextToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_videoFileName) &&
                (_videoFileName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 _videoFileName.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("SE cannot generate text from online video/audio");
                return;
            }

            if (!ContinueNewOrExit())
            {
                return;
            }

            if (!RequireFfmpegOk())
            {
                return;
            }

            var voskFolder = Path.Combine(Configuration.DataDirectory, "Vosk");
            if (!Directory.Exists(voskFolder))
            {
                Directory.CreateDirectory(voskFolder);
            }

            if (Configuration.IsRunningOnWindows && !HasCurrentVosk(voskFolder))
            {
                if (MessageBox.Show(string.Format(LanguageSettings.Current.Settings.DownloadX, "libvosk"), "Subtitle Edit", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }

                using (var form = new DownloadVosk())
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            var oldVideoFileName = _videoFileName;
            var isVlc = mediaPlayer.VideoPlayer is LibVlcDynamic;
            if (isVlc)
            {
                CloseVideoToolStripMenuItemClick(sender, e);
            }

            using (var form = new VoskAudioToText(oldVideoFileName, _videoAudioTrackNumber, this))
            {
                var result = form.ShowDialog(this);

                if (isVlc)
                {
                    OpenVideo(oldVideoFileName);
                }

                if (result != DialogResult.OK)
                {
                    return;
                }

                if (form.TranscribedSubtitle.Paragraphs.Count == 0)
                {
                    MessageBox.Show(LanguageSettings.Current.AudioToText.NoTextFound);
                    return;
                }

                _subtitle.Paragraphs.Clear();
                _subtitle.Paragraphs.AddRange(form.TranscribedSubtitle.Paragraphs);
                var idx = FirstSelectedIndex;
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisibleFaster(idx);
            }
        }

        private bool HasCurrentVosk(string voskFolder)
        {
            if (Configuration.IsRunningOnLinux || _hasCurrentVosk)
            {
                return true;
            }

            var voskDll = Path.Combine(voskFolder, "libvosk.dll");
            if (!File.Exists(voskDll))
            {
                return false;
            }

            var currentVoskDllSha512Hash =
                IntPtr.Size * 8 == 32
                ? "1cc13d8e2ffd3ad7ca76941c99e8ad00567d0b8135878c3a80fb938054cf98bde1f692647e6d19df7526c98aa5ad975d72dba20bf1759baedba5c753a14480bb"
                : "77479a934650b40968d54dcf71fce17237c59b62b6c64ad3d6b5433486b76b6202eb956e93597ba466c67aa0d553db7b2863e0aeb8856a6dd29a3aba3a14bf66";
            var hash = Utilities.GetSha512Hash(FileUtil.ReadAllBytesShared(voskDll));

            _hasCurrentVosk = currentVoskDllSha512Hash == hash;
            return _hasCurrentVosk;
        }


        private void AudioToTextWhisperTolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_videoFileName) &&
               (_videoFileName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                _videoFileName.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("SE cannot generate text from online video/audio");
                return;
            }

            if (!ContinueNewOrExit())
            {
                return;
            }

            if (!RequireFfmpegOk())
            {
                return;
            }

            if (Configuration.Settings.Tools.WhisperChoice == WhisperChoice.Cpp)
            {
                if (!RequireWhisperCpp())
                {
                    return;
                }
            }

            CheckWhisperCpp();

            var oldVideoFileName = _videoFileName;
            var isVlc = mediaPlayer.VideoPlayer is LibVlcDynamic;
            if (isVlc)
            {
                CloseVideoToolStripMenuItemClick(sender, e);
            }

            using (var form = new WhisperAudioToText(oldVideoFileName, _subtitle, _videoAudioTrackNumber, this, audioVisualizer?.WavePeaks))
            {
                var result = form.ShowDialog(this);

                if (isVlc)
                {
                    OpenVideo(oldVideoFileName);
                }

                if (result != DialogResult.OK)
                {
                    return;
                }

                if (form.TranscribedSubtitle.Paragraphs.Count == 0)
                {
                    if (form.IncompleteModel)
                    {
                        MessageBox.Show($"Model incomplete.{Environment.NewLine}" +
                                        $"Please re-download model: {form.IncompleteModelName}", MessageBoxIcon.Error);
                    }
                    else if (form.UnknownArgument)
                    {
                        var customArgument = Configuration.Settings.Tools.WhisperExtraSettings;
                        var extraMessage = string.Empty;
                        if (!string.IsNullOrEmpty(customArgument))
                        {
                            extraMessage = Environment.NewLine + "Note you have a custom argument: " + customArgument;
                        }
                        extraMessage = extraMessage + Environment.NewLine + Environment.NewLine + "View the log file `whisper_log.txt`?";
                        var r = MessageBox.Show($"Whisper reported unknown argument'" + extraMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                        if (r == DialogResult.Yes)
                        {
                            UiUtil.OpenFile(SeLogger.GetWhisperLogFilePath());
                        }
                    }
                    else if (form.RunningOnCuda &&
                             Configuration.Settings.Tools.WhisperChoice == WhisperChoice.PurfviewFasterWhisper &&
                             !WhisperAudioToText.IsFasterWhisperCudaInstalled())
                    {
                        MessageBox.Show("cuBLAS or cuDNN seems to be missing.");
                        WhisperAudioToText.DownloadCudaForWhisperFaster(this);
                    }
                    else
                    {
                        var customArgument = Configuration.Settings.Tools.WhisperExtraSettings;
                        var extraMessage = string.Empty;
                        if (!string.IsNullOrEmpty(customArgument))
                        {
                            extraMessage = Environment.NewLine + "Note you have a custom argument: " + customArgument;
                        }
                        extraMessage = extraMessage + Environment.NewLine + Environment.NewLine + string.Format(LanguageSettings.Current.General.ViewX, "`whisper_log.txt`?");
                        var r = MessageBox.Show(LanguageSettings.Current.AudioToText.NoTextFound + extraMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                        if (r == DialogResult.Yes)
                        {
                            UiUtil.OpenFile(SeLogger.GetWhisperLogFilePath());
                        }
                    }

                    return;
                }

                MakeHistoryForUndo(string.Format(_language.BeforeX, string.Format(LanguageSettings.Current.Main.Menu.Video.VideoAudioToTextX, "Whisper")));
                _subtitle.Paragraphs.Clear();
                _subtitle.Paragraphs.AddRange(form.TranscribedSubtitle.Paragraphs);
                var idx = FirstSelectedIndex;
                SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
                _subtitleListViewIndex = -1;
                SubtitleListview1.SelectIndexAndEnsureVisibleFaster(idx);
            }
        }

        private static void CheckWhisperCpp()
        {
            if (Configuration.Settings.Tools.WhisperChoice != WhisperChoice.Cpp)
            {
                return;
            }

            if (Configuration.IsRunningOnLinux && WhisperHelper.GetWhisperPathAndFileName() == "whisper")
            {
                SeLogger.Error("UseWhisperChoice changed to 'OpenAI' as 'Whisper/whisper' or '/Whisper/main' was not found!");
                Configuration.Settings.Tools.WhisperChoice = WhisperChoice.OpenAi;
            }

            if (Configuration.IsRunningOnWindows && WhisperHelper.GetWhisperPathAndFileName() == "whisper")
            {
                SeLogger.Error("UseWhisperChoice changed to 'OpenAI' as 'Whisper/whisper.exe' or '/Whisper/main.exe' was not found!");
                Configuration.Settings.Tools.WhisperChoice = WhisperChoice.OpenAi;
            }
        }

        public void ReloadSubtitle(Subtitle subtitle)
        {
            SaveSubtitleListviewIndices();
            SubtitleListview1.Fill(_subtitle, _subtitleOriginal);
            RestoreSubtitleListviewIndices();
            RefreshSelectedParagraph();
        }


        public void RedockFromFullscreen()
        {
            if (_videoPlayerUndocked != null && !_videoPlayerUndocked.IsDisposed)
            {
                var control = _videoPlayerUndocked.PanelContainer.Controls[0];
                control.Parent.Controls.Remove(control);
                ReDockVideoPlayer(control);
                _videoPlayerUndocked = null;
                mediaPlayer.ShowFullscreenButton = Configuration.Settings.General.VideoPlayerShowFullscreenButton;
            }

            Configuration.Settings.General.Undocked = false;
        }
    }
}