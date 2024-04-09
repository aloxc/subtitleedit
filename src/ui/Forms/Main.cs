using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Enums;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.Forms.Assa;
using Nikse.SubtitleEdit.Forms.AudioToText;
using Nikse.SubtitleEdit.Forms.Networking;
using Nikse.SubtitleEdit.Forms.Ocr;
using Nikse.SubtitleEdit.Forms.Styles;
using Nikse.SubtitleEdit.Logic;
using Nikse.SubtitleEdit.Logic.CommandLineConvert;
using Nikse.SubtitleEdit.Logic.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CheckForUpdatesHelper = Nikse.SubtitleEdit.Logic.CheckForUpdatesHelper;
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class Main : Form
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



        


        private bool _forceClose = false;
        private readonly Timer _timerClearStatus = new Timer();
        private readonly Timer _timerSlow = new Timer();
        private readonly ContextMenuStrip _contextMenuStripPlayRate;


        private NikseWebServiceSession _networkSession;


        private string _clearLastFindText = string.Empty;
        private string[] _dragAndDropFiles;
        private readonly MainShortcuts _shortcuts = new MainShortcuts();

        public bool IsMenuOpen { get; private set; }
        public string Title { get; internal set; }

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



                // set up UI interfaces / injections
                Ebu.EbuUiHelper = new UiEbuSaveHelper();
                Pac.GetPacEncodingImplementation = new UiGetPacEncoding(this);
                RichTextToPlainText.NativeRtfTextConverter = new RtfTextConverterRichTextBox();


                if (Configuration.Settings.General.RightToLeftMode)
                {
                    SubtitleListview1.RightToLeft = RightToLeft.Yes;
                    SubtitleListview1.RightToLeftLayout = true;
                }


                _timerClearStatus.Interval = Configuration.Settings.General.ClearStatusBarAfterSeconds * 1000;

                var commandLineArgs = Environment.GetCommandLineArgs();
                var fileName = string.Empty;
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

                }


                if (fileName.Length > 0 && File.Exists(fileName))
                {
                    fileName = Path.GetFullPath(fileName);

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
            }
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClose)
            {
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

        }



        private readonly object _updateShowEarlierLock = new object();

        string _lastTranslationDebugError = string.Empty;




        internal static string MainTextBox;

        private void SubtitleListView1_DragDrop(object sender, DragEventArgs e)
        {
            _dragAndDropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            DoSubtitleListview1Drop(sender, e);
        }

        private void SubtitleListView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void DoSubtitleListview1Drop(object sender, EventArgs e)
        {

            string fileName = "D:\\影视\\甄嬛传\\sup字幕\\05.sup";
            var file = new FileInfo(fileName);

            // Do not allow directory drop
            if (FileUtil.IsDirectory(fileName))
            {
                MessageBox.Show("不能打开目录", file.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dirName = Path.GetDirectoryName(fileName);
            var ext = file.Extension.ToLowerInvariant();


            if (file.Length < 250000000 && ext == ".sup" && FileUtil.IsBluRaySup(fileName)) // max 250 mb
            {
                OpenSubtitle(fileName, -1);
                //OpenSubtitle(fileName, encoding, null, -1, null);
                // OpenSubtitle(fileName, encoding, videoFileName, audioTrack, originalFileName, false);
            }
            else
            {
                MessageBox.Show("不支持的文件");
            }
        }

        private void OpenSubtitle(string fileName, int audioTrack)
        {


            var file = new FileInfo(fileName);
            var ext = file.Extension.ToLowerInvariant();

            Configuration.Settings.General.CurrentVideoOffsetInMs = 0;
            Configuration.Settings.General.CurrentVideoIsSmpte = false;

            ImportAndOcrBluRaySup(fileName);
        }

        private void ImportAndOcrBluRaySup(string fileName)
        {
            var log = new StringBuilder();
            var subtitles = BluRaySupParser.ParseBluRaySup(fileName, log);
            if (subtitles.Count == 0)
            {
                MessageBox.Show("字幕为空...");
                return;
            }

            using (var vobSubOcr = new VobSubOcr())
            {
                vobSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName);
                vobSubOcr.FileName = Path.GetFileName(fileName);
                vobSubOcr.ShowDialog(this);

                //MakeHistoryForUndo(_language.BeforeImportingBluRaySupFile);
                    /*FileNew();
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
                    */
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoSubtitleListview1Drop(null, null);
        }

        internal void ToggleBookmarks(bool v, SpellCheck spellCheck)
        {
            throw new NotImplementedException();
        }

        internal void EditBookmark(int currentIndex, SpellCheck spellCheck)
        {
            throw new NotImplementedException();
        }

        internal void CorrectWord(string v1, Paragraph currentParagraph, string v2, ref bool firstChange, int wordsIndex)
        {
            throw new NotImplementedException();
        }

        internal void ShowStatus(string v)
        {
            throw new NotImplementedException();
        }

        internal void ChangeWholeTextMainPart(ref int noOfChangedWords, ref bool firstChange, int currentIndex, Paragraph currentParagraph)
        {
            throw new NotImplementedException();
        }

        internal void DeleteLine()
        {
            throw new NotImplementedException();
        }

        internal Subtitle UndoFromSpellCheck(Subtitle subtitle)
        {
            throw new NotImplementedException();
        }

        internal void FocusParagraph(int currentIndex)
        {
            throw new NotImplementedException();
        }

        internal void RemoveBookmark(int index)
        {
            throw new NotImplementedException();
        }

        internal void ReDockWaveform(Control controlWaveform, Control controlButtons, Control controlTrackBar)
        {
            throw new NotImplementedException();
        }

        internal void RedockVideoControlsToolStripMenuItemClick(object value1, object value2)
        {
            throw new NotImplementedException();
        }

        internal void MainKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal bool ProcessCmdKeyFromChildForm(ref Message msg, Keys keyData)
        {
            throw new NotImplementedException();
        }

        internal void GotoPrevSubPosFromvideoPos()
        {
            throw new NotImplementedException();
        }

        internal void GotoNextSubPosFromVideoPos()
        {
            throw new NotImplementedException();
        }

        internal void RedockFromFullscreen()
        {
            throw new NotImplementedException();
        }

        internal void ReDockVideoPlayer(Control control)
        {
            throw new NotImplementedException();
        }

        internal static object GetPropertiesAndDoAction(string assaDrawPluginFileName, out object name, out object text, out object version, out object description, out object actionType, out object shortcut, out object mi)
        {
            throw new NotImplementedException();
        }

        internal void ReloadFromSubtitle(Subtitle subtitle, string beforeRemovalOfTextingForHearingImpaired)
        {
            throw new NotImplementedException();
        }

        internal void ReDockVideoButtons(Control control, Control controlCheckBox)
        {
            throw new NotImplementedException();
        }

        internal void ApplySsaStyles(SubStationAlphaStyles subStationAlphaStyles)
        {
            throw new NotImplementedException();
        }

        internal void ApplyAssaStyles(AssaStyles assaStyles)
        {
            throw new NotImplementedException();
        }
    }

}