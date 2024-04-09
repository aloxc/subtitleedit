using System.IO;
using System.Threading;
using System;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Logic;
using static System.Resources.ResXFileRef;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Forms.Ocr;

namespace Nikse.SubtitleEdit.Forms
{
    public partial class MainForm : Form
    {
        private string[] _dragAndDropFiles;
        public MainForm()
        {
            InitializeComponent();
        }

        private void subtitleListView1_DragDrop(object sender, DragEventArgs e)
        {
            _dragAndDropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            DoSubtitleListview1Drop(sender,e);
        }

        private void subtitleListView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void DoSubtitleListview1Drop(object sender, EventArgs e)
        {

            string fileName = _dragAndDropFiles[0];
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
                OpenSubtitle(fileName,-1);
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
                MessageBox.Show( "字幕为空...");
                return;
            }

            using (var vobSubOcr = new VobSubOcr())
            {
                vobSubOcr.Initialize(subtitles, Configuration.Settings.VobSubOcr, fileName);
                vobSubOcr.FileName = Path.GetFileName(fileName);
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            MessageBox.Show("a");
            DoSubtitleListview1Drop(null, null);
        }
    }
}
