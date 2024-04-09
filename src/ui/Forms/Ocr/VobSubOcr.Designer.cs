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
            this.toolStripSeparatorOcrSelected = new System.Windows.Forms.ToolStripSeparator();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSaveSubtitleAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveImageAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.labelSubtitleText = new System.Windows.Forms.Label();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStartOcr = new System.Windows.Forms.Button();
            this.groupBoxOcrAutoFix = new System.Windows.Forms.GroupBox();
            this.contextMenuStripAllFixes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClearFixes = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUnknownWords = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripGuessesUsed = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClearGuesses = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxSubtitleImage = new System.Windows.Forms.GroupBox();
            this.labelMinAlpha = new System.Windows.Forms.Label();
            this.numericUpDownAutoTransparentAlphaMax = new Nikse.SubtitleEdit.Controls.NikseUpDown();
            this.pictureBoxSubtitleImage = new System.Windows.Forms.PictureBox();
            this.contextMenuStripImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.autoTransparentBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setItalicAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCaptureTopAlign = new System.Windows.Forms.ToolStripMenuItem();
            this.imagePreprocessingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemImageSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.subtitleListView1 = new Nikse.SubtitleEdit.Controls.SubtitleListView();
            this.contextMenuStripTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.timerHideStatus = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripListview.SuspendLayout();
            this.groupBoxOcrAutoFix.SuspendLayout();
            this.contextMenuStripAllFixes.SuspendLayout();
            this.contextMenuStripUnknownWords.SuspendLayout();
            this.contextMenuStripGuessesUsed.SuspendLayout();
            this.groupBoxSubtitleImage.SuspendLayout();
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
            this.toolStripSeparatorOcrSelected,
            this.normalToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemSaveSubtitleAs,
            this.toolStripSeparator2,
            this.saveImageAsToolStripMenuItem,
            this.saveAllImagesWithHtmlIndexViewToolStripMenuItem,
            this.toolStripSeparatorImageCompare,
            this.toolStripMenuItemInspectNOcrMatches,
            this.inspectImageCompareMatchesForCurrentImageToolStripMenuItem,
            this.EditLastAdditionsToolStripMenuItem,
            this.OcrTrainingToolStripMenuItem,
            this.toolStripSeparator4,
            this.toolStripMenuItemSetUnItalicFactor,
            this.captureTopAlignmentToolStripMenuItem,
            this.ImagePreProcessingToolStripMenuItem,
            this.toolStripSeparator3});
            this.contextMenuStripListview.Name = "contextMenuStripListview";
            this.contextMenuStripListview.Size = new System.Drawing.Size(333, 304);
            this.contextMenuStripListview.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripListviewOpening);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(329, 6);
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
            // labelSubtitleText
            // 
            this.labelSubtitleText.AutoSize = true;
            this.labelSubtitleText.Location = new System.Drawing.Point(7, 5);
            this.labelSubtitleText.Name = "labelSubtitleText";
            this.labelSubtitleText.Size = new System.Drawing.Size(66, 13);
            this.labelSubtitleText.TabIndex = 6;
            this.labelSubtitleText.Text = "Subtitle text";
            // 
            // buttonPause
            // 
            this.buttonPause.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPause.Location = new System.Drawing.Point(11, 70);
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
            this.buttonStartOcr.Location = new System.Drawing.Point(11, 21);
            this.buttonStartOcr.Name = "buttonStartOcr";
            this.buttonStartOcr.Size = new System.Drawing.Size(105, 23);
            this.buttonStartOcr.TabIndex = 0;
            this.buttonStartOcr.Text = "Start OCR";
            this.buttonStartOcr.UseVisualStyleBackColor = true;
            this.buttonStartOcr.Click += new System.EventHandler(this.ButtonStartOcrClick);
            // 
            // groupBoxOcrAutoFix
            // 
            this.groupBoxOcrAutoFix.Controls.Add(this.buttonPause);
            this.groupBoxOcrAutoFix.Controls.Add(this.buttonStartOcr);
            this.groupBoxOcrAutoFix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOcrAutoFix.Location = new System.Drawing.Point(0, 0);
            this.groupBoxOcrAutoFix.Name = "groupBoxOcrAutoFix";
            this.groupBoxOcrAutoFix.Size = new System.Drawing.Size(144, 333);
            this.groupBoxOcrAutoFix.TabIndex = 0;
            this.groupBoxOcrAutoFix.TabStop = false;
            this.groupBoxOcrAutoFix.Text = "OCR Start/stop";
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
            // contextMenuStripUnknownWords
            // 
            this.contextMenuStripUnknownWords.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuStripUnknownWords.Name = "contextMenuStripUnknownWords";
            this.contextMenuStripUnknownWords.Size = new System.Drawing.Size(107, 26);
            this.contextMenuStripUnknownWords.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripUnknownWords_Opening);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
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
            // groupBoxSubtitleImage
            // 
            this.groupBoxSubtitleImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSubtitleImage.Controls.Add(this.labelMinAlpha);
            this.groupBoxSubtitleImage.Controls.Add(this.numericUpDownAutoTransparentAlphaMax);
            this.groupBoxSubtitleImage.Controls.Add(this.pictureBoxSubtitleImage);
            this.groupBoxSubtitleImage.Location = new System.Drawing.Point(12, 6);
            this.groupBoxSubtitleImage.Name = "groupBoxSubtitleImage";
            this.groupBoxSubtitleImage.Size = new System.Drawing.Size(1065, 191);
            this.groupBoxSubtitleImage.TabIndex = 36;
            this.groupBoxSubtitleImage.TabStop = false;
            this.groupBoxSubtitleImage.Text = "Subtitle image";
            // 
            // labelMinAlpha
            // 
            this.labelMinAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinAlpha.AutoSize = true;
            this.labelMinAlpha.Location = new System.Drawing.Point(751, 171);
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
            this.numericUpDownAutoTransparentAlphaMax.Location = new System.Drawing.Point(1007, 169);
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
            // pictureBoxSubtitleImage
            // 
            this.pictureBoxSubtitleImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSubtitleImage.BackColor = System.Drawing.Color.RosyBrown;
            this.pictureBoxSubtitleImage.ContextMenuStrip = this.contextMenuStripImage;
            this.pictureBoxSubtitleImage.Location = new System.Drawing.Point(13, 60);
            this.pictureBoxSubtitleImage.Name = "pictureBoxSubtitleImage";
            this.pictureBoxSubtitleImage.Size = new System.Drawing.Size(1044, 127);
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
            this.splitContainerBottom.Panel1.Controls.Add(this.subtitleListView1);
            this.splitContainerBottom.Panel1.Controls.Add(this.labelSubtitleText);
            this.splitContainerBottom.Panel1MinSize = 100;
            // 
            // splitContainerBottom.Panel2
            // 
            this.splitContainerBottom.Panel2.Controls.Add(this.groupBoxOcrAutoFix);
            this.splitContainerBottom.Panel2MinSize = 100;
            this.splitContainerBottom.Size = new System.Drawing.Size(1062, 333);
            this.splitContainerBottom.SplitterDistance = 914;
            this.splitContainerBottom.TabIndex = 39;
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
            this.subtitleListView1.Size = new System.Drawing.Size(887, 183);
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
            // contextMenuStripTextBox
            // 
            this.contextMenuStripTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator18,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator17});
            this.contextMenuStripTextBox.Name = "contextMenuStripTextBoxListView";
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(173, 126);
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
            // timerHideStatus
            // 
            this.timerHideStatus.Interval = 2000;
            this.timerHideStatus.Tick += new System.EventHandler(this.timerHideStatus_Tick);
            // 
            // VobSubOcr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 582);
            this.Controls.Add(this.splitContainerBottom);
            this.Controls.Add(this.groupBoxSubtitleImage);
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
            this.groupBoxOcrAutoFix.ResumeLayout(false);
            this.contextMenuStripAllFixes.ResumeLayout(false);
            this.contextMenuStripUnknownWords.ResumeLayout(false);
            this.contextMenuStripGuessesUsed.ResumeLayout(false);
            this.groupBoxSubtitleImage.ResumeLayout(false);
            this.groupBoxSubtitleImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSubtitleImage)).EndInit();
            this.contextMenuStripImage.ResumeLayout(false);
            this.splitContainerBottom.Panel1.ResumeLayout(false);
            this.splitContainerBottom.Panel1.PerformLayout();
            this.splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
            this.splitContainerBottom.ResumeLayout(false);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSubtitleImage;
        private System.Windows.Forms.Label labelSubtitleText;
        private SubtitleListView subtitleListView1;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStartOcr;
        private System.Windows.Forms.GroupBox groupBoxOcrAutoFix;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripListview;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveImageAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBoxSubtitleImage;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveAllImagesWithHtmlIndexViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorImageCompare;
        private System.Windows.Forms.ToolStripMenuItem inspectImageCompareMatchesForCurrentImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditLastAdditionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerBottom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetUnItalicFactor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUnknownWords;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAllFixes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearFixes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGuessesUsed;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearGuesses;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInspectNOcrMatches;
        private System.Windows.Forms.Timer timerHideStatus;
        private Nikse.SubtitleEdit.Controls.NikseUpDown numericUpDownAutoTransparentAlphaMax;
        private System.Windows.Forms.Label labelMinAlpha;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImageSaveAs;
        private System.Windows.Forms.ToolStripMenuItem OcrTrainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ImagePreProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveSubtitleAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCaptureTopAlign;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureTopAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imagePreprocessingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setItalicAngleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoTransparentBackgroundToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorOcrSelected;
    }
}