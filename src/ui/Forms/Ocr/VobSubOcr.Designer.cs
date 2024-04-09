﻿using Nikse.SubtitleEdit.Controls;

namespace Nikse.SubtitleEdit.Forms.Ocr
{
    sealed partial class VobSubOcr
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripListview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.oCRSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorOcrSelected = new System.Windows.Forms.ToolStripSeparator();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.italicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importTextWithMatchingTimeCodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importNewTimeCodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveSubtitleAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveImageAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.bDNXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bluraySupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vobSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dOSTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.finalCutProImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageWithTimeCodeInFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorImageCompare = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemInspectNOcrMatches = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditLastAdditionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OcrTrainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSetUnItalicFactor = new System.Windows.Forms.ToolStripMenuItem();
            this.captureTopAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImagePreProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelSubtitleText = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxOCRControls = new System.Windows.Forms.GroupBox();
            this.labelStartFrom = new System.Windows.Forms.Label();
            this.numericUpDownStartNumber = new Nikse.SubtitleEdit.Controls.NikseUpDown();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStartOcr = new System.Windows.Forms.Button();
            this.groupBoxOcrAutoFix = new System.Windows.Forms.GroupBox();
            this.tabControlLogs = new System.Windows.Forms.TabControl();
            this.contextMenuStripAllFixes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClearFixes = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageUnknownWords = new System.Windows.Forms.TabPage();
            this.buttonAddToOcrReplaceList = new System.Windows.Forms.Button();
            this.buttonUknownToUserDic = new System.Windows.Forms.Button();
            this.listBoxUnknownWords = new System.Windows.Forms.ListBox();
            this.contextMenuStripUnknownWords = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageAllFixes = new System.Windows.Forms.TabPage();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.tabPageSuggestions = new System.Windows.Forms.TabPage();
            this.listBoxLogSuggestions = new System.Windows.Forms.ListBox();
            this.contextMenuStripGuessesUsed = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClearGuesses = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxImagePalette = new System.Windows.Forms.GroupBox();
            this.checkBoxBackgroundTransparent = new System.Windows.Forms.CheckBox();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.checkBoxEmphasis2Transparent = new System.Windows.Forms.CheckBox();
            this.checkBoxEmphasis1Transparent = new System.Windows.Forms.CheckBox();
            this.checkBoxPatternTransparent = new System.Windows.Forms.CheckBox();
            this.pictureBoxEmphasis2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxEmphasis1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPattern = new System.Windows.Forms.PictureBox();
            this.checkBoxCustomFourColors = new System.Windows.Forms.CheckBox();
            this.groupBoxSubtitleImage = new System.Windows.Forms.GroupBox();
            this.labelMinAlpha = new System.Windows.Forms.Label();
            this.numericUpDownAutoTransparentAlphaMax = new Nikse.SubtitleEdit.Controls.NikseUpDown();
            this.groupBoxTransportStream = new System.Windows.Forms.GroupBox();
            this.checkBoxTransportStreamGetColorAndSplit = new System.Windows.Forms.CheckBox();
            this.checkBoxTransportStreamGrayscale = new System.Windows.Forms.CheckBox();
            this.pictureBoxSubtitleImage = new System.Windows.Forms.PictureBox();
            this.contextMenuStripImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.autoTransparentBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setItalicAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCaptureTopAlign = new System.Windows.Forms.ToolStripMenuItem();
            this.imagePreprocessingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemImageSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxShowOnlyForced = new System.Windows.Forms.CheckBox();
            this.checkBoxUseTimeCodesFromIdx = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.textBoxCurrentText = new Nikse.SubtitleEdit.Controls.SETextBox();
            this.contextMenuStripTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.normalToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.boldToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.italicToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.underlineToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.subtitleListView1 = new Nikse.SubtitleEdit.Controls.SubtitleListView();
            this.timerHideStatus = new System.Windows.Forms.Timer(this.components);
            this.buttonUknownToNames = new System.Windows.Forms.Button();
            this.contextMenuStripListview.SuspendLayout();
            this.groupBoxOCRControls.SuspendLayout();
            this.groupBoxOcrAutoFix.SuspendLayout();
            this.tabControlLogs.SuspendLayout();
            this.contextMenuStripAllFixes.SuspendLayout();
            this.tabPageUnknownWords.SuspendLayout();
            this.contextMenuStripUnknownWords.SuspendLayout();
            this.tabPageAllFixes.SuspendLayout();
            this.tabPageSuggestions.SuspendLayout();
            this.contextMenuStripGuessesUsed.SuspendLayout();
            this.groupBoxImagePalette.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmphasis2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmphasis1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPattern)).BeginInit();
            this.groupBoxSubtitleImage.SuspendLayout();
            this.groupBoxTransportStream.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSubtitleImage)).BeginInit();
            this.contextMenuStripImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).BeginInit();
            this.splitContainerBottom.Panel1.SuspendLayout();
            this.splitContainerBottom.Panel2.SuspendLayout();
            this.splitContainerBottom.SuspendLayout();
            this.contextMenuStripTextBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripListview
            // 
            this.contextMenuStripListview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oCRSelectedLinesToolStripMenuItem,
            this.toolStripSeparatorOcrSelected,
            this.normalToolStripMenuItem,
            this.italicToolStripMenuItem,
            this.toolStripSeparator1,
            this.importTextWithMatchingTimeCodesToolStripMenuItem,
            this.importNewTimeCodesToolStripMenuItem,
            this.toolStripMenuItemSaveSubtitleAs,
            this.toolStripSeparator2,
            this.saveImageAsToolStripMenuItem,
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem,
            this.toolStripMenuItemExport,
            this.toolStripSeparatorImageCompare,
            this.toolStripMenuItemInspectNOcrMatches,
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem,
            this.EditLastAdditionsToolStripMenuItem,
            this.OcrTrainingToolStripMenuItem,
            this.toolStripSeparator4,
            this.toolStripMenuItemSetUnItalicFactor,
            this.captureTopAlignmentToolStripMenuItem,
            this.ImagePreProcessingToolStripMenuItem,
            this.toolStripSeparator3,
            this.deleteToolStripMenuItem});
            this.contextMenuStripListview.Name = "contextMenuStripListview";
            this.contextMenuStripListview.Size = new System.Drawing.Size(333, 414);
            this.contextMenuStripListview.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripListviewOpening);
            // 
            // oCRSelectedLinesToolStripMenuItem
            // 
            this.oCRSelectedLinesToolStripMenuItem.Name = "oCRSelectedLinesToolStripMenuItem";
            this.oCRSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.oCRSelectedLinesToolStripMenuItem.Text = "OCR selected lines...";
            this.oCRSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.oCRSelectedLinesToolStripMenuItem_Click);
            // 
            // toolStripSeparatorOcrSelected
            // 
            this.toolStripSeparatorOcrSelected.Name = "toolStripSeparatorOcrSelected";
            this.toolStripSeparatorOcrSelected.Size = new System.Drawing.Size(329, 6);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.NormalToolStripMenuItemClick);
            // 
            // italicToolStripMenuItem
            // 
            this.italicToolStripMenuItem.Name = "italicToolStripMenuItem";
            this.italicToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.italicToolStripMenuItem.Text = "Italic";
            this.italicToolStripMenuItem.Click += new System.EventHandler(this.ItalicToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(329, 6);
            // 
            // importTextWithMatchingTimeCodesToolStripMenuItem
            // 
            this.importTextWithMatchingTimeCodesToolStripMenuItem.Name = "importTextWithMatchingTimeCodesToolStripMenuItem";
            this.importTextWithMatchingTimeCodesToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.importTextWithMatchingTimeCodesToolStripMenuItem.Text = "Import text with matching time codes...";
            this.importTextWithMatchingTimeCodesToolStripMenuItem.Click += new System.EventHandler(this.importTextWithMatchingTimeCodesToolStripMenuItem_Click);
            // 
            // importNewTimeCodesToolStripMenuItem
            // 
            this.importNewTimeCodesToolStripMenuItem.Name = "importNewTimeCodesToolStripMenuItem";
            this.importNewTimeCodesToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.importNewTimeCodesToolStripMenuItem.Text = "Import new time codes...";
            this.importNewTimeCodesToolStripMenuItem.Click += new System.EventHandler(this.importNewTimeCodesToolStripMenuItem_Click);
            // 
            // toolStripMenuItemSaveSubtitleAs
            // 
            this.toolStripMenuItemSaveSubtitleAs.Name = "toolStripMenuItemSaveSubtitleAs";
            this.toolStripMenuItemSaveSubtitleAs.Size = new System.Drawing.Size(332, 22);
            this.toolStripMenuItemSaveSubtitleAs.Text = "Save subtitle as...";
            this.toolStripMenuItemSaveSubtitleAs.Click += new System.EventHandler(this.toolStripMenuItemSaveSubtitleAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(329, 6);
            // 
            // saveImageAsToolStripMenuItem
            // 
            this.saveImageAsToolStripMenuItem.Name = "saveImageAsToolStripMenuItem";
            this.saveImageAsToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.saveImageAsToolStripMenuItem.Text = "Save image as...";
            this.saveImageAsToolStripMenuItem.Click += new System.EventHandler(this.SaveImageAsToolStripMenuItemClick);
            // 
            // saveAllImagesWithHtmlIndexViewToolStripMenuItem
            // 
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem.Name = "saveAllImagesWithHtmlIndexViewToolStripMenuItem";
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem.Text = "Save all images with HTML index view...";
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem.Click += new System.EventHandler(this.SaveAllImagesWithHtmlIndexViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItemExport
            // 
            this.toolStripMenuItemExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bDNXMLToolStripMenuItem,
            this.bluraySupToolStripMenuItem,
            this.vobSubToolStripMenuItem,
            this.dOSTToolStripMenuItem,
            this.finalCutProImageToolStripMenuItem,
            this.imageWithTimeCodeInFileNameToolStripMenuItem});
            this.toolStripMenuItemExport.Name = "toolStripMenuItemExport";
            this.toolStripMenuItemExport.Size = new System.Drawing.Size(332, 22);
            this.toolStripMenuItemExport.Text = "Export all images as...";
            // 
            // bDNXMLToolStripMenuItem
            // 
            this.bDNXMLToolStripMenuItem.Name = "bDNXMLToolStripMenuItem";
            this.bDNXMLToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.bDNXMLToolStripMenuItem.Text = "BDN XML...";
            this.bDNXMLToolStripMenuItem.Click += new System.EventHandler(this.BDNXMLToolStripMenuItem_Click);
            // 
            // bluraySupToolStripMenuItem
            // 
            this.bluraySupToolStripMenuItem.Name = "bluraySupToolStripMenuItem";
            this.bluraySupToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.bluraySupToolStripMenuItem.Text = "Blu-ray sup...";
            this.bluraySupToolStripMenuItem.Click += new System.EventHandler(this.BluraySupToolStripMenuItem_Click);
            // 
            // vobSubToolStripMenuItem
            // 
            this.vobSubToolStripMenuItem.Name = "vobSubToolStripMenuItem";
            this.vobSubToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.vobSubToolStripMenuItem.Text = "VobSub...";
            this.vobSubToolStripMenuItem.Click += new System.EventHandler(this.VobSubToolStripMenuItem_Click);
            // 
            // dOSTToolStripMenuItem
            // 
            this.dOSTToolStripMenuItem.Name = "dOSTToolStripMenuItem";
            this.dOSTToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.dOSTToolStripMenuItem.Text = "DOST...";
            this.dOSTToolStripMenuItem.Click += new System.EventHandler(this.DOSTToolStripMenuItem_Click);
            // 
            // finalCutProImageToolStripMenuItem
            // 
            this.finalCutProImageToolStripMenuItem.Name = "finalCutProImageToolStripMenuItem";
            this.finalCutProImageToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.finalCutProImageToolStripMenuItem.Text = "Final Cut Pro + image...";
            this.finalCutProImageToolStripMenuItem.Click += new System.EventHandler(this.finalCutProImageToolStripMenuItem_Click);
            // 
            // imageWithTimeCodeInFileNameToolStripMenuItem
            // 
            this.imageWithTimeCodeInFileNameToolStripMenuItem.Name = "imageWithTimeCodeInFileNameToolStripMenuItem";
            this.imageWithTimeCodeInFileNameToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.imageWithTimeCodeInFileNameToolStripMenuItem.Text = "Images with time code in file name...";
            this.imageWithTimeCodeInFileNameToolStripMenuItem.Click += new System.EventHandler(this.imageWithTimeCodeInFileNameToolStripMenuItem_Click);
            // 
            // toolStripSeparatorImageCompare
            // 
            this.toolStripSeparatorImageCompare.Name = "toolStripSeparatorImageCompare";
            this.toolStripSeparatorImageCompare.Size = new System.Drawing.Size(329, 6);
            // 
            // toolStripMenuItemInspectNOcrMatches
            // 
            this.toolStripMenuItemInspectNOcrMatches.Name = "toolStripMenuItemInspectNOcrMatches";
            this.toolStripMenuItemInspectNOcrMatches.Size = new System.Drawing.Size(332, 22);
            this.toolStripMenuItemInspectNOcrMatches.Text = "Inspect nocr matches for current image...";
            this.toolStripMenuItemInspectNOcrMatches.Click += new System.EventHandler(this.toolStripMenuItemInspectNOcrMatches_Click);
            // 
            // inspectImageCompareMatchesForCurrentImageToolStripMenuItem
            // 
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Name = "inspectImageCompareMatchesForCurrentImageToolStripMenuItem";
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Text = "Inspect compare matches for current image";
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem.Click += new System.EventHandler(this.InspectImageCompareMatchesForCurrentImageToolStripMenuItem_Click);
            // 
            // EditLastAdditionsToolStripMenuItem
            // 
            this.EditLastAdditionsToolStripMenuItem.Name = "EditLastAdditionsToolStripMenuItem";
            this.EditLastAdditionsToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.EditLastAdditionsToolStripMenuItem.Text = "Edit last OCR image additions...";
            this.EditLastAdditionsToolStripMenuItem.Click += new System.EventHandler(this.inspectLastAdditionsToolStripMenuItem_Click);
            // 
            // OcrTrainingToolStripMenuItem
            // 
            this.OcrTrainingToolStripMenuItem.Name = "OcrTrainingToolStripMenuItem";
            this.OcrTrainingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.OcrTrainingToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.OcrTrainingToolStripMenuItem.Text = "OCR training..";
            this.OcrTrainingToolStripMenuItem.Click += new System.EventHandler(this.OcrTrainingToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(329, 6);
            // 
            // toolStripMenuItemSetUnItalicFactor
            // 
            this.toolStripMenuItemSetUnItalicFactor.Name = "toolStripMenuItemSetUnItalicFactor";
            this.toolStripMenuItemSetUnItalicFactor.Size = new System.Drawing.Size(332, 22);
            this.toolStripMenuItemSetUnItalicFactor.Text = "Set italic angle...";
            this.toolStripMenuItemSetUnItalicFactor.Click += new System.EventHandler(this.toolStripMenuItemSetUnItalicFactor_Click);
            // 
            // captureTopAlignmentToolStripMenuItem
            // 
            this.captureTopAlignmentToolStripMenuItem.CheckOnClick = true;
            this.captureTopAlignmentToolStripMenuItem.Name = "captureTopAlignmentToolStripMenuItem";
            this.captureTopAlignmentToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.captureTopAlignmentToolStripMenuItem.Text = "Capture top alignment";
            this.captureTopAlignmentToolStripMenuItem.Click += new System.EventHandler(this.captureTopAlignmentToolStripMenuItem_Click);
            // 
            // ImagePreProcessingToolStripMenuItem
            // 
            this.ImagePreProcessingToolStripMenuItem.Name = "ImagePreProcessingToolStripMenuItem";
            this.ImagePreProcessingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.ImagePreProcessingToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.ImagePreProcessingToolStripMenuItem.Text = "Image preprocessing...";
            this.ImagePreProcessingToolStripMenuItem.Click += new System.EventHandler(this.ImagePreProcessingToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(329, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // labelSubtitleText
            // 
            this.labelSubtitleText.AutoSize = true;
            this.labelSubtitleText.Location = new System.Drawing.Point(7, 5);
            this.labelSubtitleText.Name = "labelSubtitleText";
            this.labelSubtitleText.Size = new System.Drawing.Size(66, 13);
            this.labelSubtitleText.TabIndex = 6;
            this.labelSubtitleText.Text = "Subtitle text";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 564);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(885, 10);
            this.progressBar1.TabIndex = 7;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 543);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(131, 13);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "Loading VobSub images...";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonOK.Location = new System.Drawing.Point(903, 550);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(83, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(992, 550);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(85, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "C&ancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxOCRControls
            // 
            this.groupBoxOCRControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOCRControls.Controls.Add(this.labelStartFrom);
            this.groupBoxOCRControls.Controls.Add(this.numericUpDownStartNumber);
            this.groupBoxOCRControls.Controls.Add(this.buttonPause);
            this.groupBoxOCRControls.Controls.Add(this.buttonStartOcr);
            this.groupBoxOCRControls.Location = new System.Drawing.Point(368, 207);
            this.groupBoxOCRControls.Name = "groupBoxOCRControls";
            this.groupBoxOCRControls.Size = new System.Drawing.Size(287, 84);
            this.groupBoxOCRControls.TabIndex = 2;
            this.groupBoxOCRControls.TabStop = false;
            this.groupBoxOCRControls.Text = "OCR Start/stop";
            // 
            // labelStartFrom
            // 
            this.labelStartFrom.AutoSize = true;
            this.labelStartFrom.Location = new System.Drawing.Point(120, 26);
            this.labelStartFrom.Name = "labelStartFrom";
            this.labelStartFrom.Size = new System.Drawing.Size(127, 13);
            this.labelStartFrom.TabIndex = 1;
            this.labelStartFrom.Text = "Start OCR from subtitle#";
            // 
            // numericUpDownStartNumber
            // 
            this.numericUpDownStartNumber.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownStartNumber.BackColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.numericUpDownStartNumber.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
            this.numericUpDownStartNumber.BorderColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericUpDownStartNumber.ButtonForeColor = System.Drawing.SystemColors.ControlText;
            this.numericUpDownStartNumber.ButtonForeColorDown = System.Drawing.Color.Orange;
            this.numericUpDownStartNumber.ButtonForeColorOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.numericUpDownStartNumber.DecimalPlaces = 0;
            this.numericUpDownStartNumber.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStartNumber.Location = new System.Drawing.Point(123, 47);
            this.numericUpDownStartNumber.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDownStartNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStartNumber.Name = "numericUpDownStartNumber";
            this.numericUpDownStartNumber.Size = new System.Drawing.Size(64, 21);
            this.numericUpDownStartNumber.TabIndex = 3;
            this.numericUpDownStartNumber.TabStop = false;
            this.numericUpDownStartNumber.ThousandsSeparator = false;
            this.numericUpDownStartNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonPause
            // 
            this.buttonPause.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPause.Location = new System.Drawing.Point(11, 52);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(105, 23);
            this.buttonPause.TabIndex = 2;
            this.buttonPause.Text = "Pause OCR";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.ButtonPauseClick);
            // 
            // buttonStartOcr
            // 
            this.buttonStartOcr.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartOcr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStartOcr.Location = new System.Drawing.Point(11, 24);
            this.buttonStartOcr.Name = "buttonStartOcr";
            this.buttonStartOcr.Size = new System.Drawing.Size(105, 23);
            this.buttonStartOcr.TabIndex = 0;
            this.buttonStartOcr.Text = "Start OCR";
            this.buttonStartOcr.UseVisualStyleBackColor = true;
            this.buttonStartOcr.Click += new System.EventHandler(this.ButtonStartOcrClick);
            // 
            // groupBoxOcrAutoFix
            // 
            this.groupBoxOcrAutoFix.Controls.Add(this.tabControlLogs);
            this.groupBoxOcrAutoFix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOcrAutoFix.Location = new System.Drawing.Point(0, 0);
            this.groupBoxOcrAutoFix.Name = "groupBoxOcrAutoFix";
            this.groupBoxOcrAutoFix.Size = new System.Drawing.Size(400, 333);
            this.groupBoxOcrAutoFix.TabIndex = 0;
            this.groupBoxOcrAutoFix.TabStop = false;
            this.groupBoxOcrAutoFix.Text = "OCR auto correction / spell checking";
            // 
            // tabControlLogs
            // 
            this.tabControlLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlLogs.ContextMenuStrip = this.contextMenuStripAllFixes;
            this.tabControlLogs.Controls.Add(this.tabPageUnknownWords);
            this.tabControlLogs.Controls.Add(this.tabPageAllFixes);
            this.tabControlLogs.Controls.Add(this.tabPageSuggestions);
            this.tabControlLogs.Location = new System.Drawing.Point(8, 144);
            this.tabControlLogs.Name = "tabControlLogs";
            this.tabControlLogs.SelectedIndex = 0;
            this.tabControlLogs.Size = new System.Drawing.Size(383, 181);
            this.tabControlLogs.TabIndex = 7;
            // 
            // contextMenuStripAllFixes
            // 
            this.contextMenuStripAllFixes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemClearFixes});
            this.contextMenuStripAllFixes.Name = "contextMenuStripUnknownWords";
            this.contextMenuStripAllFixes.Size = new System.Drawing.Size(107, 26);
            // 
            // toolStripMenuItemClearFixes
            // 
            this.toolStripMenuItemClearFixes.Name = "toolStripMenuItemClearFixes";
            this.toolStripMenuItemClearFixes.Size = new System.Drawing.Size(106, 22);
            this.toolStripMenuItemClearFixes.Text = "Clear";
            this.toolStripMenuItemClearFixes.Click += new System.EventHandler(this.toolStripMenuItemClearFixes_Click);
            // 
            // tabPageUnknownWords
            // 
            this.tabPageUnknownWords.Controls.Add(this.buttonAddToOcrReplaceList);
            this.tabPageUnknownWords.Controls.Add(this.buttonUknownToUserDic);
            this.tabPageUnknownWords.Controls.Add(this.buttonUknownToNames);
            this.tabPageUnknownWords.Controls.Add(this.listBoxUnknownWords);
            this.tabPageUnknownWords.Location = new System.Drawing.Point(4, 22);
            this.tabPageUnknownWords.Name = "tabPageUnknownWords";
            this.tabPageUnknownWords.Size = new System.Drawing.Size(375, 155);
            this.tabPageUnknownWords.TabIndex = 2;
            this.tabPageUnknownWords.Text = "Unknown words";
            this.tabPageUnknownWords.UseVisualStyleBackColor = true;
            // 
            // buttonAddToOcrReplaceList
            // 
            this.buttonAddToOcrReplaceList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddToOcrReplaceList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonAddToOcrReplaceList.Location = new System.Drawing.Point(152, 65);
            this.buttonAddToOcrReplaceList.Name = "buttonAddToOcrReplaceList";
            this.buttonAddToOcrReplaceList.Size = new System.Drawing.Size(217, 23);
            this.buttonAddToOcrReplaceList.TabIndex = 2;
            this.buttonAddToOcrReplaceList.Text = "Add OCR replace pair";
            this.buttonAddToOcrReplaceList.UseVisualStyleBackColor = true;
            this.buttonAddToOcrReplaceList.Click += new System.EventHandler(this.buttonAddToOcrReplaceList_Click);
            // 
            // buttonUknownToUserDic
            // 
            this.buttonUknownToUserDic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUknownToUserDic.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonUknownToUserDic.Location = new System.Drawing.Point(152, 36);
            this.buttonUknownToUserDic.Name = "buttonUknownToUserDic";
            this.buttonUknownToUserDic.Size = new System.Drawing.Size(217, 23);
            this.buttonUknownToUserDic.TabIndex = 1;
            this.buttonUknownToUserDic.Text = "Add to user dictionary";
            this.buttonUknownToUserDic.UseVisualStyleBackColor = true;
            this.buttonUknownToUserDic.Click += new System.EventHandler(this.buttonUnknownToUserDic_Click);
            // 
            // listBoxUnknownWords
            // 
            this.listBoxUnknownWords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxUnknownWords.ContextMenuStrip = this.contextMenuStripUnknownWords;
            this.listBoxUnknownWords.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxUnknownWords.FormattingEnabled = true;
            this.listBoxUnknownWords.HorizontalScrollbar = true;
            this.listBoxUnknownWords.Location = new System.Drawing.Point(3, 3);
            this.listBoxUnknownWords.Name = "listBoxUnknownWords";
            this.listBoxUnknownWords.Size = new System.Drawing.Size(143, 147);
            this.listBoxUnknownWords.TabIndex = 40;
            this.listBoxUnknownWords.SelectedIndexChanged += new System.EventHandler(this.ListBoxLogSelectedIndexChanged);
            this.listBoxUnknownWords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxCopyToClipboard_KeyDown);
            // 
            // contextMenuStripUnknownWords
            // 
            this.contextMenuStripUnknownWords.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.removeAllXToolStripMenuItem});
            this.contextMenuStripUnknownWords.Name = "contextMenuStripUnknownWords";
            this.contextMenuStripUnknownWords.Size = new System.Drawing.Size(146, 48);
            this.contextMenuStripUnknownWords.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripUnknownWords_Opening);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // removeAllXToolStripMenuItem
            // 
            this.removeAllXToolStripMenuItem.Name = "removeAllXToolStripMenuItem";
            this.removeAllXToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.removeAllXToolStripMenuItem.Text = "RemoveAllX";
            this.removeAllXToolStripMenuItem.Click += new System.EventHandler(this.removeAllXToolStripMenuItem_Click);
            // 
            // tabPageAllFixes
            // 
            this.tabPageAllFixes.Controls.Add(this.listBoxLog);
            this.tabPageAllFixes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAllFixes.Name = "tabPageAllFixes";
            this.tabPageAllFixes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAllFixes.Size = new System.Drawing.Size(375, 155);
            this.tabPageAllFixes.TabIndex = 0;
            this.tabPageAllFixes.Text = "All fixes";
            this.tabPageAllFixes.UseVisualStyleBackColor = true;
            // 
            // listBoxLog
            // 
            this.listBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLog.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.Location = new System.Drawing.Point(3, 3);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(369, 149);
            this.listBoxLog.TabIndex = 0;
            this.listBoxLog.SelectedIndexChanged += new System.EventHandler(this.ListBoxLogSelectedIndexChanged);
            this.listBoxLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxCopyToClipboard_KeyDown);
            // 
            // tabPageSuggestions
            // 
            this.tabPageSuggestions.Controls.Add(this.listBoxLogSuggestions);
            this.tabPageSuggestions.Location = new System.Drawing.Point(4, 22);
            this.tabPageSuggestions.Name = "tabPageSuggestions";
            this.tabPageSuggestions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSuggestions.Size = new System.Drawing.Size(375, 155);
            this.tabPageSuggestions.TabIndex = 1;
            this.tabPageSuggestions.Text = "Guesses used";
            this.tabPageSuggestions.UseVisualStyleBackColor = true;
            // 
            // listBoxLogSuggestions
            // 
            this.listBoxLogSuggestions.ContextMenuStrip = this.contextMenuStripGuessesUsed;
            this.listBoxLogSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLogSuggestions.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLogSuggestions.FormattingEnabled = true;
            this.listBoxLogSuggestions.HorizontalScrollbar = true;
            this.listBoxLogSuggestions.Location = new System.Drawing.Point(3, 3);
            this.listBoxLogSuggestions.Name = "listBoxLogSuggestions";
            this.listBoxLogSuggestions.Size = new System.Drawing.Size(369, 149);
            this.listBoxLogSuggestions.TabIndex = 40;
            this.listBoxLogSuggestions.SelectedIndexChanged += new System.EventHandler(this.ListBoxLogSelectedIndexChanged);
            this.listBoxLogSuggestions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxCopyToClipboard_KeyDown);
            // 
            // contextMenuStripGuessesUsed
            // 
            this.contextMenuStripGuessesUsed.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemClearGuesses});
            this.contextMenuStripGuessesUsed.Name = "contextMenuStripUnknownWords";
            this.contextMenuStripGuessesUsed.Size = new System.Drawing.Size(107, 26);
            // 
            // toolStripMenuItemClearGuesses
            // 
            this.toolStripMenuItemClearGuesses.Name = "toolStripMenuItemClearGuesses";
            this.toolStripMenuItemClearGuesses.Size = new System.Drawing.Size(106, 22);
            this.toolStripMenuItemClearGuesses.Text = "Clear";
            this.toolStripMenuItemClearGuesses.Click += new System.EventHandler(this.toolStripMenuItemClearGuesses_Click);
            // 
            // groupBoxImagePalette
            // 
            this.groupBoxImagePalette.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxImagePalette.Controls.Add(this.checkBoxBackgroundTransparent);
            this.groupBoxImagePalette.Controls.Add(this.pictureBoxBackground);
            this.groupBoxImagePalette.Controls.Add(this.checkBoxEmphasis2Transparent);
            this.groupBoxImagePalette.Controls.Add(this.checkBoxEmphasis1Transparent);
            this.groupBoxImagePalette.Controls.Add(this.checkBoxPatternTransparent);
            this.groupBoxImagePalette.Controls.Add(this.pictureBoxEmphasis2);
            this.groupBoxImagePalette.Controls.Add(this.pictureBoxEmphasis1);
            this.groupBoxImagePalette.Controls.Add(this.pictureBoxPattern);
            this.groupBoxImagePalette.Controls.Add(this.checkBoxCustomFourColors);
            this.groupBoxImagePalette.Location = new System.Drawing.Point(13, 16);
            this.groupBoxImagePalette.Name = "groupBoxImagePalette";
            this.groupBoxImagePalette.Size = new System.Drawing.Size(644, 38);
            this.groupBoxImagePalette.TabIndex = 35;
            this.groupBoxImagePalette.TabStop = false;
            this.groupBoxImagePalette.Text = "Image palette";
            // 
            // checkBoxBackgroundTransparent
            // 
            this.checkBoxBackgroundTransparent.AutoSize = true;
            this.checkBoxBackgroundTransparent.Checked = true;
            this.checkBoxBackgroundTransparent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBackgroundTransparent.Location = new System.Drawing.Point(144, 19);
            this.checkBoxBackgroundTransparent.Name = "checkBoxBackgroundTransparent";
            this.checkBoxBackgroundTransparent.Size = new System.Drawing.Size(85, 17);
            this.checkBoxBackgroundTransparent.TabIndex = 8;
            this.checkBoxBackgroundTransparent.Text = "Transparent";
            this.checkBoxBackgroundTransparent.UseVisualStyleBackColor = true;
            this.checkBoxBackgroundTransparent.CheckedChanged += new System.EventHandler(this.CheckBoxPatternTransparentCheckedChanged);
            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBackground.Location = new System.Drawing.Point(120, 12);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(21, 21);
            this.pictureBoxBackground.TabIndex = 7;
            this.pictureBoxBackground.TabStop = false;
            this.pictureBoxBackground.Click += new System.EventHandler(this.PictureBoxColorChooserClick);
            // 
            // checkBoxEmphasis2Transparent
            // 
            this.checkBoxEmphasis2Transparent.AutoSize = true;
            this.checkBoxEmphasis2Transparent.Location = new System.Drawing.Point(507, 19);
            this.checkBoxEmphasis2Transparent.Name = "checkBoxEmphasis2Transparent";
            this.checkBoxEmphasis2Transparent.Size = new System.Drawing.Size(85, 17);
            this.checkBoxEmphasis2Transparent.TabIndex = 6;
            this.checkBoxEmphasis2Transparent.Text = "Transparent";
            this.checkBoxEmphasis2Transparent.UseVisualStyleBackColor = true;
            this.checkBoxEmphasis2Transparent.CheckedChanged += new System.EventHandler(this.CheckBoxEmphasis2TransparentCheckedChanged);
            // 
            // checkBoxEmphasis1Transparent
            // 
            this.checkBoxEmphasis1Transparent.AutoSize = true;
            this.checkBoxEmphasis1Transparent.Location = new System.Drawing.Point(387, 19);
            this.checkBoxEmphasis1Transparent.Name = "checkBoxEmphasis1Transparent";
            this.checkBoxEmphasis1Transparent.Size = new System.Drawing.Size(85, 17);
            this.checkBoxEmphasis1Transparent.TabIndex = 5;
            this.checkBoxEmphasis1Transparent.Text = "Transparent";
            this.checkBoxEmphasis1Transparent.UseVisualStyleBackColor = true;
            this.checkBoxEmphasis1Transparent.CheckedChanged += new System.EventHandler(this.CheckBoxEmphasis1TransparentCheckedChanged);
            // 
            // checkBoxPatternTransparent
            // 
            this.checkBoxPatternTransparent.AutoSize = true;
            this.checkBoxPatternTransparent.Location = new System.Drawing.Point(266, 19);
            this.checkBoxPatternTransparent.Name = "checkBoxPatternTransparent";
            this.checkBoxPatternTransparent.Size = new System.Drawing.Size(85, 17);
            this.checkBoxPatternTransparent.TabIndex = 4;
            this.checkBoxPatternTransparent.Text = "Transparent";
            this.checkBoxPatternTransparent.UseVisualStyleBackColor = true;
            this.checkBoxPatternTransparent.CheckedChanged += new System.EventHandler(this.CheckBoxPatternTransparentCheckedChanged);
            // 
            // pictureBoxEmphasis2
            // 
            this.pictureBoxEmphasis2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxEmphasis2.Location = new System.Drawing.Point(484, 12);
            this.pictureBoxEmphasis2.Name = "pictureBoxEmphasis2";
            this.pictureBoxEmphasis2.Size = new System.Drawing.Size(21, 21);
            this.pictureBoxEmphasis2.TabIndex = 3;
            this.pictureBoxEmphasis2.TabStop = false;
            this.pictureBoxEmphasis2.Click += new System.EventHandler(this.PictureBoxColorChooserClick);
            this.pictureBoxEmphasis2.DoubleClick += new System.EventHandler(this.PictureBoxColorChooserClick);
            // 
            // pictureBoxEmphasis1
            // 
            this.pictureBoxEmphasis1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxEmphasis1.Location = new System.Drawing.Point(363, 12);
            this.pictureBoxEmphasis1.Name = "pictureBoxEmphasis1";
            this.pictureBoxEmphasis1.Size = new System.Drawing.Size(21, 21);
            this.pictureBoxEmphasis1.TabIndex = 2;
            this.pictureBoxEmphasis1.TabStop = false;
            this.pictureBoxEmphasis1.Click += new System.EventHandler(this.PictureBoxColorChooserClick);
            this.pictureBoxEmphasis1.DoubleClick += new System.EventHandler(this.PictureBoxColorChooserClick);
            // 
            // pictureBoxPattern
            // 
            this.pictureBoxPattern.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPattern.Location = new System.Drawing.Point(242, 12);
            this.pictureBoxPattern.Name = "pictureBoxPattern";
            this.pictureBoxPattern.Size = new System.Drawing.Size(21, 21);
            this.pictureBoxPattern.TabIndex = 1;
            this.pictureBoxPattern.TabStop = false;
            this.pictureBoxPattern.Click += new System.EventHandler(this.PictureBoxColorChooserClick);
            this.pictureBoxPattern.DoubleClick += new System.EventHandler(this.PictureBoxColorChooserClick);
            // 
            // checkBoxCustomFourColors
            // 
            this.checkBoxCustomFourColors.AutoSize = true;
            this.checkBoxCustomFourColors.Location = new System.Drawing.Point(7, 16);
            this.checkBoxCustomFourColors.Name = "checkBoxCustomFourColors";
            this.checkBoxCustomFourColors.Size = new System.Drawing.Size(116, 17);
            this.checkBoxCustomFourColors.TabIndex = 0;
            this.checkBoxCustomFourColors.Text = "Use custom colors:";
            this.checkBoxCustomFourColors.UseVisualStyleBackColor = true;
            this.checkBoxCustomFourColors.CheckedChanged += new System.EventHandler(this.CheckBoxCustomFourColorsCheckedChanged);
            // 
            // groupBoxSubtitleImage
            // 
            this.groupBoxSubtitleImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSubtitleImage.Controls.Add(this.labelMinAlpha);
            this.groupBoxSubtitleImage.Controls.Add(this.numericUpDownAutoTransparentAlphaMax);
            this.groupBoxSubtitleImage.Controls.Add(this.groupBoxTransportStream);
            this.groupBoxSubtitleImage.Controls.Add(this.groupBoxImagePalette);
            this.groupBoxSubtitleImage.Controls.Add(this.pictureBoxSubtitleImage);
            this.groupBoxSubtitleImage.Location = new System.Drawing.Point(412, 6);
            this.groupBoxSubtitleImage.Name = "groupBoxSubtitleImage";
            this.groupBoxSubtitleImage.Size = new System.Drawing.Size(665, 191);
            this.groupBoxSubtitleImage.TabIndex = 36;
            this.groupBoxSubtitleImage.TabStop = false;
            this.groupBoxSubtitleImage.Text = "Subtitle image";
            // 
            // labelMinAlpha
            // 
            this.labelMinAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinAlpha.AutoSize = true;
            this.labelMinAlpha.Location = new System.Drawing.Point(351, 171);
            this.labelMinAlpha.Name = "labelMinAlpha";
            this.labelMinAlpha.Size = new System.Drawing.Size(252, 13);
            this.labelMinAlpha.TabIndex = 40;
            this.labelMinAlpha.Text = "Min. alpha value (0=transparent, 255=fully visible)";
            this.labelMinAlpha.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelMinAlpha.Visible = false;
            // 
            // numericUpDownAutoTransparentAlphaMax
            // 
            this.numericUpDownAutoTransparentAlphaMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAutoTransparentAlphaMax.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDownAutoTransparentAlphaMax.BackColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.numericUpDownAutoTransparentAlphaMax.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
            this.numericUpDownAutoTransparentAlphaMax.BorderColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericUpDownAutoTransparentAlphaMax.ButtonForeColor = System.Drawing.SystemColors.ControlText;
            this.numericUpDownAutoTransparentAlphaMax.ButtonForeColorDown = System.Drawing.Color.Orange;
            this.numericUpDownAutoTransparentAlphaMax.ButtonForeColorOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.numericUpDownAutoTransparentAlphaMax.DecimalPlaces = 0;
            this.numericUpDownAutoTransparentAlphaMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownAutoTransparentAlphaMax.Location = new System.Drawing.Point(607, 169);
            this.numericUpDownAutoTransparentAlphaMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownAutoTransparentAlphaMax.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownAutoTransparentAlphaMax.Name = "numericUpDownAutoTransparentAlphaMax";
            this.numericUpDownAutoTransparentAlphaMax.Size = new System.Drawing.Size(44, 21);
            this.numericUpDownAutoTransparentAlphaMax.TabIndex = 37;
            this.numericUpDownAutoTransparentAlphaMax.TabStop = false;
            this.numericUpDownAutoTransparentAlphaMax.ThousandsSeparator = false;
            this.numericUpDownAutoTransparentAlphaMax.Value = new decimal(new int[] {
            140,
            0,
            0,
            0});
            this.numericUpDownAutoTransparentAlphaMax.Visible = false;
            this.numericUpDownAutoTransparentAlphaMax.ValueChanged += new System.EventHandler(this.numericUpDownAutoTransparentAlphaMax_ValueChanged);
            // 
            // groupBoxTransportStream
            // 
            this.groupBoxTransportStream.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTransportStream.Controls.Add(this.checkBoxTransportStreamGetColorAndSplit);
            this.groupBoxTransportStream.Controls.Add(this.checkBoxTransportStreamGrayscale);
            this.groupBoxTransportStream.Location = new System.Drawing.Point(94, 6);
            this.groupBoxTransportStream.Name = "groupBoxTransportStream";
            this.groupBoxTransportStream.Size = new System.Drawing.Size(644, 38);
            this.groupBoxTransportStream.TabIndex = 36;
            this.groupBoxTransportStream.TabStop = false;
            this.groupBoxTransportStream.Text = "Transport stream";
            this.groupBoxTransportStream.Visible = false;
            // 
            // checkBoxTransportStreamGetColorAndSplit
            // 
            this.checkBoxTransportStreamGetColorAndSplit.AutoSize = true;
            this.checkBoxTransportStreamGetColorAndSplit.Location = new System.Drawing.Point(135, 15);
            this.checkBoxTransportStreamGetColorAndSplit.Name = "checkBoxTransportStreamGetColorAndSplit";
            this.checkBoxTransportStreamGetColorAndSplit.Size = new System.Drawing.Size(253, 17);
            this.checkBoxTransportStreamGetColorAndSplit.TabIndex = 1;
            this.checkBoxTransportStreamGetColorAndSplit.Text = "Try get color (will include some splitting of lines)";
            this.checkBoxTransportStreamGetColorAndSplit.UseVisualStyleBackColor = true;
            this.checkBoxTransportStreamGetColorAndSplit.CheckedChanged += new System.EventHandler(this.checkBoxTransportStreamGetColorAndSplit_CheckedChanged);
            // 
            // checkBoxTransportStreamGrayscale
            // 
            this.checkBoxTransportStreamGrayscale.AutoSize = true;
            this.checkBoxTransportStreamGrayscale.Location = new System.Drawing.Point(7, 16);
            this.checkBoxTransportStreamGrayscale.Name = "checkBoxTransportStreamGrayscale";
            this.checkBoxTransportStreamGrayscale.Size = new System.Drawing.Size(73, 17);
            this.checkBoxTransportStreamGrayscale.TabIndex = 0;
            this.checkBoxTransportStreamGrayscale.Text = "Grayscale";
            this.checkBoxTransportStreamGrayscale.UseVisualStyleBackColor = true;
            this.checkBoxTransportStreamGrayscale.CheckedChanged += new System.EventHandler(this.checkBoxTransportStreamGrayscale_CheckedChanged);
            // 
            // pictureBoxSubtitleImage
            // 
            this.pictureBoxSubtitleImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSubtitleImage.BackColor = System.Drawing.Color.DimGray;
            this.pictureBoxSubtitleImage.ContextMenuStrip = this.contextMenuStripImage;
            this.pictureBoxSubtitleImage.Location = new System.Drawing.Point(13, 60);
            this.pictureBoxSubtitleImage.Name = "pictureBoxSubtitleImage";
            this.pictureBoxSubtitleImage.Size = new System.Drawing.Size(644, 127);
            this.pictureBoxSubtitleImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSubtitleImage.TabIndex = 3;
            this.pictureBoxSubtitleImage.TabStop = false;
            // 
            // contextMenuStripImage
            // 
            this.contextMenuStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoTransparentBackgroundToolStripMenuItem,
            this.setItalicAngleToolStripMenuItem,
            this.toolStripMenuItemCaptureTopAlign,
            this.imagePreprocessingToolStripMenuItem1,
            this.toolStripSeparator5,
            this.toolStripMenuItemImageSaveAs,
            this.previewToolStripMenuItem});
            this.contextMenuStripImage.Name = "contextMenuStripUnknownWords";
            this.contextMenuStripImage.Size = new System.Drawing.Size(289, 142);
            this.contextMenuStripImage.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripImage_Opening);
            // 
            // autoTransparentBackgroundToolStripMenuItem
            // 
            this.autoTransparentBackgroundToolStripMenuItem.CheckOnClick = true;
            this.autoTransparentBackgroundToolStripMenuItem.Name = "autoTransparentBackgroundToolStripMenuItem";
            this.autoTransparentBackgroundToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.autoTransparentBackgroundToolStripMenuItem.Text = "Auto transparent background";
            // 
            // setItalicAngleToolStripMenuItem
            // 
            this.setItalicAngleToolStripMenuItem.Name = "setItalicAngleToolStripMenuItem";
            this.setItalicAngleToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.setItalicAngleToolStripMenuItem.Text = "Set italic angle...";
            this.setItalicAngleToolStripMenuItem.Click += new System.EventHandler(this.setItalicAngleToolStripMenuItem_Click);
            // 
            // toolStripMenuItemCaptureTopAlign
            // 
            this.toolStripMenuItemCaptureTopAlign.CheckOnClick = true;
            this.toolStripMenuItemCaptureTopAlign.Name = "toolStripMenuItemCaptureTopAlign";
            this.toolStripMenuItemCaptureTopAlign.Size = new System.Drawing.Size(288, 22);
            this.toolStripMenuItemCaptureTopAlign.Text = "Capture top alignment";
            this.toolStripMenuItemCaptureTopAlign.Click += new System.EventHandler(this.toolStripMenuItemCaptureTopAlign_Click);
            // 
            // imagePreprocessingToolStripMenuItem1
            // 
            this.imagePreprocessingToolStripMenuItem1.Name = "imagePreprocessingToolStripMenuItem1";
            this.imagePreprocessingToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.imagePreprocessingToolStripMenuItem1.Size = new System.Drawing.Size(288, 22);
            this.imagePreprocessingToolStripMenuItem1.Text = "Image preprocessing...";
            this.imagePreprocessingToolStripMenuItem1.Click += new System.EventHandler(this.imagePreprocessingToolStripMenuItem1_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(285, 6);
            // 
            // toolStripMenuItemImageSaveAs
            // 
            this.toolStripMenuItemImageSaveAs.Name = "toolStripMenuItemImageSaveAs";
            this.toolStripMenuItemImageSaveAs.Size = new System.Drawing.Size(288, 22);
            this.toolStripMenuItemImageSaveAs.Text = "Save image as...";
            this.toolStripMenuItemImageSaveAs.Click += new System.EventHandler(this.toolStripMenuItemImageSaveAs_Click);
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.previewToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.previewToolStripMenuItem.Text = "Preview...";
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.previewToolStripMenuItem_Click);
            // 
            // checkBoxShowOnlyForced
            // 
            this.checkBoxShowOnlyForced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowOnlyForced.AutoSize = true;
            this.checkBoxShowOnlyForced.Location = new System.Drawing.Point(369, 313);
            this.checkBoxShowOnlyForced.Name = "checkBoxShowOnlyForced";
            this.checkBoxShowOnlyForced.Size = new System.Drawing.Size(152, 17);
            this.checkBoxShowOnlyForced.TabIndex = 4;
            this.checkBoxShowOnlyForced.Text = "Show only forced subtitles";
            this.checkBoxShowOnlyForced.UseVisualStyleBackColor = true;
            this.checkBoxShowOnlyForced.CheckedChanged += new System.EventHandler(this.checkBoxShowOnlyForced_CheckedChanged);
            // 
            // checkBoxUseTimeCodesFromIdx
            // 
            this.checkBoxUseTimeCodesFromIdx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseTimeCodesFromIdx.AutoSize = true;
            this.checkBoxUseTimeCodesFromIdx.Location = new System.Drawing.Point(335, 295);
            this.checkBoxUseTimeCodesFromIdx.Name = "checkBoxUseTimeCodesFromIdx";
            this.checkBoxUseTimeCodesFromIdx.Size = new System.Drawing.Size(186, 17);
            this.checkBoxUseTimeCodesFromIdx.TabIndex = 3;
            this.checkBoxUseTimeCodesFromIdx.Text = "Use lines/time codes from .idx file";
            this.checkBoxUseTimeCodesFromIdx.UseVisualStyleBackColor = true;
            this.checkBoxUseTimeCodesFromIdx.CheckedChanged += new System.EventHandler(this.checkBoxUseTimeCodesFromIdx_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // splitContainerBottom
            // 
            this.splitContainerBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerBottom.Location = new System.Drawing.Point(15, 199);
            this.splitContainerBottom.Name = "splitContainerBottom";
            // 
            // splitContainerBottom.Panel1
            // 
            this.splitContainerBottom.Panel1.Controls.Add(this.checkBoxShowOnlyForced);
            this.splitContainerBottom.Panel1.Controls.Add(this.textBoxCurrentText);
            this.splitContainerBottom.Panel1.Controls.Add(this.groupBoxOCRControls);
            this.splitContainerBottom.Panel1.Controls.Add(this.checkBoxUseTimeCodesFromIdx);
            this.splitContainerBottom.Panel1.Controls.Add(this.subtitleListView1);
            this.splitContainerBottom.Panel1.Controls.Add(this.labelSubtitleText);
            this.splitContainerBottom.Panel1MinSize = 100;
            // 
            // splitContainerBottom.Panel2
            // 
            this.splitContainerBottom.Panel2.Controls.Add(this.groupBoxOcrAutoFix);
            this.splitContainerBottom.Panel2MinSize = 100;
            this.splitContainerBottom.Size = new System.Drawing.Size(1062, 333);
            this.splitContainerBottom.SplitterDistance = 658;
            this.splitContainerBottom.TabIndex = 39;
            // 
            // textBoxCurrentText
            // 
            this.textBoxCurrentText.AllowDrop = true;
            this.textBoxCurrentText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurrentText.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxCurrentText.ContextMenuStrip = this.contextMenuStripTextBox;
            this.textBoxCurrentText.CurrentLanguage = "";
            this.textBoxCurrentText.CurrentLineIndex = 0;
            this.textBoxCurrentText.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentText.HideSelection = true;
            this.textBoxCurrentText.IsDictionaryDownloaded = true;
            this.textBoxCurrentText.IsSpellCheckerInitialized = false;
            this.textBoxCurrentText.IsSpellCheckRequested = false;
            this.textBoxCurrentText.IsWrongWord = false;
            this.textBoxCurrentText.LanguageChanged = false;
            this.textBoxCurrentText.Location = new System.Drawing.Point(8, 214);
            this.textBoxCurrentText.MaxLength = 32767;
            this.textBoxCurrentText.Multiline = true;
            this.textBoxCurrentText.Name = "textBoxCurrentText";
            this.textBoxCurrentText.Padding = new System.Windows.Forms.Padding(1);
            this.textBoxCurrentText.ReadOnly = false;
            this.textBoxCurrentText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.textBoxCurrentText.SelectedText = "";
            this.textBoxCurrentText.SelectionLength = 0;
            this.textBoxCurrentText.SelectionStart = 0;
            this.textBoxCurrentText.Size = new System.Drawing.Size(354, 77);
            this.textBoxCurrentText.TabIndex = 1;
            this.textBoxCurrentText.TextBoxFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.textBoxCurrentText.UseSystemPasswordChar = false;
            this.textBoxCurrentText.TextChanged += new System.EventHandler(this.TextBoxCurrentTextTextChanged);
            this.textBoxCurrentText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCurrentText_KeyDown);
            // 
            // contextMenuStripTextBox
            // 
            this.contextMenuStripTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator18,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator17,
            this.normalToolStripMenuItem1,
            this.boldToolStripMenuItem1,
            this.italicToolStripMenuItem1,
            this.underlineToolStripMenuItem1});
            this.contextMenuStripTextBox.Name = "contextMenuStripTextBoxListView";
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(173, 214);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.toolStripMenuItem1.Text = "Delete";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(169, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(169, 6);
            // 
            // normalToolStripMenuItem1
            // 
            this.normalToolStripMenuItem1.Name = "normalToolStripMenuItem1";
            this.normalToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.normalToolStripMenuItem1.Text = "Normal";
            this.normalToolStripMenuItem1.Click += new System.EventHandler(this.normalToolStripMenuItem1_Click);
            // 
            // boldToolStripMenuItem1
            // 
            this.boldToolStripMenuItem1.Name = "boldToolStripMenuItem1";
            this.boldToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.boldToolStripMenuItem1.Text = "Bold";
            this.boldToolStripMenuItem1.Click += new System.EventHandler(this.boldToolStripMenuItem1_Click);
            // 
            // italicToolStripMenuItem1
            // 
            this.italicToolStripMenuItem1.Name = "italicToolStripMenuItem1";
            this.italicToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.italicToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.italicToolStripMenuItem1.Text = "Italic";
            this.italicToolStripMenuItem1.Click += new System.EventHandler(this.italicToolStripMenuItem1_Click);
            // 
            // underlineToolStripMenuItem1
            // 
            this.underlineToolStripMenuItem1.Name = "underlineToolStripMenuItem1";
            this.underlineToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.underlineToolStripMenuItem1.Text = "Underline";
            this.underlineToolStripMenuItem1.Click += new System.EventHandler(this.underlineToolStripMenuItem1_Click);
            // 
            // subtitleListView1
            // 
            this.subtitleListView1.AllowColumnReorder = true;
            this.subtitleListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subtitleListView1.ContextMenuStrip = this.contextMenuStripListview;
            this.subtitleListView1.FirstVisibleIndex = -1;
            this.subtitleListView1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subtitleListView1.FullRowSelect = true;
            this.subtitleListView1.GridLines = true;
            this.subtitleListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.subtitleListView1.HideSelection = false;
            this.subtitleListView1.Location = new System.Drawing.Point(8, 21);
            this.subtitleListView1.Name = "subtitleListView1";
            this.subtitleListView1.OwnerDraw = true;
            this.subtitleListView1.Size = new System.Drawing.Size(631, 183);
            this.subtitleListView1.SubtitleFontBold = false;
            this.subtitleListView1.SubtitleFontName = "Tahoma";
            this.subtitleListView1.SubtitleFontSize = 8;
            this.subtitleListView1.TabIndex = 0;
            this.subtitleListView1.UseCompatibleStateImageBehavior = false;
            this.subtitleListView1.UseSyntaxColoring = true;
            this.subtitleListView1.View = System.Windows.Forms.View.Details;
            this.subtitleListView1.SelectedIndexChanged += new System.EventHandler(this.SubtitleListView1SelectedIndexChanged);
            this.subtitleListView1.DoubleClick += new System.EventHandler(this.subtitleListView1_DoubleClick);
            this.subtitleListView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.subtitleListView1_KeyDown);
            // 
            // timerHideStatus
            // 
            this.timerHideStatus.Interval = 2000;
            this.timerHideStatus.Tick += new System.EventHandler(this.timerHideStatus_Tick);
            // 
            // buttonUknownToNames
            // 
            this.buttonUknownToNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUknownToNames.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonUknownToNames.Location = new System.Drawing.Point(152, 7);
            this.buttonUknownToNames.Name = "buttonUknownToNames";
            this.buttonUknownToNames.Size = new System.Drawing.Size(217, 23);
            this.buttonUknownToNames.TabIndex = 0;
            this.buttonUknownToNames.Text = "Add to names etc list";
            this.buttonUknownToNames.UseVisualStyleBackColor = true;
            this.buttonUknownToNames.Click += new System.EventHandler(this.buttonUknownToNames_Click);
            // 
            // VobSubOcr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 582);
            this.Controls.Add(this.splitContainerBottom);
            this.Controls.Add(this.groupBoxSubtitleImage);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(920, 560);
            this.Name = "VobSubOcr";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import/OCR VobSub (sub/idx) subtitle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VobSubOcr_FormClosing);
            this.Shown += new System.EventHandler(this.FormVobSubOcr_Shown);
            this.ResizeEnd += new System.EventHandler(this.VobSubOcr_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VobSubOcr_KeyDown);
            this.Resize += new System.EventHandler(this.VobSubOcr_Resize);
            this.contextMenuStripListview.ResumeLayout(false);
            this.groupBoxOCRControls.ResumeLayout(false);
            this.groupBoxOCRControls.PerformLayout();
            this.groupBoxOcrAutoFix.ResumeLayout(false);
            this.tabControlLogs.ResumeLayout(false);
            this.contextMenuStripAllFixes.ResumeLayout(false);
            this.tabPageUnknownWords.ResumeLayout(false);
            this.contextMenuStripUnknownWords.ResumeLayout(false);
            this.tabPageAllFixes.ResumeLayout(false);
            this.tabPageSuggestions.ResumeLayout(false);
            this.contextMenuStripGuessesUsed.ResumeLayout(false);
            this.groupBoxImagePalette.ResumeLayout(false);
            this.groupBoxImagePalette.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmphasis2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEmphasis1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPattern)).EndInit();
            this.groupBoxSubtitleImage.ResumeLayout(false);
            this.groupBoxSubtitleImage.PerformLayout();
            this.groupBoxTransportStream.ResumeLayout(false);
            this.groupBoxTransportStream.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSubtitleImage)).EndInit();
            this.contextMenuStripImage.ResumeLayout(false);
            this.splitContainerBottom.Panel1.ResumeLayout(false);
            this.splitContainerBottom.Panel1.PerformLayout();
            this.splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
            this.splitContainerBottom.ResumeLayout(false);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSubtitleImage;
        private System.Windows.Forms.Label labelSubtitleText;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private SubtitleListView subtitleListView1;
        private SETextBox textBoxCurrentText;
        private System.Windows.Forms.GroupBox groupBoxOCRControls;
        private System.Windows.Forms.Label labelStartFrom;
        private Nikse.SubtitleEdit.Controls.NikseUpDown numericUpDownStartNumber;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStartOcr;
        private System.Windows.Forms.GroupBox groupBoxOcrAutoFix;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripListview;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem italicToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveImageAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TabControl tabControlLogs;
        private System.Windows.Forms.TabPage tabPageAllFixes;
        private System.Windows.Forms.TabPage tabPageSuggestions;
        private System.Windows.Forms.ListBox listBoxLogSuggestions;
        private System.Windows.Forms.TabPage tabPageUnknownWords;
        private System.Windows.Forms.ListBox listBoxUnknownWords;
        private System.Windows.Forms.GroupBox groupBoxImagePalette;
        private System.Windows.Forms.PictureBox pictureBoxEmphasis2;
        private System.Windows.Forms.PictureBox pictureBoxEmphasis1;
        private System.Windows.Forms.PictureBox pictureBoxPattern;
        private System.Windows.Forms.CheckBox checkBoxCustomFourColors;
        private System.Windows.Forms.GroupBox groupBoxSubtitleImage;
        private System.Windows.Forms.CheckBox checkBoxEmphasis2Transparent;
        private System.Windows.Forms.CheckBox checkBoxEmphasis1Transparent;
        private System.Windows.Forms.CheckBox checkBoxPatternTransparent;
        private System.Windows.Forms.CheckBox checkBoxShowOnlyForced;
        private System.Windows.Forms.CheckBox checkBoxUseTimeCodesFromIdx;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox checkBoxBackgroundTransparent;
        private System.Windows.Forms.PictureBox pictureBoxBackground;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importTextWithMatchingTimeCodesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveAllImagesWithHtmlIndexViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorImageCompare;
        private System.Windows.Forms.ToolStripMenuItem inspectImageCompareMatchesForCurrentImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditLastAdditionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerBottom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button buttonUknownToUserDic;
        private System.Windows.Forms.Button buttonAddToOcrReplaceList;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetUnItalicFactor;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem vobSubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bluraySupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bDNXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUnknownWords;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAllFixes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearFixes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGuessesUsed;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearGuesses;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInspectNOcrMatches;
        private System.Windows.Forms.Timer timerHideStatus;
        private System.Windows.Forms.ToolStripMenuItem dOSTToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxTransportStream;
        private System.Windows.Forms.CheckBox checkBoxTransportStreamGrayscale;
        private System.Windows.Forms.CheckBox checkBoxTransportStreamGetColorAndSplit;
        private Nikse.SubtitleEdit.Controls.NikseUpDown numericUpDownAutoTransparentAlphaMax;
        private System.Windows.Forms.Label labelMinAlpha;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImageSaveAs;
        private System.Windows.Forms.ToolStripMenuItem OcrTrainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importNewTimeCodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ImagePreProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveSubtitleAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCaptureTopAlign;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureTopAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imagePreprocessingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setItalicAngleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoTransparentBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem finalCutProImageToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem boldToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem italicToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem underlineToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem imageWithTimeCodeInFileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oCRSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorOcrSelected;
        private System.Windows.Forms.Button buttonUknownToNames;
    }
}