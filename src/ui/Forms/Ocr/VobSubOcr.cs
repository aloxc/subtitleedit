using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats;
using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using Nikse.SubtitleEdit.Core.Enums;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Core.VobSub.Ocr.Service;
using Nikse.SubtitleEdit.Logic;
using Nikse.SubtitleEdit.Logic.Ocr;
using Nikse.SubtitleEdit.Logic.Ocr.Binary;
using Nikse.SubtitleEdit.Logic.Ocr.Tesseract;
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
using System.Xml;
using MessageBox = Nikse.SubtitleEdit.Forms.SeMsgBox.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace Nikse.SubtitleEdit.Forms.Ocr
{
    using LogItem = OcrFixEngine.LogItem;

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

        internal class NOcrThreadParameter
        {
            public int Index { get; set; }
            public int Increment { get; set; }
            public string ResultText { get; set; }
            public List<CompareMatch> ResultMatches { get; set; }
            public NOcrDb NOcrDb { get; set; }
            public BackgroundWorker Self { get; set; }
            public double UnItalicFactor { get; set; }
            public bool AdvancedItalicDetection { get; set; }
            public int NOcrLastLowercaseHeight;
            public int NOcrLastUppercaseHeight;
            public int NumberOfPixelsIsSpace;
            public bool RightToLeft;

            public NOcrThreadParameter(int index, NOcrDb nOcrDb, BackgroundWorker self, int increment, double unItalicFactor, bool advancedItalicDetection, int numberOfPixelsIsSpace, bool rightToLeft)
            {
                Self = self;
                Index = index;
                NOcrDb = nOcrDb;
                Increment = increment;
                UnItalicFactor = unItalicFactor;
                AdvancedItalicDetection = advancedItalicDetection;
                NOcrLastLowercaseHeight = -1;
                NOcrLastUppercaseHeight = -1;
                NumberOfPixelsIsSpace = numberOfPixelsIsSpace;
                RightToLeft = rightToLeft;
            }
        }


        internal class ImageCompareThreadParameter
        {
            public Bitmap Picture { get; set; }
            public int Index { get; set; }
            public int Increment { get; set; }
            public string Result { get; set; }
            public List<CompareItem> CompareBitmaps { get; set; }
            public BackgroundWorker Self { get; set; }
            public int NumberOfPixelsIsSpace;
            public bool RightToLeft;
            public float MaxErrorPercent;

            public ImageCompareThreadParameter(Bitmap picture, int index, List<CompareItem> compareBitmaps, BackgroundWorker self, int increment, int numberOfPixelsIsSpace, bool rightToLeft, float maxErrorPercent)
            {
                Self = self;
                Picture = picture;
                Index = index;
                CompareBitmaps = new List<CompareItem>();
                foreach (CompareItem c in compareBitmaps)
                {
                    CompareBitmaps.Add(c);
                }
                Increment = increment;
                NumberOfPixelsIsSpace = numberOfPixelsIsSpace;
                RightToLeft = rightToLeft;
                MaxErrorPercent = maxErrorPercent;
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

        private class ModiParameter
        {
            public Bitmap Bitmap { get; set; }
            public string Text { get; set; }
            public int Language { get; set; }
        }

        private class OcrFix : LogItem
        {
            public OcrFix(int index, string oldLine, string newLine)
                : base(index + 1, $"{oldLine.Replace(Environment.NewLine, " ")} ⇒ {newLine.Replace(Environment.NewLine, " ")}")
            {
            }
        }

        public delegate void ProgressCallbackDelegate(string progress);
        public ProgressCallbackDelegate ProgressCallback { get; set; }

        private Main _main;
        public string FileName { get; set; }
        private Subtitle _subtitle = new Subtitle();
        private List<CompareItem> _compareBitmaps;
        private XmlDocument _compareDoc = new XmlDocument();
        private Point _manualOcrDialogPosition = new Point(-1, -1);
        private volatile bool _abort;
        private CancellationToken _cancellationToken = CancellationToken.None;
        private int _selectedIndex = -1;
        private VobSubOcrSettings _vobSubOcrSettings;
        private bool _italicCheckedLast;
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
        private int _captureTopAlignHeightThird = -1;

        private Timer _mainOcrTimer;
        private int _mainOcrTimerMax;
        private int _mainOcrIndex;
        private bool _mainOcrRunning;
        private Bitmap _mainOcrBitmap;
        private List<int> _mainOcrSelectedIndices;

        private Type _modiType;
        private object _modiDoc;
        private bool _modiEnabled;

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

        // SP vobsub list (mp4)
        private List<SubPicturesWithSeparateTimeCodes> _mp4List;

        // XSub (divx)
        private List<XSub> _xSubList;

        // DVB (from transport stream)
        private List<TransportStreamSubtitle> _dvbSubtitles;
        private List<Color> _dvbSubColor;
        private bool _transportStreamUseColor;

        // DVB (from transport stream inside mkv)
        private List<DvbSubPes> _dvbPesSubtitles;

        // Other
        private IList<IBinaryParagraphWithPosition> _binaryParagraphWithPositions;

        private string _languageId;
        private string _importLanguageString;

        // Dictionaries/spellchecking/fixing
        private OcrFixEngine _ocrFixEngine;
        private string Tesseract5Version = "5.3.3";

        private Subtitle _bdnXmlOriginal;
        private Subtitle _bdnXmlSubtitle;
        private XmlDocument _bdnXmlDocument;
        private string _bdnFileName;
        private bool _isSon;

        private List<ImageCompareAddition> _lastAdditions = new List<ImageCompareAddition>();
        private readonly VobSubOcrCharacter _vobSubOcrCharacter = new VobSubOcrCharacter();

        private NOcrDb _nOcrDb;
        private readonly VobSubOcrNOcrCharacter _vobSubOcrNOcrCharacter = new VobSubOcrNOcrCharacter();
        public const int NOcrMinColor = 300;
        private bool _ocrThreadStop;

        private IOcrStrategy _ocrService;

        private string[] _tesseractAsyncStrings;
        private int _tesseractAsyncIndex;
        private int _tesseractEngineMode = 3;

        private bool _okClicked;
        private readonly Dictionary<string, int> _unknownWordsDictionary;

        // optimization vars
        private int _numericUpDownPixelsIsSpace = 12;
        private bool _autoLineHeight = true;
        private double _numericUpDownMaxErrorPct = 6;
        private int _ocrMethodIndex;
        private bool _autoBreakLines;
        private bool _hasForcedSubtitles;

        private readonly int _ocrMethodBinaryImageCompare = -1;
        private readonly int _ocrMethodTesseract302 = -1;
        private readonly int _ocrMethodTesseract5 = -1;
        private readonly int _ocrMethodModi = -1;
        private readonly int _ocrMethodNocr = -1;
        private readonly int _ocrMethodCloudVision = -1;

        private FindReplaceDialogHelper _findHelper;
        private FindDialog _findDialog;

        public static void SetDoubleBuffered(Control c)
        {
            //Taxes: Remote Desktop Connection and painting http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
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
            var language = LanguageSettings.Current.VobSubOcr;
            Text = language.Title;
            buttonStartOcr.Text = language.StartOcr;
            buttonPause.Text = LanguageSettings.Current.Settings.Pause;
            groupBoxSubtitleImage.Text = language.SubtitleImage;
            labelSubtitleText.Text = language.SubtitleText;
            subtitleListView1.InitializeLanguage(LanguageSettings.Current.General, Configuration.Settings);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.CharactersPerSeconds);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Actor);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.WordsPerMinute);
            subtitleListView1.HideColumn(SubtitleListView.SubtitleColumn.Region);
            subtitleListView1.AutoSizeColumns();

            labelMinAlpha.Text = language.TransparentMinAlpha;
            toolStripMenuItemCaptureTopAlign.Text = language.CaptureTopAlign;

            groupBoxOcrAutoFix.Text = language.OcrAutoCorrectionSpellChecking;


            groupBoxSubtitleImage.Text = string.Empty;


            FillSpellCheckDictionaries();

            cutToolStripMenuItem.Text = LanguageSettings.Current.Main.Menu.ContextMenu.Cut;
            copyToolStripMenuItem.Text = LanguageSettings.Current.Main.Menu.ContextMenu.Copy;
            pasteToolStripMenuItem.Text = LanguageSettings.Current.Main.Menu.ContextMenu.Paste;
            selectAllToolStripMenuItem.Text = LanguageSettings.Current.Main.Menu.ContextMenu.SelectAll;

            InitializeModi();

      

            toolStripMenuItemCaptureTopAlign.Checked = Configuration.Settings.VobSubOcr.CaptureTopAlign;

            if (Configuration.Settings.VobSubOcr.ItalicFactor >= 0.1 && Configuration.Settings.VobSubOcr.ItalicFactor < 1)
            {
                _unItalicFactor = Configuration.Settings.VobSubOcr.ItalicFactor;
            }

            saveImageAsToolStripMenuItem.Text = language.SaveSubtitleImageAs;
            toolStripMenuItemImageSaveAs.Text = language.SaveSubtitleImageAs;
            previewToolStripMenuItem.Text = LanguageSettings.Current.General.Preview;
            saveAllImagesWithHtmlIndexViewToolStripMenuItem.Text = language.SaveAllSubtitleImagesWithHtml;
            inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Text = language.InspectCompareMatchesForCurrentImage;
            EditLastAdditionsToolStripMenuItem.Text = language.EditLastAdditions;
            setItalicAngleToolStripMenuItem.Text = language.SetItalicAngle;
            imagePreprocessingToolStripMenuItem1.Text = language.ImagePreProcessing;
            autoTransparentBackgroundToolStripMenuItem.Text = language.AutoTransparentBackground;
            toolStripMenuItemSaveSubtitleAs.Text = LanguageSettings.Current.Main.SaveSubtitleAs;

            toolStripMenuItemClearFixes.Text = LanguageSettings.Current.DvdSubRip.Clear;
            toolStripMenuItemClearGuesses.Text = LanguageSettings.Current.DvdSubRip.Clear;
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
            _vobSubOcrSettings = vobSubOcrSettings;

            _vobSubMergedPackList = vobSubMergedPackList;
            _palette = palette;

            _importLanguageString = languageString;
        }


        internal void InitializeQuick(List<VobSubMergedPack> vobSubMergedPackist, List<Color> palette, VobSubOcrSettings vobSubOcrSettings, string languageString)
        {
            SetButtonsStartOcr();
            _vobSubOcrSettings = vobSubOcrSettings;
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

            _cancellationToken = cancellationToken;

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
            _cancellationToken = cancellationToken;
        }

        internal void InitializeBatch(List<SubPicturesWithSeparateTimeCodes> list, string fileName, string language, string ocrEngine)
        {
            Initialize(list, Configuration.Settings.VobSubOcr, fileName);
        }

        internal void InitializeBatch(Subtitle imageListSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon, string language, string ocrEngine)
        {
            Initialize(imageListSubtitle, vobSubOcrSettings, isSon);
            _bdnXmlOriginal = imageListSubtitle;
            _bdnFileName = imageListSubtitle.FileName;
            _isSon = isSon;

        }

        internal void Initialize(List<BluRaySupParser.PcsData> subtitles, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            SetButtonsStartOcr();
            _vobSubOcrSettings = vobSubOcrSettings;

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

            if (_ocrMethodIndex == _ocrMethodBinaryImageCompare)
            {
                var binaryOcrDbs = BinaryOcrDb.GetDatabases();
                
            }
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
            if (_mp4List != null)
            {
                return _mp4List[index].Picture.Forced;
            }

            if (_spList != null)
            {
                return _spList[index].Picture.Forced;
            }

            if (_bdnXmlSubtitle != null)
            {
                return false;
            }

            if (_xSubList != null)
            {
                return false;
            }

            if (_dvbSubtitles != null)
            {
                //                return _dvbSubtitles[index]. ??
                return false;
            }

            if (_dvbPesSubtitles != null)
            {
                return false;
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
            if (_ocrMethodIndex == _ocrMethodCloudVision)
            {
                // Cloud Vision doesn't like transparent images
                makeTransparent = false;
            }

            if (_mp4List != null)
            {
                if (index >= 0 && index < _mp4List.Count)
                {
           
                        returnBmp = _mp4List[index].Picture.GetBitmap(null, Color.Transparent, Color.Black, Color.White, Color.Black, false);
                        if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                        {
                            returnBmp.MakeTransparent();
                        }
                    
                }
            }
            else if (_spList != null)
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
            else if (_bdnXmlSubtitle != null)
            {
                if (index >= 0 && index < _bdnXmlSubtitle.Paragraphs.Count)
                {
                    var fileNames = _bdnXmlSubtitle.Paragraphs[index].Text.SplitToLines();
                    var bitmaps = new List<Bitmap>();
                    int maxWidth = 0;
                    int totalHeight = 0;

                    string fullFileName = string.Empty;
                    if (!string.IsNullOrEmpty(_bdnXmlSubtitle.Paragraphs[index].Extra))
                    {
                        fullFileName = Path.Combine(Path.GetDirectoryName(_bdnFileName), _bdnXmlSubtitle.Paragraphs[index].Extra.Replace("file://", string.Empty));
                    }

                    if (File.Exists(fullFileName))
                    {
                        try
                        {
                            returnBmp = new Bitmap(fullFileName);
                            if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                            {
                                returnBmp.MakeTransparent();
                            }
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                    else
                    {
                        foreach (string fn in fileNames)
                        {
                            fullFileName = Path.Combine(Path.GetDirectoryName(_bdnFileName), fn);

                            if (!File.Exists(fullFileName))
                            {
                                // fix AVISubDetector lines
                                int idxOfIEquals = fn.IndexOf("i=", StringComparison.OrdinalIgnoreCase);
                                if (idxOfIEquals >= 0)
                                {
                                    int idxOfSpace = fn.IndexOf(' ', idxOfIEquals);
                                    if (idxOfSpace > 0)
                                    {
                                        fullFileName = Path.Combine(Path.GetDirectoryName(_bdnFileName), fn.Remove(0, idxOfSpace).Trim());
                                    }
                                }
                            }

                            if (File.Exists(fullFileName))
                            {
                                try
                                {
                                    var temp = new Bitmap(fullFileName);
                                    if (temp.Width > maxWidth)
                                    {
                                        maxWidth = temp.Width;
                                    }

                                    totalHeight += temp.Height;
                                    bitmaps.Add(temp);
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                        }

                        Bitmap b = null;
                        if (bitmaps.Count > 1)
                        {
                            var merged = new Bitmap(maxWidth, totalHeight + 7 * bitmaps.Count);
                            int y = 0;
                            for (int k = 0; k < bitmaps.Count; k++)
                            {
                                Bitmap part = bitmaps[k];
                                if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                                {
                                    part.MakeTransparent();
                                }

                                using (var g = Graphics.FromImage(merged))
                                {
                                    g.DrawImage(part, 0, y);
                                }

                                y += part.Height + 7;
                                part.Dispose();
                            }

                            b = merged;
                        }
                        else if (bitmaps.Count == 1)
                        {
                            b = bitmaps[0];
                        }

                        if (b != null)
                        {

                            if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                            {
                                b.MakeTransparent();
                            }

                            returnBmp = b;
                        }
                    }
                }
            }
            else if (_xSubList != null)
            {
                if (index >= 0 && index < _xSubList.Count)
                {
                   
                        returnBmp = _xSubList[index].GetImage();
                }
            }
            else if (_dvbSubtitles != null)
            {
                if (index >= 0 && index < _dvbSubtitles.Count)
                {
                    var dvbBmp = _dvbSubtitles[index].GetBitmap();
                    var nDvbBmp = new NikseBitmap(dvbBmp);
                    nDvbBmp.CropTopTransparent(2);
                    nDvbBmp.CropTransparentSidesAndBottom(2, true);
                    if (_transportStreamUseColor)
                    {
                        _dvbSubColor[index] = nDvbBmp.GetBrightestColorWhiteIsTransparent();
                    }

                    if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                    {
                        nDvbBmp.MakeBackgroundTransparent((int)numericUpDownAutoTransparentAlphaMax.Value);
                    }

                    dvbBmp.Dispose();
                    returnBmp = nDvbBmp.GetBitmap();
                }
            }
            else if (_dvbPesSubtitles != null)
            {
                if (index >= 0 && index < _dvbPesSubtitles.Count)
                {
                    var dvbBmp = _dvbPesSubtitles[index].GetImageFull();
                    var nDvbBmp = new NikseBitmap(dvbBmp);
                    nDvbBmp.CropTopTransparent(2);
                    nDvbBmp.CropTransparentSidesAndBottom(2, true);
                    if (_transportStreamUseColor)
                    {
                        _dvbSubColor[index] = nDvbBmp.GetBrightestColorWhiteIsTransparent();
                    }

                    if (makeTransparent && autoTransparentBackgroundToolStripMenuItem.Checked)
                    {
                        nDvbBmp.MakeBackgroundTransparent((int)numericUpDownAutoTransparentAlphaMax.Value);
                    }

     
                    dvbBmp.Dispose();
                    returnBmp = nDvbBmp.GetBitmap();
                }
            }
            else if (_binaryParagraphWithPositions != null)
            {
                var bmp = _binaryParagraphWithPositions[index].GetBitmap();
                var nDvbBmp = new NikseBitmap(bmp);
                nDvbBmp.CropTopTransparent(2);
                nDvbBmp.CropTransparentSidesAndBottom(2, true);
                if (_transportStreamUseColor)
                {
                    _dvbSubColor[index] = nDvbBmp.GetBrightestColorWhiteIsTransparent();
                }

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

            if (_ocrMethodIndex == _ocrMethodTesseract5 && !_fromMenuItem)
            {
                var nb = new NikseBitmap(returnBmp);

                if (_preprocessingSettings != null && _preprocessingSettings.CropTransparentColors)
                {
                    nb.CropTransparentSidesAndBottom(2, true);
                    nb.CropTopTransparent(2);
                }

                nb.AddMargin(10);

                if (_preprocessingSettings != null && _preprocessingSettings.InvertColors)
                {
                    nb.InvertColors();
                }

                if (_preprocessingSettings != null && _preprocessingSettings.ScalingPercent > 100)
                {
                    var bTemp = nb.GetBitmap();
                    var f = _preprocessingSettings.ScalingPercent / 100.0;
                    var b = ResizeBitmap(bTemp, (int)Math.Round(bTemp.Width * f), (int)Math.Round(bTemp.Height * f));
                    bTemp.Dispose();
                    nb = new NikseBitmap(b);
                }

                if (_preprocessingSettings != null && _preprocessingSettings.InvertColors)
                {
                    nb.MakeTwoColor(_preprocessingSettings?.BinaryImageCompareThreshold ?? Configuration.Settings.Tools.OcrTesseract4RgbThreshold, Color.Black, Color.White);
                }
                else
                {
                    nb.MakeTwoColor(_preprocessingSettings?.BinaryImageCompareThreshold ?? Configuration.Settings.Tools.OcrTesseract4RgbThreshold, Color.White, Color.Black);
                }

                returnBmp.Dispose();
                return nb.GetBitmap();
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

        private void GetSubtitleTime(int index, out TimeCode start, out TimeCode end)
        {
            if (_mp4List != null)
            {
                var item = _mp4List[index];
                start = new TimeCode(item.Start.TotalMilliseconds);
                end = new TimeCode(item.End.TotalMilliseconds);
            }
            else if (_spList != null)
            {
                var item = _spList[index];
                start = new TimeCode(item.StartTime.TotalMilliseconds);
                end = new TimeCode(item.StartTime.TotalMilliseconds + item.Picture.Delay.TotalMilliseconds);
            }
            else if (_bdnXmlSubtitle != null)
            {
                var item = _bdnXmlSubtitle.Paragraphs[index];
                start = new TimeCode(item.StartTime.TotalMilliseconds);
                end = new TimeCode(item.EndTime.TotalMilliseconds);
            }
            else if (_bluRaySubtitlesOriginal != null)
            {
                var item = _bluRaySubtitles[index];
                start = new TimeCode(item.StartTime / 90.0);
                end = new TimeCode(item.EndTime / 90.0);
            }
            else if (_xSubList != null)
            {
                var item = _xSubList[index];
                start = new TimeCode(item.Start.TotalMilliseconds);
                end = new TimeCode(item.End.TotalMilliseconds);
            }
            else if (_dvbSubtitles != null)
            {
                var item = _dvbSubtitles[index];
                start = new TimeCode(item.StartMilliseconds);
                end = new TimeCode(item.EndMilliseconds);
            }
            else if (_dvbPesSubtitles != null)
            {
                var item = _subtitle.Paragraphs[index];
                start = item.StartTime;
                end = item.EndTime;
            }
            else if (_binaryParagraphWithPositions != null)
            {
                var item = _binaryParagraphWithPositions[index];
                start = item.StartTimeCode;
                end = item.EndTimeCode;
            }
            else
            {
                var item = _vobSubMergedPackList[index];
                start = new TimeCode(item.StartTime.TotalMilliseconds);
                end = new TimeCode(item.EndTime.TotalMilliseconds);
            }
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
            if (_mp4List != null)
            {
                left = 0;
                top = 0;
                width = 0;
                height = 0;
                return;
            }

            if (_spList != null)
            {
                var item = _spList[index];
                left = item.Picture.ImageDisplayArea.Left;
                top = item.Picture.ImageDisplayArea.Top;
                width = item.Picture.ImageDisplayArea.Width;
                height = item.Picture.ImageDisplayArea.Bottom;
                return;
            }

            if (_bdnXmlSubtitle != null)
            {
                var p = _subtitle.GetParagraphOrDefault(index);
                if (p != null && p.Extra != null)
                {
                    var parts = p.Extra.Split(',');
                    if (parts.Length == 2)
                    {
                        left = int.Parse(parts[0]);
                        top = int.Parse(parts[1]);
                        var bmp = GetSubtitleBitmap(index, false);
                        width = bmp.Width;
                        height = bmp.Height;
                        bmp.Dispose();
                        return;
                    }
                }

                left = 0;
                top = 0;
                width = 0;
                height = 0;
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

            if (_xSubList != null)
            {
                left = 0;
                top = 0;
                width = 0;
                height = 0;
                return;
            }

            if (_dvbSubtitles != null)
            {
                var item = _dvbSubtitles[index];
                var pos = item.GetPosition();
                var bmp = item.GetBitmap();
                top = pos.Top;
                left = pos.Left;
                width = bmp.Width;
                height = bmp.Height;
                bmp.Dispose();
                return;
            }

            if (_dvbPesSubtitles != null)
            {
                var item = _subtitle.Paragraphs[index];
                left = 0;
                top = 0;
                width = 0;
                height = 0;

                if (index < _dvbPesSubtitles.Count)
                {
                    var pes = _dvbPesSubtitles[index];
                    var bmp = pes.GetImageFull();
                    var nikseBitmap = new NikseBitmap(bmp);
                    top = nikseBitmap.CropTopTransparent(0);
                    left = nikseBitmap.CropSidesAndBottom(0, Color.FromArgb(0, 0, 0, 0), true);
                    width = nikseBitmap.Width;
                    height = nikseBitmap.Height;
                    bmp.Dispose();
                }

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

            if (_bdnXmlSubtitle != null && File.Exists(_bdnFileName))
            {
                width = 0;
                height = 0;
                try
                {
                    if (_bdnXmlDocument == null)
                    {
                        _bdnXmlDocument = new XmlDocument { XmlResolver = null };
                        _bdnXmlDocument.Load(_bdnFileName);
                    }

                    var formatNode = _bdnXmlDocument.DocumentElement.SelectSingleNode("Description/Format");
                    var videoFormat = formatNode?.Attributes["VideoFormat"].InnerText;
                    if (videoFormat == "480i" || videoFormat == "480p")
                    {
                        width = 720; // not certain
                        height = 480;
                    }
                    else if (videoFormat == "576i" || videoFormat == "576p")
                    {
                        width = 720; // not certain
                        height = 576;
                    }
                    else if (videoFormat == "720i" || videoFormat == "720p")
                    {
                        width = 1280;
                        height = 720;
                    }
                    else if (videoFormat == "1080i" || videoFormat == "1080p")
                    {
                        width = 1920;
                        height = 1080;
                    }
                    else if (videoFormat == "2160i" || videoFormat == "2160p")
                    {
                        width = 3840;
                        height = 2160;
                    }
                    else if (videoFormat == "4320i" || videoFormat == "4320p")
                    {
                        width = 7680;
                        height = 4320;
                    }
                    else if (videoFormat.Contains("x"))
                    {
                        var parts = videoFormat.Split('x');
                        if (parts.Length == 2)
                        {
                            width = int.Parse(parts[0]);
                            height = int.Parse(parts[1]);
                        }
                    }
                }
                catch
                {
                    width = 0;
                    height = 0;
                }

                return;
            }

            if (_bluRaySubtitlesOriginal != null)
            {
                var item = _bluRaySubtitles[index];
                height = item.Size.Height;
                width = item.Size.Width;
            }

            if (_dvbPesSubtitles != null)
            {
                var size = _dvbPesSubtitles[index].GetScreenSize();
                width = size.Width;
                height = size.Height;
            }

            if (_binaryParagraphWithPositions != null)
            {
                var size = _binaryParagraphWithPositions[index].GetScreenSize();
                width = size.Width;
                height = size.Height;
            }

            if (_dvbSubtitles != null)
            {
                var item = _dvbSubtitles[index];
                var pos = item.GetScreenSize();
                width = pos.Width;
                height = pos.Height;
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

        internal CompareMatch GetNOcrCompareMatchNew(ImageSplitterItem targetItem, NikseBitmap parentBitmap, NOcrDb nOcrDb, bool tryItalicScaling, bool deepSeek, int index, List<ImageSplitterItem> list)
        {
            deepSeek = true;
            var expandedResult = nOcrDb.GetMatchExpanded(parentBitmap, targetItem, index, list);
            if (expandedResult != null)
            {
                return new CompareMatch(expandedResult.Text, expandedResult.Italic, expandedResult.ExpandCount, null, expandedResult) { ImageSplitterItem = targetItem };
            }

            NOcrChar result = null;
            if (result == null)
            {

                // try to make letter normal via un-italic angle
                if (tryItalicScaling && targetItem.NikseBitmap != null)
                {
                    var unItalicNikseBitmap = new NikseBitmap(targetItem.NikseBitmap);
                    unItalicNikseBitmap.ReplaceColor(255, 0, 0, 0, 0, 0, 0, 0);
                    unItalicNikseBitmap.MakeTwoColor(200);
                    var oldBmp = unItalicNikseBitmap.GetBitmap();
                    var unItalicImage = UnItalic(oldBmp, _unItalicFactor); //TODO: make un-italic in NikseBitmap
                    unItalicNikseBitmap = new NikseBitmap(unItalicImage);
                    unItalicNikseBitmap.CropTransparentSidesAndBottom(0, false);
                    oldBmp.Dispose();
                    unItalicImage.Dispose();
                    var unItalicTargetItem = new ImageSplitterItem(targetItem.X, targetItem.Y, unItalicNikseBitmap) { Top = targetItem.Top };
                }

                if (result == null)
                {
                    return new CompareMatch("*", false, 0, null);
                }
            }

            var text = FixUppercaseLowercaseIssues(targetItem, result);
            return new CompareMatch(text, result.Italic, 0, null, result) { Y = targetItem.Y };
        }

        public static int _italicFixes = 0;

        private CompareMatch GetCompareMatchNew(ImageSplitterItem targetItem, out CompareMatch secondBestGuess, List<ImageSplitterItem> list, int listIndex, BinaryOcrDb binaryOcrDb)
        {
            double maxDiff = _numericUpDownMaxErrorPct;
            secondBestGuess = null;
            int index = 0;
            int smallestDifference = 10000;
            var target = targetItem.NikseBitmap;
            if (binaryOcrDb == null)
            {
                return null;
            }

            var bob = new BinaryOcrBitmap(target) { X = targetItem.X, Y = targetItem.Top };

            // precise expanded match
            for (int k = 0; k < binaryOcrDb.CompareImagesExpanded.Count; k++)
            {
                var b = binaryOcrDb.CompareImagesExpanded[k];
                if (bob.Hash == b.Hash && bob.Width == b.Width && bob.Height == b.Height && bob.NumberOfColoredPixels == b.NumberOfColoredPixels)
                {
                    bool ok = false;
                    for (int i = 0; i < b.ExpandedList.Count; i++)
                    {
                        if (listIndex + i + 1 < list.Count && list[listIndex + i + 1].NikseBitmap != null)
                        {
                            var bobNext = new BinaryOcrBitmap(list[listIndex + i + 1].NikseBitmap);
                            if (b.ExpandedList[i].Hash == bobNext.Hash)
                            {
                                ok = true;
                            }
                            else
                            {
                                ok = false;
                                break;
                            }
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        return new CompareMatch(b.Text, b.Italic, b.ExpandCount, b.Key);
                    }
                }
            }


            // allow for error %
            for (int k = 0; k < binaryOcrDb.CompareImagesExpanded.Count; k++)
            {
                var b = binaryOcrDb.CompareImagesExpanded[k];
                if (Math.Abs(bob.Width - b.Width) < 3 && Math.Abs(bob.Height - b.Height) < 3 && Math.Abs(bob.NumberOfColoredPixels - b.NumberOfColoredPixels) < 5 && GetPixelDifPercentage(b, bob, target, maxDiff) <= maxDiff)
                {
                    bool ok = false;
                    for (int i = 0; i < b.ExpandedList.Count; i++)
                    {
                        if (listIndex + i + 1 < list.Count && list[listIndex + i + 1].NikseBitmap != null)
                        {
                            var bobNext = new BinaryOcrBitmap(list[listIndex + i + 1].NikseBitmap);
                            if (b.ExpandedList[i].Hash == bobNext.Hash)
                            {
                                ok = true;
                            }
                            else if (Math.Abs(b.ExpandedList[i].Y - bobNext.Y) < 6 && GetPixelDifPercentage(b.ExpandedList[i], bobNext, list[listIndex + i + 1].NikseBitmap, maxDiff) <= maxDiff)
                            {
                                ok = true;
                            }
                            else
                            {
                                ok = false;
                                break;
                            }
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        return new CompareMatch(b.Text, b.Italic, b.ExpandCount, b.Key);
                    }
                }
            }

            FindBestMatchNew(ref index, ref smallestDifference, out var hit, target, binaryOcrDb, bob, maxDiff);
            if (maxDiff > 0)
            {
                if (target.Width > 16 && target.Height > 16 && (hit == null || smallestDifference * 100.0 / (target.Width * target.Height) > maxDiff))
                {
                    var t2 = target.CopyRectangle(new Rectangle(0, 1, target.Width, target.Height - 1));
                    FindBestMatchNew(ref index, ref smallestDifference, out hit, t2, binaryOcrDb, bob, maxDiff);
                }

                if (target.Width > 16 && target.Height > 16 && (hit == null || smallestDifference * 100.0 / (target.Width * target.Height) > maxDiff))
                {
                    var t2 = target.CopyRectangle(new Rectangle(1, 0, target.Width - 1, target.Height));
                    FindBestMatchNew(ref index, ref smallestDifference, out hit, t2, binaryOcrDb, bob, maxDiff);
                }

                if (target.Width > 16 && target.Height > 16 && (hit == null || smallestDifference * 100.0 / (target.Width * target.Height) > maxDiff))
                {
                    var t2 = target.CopyRectangle(new Rectangle(0, 0, target.Width - 1, target.Height));
                    FindBestMatchNew(ref index, ref smallestDifference, out hit, t2, binaryOcrDb, bob, maxDiff);
                }
            }

            if (hit != null)
            {
                double differencePercentage = smallestDifference * 100.0 / (target.Width * target.Height);
                if (differencePercentage <= maxDiff)
                {
                    string text = hit.Text;
                    if (smallestDifference > 0)
                    {
                        int h = hit.Height;
                        if (text == "V" || text == "W" || text == "U" || text == "S" || text == "Z" || text == "O" || text == "X" || text == "Ø" || text == "C")
                        {
                            if (_ocrLowercaseHeightsTotal > 10 && h - _ocrLowercaseHeightsTotal / _ocrLowercaseHeightsTotalCount < 2.0)
                            {
                                text = text.ToLowerInvariant();
                            }
                        }
                        else if (text == "v" || text == "w" || text == "u" || text == "s" || text == "z" || text == "o" || text == "x" || text == "ø" || text == "c")
                        {
                            if (_ocrUppercaseHeightsTotal > 10 && _ocrUppercaseHeightsTotal / _ocrUppercaseHeightsTotalCount - h < 2)
                            {
                                text = text.ToUpperInvariant();
                            }
                        }
                    }
                    else
                    {
                        SetBinOcrLowercaseUppercase(hit.Height, text);
                    }

                    if (differencePercentage > 0)
                    {
                        bool dummy;
                        if ((hit.Text == "l" || hit.Text == "!") && bob.IsLowercaseI(out dummy))
                        {
                            hit = null;
                        }
                        else if ((hit.Text == "i" || hit.Text == "!") && bob.IsLowercaseL())
                        {
                            hit = null;
                        }
                        else if ((hit.Text == "o" || hit.Text == "O") && bob.IsC())
                        {
                            return new CompareMatch(hit.Text == "o" ? "c" : "C", false, 0, null);
                        }
                        else if ((hit.Text == "c" || hit.Text == "C") && !bob.IsC() && bob.IsO())
                        {
                            return new CompareMatch(hit.Text == "c" ? "o" : "O", false, 0, null);
                        }
                    }

                    if (hit != null)
                    {
                        if (differencePercentage < 9 && (text == "e" || text == "d" || text == "a"))
                        {
                            _ocrLowercaseHeightsTotalCount++;
                            _ocrLowercaseHeightsTotal += bob.Height;
                        }

                        return new CompareMatch(text, hit.Italic, hit.ExpandCount, hit.Key);
                    }
                }

                if (hit != null)
                {
                    secondBestGuess = new CompareMatch(hit.Text, hit.Italic, hit.ExpandCount, hit.Key);
                }
            }

            if (maxDiff > 1 && _isLatinDb)
            {
                if (bob.IsPeriod())
                {
                    ImageSplitterItem next = null;
                    if (listIndex + 1 < list.Count)
                    {
                        next = list[listIndex + 1];
                    }

                    if (next?.NikseBitmap == null)
                    {
                        return new CompareMatch(".", false, 0, null);
                    }

                    var nextBob = new BinaryOcrBitmap(next.NikseBitmap) { X = next.X, Y = next.Top };
                    if (!nextBob.IsPeriodAtTop(GetLastBinOcrLowercaseHeight())) // avoid italic ":"
                    {
                        return new CompareMatch(".", false, 0, null);
                    }
                }

                if (bob.IsComma())
                {
                    ImageSplitterItem next = null;
                    if (listIndex + 1 < list.Count)
                    {
                        next = list[listIndex + 1];
                    }

                    if (next?.NikseBitmap == null)
                    {
                        return new CompareMatch(",", false, 0, null);
                    }

                    var nextBob = new BinaryOcrBitmap(next.NikseBitmap) { X = next.X, Y = next.Top };
                    if (!nextBob.IsPeriodAtTop(GetLastBinOcrLowercaseHeight())) // avoid italic ";"
                    {
                        return new CompareMatch(",", false, 0, null);
                    }
                }

                if (bob.IsApostrophe())
                {
                    return new CompareMatch("'", false, 0, null);
                }

                if (bob.IsLowercaseJ()) // "j" detection must be before "i"
                {
                    return new CompareMatch("j", false, 0, null);
                }

                if (bob.IsLowercaseI(out var italicLowercaseI))
                {
                    return new CompareMatch("i", italicLowercaseI, 0, null);
                }

                if (bob.IsColon())
                {
                    return new CompareMatch(":", false, 0, null);
                }

                if (bob.IsExclamationMark())
                {
                    return new CompareMatch("!", false, 0, null);
                }

                if (bob.IsDash())
                {
                    return new CompareMatch("-", false, 0, null);
                }
            }

            return null;
        }

        private static double GetPixelDifPercentage(BinaryOcrBitmap expanded, BinaryOcrBitmap bobNext, NikseBitmap nbmpNext, double maxDiff)
        {
            var difColoredPercentage = (Math.Abs(expanded.NumberOfColoredPixels - bobNext.NumberOfColoredPixels)) * 100.0 / (bobNext.Width * bobNext.Height);
            if (difColoredPercentage > 1 && expanded.Width < 3 || bobNext.Width < 3)
            {
                return 100;
            }

            int dif = int.MaxValue;
            if (expanded.Height == bobNext.Height && expanded.Width == bobNext.Width)
            {
                dif = NikseBitmapImageSplitter.IsBitmapsAlike(nbmpNext, expanded);
            }
            else if (maxDiff > 0)
            {
                if (expanded.Height == bobNext.Height && expanded.Width == bobNext.Width + 1)
                {
                    dif = NikseBitmapImageSplitter.IsBitmapsAlike(nbmpNext, expanded);
                }
                else if (expanded.Height == bobNext.Height && expanded.Width == bobNext.Width - 1)
                {
                    dif = NikseBitmapImageSplitter.IsBitmapsAlike(expanded, nbmpNext);
                }
                else if (expanded.Width == bobNext.Width && expanded.Height == bobNext.Height + 1)
                {
                    dif = NikseBitmapImageSplitter.IsBitmapsAlike(nbmpNext, expanded);
                }
                else if (expanded.Width == bobNext.Width && expanded.Height == bobNext.Height - 1)
                {
                    dif = NikseBitmapImageSplitter.IsBitmapsAlike(expanded, nbmpNext);
                }
            }

            var percentage = dif * 100.0 / (bobNext.Width * bobNext.Height);
            return percentage;
        }

        private static void FindBestMatchNew(ref int index, ref int smallestDifference, out BinaryOcrBitmap hit, NikseBitmap target, BinaryOcrDb binOcrDb, BinaryOcrBitmap bob, double maxDiff)
        {
            hit = null;
            var bobExactMatch = binOcrDb.FindExactMatch(bob);
            if (bobExactMatch >= 0)
            {
                var m = binOcrDb.CompareImages[bobExactMatch];
                index = bobExactMatch;
                smallestDifference = 0;
                hit = m;
                return;
            }

            var tWidth = target.Width;
            var tHeight = target.Height;
            if (maxDiff < 0.2 || tWidth < 3 || tHeight < 5)
            {
                return;
            }

            int numberOfForegroundColors = bob.NumberOfColoredPixels;
            const int minForeColorMatch = 90;

            foreach (var compareItem in binOcrDb.CompareImages)
            {
                if (compareItem.Width == tWidth && compareItem.Height == tHeight) // precise math in size
                {
                    if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < 3)
                    {
                        int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                        if (dif < smallestDifference)
                        {
                            if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                            {
                                continue;
                            }

                            smallestDifference = dif;
                            hit = compareItem;
                            if (dif < 3)
                            {
                                break; // foreach ending
                            }
                        }
                    }
                }
            }

            if (smallestDifference > 1)
            {
                foreach (var compareItem in binOcrDb.CompareImages)
                {
                    if (compareItem.Width == tWidth && compareItem.Height == tHeight) // precise math in size
                    {
                        if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < 40)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                            if (dif < smallestDifference)
                            {
                                if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                {
                                    continue;
                                }

                                smallestDifference = dif;
                                hit = compareItem;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }
                }
            }

            if (tWidth > 16 && tHeight > 16 && smallestDifference > 2) // for other than very narrow letter (like 'i' and 'l' and 'I'), try more sizes
            {
                foreach (var compareItem in binOcrDb.CompareImages)
                {
                    if (compareItem.Width == tWidth && compareItem.Height == tHeight - 1)
                    {
                        if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                        {
                            int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                            if (dif < smallestDifference)
                            {
                                if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                {
                                    continue;
                                }

                                smallestDifference = dif;
                                hit = compareItem;
                                if (dif == 0)
                                {
                                    break; // foreach ending
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 2)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth && compareItem.Height == tHeight + 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 3)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth + 1 && compareItem.Height == tHeight + 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 5)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth - 1 && compareItem.Height == tHeight - 1)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 5)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width - 1 == tWidth && compareItem.Height == tHeight)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 9 && tWidth > 11)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth - 2 && compareItem.Height == tHeight)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 9 && tWidth > 14)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth - 3 && compareItem.Height == tHeight)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 9 && tWidth > 14)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width == tWidth && compareItem.Height == tHeight - 3)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(compareItem, target);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }

                if (smallestDifference > 9 && tWidth > 14)
                {
                    foreach (var compareItem in binOcrDb.CompareImages)
                    {
                        if (compareItem.Width - 2 == tWidth && compareItem.Height == tHeight)
                        {
                            if (Math.Abs(compareItem.NumberOfColoredPixels - numberOfForegroundColors) < minForeColorMatch)
                            {
                                int dif = NikseBitmapImageSplitter.IsBitmapsAlike(target, compareItem);
                                if (dif < smallestDifference)
                                {
                                    if (!BinaryOcrDb.AllowEqual(compareItem, bob))
                                    {
                                        continue;
                                    }

                                    smallestDifference = dif;
                                    hit = compareItem;
                                    if (dif == 0)
                                    {
                                        break; // foreach ending
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (smallestDifference == 0)
            {
                if (binOcrDb.CompareImages.IndexOf(hit) > 200)
                {
                    lock (BinOcrDbMoveFirstLock)
                    {
                        binOcrDb.CompareImages.Remove(hit);
                        binOcrDb.CompareImages.Insert(0, hit);
                        index = 0;
                    }
                }
            }
        }

        private static readonly object BinOcrDbMoveFirstLock = new object();

        private string SaveCompareItemNew(ImageSplitterItem newTarget, string text, bool isItalic, List<ImageSplitterItem> expandList)
        {
            int expandCount = 0;
            if (expandList != null)
            {
                expandCount = expandList.Count;
            }

            if (expandCount > 0)
            {
                var bob = new BinaryOcrBitmap(expandList[0].NikseBitmap, isItalic, expandCount, text, expandList[0].X, expandList[0].Top) { ExpandedList = new List<BinaryOcrBitmap>() };
                for (int j = 1; j < expandList.Count; j++)
                {
                    var expandedBob = new BinaryOcrBitmap(expandList[j].NikseBitmap)
                    {
                        X = expandList[j].X,
                        Y = expandList[j].Top
                    };
                    bob.ExpandedList.Add(expandedBob);
                }

                _binaryOcrDb.Add(bob);
                _binaryOcrDb.Save();
                return bob.Key;
            }
            else
            {
                var bob = new BinaryOcrBitmap(newTarget.NikseBitmap, isItalic, expandCount, text, newTarget.X, newTarget.Top);
                _binaryOcrDb.Add(bob);
                _binaryOcrDb.Save();
                return bob.Key;
            }
        }

        private void ColorLineByNumberOfUnknownWords(int index, int wordsNotFound, string line)
        {
            SetUnknownWordsColor(index, wordsNotFound, line);

            var p = _subtitle.GetParagraphOrDefault(index);
            if (p != null && _unknownWordsDictionary.ContainsKey(p.Id))
            {
                _unknownWordsDictionary[p.Id] = wordsNotFound;
            }
            else if (p != null)
            {
                _unknownWordsDictionary.Add(p.Id, wordsNotFound);
            }
        }

        private void SetUnknownWordsColor(int index, int wordsNotFound, string line)
        {
            if (_ocrFixEngine == null || !_ocrFixEngine.IsDictionaryLoaded)
            {
                subtitleListView1.SetBackgroundColor(index, DefaultBackColor);
                return;
            }

            if (wordsNotFound >= 2)
            {
                subtitleListView1.SetBackgroundColor(index, _listViewOrange);
            }
            else if (wordsNotFound == 1 || line.Length == 1 || line.Contains('_') || HasSingleLetters(line))
            {
                subtitleListView1.SetBackgroundColor(index, _listViewYellow);
            }
            else if (wordsNotFound == 1)
            {
                subtitleListView1.SetBackgroundColor(index, _listViewYellow);
            }
            else if (string.IsNullOrWhiteSpace(line))
            {
                subtitleListView1.SetBackgroundColor(index, _listViewOrange);
            }
            else
            {
                subtitleListView1.SetBackgroundColor(index, _listViewGreen);
            }
        }

        private long _ocrCount;
        private double _ocrHeight = 20;

        /// <summary>
        /// Ocr via binary (two color) image compare
        /// </summary>
        private string SplitAndOcrBinaryImageCompare(Bitmap bitmap, int listViewIndex)
        {
            if (_ocrFixEngine == null)
            {
                LoadOcrFixEngine(string.Empty, LanguageString);
            }

            var matches = new List<CompareMatch>();
            var parentBitmap = new NikseBitmap(bitmap);
            int minLineHeight = GetMinLineHeight();
            var list = NikseBitmapImageSplitter.SplitBitmapToLettersNew(parentBitmap, _numericUpDownPixelsIsSpace, false, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight, _autoLineHeight, _ocrCount > 20 ? _ocrHeight : -1);
            UpdateLineHeights(list);
            int index = 0;
            bool expandSelection = false;
            bool shrinkSelection = false;
            var expandSelectionList = new List<ImageSplitterItem>();
            while (index < list.Count)
            {
                var item = list[index];
                if (expandSelection || shrinkSelection)
                {
                    expandSelection = false;
                    if (shrinkSelection && index > 0)
                    {
                        shrinkSelection = false;
                    }
                    else if (index + 1 < list.Count && list[index + 1].NikseBitmap != null) // only allow expand to EndOfLine or space
                    {
                        index++;
                        expandSelectionList.Add(list[index]);
                    }

                    item = GetExpandedSelectionNew(parentBitmap, expandSelectionList);

                    _vobSubOcrCharacter.Initialize(bitmap, item, _manualOcrDialogPosition, _italicCheckedLast, expandSelectionList.Count > 1, null, _lastAdditions, this);
                    DialogResult result = _vobSubOcrCharacter.ShowDialog(this);
                    _manualOcrDialogPosition = _vobSubOcrCharacter.FormPosition;
                    if (result == DialogResult.Cancel && _vobSubOcrCharacter.SkipImage)
                    {
                        break;
                    }

                    if (result == DialogResult.OK && _vobSubOcrCharacter.ShrinkSelection)
                    {
                        shrinkSelection = true;
                        index--;
                        if (expandSelectionList.Count > 0)
                        {
                            expandSelectionList.RemoveAt(expandSelectionList.Count - 1);
                        }
                    }
                    else if (result == DialogResult.OK && _vobSubOcrCharacter.ExpandSelection)
                    {
                        expandSelection = true;
                    }
                    else if (result == DialogResult.OK)
                    {
                        string text = _vobSubOcrCharacter.ManualRecognizedCharacters;

                        if (!_vobSubOcrCharacter.UseOnce)
                        {
                            string name = SaveCompareItemNew(item, text, _vobSubOcrCharacter.IsItalic, expandSelectionList);
                            var addition = new ImageCompareAddition(name, text, item.NikseBitmap, _vobSubOcrCharacter.IsItalic, listViewIndex);
                            _lastAdditions.Add(addition);
                        }

                        matches.Add(new CompareMatch(text, _vobSubOcrCharacter.IsItalic, expandSelectionList.Count, null));
                        expandSelectionList = new List<ImageSplitterItem>();
                    }
                    else if (result == DialogResult.Abort)
                    {
                        _abort = true;
                    }
                    else
                    {
                        matches.Add(new CompareMatch("*", false, 0, null));
                    }

                    _italicCheckedLast = _vobSubOcrCharacter.IsItalic;
                }
                else if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                }
                else
                {
                    _ocrCount++;
                    _ocrHeight += (item.NikseBitmap.Height - _ocrHeight) / _ocrCount;
                    var match = GetCompareMatchNew(item, out var bestGuess, list, index, _binaryOcrDb);
                    if (match == null) // Try nOCR (line OCR) if no image compare match
                    {
                        if (_nOcrDb != null && _nOcrDb.OcrCharacters.Count > 0)
                        {
                            match = GetNOcrCompareMatchNew(item, parentBitmap, _nOcrDb, true, true, index, list);
                        }
                    }

                    if (match == null)
                    {
                        int nextIndex = index + 1;
                        var allowExpand = nextIndex < list.Count && (list[nextIndex].SpecialCharacter != Environment.NewLine && list[nextIndex].SpecialCharacter != " ");

                        _vobSubOcrCharacter.Initialize(bitmap, item, _manualOcrDialogPosition, _italicCheckedLast, false, bestGuess, _lastAdditions, this, allowExpand);
                        DialogResult result = _vobSubOcrCharacter.ShowDialog(this);
                        _manualOcrDialogPosition = _vobSubOcrCharacter.FormPosition;

                        if (result == DialogResult.Cancel && _vobSubOcrCharacter.SkipImage)
                        {
                            break;
                        }

                        if (result == DialogResult.OK && _vobSubOcrCharacter.ExpandSelection)
                        {
                            expandSelectionList.Add(item);
                            expandSelection = true;
                        }
                        else if (result == DialogResult.OK)
                        {
                            string text = _vobSubOcrCharacter.ManualRecognizedCharacters;

                            if (!_vobSubOcrCharacter.UseOnce)
                            {
                                string name = SaveCompareItemNew(item, text, _vobSubOcrCharacter.IsItalic, null);
                                var addition = new ImageCompareAddition(name, text, item.NikseBitmap, _vobSubOcrCharacter.IsItalic, listViewIndex);
                                _lastAdditions.Add(addition);
                            }

                            matches.Add(new CompareMatch(text, _vobSubOcrCharacter.IsItalic, 0, null, item));
                            SetBinOcrLowercaseUppercase(item.NikseBitmap.Height, text);
                        }
                        else if (result == DialogResult.Abort)
                        {
                            _abort = true;
                        }
                        else
                        {
                            matches.Add(new CompareMatch("*", false, 0, null));
                        }

                        _italicCheckedLast = _vobSubOcrCharacter.IsItalic;
                    }
                    else // found image match
                    {
                        matches.Add(new CompareMatch(match.Text, match.Italic, 0, null, item));
                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                if (_abort)
                {
                    return MatchesToItalicStringConverter.GetStringWithItalicTags(matches);
                }

                if (!expandSelection && !shrinkSelection)
                {
                    index++;
                }

                if (shrinkSelection && expandSelectionList.Count < 2)
                {
                    shrinkSelection = false;
                    expandSelectionList = new List<ImageSplitterItem>();
                }
            }

            string line = MatchesToItalicStringConverter.GetStringWithItalicTags(matches);

            //OCR fix engine
            string textWithOutFixes = line;
            if (_ocrFixEngine != null && _ocrFixEngine.IsDictionaryLoaded)
            {
                var autoGuessLevel = OcrFixEngine.AutoGuessLevel.None;
                int wordsNotFound = _ocrFixEngine.CountUnknownWordsViaDictionary(line, out var correctWords);

                // smaller space pixels for italic
                if (wordsNotFound > 0 && line.Contains("<i>", StringComparison.Ordinal))
                {
                    AddItalicCouldBeSpace(matches, parentBitmap, _unItalicFactor, _numericUpDownPixelsIsSpace);
                }
                if (wordsNotFound > 0 && line.Contains("<i>", StringComparison.Ordinal) && matches.Any(p => p?.ImageSplitterItem?.CouldBeSpaceBefore == true))
                {
                    int j = 1;
                    while (j < matches.Count)
                    {
                        var match = matches[j];
                        var prevMatch = matches[j - 1];
                        if (match.ImageSplitterItem?.CouldBeSpaceBefore == true)
                        {
                            match.ImageSplitterItem.CouldBeSpaceBefore = false;
                            if (prevMatch.Italic)
                            {
                                matches.Insert(j, new CompareMatch(" ", false, 0, string.Empty, new ImageSplitterItem(" ")));
                            }
                        }

                        j++;
                    }

                    var tempLine = MatchesToItalicStringConverter.GetStringWithItalicTags(matches);
                    var oldAutoGuessesUsed = new List<LogItem>(_ocrFixEngine.AutoGuessesUsed);
                    var oldUnknownWordsFound = new List<LogItem>(_ocrFixEngine.UnknownWordsFound);
                    _ocrFixEngine.AutoGuessesUsed.Clear();
                    _ocrFixEngine.UnknownWordsFound.Clear();

                    int tempWordsNotFound = _ocrFixEngine.CountUnknownWordsViaDictionary(tempLine, out var tempCorrectWords);
                    if (tempWordsNotFound <= wordsNotFound && tempCorrectWords > correctWords)
                    {
                        wordsNotFound = tempWordsNotFound;
                        correctWords = tempCorrectWords;
                        line = tempLine;
                    }
                    else
                    {
                        _ocrFixEngine.AutoGuessesUsed = oldAutoGuessesUsed;
                        _ocrFixEngine.UnknownWordsFound = oldUnknownWordsFound;
                    }
                }

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null && string.IsNullOrWhiteSpace(textWithOutFixes.Replace("~", string.Empty)))
                {
                    _ocrFixEngine.AutoGuessesUsed.Clear();
                    _ocrFixEngine.UnknownWordsFound.Clear();
                }

                if (_ocrFixEngine.Abort)
                {
                    ButtonPauseClick(null, null);
                    _ocrFixEngine.Abort = false;

                    if (_ocrFixEngine.LastAction == OcrSpellCheck.Action.InspectCompareMatches)
                    {
                        InspectImageCompareMatchesForCurrentImageToolStripMenuItem_Click(null, null);
                    }

                    return string.Empty;
                }

                // Log used word guesses (via word replace list)


                _ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unknown words guess (found via spelling dictionaries)
                LogUnknownWords();

                ColorLineByNumberOfUnknownWords(listViewIndex, wordsNotFound, line);
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                LogOcrFix(listViewIndex, textWithOutFixes, line);
            }

            return line;
        }

        private string GetLastLastText(int listViewIndex)
        {
            string lastLastLine = null;
            var lastLastP = _subtitle.GetParagraphOrDefault(listViewIndex - 2);
            if (lastLastP != null && !string.IsNullOrEmpty(lastLastP.Text))
            {
                lastLastLine = lastLastP.Text;
            }

            return lastLastLine;
        }

        private static void AddItalicCouldBeSpace(List<CompareMatch> matches, NikseBitmap parentBitmap, double unItalicFactor, int pixelsIsSpace)
        {
            foreach (var match in matches)
            {
                if (match.ImageSplitterItem != null)
                {
                    match.ImageSplitterItem.CouldBeSpaceBefore = false;
                }
            }

            for (int i = 0; i < matches.Count - 1; i++)
            {
                var match = matches[i];
                var matchNext = matches[i + 1];
                if (!match.Italic || matchNext.Text == "," ||
                    string.IsNullOrWhiteSpace(match.Text) || string.IsNullOrWhiteSpace(matchNext.Text) ||
                    match.ImageSplitterItem == null || matchNext.ImageSplitterItem == null)
                {

                    continue;
                }

                int blankVerticalLines = IsVerticalAngledLineTransparent(parentBitmap, match, matchNext, unItalicFactor);
                if (match.Text == "f" || match.Text == "," || matchNext.Text.StartsWith('y') || matchNext.Text.StartsWith('j'))
                {
                    blankVerticalLines++;
                }

                if (blankVerticalLines >= pixelsIsSpace)
                {
                    matchNext.ImageSplitterItem.CouldBeSpaceBefore = true;
                }
            }
        }

        private static int IsVerticalAngledLineTransparent(NikseBitmap parentBitmap, CompareMatch match, CompareMatch next, double unItalicFactor)
        {
            int blanks = 0;
            var min = match.ImageSplitterItem.X + match.ImageSplitterItem.NikseBitmap.Width;
            var max = next.ImageSplitterItem.X + next.ImageSplitterItem.NikseBitmap.Width / 2;
            for (int startX = min; startX < max; startX++)
            {
                var lineBlank = true;
                for (int y = match.ImageSplitterItem.Y; y < match.ImageSplitterItem.Y + match.ImageSplitterItem.NikseBitmap.Height; y++)
                {
                    var x = startX - (y - match.ImageSplitterItem.Y) * unItalicFactor;
                    if (x >= 0)
                    {
                        var color = parentBitmap.GetPixel((int)Math.Round(x), y);
                        if (color.A != 0)
                        {
                            lineBlank = false;
                            if (blanks > 0)
                            {
                                return blanks;
                            }
                        }
                    }
                }

                if (lineBlank)
                {
                    blanks++;
                }
            }

            return blanks;
        }

        private void SetBinOcrLowercaseUppercase(int height, string text)
        {
            if (text == "e" || text == "a")
            {
                _ocrLowercaseHeightsTotalCount++;
                _ocrLowercaseHeightsTotal += height;
            }

            if (text == "E" || text == "H" || text == "R" || text == "D" || text == "T")
            {
                _ocrUppercaseHeightsTotalCount++;
                _ocrUppercaseHeightsTotal += height;
            }
        }

      

        private void UpdateLineHeights(List<ImageSplitterItem> list)
        {
            if (_ocrLetterHeightsTotalCount < 1000)
            {
                foreach (var letter in list)
                {
                    if (letter.NikseBitmap != null)
                    {
                        _ocrLetterHeightsTotal += letter.NikseBitmap.Height;
                        _ocrLetterHeightsTotalCount++;
                    }
                }
            }
        }

        private int GetMinLineHeight()
        {
            if (_ocrMinLineHeight > 0)
            {
                return _ocrMinLineHeight;
            }

            if (_ocrLetterHeightsTotalCount > 20)
            {
                var averageLineHeight = _ocrLetterHeightsTotal / _ocrLetterHeightsTotalCount;
                return (int)Math.Round(averageLineHeight * 0.9);
            }

            return _bluRaySubtitlesOriginal != null ? 25 : 12;
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
            if (_mp4List != null)
            {
                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
            else if (_spList != null)
            {
                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
            else if (_dvbSubtitles != null)
            {

                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
            else if (_dvbPesSubtitles != null)
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
            else if (_xSubList != null)
            {
                SetButtonsEnabledAfterOcrDone();
                buttonStartOcr.Focus();
            }
            else
            {
                ReadyVobSubRip();
            }

            VobSubOcr_Resize(null, null);

            if (Configuration.Settings.VobSubOcr.BinaryAutoDetectBestDb)
            {
                SelectBestImageCompareDatabase();
            }

        }

        public Subtitle ReadyVobSubRip()
        {
            _vobSubMergedPackListOriginal = new List<VobSubMergedPack>();
            bool hasIdxTimeCodes = false;
            _hasForcedSubtitles = false;
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

                if (x.SubPicture.Forced)
                {
                    _hasForcedSubtitles = true;
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
            _mainOcrSelectedIndices = null;
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
            _abort = false;


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
            _captureTopAlign = toolStripMenuItemCaptureTopAlign.Checked;
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

                _captureTopAlignHeightThird = maxHeight / 3;

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


        private readonly object _lockObj = new object();

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


        private static string FixItalics(string input)
        {
            int italicStartCount = Utilities.CountTagInText(input, "<i>");
            if (italicStartCount == 0)
            {
                return input;
            }

            var s = input.Replace(Environment.NewLine + " ", Environment.NewLine);
            s = s.Replace(Environment.NewLine + " ", Environment.NewLine);
            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            s = s.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            s = s.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            if (italicStartCount == 1 && s.Contains("<i>-</i>"))
            {
                s = s.Replace("<i>-</i>", "-");
                s = s.Replace("  ", " ");
            }

            if (s.Contains("</i> / <i>"))
            {
                s = s.Replace("</i> / <i>", " I ").Replace("  ", " ");
            }

            if (s.StartsWith("/ <i>", StringComparison.Ordinal))
            {
                s = ("<i>I " + s.Remove(0, 5)).Replace("  ", " ");
            }

            if (s.StartsWith("I <i>", StringComparison.Ordinal))
            {
                s = ("<i>I " + s.Remove(0, 5)).Replace("  ", " ");
            }
            else if (italicStartCount == 1 && s.Length > 20 &&
                     s.IndexOf("<i>", StringComparison.Ordinal) > 1 && s.IndexOf("<i>", StringComparison.Ordinal) < 10 && s.EndsWith("</i>", StringComparison.Ordinal))
            {
                s = "<i>" + HtmlUtil.RemoveOpenCloseTags(s, HtmlUtil.TagItalic) + "</i>";
            }

            s = s.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);

            s = s.Replace("</i> a <i>", " a ");

            return HtmlUtil.FixInvalidItalicTags(s);
        }

        private void LogUnknownWords()
        {


            _ocrFixEngine.UnknownWordsFound.Clear();
        }

        private void LogOcrFix(int index, string oldLine, string newLine)
        {
        }

        private string CallModi(int i)
        {
            Bitmap bmp;
            try
            {
                var tmp = GetSubtitleBitmap(i);
                if (tmp == null)
                {
                    return string.Empty;
                }

                bmp = tmp.Clone() as Bitmap;
                tmp.Dispose();
            }
            catch
            {
                return string.Empty;
            }

            var mp = new ModiParameter { Bitmap = bmp, Text = "", Language = GetModiLanguage() };

            // We call in a separate thread... or app will crash sometimes :(
            var modiThread = new System.Threading.Thread(DoWork);
            modiThread.Start(mp);
            modiThread.Join(3000); // wait max 3 seconds
            modiThread.Abort();

            if (!string.IsNullOrEmpty(mp.Text) && mp.Text.Length > 3 && mp.Text.EndsWith(";0]", StringComparison.Ordinal))
            {
                mp.Text = mp.Text.Substring(0, mp.Text.Length - 3);
            }

            // Try to avoid blank lines by resizing image
            if (string.IsNullOrEmpty(mp.Text))
            {
                bmp = ResizeBitmap(bmp, (int)(bmp.Width * 1.2), (int)(bmp.Height * 1.2));
                mp = new ModiParameter { Bitmap = bmp, Text = "", Language = GetModiLanguage() };

                // We call in a separate thread... or app will crash sometimes :(
                modiThread = new System.Threading.Thread(DoWork);
                modiThread.Start(mp);
                modiThread.Join(3000); // wait max 3 seconds
                modiThread.Abort();
            }

            int k = 0;
            while (string.IsNullOrEmpty(mp.Text) && k < 5)
            {
                if (string.IsNullOrEmpty(mp.Text))
                {
                    bmp = ResizeBitmap(bmp, (int)(bmp.Width * 1.3), (int)(bmp.Height * 1.4)); // a bit scaling
                    mp = new ModiParameter { Bitmap = bmp, Text = "", Language = GetModiLanguage() };

                    // We call in a separate thread... or app will crash sometimes :(
                    modiThread = new System.Threading.Thread(DoWork);
                    modiThread.Start(mp);
                    modiThread.Join(3000); // wait max 3 seconds
                    modiThread.Abort();
                    k++;
                }
            }

            bmp?.Dispose();

            if (mp.Text != null)
            {
                mp.Text = mp.Text.Replace("•", "o");
            }

            return mp.Text;
        }

        public static void DoWork(object data)
        {
            var paramter = (ModiParameter)data;
            string fileName = Path.GetTempPath() + Path.DirectorySeparatorChar + Guid.NewGuid() + ".bmp";
            Object ocrResult = null;
            try
            {
                paramter.Bitmap.Save(fileName);

                Type modiDocType = Type.GetTypeFromProgID("MODI.Document");
                Object modiDoc = Activator.CreateInstance(modiDocType);
                modiDocType.InvokeMember("Create", BindingFlags.InvokeMethod, null, modiDoc, new Object[] { fileName });

                modiDocType.InvokeMember("OCR", BindingFlags.InvokeMethod, null, modiDoc, new Object[] { paramter.Language, true, true });

                Object images = modiDocType.InvokeMember("Images", BindingFlags.GetProperty, null, modiDoc, new Object[] { });
                Type imagesType = images.GetType();

                Object item = imagesType.InvokeMember("Item", BindingFlags.GetProperty, null, images, new Object[] { "0" });
                Type itemType = item.GetType();

                Object layout = itemType.InvokeMember("Layout", BindingFlags.GetProperty, null, item, new Object[] { });
                Type layoutType = layout.GetType();
                ocrResult = layoutType.InvokeMember("Text", BindingFlags.GetProperty, null, layout, new Object[] { });

                modiDocType.InvokeMember("Close", BindingFlags.InvokeMethod, null, modiDoc, new Object[] { false });
            }
            catch
            {
                paramter.Text = string.Empty;
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
                // ignored
            }

            if (ocrResult != null)
            {
                paramter.Text = ocrResult.ToString().Trim();
            }
        }
        private void InitializeModi()
        {
            _modiEnabled = false;

            if (!Configuration.IsRunningOnWindows)
            {
                return;
            }

            try
            {
                InitializeModiLanguages();

                _modiType = Type.GetTypeFromProgID("MODI.Document");
                _modiDoc = Activator.CreateInstance(_modiType);

                _modiEnabled = _modiDoc != null;
            }
            catch
            {
                _modiEnabled = false;
            }
        }
        private string OcrViaCloudVision(Bitmap bitmap, int listViewIndex)
        {
            if (_ocrFixEngine == null)
            {
                comboBoxDictionaries_SelectedIndexChanged(null, null);
            }

            string line = string.Empty;

            //OCR fix engine
            string textWithOutFixes = line;
            if (_ocrFixEngine != null && _ocrFixEngine.IsDictionaryLoaded)
            {
                var autoGuessLevel = OcrFixEngine.AutoGuessLevel.None;

                int wordsNotFound = _ocrFixEngine.CountUnknownWordsViaDictionary(line, out var correctWords);

                if (wordsNotFound > 0 || correctWords == 0 || textWithOutFixes != null)
                {
                    _ocrFixEngine.AutoGuessesUsed.Clear();
                    _ocrFixEngine.UnknownWordsFound.Clear();
                }

                if (_ocrFixEngine.Abort)
                {
                    ButtonPauseClick(null, null);
                    _ocrFixEngine.Abort = false;

                    return string.Empty;
                }

                // Log used word guesses (via word replace list)


                _ocrFixEngine.AutoGuessesUsed.Clear();

                // Log unknown words guess (found via spelling dictionaries)
                LogUnknownWords();

                ColorLineByNumberOfUnknownWords(listViewIndex, wordsNotFound, line);
            }

            if (textWithOutFixes.Trim() != line.Trim())
            {
                LogOcrFix(listViewIndex, textWithOutFixes, line);
            }

            return line;
        }


        private void InitializeModiLanguages()
        {
            
        }

        private int GetModiLanguage()
        {
            return -1;
        }

        private void ButtonPauseClick(object sender, EventArgs e)
        {
            _mainOcrTimer?.Stop();
            _abort = true;
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

        private void LoadOcrFixEngine(string threeLetterIsoLanguageName, string hunspellName)
        {
            if (_ocrMethodIndex != _ocrMethodTesseract5 && _ocrMethodIndex != _ocrMethodTesseract302)
            {
                try
                {
                    if (!string.IsNullOrEmpty(LanguageString))
                    {
                        var ci = CultureInfo.GetCultureInfo(LanguageString.Replace("_", "-"));
                        _languageId = ci.GetThreeLetterIsoLanguageName();
                    }
                }
                catch
                {
                    // ignored
                }
            }


            var tempOcrFixEngine = new OcrFixEngine(threeLetterIsoLanguageName, hunspellName, this, _ocrMethodIndex == _ocrMethodBinaryImageCompare || _ocrMethodIndex == _ocrMethodNocr);
            var error = _ocrFixEngine?.GetOcrFixReplaceListError();
            if (error != null)
            {
                MessageBox.Show(error);
            }

            if (tempOcrFixEngine.IsDictionaryLoaded)
            {
                _ocrFixEngine?.Dispose();
                _ocrFixEngine = tempOcrFixEngine;
                string loadedDictionaryName = _ocrFixEngine.SpellCheckDictionaryName;
            }
            else
            {
                tempOcrFixEngine.Dispose();
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

        private void comboBoxDictionaries_SelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration.Settings.General.SpellCheckLanguage = LanguageString;
            if (_ocrMethodIndex == _ocrMethodTesseract5 || _ocrMethodIndex == _ocrMethodTesseract302)
            {
                Configuration.Settings.VobSubOcr.LastTesseractSpellCheck = LanguageString;
            }

            string threeLetterIsoLanguageName = string.Empty;
            var language = LanguageString;
            if (language == null)
            {
                _ocrFixEngine?.Dispose();
                _ocrFixEngine = new OcrFixEngine(string.Empty, string.Empty, this, _ocrMethodIndex == _ocrMethodBinaryImageCompare || _ocrMethodIndex == _ocrMethodNocr);
                return;
            }

            try
            {
                _ocrFixEngine?.Dispose();
                _ocrFixEngine = null;
                var ci = CultureInfo.GetCultureInfo(language.Replace("_", "-"));
                threeLetterIsoLanguageName = ci.GetThreeLetterIsoLanguageName();
            }
            catch
            {
                var arr = language.Split('-', '_');
                if (arr.Length > 1 && arr[0].Length == 2)
                {
                    foreach (var x in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                    {
                        if (string.Equals(x.TwoLetterISOLanguageName, arr[0], StringComparison.OrdinalIgnoreCase))
                        {
                            threeLetterIsoLanguageName = x.GetThreeLetterIsoLanguageName();
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(threeLetterIsoLanguageName) && !string.IsNullOrEmpty(language) && language.Length >= 2)
            {
                var twoLetterCode = language.Substring(0, 2);
                var threeLetters = Iso639Dash2LanguageCode.GetThreeLetterCodeFromTwoLetterCode(twoLetterCode);
                if (!string.IsNullOrEmpty(threeLetters))
                {
                    threeLetterIsoLanguageName = threeLetters;
                }
            }

            LoadOcrFixEngine(threeLetterIsoLanguageName, LanguageString);
        }

        internal void Initialize(Subtitle bdnSubtitle, VobSubOcrSettings vobSubOcrSettings, bool isSon)
        {
            if (!string.IsNullOrEmpty(bdnSubtitle.FileName) && bdnSubtitle.FileName != new Subtitle().FileName)
            {
                FileName = bdnSubtitle.FileName;
            }

            _bdnXmlOriginal = bdnSubtitle;
            _bdnFileName = bdnSubtitle.FileName;
            _isSon = isSon;


            SetButtonsStartOcr();
            _vobSubOcrSettings = vobSubOcrSettings;


            Text = LanguageSettings.Current.VobSubOcr.TitleBluRay;
            Text += " - " + Path.GetFileName(_bdnFileName);

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

        private void InspectImageCompareMatchesForCurrentImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (subtitleListView1.SelectedItems.Count != 1)
            {
                return;
            }

            if (_compareBitmaps == null)
            {
                LoadImageCompareBitmaps();
            }

            Cursor = Cursors.WaitCursor;
            var bitmap = GetSubtitleBitmap(subtitleListView1.SelectedItems[0].Index);
            var parentBitmap = new NikseBitmap(bitmap);
            int minLineHeight = GetMinLineHeight();
            Cursor = Cursors.Default;
            using (var inspect = new VobSubOcrCharacterInspect())
            {
                do
                {
                    var matches = new List<CompareMatch>();
                    var sourceList = NikseBitmapImageSplitter.SplitBitmapToLettersNew(parentBitmap, (int)0, false, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight, _autoLineHeight);
                    var imageSources = CalcInspectMatches(sourceList, matches, parentBitmap);
                    var result = inspect.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        Cursor = Cursors.WaitCursor;
                        if (_binaryOcrDb != null)
                        {
                            _binaryOcrDb.Save();
                            Cursor = Cursors.Default;
                        }
                        else
                        {
                            _compareDoc = inspect.ImageCompareDocument;
                            LoadImageCompareBitmaps();
                            Cursor = Cursors.Default;
                        }
                    }
                } while (inspect.DeleteMultiMatch);
            }

            _binaryOcrDb?.LoadCompareImages();
            Cursor = Cursors.Default;
        }

        private List<Bitmap> CalcInspectMatches(List<ImageSplitterItem> sourceList, List<CompareMatch> matches, NikseBitmap parentBitmap)
        {
            int index = 0;
            var imageSources = new List<Bitmap>();
            while (index < sourceList.Count)
            {
                var item = sourceList[index];
                if (item.NikseBitmap == null)
                {
                    matches.Add(new CompareMatch(item.SpecialCharacter, false, 0, null));
                    imageSources.Add(null);
                }
                else
                {
                    var match = GetCompareMatchNew(item, out _, sourceList, index, _binaryOcrDb);
                    if (match == null)
                    {
                        var cm = new CompareMatch(LanguageSettings.Current.VobSubOcr.NoMatch, false, 0, null)
                        {
                            ImageSplitterItem = item
                        };
                        matches.Add(cm);
                        imageSources.Add(item.NikseBitmap.GetBitmap());
                    }
                    else // found image match
                    {
                        if (match.ExpandCount > 0)
                        {
                            List<ImageSplitterItem> expandSelectionList = new List<ImageSplitterItem>();
                            for (int i = 0; i < match.ExpandCount; i++)
                            {
                                expandSelectionList.Add(sourceList[index + i]);
                            }

                            item = GetExpandedSelectionNew(parentBitmap, expandSelectionList);
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, match.Name, item) { Extra = expandSelectionList });
                            imageSources.Add(item.NikseBitmap.GetBitmap());
                        }
                        else
                        {
                            matches.Add(new CompareMatch(match.Text, match.Italic, 0, match.Name, item));
                            imageSources.Add(item.NikseBitmap.GetBitmap());
                        }

                        if (match.ExpandCount > 0)
                        {
                            index += match.ExpandCount - 1;
                        }
                    }
                }

                index++;
            }

            return imageSources;
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

        private void checkBoxAutoTransparentBackground_CheckedChanged(object sender, EventArgs e)
        {
            SubtitleListView1SelectedIndexChanged(null, null);
            if (autoTransparentBackgroundToolStripMenuItem.Checked && _dvbSubtitles != null)
            {
                numericUpDownAutoTransparentAlphaMax.Visible = true;
            }
            else
            {
                numericUpDownAutoTransparentAlphaMax.Visible = false;
            }

            labelMinAlpha.Visible = numericUpDownAutoTransparentAlphaMax.Visible;
        }

        internal void Initialize(List<SubPicturesWithSeparateTimeCodes> subPicturesWithTimeCodes, VobSubOcrSettings vobSubOcrSettings, string fileName)
        {
            _mp4List = subPicturesWithTimeCodes;

            SetButtonsStartOcr();
            _vobSubOcrSettings = vobSubOcrSettings;

            FileName = fileName;
            Text += " - " + Path.GetFileName(FileName);

            foreach (SubPicturesWithSeparateTimeCodes subItem in _mp4List)
            {
                var p = new Paragraph(string.Empty, subItem.Start.TotalMilliseconds, subItem.End.TotalMilliseconds);
                _subtitle.Paragraphs.Add(p);
            }

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
            _abort = true;
            _mainOcrTimer?.Stop();

            _tesseractAsyncIndex = 10000;

            System.Threading.Thread.Sleep(100);
            DisposeImageCompareBitmaps();


            Configuration.Settings.VobSubOcr.ItalicFactor = _unItalicFactor;
            Configuration.Settings.VobSubOcr.CaptureTopAlign = toolStripMenuItemCaptureTopAlign.Checked;


            if (_ocrMethodIndex == _ocrMethodTesseract5 || _ocrMethodTesseract5 == _ocrMethodTesseract302)
            {
                Configuration.Settings.VobSubOcr.LastTesseractSpellCheck = LanguageString;
            }


            if (_ocrMethodIndex == _ocrMethodNocr)
            {
                Configuration.Settings.VobSubOcr.LineOcrLastSpellCheck = LanguageString;
            }

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

        private void ImagePreProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fromMenuItem = true;
            var temp = _preprocessingSettings;
            _preprocessingSettings = null;
            var bmp = GetSubtitleBitmap(_selectedIndex);
            _preprocessingSettings = temp;
            _fromMenuItem = false;

            if (_ocrMethodIndex == _ocrMethodTesseract5)
            {
                var ps = new PreprocessingSettings
                {
                    BinaryImageCompareThreshold = Configuration.Settings.Tools.OcrTesseract4RgbThreshold,
                    InvertColors = _preprocessingSettings != null ? _preprocessingSettings.InvertColors : false,
                    CropTransparentColors = _preprocessingSettings != null ? _preprocessingSettings.CropTransparentColors : false,
                };
                using (var form = new OcrPreprocessingT4(bmp, ps))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        _preprocessingSettings = form.PreprocessingSettings;
                        Configuration.Settings.Tools.OcrTesseract4RgbThreshold = _preprocessingSettings.BinaryImageCompareThreshold;
                        SubtitleListView1SelectedIndexChanged(null, null);
                    }
                }

                return;
            }

            using (var form = new OcrPreprocessingSettings(bmp, _ocrMethodIndex == _ocrMethodBinaryImageCompare || _ocrMethodIndex == _ocrMethodNocr, _preprocessingSettings))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _preprocessingSettings = form.PreprocessingSettings;
                    Configuration.Settings.Tools.OcrBinaryImageCompareRgbThreshold = _preprocessingSettings.BinaryImageCompareThreshold;
                    SubtitleListView1SelectedIndexChanged(null, null);
                }
            }
        }

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
            _vobSubOcrSettings = vobSubOcrSettings;

            _dvbSubtitles = subtitles;
            InitializeDvbSubColor();

            ShowDvbSubs();

            FileName = fileName;
            _subtitle.FileName = fileName;
            Text += " - " + Path.GetFileName(fileName);

            _fromMenuItem = skipMakeBinary;
        }

        private void InitializeDvbSubColor()
        {
            _dvbSubColor = new List<Color>(_dvbSubtitles.Count);
            for (int i = 0; i < _dvbSubtitles.Count; i++)
            {
                _dvbSubColor.Add(Color.Transparent);
            }
        }

        private void ShowDvbSubs()
        {
            _subtitle.Paragraphs.Clear();
            foreach (var sub in _dvbSubtitles)
            {
                _subtitle.Paragraphs.Add(new Paragraph(string.Empty, sub.StartMilliseconds, sub.EndMilliseconds));
            }

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

        private void SelectBestImageCompareDatabase()
        {
            if (_ocrMethodIndex == _ocrMethodBinaryImageCompare)
            {
                var s = FindBestImageCompareDatabase();
                if (string.IsNullOrEmpty(s))
                {
                    return;
                }

                if (Configuration.Settings.VobSubOcr.LastBinaryImageCompareDb != null &&
                    Configuration.Settings.VobSubOcr.LastBinaryImageCompareDb.Contains("+"))
                {
                    s += "+" + Configuration.Settings.VobSubOcr.LastBinaryImageCompareDb.Split('+')[1];
                }

                Configuration.Settings.VobSubOcr.LastBinaryImageCompareDb = s;
            }
        }

        private string FindBestImageCompareDatabase()
        {
            var bestDbName = string.Empty;
            int bestHits = -1;

            using (var bitmap = GetSubtitleBitmap(0))
            {
                if (bitmap == null)
                {
                    return string.Empty;
                }

                var parentBitmap = new NikseBitmap(bitmap);

                foreach (string s in BinaryOcrDb.GetDatabases())
                {
                    var binaryOcrDb = new BinaryOcrDb(Path.Combine(Configuration.OcrDirectory, s + ".db"), true);
                    int minLineHeight = GetMinLineHeight();
                    var sourceList = NikseBitmapImageSplitter.SplitBitmapToLettersNew(parentBitmap, (int)0, false, Configuration.Settings.VobSubOcr.TopToBottom, minLineHeight, _autoLineHeight);
                    int index = 0;
                    int hits = 0;
                    foreach (var item in sourceList)
                    {
                        if (item?.NikseBitmap != null && GetCompareMatchNew(item, out _, sourceList, index, binaryOcrDb) != null)
                        {
                            hits++;
                        }
                    }

                    if (hits > bestHits)
                    {
                        bestDbName = s;
                        bestHits = hits;
                    }
                }
            }

            return bestDbName;
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


        private void toolStripMenuItemCaptureTopAlign_Click(object sender, EventArgs e)
        {
        }

        private void imagePreprocessingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImagePreProcessingToolStripMenuItem_Click(null, null);
        }

        private void setItalicAngleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItemSetUnItalicFactor_Click(null, null);
        }

        

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
