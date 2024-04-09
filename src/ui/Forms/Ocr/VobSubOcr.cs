using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using Nikse.SubtitleEdit.Core.Enums;
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms.Ocr
{
    public sealed partial class VobSubOcr : PositionAndSizeForm, IBinaryParagraphList, IFindAndReplace
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

            public CompareMatch(string text, bool italic, int expandCount, string name, NOcrChar character)
                : this(text, italic, expandCount, name)
            {
                NOcrCharacter = character;
            }

            public CompareMatch(string text, bool italic, int expandCount, string name, ImageSplitterItem imageSplitterItem)
                : this(text, italic, expandCount, name)
            {
                ImageSplitterItem = imageSplitterItem;
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
        private List<CompareItem> _compareBitmaps;
        private int _selectedIndex = -1;
        private double _unItalicFactor = 0.33;

        private BinaryOcrDb _binaryOcrDb;

        private long _ocrLowercaseHeightsTotal;
        private int _ocrLowercaseHeightsTotalCount;
        private long _ocrUppercaseHeightsTotal;
        private int _ocrUppercaseHeightsTotalCount;
        private long _ocrLetterHeightsTotal;
        private int _ocrLetterHeightsTotalCount;
        private int _ocrMinLineHeight = -1;

        private bool _captureTopAlign;
        private int _captureTopAlignHeight = -1;

        private Timer _mainOcrTimer;
        private bool _mainOcrRunning;
        private Bitmap _mainOcrBitmap;
        private bool _hasForcedSubtitles;

        private bool _fromMenuItem;

        // DVD rip/vobsub
        private List<VobSubMergedPack> _vobSubMergedPackListOriginal;
        private List<VobSubMergedPack> _vobSubMergedPackList;
        private List<Color> _palette;

        // Blu-ray sup
        private List<BluRaySupParser.PcsData> _bluRaySubtitlesOriginal;
        private List<BluRaySupParser.PcsData> _bluRaySubtitles;

        // SP list
        private List<SpHeader> _spList;

        private bool _transportStreamUseColor;

        // Other
        private IList<IBinaryParagraphWithPosition> _binaryParagraphWithPositions;

        private string _languageId;
        private string _importLanguageString;

        // Dictionaries/spellchecking/fixing
        private OcrFixEngine _ocrFixEngine;

        private bool _isSon;

        private List<ImageCompareAddition> _lastAdditions = new List<ImageCompareAddition>();
        private readonly VobSubOcrCharacter _vobSubOcrCharacter = new VobSubOcrCharacter();

        private NOcrDb _nOcrDb;
        private readonly VobSubOcrNOcrCharacter _vobSubOcrNOcrCharacter = new VobSubOcrNOcrCharacter();
        public const int NOcrMinColor = 300;
        private bool _ocrThreadStop;

        private IOcrStrategy _ocrService;

        private bool _okClicked;
        private readonly Dictionary<string, int> _unknownWordsDictionary;

        // optimization vars
        private int _numericUpDownPixelsIsSpace = 12;
        private bool _autoLineHeight = true;
        private double _numericUpDownMaxErrorPct = 6;
        private int _ocrMethodIndex;

        private FindReplaceDialogHelper _findHelper;
        private FindDialog _findDialog;

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

            _unknownWordsDictionary = new Dictionary<string, int>();
            buttonPause.Text = LanguageSettings.Current.Settings.Pause;
            subtitleListView1.InitializeLanguage(LanguageSettings.Current.General, Configuration.Settings);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.CharactersPerSeconds);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Actor);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.WordsPerMinute);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Region);
            subtitleListView1.AutoSizeColumns();

            groupBoxSubtitleImage.Text = string.Empty;


            FillSpellCheckDictionaries();

            if (Configuration.Settings.VobSubOcr.ItalicFactor >= 0.1 && Configuration.Settings.VobSubOcr.ItalicFactor < 1)
            {
                _unItalicFactor = Configuration.Settings.VobSubOcr.ItalicFactor;
            }

            previewToolStripMenuItem.Text = LanguageSettings.Current.General.Preview;
            toolStripMenuItemSaveSubtitleAs.Text = LanguageSettings.Current.Main.SaveSubtitleAs;

            toolStripMenuItemClearFixes.Text = LanguageSettings.Current.DvdSubRip.Clear;
            clearToolStripMenuItem.Text = LanguageSettings.Current.DvdSubRip.Clear;


            UiUtil.InitializeSubtitleFont(subtitleListView1);
            subtitleListView1.AutoSizeAllColumns(this);


            splitContainerBottom.Panel1MinSize = 400;
            splitContainerBottom.Panel2MinSize = 250;

            var ocrLanguages = new GoogleOcrService(new GoogleCloudVisionApi(string.Empty)).GetLanguages().OrderBy(p => p.ToString());
            var selectedOcrLanguage = ocrLanguages.FirstOrDefault(p => p.Code == Configuration.Settings.VobSubOcr.CloudVisionLanguage);
            if (selectedOcrLanguage == null)
            {
                selectedOcrLanguage = ocrLanguages.FirstOrDefault(p => p.Code == "en");
            }
        }

        private void FillSpellCheckDictionaries()
        {
        }


        internal void Initialize(List<VobSubMergedPack> vobSubMergedPackList, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            SetButtonsStartOcr();

            _vobSubMergedPackList = vobSubMergedPackList;
            _palette = palette;

            _importLanguageString = languageString;
        }


        internal void InitializeQuick(List<VobSubMergedPack> vobSubMergedPackist, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            SetButtonsStartOcr();
            _vobSubMergedPackList = vobSubMergedPackist;
            _palette = palette;

            _importLanguageString = languageString;
            if (_importLanguageString != null && _importLanguageString.Contains('(') && !_importLanguageString.StartsWith('('))
            {
                _importLanguageString = _importLanguageString.Substring(0, languageString.IndexOf('(') - 1).Trim();
            }
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
            _isSon = isSon;

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


        private void LoadImageCompareBitmaps()
        {
            DisposeImageCompareBitmaps();
            _binaryOcrDb = null;
            _nOcrDb = null;
        }

        private void DisposeImageCompareBitmaps()
        {
            _compareBitmaps = null;
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

        private void LoadVobRip()
        {
            _subtitle = new Subtitle();
            _vobSubMergedPackList = new List<VobSubMergedPack>();
            int max = _vobSubMergedPackListOriginal.Count;
            for (int i = 0; i < max; i++)
            {
                var x = _vobSubMergedPackListOriginal[i];
                _vobSubMergedPackList.Add(x);
                Paragraph p = new Paragraph(string.Empty, x.StartTime.TotalMilliseconds, x.EndTime.TotalMilliseconds);
                _subtitle.Paragraphs.Add(p);
            }

            _subtitle.Renumber();

            FixShortDisplayTimes(_subtitle);

            subtitleListView1.Fill(_subtitle);
            subtitleListView1.SelectIndexAndEnsureVisible(0);

            SetButtonsEnabledAfterOcrDone();
            buttonStartOcr.Focus();
        }

        private void LoadBinarySubtitlesWithPosition()
        {
            _subtitle = new Subtitle();

            int max = _binaryParagraphWithPositions.Count;
            for (int i = 0; i < max; i++)
            {
                var x = _binaryParagraphWithPositions[i];
                _subtitle.Paragraphs.Add(new Paragraph
                {
                    StartTime = new TimeCode(x.StartTimeCode.TotalMilliseconds),
                    EndTime = new TimeCode(x.EndTimeCode.TotalMilliseconds)
                });
            }

            _subtitle.Renumber();

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
            if (_spList != null)
            {
                return _spList[index].Picture.Forced;
            }

            if (_binaryParagraphWithPositions != null)
            {
                return _binaryParagraphWithPositions[index].IsForced;
            }

            if (_bluRaySubtitlesOriginal != null)
            {
                return _bluRaySubtitles[index].IsForced;
            }

            if (_vobSubMergedPackList != null)
            {
                return _vobSubMergedPackList[index].SubPicture.Forced;
            }

            if (_vobSubMergedPackList != null && index < _vobSubMergedPackList.Count)
            {
                return _vobSubMergedPackList[index].SubPicture.Forced;
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

            if (_spList != null)
            {
                if (index >= 0 && index < _spList.Count)
                {
                    
                        returnBmp = _spList[index].Picture.GetBitmap(null, Color.Transparent, Color.Black, Color.White, Color.Black, false);
                        if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                        {
                            returnBmp.MakeTransparent();
                        }
                    
                }
            }
            else if (_binaryParagraphWithPositions != null)
            {
                var bmp = _binaryParagraphWithPositions[index].GetBitmap();
                var nDvbBmp = new NikseBitmap(bmp);
                nDvbBmp.CropTopTransparent(2);
                nDvbBmp.CropTransparentSidesAndBottom(2, true);


                if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                {
                    nDvbBmp.MakeBackgroundTransparent((int)numericUpDownAutoTransparentAlphaMax.Value);
                }
                bmp.Dispose();
                returnBmp = nDvbBmp.GetBitmap();
            }
            else if (_bluRaySubtitlesOriginal != null)
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
            else if (index >= 0 && index < _vobSubMergedPackList.Count)
            {
               
                    returnBmp = _vobSubMergedPackList[index].SubPicture.GetBitmap(_palette, Color.Transparent, Color.Black, Color.White, Color.Black, false, crop);
                    if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                    {
                        returnBmp.MakeTransparent();
                    }
                
            }

            if (returnBmp == null)
            {
                return null;
            }


            if (_binaryOcrDb == null && _nOcrDb == null || _fromMenuItem)
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

            if (_spList != null)
            {
                var item = _spList[index];
                left = item.Picture.ImageDisplayArea.Left;
                top = item.Picture.ImageDisplayArea.Top;
                width = item.Picture.ImageDisplayArea.Width;
                height = item.Picture.ImageDisplayArea.Bottom;
                return;
            }


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

            if (_binaryParagraphWithPositions != null)
            {
                var item = _binaryParagraphWithPositions[index];
                var pos = item.GetPosition();
                left = pos.Left;
                top = pos.Top;
                var bmp = item.GetBitmap();
                width = bmp.Width;
                height = bmp.Height;
                bmp.Dispose();
                return;
            }

            if (_vobSubMergedPackList != null)
            {
                var item = _vobSubMergedPackList[index];
                left = item.SubPicture.ImageDisplayArea.Left;
                top = item.SubPicture.ImageDisplayArea.Top;
                var bmp = item.SubPicture.GetBitmap(_palette, Color.Transparent, Color.Black, Color.White, Color.Black, false, false);
                var nbmp = new NikseBitmap(bmp);
                var topCropped = nbmp.CropTopTransparent(0);
                top += topCropped;
                var bottomCropped = nbmp.CalcBottomTransparent();
                width = bmp.Width;
                height = bmp.Height;
                height -= topCropped;
                height -= bottomCropped;
                bmp.Dispose();
                return;
            }

            left = 0;
            top = 0;
            width = 0;
            height = 0;
        }

        private void GetSubtitleScreenSize(int index, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (_spList != null)
            {
                var item = _spList[index];
                width = item.Picture.ImageDisplayArea.Width;
                height = item.Picture.ImageDisplayArea.Bottom;
                return;
            }


            if (_bluRaySubtitlesOriginal != null)
            {
                var item = _bluRaySubtitles[index];
                height = item.Size.Height;
                width = item.Size.Width;
            }

            if (_binaryParagraphWithPositions != null)
            {
                var size = _binaryParagraphWithPositions[index].GetScreenSize();
                width = size.Width;
                height = size.Height;
            }

            if (_vobSubMergedPackList != null && index < _vobSubMergedPackList.Count)
            {
                var item = _vobSubMergedPackList[index];
                width = item.SubPicture.ImageDisplayArea.Width + +item.SubPicture.ImageDisplayArea.Location.X + 1;
                height = item.SubPicture.ImageDisplayArea.Height + item.SubPicture.ImageDisplayArea.Location.Y + 1;
            }
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

        private static readonly HashSet<string> UppercaseLikeLowercase = new HashSet<string> { "V", "W", "U", "S", "Z", "O", "X", "Ø", "C" };
        private static readonly HashSet<string> LowercaseLikeUppercase = new HashSet<string> { "v", "w", "u", "s", "z", "o", "x", "ø", "c" };
        private static readonly HashSet<string> UppercaseWithAccent = new HashSet<string> { "Č", "Š", "Ž", "Ś", "Ż", "Ś", "Ö", "Ü", "Ú", "Ï", "Í", "Ç", "Ì", "Ò", "Ù", "Ó", "Í" };
        private static readonly HashSet<string> LowercaseWithAccent = new HashSet<string> { "č", "š", "ž", "ś", "ż", "ś", "ö", "ü", "ú", "ï", "í", "ç", "ì", "ò", "ù", "ó", "í" };

        /// <summary>
        /// Fix uppercase/lowercase issues (not I/l)
        /// </summary>
        private string FixUppercaseLowercaseIssues(ImageSplitterItem targetItem, NOcrChar result)
        {
            if (result.Text == "e" || result.Text == "a" || result.Text == "d" || result.Text == "t")
            {
                _ocrLowercaseHeightsTotalCount++;
                _ocrLowercaseHeightsTotal += targetItem.NikseBitmap.Height;
                if (_ocrUppercaseHeightsTotalCount < 3)
                {
                    _ocrUppercaseHeightsTotalCount++;
                    _ocrUppercaseHeightsTotal += targetItem.NikseBitmap.Height + 10;
                }
            }

            if (result.Text == "E" || result.Text == "H" || result.Text == "R" || result.Text == "D" || result.Text == "T" || result.Text == "M")
            {
                _ocrUppercaseHeightsTotalCount++;
                _ocrUppercaseHeightsTotal += targetItem.NikseBitmap.Height;
                if (_ocrLowercaseHeightsTotalCount < 3 && targetItem.NikseBitmap.Height > 20)
                {
                    _ocrLowercaseHeightsTotalCount++;
                    _ocrLowercaseHeightsTotal += targetItem.NikseBitmap.Height - 10;
                }
            }

            if (_ocrLowercaseHeightsTotalCount <= 2 || _ocrUppercaseHeightsTotalCount <= 2)
            {
                return result.Text;
            }

            // Latin letters where lowercase versions look like uppercase version 
            if (UppercaseLikeLowercase.Contains(result.Text))
            {
                var averageLowercase = _ocrLowercaseHeightsTotal / _ocrLowercaseHeightsTotalCount;
                var averageUppercase = _ocrUppercaseHeightsTotal / _ocrUppercaseHeightsTotalCount;
                if (Math.Abs(averageLowercase - targetItem.NikseBitmap.Height) < Math.Abs(averageUppercase - targetItem.NikseBitmap.Height))
                {
                    return result.Text.ToLowerInvariant();
                }

                return result.Text;
            }

            if (LowercaseLikeUppercase.Contains(result.Text))
            {
                var averageLowercase = _ocrLowercaseHeightsTotal / _ocrLowercaseHeightsTotalCount;
                var averageUppercase = _ocrUppercaseHeightsTotal / _ocrUppercaseHeightsTotalCount;
                if (Math.Abs(averageLowercase - targetItem.NikseBitmap.Height) > Math.Abs(averageUppercase - targetItem.NikseBitmap.Height))
                {
                    return result.Text.ToUpperInvariant();
                }

                return result.Text;
            }

            if (UppercaseWithAccent.Contains(result.Text))
            {
                var averageUppercase = _ocrUppercaseHeightsTotal / (double)_ocrUppercaseHeightsTotalCount;
                if (targetItem.NikseBitmap.Height < averageUppercase + 3)
                {
                    return result.Text.ToLowerInvariant();
                }

                return result.Text;
            }

            if (LowercaseWithAccent.Contains(result.Text))
            {
                var averageUppercase = _ocrUppercaseHeightsTotal / (double)_ocrUppercaseHeightsTotalCount;
                if (targetItem.NikseBitmap.Height > averageUppercase + 4)
                {
                    return result.Text.ToUpperInvariant();
                }
            }

            return result.Text;
        }

        public static int _italicFixes = 0;


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
            if (_spList != null)
            {
                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
            else if (_binaryParagraphWithPositions != null)
            {
                LoadBinarySubtitlesWithPosition();
                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
          
            else if (_bluRaySubtitlesOriginal != null)
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
            else
            {
                ReadyVobSubRip();
            }

            VobSubOcr_Resize(null, null);

        }

        public Subtitle ReadyVobSubRip()
        {
            _vobSubMergedPackListOriginal = new List<VobSubMergedPack>();
            bool hasIdxTimeCodes = false;
            if (_vobSubMergedPackList == null)
            {
                return null;
            }

            foreach (var x in _vobSubMergedPackList)
            {
                _vobSubMergedPackListOriginal.Add(x);
                if (x.IdxLine != null)
                {
                    hasIdxTimeCodes = true;
                }
            }

            LoadVobRip();
            return _subtitle;
        }

        private void SetButtonsStartOcr()
        {
            buttonStartOcr.Enabled = false;
            buttonPause.Enabled = true;
            _mainOcrRunning = true;
            subtitleListView1.MultiSelect = false;
        }

        private void SetButtonsEnabledAfterOcrDone()
        {
            buttonStartOcr.Enabled = true;
            buttonPause.Enabled = false;
            _mainOcrRunning = false;
            subtitleListView1.MultiSelect = true;
        }

        private bool _isLatinDb;
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

            new Task(() =>
            {
                //识别结果对象
                var ocrResult = new OCRResult();
                 PaddleOCREngine engine = new PaddleOCREngine(null, new OCRParameter());
                Bitmap bitmap = GetSubtitleBitmap(_selectedIndex);
                //_subtitle.Paragraphs[2].
                if (bitmap == null)
                {
                    MessageBox.Show("No image!");
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
                            txt += list[i].Text + "\r\n";
                        }
                        txt += list[list.Count - 1].Text;
                    }
                }
                this.BeginInvoke(new Action(() =>
                {
                    Clipboard.SetText(txt);
                }));

            }).Start();

            _mainOcrTimer = new Timer();
            //_mainOcrTimer.Tick += mainOcrTimer_Tick;
            _mainOcrTimer.Interval = 5;
            _mainOcrRunning = true;
            subtitleListView1.MultiSelect = false;
            //mainOcrTimer_Tick(null, null);
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
                    else if (_binaryParagraphWithPositions != null && idx < _binaryParagraphWithPositions.Count)
                    {
                        height = _binaryParagraphWithPositions[idx].GetScreenSize().Height;
                        if (height > maxHeight)
                        {
                            maxHeight = height;
                        }
                    }
                    else if (_vobSubMergedPackList != null && idx < _vobSubMergedPackList.Count)
                    {
                        height = _vobSubMergedPackList[idx].GetScreenSize().Height;
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

        private bool HasSingleLetters(string line)
        {
            if (!_ocrFixEngine.IsDictionaryLoaded || !_ocrFixEngine.SpellCheckDictionaryName.StartsWith("en_", StringComparison.Ordinal))
            {
                return false;
            }

            line = line.RemoveChar('[');
            line = line.RemoveChar(']');
            line = HtmlUtil.RemoveOpenCloseTags(line, HtmlUtil.TagItalic);

            var arr = line.Replace("a.m", string.Empty).Replace("p.m", string.Empty).Replace("o.r", string.Empty)
                .Replace("e.g", string.Empty).Replace("Ph.D", string.Empty).Replace("d.t.s", string.Empty)
                .Split(new[] { ' ', ',', '.', '?', '!', '(', ')', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                if (s.Length == 1 && !@"♪♫-:'”1234567890&aAI""".Contains(s))
                {
                    return true;
                }
            }

            return false;
        }

        private void ButtonPauseClick(object sender, EventArgs e)
        {
            _mainOcrTimer?.Stop();
            _ocrThreadStop = true;
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
            if (_mainOcrRunning && _mainOcrBitmap != null)
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

        public string LanguageString
        {
            get
            {
                return null;
            }
        }

        internal void Initialize(Subtitle bdnSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon)
        {
            if (!string.IsNullOrEmpty(bdnSubtitle.FileName) && bdnSubtitle.FileName != new Subtitle().FileName)
            {
                FileName = bdnSubtitle.FileName;
            }

            _isSon = isSon;


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

            splitContainerBottom.Top = groupBoxSubtitleImage.Bottom + 5;


            // Hack for resize after minimize...
            groupBoxSubtitleImage.Width = Width - groupBoxSubtitleImage.Left - 25;
            splitContainerBottom.Width = Width - 40;
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

        private void inspectLastAdditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var formVobSubEditCharacters = new VobSubEditCharacters(_lastAdditions, _binaryOcrDb))
            {
                if (formVobSubEditCharacters.ShowDialog(this) == DialogResult.OK)
                {
                    _lastAdditions = formVobSubEditCharacters.Additions;
                    if (_binaryOcrDb != null)
                    {
                        _binaryOcrDb.Save();
                    }
                }
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

            _ocrThreadStop = true;
            _mainOcrTimer?.Stop();


            System.Threading.Thread.Sleep(100);
            DisposeImageCompareBitmaps();


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
        private void toolStripMenuItemSetUnItalicFactor_Click(object sender, EventArgs e)
        {
            using (var form = new VobSubOcrSetItalicFactor(GetSubtitleBitmap(_selectedIndex), _unItalicFactor))
            {
                form.ShowDialog(this);
                _unItalicFactor = form.GetUnItalicFactor();
            }
        }

        private PreprocessingSettings _preprocessingSettings;

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItemClearFixes_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItemClearGuesses_Click(object sender, EventArgs e)
        {
        }

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

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                if (_ocrFixEngine != null)
                {
                    _ocrFixEngine.Dispose();
                    _ocrFixEngine = null;
                }
            }

            base.Dispose(disposing);
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

        private int GetLastBinOcrLowercaseHeight()
        {
            var lowercaseHeight = 25;
            if (_ocrLowercaseHeightsTotalCount > 5)
            {
                lowercaseHeight = (int)Math.Round((double)_ocrLowercaseHeightsTotal / _ocrLowercaseHeightsTotalCount);
            }

            return lowercaseHeight;
        }

        private void contextMenuStripImage_Opening(object sender, CancelEventArgs e)
        {
            GetSubtitleScreenSize(_selectedIndex, out var width, out var height);
            previewToolStripMenuItem.Visible = width > 0 && height > 0;
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetSubtitleScreenSize(_selectedIndex, out var width, out var height);
            if (width == 0 || height == 0)
            {
                return;
            }

            bool goNext;
            bool goPrevious;
            Cursor = Cursors.WaitCursor;
            try
            {
                GetSubtitleTopAndHeight(_selectedIndex, out var left, out var top, out _, out _);
                using (var bmp = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        var p = _subtitle.Paragraphs[subtitleListView1.SelectedItems[0].Index];
                        FillPreviewBackground(bmp, g, p);
                        _fromMenuItem = true;
                        var subBitmap = GetSubtitleBitmap(_selectedIndex, false);
                        _fromMenuItem = false;

                        if (_vobSubMergedPackList != null)
                        {
                            var nbmp = new NikseBitmap(subBitmap);
                            var topCropped = nbmp.CropTopTransparent(0);
                            top -= topCropped;
                        }

                        g.DrawImageUnscaled(subBitmap, new Point(left, top));
                    }

                    using (var form = new ExportPngXmlPreview(bmp))
                    {

                        Cursor = Cursors.Default;
                        form.AllowNext = _selectedIndex < _subtitle.Paragraphs.Count - 1;
                        form.AllowPrevious = _selectedIndex > 0;
                        form.ShowDialog(this);
                        goNext = form.NextPressed;
                        goPrevious = form.PreviousPressed;
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (goNext)
            {
                subtitleListView1.SelectIndexAndEnsureVisible(_selectedIndex + 1);
                previewToolStripMenuItem_Click(null, null);
            }
            else if (goPrevious)
            {
                subtitleListView1.SelectIndexAndEnsureVisible(_selectedIndex - 1);
                previewToolStripMenuItem_Click(null, null);
            }
        }

        private void FillPreviewBackground(Bitmap bmp, Graphics g, Paragraph p)
        {
            // Draw background with generated image
            var rect = new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1);
            using (var br = new LinearGradientBrush(rect, Color.Black, Color.Black, 0, false))
            {
                var cb = new ColorBlend
                {
                    Positions = new[] { 0, 1 / 6f, 2 / 6f, 3 / 6f, 4 / 6f, 5 / 6f, 1 },
                    Colors = new[] { Color.Black, Color.Black, Color.White, Color.Black, Color.Black, Color.White, Color.Black }
                };
                br.InterpolationColors = cb;
                br.RotateTransform(0);
                g.FillRectangle(br, rect);
            }
        }

 

        private void contextMenuStripUnknownWords_Opening(object sender, CancelEventArgs e)
        {
            string word = null;
            if (string.IsNullOrEmpty(word))
            {
            }
            else
            {
            }
        }

      


        public void FindDialogFind(string findText, ReplaceType findReplaceType, Regex regex)
        {
           
        }

        public void FindDialogFindPrevious(string findText)
        {
            
        }

        public void FindDialogClose()
        {
            if (_findHelper == null)
            {
                return;
            }

            _findHelper.InProgress = false;
        }

        public void ReplaceDialogFind(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            throw new NotImplementedException();
        }

        public void ReplaceDialogReplace(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            throw new NotImplementedException();
        }

        public void ReplaceDialogReplaceAll(FindReplaceDialogHelper findReplaceDialogHelper)
        {
            throw new NotImplementedException();
        }

        public void ReplaceDialogClose()
        {
            throw new NotImplementedException();
        }

        public bool GetAllowReplaceInOriginal()
        {
            return false;
        }
    }
}
