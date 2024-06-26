﻿using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Core.VobSub.Ocr.Service;
using Nikse.SubtitleEdit.Logic;
using Nikse.SubtitleEdit.Logic.Ocr;
using Nikse.SubtitleEdit.Logic.Ocr.Binary;
using PaddleOCRSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms.Ocr
{
    public sealed partial class VobSubOcr : PositionAndSizeForm, IBinaryParagraphList
    {
        private static readonly Color _listViewGreen = Configuration.Settings.General.UseDarkTheme ? Color.Green : Color.LightGreen;
        private static readonly Color _listViewYellow = Configuration.Settings.General.UseDarkTheme ? Color.FromArgb(218, 135, 32) : Color.Yellow;
        private static readonly Color _listViewOrange = Configuration.Settings.General.UseDarkTheme ? Color.OrangeRed : Color.Orange;

        internal class CompareItem
        {
            public ManagedBitmap Bitmap { get; }
            public string Name { get; }
            public bool Italic { get; set; }
            public int ExpandCount { get; }
            public int NumberOfForegroundColors { get; set; }
            public string Text { get; set; }

            public CompareItem(ManagedBitmap bmp, string name, bool isItalic, int expandCount, string text)
            {
                Bitmap = bmp;
                Name = name;
                Italic = isItalic;
                ExpandCount = expandCount;
                NumberOfForegroundColors = -1;
                Text = text;
            }
        }

        internal class SubPicturesWithSeparateTimeCodes
        {
            public SubPicture Picture { get; }
            public TimeSpan Start { get; }
            public TimeSpan End { get; }

            public SubPicturesWithSeparateTimeCodes(SubPicture subPicture, TimeSpan start, TimeSpan end)
            {
                Picture = subPicture;
                Start = start;
                End = end;
            }
        }

        public class CompareMatch
        {
            public string Text { get; set; }
            public bool Italic { get; set; }
            public int ExpandCount { get; set; }
            public string Name { get; set; }
            public NOcrChar NOcrCharacter { get; set; }
            public ImageSplitterItem ImageSplitterItem { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public List<ImageSplitterItem> Extra { get; set; }

            public CompareMatch(string text, bool italic, int expandCount, string name)
            {
                Text = text;
                Italic = italic;
                ExpandCount = expandCount;
                Name = name;
            }

            public override string ToString()
            {
                if (Italic)
                {
                    return Text + " (italic)";
                }

                if (Text == null)
                {
                    return string.Empty;
                }

                return Text;
            }
        }

        internal class ImageCompareAddition
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public NikseBitmap Image { get; set; }
            public bool Italic { get; set; }
            public int Index { get; set; }

            public ImageCompareAddition(string name, string text, NikseBitmap image, bool italic, int index)
            {
                Name = name;
                Text = text;
                Image = image;
                Text = text;
                Italic = italic;
                Index = index;
            }

            public override string ToString()
            {
                if (Image == null)
                {
                    return Text;
                }

                if (Italic)
                {
                    return Text + " (" + Image.Width + "x" + Image.Height + ", italic)";
                }

                return Text + " (" + Image.Width + "x" + Image.Height + ")";
            }
        }

        public delegate void ProgressCallbackDelegate(string progress);
        public ProgressCallbackDelegate ProgressCallback { get; set; }

        public string FileName { get; set; }
        private Subtitle _subtitle = new Subtitle();
        private int _selectedIndex = -1;
        private double _unItalicFactor = 0.33;

        private BinaryOcrDb _binaryOcrDb;


        private bool _captureTopAlign;
        private int _captureTopAlignHeight = -1;

        private Bitmap _mainOcrBitmap;
        private bool _hasForcedSubtitles;

        private bool _fromMenuItem;

        // Blu-ray sup
        private List<BluRaySupParser.PcsData> _bluRaySubtitlesOriginal;
        private List<BluRaySupParser.PcsData> _bluRaySubtitles;

        private List<ImageCompareAddition> _lastAdditions = new List<ImageCompareAddition>();
        private bool _okClicked;
        private static SubtitleListView __list;
        private static VobSubOcr __this;

        // optimization vars


        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp?.SetValue(c, true, null);
        }

        public VobSubOcr()
        {
            UiUtil.PreInitialize(this);
            InitializeComponent();
            UiUtil.FixFonts(this);
            SetDoubleBuffered(subtitleListView1);
            CheckForIllegalCrossThreadCalls = false;

            buttonPause.Text = LanguageSettings.Current.Settings.Pause;
            subtitleListView1.InitializeLanguage(LanguageSettings.Current.General, Configuration.Settings);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.CharactersPerSeconds);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Actor);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.WordsPerMinute);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Region);
            subtitleListView1.AutoSizeColumns();

            groupBoxSubtitleImage.Text = string.Empty;

            if (Configuration.Settings.VobSubOcr.ItalicFactor >= 0.1 && Configuration.Settings.VobSubOcr.ItalicFactor < 1)
            {
                _unItalicFactor = Configuration.Settings.VobSubOcr.ItalicFactor;
            }

            toolStripMenuItemSaveSubtitleAs.Text = LanguageSettings.Current.Main.SaveSubtitleAs;

            UiUtil.InitializeSubtitleFont(subtitleListView1);
            subtitleListView1.AutoSizeAllColumns(this);


            var ocrLanguages = new GoogleOcrService(new GoogleCloudVisionApi(string.Empty)).GetLanguages().OrderBy(p => p.ToString());
            var selectedOcrLanguage = ocrLanguages.FirstOrDefault(p => p.Code == Configuration.Settings.VobSubOcr.CloudVisionLanguage);
            if (selectedOcrLanguage == null)
            {
                selectedOcrLanguage = ocrLanguages.FirstOrDefault(p => p.Code == "en");
            }
            __this = this;
        }

        internal void Initialize(List<VobSubMergedPack> vobSubMergedPackList, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            SetButtonsStartOcr();
        }

        internal void InitializeBatch(IList<IBinaryParagraph> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName, bool forcedOnly, string language, string ocrEngine, CancellationToken cancellationToken)
        {
            if (subtitles.Count == 0)
            {
                return;
            }

            if (subtitles.First() is TransportStreamSubtitle)
            {
                var tssList = new List<TransportStreamSubtitle>();
                foreach (var binaryParagraph in subtitles)
                {
                    tssList.Add(binaryParagraph as TransportStreamSubtitle);
                }

            }
        }

        internal void InitializeBatch(List<VobSubMergedPack> vobSubMergedPackList, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string fileName, bool forcedOnly, string language, string ocrEngine, CancellationToken cancellationToken)
        {
            Initialize(vobSubMergedPackList, palette, vobSubOcrSettings, language);
        }

        internal void InitializeBatch(List<SubPicturesWithSeparateTimeCodes> list, string fileName, string language, string ocrEngine)
        {
            Initialize(list, Configuration.Settings.VobSubOcr, fileName);
        }

        internal void InitializeBatch(Subtitle imageListSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon, string language, string ocrEngine)
        {
            Initialize(imageListSubtitle, vobSubOcrSettings, isSon);
        }

        internal void Initialize(List<BluRaySupParser.PcsData> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            SetButtonsStartOcr();

            _bluRaySubtitlesOriginal = subtitles;


            Text = LanguageSettings.Current.VobSubOcr.TitleBluRay;
            if (!string.IsNullOrEmpty(fileName))
            {
                Text += " - " + Path.GetFileName(fileName);
                FileName = fileName;
                _subtitle.FileName = fileName;
            }

            autoTransparentBackgroundToolStripMenuItem.Checked = false;
            autoTransparentBackgroundToolStripMenuItem.Visible = false;
        }

        private void LoadBluRaySup()
        {
            _subtitle = new Subtitle();

            _bluRaySubtitles = new List<BluRaySupParser.PcsData>();
            int max = _bluRaySubtitlesOriginal.Count;
            for (int i = 0; i < max; i++)
            {
                var x = _bluRaySubtitlesOriginal[i];
                _bluRaySubtitles.Add(x);
                _subtitle.Paragraphs.Add(new Paragraph
                {
                    StartTime = new TimeCode(x.StartTime / 90.0),
                    EndTime = new TimeCode(x.EndTime / 90.0)
                });
            }

            _subtitle.Renumber();

            FixShortDisplayTimes(_subtitle);

            subtitleListView1.Fill(_subtitle);
            subtitleListView1.SelectIndexAndEnsureVisible(0);

            SetButtonsEnabledAfterOcrDone();
            buttonStartOcr.Focus();
        }


        public void FixShortDisplayTimes(Subtitle subtitle)
        {
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = _subtitle.Paragraphs[i];
                if (p.EndTime.TotalMilliseconds <= p.StartTime.TotalMilliseconds)
                {
                    Paragraph next = _subtitle.GetParagraphOrDefault(i + 1);
                    double newEndTime = p.StartTime.TotalMilliseconds + Configuration.Settings.VobSubOcr.DefaultMillisecondsForUnknownDurations;
                    if (next == null || (newEndTime < next.StartTime.TotalMilliseconds))
                    {
                        p.EndTime.TotalMilliseconds = newEndTime;
                    }
                    else
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                    }
                }
            }
        }

        public bool GetIsForced(int index)
        {
         

            if (_bluRaySubtitlesOriginal != null)
            {
                return _bluRaySubtitles[index].IsForced;
            }

            return false;
        }

        public Bitmap GetSubtitleBitmap(int index, bool crop = true)
        {
            Bitmap returnBmp = null;
            Color background;
            Color pattern;
            Color emphasis1;
            Color emphasis2;

            var makeTransparent = true;
            if (_bluRaySubtitlesOriginal != null)
            {
                if (_bluRaySubtitles != null)
                {
                    if (index >= 0 && index < _bluRaySubtitles.Count)
                    {
                        returnBmp = _bluRaySubtitles[index].GetBitmap();
                    }
                }
                else
                {
                    if (index >= 0 && index < _bluRaySubtitlesOriginal.Count)
                    {
                        returnBmp = _bluRaySubtitlesOriginal[index].GetBitmap();
                    }
                }
            }

            if (returnBmp == null)
            {
                return null;
            }


            if (_binaryOcrDb == null  || _fromMenuItem)
            {
                if (_preprocessingSettings == null || !_preprocessingSettings.Active)
                {
                    return returnBmp;
                }

                var nb = new NikseBitmap(returnBmp);
                nb.CropTransparentSidesAndBottom(2, true);
                nb.CropTopTransparent(2);
                if (_preprocessingSettings.InvertColors)
                {
                    nb.InvertColors();
                }

                if (_preprocessingSettings.YellowToWhite)
                {
                    nb.ReplaceYellowWithWhite();
                }

                if (_preprocessingSettings.ColorToWhite != Color.Transparent)
                {
                    nb.ReplaceColor(_preprocessingSettings.ColorToWhite.A, _preprocessingSettings.ColorToWhite.R, _preprocessingSettings.ColorToWhite.G, _preprocessingSettings.ColorToWhite.B, 255, 255, 255, 255);
                }

                if (_preprocessingSettings.ColorToRemove.A > 0)
                {
                    nb.ReplaceColor(_preprocessingSettings.ColorToRemove.A, _preprocessingSettings.ColorToRemove.R, _preprocessingSettings.ColorToRemove.G, _preprocessingSettings.ColorToRemove.B, Color.Transparent.A, Color.Transparent.R, Color.Transparent.G, Color.Transparent.B);
                }

                returnBmp.Dispose();
                return nb.GetBitmap();
            }

            var n = new NikseBitmap(returnBmp);
            n.CropTransparentSidesAndBottom(2, true);
            n.CropTopTransparent(2);
            if (_preprocessingSettings != null && _preprocessingSettings.Active)
            {
                if (_preprocessingSettings.InvertColors)
                {
                    n.InvertColors();
                }

                if (_preprocessingSettings.YellowToWhite)
                {
                    n.ReplaceYellowWithWhite();
                }

                if (_preprocessingSettings.ColorToWhite != Color.Transparent)
                {
                    n.ReplaceColor(_preprocessingSettings.ColorToWhite.A, _preprocessingSettings.ColorToWhite.R, _preprocessingSettings.ColorToWhite.G, _preprocessingSettings.ColorToWhite.B, 255, 255, 255, 255);
                }

                if (_preprocessingSettings.ColorToRemove.A > 0)
                {
                    n.ReplaceColor(_preprocessingSettings.ColorToRemove.A, _preprocessingSettings.ColorToRemove.R, _preprocessingSettings.ColorToRemove.G, _preprocessingSettings.ColorToRemove.B, Color.Transparent.A, Color.Transparent.R, Color.Transparent.G, Color.Transparent.B);
                }

                if (_preprocessingSettings.ScalingPercent > 100)
                {
                    var bTemp = n.GetBitmap();
                    var f = _preprocessingSettings.ScalingPercent / 100.0;
                    var b = ResizeBitmap(bTemp, (int)Math.Round(bTemp.Width * f), (int)Math.Round(bTemp.Height * f));
                    bTemp.Dispose();
                    n = new NikseBitmap(b);
                }
            }

            n.MakeTwoColor(_preprocessingSettings?.BinaryImageCompareThreshold ?? Configuration.Settings.Tools.OcrBinaryImageCompareRgbThreshold);
            returnBmp.Dispose();
            return n.GetBitmap();
        }


        private int GetSubtitleCount()
        {
            if (_bluRaySubtitlesOriginal != null)
            {
                return _bluRaySubtitles?.Count ?? _bluRaySubtitlesOriginal.Count;
            }

            return 0;
        }

        /// <summary>
        /// Get position of sub + sub size
        /// </summary>
        private void GetSubtitleTopAndHeight(int index, out int left, out int top, out int width, out int height)
        {
            if (_bluRaySubtitlesOriginal != null)
            {
                var item = _bluRaySubtitles[index];
                var bmp = item.GetBitmap();
                height = bmp.Height;
                width = bmp.Width;
                bmp.Dispose();
                left = item.PcsObjects.Min(p => p.Origin.X);
                top = item.PcsObjects.Min(p => p.Origin.Y);
                return;
            }

            left = 0;
            top = 0;
            width = 0;
            height = 0;
        }


        private Bitmap ShowSubtitleImage(int index)
        {
            int numberOfImages = GetSubtitleCount();
            Bitmap bmp;
            if (index < numberOfImages)
            {
                bmp = GetSubtitleBitmap(index);
                if (bmp == null)
                {
                    bmp = new Bitmap(1, 1);
                }

                groupBoxSubtitleImage.Text = string.Format(LanguageSettings.Current.VobSubOcr.SubtitleImageXofY, index + 1, numberOfImages) + "   " + bmp.Width + "x" + bmp.Height;
            }
            else
            {
                groupBoxSubtitleImage.Text = LanguageSettings.Current.VobSubOcr.SubtitleImage;
                bmp = new Bitmap(1, 1);
            }

            var old = pictureBoxSubtitleImage.Image as Bitmap;
            pictureBoxSubtitleImage.Image = bmp.Clone() as Bitmap;
            pictureBoxSubtitleImage.Invalidate();
            old?.Dispose();
            return bmp;
        }

        private void ShowSubtitleImage(int index, Bitmap bmp)
        {
            try
            {
                int numberOfImages = GetSubtitleCount();
                if (index < numberOfImages)
                {
                    groupBoxSubtitleImage.Text = string.Format(LanguageSettings.Current.VobSubOcr.SubtitleImageXofY, index + 1, numberOfImages) + "   " + bmp.Width + "x" + bmp.Height;
                }
                else
                {
                    groupBoxSubtitleImage.Text = LanguageSettings.Current.VobSubOcr.SubtitleImage;
                }

                Bitmap old = pictureBoxSubtitleImage.Image as Bitmap;
                pictureBoxSubtitleImage.Image = bmp.Clone() as Bitmap;
                pictureBoxSubtitleImage.Invalidate();
                old?.Dispose();
            }
            catch
            {
                // can crash if user is clicking around...
            }
        }

        internal static ImageSplitterItem GetExpandedSelectionNew(NikseBitmap bitmap, List<ImageSplitterItem> expandSelectionList)
        {
            int minimumX = expandSelectionList[0].X;
            int maximumX = expandSelectionList[expandSelectionList.Count - 1].X + expandSelectionList[expandSelectionList.Count - 1].NikseBitmap.Width;
            int minimumY = expandSelectionList[0].Y;
            int maximumY = expandSelectionList[0].Y + expandSelectionList[0].NikseBitmap.Height;
            var nbmp = new NikseBitmap(bitmap.Width, bitmap.Height);
            foreach (ImageSplitterItem item in expandSelectionList)
            {
                for (int y = 0; y < item.NikseBitmap.Height; y++)
                {
                    for (int x = 0; x < item.NikseBitmap.Width; x++)
                    {
                        var c = item.NikseBitmap.GetPixel(x, y);
                        if (c.A > 100 && c.R + c.G + c.B > 100)
                        {
                            nbmp.SetPixel(item.X + x, item.Y + y, Color.White);
                        }
                    }
                }

                if (item.Y < minimumY)
                {
                    minimumY = item.Y;
                }

                if (item.Y + item.NikseBitmap.Height > maximumY)
                {
                    maximumY = item.Y + item.NikseBitmap.Height;
                }

                if (item.X < minimumX)
                {
                    minimumX = item.X;
                }

                if (item.X + item.NikseBitmap.Width > maximumX)
                {
                    maximumX = item.X + item.NikseBitmap.Width;
                }
            }

            nbmp.CropTransparentSidesAndBottom(0, true);
            nbmp = NikseBitmapImageSplitter.CropTopAndBottom(nbmp, out _);

            return new ImageSplitterItem(minimumX, minimumY, nbmp);
        }

        public Subtitle SubtitleFromOcr => _subtitle;

        private void FormVobSubOcr_Shown(object sender, EventArgs e)
        {
            if (_bluRaySubtitlesOriginal != null)
            {
                var v = (decimal)Configuration.Settings.VobSubOcr.BlurayAllowDifferenceInPercent;

                LoadBluRaySup();
                _hasForcedSubtitles = false;
                foreach (var x in _bluRaySubtitlesOriginal)
                {
                    if (x.IsForced)
                    {
                        _hasForcedSubtitles = true;
                        break;
                    }
                }

            }
            VobSubOcr_Resize(null, null);
        }

        private void SetButtonsStartOcr()
        {
            buttonStartOcr.Enabled = false;
            buttonPause.Enabled = true;
            subtitleListView1.MultiSelect = false;
        }

        private void SetButtonsEnabledAfterOcrDone()
        {
            buttonStartOcr.Enabled = true;
            buttonPause.Enabled = false;
            subtitleListView1.MultiSelect = true;
        }
        static void PaddleOCR(Object stateInfo )
        {
            // 任务处理逻辑
            int number = (int)stateInfo;
            Console.WriteLine("任务处理器正在处理任务：{0}", number);
            ListViewItem item = __list.Items[number];
            var ocrResult = new OCRResult();
            PaddleOCREngine engine = new PaddleOCREngine(null, new OCRParameter());
            Bitmap bitmap = __this.GetSubtitleBitmap(number);
            //_subtitle.Paragraphs[2].
            if (bitmap == null)
            {
                return;
            }
            ocrResult = engine.DetectText(bitmap);
            var txt = "";
            if (ocrResult.TextBlocks.Count > 0)
            {
                List<TextBlock> list = ocrResult.TextBlocks;
                if (list.Count == 1)
                {
                    txt = list[0].Text;
                }
                else
                {
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        txt += list[i].Text + " ";
                    }
                    txt += list[list.Count - 1].Text;
                }
            }
            __this._subtitle.Paragraphs[number].Text = txt;
            item.SubItems[4].Text = txt;
            // 模拟任务处理，使用线程睡眠
        }
        private void ButtonStartOcrClick(object sender, EventArgs e)
        {
            if (_subtitle.Paragraphs.Count == 0)
            {
                return;
            }
            InitializeTopAlign();

            _mainOcrBitmap = null;

            SetButtonsStartOcr();
            _fromMenuItem = false;

            int max = GetSubtitleCount();
            __list = subtitleListView1;
            WaitCallback callback = new WaitCallback(PaddleOCR);
            for (int i = 0; i < max; i++)
            {
                ListViewItem item = subtitleListView1.Items[0];

                ThreadPool.QueueUserWorkItem(callback, i);
            }
           
            subtitleListView1.MultiSelect = false;
        }

        private void InitializeTopAlign()
        {
            if (_captureTopAlign && _captureTopAlignHeight == -1 && _subtitle.Paragraphs.Count > 2)
            {
                int maxHeight = -1;
                var idxList = new List<int> { 0, 1, _subtitle.Paragraphs.Count / 2, _subtitle.Paragraphs.Count - 1 };
                foreach (var idx in idxList)
                {
                    GetSubtitleTopAndHeight(idx, out _, out var top, out _, out var height);
                    if (top + height > maxHeight)
                    {
                        maxHeight = top + height;
                    }

                    if (_bluRaySubtitles != null && idx < _bluRaySubtitles.Count)
                    {
                        height = _bluRaySubtitles[idx].Size.Height;
                        if (height > maxHeight)
                        {
                            maxHeight = height;
                        }
                    }
                }

                if (maxHeight >= 720)
                {
                    _captureTopAlignHeight = maxHeight / 2 - 10;
                }
                else if (maxHeight > 320)
                {
                    _captureTopAlignHeight = maxHeight / 3;
                }
            }
        }


        public static Bitmap ResizeBitmap(Bitmap b, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (var g = Graphics.FromImage(result))
            {
                g.DrawImage(b, 0, 0, width, height);
            }

            return result;
        }

        public static Bitmap UnItalic(Bitmap bmp, double factor)
        {
            int left = (int)(bmp.Height * factor);
            var unItaliced = new Bitmap(bmp.Width + left + 4, bmp.Height);
            using (var g = Graphics.FromImage(unItaliced))
            {
                g.DrawImage(bmp, new[]
                {
                    new Point(0, 0), // destination for upper-left point of original
                    new Point(bmp.Width, 0), // destination for upper-right point of original
                    new Point(left, bmp.Height) // destination for lower-left point of original
                });
            }

            return unItaliced;
        }

        private void ButtonPauseClick(object sender, EventArgs e)
        {
            buttonPause.Enabled = false;
            SetButtonsEnabledAfterOcrDone();
        }

        private void ChangeDelayTimerTick(object sender, EventArgs e)
        {
            _changeDelayTimer.Enabled = false;
            _changeDelayTimer.Dispose();
            _changeDelayTimer = null;
            _slvSelIdxChangedTicks = 0;
            if (_slvSelIdx.Key == _slvSelIdxChangedTicks)
            {
                _selectedIndex = _slvSelIdx.Value;
                if (_selectedIndex < 0)
                {
                    return;
                }
                SelectedIndexChangedAction();
            }
        }

        private long _slvSelIdxChangedTicks;
        private Timer _changeDelayTimer = null;
        private KeyValuePair<long, int> _slvSelIdx;
        private void SubtitleListView1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (subtitleListView1.SelectedItems.Count > 0)
            {
                try
                {
                    if (subtitleListView1.SelectedItems.Count > 1)
                    {
                        if (DateTime.UtcNow.Ticks - _slvSelIdxChangedTicks < 100000)
                        {
                            _slvSelIdx = new KeyValuePair<long, int>(_slvSelIdxChangedTicks, subtitleListView1.SelectedItems[0].Index);
                            if (_changeDelayTimer == null)
                            {
                                _changeDelayTimer = new Timer();
                                _changeDelayTimer.Tick += ChangeDelayTimerTick;
                                _changeDelayTimer.Interval = 200;
                            }
                            else
                            {
                                _changeDelayTimer.Stop();
                                _changeDelayTimer.Start();
                            }
                            _slvSelIdxChangedTicks = DateTime.UtcNow.Ticks;
                            return;
                        }
                        _slvSelIdxChangedTicks = DateTime.UtcNow.Ticks;
                    }
                    _selectedIndex = subtitleListView1.SelectedItems[0].Index;
                }
                catch
                {
                    return;
                }

                SelectedIndexChangedAction();
            }
            else
            {
                _selectedIndex = -1;
            }
        }

        private void SelectedIndexChangedAction()
        {
            if ( _mainOcrBitmap != null)
            {
                ShowSubtitleImage(_selectedIndex, _mainOcrBitmap);
            }
            else
            {
                var bmp = ShowSubtitleImage(_selectedIndex);
                bmp.Dispose();
            }

        }


        public DialogResult EditImageCompareCharacters(string name, string text)
        {
            using (var formVobSubEditCharacters = new VobSubEditCharacters(null, _binaryOcrDb))
            {
                formVobSubEditCharacters.Initialize(name, text);
                DialogResult result = formVobSubEditCharacters.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (_binaryOcrDb != null)
                    {
                        _binaryOcrDb.Save();
                    }

                    return result;
                }

                Cursor = Cursors.WaitCursor;
                if (formVobSubEditCharacters.ChangesMade)
                {
                    _binaryOcrDb.LoadCompareImages();
                }

                Cursor = Cursors.Default;
                return result;
            }
        }

        private void SaveImageAsToolStripMenuItemClick(object sender, EventArgs e)
        {

            saveFileDialog1.Title = LanguageSettings.Current.VobSubOcr.SaveSubtitleImageAs;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.FileName = "Image" + (_selectedIndex + 1);
            saveFileDialog1.Filter = "PNG image|*.png|BMP image|*.bmp|GIF image|*.gif|TIFF image|*.tiff";
            saveFileDialog1.FilterIndex = 0;

            DialogResult result = saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var bmp = GetSubtitleBitmap(_selectedIndex);
                if (bmp == null)
                {
                    MessageBox.Show("No image!");
                    return;
                }

                try
                {
                    if (saveFileDialog1.FilterIndex == 0)
                    {
                        bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (saveFileDialog1.FilterIndex == 1)
                    {
                        bmp.Save(saveFileDialog1.FileName);
                    }
                    else if (saveFileDialog1.FilterIndex == 2)
                    {
                        bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else
                    {
                        bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    bmp.Dispose();
                }
            }
        }

        internal void Initialize(Subtitle bdnSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon)
        {
            if (!string.IsNullOrEmpty(bdnSubtitle.FileName) && bdnSubtitle.FileName != new Subtitle().FileName)
            {
                FileName = bdnSubtitle.FileName;
            }

            SetButtonsStartOcr();

            Text = LanguageSettings.Current.VobSubOcr.TitleBluRay;

            autoTransparentBackgroundToolStripMenuItem.Checked = true;
            autoTransparentBackgroundToolStripMenuItem.Visible = true;
        }


        internal void StartOcrFromDelayed()
        {
            if (_lastAdditions.Count > 0)
            {
                var last = _lastAdditions[_lastAdditions.Count - 1];

                // Simulate a click on ButtonStartOcr in 200ms.
                var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                Utilities.TaskDelay(200).ContinueWith(_ => ButtonStartOcrClick(null, null), uiContext);
            }
        }

        private void VobSubOcr_Resize(object sender, EventArgs e)
        {
            const int originalTopHeight = 105;

            int adjustPercent = (int)(Height * 0.15);
            groupBoxSubtitleImage.Height = originalTopHeight + adjustPercent;



            // Hack for resize after minimize...
            groupBoxSubtitleImage.Width = Width - groupBoxSubtitleImage.Left - 25;
        }

        private void VobSubOcr_ResizeEnd(object sender, EventArgs e)
        {
            VobSubOcr_Resize(null, null);
        }

        internal void SaveAllImagesWithHtmlIndexViewToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                int imagesSavedCount = 0;
                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("   <meta charset=\"UTF-8\" />");
                sb.AppendLine("   <title>Subtitle images</title>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
                _fromMenuItem = true;
                for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
                {
                    var bmp = GetSubtitleBitmap(i);
                    if (bmp != null)
                    {
                        var fileName = string.Format(CultureInfo.InvariantCulture, "{0:0000}.png", i + 1);
                        var filePath = Path.Combine(folderBrowserDialog1.SelectedPath, fileName);
                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        imagesSavedCount++;
                        var p = _subtitle.Paragraphs[i];
                        sb.AppendFormat(CultureInfo.InvariantCulture, "#{3}:{0}->{1}<div style='text-align:center'><img src='{2}' />", p.StartTime.ToShortString(), p.EndTime.ToShortString(), fileName, i + 1);
                        if (!string.IsNullOrEmpty(p.Text))
                        {
                            var backgroundColor = ColorTranslator.ToHtml(Color.WhiteSmoke);
                            var text = WebUtility.HtmlEncode(p.Text.Replace("<i>", "@1__").Replace("</i>", "@2__")).Replace("@1__", "<i>").Replace("@2__", "</i>").Replace(Environment.NewLine, "<br />");
                            sb.Append("<br /><div style='font-size:22px; background-color:").Append(backgroundColor).Append("'>").Append(text).Append("</div>");
                        }

                        sb.AppendLine("</div><br /><hr />");
                        bmp.Dispose();
                    }
                }

                _fromMenuItem = false;
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                var htmlFileName = Path.Combine(folderBrowserDialog1.SelectedPath, "index.html");
                File.WriteAllText(htmlFileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"{imagesSavedCount} images saved in {folderBrowserDialog1.SelectedPath}");
                UiUtil.OpenFile(htmlFileName);
            }
        }


        internal void Initialize(List<SubPicturesWithSeparateTimeCodes> subPicturesWithTimeCodes, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            SetButtonsStartOcr();

            FileName = fileName;
            Text += " - " + Path.GetFileName(FileName);

          
            _subtitle.Renumber();
            subtitleListView1.Fill(_subtitle);
            subtitleListView1.SelectIndexAndEnsureVisible(0);
        }


        private bool HasChangesBeenMade()
        {
            return _subtitle != null && _subtitle.Paragraphs.Any(p => !string.IsNullOrWhiteSpace(p.Text));
        }

        private bool _forceClose;
        private DialogResult _dialogResult;

        private void VobSubOcr_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClose)
            {
                return;
            }

            if (!_okClicked && HasChangesBeenMade())
            {
                if (MessageBox.Show(LanguageSettings.Current.VobSubOcr.DiscardText, LanguageSettings.Current.VobSubOcr.DiscardTitle, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            System.Threading.Thread.Sleep(100);
            Configuration.Settings.VobSubOcr.ItalicFactor = _unItalicFactor;

            if (!e.Cancel)
            {
                e.Cancel = true; // Hack as FormClosing will crash if any Forms are created here (e.g. a msgbox). 
                _forceClose = true;
                _dialogResult = DialogResult;
                TaskDelayHelper.RunDelayed(TimeSpan.FromMilliseconds(10), () =>
                {
                    DialogResult = _dialogResult;
                    Close();
                });
            }
        }
        private PreprocessingSettings _preprocessingSettings;

        private void timerHideStatus_Tick(object sender, EventArgs e)
        {
            timerHideStatus.Stop();
        }
        internal void Initialize(List<TransportStreamSubtitle> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName, string language, bool skipMakeBinary = false)
        {
            SetButtonsStartOcr();
            ShowDvbSubs();

            FileName = fileName;
            _subtitle.FileName = fileName;
            Text += " - " + Path.GetFileName(fileName);

            _fromMenuItem = skipMakeBinary;
        }
        private void ShowDvbSubs()
        {
            _subtitle.Paragraphs.Clear();
            _subtitle.Renumber();
            subtitleListView1.Fill(_subtitle);
            subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        private void numericUpDownAutoTransparentAlphaMax_ValueChanged(object sender, EventArgs e)
        {
            SubtitleListView1SelectedIndexChanged(null, null);
        }

        private void toolStripMenuItemImageSaveAs_Click(object sender, EventArgs e)
        {
            SaveImageAsToolStripMenuItemClick(sender, e);
        }

       
        private void toolStripMenuItemSaveSubtitleAs_Click(object sender, EventArgs e)
        {
            var format = new SubRip();
            saveFileDialog1.Title = LanguageSettings.Current.Main.Menu.File.SaveAs;
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(FileName);
            saveFileDialog1.Filter = format.Name + "|*" + format.Extension;
            saveFileDialog1.DefaultExt = "*" + format.Extension;
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var allText = format.ToText(_subtitle, null);
                File.WriteAllText(saveFileDialog1.FileName, allText, Encoding.UTF8);
                FileName = saveFileDialog1.FileName;
            }
        }

    }
}
