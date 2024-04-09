using Nikse.SubtitleEdit.Controls;

namespace Nikse.SubtitleEdit.Forms
{
    sealed partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                if (_networkSession != null)
                {
                    _networkSession.Dispose();
                    _networkSession = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusNetworking = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenKeepVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.reopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRestoreAutoBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFileFormatProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAssaStyles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.openOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOriginalAstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemOpenContainingFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemVerifyCompleteness = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportImages = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportTimeCodes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportFromVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportBluRaySup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportBluraySupFileForEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportSubIdx = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportDvdSubtitles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportOcrHardSub = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportManualAnsi = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportAdobeEncoreFABImageScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportAvidStl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportAyato = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportPngXml = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportBluraySup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportBdTextSt = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportCapMakerPlus = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportCaptionInc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportCavena890 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportCheetahCap = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportDcinemaInterop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportDcinemaSMPTE2014 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportDost = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportDvdStudioProStl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportEBUSTL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportEdl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportEdlClipName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportFcpIImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportFcpXmlAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportImagePerFrame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportTextTimeCodePair = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportPACScreenElectronics = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportUniPac = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportPlainText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportSpumux = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportTtmlImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportUltech130 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExportVobSubSubIdx = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorExportCustomText = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExportCustomTextFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.showHistoryforUndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemInsertUnicodeCharacter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorInsertUnicodeCharacter = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multipleReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoLineNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShowOriginalInPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRightToLeftMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRtlUnicodeControlChars = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemoveUnicodeControlChars = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemReverseRightToLeftStartEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemModifySelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInverseSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.editSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustDisplayTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemApplyDurationLimits = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSubtitlesBridgeGaps = new System.Windows.Forms.ToolStripMenuItem();
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startNumberingFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTextForHearImpairedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertColorsToDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeCasingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoMergeShortLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMergeDuplicateText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMergeLinesWithSameTimeCodes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoSplitLongLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortBy = new System.Windows.Forms.ToolStripMenuItem();
            this.sortNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortStartTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortEndTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortDisplayTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByGapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortTextAlphabeticallytoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortTextMaxLineLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortTextTotalLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortTextNumberOfLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textCharssecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textWordsPerMinutewpmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorAscOrDesc = new System.Windows.Forms.ToolStripSeparator();
            this.AscendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.descendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.netflixQualityCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemBeautifyTimeCodes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemMakeEmptyFromCurrent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBatchConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.generateDatetimeInfoFromVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMeasurementConverter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendTextVisuallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinSubtitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpellCheckMain = new System.Windows.Forms.ToolStripMenuItem();
            this.spellCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpellCheckFromCurrentLine = new System.Windows.Forms.ToolStripMenuItem();
            this.findDoubleWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FindDoubleLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.GetDictionariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addWordToNameListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.openVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenVideoFromUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenDvd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetAudioTrack = new System.Windows.Forms.ToolStripMenuItem();
            this.closeVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSecondSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setVideoOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smpteTimeModedropFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportChapters = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImportShotChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemListShotChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddWaveformBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.generateBlankVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToTextWhisperTolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoaudioToTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.undockVideoControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redockVideoControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSynchronization = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAdjustAllTimes = new System.Windows.Forms.ToolStripMenuItem();
            this.visualSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPointSync = new System.Windows.Forms.ToolStripMenuItem();
            this.pointSyncViaOtherSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChangeFrameRate2 = new System.Windows.Forms.ToolStripMenuItem();
            this.changeSpeedInPercentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoTranslate = new System.Windows.Forms.ToolStripMenuItem();
            this.translateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autotranslateNLLBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autotranslateViaCopypasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.mergeSentencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breaksplitLongLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNetworking = new System.Windows.Forms.ToolStripMenuItem();
            this.startServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSessionKeyLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSplitterCheckForUpdates = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setStylesForSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setActorForSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAssaTools = new System.Windows.Forms.ToolStripMenuItem();
            this.applyCustomStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoResolutionResamplerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateBackgroundBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorPickerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAssStyles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorAssa = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSetRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWebVttStyle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWebVTT = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuWebVttBrowserPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInsertBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInsertAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInsertSubtitle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopySourceText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.columnDeleteTextOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemColumnDeleteText = new System.Windows.Forms.ToolStripMenuItem();
            this.ShiftTextCellsDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInsertTextFromSub = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemColumnImportText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPasteSpecial = new System.Windows.Forms.ToolStripMenuItem();
            this.copyOriginalTextToCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveTextUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveTextDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemGoToSourceView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.splitLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMergeLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMergeDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4Extend = new System.Windows.Forms.ToolStripSeparator();
            this.extendBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extendAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.removeFormattinglToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllFormattingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBoldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeItalicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeUnderlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFontNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.italicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.underlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFont = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAlignment = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSurroundWithMusicSymbols = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSelectedLines = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeCasingForSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualSyncSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectedLinesEarlierlaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTranslateSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.genericTranslateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUnbreakLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoBreakLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEvenlyDistributeLines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveSelectedLines = new System.Windows.Forms.ToolStripMenuItem();
            this.typeEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.karaokeEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEmptyGoToSourceView = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStripHideVideoControls = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideVideoControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowSubtitleTimer = new System.Windows.Forms.Timer(this.components);
            this.timerAutoDuration = new System.Windows.Forms.Timer(this.components);
            this.timerAutoContinue = new System.Windows.Forms.Timer(this.components);
            this.timerWaveform = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripWaveform = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addParagraphHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParagraphAndPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetParagraphAsSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFocusTextbox = new System.Windows.Forms.ToolStripMenuItem();
            this.goToPreviousSubtitleStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToNextSubtitleStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteParagraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeWithPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeWithNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runWhiperOnParagraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.extendToPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extendToNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemWaveformPlaySelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.showWaveformAndSpectrogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyWaveformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlySpectrogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorGuessTimeCodes = new System.Windows.Forms.ToolStripSeparator();
            this.removeShotChangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addShotChangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guessTimeCodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seekSilenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertSubtitleHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerListViewAndText = new System.Windows.Forms.SplitContainer();
            this.SubtitleListview1 = new Nikse.SubtitleEdit.Controls.SubtitleListView();
            this.imageListBookmarks = new System.Windows.Forms.ImageList(this.components);
            this.groupBoxEdit = new System.Windows.Forms.GroupBox();
            this.contextMenuStripTextBoxListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparatorSpellCheckSuggestions = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSpellCheckSkipOnce = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpellCheckSkipAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpellCheckAddToDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpellCheckAddToNames = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorSpellCheck = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemWebVttVoice = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorWebVTT = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSplitViaWaveform = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSplitTextAtCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.normalToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.boldToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.italicToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.boxToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.underlineToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHorizontalDigits = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBouten = new System.Windows.Forms.ToolStripMenuItem();
            this.boutendotbeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutendotafterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutendotoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenfilledcircleoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenopencircleoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenopendotoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenfilledsesameoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenopensesameoutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenautooutsideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boutenautoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRuby = new System.Windows.Forms.ToolStripMenuItem();
            this.fontNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.superscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorInsertUnicode = new System.Windows.Forms.ToolStripSeparator();
            this.insertUnicodeCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertUnicodeControlCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lefttorightMarkLRMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.righttoleftMarkRLMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfLefttorightOverrideLROToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfRighttoleftOverrideRLOToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.labelOriginalSingleLinePixels = new System.Windows.Forms.Label();
            this.labelOriginalSingleLine = new System.Windows.Forms.Label();
            this.labelTextOriginalLineLengths = new System.Windows.Forms.Label();
            this.labelOriginalText = new Nikse.SubtitleEdit.Controls.NikseLabel();
            this.textBoxListViewTextOriginal = new Nikse.SubtitleEdit.Controls.SETextBox();
            this.labelTextLineTotal = new System.Windows.Forms.Label();
            this.textBoxSource = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.contextMenuStripTextBoxSourceView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemGoToListView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemInsertUnicodeSymbol = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInsertUnicodeControlCharacters = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.righttoleftMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfLefttorightEmbeddingLREToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfLefttorightOverrideLROToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfRighttoleftOverrideRLOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelVideoPlayer = new System.Windows.Forms.Panel();
            this.contextMenuStripEmpty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSSStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerTextUndo = new System.Windows.Forms.Timer(this.components);
            this.timerOriginalTextUndo = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripShowVideoControls = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemShowVideoControls = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripListView.SuspendLayout();
            this.contextMenuStripHideVideoControls.SuspendLayout();
            this.contextMenuStripWaveform.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListViewAndText)).BeginInit();
            this.splitContainerListViewAndText.Panel1.SuspendLayout();
            this.splitContainerListViewAndText.Panel2.SuspendLayout();
            this.splitContainerListViewAndText.SuspendLayout();
            this.groupBoxEdit.SuspendLayout();
            this.contextMenuStripTextBoxListView.SuspendLayout();
            this.contextMenuStripTextBoxSourceView.SuspendLayout();
            this.contextMenuStripEmpty.SuspendLayout();
            this.contextMenuStripShowVideoControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus,
            this.toolStripSelected,
            this.toolStripStatusLabelProgress,
            this.toolStripStatusNetworking});
            this.statusStrip1.Location = new System.Drawing.Point(0, 624);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(975, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 17);
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelStatus.Click += new System.EventHandler(this.LabelStatusClick);
            // 
            // toolStripSelected
            // 
            this.toolStripSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSelected.Name = "toolStripSelected";
            this.toolStripSelected.Size = new System.Drawing.Size(746, 17);
            this.toolStripSelected.Spring = true;
            this.toolStripSelected.Text = "toolStripSelected";
            this.toolStripSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripSelected.Click += new System.EventHandler(this.ToolStripSelectedClick);
            // 
            // toolStripStatusLabelProgress
            // 
            this.toolStripStatusLabelProgress.Name = "toolStripStatusLabelProgress";
            this.toolStripStatusLabelProgress.Size = new System.Drawing.Size(166, 17);
            this.toolStripStatusLabelProgress.Text = "toolStripStatusLabelProgress";
            this.toolStripStatusLabelProgress.Visible = false;
            // 
            // toolStripStatusNetworking
            // 
            this.toolStripStatusNetworking.Image = global::Nikse.SubtitleEdit.Properties.Resources.connect;
            this.toolStripStatusNetworking.Name = "toolStripStatusNetworking";
            this.toolStripStatusNetworking.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.toolStripStatusNetworking.Size = new System.Drawing.Size(214, 17);
            this.toolStripStatusNetworking.Text = "toolStripStatusNetworking";
            this.toolStripStatusNetworking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusNetworking.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripStatusNetworking.Click += new System.EventHandler(this.ToolStripStatusNetworkingClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.toolStripMenuItemSpellCheckMain,
            this.toolStripMenuItemVideo,
            this.toolStripMenuItemSynchronization,
            this.toolStripMenuItemAutoTranslate,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItemNetworking,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(975, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItemOpenKeepVideo,
            this.reopenToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItemRestoreAutoBackup,
            this.toolStripMenuItemFileFormatProperties,
            this.toolStripMenuItemAssaStyles,
            this.toolStripSeparator20,
            this.openOriginalToolStripMenuItem,
            this.saveOriginalToolStripMenuItem,
            this.saveOriginalAstoolStripMenuItem,
            this.removeOriginalToolStripMenuItem,
            this.removeTranslationToolStripMenuItem,
            this.toolStripSeparator12,
            this.toolStripMenuItemOpenContainingFolder,
            this.toolStripMenuItemCompare,
            this.toolStripMenuItemVerifyCompleteness,
            this.toolStripMenuItemStatistics,
            this.toolStripMenuItemPlugins,
            this.toolStripSeparator1,
            this.toolStripMenuItemImport,
            this.toolStripMenuItemExport,
            this.toolStripSeparator10,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.FileToolStripMenuItemDropDownOpening);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItemClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // toolStripMenuItemOpenKeepVideo
            // 
            this.toolStripMenuItemOpenKeepVideo.Name = "toolStripMenuItemOpenKeepVideo";
            this.toolStripMenuItemOpenKeepVideo.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemOpenKeepVideo.Text = "Open (keep video)";
            this.toolStripMenuItemOpenKeepVideo.Click += new System.EventHandler(this.ToolStripMenuItemOpenKeepVideoClick);
            // 
            // reopenToolStripMenuItem
            // 
            this.reopenToolStripMenuItem.Name = "reopenToolStripMenuItem";
            this.reopenToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.reopenToolStripMenuItem.Text = "Reopen";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItemClick);
            // 
            // toolStripMenuItemRestoreAutoBackup
            // 
            this.toolStripMenuItemRestoreAutoBackup.Name = "toolStripMenuItemRestoreAutoBackup";
            this.toolStripMenuItemRestoreAutoBackup.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemRestoreAutoBackup.Text = "Restore auto-backup...";
            this.toolStripMenuItemRestoreAutoBackup.Click += new System.EventHandler(this.ToolStripMenuItemRestoreAutoBackupClick);
            // 
            // toolStripMenuItemFileFormatProperties
            // 
            this.toolStripMenuItemFileFormatProperties.Name = "toolStripMenuItemFileFormatProperties";
            this.toolStripMenuItemFileFormatProperties.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemFileFormatProperties.Text = "<format> properties...";
            this.toolStripMenuItemFileFormatProperties.Click += new System.EventHandler(this.ToolStripMenuItemFileFormatPropertiesClick);
            // 
            // toolStripMenuItemAssaStyles
            // 
            this.toolStripMenuItemAssaStyles.Name = "toolStripMenuItemAssaStyles";
            this.toolStripMenuItemAssaStyles.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemAssaStyles.Text = "Advanced Sub Station Alpha styles...";
            this.toolStripMenuItemAssaStyles.Click += new System.EventHandler(this.ToolStripMenuItemAssaStylesClick);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(266, 6);
            // 
            // openOriginalToolStripMenuItem
            // 
            this.openOriginalToolStripMenuItem.Name = "openOriginalToolStripMenuItem";
            this.openOriginalToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.openOriginalToolStripMenuItem.Text = "Open original (translator mode)...";
            this.openOriginalToolStripMenuItem.Click += new System.EventHandler(this.OpenOriginalToolStripMenuItemClick);
            // 
            // saveOriginalToolStripMenuItem
            // 
            this.saveOriginalToolStripMenuItem.Name = "saveOriginalToolStripMenuItem";
            this.saveOriginalToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveOriginalToolStripMenuItem.Text = "Save original";
            this.saveOriginalToolStripMenuItem.Click += new System.EventHandler(this.SaveOriginalToolStripMenuItemClick);
            // 
            // saveOriginalAstoolStripMenuItem
            // 
            this.saveOriginalAstoolStripMenuItem.Name = "saveOriginalAstoolStripMenuItem";
            this.saveOriginalAstoolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveOriginalAstoolStripMenuItem.Text = "Save original as...";
            this.saveOriginalAstoolStripMenuItem.Click += new System.EventHandler(this.SaveOriginalAstoolStripMenuItemClick);
            // 
            // removeOriginalToolStripMenuItem
            // 
            this.removeOriginalToolStripMenuItem.Name = "removeOriginalToolStripMenuItem";
            this.removeOriginalToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.removeOriginalToolStripMenuItem.Text = "Close original subtitle";
            this.removeOriginalToolStripMenuItem.Click += new System.EventHandler(this.RemoveOriginalToolStripMenuItemClick);
            // 
            // removeTranslationToolStripMenuItem
            // 
            this.removeTranslationToolStripMenuItem.Name = "removeTranslationToolStripMenuItem";
            this.removeTranslationToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.removeTranslationToolStripMenuItem.Text = "Close translated subtitle";
            this.removeTranslationToolStripMenuItem.Click += new System.EventHandler(this.RemoveTranslationToolStripMenuItemClick);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(266, 6);
            // 
            // toolStripMenuItemOpenContainingFolder
            // 
            this.toolStripMenuItemOpenContainingFolder.Name = "toolStripMenuItemOpenContainingFolder";
            this.toolStripMenuItemOpenContainingFolder.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemOpenContainingFolder.Text = "Open containing folder";
            this.toolStripMenuItemOpenContainingFolder.Click += new System.EventHandler(this.ToolStripMenuItemOpenContainingFolderClick);
            // 
            // toolStripMenuItemCompare
            // 
            this.toolStripMenuItemCompare.Name = "toolStripMenuItemCompare";
            this.toolStripMenuItemCompare.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemCompare.Text = "Compare...";
            this.toolStripMenuItemCompare.Click += new System.EventHandler(this.ToolStripMenuItemCompareClick);
            // 
            // toolStripMenuItemVerifyCompleteness
            // 
            this.toolStripMenuItemVerifyCompleteness.Name = "toolStripMenuItemVerifyCompleteness";
            this.toolStripMenuItemVerifyCompleteness.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemVerifyCompleteness.Text = "Verify completeness...";
            this.toolStripMenuItemVerifyCompleteness.Click += new System.EventHandler(this.ToolStripMenuItemVerifyCompletenessClick);
            // 
            // toolStripMenuItemStatistics
            // 
            this.toolStripMenuItemStatistics.Name = "toolStripMenuItemStatistics";
            this.toolStripMenuItemStatistics.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemStatistics.Text = "Statistics...";
            this.toolStripMenuItemStatistics.Click += new System.EventHandler(this.ToolStripMenuItemStatisticsClick);
            // 
            // toolStripMenuItemPlugins
            // 
            this.toolStripMenuItemPlugins.Name = "toolStripMenuItemPlugins";
            this.toolStripMenuItemPlugins.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemPlugins.Text = "Plugins...";
            this.toolStripMenuItemPlugins.Click += new System.EventHandler(this.ToolStripMenuItemPluginsClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(266, 6);
            // 
            // toolStripMenuItemImport
            // 
            this.toolStripMenuItemImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemImportImages,
            this.toolStripMenuItemImportText,
            this.toolStripMenuItemImportTimeCodes,
            this.toolStripMenuItemImportFromVideo,
            this.toolStripMenuItemImportBluRaySup,
            this.toolStripMenuItemImportBluraySupFileForEdit,
            this.toolStripMenuItemImportSubIdx,
            this.toolStripMenuItemImportDvdSubtitles,
            this.toolStripMenuItemImportOcrHardSub,
            this.toolStripMenuItemImportManualAnsi});
            this.toolStripMenuItemImport.Name = "toolStripMenuItemImport";
            this.toolStripMenuItemImport.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemImport.Text = "Import";
            // 
            // toolStripMenuItemImportImages
            // 
            this.toolStripMenuItemImportImages.Name = "toolStripMenuItemImportImages";
            this.toolStripMenuItemImportImages.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportImages.Text = "Images...";
            this.toolStripMenuItemImportImages.Click += new System.EventHandler(this.ToolStripMenuItemImportImagesClick);
            // 
            // toolStripMenuItemImportText
            // 
            this.toolStripMenuItemImportText.Name = "toolStripMenuItemImportText";
            this.toolStripMenuItemImportText.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportText.Text = "Text...";
            this.toolStripMenuItemImportText.Click += new System.EventHandler(this.ToolStripMenuItemImportTextClick);
            // 
            // toolStripMenuItemImportTimeCodes
            // 
            this.toolStripMenuItemImportTimeCodes.Name = "toolStripMenuItemImportTimeCodes";
            this.toolStripMenuItemImportTimeCodes.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportTimeCodes.Text = "Time codes into existing subtitle...";
            this.toolStripMenuItemImportTimeCodes.Click += new System.EventHandler(this.ToolStripMenuItemImportTimeCodesClick);
            // 
            // toolStripMenuItemImportFromVideo
            // 
            this.toolStripMenuItemImportFromVideo.Name = "toolStripMenuItemImportFromVideo";
            this.toolStripMenuItemImportFromVideo.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportFromVideo.Text = "Subtitle from video file...";
            this.toolStripMenuItemImportFromVideo.Click += new System.EventHandler(this.ToolStripMenuItemImportFromVideoClick);
            // 
            // toolStripMenuItemImportBluRaySup
            // 
            this.toolStripMenuItemImportBluRaySup.Name = "toolStripMenuItemImportBluRaySup";
            this.toolStripMenuItemImportBluRaySup.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportBluRaySup.Text = "Blu-ray sup file for OCR...";
            this.toolStripMenuItemImportBluRaySup.Click += new System.EventHandler(this.ToolStripMenuItemImportBluRaySupClick);
            // 
            // toolStripMenuItemImportBluraySupFileForEdit
            // 
            this.toolStripMenuItemImportBluraySupFileForEdit.Name = "toolStripMenuItemImportBluraySupFileForEdit";
            this.toolStripMenuItemImportBluraySupFileForEdit.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportBluraySupFileForEdit.Text = "Blu-ray sup file for edit...";
            this.toolStripMenuItemImportBluraySupFileForEdit.Click += new System.EventHandler(this.ToolStripMenuItemImportBluraySupFileForEditClick);
            // 
            // toolStripMenuItemImportSubIdx
            // 
            this.toolStripMenuItemImportSubIdx.Name = "toolStripMenuItemImportSubIdx";
            this.toolStripMenuItemImportSubIdx.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportSubIdx.Text = "VobSub (sub/idx) subtitle for OCR...";
            this.toolStripMenuItemImportSubIdx.Click += new System.EventHandler(this.ToolStripMenuItemImportSubIdxClick);
            // 
            // toolStripMenuItemImportDvdSubtitles
            // 
            this.toolStripMenuItemImportDvdSubtitles.Name = "toolStripMenuItemImportDvdSubtitles";
            this.toolStripMenuItemImportDvdSubtitles.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportDvdSubtitles.Text = "VOB/IFO (DVD)...";
            this.toolStripMenuItemImportDvdSubtitles.Click += new System.EventHandler(this.ToolStripMenuItemImportDvdSubtitlesClick);
            // 
            // toolStripMenuItemImportOcrHardSub
            // 
            this.toolStripMenuItemImportOcrHardSub.Name = "toolStripMenuItemImportOcrHardSub";
            this.toolStripMenuItemImportOcrHardSub.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportOcrHardSub.Text = "Import/OCR burned-in subtitles from video file...";
            this.toolStripMenuItemImportOcrHardSub.Click += new System.EventHandler(this.ToolStripMenuItemImportOcrHardSubClick);
            // 
            // toolStripMenuItemImportManualAnsi
            // 
            this.toolStripMenuItemImportManualAnsi.Name = "toolStripMenuItemImportManualAnsi";
            this.toolStripMenuItemImportManualAnsi.Size = new System.Drawing.Size(330, 22);
            this.toolStripMenuItemImportManualAnsi.Text = "Subtitle file with manually chosen encoding...";
            this.toolStripMenuItemImportManualAnsi.Click += new System.EventHandler(this.ToolStripMenuItemImportManualAnsiClick);
            // 
            // toolStripMenuItemExport
            // 
            this.toolStripMenuItemExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemExportAdobeEncoreFABImageScript,
            this.toolStripMenuItemExportAvidStl,
            this.toolStripMenuItemExportAyato,
            this.toolStripMenuItemExportPngXml,
            this.toolStripMenuItemExportBluraySup,
            this.toolStripMenuItemExportBdTextSt,
            this.toolStripMenuItemExportCapMakerPlus,
            this.toolStripMenuItemExportCaptionInc,
            this.toolStripMenuItemExportCavena890,
            this.toolStripMenuItemExportCheetahCap,
            this.toolStripMenuItemExportDcinemaInterop,
            this.toolStripMenuItemExportDcinemaSMPTE2014,
            this.toolStripMenuItemExportDost,
            this.toolStripMenuItemExportDvdStudioProStl,
            this.toolStripMenuItemExportEBUSTL,
            this.toolStripMenuItemExportEdl,
            this.toolStripMenuItemExportEdlClipName,
            this.toolStripMenuItemExportFcpIImage,
            this.toolStripMenuItemExportFcpXmlAdvanced,
            this.toolStripMenuItemExportImagePerFrame,
            this.toolStripMenuItemExportTextTimeCodePair,
            this.toolStripMenuItemExportPACScreenElectronics,
            this.toolStripMenuItemExportUniPac,
            this.toolStripMenuItemExportPlainText,
            this.toolStripMenuItemExportSpumux,
            this.toolStripMenuItemExportTtmlImage,
            this.toolStripMenuItemExportUltech130,
            this.toolStripMenuItemExportVobSubSubIdx,
            this.toolStripSeparatorExportCustomText,
            this.toolStripMenuItemExportCustomTextFormat});
            this.toolStripMenuItemExport.Name = "toolStripMenuItemExport";
            this.toolStripMenuItemExport.Size = new System.Drawing.Size(269, 22);
            this.toolStripMenuItemExport.Text = "Export";
            // 
            // toolStripMenuItemExportAdobeEncoreFABImageScript
            // 
            this.toolStripMenuItemExportAdobeEncoreFABImageScript.Name = "toolStripMenuItemExportAdobeEncoreFABImageScript";
            this.toolStripMenuItemExportAdobeEncoreFABImageScript.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportAdobeEncoreFABImageScript.Text = "Adobe Encore FAB image script...";
            this.toolStripMenuItemExportAdobeEncoreFABImageScript.Click += new System.EventHandler(this.ToolStripMenuItemExportAdobeEncoreFABImageScriptClick);
            // 
            // toolStripMenuItemExportAvidStl
            // 
            this.toolStripMenuItemExportAvidStl.Name = "toolStripMenuItemExportAvidStl";
            this.toolStripMenuItemExportAvidStl.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportAvidStl.Text = "Avid STL...";
            this.toolStripMenuItemExportAvidStl.Click += new System.EventHandler(this.ToolStripMenuItemExportAvidStlClick);
            // 
            // toolStripMenuItemExportAyato
            // 
            this.toolStripMenuItemExportAyato.Name = "toolStripMenuItemExportAyato";
            this.toolStripMenuItemExportAyato.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportAyato.Text = "Ayato...";
            this.toolStripMenuItemExportAyato.Click += new System.EventHandler(this.ToolStripMenuItemExportAyatoClick);
            // 
            // toolStripMenuItemExportPngXml
            // 
            this.toolStripMenuItemExportPngXml.Name = "toolStripMenuItemExportPngXml";
            this.toolStripMenuItemExportPngXml.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportPngXml.Text = "BDN xml/png...";
            this.toolStripMenuItemExportPngXml.Click += new System.EventHandler(this.ToolStripMenuItemExportPngXmlClick);
            // 
            // toolStripMenuItemExportBluraySup
            // 
            this.toolStripMenuItemExportBluraySup.Name = "toolStripMenuItemExportBluraySup";
            this.toolStripMenuItemExportBluraySup.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportBluraySup.Text = "Blu-ray sup...";
            this.toolStripMenuItemExportBluraySup.Click += new System.EventHandler(this.ToolStripMenuItemExportBluraySupClick);
            // 
            // toolStripMenuItemExportBdTextSt
            // 
            this.toolStripMenuItemExportBdTextSt.Name = "toolStripMenuItemExportBdTextSt";
            this.toolStripMenuItemExportBdTextSt.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportBdTextSt.Text = "Blu-ray TextST...";
            this.toolStripMenuItemExportBdTextSt.Click += new System.EventHandler(this.ToolStripMenuItemExportBdTextStClick);
            // 
            // toolStripMenuItemExportCapMakerPlus
            // 
            this.toolStripMenuItemExportCapMakerPlus.Name = "toolStripMenuItemExportCapMakerPlus";
            this.toolStripMenuItemExportCapMakerPlus.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportCapMakerPlus.Text = "CapMaker Plus...";
            this.toolStripMenuItemExportCapMakerPlus.Click += new System.EventHandler(this.ToolStripMenuItemExportCapMakerPlusClick);
            // 
            // toolStripMenuItemExportCaptionInc
            // 
            this.toolStripMenuItemExportCaptionInc.Name = "toolStripMenuItemExportCaptionInc";
            this.toolStripMenuItemExportCaptionInc.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportCaptionInc.Text = "Captions Inc...";
            this.toolStripMenuItemExportCaptionInc.Click += new System.EventHandler(this.ToolStripMenuItemExportCaptionIncClick);
            // 
            // toolStripMenuItemExportCavena890
            // 
            this.toolStripMenuItemExportCavena890.Name = "toolStripMenuItemExportCavena890";
            this.toolStripMenuItemExportCavena890.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportCavena890.Text = "Cavena 890...";
            this.toolStripMenuItemExportCavena890.Click += new System.EventHandler(this.ToolStripMenuItemExportCavena890Click);
            // 
            // toolStripMenuItemExportCheetahCap
            // 
            this.toolStripMenuItemExportCheetahCap.Name = "toolStripMenuItemExportCheetahCap";
            this.toolStripMenuItemExportCheetahCap.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportCheetahCap.Text = "Cheetah CAP...";
            this.toolStripMenuItemExportCheetahCap.Click += new System.EventHandler(this.ToolStripMenuItemExportCheetahCapClick);
            // 
            // toolStripMenuItemExportDcinemaInterop
            // 
            this.toolStripMenuItemExportDcinemaInterop.Name = "toolStripMenuItemExportDcinemaInterop";
            this.toolStripMenuItemExportDcinemaInterop.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportDcinemaInterop.Text = "D-Cinema interop/png...";
            this.toolStripMenuItemExportDcinemaInterop.Click += new System.EventHandler(this.ToolStripMenuItemExportDcinemaInteropClick);
            // 
            // toolStripMenuItemExportDcinemaSMPTE2014
            // 
            this.toolStripMenuItemExportDcinemaSMPTE2014.Name = "toolStripMenuItemExportDcinemaSMPTE2014";
            this.toolStripMenuItemExportDcinemaSMPTE2014.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportDcinemaSMPTE2014.Text = "D-Cinema SMPTE 2014/png...";
            this.toolStripMenuItemExportDcinemaSMPTE2014.Click += new System.EventHandler(this.ToolStripMenuItemExportDcinemaSMPTE2014Click);
            // 
            // toolStripMenuItemExportDost
            // 
            this.toolStripMenuItemExportDost.Name = "toolStripMenuItemExportDost";
            this.toolStripMenuItemExportDost.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportDost.Text = "DOST...";
            this.toolStripMenuItemExportDost.Click += new System.EventHandler(this.ToolStripMenuItemExportDostClick);
            // 
            // toolStripMenuItemExportDvdStudioProStl
            // 
            this.toolStripMenuItemExportDvdStudioProStl.Name = "toolStripMenuItemExportDvdStudioProStl";
            this.toolStripMenuItemExportDvdStudioProStl.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportDvdStudioProStl.Text = "DVD Studio Pro STL";
            this.toolStripMenuItemExportDvdStudioProStl.Click += new System.EventHandler(this.ToolStripMenuItemExportDvdStudioProStlClick);
            // 
            // toolStripMenuItemExportEBUSTL
            // 
            this.toolStripMenuItemExportEBUSTL.Name = "toolStripMenuItemExportEBUSTL";
            this.toolStripMenuItemExportEBUSTL.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportEBUSTL.Text = "EBU STL...";
            this.toolStripMenuItemExportEBUSTL.Click += new System.EventHandler(this.ToolStripMenuItemExportEBUSTLClick);
            // 
            // toolStripMenuItemExportEdl
            // 
            this.toolStripMenuItemExportEdl.Name = "toolStripMenuItemExportEdl";
            this.toolStripMenuItemExportEdl.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportEdl.Text = "EDL...";
            this.toolStripMenuItemExportEdl.Click += new System.EventHandler(this.ToolStripMenuItemExportEdlClick);
            // 
            // toolStripMenuItemExportEdlClipName
            // 
            this.toolStripMenuItemExportEdlClipName.Name = "toolStripMenuItemExportEdlClipName";
            this.toolStripMenuItemExportEdlClipName.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportEdlClipName.Text = "EDL/CLIPNAME...";
            this.toolStripMenuItemExportEdlClipName.Click += new System.EventHandler(this.ToolStripMenuItemExportEdlClipNameClick);
            // 
            // toolStripMenuItemExportFcpIImage
            // 
            this.toolStripMenuItemExportFcpIImage.Name = "toolStripMenuItemExportFcpIImage";
            this.toolStripMenuItemExportFcpIImage.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportFcpIImage.Text = "Final Cut Pro + image...";
            this.toolStripMenuItemExportFcpIImage.Click += new System.EventHandler(this.ToolStripMenuItemExportFcpIImageClick);
            // 
            // toolStripMenuItemExportFcpXmlAdvanced
            // 
            this.toolStripMenuItemExportFcpXmlAdvanced.Name = "toolStripMenuItemExportFcpXmlAdvanced";
            this.toolStripMenuItemExportFcpXmlAdvanced.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportFcpXmlAdvanced.Text = "Final Cut Pro XML advanced...";
            this.toolStripMenuItemExportFcpXmlAdvanced.Click += new System.EventHandler(this.ToolStripMenuItemExportFcpXmlAdvancedClick);
            // 
            // toolStripMenuItemExportImagePerFrame
            // 
            this.toolStripMenuItemExportImagePerFrame.Name = "toolStripMenuItemExportImagePerFrame";
            this.toolStripMenuItemExportImagePerFrame.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportImagePerFrame.Text = "Image per frame...";
            this.toolStripMenuItemExportImagePerFrame.Visible = false;
            this.toolStripMenuItemExportImagePerFrame.Click += new System.EventHandler(this.ToolStripMenuItemExportImagePerFrameClick);
            // 
            // toolStripMenuItemExportTextTimeCodePair
            // 
            this.toolStripMenuItemExportTextTimeCodePair.Name = "toolStripMenuItemExportTextTimeCodePair";
            this.toolStripMenuItemExportTextTimeCodePair.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportTextTimeCodePair.Text = "Korean ATS file pair...";
            this.toolStripMenuItemExportTextTimeCodePair.Click += new System.EventHandler(this.ToolStripMenuItemExportTextTimeCodePairClick);
            // 
            // toolStripMenuItemExportPACScreenElectronics
            // 
            this.toolStripMenuItemExportPACScreenElectronics.Name = "toolStripMenuItemExportPACScreenElectronics";
            this.toolStripMenuItemExportPACScreenElectronics.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportPACScreenElectronics.Text = "PAC (Screen Electronics)...";
            this.toolStripMenuItemExportPACScreenElectronics.Click += new System.EventHandler(this.ToolStripMenuItemExportPACScreenElectronicsClick);
            // 
            // toolStripMenuItemExportUniPac
            // 
            this.toolStripMenuItemExportUniPac.Name = "toolStripMenuItemExportUniPac";
            this.toolStripMenuItemExportUniPac.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportUniPac.Text = "PAC Unicode (UniPac)...";
            this.toolStripMenuItemExportUniPac.Click += new System.EventHandler(this.ToolStripMenuItemExportUniPacClick);
            // 
            // toolStripMenuItemExportPlainText
            // 
            this.toolStripMenuItemExportPlainText.Name = "toolStripMenuItemExportPlainText";
            this.toolStripMenuItemExportPlainText.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportPlainText.Text = "Plain text...";
            this.toolStripMenuItemExportPlainText.Click += new System.EventHandler(this.ToolStripMenuItemExportPlainTextClick);
            // 
            // toolStripMenuItemExportSpumux
            // 
            this.toolStripMenuItemExportSpumux.Name = "toolStripMenuItemExportSpumux";
            this.toolStripMenuItemExportSpumux.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportSpumux.Text = "Spumux...";
            this.toolStripMenuItemExportSpumux.Click += new System.EventHandler(this.ToolStripMenuItemExportSpumuxClick);
            // 
            // toolStripMenuItemExportTtmlImage
            // 
            this.toolStripMenuItemExportTtmlImage.Name = "toolStripMenuItemExportTtmlImage";
            this.toolStripMenuItemExportTtmlImage.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportTtmlImage.Text = "TTML/PNG...";
            this.toolStripMenuItemExportTtmlImage.Click += new System.EventHandler(this.ToolStripMenuItemExportTtmlImage_Click);
            // 
            // toolStripMenuItemExportUltech130
            // 
            this.toolStripMenuItemExportUltech130.Name = "toolStripMenuItemExportUltech130";
            this.toolStripMenuItemExportUltech130.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportUltech130.Text = "Ultech caption...";
            this.toolStripMenuItemExportUltech130.Click += new System.EventHandler(this.ToolStripMenuItemExportUltech130Click);
            // 
            // toolStripMenuItemExportVobSubSubIdx
            // 
            this.toolStripMenuItemExportVobSubSubIdx.Name = "toolStripMenuItemExportVobSubSubIdx";
            this.toolStripMenuItemExportVobSubSubIdx.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportVobSubSubIdx.Text = "VobSub (sub/idx)...";
            this.toolStripMenuItemExportVobSubSubIdx.Click += new System.EventHandler(this.ToolStripMenuItemExportVobSubSubIdxClick);
            // 
            // toolStripSeparatorExportCustomText
            // 
            this.toolStripSeparatorExportCustomText.Name = "toolStripSeparatorExportCustomText";
            this.toolStripSeparatorExportCustomText.Size = new System.Drawing.Size(252, 6);
            // 
            // toolStripMenuItemExportCustomTextFormat
            // 
            this.toolStripMenuItemExportCustomTextFormat.Name = "toolStripMenuItemExportCustomTextFormat";
            this.toolStripMenuItemExportCustomTextFormat.Size = new System.Drawing.Size(255, 22);
            this.toolStripMenuItemExportCustomTextFormat.Text = "Export custom text format...";
            this.toolStripMenuItemExportCustomTextFormat.Click += new System.EventHandler(this.ToolStripMenuItemExportExportCustomTextFormatClick);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(266, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemUndo,
            this.toolStripMenuItemRedo,
            this.showHistoryforUndoToolStripMenuItem,
            this.toolStripSeparator14,
            this.toolStripMenuItemInsertUnicodeCharacter,
            this.toolStripSeparatorInsertUnicodeCharacter,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.multipleReplaceToolStripMenuItem,
            this.gotoLineNumberToolStripMenuItem,
            this.toolStripMenuItemShowOriginalInPreview,
            this.toolStripSeparator24,
            this.toolStripMenuItemRightToLeftMode,
            this.toolStripMenuItemRtlUnicodeControlChars,
            this.toolStripMenuItemRemoveUnicodeControlChars,
            this.toolStripMenuItemReverseRightToLeftStartEnd,
            this.toolStripSeparator21,
            this.toolStripMenuItemModifySelection,
            this.toolStripMenuItemInverseSelection,
            this.editSelectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.EditToolStripMenuItemDropDownOpening);
            // 
            // toolStripMenuItemUndo
            // 
            this.toolStripMenuItemUndo.Name = "toolStripMenuItemUndo";
            this.toolStripMenuItemUndo.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemUndo.Text = "Undo";
            this.toolStripMenuItemUndo.Click += new System.EventHandler(this.ToolStripMenuItemUndoClick);
            // 
            // toolStripMenuItemRedo
            // 
            this.toolStripMenuItemRedo.Name = "toolStripMenuItemRedo";
            this.toolStripMenuItemRedo.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemRedo.Text = "Redo";
            this.toolStripMenuItemRedo.Click += new System.EventHandler(this.ToolStripMenuItemRedoClick);
            // 
            // showHistoryforUndoToolStripMenuItem
            // 
            this.showHistoryforUndoToolStripMenuItem.Name = "showHistoryforUndoToolStripMenuItem";
            this.showHistoryforUndoToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.showHistoryforUndoToolStripMenuItem.Text = "Show history (for undo)";
            this.showHistoryforUndoToolStripMenuItem.Click += new System.EventHandler(this.ShowHistoryforUndoToolStripMenuItemClick);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemInsertUnicodeCharacter
            // 
            this.toolStripMenuItemInsertUnicodeCharacter.Name = "toolStripMenuItemInsertUnicodeCharacter";
            this.toolStripMenuItemInsertUnicodeCharacter.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemInsertUnicodeCharacter.Text = "Insert unicode character";
            // 
            // toolStripSeparatorInsertUnicodeCharacter
            // 
            this.toolStripSeparatorInsertUnicodeCharacter.Name = "toolStripSeparatorInsertUnicodeCharacter";
            this.toolStripSeparatorInsertUnicodeCharacter.Size = new System.Drawing.Size(298, 6);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.FindToolStripMenuItemClick);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.findNextToolStripMenuItem.Text = "Find next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.FindNextToolStripMenuItemClick);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.ReplaceToolStripMenuItemClick);
            // 
            // multipleReplaceToolStripMenuItem
            // 
            this.multipleReplaceToolStripMenuItem.Name = "multipleReplaceToolStripMenuItem";
            this.multipleReplaceToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.multipleReplaceToolStripMenuItem.Text = "Multiple replace";
            this.multipleReplaceToolStripMenuItem.Click += new System.EventHandler(this.MultipleReplaceToolStripMenuItemClick);
            // 
            // gotoLineNumberToolStripMenuItem
            // 
            this.gotoLineNumberToolStripMenuItem.Name = "gotoLineNumberToolStripMenuItem";
            this.gotoLineNumberToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoLineNumberToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.gotoLineNumberToolStripMenuItem.Text = "Goto line number...";
            this.gotoLineNumberToolStripMenuItem.Click += new System.EventHandler(this.GotoLineNumberToolStripMenuItemClick);
            // 
            // toolStripMenuItemShowOriginalInPreview
            // 
            this.toolStripMenuItemShowOriginalInPreview.Name = "toolStripMenuItemShowOriginalInPreview";
            this.toolStripMenuItemShowOriginalInPreview.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemShowOriginalInPreview.Text = "Show original text in video/audio previews";
            this.toolStripMenuItemShowOriginalInPreview.Click += new System.EventHandler(this.ToolStripMenuItemShowOriginalInPreviewClick);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemRightToLeftMode
            // 
            this.toolStripMenuItemRightToLeftMode.Name = "toolStripMenuItemRightToLeftMode";
            this.toolStripMenuItemRightToLeftMode.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemRightToLeftMode.Text = "Right to left mode";
            this.toolStripMenuItemRightToLeftMode.Click += new System.EventHandler(this.ToolStripMenuItemRightToLeftModeClick);
            // 
            // toolStripMenuItemRtlUnicodeControlChars
            // 
            this.toolStripMenuItemRtlUnicodeControlChars.Name = "toolStripMenuItemRtlUnicodeControlChars";
            this.toolStripMenuItemRtlUnicodeControlChars.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemRtlUnicodeControlChars.Text = "Fix RTL via Unicode tags";
            this.toolStripMenuItemRtlUnicodeControlChars.Click += new System.EventHandler(this.ToolStripMenuItemRtlUnicodeControlCharsClick);
            // 
            // toolStripMenuItemRemoveUnicodeControlChars
            // 
            this.toolStripMenuItemRemoveUnicodeControlChars.Name = "toolStripMenuItemRemoveUnicodeControlChars";
            this.toolStripMenuItemRemoveUnicodeControlChars.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemRemoveUnicodeControlChars.Text = "Remove Unicode tags";
            this.toolStripMenuItemRemoveUnicodeControlChars.Click += new System.EventHandler(this.ToolStripMenuItemRemoveUnicodeControlCharsClick);
            // 
            // toolStripMenuItemReverseRightToLeftStartEnd
            // 
            this.toolStripMenuItemReverseRightToLeftStartEnd.Name = "toolStripMenuItemReverseRightToLeftStartEnd";
            this.toolStripMenuItemReverseRightToLeftStartEnd.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemReverseRightToLeftStartEnd.Text = "Reverse RTL start/end";
            this.toolStripMenuItemReverseRightToLeftStartEnd.Click += new System.EventHandler(this.ToolStripMenuItemReverseRightToLeftStartEndClick);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemModifySelection
            // 
            this.toolStripMenuItemModifySelection.Name = "toolStripMenuItemModifySelection";
            this.toolStripMenuItemModifySelection.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemModifySelection.Text = "Create/modify selection...";
            this.toolStripMenuItemModifySelection.Click += new System.EventHandler(this.ToolStripMenuItemModifySelectionClick);
            // 
            // toolStripMenuItemInverseSelection
            // 
            this.toolStripMenuItemInverseSelection.Name = "toolStripMenuItemInverseSelection";
            this.toolStripMenuItemInverseSelection.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
            this.toolStripMenuItemInverseSelection.Size = new System.Drawing.Size(301, 22);
            this.toolStripMenuItemInverseSelection.Text = "Inverse selection";
            this.toolStripMenuItemInverseSelection.Click += new System.EventHandler(this.ToolStripMenuItemInverseSelectionClick);
            // 
            // editSelectAllToolStripMenuItem
            // 
            this.editSelectAllToolStripMenuItem.Name = "editSelectAllToolStripMenuItem";
            this.editSelectAllToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.editSelectAllToolStripMenuItem.Text = "Select all";
            this.editSelectAllToolStripMenuItem.Click += new System.EventHandler(this.EditSelectAllToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adjustDisplayTimeToolStripMenuItem,
            this.toolStripMenuItemApplyDurationLimits,
            this.toolStripMenuItemSubtitlesBridgeGaps,
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem,
            this.fixToolStripMenuItem,
            this.startNumberingFromToolStripMenuItem,
            this.removeTextForHearImpairedToolStripMenuItem,
            this.convertColorsToDialogToolStripMenuItem,
            this.ChangeCasingToolStripMenuItem,
            this.toolStripMenuItemAutoMergeShortLines,
            this.toolStripMenuItemMergeDuplicateText,
            this.toolStripMenuItemMergeLinesWithSameTimeCodes,
            this.toolStripMenuItemAutoSplitLongLines,
            this.toolStripMenuItemSortBy,
            this.listErrorsToolStripMenuItem,
            this.netflixQualityCheckToolStripMenuItem,
            this.toolStripSeparator27,
            this.toolStripMenuItemBeautifyTimeCodes,
            this.toolStripSeparator22,
            this.toolStripMenuItemMakeEmptyFromCurrent,
            this.toolStripMenuItemBatchConvert,
            this.generateDatetimeInfoFromVideoToolStripMenuItem,
            this.toolStripMenuItemMeasurementConverter,
            this.toolStripSeparator3,
            this.splitToolStripMenuItem,
            this.appendTextVisuallyToolStripMenuItem,
            this.joinSubtitlesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.ToolsToolStripMenuItemDropDownOpening);
            // 
            // adjustDisplayTimeToolStripMenuItem
            // 
            this.adjustDisplayTimeToolStripMenuItem.Name = "adjustDisplayTimeToolStripMenuItem";
            this.adjustDisplayTimeToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.adjustDisplayTimeToolStripMenuItem.Text = "Adjust display time...";
            this.adjustDisplayTimeToolStripMenuItem.Click += new System.EventHandler(this.AdjustDisplayTimeToolStripMenuItemClick);
            // 
            // toolStripMenuItemApplyDurationLimits
            // 
            this.toolStripMenuItemApplyDurationLimits.Name = "toolStripMenuItemApplyDurationLimits";
            this.toolStripMenuItemApplyDurationLimits.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemApplyDurationLimits.Text = "Apply duration limits...";
            this.toolStripMenuItemApplyDurationLimits.Click += new System.EventHandler(this.ToolStripMenuItemApplyDisplayTimeLimitsClick);
            // 
            // toolStripMenuItemSubtitlesBridgeGaps
            // 
            this.toolStripMenuItemSubtitlesBridgeGaps.Name = "toolStripMenuItemSubtitlesBridgeGaps";
            this.toolStripMenuItemSubtitlesBridgeGaps.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemSubtitlesBridgeGaps.Text = "Bridge gap between subtitles...";
            this.toolStripMenuItemSubtitlesBridgeGaps.Click += new System.EventHandler(this.ToolStripMenuItemBridgeGapsBetweenSubtitlesClick);
            // 
            // setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem
            // 
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem.Name = "setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem";
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem.Text = "Apply mininum gap between subtitles...";
            this.setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem.Click += new System.EventHandler(this.SetMinimalDisplayTimeDifferenceToolStripMenuItemClick);
            // 
            // fixToolStripMenuItem
            // 
            this.fixToolStripMenuItem.Name = "fixToolStripMenuItem";
            this.fixToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.fixToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.fixToolStripMenuItem.Text = "Fix common errors...";
            this.fixToolStripMenuItem.Click += new System.EventHandler(this.FixToolStripMenuItemClick);
            // 
            // startNumberingFromToolStripMenuItem
            // 
            this.startNumberingFromToolStripMenuItem.Name = "startNumberingFromToolStripMenuItem";
            this.startNumberingFromToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.startNumberingFromToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.startNumberingFromToolStripMenuItem.Text = "Start numbering from...";
            this.startNumberingFromToolStripMenuItem.Click += new System.EventHandler(this.StartNumberingFromToolStripMenuItemClick);
            // 
            // removeTextForHearImpairedToolStripMenuItem
            // 
            this.removeTextForHearImpairedToolStripMenuItem.Name = "removeTextForHearImpairedToolStripMenuItem";
            this.removeTextForHearImpairedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.H)));
            this.removeTextForHearImpairedToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.removeTextForHearImpairedToolStripMenuItem.Text = "Remove text for hearing impaired...";
            this.removeTextForHearImpairedToolStripMenuItem.Click += new System.EventHandler(this.RemoveTextForHearImpairedToolStripMenuItemClick);
            // 
            // convertColorsToDialogToolStripMenuItem
            // 
            this.convertColorsToDialogToolStripMenuItem.Name = "convertColorsToDialogToolStripMenuItem";
            this.convertColorsToDialogToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.convertColorsToDialogToolStripMenuItem.Text = "Convert colors to dialog...";
            this.convertColorsToDialogToolStripMenuItem.Click += new System.EventHandler(this.ConvertColorsToDialogToolStripMenuItemClick);
            // 
            // ChangeCasingToolStripMenuItem
            // 
            this.ChangeCasingToolStripMenuItem.Name = "ChangeCasingToolStripMenuItem";
            this.ChangeCasingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.ChangeCasingToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.ChangeCasingToolStripMenuItem.Text = "Change casing...";
            this.ChangeCasingToolStripMenuItem.Click += new System.EventHandler(this.ChangeCasingToolStripMenuItem_Click);
            // 
            // toolStripMenuItemAutoMergeShortLines
            // 
            this.toolStripMenuItemAutoMergeShortLines.Name = "toolStripMenuItemAutoMergeShortLines";
            this.toolStripMenuItemAutoMergeShortLines.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemAutoMergeShortLines.Text = "Merge short lines...";
            this.toolStripMenuItemAutoMergeShortLines.Click += new System.EventHandler(this.ToolStripMenuItemAutoMergeShortLinesClick);
            // 
            // toolStripMenuItemMergeDuplicateText
            // 
            this.toolStripMenuItemMergeDuplicateText.Name = "toolStripMenuItemMergeDuplicateText";
            this.toolStripMenuItemMergeDuplicateText.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemMergeDuplicateText.Text = "Merge lines with same text...";
            this.toolStripMenuItemMergeDuplicateText.Click += new System.EventHandler(this.ToolStripMenuItemMergeDuplicateTextClick);
            // 
            // toolStripMenuItemMergeLinesWithSameTimeCodes
            // 
            this.toolStripMenuItemMergeLinesWithSameTimeCodes.Name = "toolStripMenuItemMergeLinesWithSameTimeCodes";
            this.toolStripMenuItemMergeLinesWithSameTimeCodes.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemMergeLinesWithSameTimeCodes.Text = "Merge lines with same time codes...";
            this.toolStripMenuItemMergeLinesWithSameTimeCodes.Click += new System.EventHandler(this.ToolStripMenuItemMergeLinesWithSameTimeCodesClick);
            // 
            // toolStripMenuItemAutoSplitLongLines
            // 
            this.toolStripMenuItemAutoSplitLongLines.Name = "toolStripMenuItemAutoSplitLongLines";
            this.toolStripMenuItemAutoSplitLongLines.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemAutoSplitLongLines.Text = "Break/split long lines...";
            this.toolStripMenuItemAutoSplitLongLines.Click += new System.EventHandler(this.ToolStripMenuItemAutoSplitLongLinesClick);
            // 
            // toolStripMenuItemSortBy
            // 
            this.toolStripMenuItemSortBy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortNumberToolStripMenuItem,
            this.sortStartTimeToolStripMenuItem,
            this.sortEndTimeToolStripMenuItem,
            this.sortDisplayTimeToolStripMenuItem,
            this.sortByGapToolStripMenuItem,
            this.sortTextAlphabeticallytoolStripMenuItem,
            this.sortTextMaxLineLengthToolStripMenuItem,
            this.sortTextTotalLengthToolStripMenuItem,
            this.sortTextNumberOfLinesToolStripMenuItem,
            this.textCharssecToolStripMenuItem,
            this.textWordsPerMinutewpmToolStripMenuItem,
            this.actorToolStripMenuItem,
            this.styleToolStripMenuItem,
            this.toolStripSeparatorAscOrDesc,
            this.AscendingToolStripMenuItem,
            this.descendingToolStripMenuItem});
            this.toolStripMenuItemSortBy.Name = "toolStripMenuItemSortBy";
            this.toolStripMenuItemSortBy.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemSortBy.Text = "Sort by";
            // 
            // sortNumberToolStripMenuItem
            // 
            this.sortNumberToolStripMenuItem.Name = "sortNumberToolStripMenuItem";
            this.sortNumberToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortNumberToolStripMenuItem.Text = "Number";
            this.sortNumberToolStripMenuItem.Click += new System.EventHandler(this.SortNumberToolStripMenuItemClick);
            // 
            // sortStartTimeToolStripMenuItem
            // 
            this.sortStartTimeToolStripMenuItem.Name = "sortStartTimeToolStripMenuItem";
            this.sortStartTimeToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortStartTimeToolStripMenuItem.Text = "Start time";
            this.sortStartTimeToolStripMenuItem.Click += new System.EventHandler(this.SortStartTimeToolStripMenuItemClick);
            // 
            // sortEndTimeToolStripMenuItem
            // 
            this.sortEndTimeToolStripMenuItem.Name = "sortEndTimeToolStripMenuItem";
            this.sortEndTimeToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortEndTimeToolStripMenuItem.Text = "End time";
            this.sortEndTimeToolStripMenuItem.Click += new System.EventHandler(this.SortEndTimeToolStripMenuItemClick);
            // 
            // sortDisplayTimeToolStripMenuItem
            // 
            this.sortDisplayTimeToolStripMenuItem.Name = "sortDisplayTimeToolStripMenuItem";
            this.sortDisplayTimeToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortDisplayTimeToolStripMenuItem.Text = "Duration";
            this.sortDisplayTimeToolStripMenuItem.Click += new System.EventHandler(this.SortDisplayTimeToolStripMenuItemClick);
            // 
            // sortByGapToolStripMenuItem
            // 
            this.sortByGapToolStripMenuItem.Name = "sortByGapToolStripMenuItem";
            this.sortByGapToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortByGapToolStripMenuItem.Text = "Gap";
            this.sortByGapToolStripMenuItem.Click += new System.EventHandler(this.SortByGapToolStripMenuItemClick);
            // 
            // sortTextAlphabeticallytoolStripMenuItem
            // 
            this.sortTextAlphabeticallytoolStripMenuItem.Name = "sortTextAlphabeticallytoolStripMenuItem";
            this.sortTextAlphabeticallytoolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortTextAlphabeticallytoolStripMenuItem.Text = "Text - alphabetically";
            this.sortTextAlphabeticallytoolStripMenuItem.Click += new System.EventHandler(this.SortTextAlphabeticallytoolStripMenuItemClick);
            // 
            // sortTextMaxLineLengthToolStripMenuItem
            // 
            this.sortTextMaxLineLengthToolStripMenuItem.Name = "sortTextMaxLineLengthToolStripMenuItem";
            this.sortTextMaxLineLengthToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortTextMaxLineLengthToolStripMenuItem.Text = "Text - single line max. length";
            this.sortTextMaxLineLengthToolStripMenuItem.Click += new System.EventHandler(this.SortTextMaxLineLengthToolStripMenuItemClick);
            // 
            // sortTextTotalLengthToolStripMenuItem
            // 
            this.sortTextTotalLengthToolStripMenuItem.Name = "sortTextTotalLengthToolStripMenuItem";
            this.sortTextTotalLengthToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortTextTotalLengthToolStripMenuItem.Text = "Text - total length";
            this.sortTextTotalLengthToolStripMenuItem.Click += new System.EventHandler(this.SortTextTotalLengthToolStripMenuItemClick);
            // 
            // sortTextNumberOfLinesToolStripMenuItem
            // 
            this.sortTextNumberOfLinesToolStripMenuItem.Name = "sortTextNumberOfLinesToolStripMenuItem";
            this.sortTextNumberOfLinesToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.sortTextNumberOfLinesToolStripMenuItem.Text = "Text - number of lines";
            this.sortTextNumberOfLinesToolStripMenuItem.Click += new System.EventHandler(this.SortTextNumberOfLinesToolStripMenuItemClick);
            // 
            // textCharssecToolStripMenuItem
            // 
            this.textCharssecToolStripMenuItem.Name = "textCharssecToolStripMenuItem";
            this.textCharssecToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.textCharssecToolStripMenuItem.Text = "Text - chars/sec";
            this.textCharssecToolStripMenuItem.Click += new System.EventHandler(this.TextCharsSecToolStripMenuItemClick);
            // 
            // textWordsPerMinutewpmToolStripMenuItem
            // 
            this.textWordsPerMinutewpmToolStripMenuItem.Name = "textWordsPerMinutewpmToolStripMenuItem";
            this.textWordsPerMinutewpmToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.textWordsPerMinutewpmToolStripMenuItem.Text = "Text - words per minute (wpm)";
            this.textWordsPerMinutewpmToolStripMenuItem.Click += new System.EventHandler(this.TextWordsPerMinutewpmToolStripMenuItemClick);
            // 
            // actorToolStripMenuItem
            // 
            this.actorToolStripMenuItem.Name = "actorToolStripMenuItem";
            this.actorToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.actorToolStripMenuItem.Text = "Actor";
            this.actorToolStripMenuItem.Click += new System.EventHandler(this.ActorToolStripMenuItemClick);
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.styleToolStripMenuItem.Text = "Style";
            this.styleToolStripMenuItem.Click += new System.EventHandler(this.StyleToolStripMenuItemClick);
            // 
            // toolStripSeparatorAscOrDesc
            // 
            this.toolStripSeparatorAscOrDesc.Name = "toolStripSeparatorAscOrDesc";
            this.toolStripSeparatorAscOrDesc.Size = new System.Drawing.Size(237, 6);
            // 
            // AscendingToolStripMenuItem
            // 
            this.AscendingToolStripMenuItem.Checked = true;
            this.AscendingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AscendingToolStripMenuItem.Name = "AscendingToolStripMenuItem";
            this.AscendingToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.AscendingToolStripMenuItem.Text = "Ascending";
            this.AscendingToolStripMenuItem.Click += new System.EventHandler(this.AscendingToolStripMenuItem_Click);
            // 
            // descendingToolStripMenuItem
            // 
            this.descendingToolStripMenuItem.Name = "descendingToolStripMenuItem";
            this.descendingToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.descendingToolStripMenuItem.Text = "Descending";
            this.descendingToolStripMenuItem.Click += new System.EventHandler(this.DescendingToolStripMenuItemClick);
            // 
            // listErrorsToolStripMenuItem
            // 
            this.listErrorsToolStripMenuItem.Name = "listErrorsToolStripMenuItem";
            this.listErrorsToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.listErrorsToolStripMenuItem.Text = "List errors...";
            this.listErrorsToolStripMenuItem.Click += new System.EventHandler(this.ListErrorsToolStripMenuItemClick);
            // 
            // netflixQualityCheckToolStripMenuItem
            // 
            this.netflixQualityCheckToolStripMenuItem.Name = "netflixQualityCheckToolStripMenuItem";
            this.netflixQualityCheckToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.netflixQualityCheckToolStripMenuItem.Text = "Netflix quality check";
            this.netflixQualityCheckToolStripMenuItem.Click += new System.EventHandler(this.NetflixGlyphCheckToolStripMenuItem_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(335, 6);
            // 
            // toolStripMenuItemBeautifyTimeCodes
            // 
            this.toolStripMenuItemBeautifyTimeCodes.Name = "toolStripMenuItemBeautifyTimeCodes";
            this.toolStripMenuItemBeautifyTimeCodes.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemBeautifyTimeCodes.Text = "Beautify time codes...";
            this.toolStripMenuItemBeautifyTimeCodes.Click += new System.EventHandler(this.ToolStripMenuItemBeautifyTimeCodesClick);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(335, 6);
            // 
            // toolStripMenuItemMakeEmptyFromCurrent
            // 
            this.toolStripMenuItemMakeEmptyFromCurrent.Name = "toolStripMenuItemMakeEmptyFromCurrent";
            this.toolStripMenuItemMakeEmptyFromCurrent.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemMakeEmptyFromCurrent.Text = "Make new empty translation from current subtitle";
            this.toolStripMenuItemMakeEmptyFromCurrent.Click += new System.EventHandler(this.ToolStripMenuItemMakeEmptyFromCurrentClick);
            // 
            // toolStripMenuItemBatchConvert
            // 
            this.toolStripMenuItemBatchConvert.Name = "toolStripMenuItemBatchConvert";
            this.toolStripMenuItemBatchConvert.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemBatchConvert.Text = "Batch convert...";
            this.toolStripMenuItemBatchConvert.Click += new System.EventHandler(this.ToolStripMenuItemBatchConvertClick);
            // 
            // generateDatetimeInfoFromVideoToolStripMenuItem
            // 
            this.generateDatetimeInfoFromVideoToolStripMenuItem.Name = "generateDatetimeInfoFromVideoToolStripMenuItem";
            this.generateDatetimeInfoFromVideoToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.generateDatetimeInfoFromVideoToolStripMenuItem.Text = "Generate date/time info from video...";
            this.generateDatetimeInfoFromVideoToolStripMenuItem.Click += new System.EventHandler(this.GenerateDatetimeInfoFromVideoToolStripMenuItemClick);
            // 
            // toolStripMenuItemMeasurementConverter
            // 
            this.toolStripMenuItemMeasurementConverter.Name = "toolStripMenuItemMeasurementConverter";
            this.toolStripMenuItemMeasurementConverter.Size = new System.Drawing.Size(338, 22);
            this.toolStripMenuItemMeasurementConverter.Text = "Measurement converter...";
            this.toolStripMenuItemMeasurementConverter.Click += new System.EventHandler(this.ToolStripMenuItemMeasurementConverterClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(335, 6);
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            this.splitToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.splitToolStripMenuItem.Text = "Split subtitle...";
            this.splitToolStripMenuItem.Click += new System.EventHandler(this.SplitToolStripMenuItemClick);
            // 
            // appendTextVisuallyToolStripMenuItem
            // 
            this.appendTextVisuallyToolStripMenuItem.Name = "appendTextVisuallyToolStripMenuItem";
            this.appendTextVisuallyToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.appendTextVisuallyToolStripMenuItem.Text = "Append subtitle...";
            this.appendTextVisuallyToolStripMenuItem.Click += new System.EventHandler(this.AppendTextVisuallyToolStripMenuItemClick);
            // 
            // joinSubtitlesToolStripMenuItem
            // 
            this.joinSubtitlesToolStripMenuItem.Name = "joinSubtitlesToolStripMenuItem";
            this.joinSubtitlesToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.joinSubtitlesToolStripMenuItem.Text = "Join subtitles...";
            this.joinSubtitlesToolStripMenuItem.Click += new System.EventHandler(this.JoinSubtitlesToolStripMenuItemClick);
            // 
            // toolStripMenuItemSpellCheckMain
            // 
            this.toolStripMenuItemSpellCheckMain.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spellCheckToolStripMenuItem,
            this.toolStripMenuItemSpellCheckFromCurrentLine,
            this.findDoubleWordsToolStripMenuItem,
            this.FindDoubleLinesToolStripMenuItem,
            this.toolStripSeparator9,
            this.GetDictionariesToolStripMenuItem,
            this.addWordToNameListToolStripMenuItem});
            this.toolStripMenuItemSpellCheckMain.Name = "toolStripMenuItemSpellCheckMain";
            this.toolStripMenuItemSpellCheckMain.Size = new System.Drawing.Size(82, 20);
            this.toolStripMenuItemSpellCheckMain.Text = "Spell check";
            this.toolStripMenuItemSpellCheckMain.DropDownOpening += new System.EventHandler(this.ToolStripMenuItemSpellCheckMainDropDownOpening);
            // 
            // spellCheckToolStripMenuItem
            // 
            this.spellCheckToolStripMenuItem.Name = "spellCheckToolStripMenuItem";
            this.spellCheckToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.spellCheckToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.spellCheckToolStripMenuItem.Text = "Spell check...";
            this.spellCheckToolStripMenuItem.Click += new System.EventHandler(this.SpellCheckToolStripMenuItemClick);
            // 
            // toolStripMenuItemSpellCheckFromCurrentLine
            // 
            this.toolStripMenuItemSpellCheckFromCurrentLine.Name = "toolStripMenuItemSpellCheckFromCurrentLine";
            this.toolStripMenuItemSpellCheckFromCurrentLine.Size = new System.Drawing.Size(267, 22);
            this.toolStripMenuItemSpellCheckFromCurrentLine.Text = "Spell check from current line...";
            this.toolStripMenuItemSpellCheckFromCurrentLine.Click += new System.EventHandler(this.ToolStripMenuItemSpellCheckFromCurrentLineClick);
            // 
            // findDoubleWordsToolStripMenuItem
            // 
            this.findDoubleWordsToolStripMenuItem.Name = "findDoubleWordsToolStripMenuItem";
            this.findDoubleWordsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.findDoubleWordsToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.findDoubleWordsToolStripMenuItem.Text = "Find double words";
            this.findDoubleWordsToolStripMenuItem.Click += new System.EventHandler(this.FindDoubleWordsToolStripMenuItemClick);
            // 
            // FindDoubleLinesToolStripMenuItem
            // 
            this.FindDoubleLinesToolStripMenuItem.Name = "FindDoubleLinesToolStripMenuItem";
            this.FindDoubleLinesToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.FindDoubleLinesToolStripMenuItem.Text = "Find double lines";
            this.FindDoubleLinesToolStripMenuItem.Click += new System.EventHandler(this.FindDoubleLinesToolStripMenuItemClick);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(264, 6);
            // 
            // GetDictionariesToolStripMenuItem
            // 
            this.GetDictionariesToolStripMenuItem.Name = "GetDictionariesToolStripMenuItem";
            this.GetDictionariesToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.GetDictionariesToolStripMenuItem.Text = "Get dictionary...";
            this.GetDictionariesToolStripMenuItem.Click += new System.EventHandler(this.GetDictionariesToolStripMenuItem_Click);
            // 
            // addWordToNameListToolStripMenuItem
            // 
            this.addWordToNameListToolStripMenuItem.Name = "addWordToNameListToolStripMenuItem";
            this.addWordToNameListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.addWordToNameListToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.addWordToNameListToolStripMenuItem.Text = "Add word to names list";
            this.addWordToNameListToolStripMenuItem.Click += new System.EventHandler(this.AddWordToNameListToolStripMenuItemClick);
            // 
            // toolStripMenuItemVideo
            // 
            this.toolStripMenuItemVideo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openVideoToolStripMenuItem,
            this.toolStripMenuItemOpenVideoFromUrl,
            this.toolStripMenuItemOpenDvd,
            this.toolStripMenuItemSetAudioTrack,
            this.closeVideoToolStripMenuItem,
            this.openSecondSubtitleToolStripMenuItem,
            this.setVideoOffsetToolStripMenuItem,
            this.smpteTimeModedropFrameToolStripMenuItem,
            this.toolStripMenuItemImportChapters,
            this.toolStripMenuItemImportShotChanges,
            this.toolStripMenuItemListShotChanges,
            this.toolStripMenuItemAddWaveformBatch,
            this.generateBlankVideoToolStripMenuItem,
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem,
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem,
            this.audioToTextWhisperTolStripMenuItem,
            this.videoaudioToTextToolStripMenuItem,
            this.toolStripSeparator5,
            this.undockVideoControlsToolStripMenuItem,
            this.redockVideoControlsToolStripMenuItem});
            this.toolStripMenuItemVideo.Name = "toolStripMenuItemVideo";
            this.toolStripMenuItemVideo.Size = new System.Drawing.Size(50, 20);
            this.toolStripMenuItemVideo.Text = "Video";
            this.toolStripMenuItemVideo.DropDownClosed += new System.EventHandler(this.ToolStripMenuItemVideoDropDownClosed);
            this.toolStripMenuItemVideo.DropDownOpening += new System.EventHandler(this.ToolStripMenuItemVideoDropDownOpening);
            // 
            // openVideoToolStripMenuItem
            // 
            this.openVideoToolStripMenuItem.Name = "openVideoToolStripMenuItem";
            this.openVideoToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.openVideoToolStripMenuItem.Text = "Open video...";
            this.openVideoToolStripMenuItem.Click += new System.EventHandler(this.ButtonOpenVideoClick);
            // 
            // toolStripMenuItemOpenVideoFromUrl
            // 
            this.toolStripMenuItemOpenVideoFromUrl.Name = "toolStripMenuItemOpenVideoFromUrl";
            this.toolStripMenuItemOpenVideoFromUrl.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemOpenVideoFromUrl.Text = "Open video from url...";
            this.toolStripMenuItemOpenVideoFromUrl.Click += new System.EventHandler(this.ToolStripMenuItemOpenVideoFromUrlClick);
            // 
            // toolStripMenuItemOpenDvd
            // 
            this.toolStripMenuItemOpenDvd.Name = "toolStripMenuItemOpenDvd";
            this.toolStripMenuItemOpenDvd.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemOpenDvd.Text = "Open DVD...";
            this.toolStripMenuItemOpenDvd.Click += new System.EventHandler(this.ToolStripMenuItemOpenDvdClick);
            // 
            // toolStripMenuItemSetAudioTrack
            // 
            this.toolStripMenuItemSetAudioTrack.Name = "toolStripMenuItemSetAudioTrack";
            this.toolStripMenuItemSetAudioTrack.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSetAudioTrack.Text = "Choose audio track";
            // 
            // closeVideoToolStripMenuItem
            // 
            this.closeVideoToolStripMenuItem.Name = "closeVideoToolStripMenuItem";
            this.closeVideoToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.closeVideoToolStripMenuItem.Text = "Close video";
            this.closeVideoToolStripMenuItem.Click += new System.EventHandler(this.CloseVideoToolStripMenuItemClick);
            // 
            // openSecondSubtitleToolStripMenuItem
            // 
            this.openSecondSubtitleToolStripMenuItem.Name = "openSecondSubtitleToolStripMenuItem";
            this.openSecondSubtitleToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.openSecondSubtitleToolStripMenuItem.Text = "Open second subtitle...";
            this.openSecondSubtitleToolStripMenuItem.Click += new System.EventHandler(this.OpenSecondSubtitleToolStripMenuItemClick);
            // 
            // setVideoOffsetToolStripMenuItem
            // 
            this.setVideoOffsetToolStripMenuItem.Name = "setVideoOffsetToolStripMenuItem";
            this.setVideoOffsetToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.setVideoOffsetToolStripMenuItem.Text = "Set video offset...";
            this.setVideoOffsetToolStripMenuItem.Click += new System.EventHandler(this.SetVideoOffsetToolStripMenuItemClick);
            // 
            // smpteTimeModedropFrameToolStripMenuItem
            // 
            this.smpteTimeModedropFrameToolStripMenuItem.Name = "smpteTimeModedropFrameToolStripMenuItem";
            this.smpteTimeModedropFrameToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.smpteTimeModedropFrameToolStripMenuItem.Text = "SMPTE timing (drop frame)";
            this.smpteTimeModedropFrameToolStripMenuItem.Click += new System.EventHandler(this.SmpteTimeModedropFrameToolStripMenuItem_Click);
            // 
            // toolStripMenuItemImportChapters
            // 
            this.toolStripMenuItemImportChapters.Name = "toolStripMenuItemImportChapters";
            this.toolStripMenuItemImportChapters.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemImportChapters.Text = "Import chapters from video...";
            this.toolStripMenuItemImportChapters.Click += new System.EventHandler(this.ToolStripMenuItemImportChaptersClick);
            // 
            // toolStripMenuItemImportShotChanges
            // 
            this.toolStripMenuItemImportShotChanges.Name = "toolStripMenuItemImportShotChanges";
            this.toolStripMenuItemImportShotChanges.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemImportShotChanges.Text = "Import shot changes...";
            this.toolStripMenuItemImportShotChanges.Click += new System.EventHandler(this.ToolStripMenuItemImportShotChangesClick);
            // 
            // toolStripMenuItemListShotChanges
            // 
            this.toolStripMenuItemListShotChanges.Name = "toolStripMenuItemListShotChanges";
            this.toolStripMenuItemListShotChanges.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemListShotChanges.Text = "List shot changes";
            this.toolStripMenuItemListShotChanges.Click += new System.EventHandler(this.ToolStripMenuItemListShotChangesClick);
            // 
            // toolStripMenuItemAddWaveformBatch
            // 
            this.toolStripMenuItemAddWaveformBatch.Name = "toolStripMenuItemAddWaveformBatch";
            this.toolStripMenuItemAddWaveformBatch.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemAddWaveformBatch.Text = "Add waveform batch...";
            this.toolStripMenuItemAddWaveformBatch.Click += new System.EventHandler(this.ToolStripMenuItemAddWaveformBatchClick);
            // 
            // generateBlankVideoToolStripMenuItem
            // 
            this.generateBlankVideoToolStripMenuItem.Name = "generateBlankVideoToolStripMenuItem";
            this.generateBlankVideoToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.generateBlankVideoToolStripMenuItem.Text = "Generate blank video...";
            this.generateBlankVideoToolStripMenuItem.Click += new System.EventHandler(this.GenerateBlankVideoToolStripMenuItemClick);
            // 
            // generateVideoWithSoftcodedSubtitlesToolStripMenuItem
            // 
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem.Name = "generateVideoWithSoftcodedSubtitlesToolStripMenuItem";
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem.Text = "Generate video with softcoded subtitles...";
            this.generateVideoWithSoftcodedSubtitlesToolStripMenuItem.Click += new System.EventHandler(this.GenerateVideoWithSoftcodedSubtitlesToolStripMenuItemClick);
            // 
            // generateVideoWithHardcodedSubtitleToolStripMenuItem
            // 
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem.Name = "generateVideoWithHardcodedSubtitleToolStripMenuItem";
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem.Text = "Generate video with hardcoded subtitle...";
            this.generateVideoWithHardcodedSubtitleToolStripMenuItem.Click += new System.EventHandler(this.GenerateVideoWithHardcodedSubtitleToolStripMenuItemClick);
            // 
            // audioToTextWhisperTolStripMenuItem
            // 
            this.audioToTextWhisperTolStripMenuItem.Name = "audioToTextWhisperTolStripMenuItem";
            this.audioToTextWhisperTolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.audioToTextWhisperTolStripMenuItem.Text = "Audio to text (whisper)...";
            this.audioToTextWhisperTolStripMenuItem.Click += new System.EventHandler(this.AudioToTextWhisperTolStripMenuItemClick);
            // 
            // videoaudioToTextToolStripMenuItem
            // 
            this.videoaudioToTextToolStripMenuItem.Name = "videoaudioToTextToolStripMenuItem";
            this.videoaudioToTextToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.videoaudioToTextToolStripMenuItem.Text = "Audio to text (Vosk/Kaldi)...";
            this.videoaudioToTextToolStripMenuItem.Click += new System.EventHandler(this.VideoaudioToTextToolStripMenuItemClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(292, 6);
            // 
            // undockVideoControlsToolStripMenuItem
            // 
            this.undockVideoControlsToolStripMenuItem.Name = "undockVideoControlsToolStripMenuItem";
            this.undockVideoControlsToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.undockVideoControlsToolStripMenuItem.Text = "Un-dock video controls";
            this.undockVideoControlsToolStripMenuItem.Click += new System.EventHandler(this.UndockVideoControlsToolStripMenuItemClick);
            // 
            // redockVideoControlsToolStripMenuItem
            // 
            this.redockVideoControlsToolStripMenuItem.Name = "redockVideoControlsToolStripMenuItem";
            this.redockVideoControlsToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.redockVideoControlsToolStripMenuItem.Text = "Re-dock video controls";
            this.redockVideoControlsToolStripMenuItem.Visible = false;
            this.redockVideoControlsToolStripMenuItem.Click += new System.EventHandler(this.RedockVideoControlsToolStripMenuItemClick);
            // 
            // toolStripMenuItemSynchronization
            // 
            this.toolStripMenuItemSynchronization.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAdjustAllTimes,
            this.visualSyncToolStripMenuItem,
            this.toolStripMenuItemPointSync,
            this.pointSyncViaOtherSubtitleToolStripMenuItem,
            this.toolStripMenuItemChangeFrameRate2,
            this.changeSpeedInPercentToolStripMenuItem});
            this.toolStripMenuItemSynchronization.Name = "toolStripMenuItemSynchronization";
            this.toolStripMenuItemSynchronization.Size = new System.Drawing.Size(106, 20);
            this.toolStripMenuItemSynchronization.Text = "Synchronization";
            // 
            // toolStripMenuItemAdjustAllTimes
            // 
            this.toolStripMenuItemAdjustAllTimes.Name = "toolStripMenuItemAdjustAllTimes";
            this.toolStripMenuItemAdjustAllTimes.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.toolStripMenuItemAdjustAllTimes.Size = new System.Drawing.Size(324, 22);
            this.toolStripMenuItemAdjustAllTimes.Text = "Adjust times (show earlier/later)...";
            this.toolStripMenuItemAdjustAllTimes.Click += new System.EventHandler(this.ToolStripMenuItemAdjustAllTimesClick);
            // 
            // visualSyncToolStripMenuItem
            // 
            this.visualSyncToolStripMenuItem.Name = "visualSyncToolStripMenuItem";
            this.visualSyncToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.visualSyncToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.visualSyncToolStripMenuItem.Text = "Visual sync...";
            this.visualSyncToolStripMenuItem.Click += new System.EventHandler(this.VisualSyncToolStripMenuItemClick);
            // 
            // toolStripMenuItemPointSync
            // 
            this.toolStripMenuItemPointSync.Name = "toolStripMenuItemPointSync";
            this.toolStripMenuItemPointSync.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.toolStripMenuItemPointSync.Size = new System.Drawing.Size(324, 22);
            this.toolStripMenuItemPointSync.Text = "Point sync...";
            this.toolStripMenuItemPointSync.Click += new System.EventHandler(this.ToolStripMenuItemPointSyncClick);
            // 
            // pointSyncViaOtherSubtitleToolStripMenuItem
            // 
            this.pointSyncViaOtherSubtitleToolStripMenuItem.Name = "pointSyncViaOtherSubtitleToolStripMenuItem";
            this.pointSyncViaOtherSubtitleToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.pointSyncViaOtherSubtitleToolStripMenuItem.Text = "Point sync via other subtitle...";
            this.pointSyncViaOtherSubtitleToolStripMenuItem.Click += new System.EventHandler(this.PointSyncViaOtherSubtitleToolStripMenuItemClick);
            // 
            // toolStripMenuItemChangeFrameRate2
            // 
            this.toolStripMenuItemChangeFrameRate2.Name = "toolStripMenuItemChangeFrameRate2";
            this.toolStripMenuItemChangeFrameRate2.Size = new System.Drawing.Size(324, 22);
            this.toolStripMenuItemChangeFrameRate2.Text = "Change frame rate...";
            this.toolStripMenuItemChangeFrameRate2.Click += new System.EventHandler(this.ToolStripMenuItemChangeFrameRateClick);
            // 
            // changeSpeedInPercentToolStripMenuItem
            // 
            this.changeSpeedInPercentToolStripMenuItem.Name = "changeSpeedInPercentToolStripMenuItem";
            this.changeSpeedInPercentToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.changeSpeedInPercentToolStripMenuItem.Text = "Change speed in percent...";
            this.changeSpeedInPercentToolStripMenuItem.Click += new System.EventHandler(this.ChangeSpeedInPercentToolStripMenuItemClick);
            // 
            // toolStripMenuItemAutoTranslate
            // 
            this.toolStripMenuItemAutoTranslate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.translateToolStripMenuItem,
            this.autotranslateNLLBToolStripMenuItem,
            this.autotranslateViaCopypasteToolStripMenuItem,
            this.toolStripSeparator26,
            this.mergeSentencesToolStripMenuItem,
            this.breaksplitLongLinesToolStripMenuItem});
            this.toolStripMenuItemAutoTranslate.Name = "toolStripMenuItemAutoTranslate";
            this.toolStripMenuItemAutoTranslate.Size = new System.Drawing.Size(94, 20);
            this.toolStripMenuItemAutoTranslate.Text = "Auto-translate";
            // 
            // translateToolStripMenuItem
            // 
            this.translateToolStripMenuItem.Name = "translateToolStripMenuItem";
            this.translateToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.translateToolStripMenuItem.Text = "Auto-translate...";
            this.translateToolStripMenuItem.Click += new System.EventHandler(this.TranslateToolStripMenuItemClick);
            // 
            // autotranslateNLLBToolStripMenuItem
            // 
            this.autotranslateNLLBToolStripMenuItem.Name = "autotranslateNLLBToolStripMenuItem";
            this.autotranslateNLLBToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.autotranslateNLLBToolStripMenuItem.Text = "Auto-translate (NLLB/LibreTranslate)...";
            this.autotranslateNLLBToolStripMenuItem.Click += new System.EventHandler(this.autotranslateNLLBToolStripMenuItem_Click);
            // 
            // autotranslateViaCopypasteToolStripMenuItem
            // 
            this.autotranslateViaCopypasteToolStripMenuItem.Name = "autotranslateViaCopypasteToolStripMenuItem";
            this.autotranslateViaCopypasteToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.autotranslateViaCopypasteToolStripMenuItem.Text = "Auto-translate via copy-paste...";
            this.autotranslateViaCopypasteToolStripMenuItem.Click += new System.EventHandler(this.AutotranslateViaCopypasteToolStripMenuItemClick);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(279, 6);
            // 
            // mergeSentencesToolStripMenuItem
            // 
            this.mergeSentencesToolStripMenuItem.Name = "mergeSentencesToolStripMenuItem";
            this.mergeSentencesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.mergeSentencesToolStripMenuItem.Text = "Merge sentences...";
            this.mergeSentencesToolStripMenuItem.Click += new System.EventHandler(this.MergeSentencesToolStripMenuItemClick);
            // 
            // breaksplitLongLinesToolStripMenuItem
            // 
            this.breaksplitLongLinesToolStripMenuItem.Name = "breaksplitLongLinesToolStripMenuItem";
            this.breaksplitLongLinesToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.breaksplitLongLinesToolStripMenuItem.Text = "Break/split long lines...";
            this.breaksplitLongLinesToolStripMenuItem.Click += new System.EventHandler(this.BreaksplitLongLinesToolStripMenuItemClick);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.wordListsToolStripMenuItem,
            this.changeLanguageToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItemClick);
            // 
            // wordListsToolStripMenuItem
            // 
            this.wordListsToolStripMenuItem.Name = "wordListsToolStripMenuItem";
            this.wordListsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.wordListsToolStripMenuItem.Text = "Word lists...";
            this.wordListsToolStripMenuItem.Click += new System.EventHandler(this.WordListsToolStripMenuItemClick);
            // 
            // changeLanguageToolStripMenuItem
            // 
            this.changeLanguageToolStripMenuItem.Name = "changeLanguageToolStripMenuItem";
            this.changeLanguageToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.changeLanguageToolStripMenuItem.Text = "Change language...";
            this.changeLanguageToolStripMenuItem.Click += new System.EventHandler(this.ChangeLanguageToolStripMenuItemClick);
            // 
            // toolStripMenuItemNetworking
            // 
            this.toolStripMenuItemNetworking.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServerToolStripMenuItem,
            this.joinSessionToolStripMenuItem,
            this.chatToolStripMenuItem,
            this.showSessionKeyLogToolStripMenuItem,
            this.leaveSessionToolStripMenuItem});
            this.toolStripMenuItemNetworking.Name = "toolStripMenuItemNetworking";
            this.toolStripMenuItemNetworking.Size = new System.Drawing.Size(81, 20);
            this.toolStripMenuItemNetworking.Text = "Networking";
            this.toolStripMenuItemNetworking.DropDownOpening += new System.EventHandler(this.ToolStripMenuItemNetworkingDropDownOpening);
            // 
            // startServerToolStripMenuItem
            // 
            this.startServerToolStripMenuItem.Name = "startServerToolStripMenuItem";
            this.startServerToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.startServerToolStripMenuItem.Text = "Start new session";
            this.startServerToolStripMenuItem.Click += new System.EventHandler(this.StartServerToolStripMenuItemClick);
            // 
            // joinSessionToolStripMenuItem
            // 
            this.joinSessionToolStripMenuItem.Name = "joinSessionToolStripMenuItem";
            this.joinSessionToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.joinSessionToolStripMenuItem.Text = "Join session";
            this.joinSessionToolStripMenuItem.Click += new System.EventHandler(this.JoinSessionToolStripMenuItemClick);
            // 
            // chatToolStripMenuItem
            // 
            this.chatToolStripMenuItem.Name = "chatToolStripMenuItem";
            this.chatToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.chatToolStripMenuItem.Text = "Chat";
            this.chatToolStripMenuItem.Click += new System.EventHandler(this.ChatToolStripMenuItemClick);
            // 
            // showSessionKeyLogToolStripMenuItem
            // 
            this.showSessionKeyLogToolStripMenuItem.Name = "showSessionKeyLogToolStripMenuItem";
            this.showSessionKeyLogToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showSessionKeyLogToolStripMenuItem.Text = "Show session info and log";
            this.showSessionKeyLogToolStripMenuItem.Click += new System.EventHandler(this.ShowSessionKeyLogToolStripMenuItemClick);
            // 
            // leaveSessionToolStripMenuItem
            // 
            this.leaveSessionToolStripMenuItem.Name = "leaveSessionToolStripMenuItem";
            this.leaveSessionToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.leaveSessionToolStripMenuItem.Text = "Leave session";
            this.leaveSessionToolStripMenuItem.Click += new System.EventHandler(this.LeaveSessionToolStripMenuItemClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripMenuItemSplitterCheckForUpdates,
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.DropDownClosed += new System.EventHandler(this.MenuClosed);
            this.helpToolStripMenuItem.DropDownOpening += new System.EventHandler(this.MenuOpened);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItemClick);
            // 
            // toolStripMenuItemSplitterCheckForUpdates
            // 
            this.toolStripMenuItemSplitterCheckForUpdates.Name = "toolStripMenuItemSplitterCheckForUpdates";
            this.toolStripMenuItemSplitterCheckForUpdates.Size = new System.Drawing.Size(178, 6);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.HelpToolStripMenuItem1Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // contextMenuStripListView
            // 
            this.contextMenuStripListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setStylesForSelectedLinesToolStripMenuItem,
            this.setActorForSelectedLinesToolStripMenuItem,
            this.toolStripMenuItemSetLayer,
            this.toolStripMenuItemAssaTools,
            this.toolStripMenuItemAssStyles,
            this.toolStripSeparatorAssa,
            this.toolStripMenuItemSetRegion,
            this.toolStripMenuItemSetLanguage,
            this.toolStripMenuItemWebVttStyle,
            this.toolStripMenuItemWebVTT,
            this.toolStripMenuWebVttBrowserPreview,
            this.toolStripMenuItemDelete,
            this.toolStripMenuItemInsertBefore,
            this.toolStripMenuItemInsertAfter,
            this.toolStripMenuItemInsertSubtitle,
            this.toolStripMenuItemCopySourceText,
            this.toolStripMenuItemColumn,
            this.toolStripMenuItemBookmark,
            this.toolStripMenuItemGoToSourceView,
            this.toolStripSeparator7,
            this.splitLineToolStripMenuItem,
            this.toolStripMenuItemMergeLines,
            this.toolStripMenuItemMergeDialog,
            this.mergeBeforeToolStripMenuItem,
            this.mergeAfterToolStripMenuItem,
            this.toolStripSeparator4Extend,
            this.extendBeforeToolStripMenuItem,
            this.extendAfterToolStripMenuItem,
            this.toolStripSeparator8,
            this.removeFormattinglToolStripMenuItem,
            this.boldToolStripMenuItem,
            this.italicToolStripMenuItem,
            this.boxToolStripMenuItem,
            this.underlineToolStripMenuItem,
            this.colorToolStripMenuItem,
            this.toolStripMenuItemFont,
            this.toolStripMenuItemAlignment,
            this.toolStripMenuItemSurroundWithMusicSymbols,
            this.toolStripSeparator2,
            this.toolStripMenuItemSelectedLines,
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine});
            this.contextMenuStripListView.Name = "contextMenuStripListView";
            this.contextMenuStripListView.Size = new System.Drawing.Size(311, 826);
            this.contextMenuStripListView.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.MenuClosed);
            this.contextMenuStripListView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripListViewOpening);
            this.contextMenuStripListView.Opened += new System.EventHandler(this.MenuOpened);
            // 
            // setStylesForSelectedLinesToolStripMenuItem
            // 
            this.setStylesForSelectedLinesToolStripMenuItem.Name = "setStylesForSelectedLinesToolStripMenuItem";
            this.setStylesForSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.setStylesForSelectedLinesToolStripMenuItem.Text = "Set style";
            // 
            // setActorForSelectedLinesToolStripMenuItem
            // 
            this.setActorForSelectedLinesToolStripMenuItem.Name = "setActorForSelectedLinesToolStripMenuItem";
            this.setActorForSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.setActorForSelectedLinesToolStripMenuItem.Text = "Set actor";
            // 
            // toolStripMenuItemSetLayer
            // 
            this.toolStripMenuItemSetLayer.Name = "toolStripMenuItemSetLayer";
            this.toolStripMenuItemSetLayer.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemSetLayer.Text = "Set layer";
            // 
            // toolStripMenuItemAssaTools
            // 
            this.toolStripMenuItemAssaTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyCustomStylesToolStripMenuItem,
            this.setPositionToolStripMenuItem,
            this.progressBarToolStripMenuItem,
            this.videoResolutionResamplerToolStripMenuItem,
            this.generateBackgroundBoxToolStripMenuItem,
            this.colorPickerToolStripMenuItem});
            this.toolStripMenuItemAssaTools.Name = "toolStripMenuItemAssaTools";
            this.toolStripMenuItemAssaTools.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemAssaTools.Text = "ASSA tools";
            // 
            // applyCustomStylesToolStripMenuItem
            // 
            this.applyCustomStylesToolStripMenuItem.Name = "applyCustomStylesToolStripMenuItem";
            this.applyCustomStylesToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.applyCustomStylesToolStripMenuItem.Text = "Apply custom tags...";
            this.applyCustomStylesToolStripMenuItem.Click += new System.EventHandler(this.ApplyCustomStylesToolStripMenuItemClick);
            // 
            // setPositionToolStripMenuItem
            // 
            this.setPositionToolStripMenuItem.Name = "setPositionToolStripMenuItem";
            this.setPositionToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.setPositionToolStripMenuItem.Text = "Set position...";
            this.setPositionToolStripMenuItem.Click += new System.EventHandler(this.SetPositionToolStripMenuItemClick);
            // 
            // progressBarToolStripMenuItem
            // 
            this.progressBarToolStripMenuItem.Name = "progressBarToolStripMenuItem";
            this.progressBarToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.progressBarToolStripMenuItem.Text = "Generate progress bar...";
            this.progressBarToolStripMenuItem.Click += new System.EventHandler(this.ProgressBarToolStripMenuItemClick);
            // 
            // videoResolutionResamplerToolStripMenuItem
            // 
            this.videoResolutionResamplerToolStripMenuItem.Name = "videoResolutionResamplerToolStripMenuItem";
            this.videoResolutionResamplerToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.videoResolutionResamplerToolStripMenuItem.Text = "Video resolution resampler...";
            this.videoResolutionResamplerToolStripMenuItem.Click += new System.EventHandler(this.VideoResolutionResamplerToolStripMenuItemClick);
            // 
            // generateBackgroundBoxToolStripMenuItem
            // 
            this.generateBackgroundBoxToolStripMenuItem.Name = "generateBackgroundBoxToolStripMenuItem";
            this.generateBackgroundBoxToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.generateBackgroundBoxToolStripMenuItem.Text = "Generate background box...";
            this.generateBackgroundBoxToolStripMenuItem.Click += new System.EventHandler(this.GenerateBackgroundBoxToolStripMenuItemClick);
            // 
            // colorPickerToolStripMenuItem
            // 
            this.colorPickerToolStripMenuItem.Name = "colorPickerToolStripMenuItem";
            this.colorPickerToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.colorPickerToolStripMenuItem.Text = "Color picker...";
            this.colorPickerToolStripMenuItem.Click += new System.EventHandler(this.ColorPickerToolStripMenuItemClick);
            // 
            // toolStripMenuItemAssStyles
            // 
            this.toolStripMenuItemAssStyles.Name = "toolStripMenuItemAssStyles";
            this.toolStripMenuItemAssStyles.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemAssStyles.Text = "ASS: Styles...";
            this.toolStripMenuItemAssStyles.Click += new System.EventHandler(this.ToolStripMenuItemAssStylesClick);
            // 
            // toolStripSeparatorAssa
            // 
            this.toolStripSeparatorAssa.Name = "toolStripSeparatorAssa";
            this.toolStripSeparatorAssa.Size = new System.Drawing.Size(307, 6);
            // 
            // toolStripMenuItemSetRegion
            // 
            this.toolStripMenuItemSetRegion.Name = "toolStripMenuItemSetRegion";
            this.toolStripMenuItemSetRegion.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemSetRegion.Text = "Timed text - set region";
            // 
            // toolStripMenuItemSetLanguage
            // 
            this.toolStripMenuItemSetLanguage.Name = "toolStripMenuItemSetLanguage";
            this.toolStripMenuItemSetLanguage.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemSetLanguage.Text = "Timed text - set language";
            // 
            // toolStripMenuItemWebVttStyle
            // 
            this.toolStripMenuItemWebVttStyle.Name = "toolStripMenuItemWebVttStyle";
            this.toolStripMenuItemWebVttStyle.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemWebVttStyle.Text = "WebVTT style...";
            this.toolStripMenuItemWebVttStyle.Click += new System.EventHandler(this.TolStripMenuItemWebVttStyleClick);
            // 
            // toolStripMenuItemWebVTT
            // 
            this.toolStripMenuItemWebVTT.Name = "toolStripMenuItemWebVTT";
            this.toolStripMenuItemWebVTT.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemWebVTT.Text = "WebVTT voice";
            // 
            // toolStripMenuWebVttBrowserPreview
            // 
            this.toolStripMenuWebVttBrowserPreview.Name = "toolStripMenuWebVttBrowserPreview";
            this.toolStripMenuWebVttBrowserPreview.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuWebVttBrowserPreview.Text = "WebVTT browser preview";
            this.toolStripMenuWebVttBrowserPreview.Click += new System.EventHandler(this.ToolStripMenuItemPreviewClick);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemDelete.Text = "Delete";
            this.toolStripMenuItemDelete.Click += new System.EventHandler(this.ToolStripMenuItemDeleteClick);
            // 
            // toolStripMenuItemInsertBefore
            // 
            this.toolStripMenuItemInsertBefore.Name = "toolStripMenuItemInsertBefore";
            this.toolStripMenuItemInsertBefore.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemInsertBefore.Text = "Insert before";
            this.toolStripMenuItemInsertBefore.Click += new System.EventHandler(this.ToolStripMenuItemInsertBeforeClick);
            // 
            // toolStripMenuItemInsertAfter
            // 
            this.toolStripMenuItemInsertAfter.Name = "toolStripMenuItemInsertAfter";
            this.toolStripMenuItemInsertAfter.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemInsertAfter.Text = "Insert after";
            this.toolStripMenuItemInsertAfter.Click += new System.EventHandler(this.ToolStripMenuItemInsertAfterClick);
            // 
            // toolStripMenuItemInsertSubtitle
            // 
            this.toolStripMenuItemInsertSubtitle.Name = "toolStripMenuItemInsertSubtitle";
            this.toolStripMenuItemInsertSubtitle.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemInsertSubtitle.Text = "Insert subtitle file after this line...";
            this.toolStripMenuItemInsertSubtitle.Click += new System.EventHandler(this.ToolStripMenuItemInsertSubtitleClick);
            // 
            // toolStripMenuItemCopySourceText
            // 
            this.toolStripMenuItemCopySourceText.Name = "toolStripMenuItemCopySourceText";
            this.toolStripMenuItemCopySourceText.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemCopySourceText.Text = "Copy as text to clipboard";
            this.toolStripMenuItemCopySourceText.Click += new System.EventHandler(this.ToolStripMenuItemCopySourceTextClick);
            // 
            // toolStripMenuItemColumn
            // 
            this.toolStripMenuItemColumn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnDeleteTextOnlyToolStripMenuItem,
            this.toolStripMenuItemColumnDeleteText,
            this.ShiftTextCellsDownToolStripMenuItem,
            this.toolStripMenuItemInsertTextFromSub,
            this.toolStripMenuItemColumnImportText,
            this.toolStripMenuItemPasteSpecial,
            this.copyOriginalTextToCurrentToolStripMenuItem,
            this.moveTextUpToolStripMenuItem,
            this.moveTextDownToolStripMenuItem});
            this.toolStripMenuItemColumn.Name = "toolStripMenuItemColumn";
            this.toolStripMenuItemColumn.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemColumn.Text = "Column";
            this.toolStripMenuItemColumn.DropDownOpening += new System.EventHandler(this.ToolStripMenuItemColumnDropDownOpening);
            // 
            // columnDeleteTextOnlyToolStripMenuItem
            // 
            this.columnDeleteTextOnlyToolStripMenuItem.Name = "columnDeleteTextOnlyToolStripMenuItem";
            this.columnDeleteTextOnlyToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.columnDeleteTextOnlyToolStripMenuItem.Text = "Delete text";
            this.columnDeleteTextOnlyToolStripMenuItem.Click += new System.EventHandler(this.ColumnDeleteTextOnlyToolStripMenuItemClick);
            // 
            // toolStripMenuItemColumnDeleteText
            // 
            this.toolStripMenuItemColumnDeleteText.Name = "toolStripMenuItemColumnDeleteText";
            this.toolStripMenuItemColumnDeleteText.Size = new System.Drawing.Size(340, 22);
            this.toolStripMenuItemColumnDeleteText.Text = "Delete text and shift text cells up";
            this.toolStripMenuItemColumnDeleteText.Click += new System.EventHandler(this.DeleteAndShiftCellsUpToolStripMenuItemClick);
            // 
            // ShiftTextCellsDownToolStripMenuItem
            // 
            this.ShiftTextCellsDownToolStripMenuItem.Name = "ShiftTextCellsDownToolStripMenuItem";
            this.ShiftTextCellsDownToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.ShiftTextCellsDownToolStripMenuItem.Text = "Insert and shift text cells down";
            this.ShiftTextCellsDownToolStripMenuItem.Click += new System.EventHandler(this.ShiftTextCellsDownToolStripMenuItem_Click);
            // 
            // toolStripMenuItemInsertTextFromSub
            // 
            this.toolStripMenuItemInsertTextFromSub.Name = "toolStripMenuItemInsertTextFromSub";
            this.toolStripMenuItemInsertTextFromSub.Size = new System.Drawing.Size(340, 22);
            this.toolStripMenuItemInsertTextFromSub.Text = "Insert text from subtitle and shift cells down...";
            this.toolStripMenuItemInsertTextFromSub.Click += new System.EventHandler(this.ToolStripMenuItemInsertTextFromSubClick);
            // 
            // toolStripMenuItemColumnImportText
            // 
            this.toolStripMenuItemColumnImportText.Name = "toolStripMenuItemColumnImportText";
            this.toolStripMenuItemColumnImportText.Size = new System.Drawing.Size(340, 22);
            this.toolStripMenuItemColumnImportText.Text = "Import text and shift text cells down...";
            this.toolStripMenuItemColumnImportText.Click += new System.EventHandler(this.ToolStripMenuItemColumnImportTextClick);
            // 
            // toolStripMenuItemPasteSpecial
            // 
            this.toolStripMenuItemPasteSpecial.Name = "toolStripMenuItemPasteSpecial";
            this.toolStripMenuItemPasteSpecial.Size = new System.Drawing.Size(340, 22);
            this.toolStripMenuItemPasteSpecial.Text = "Paste from clipboard...";
            this.toolStripMenuItemPasteSpecial.Click += new System.EventHandler(this.ToolStripMenuItemPasteSpecialClick);
            // 
            // copyOriginalTextToCurrentToolStripMenuItem
            // 
            this.copyOriginalTextToCurrentToolStripMenuItem.Name = "copyOriginalTextToCurrentToolStripMenuItem";
            this.copyOriginalTextToCurrentToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.copyOriginalTextToCurrentToolStripMenuItem.Text = "Copy original text to current";
            this.copyOriginalTextToCurrentToolStripMenuItem.Click += new System.EventHandler(this.CopyOriginalTextToCurrentToolStripMenuItemClick);
            // 
            // moveTextUpToolStripMenuItem
            // 
            this.moveTextUpToolStripMenuItem.Name = "moveTextUpToolStripMenuItem";
            this.moveTextUpToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.moveTextUpToolStripMenuItem.Text = "Move text up";
            this.moveTextUpToolStripMenuItem.Click += new System.EventHandler(this.MoveTextUpToolStripMenuItemClick);
            // 
            // moveTextDownToolStripMenuItem
            // 
            this.moveTextDownToolStripMenuItem.Name = "moveTextDownToolStripMenuItem";
            this.moveTextDownToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.moveTextDownToolStripMenuItem.Text = "Move text down";
            this.moveTextDownToolStripMenuItem.Click += new System.EventHandler(this.MoveTextDownToolStripMenuItemClick);
            // 
            // toolStripMenuItemBookmark
            // 
            this.toolStripMenuItemBookmark.Name = "toolStripMenuItemBookmark";
            this.toolStripMenuItemBookmark.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemBookmark.Text = "Bookmark...";
            this.toolStripMenuItemBookmark.Click += new System.EventHandler(this.ToolStripMenuItemBookmarkClick);
            // 
            // toolStripMenuItemGoToSourceView
            // 
            this.toolStripMenuItemGoToSourceView.Name = "toolStripMenuItemGoToSourceView";
            this.toolStripMenuItemGoToSourceView.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemGoToSourceView.Text = "Go to source view";
            this.toolStripMenuItemGoToSourceView.Click += new System.EventHandler(this.ToolStripMenuItemGoToSourceViewClick);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(307, 6);
            // 
            // splitLineToolStripMenuItem
            // 
            this.splitLineToolStripMenuItem.Name = "splitLineToolStripMenuItem";
            this.splitLineToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.splitLineToolStripMenuItem.Text = "Split";
            this.splitLineToolStripMenuItem.Click += new System.EventHandler(this.SplitLineToolStripMenuItemClick);
            // 
            // toolStripMenuItemMergeLines
            // 
            this.toolStripMenuItemMergeLines.Name = "toolStripMenuItemMergeLines";
            this.toolStripMenuItemMergeLines.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemMergeLines.Text = "Merge selected lines";
            this.toolStripMenuItemMergeLines.Click += new System.EventHandler(this.ToolStripMenuItemMergeLinesClick);
            // 
            // toolStripMenuItemMergeDialog
            // 
            this.toolStripMenuItemMergeDialog.Name = "toolStripMenuItemMergeDialog";
            this.toolStripMenuItemMergeDialog.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemMergeDialog.Text = "Merge selected lines as dialog";
            this.toolStripMenuItemMergeDialog.Click += new System.EventHandler(this.ToolStripMenuItemMergeDialogClick);
            // 
            // mergeBeforeToolStripMenuItem
            // 
            this.mergeBeforeToolStripMenuItem.Name = "mergeBeforeToolStripMenuItem";
            this.mergeBeforeToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.mergeBeforeToolStripMenuItem.Text = "Merge with line before";
            this.mergeBeforeToolStripMenuItem.Click += new System.EventHandler(this.MergeBeforeToolStripMenuItemClick);
            // 
            // mergeAfterToolStripMenuItem
            // 
            this.mergeAfterToolStripMenuItem.Name = "mergeAfterToolStripMenuItem";
            this.mergeAfterToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.mergeAfterToolStripMenuItem.Text = "Merge with line after";
            this.mergeAfterToolStripMenuItem.Click += new System.EventHandler(this.MergeAfterToolStripMenuItemClick);
            // 
            // toolStripSeparator4Extend
            // 
            this.toolStripSeparator4Extend.Name = "toolStripSeparator4Extend";
            this.toolStripSeparator4Extend.Size = new System.Drawing.Size(307, 6);
            // 
            // extendBeforeToolStripMenuItem
            // 
            this.extendBeforeToolStripMenuItem.Name = "extendBeforeToolStripMenuItem";
            this.extendBeforeToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.extendBeforeToolStripMenuItem.Text = "Extend to line before";
            this.extendBeforeToolStripMenuItem.Click += new System.EventHandler(this.ExtendBeforeToolStripMenuItemClick);
            // 
            // extendAfterToolStripMenuItem
            // 
            this.extendAfterToolStripMenuItem.Name = "extendAfterToolStripMenuItem";
            this.extendAfterToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.extendAfterToolStripMenuItem.Text = "Extend to line after";
            this.extendAfterToolStripMenuItem.Click += new System.EventHandler(this.ExtendAfterToolStripMenuItemClick);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(307, 6);
            // 
            // removeFormattinglToolStripMenuItem
            // 
            this.removeFormattinglToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllFormattingsToolStripMenuItem,
            this.removeBoldToolStripMenuItem,
            this.removeItalicToolStripMenuItem,
            this.removeUnderlineToolStripMenuItem,
            this.removeColorToolStripMenuItem,
            this.removeFontNameToolStripMenuItem,
            this.removeAlignmentToolStripMenuItem});
            this.removeFormattinglToolStripMenuItem.Name = "removeFormattinglToolStripMenuItem";
            this.removeFormattinglToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.removeFormattinglToolStripMenuItem.Text = "Remove formatting";
            // 
            // removeAllFormattingsToolStripMenuItem
            // 
            this.removeAllFormattingsToolStripMenuItem.Name = "removeAllFormattingsToolStripMenuItem";
            this.removeAllFormattingsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeAllFormattingsToolStripMenuItem.Text = "Remove all formattings";
            this.removeAllFormattingsToolStripMenuItem.Click += new System.EventHandler(this.RemoveAllFormattingsToolStripMenuItemClick);
            // 
            // removeBoldToolStripMenuItem
            // 
            this.removeBoldToolStripMenuItem.Name = "removeBoldToolStripMenuItem";
            this.removeBoldToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeBoldToolStripMenuItem.Text = "Remove bold";
            this.removeBoldToolStripMenuItem.Click += new System.EventHandler(this.RemoveBoldToolStripMenuItemClick);
            // 
            // removeItalicToolStripMenuItem
            // 
            this.removeItalicToolStripMenuItem.Name = "removeItalicToolStripMenuItem";
            this.removeItalicToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeItalicToolStripMenuItem.Text = "Remove italic";
            this.removeItalicToolStripMenuItem.Click += new System.EventHandler(this.RemoveItalicToolStripMenuItemClick);
            // 
            // removeUnderlineToolStripMenuItem
            // 
            this.removeUnderlineToolStripMenuItem.Name = "removeUnderlineToolStripMenuItem";
            this.removeUnderlineToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeUnderlineToolStripMenuItem.Text = "Remove underline";
            this.removeUnderlineToolStripMenuItem.Click += new System.EventHandler(this.RemoveUnderlineToolStripMenuItemClick);
            // 
            // removeColorToolStripMenuItem
            // 
            this.removeColorToolStripMenuItem.Name = "removeColorToolStripMenuItem";
            this.removeColorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeColorToolStripMenuItem.Text = "Remove color";
            this.removeColorToolStripMenuItem.Click += new System.EventHandler(this.RemoveColorToolStripMenuItemClick);
            // 
            // removeFontNameToolStripMenuItem
            // 
            this.removeFontNameToolStripMenuItem.Name = "removeFontNameToolStripMenuItem";
            this.removeFontNameToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeFontNameToolStripMenuItem.Text = "Remove font name";
            this.removeFontNameToolStripMenuItem.Click += new System.EventHandler(this.RemoveFontNameToolStripMenuItemClick);
            // 
            // removeAlignmentToolStripMenuItem
            // 
            this.removeAlignmentToolStripMenuItem.Name = "removeAlignmentToolStripMenuItem";
            this.removeAlignmentToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeAlignmentToolStripMenuItem.Text = "Remove alignment";
            this.removeAlignmentToolStripMenuItem.Click += new System.EventHandler(this.RemoveAlignmentToolStripMenuItemClick);
            // 
            // boldToolStripMenuItem
            // 
            this.boldToolStripMenuItem.Name = "boldToolStripMenuItem";
            this.boldToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.boldToolStripMenuItem.Text = "Bold";
            this.boldToolStripMenuItem.Click += new System.EventHandler(this.BoldToolStripMenuItemClick);
            // 
            // italicToolStripMenuItem
            // 
            this.italicToolStripMenuItem.Name = "italicToolStripMenuItem";
            this.italicToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.italicToolStripMenuItem.Text = "Italic";
            this.italicToolStripMenuItem.Click += new System.EventHandler(this.ItalicToolStripMenuItemClick);
            // 
            // boxToolStripMenuItem
            // 
            this.boxToolStripMenuItem.Name = "boxToolStripMenuItem";
            this.boxToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.boxToolStripMenuItem.Text = "Box";
            this.boxToolStripMenuItem.Click += new System.EventHandler(this.BoxToolStripMenuItemClick);
            // 
            // underlineToolStripMenuItem
            // 
            this.underlineToolStripMenuItem.Name = "underlineToolStripMenuItem";
            this.underlineToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.underlineToolStripMenuItem.Text = "Underline";
            this.underlineToolStripMenuItem.Click += new System.EventHandler(this.UnderlineToolStripMenuItemClick);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.colorToolStripMenuItem.Text = "Color...";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.ColorToolStripMenuItemClick);
            // 
            // toolStripMenuItemFont
            // 
            this.toolStripMenuItemFont.Name = "toolStripMenuItemFont";
            this.toolStripMenuItemFont.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemFont.Text = "Font name...";
            this.toolStripMenuItemFont.Click += new System.EventHandler(this.ToolStripMenuItemFontClick);
            // 
            // toolStripMenuItemAlignment
            // 
            this.toolStripMenuItemAlignment.Name = "toolStripMenuItemAlignment";
            this.toolStripMenuItemAlignment.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemAlignment.Text = "Alignment";
            this.toolStripMenuItemAlignment.Click += new System.EventHandler(this.ToolStripMenuItemAlignmentClick);
            // 
            // toolStripMenuItemSurroundWithMusicSymbols
            // 
            this.toolStripMenuItemSurroundWithMusicSymbols.Name = "toolStripMenuItemSurroundWithMusicSymbols";
            this.toolStripMenuItemSurroundWithMusicSymbols.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemSurroundWithMusicSymbols.Text = "♪";
            this.toolStripMenuItemSurroundWithMusicSymbols.Click += new System.EventHandler(this.ToolStripMenuItemSurroundWithMusicSymbolsClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(307, 6);
            // 
            // toolStripMenuItemSelectedLines
            // 
            this.toolStripMenuItemSelectedLines.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem,
            this.changeCasingForSelectedLinesToolStripMenuItem,
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem,
            this.visualSyncSelectedLinesToolStripMenuItem,
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem,
            this.showSelectedLinesEarlierlaterToolStripMenuItem,
            this.toolStripMenuItemTranslateSelected,
            this.toolStripMenuItemUnbreakLines,
            this.toolStripMenuItemAutoBreakLines,
            this.toolStripMenuItemEvenlyDistributeLines,
            this.toolStripMenuItemSaveSelectedLines,
            this.typeEffectToolStripMenuItem,
            this.karaokeEffectToolStripMenuItem});
            this.toolStripMenuItemSelectedLines.Name = "toolStripMenuItemSelectedLines";
            this.toolStripMenuItemSelectedLines.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemSelectedLines.Text = "Selected lines";
            // 
            // adjustDisplayTimeForSelectedLinesToolStripMenuItem
            // 
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem.Name = "adjustDisplayTimeForSelectedLinesToolStripMenuItem";
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem.Text = "Adjust display time for selected lines...";
            this.adjustDisplayTimeForSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.AdjustDisplayTimeForSelectedLinesToolStripMenuItemClick);
            // 
            // changeCasingForSelectedLinesToolStripMenuItem
            // 
            this.changeCasingForSelectedLinesToolStripMenuItem.Name = "changeCasingForSelectedLinesToolStripMenuItem";
            this.changeCasingForSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.changeCasingForSelectedLinesToolStripMenuItem.Text = "Change casing for selected lines...";
            this.changeCasingForSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.ChangeCasingForSelectedLinesToolStripMenuItemClick);
            // 
            // fixCommonErrorsInSelectedLinesToolStripMenuItem
            // 
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem.Name = "fixCommonErrorsInSelectedLinesToolStripMenuItem";
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem.Text = "Fix common errors in selected lines...";
            this.fixCommonErrorsInSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.FixCommonErrorsInSelectedLinesToolStripMenuItemClick);
            // 
            // visualSyncSelectedLinesToolStripMenuItem
            // 
            this.visualSyncSelectedLinesToolStripMenuItem.Name = "visualSyncSelectedLinesToolStripMenuItem";
            this.visualSyncSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.visualSyncSelectedLinesToolStripMenuItem.Text = "Visual sync selected lines...";
            this.visualSyncSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.VisualSyncSelectedLinesToolStripMenuItemClick);
            // 
            // beautifyTimeCodesOfSelectedLinesToolStripMenuItem
            // 
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem.Name = "beautifyTimeCodesOfSelectedLinesToolStripMenuItem";
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem.Text = "Beautify time codes of selected lines...";
            this.beautifyTimeCodesOfSelectedLinesToolStripMenuItem.Click += new System.EventHandler(this.BeautifyTimeCodesOfSelectedLinesToolStripMenuItemClick);
            // 
            // showSelectedLinesEarlierlaterToolStripMenuItem
            // 
            this.showSelectedLinesEarlierlaterToolStripMenuItem.Name = "showSelectedLinesEarlierlaterToolStripMenuItem";
            this.showSelectedLinesEarlierlaterToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.showSelectedLinesEarlierlaterToolStripMenuItem.Text = "Show selected lines earlier/later...";
            this.showSelectedLinesEarlierlaterToolStripMenuItem.Click += new System.EventHandler(this.ShowSelectedLinesEarlierlaterToolStripMenuItemClick);
            // 
            // toolStripMenuItemTranslateSelected
            // 
            this.toolStripMenuItemTranslateSelected.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.genericTranslateToolStripMenuItem});
            this.toolStripMenuItemTranslateSelected.Name = "toolStripMenuItemTranslateSelected";
            this.toolStripMenuItemTranslateSelected.Size = new System.Drawing.Size(298, 22);
            this.toolStripMenuItemTranslateSelected.Text = "Translate selected lines via...";
            // 
            // genericTranslateToolStripMenuItem
            // 
            this.genericTranslateToolStripMenuItem.Name = "genericTranslateToolStripMenuItem";
            this.genericTranslateToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.genericTranslateToolStripMenuItem.Text = "translate...";
            this.genericTranslateToolStripMenuItem.Click += new System.EventHandler(this.TranslateSelectedLinesToolStripMenuItemClick);
            // 
            // toolStripMenuItemUnbreakLines
            // 
            this.toolStripMenuItemUnbreakLines.Name = "toolStripMenuItemUnbreakLines";
            this.toolStripMenuItemUnbreakLines.Size = new System.Drawing.Size(298, 22);
            this.toolStripMenuItemUnbreakLines.Text = "Remove line-breaks in selected lines...";
            this.toolStripMenuItemUnbreakLines.Click += new System.EventHandler(this.ToolStripMenuItemUnbreakLinesClick);
            // 
            // toolStripMenuItemAutoBreakLines
            // 
            this.toolStripMenuItemAutoBreakLines.Name = "toolStripMenuItemAutoBreakLines";
            this.toolStripMenuItemAutoBreakLines.Size = new System.Drawing.Size(298, 22);
            this.toolStripMenuItemAutoBreakLines.Text = "Auto balance selected lines...";
            this.toolStripMenuItemAutoBreakLines.Click += new System.EventHandler(this.ToolStripMenuItemAutoBreakLinesClick);
            // 
            // toolStripMenuItemEvenlyDistributeLines
            // 
            this.toolStripMenuItemEvenlyDistributeLines.Name = "toolStripMenuItemEvenlyDistributeLines";
            this.toolStripMenuItemEvenlyDistributeLines.Size = new System.Drawing.Size(298, 22);
            this.toolStripMenuItemEvenlyDistributeLines.Text = "Evenly distribute lines (CPS)...";
            this.toolStripMenuItemEvenlyDistributeLines.Click += new System.EventHandler(this.ToolStripMenuItemEvenlyDistributeLinesClick);
            // 
            // toolStripMenuItemSaveSelectedLines
            // 
            this.toolStripMenuItemSaveSelectedLines.Name = "toolStripMenuItemSaveSelectedLines";
            this.toolStripMenuItemSaveSelectedLines.Size = new System.Drawing.Size(298, 22);
            this.toolStripMenuItemSaveSelectedLines.Text = "Save selected lines as...";
            this.toolStripMenuItemSaveSelectedLines.Click += new System.EventHandler(this.ToolStripMenuItemSaveSelectedLinesClick);
            // 
            // typeEffectToolStripMenuItem
            // 
            this.typeEffectToolStripMenuItem.Name = "typeEffectToolStripMenuItem";
            this.typeEffectToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.typeEffectToolStripMenuItem.Text = "Typewriter effect...";
            this.typeEffectToolStripMenuItem.Click += new System.EventHandler(this.TypeEffectToolStripMenuItemClick);
            // 
            // karaokeEffectToolStripMenuItem
            // 
            this.karaokeEffectToolStripMenuItem.Name = "karaokeEffectToolStripMenuItem";
            this.karaokeEffectToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.karaokeEffectToolStripMenuItem.Text = "Karaoke effect...";
            this.karaokeEffectToolStripMenuItem.Click += new System.EventHandler(this.KaraokeEffectToolStripMenuItemClick);
            // 
            // toolStripMenuItemGoogleMicrosoftTranslateSelLine
            // 
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine.Name = "toolStripMenuItemGoogleMicrosoftTranslateSelLine";
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine.Size = new System.Drawing.Size(310, 22);
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine.Text = "Google/Microsoft translate selected line";
            this.toolStripMenuItemGoogleMicrosoftTranslateSelLine.Click += new System.EventHandler(this.ToolStripMenuItemGoogleMicrosoftTranslateSelLineClick);
            // 
            // toolStripMenuItemEmptyGoToSourceView
            // 
            this.toolStripMenuItemEmptyGoToSourceView.Name = "toolStripMenuItemEmptyGoToSourceView";
            this.toolStripMenuItemEmptyGoToSourceView.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItemEmptyGoToSourceView.Text = "Go to source view";
            this.toolStripMenuItemEmptyGoToSourceView.Click += new System.EventHandler(this.ToolStripMenuItemGoToSourceViewClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // contextMenuStripHideVideoControls
            // 
            this.contextMenuStripHideVideoControls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideVideoControlsToolStripMenuItem});
            this.contextMenuStripHideVideoControls.Name = "contextMenuStripVideoControls";
            this.contextMenuStripHideVideoControls.Size = new System.Drawing.Size(191, 26);
            // 
            // hideVideoControlsToolStripMenuItem
            // 
            this.hideVideoControlsToolStripMenuItem.Name = "hideVideoControlsToolStripMenuItem";
            this.hideVideoControlsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.hideVideoControlsToolStripMenuItem.Text = "Hide video controls";
            this.hideVideoControlsToolStripMenuItem.Click += new System.EventHandler(this.HideVideoControlsToolStripMenuItemClick);
            // 
            // ShowSubtitleTimer
            // 
            this.ShowSubtitleTimer.Enabled = true;
            this.ShowSubtitleTimer.Interval = 17;
            this.ShowSubtitleTimer.Tick += new System.EventHandler(this.ShowSubtitleTimerTick);
            // 
            // timerAutoDuration
            // 
            this.timerAutoDuration.Interval = 300;
            this.timerAutoDuration.Tick += new System.EventHandler(this.TimerAutoDurationTick);
            // 
            // timerAutoContinue
            // 
            this.timerAutoContinue.Interval = 1000;
            this.timerAutoContinue.Tick += new System.EventHandler(this.TimerAutoContinueTick);
            // 
            // timerWaveform
            // 
            this.timerWaveform.Tick += new System.EventHandler(this.TimerWaveformTick);
            // 
            // contextMenuStripWaveform
            // 
            this.contextMenuStripWaveform.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParagraphHereToolStripMenuItem,
            this.addParagraphAndPasteToolStripMenuItem,
            this.toolStripMenuItemSetParagraphAsSelection,
            this.toolStripMenuItemFocusTextbox,
            this.goToPreviousSubtitleStripMenuItem,
            this.goToNextSubtitleStripMenuItem,
            this.deleteParagraphToolStripMenuItem,
            this.splitToolStripMenuItem1,
            this.mergeWithPreviousToolStripMenuItem,
            this.mergeWithNextToolStripMenuItem,
            this.runWhiperOnParagraphToolStripMenuItem,
            this.toolStripSeparator11,
            this.extendToPreviousToolStripMenuItem,
            this.extendToNextToolStripMenuItem,
            this.toolStripSeparator6,
            this.toolStripMenuItemWaveformPlaySelection,
            this.toolStripSeparator23,
            this.showWaveformAndSpectrogramToolStripMenuItem,
            this.showOnlyWaveformToolStripMenuItem,
            this.showOnlySpectrogramToolStripMenuItem,
            this.toolStripSeparatorGuessTimeCodes,
            this.removeShotChangeToolStripMenuItem,
            this.addShotChangeToolStripMenuItem,
            this.guessTimeCodesToolStripMenuItem,
            this.seekSilenceToolStripMenuItem,
            this.insertSubtitleHereToolStripMenuItem});
            this.contextMenuStripWaveform.Name = "contextMenuStripWaveform";
            this.contextMenuStripWaveform.Size = new System.Drawing.Size(306, 512);
            this.contextMenuStripWaveform.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.ContextMenuStripWaveformClosing);
            this.contextMenuStripWaveform.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripWaveformOpening);
            // 
            // addParagraphHereToolStripMenuItem
            // 
            this.addParagraphHereToolStripMenuItem.Name = "addParagraphHereToolStripMenuItem";
            this.addParagraphHereToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.addParagraphHereToolStripMenuItem.Text = "Add paragraph here";
            this.addParagraphHereToolStripMenuItem.Click += new System.EventHandler(this.AddParagraphHereToolStripMenuItemClick);
            // 
            // addParagraphAndPasteToolStripMenuItem
            // 
            this.addParagraphAndPasteToolStripMenuItem.Name = "addParagraphAndPasteToolStripMenuItem";
            this.addParagraphAndPasteToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.addParagraphAndPasteToolStripMenuItem.Text = "Add paragraph here + paste clipboard";
            this.addParagraphAndPasteToolStripMenuItem.Click += new System.EventHandler(this.AddParagraphAndPasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItemSetParagraphAsSelection
            // 
            this.toolStripMenuItemSetParagraphAsSelection.Name = "toolStripMenuItemSetParagraphAsSelection";
            this.toolStripMenuItemSetParagraphAsSelection.Size = new System.Drawing.Size(305, 22);
            this.toolStripMenuItemSetParagraphAsSelection.Text = "Set selected paragraph as selection";
            this.toolStripMenuItemSetParagraphAsSelection.Click += new System.EventHandler(this.ToolStripMenuItemSetParagraphAsSelectionClick);
            // 
            // toolStripMenuItemFocusTextbox
            // 
            this.toolStripMenuItemFocusTextbox.Name = "toolStripMenuItemFocusTextbox";
            this.toolStripMenuItemFocusTextbox.Size = new System.Drawing.Size(305, 22);
            this.toolStripMenuItemFocusTextbox.Text = "Focus textbox";
            this.toolStripMenuItemFocusTextbox.Click += new System.EventHandler(this.ToolStripMenuItemFocusTextboxClick);
            // 
            // goToPreviousSubtitleStripMenuItem
            // 
            this.goToPreviousSubtitleStripMenuItem.Name = "goToPreviousSubtitleStripMenuItem";
            this.goToPreviousSubtitleStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.goToPreviousSubtitleStripMenuItem.Text = "Go to previous subtitle";
            this.goToPreviousSubtitleStripMenuItem.Click += new System.EventHandler(this.GoToPreviousSubtitleToolStripMenuItemClick);
            // 
            // goToNextSubtitleStripMenuItem
            // 
            this.goToNextSubtitleStripMenuItem.Name = "goToNextSubtitleStripMenuItem";
            this.goToNextSubtitleStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.goToNextSubtitleStripMenuItem.Text = "Go to next subtitle";
            this.goToNextSubtitleStripMenuItem.Click += new System.EventHandler(this.GoToNextSubtitleToolStripMenuItem_Click);
            // 
            // deleteParagraphToolStripMenuItem
            // 
            this.deleteParagraphToolStripMenuItem.Name = "deleteParagraphToolStripMenuItem";
            this.deleteParagraphToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.deleteParagraphToolStripMenuItem.Text = "Delete paragraph";
            this.deleteParagraphToolStripMenuItem.Click += new System.EventHandler(this.DeleteParagraphToolStripMenuItemClick);
            // 
            // splitToolStripMenuItem1
            // 
            this.splitToolStripMenuItem1.Name = "splitToolStripMenuItem1";
            this.splitToolStripMenuItem1.Size = new System.Drawing.Size(305, 22);
            this.splitToolStripMenuItem1.Text = "Split";
            this.splitToolStripMenuItem1.Click += new System.EventHandler(this.SplitToolStripMenuItem1Click);
            // 
            // mergeWithPreviousToolStripMenuItem
            // 
            this.mergeWithPreviousToolStripMenuItem.Name = "mergeWithPreviousToolStripMenuItem";
            this.mergeWithPreviousToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.mergeWithPreviousToolStripMenuItem.Text = "Merge with previous";
            this.mergeWithPreviousToolStripMenuItem.Click += new System.EventHandler(this.MergeWithPreviousToolStripMenuItemClick);
            // 
            // mergeWithNextToolStripMenuItem
            // 
            this.mergeWithNextToolStripMenuItem.Name = "mergeWithNextToolStripMenuItem";
            this.mergeWithNextToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.mergeWithNextToolStripMenuItem.Text = "Merge with next";
            this.mergeWithNextToolStripMenuItem.Click += new System.EventHandler(this.MergeWithNextToolStripMenuItemClick);
            // 
            // runWhiperOnParagraphToolStripMenuItem
            // 
            this.runWhiperOnParagraphToolStripMenuItem.Name = "runWhiperOnParagraphToolStripMenuItem";
            this.runWhiperOnParagraphToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.runWhiperOnParagraphToolStripMenuItem.Text = "Run Whiper on paragraph...";
            this.runWhiperOnParagraphToolStripMenuItem.Click += new System.EventHandler(this.runWhiperOnParagraphToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(302, 6);
            // 
            // extendToPreviousToolStripMenuItem
            // 
            this.extendToPreviousToolStripMenuItem.Name = "extendToPreviousToolStripMenuItem";
            this.extendToPreviousToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.extendToPreviousToolStripMenuItem.Text = "Extend to previous";
            this.extendToPreviousToolStripMenuItem.Click += new System.EventHandler(this.ExtendToPreviousToolStripMenuItemClick);
            // 
            // extendToNextToolStripMenuItem
            // 
            this.extendToNextToolStripMenuItem.Name = "extendToNextToolStripMenuItem";
            this.extendToNextToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.extendToNextToolStripMenuItem.Text = "Extend to next";
            this.extendToNextToolStripMenuItem.Click += new System.EventHandler(this.ExtendToNextToolStripMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(302, 6);
            // 
            // toolStripMenuItemWaveformPlaySelection
            // 
            this.toolStripMenuItemWaveformPlaySelection.Name = "toolStripMenuItemWaveformPlaySelection";
            this.toolStripMenuItemWaveformPlaySelection.Size = new System.Drawing.Size(305, 22);
            this.toolStripMenuItemWaveformPlaySelection.Text = "Play selection";
            this.toolStripMenuItemWaveformPlaySelection.Click += new System.EventHandler(this.ToolStripMenuItemWaveformPlaySelectionClick);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(302, 6);
            // 
            // showWaveformAndSpectrogramToolStripMenuItem
            // 
            this.showWaveformAndSpectrogramToolStripMenuItem.Name = "showWaveformAndSpectrogramToolStripMenuItem";
            this.showWaveformAndSpectrogramToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.showWaveformAndSpectrogramToolStripMenuItem.Text = "Show waveform and spectrogram";
            this.showWaveformAndSpectrogramToolStripMenuItem.Click += new System.EventHandler(this.ShowWaveformAndSpectrogramToolStripMenuItemClick);
            // 
            // showOnlyWaveformToolStripMenuItem
            // 
            this.showOnlyWaveformToolStripMenuItem.Name = "showOnlyWaveformToolStripMenuItem";
            this.showOnlyWaveformToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.showOnlyWaveformToolStripMenuItem.Text = "Show only waveform";
            this.showOnlyWaveformToolStripMenuItem.Click += new System.EventHandler(this.ShowOnlyWaveformToolStripMenuItemClick);
            // 
            // showOnlySpectrogramToolStripMenuItem
            // 
            this.showOnlySpectrogramToolStripMenuItem.Name = "showOnlySpectrogramToolStripMenuItem";
            this.showOnlySpectrogramToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.showOnlySpectrogramToolStripMenuItem.Text = "Show only spectrogram";
            this.showOnlySpectrogramToolStripMenuItem.Click += new System.EventHandler(this.ShowOnlySpectrogramToolStripMenuItemClick);
            // 
            // toolStripSeparatorGuessTimeCodes
            // 
            this.toolStripSeparatorGuessTimeCodes.Name = "toolStripSeparatorGuessTimeCodes";
            this.toolStripSeparatorGuessTimeCodes.Size = new System.Drawing.Size(302, 6);
            // 
            // removeShotChangeToolStripMenuItem
            // 
            this.removeShotChangeToolStripMenuItem.Name = "removeShotChangeToolStripMenuItem";
            this.removeShotChangeToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.removeShotChangeToolStripMenuItem.Text = "Remove shot change";
            this.removeShotChangeToolStripMenuItem.Click += new System.EventHandler(this.RemoveShotChangeToolStripMenuItemClick);
            // 
            // addShotChangeToolStripMenuItem
            // 
            this.addShotChangeToolStripMenuItem.Name = "addShotChangeToolStripMenuItem";
            this.addShotChangeToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.addShotChangeToolStripMenuItem.Text = "Add shot change";
            this.addShotChangeToolStripMenuItem.Click += new System.EventHandler(this.AddShotChangeToolStripMenuItemClick);
            // 
            // guessTimeCodesToolStripMenuItem
            // 
            this.guessTimeCodesToolStripMenuItem.Name = "guessTimeCodesToolStripMenuItem";
            this.guessTimeCodesToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.guessTimeCodesToolStripMenuItem.Text = "Guess time codes...";
            this.guessTimeCodesToolStripMenuItem.Click += new System.EventHandler(this.GuessTimeCodesToolStripMenuItemClick);
            // 
            // seekSilenceToolStripMenuItem
            // 
            this.seekSilenceToolStripMenuItem.Name = "seekSilenceToolStripMenuItem";
            this.seekSilenceToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.seekSilenceToolStripMenuItem.Text = "Seek silence...";
            this.seekSilenceToolStripMenuItem.Click += new System.EventHandler(this.SeekSilenceToolStripMenuItemClick);
            // 
            // insertSubtitleHereToolStripMenuItem
            // 
            this.insertSubtitleHereToolStripMenuItem.Name = "insertSubtitleHereToolStripMenuItem";
            this.insertSubtitleHereToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.insertSubtitleHereToolStripMenuItem.Text = "Insert subtitle here...";
            this.insertSubtitleHereToolStripMenuItem.Click += new System.EventHandler(this.InsertSubtitleHereToolStripMenuItem_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainer1);
            this.splitContainerMain.Size = new System.Drawing.Size(975, 600);
            this.splitContainerMain.SplitterDistance = 291;
            this.splitContainerMain.TabIndex = 8;
            this.splitContainerMain.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.SplitContainerMainSplitterMoving);
            this.splitContainerMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerMainSplitterMoved);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerListViewAndText);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxSource);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelVideoPlayer);
            this.splitContainer1.Size = new System.Drawing.Size(975, 291);
            this.splitContainer1.SplitterDistance = 743;
            this.splitContainer1.TabIndex = 7;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainer1SplitterMoved);
            // 
            // splitContainerListViewAndText
            // 
            this.splitContainerListViewAndText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerListViewAndText.Location = new System.Drawing.Point(3, 0);
            this.splitContainerListViewAndText.Name = "splitContainerListViewAndText";
            this.splitContainerListViewAndText.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerListViewAndText.Panel1
            // 
            this.splitContainerListViewAndText.Panel1.Controls.Add(this.SubtitleListview1);
            this.splitContainerListViewAndText.Panel1MinSize = 50;
            // 
            // splitContainerListViewAndText.Panel2
            // 
            this.splitContainerListViewAndText.Panel2.Controls.Add(this.groupBoxEdit);
            this.splitContainerListViewAndText.Panel2MinSize = 105;
            this.splitContainerListViewAndText.Size = new System.Drawing.Size(740, 291);
            this.splitContainerListViewAndText.SplitterDistance = 121;
            this.splitContainerListViewAndText.TabIndex = 2;
            this.splitContainerListViewAndText.VisibleChanged += new System.EventHandler(this.ListViewVisibleChanged);
            // 
            // SubtitleListview1
            // 
            this.SubtitleListview1.AllowColumnReorder = true;
            this.SubtitleListview1.AllowDrop = true;
            this.SubtitleListview1.ContextMenuStrip = this.contextMenuStripListView;
            this.SubtitleListview1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SubtitleListview1.FirstVisibleIndex = -1;
            this.SubtitleListview1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubtitleListview1.FullRowSelect = true;
            this.SubtitleListview1.GridLines = true;
            this.SubtitleListview1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SubtitleListview1.HideSelection = false;
            this.SubtitleListview1.Location = new System.Drawing.Point(0, 0);
            this.SubtitleListview1.Name = "SubtitleListview1";
            this.SubtitleListview1.OwnerDraw = true;
            this.SubtitleListview1.Size = new System.Drawing.Size(740, 121);
            this.SubtitleListview1.StateImageList = this.imageListBookmarks;
            this.SubtitleListview1.SubtitleFontBold = false;
            this.SubtitleListview1.SubtitleFontName = "Tahoma";
            this.SubtitleListview1.SubtitleFontSize = 8;
            this.SubtitleListview1.TabIndex = 0;
            this.SubtitleListview1.UseCompatibleStateImageBehavior = false;
            this.SubtitleListview1.UseSyntaxColoring = true;
            this.SubtitleListview1.View = System.Windows.Forms.View.Details;
            this.SubtitleListview1.SelectedIndexChanged += new System.EventHandler(this.SubtitleListview1_SelectedIndexChanged);
            this.SubtitleListview1.DragDrop += new System.Windows.Forms.DragEventHandler(this.SubtitleListview1_DragDrop);
            this.SubtitleListview1.DragEnter += new System.Windows.Forms.DragEventHandler(this.SubtitleListview1_DragEnter);
            this.SubtitleListview1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubtitleListview1KeyDown);
            this.SubtitleListview1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SubtitleListview1_MouseDoubleClick);
            this.SubtitleListview1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SubtitleListview1_MouseDown);
            this.SubtitleListview1.MouseEnter += new System.EventHandler(this.SubtitleListview1_MouseEnter);
            this.SubtitleListview1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SubtitleListview1_MouseUp);
            // 
            // imageListBookmarks
            // 
            this.imageListBookmarks.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListBookmarks.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListBookmarks.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // groupBoxEdit
            // 
            this.groupBoxEdit.Controls.Add(this.labelOriginalSingleLinePixels);
            this.groupBoxEdit.Controls.Add(this.labelOriginalSingleLine);
            this.groupBoxEdit.Controls.Add(this.labelTextOriginalLineLengths);
            this.groupBoxEdit.Controls.Add(this.labelOriginalText);
            this.groupBoxEdit.Controls.Add(this.textBoxListViewTextOriginal);
            this.groupBoxEdit.Controls.Add(this.labelTextLineTotal);
            this.groupBoxEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxEdit.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEdit.Name = "groupBoxEdit";
            this.groupBoxEdit.Size = new System.Drawing.Size(740, 166);
            this.groupBoxEdit.TabIndex = 1;
            this.groupBoxEdit.TabStop = false;
            // 
            // contextMenuStripTextBoxListView
            // 
            this.contextMenuStripTextBoxListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparatorSpellCheckSuggestions,
            this.toolStripMenuItemSpellCheckSkipOnce,
            this.toolStripMenuItemSpellCheckSkipAll,
            this.toolStripMenuItemSpellCheckAddToDictionary,
            this.toolStripMenuItemSpellCheckAddToNames,
            this.toolStripSeparatorSpellCheck,
            this.toolStripMenuItemWebVttVoice,
            this.toolStripSeparatorWebVTT,
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1,
            this.deleteToolStripMenuItem1,
            this.toolStripMenuItemSplitViaWaveform,
            this.toolStripMenuItemSplitTextAtCursor,
            this.toolStripSeparator15,
            this.selectAllToolStripMenuItem1,
            this.toolStripSeparator13,
            this.normalToolStripMenuItem1,
            this.boldToolStripMenuItem1,
            this.italicToolStripMenuItem1,
            this.boxToolStripMenuItem1,
            this.underlineToolStripMenuItem1,
            this.colorToolStripMenuItem1,
            this.toolStripMenuItemHorizontalDigits,
            this.toolStripMenuItemBouten,
            this.toolStripMenuItemRuby,
            this.fontNameToolStripMenuItem,
            this.superscriptToolStripMenuItem,
            this.subscriptToolStripMenuItem,
            this.toolStripSeparatorInsertUnicode,
            this.insertUnicodeCharactersToolStripMenuItem,
            this.insertUnicodeControlCharactersToolStripMenuItem});
            this.contextMenuStripTextBoxListView.Name = "contextMenuStripTextBoxListView";
            this.contextMenuStripTextBoxListView.Size = new System.Drawing.Size(296, 612);
            this.contextMenuStripTextBoxListView.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.MenuClosed);
            this.contextMenuStripTextBoxListView.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.ContextMenuStripTextBoxListViewClosing);
            this.contextMenuStripTextBoxListView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripTextBoxListViewOpening);
            this.contextMenuStripTextBoxListView.Opened += new System.EventHandler(this.MenuOpened);
            // 
            // toolStripSeparatorSpellCheckSuggestions
            // 
            this.toolStripSeparatorSpellCheckSuggestions.Name = "toolStripSeparatorSpellCheckSuggestions";
            this.toolStripSeparatorSpellCheckSuggestions.Size = new System.Drawing.Size(292, 6);
            this.toolStripSeparatorSpellCheckSuggestions.Visible = false;
            // 
            // toolStripMenuItemSpellCheckSkipOnce
            // 
            this.toolStripMenuItemSpellCheckSkipOnce.Name = "toolStripMenuItemSpellCheckSkipOnce";
            this.toolStripMenuItemSpellCheckSkipOnce.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSpellCheckSkipOnce.Text = "Skip once";
            this.toolStripMenuItemSpellCheckSkipOnce.Visible = false;
            this.toolStripMenuItemSpellCheckSkipOnce.Click += new System.EventHandler(this.ToolStripMenuItemSpellCheckSkipOnceClick);
            // 
            // toolStripMenuItemSpellCheckSkipAll
            // 
            this.toolStripMenuItemSpellCheckSkipAll.Name = "toolStripMenuItemSpellCheckSkipAll";
            this.toolStripMenuItemSpellCheckSkipAll.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSpellCheckSkipAll.Text = "Skip all";
            this.toolStripMenuItemSpellCheckSkipAll.Visible = false;
            this.toolStripMenuItemSpellCheckSkipAll.Click += new System.EventHandler(this.ToolStripMenuItemSpellCheckSkipAllClick);
            // 
            // toolStripMenuItemSpellCheckAddToDictionary
            // 
            this.toolStripMenuItemSpellCheckAddToDictionary.Name = "toolStripMenuItemSpellCheckAddToDictionary";
            this.toolStripMenuItemSpellCheckAddToDictionary.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSpellCheckAddToDictionary.Text = "Add to dictionary";
            this.toolStripMenuItemSpellCheckAddToDictionary.Visible = false;
            this.toolStripMenuItemSpellCheckAddToDictionary.Click += new System.EventHandler(this.ToolStripMenuItemSpellCheckAddToDictionaryClick);
            // 
            // toolStripMenuItemSpellCheckAddToNames
            // 
            this.toolStripMenuItemSpellCheckAddToNames.Name = "toolStripMenuItemSpellCheckAddToNames";
            this.toolStripMenuItemSpellCheckAddToNames.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSpellCheckAddToNames.Text = "Add to names";
            this.toolStripMenuItemSpellCheckAddToNames.Visible = false;
            this.toolStripMenuItemSpellCheckAddToNames.Click += new System.EventHandler(this.ToolStripMenuItemSpellCheckAddToNamesClick);
            // 
            // toolStripSeparatorSpellCheck
            // 
            this.toolStripSeparatorSpellCheck.Name = "toolStripSeparatorSpellCheck";
            this.toolStripSeparatorSpellCheck.Size = new System.Drawing.Size(292, 6);
            this.toolStripSeparatorSpellCheck.Visible = false;
            // 
            // toolStripMenuItemWebVttVoice
            // 
            this.toolStripMenuItemWebVttVoice.Name = "toolStripMenuItemWebVttVoice";
            this.toolStripMenuItemWebVttVoice.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemWebVttVoice.Text = "WebVTT voice";
            // 
            // toolStripSeparatorWebVTT
            // 
            this.toolStripSeparatorWebVTT.Name = "toolStripSeparatorWebVTT";
            this.toolStripSeparatorWebVTT.Size = new System.Drawing.Size(292, 6);
            // 
            // cutToolStripMenuItem1
            // 
            this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            this.cutToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.cutToolStripMenuItem1.Text = "Cut";
            this.cutToolStripMenuItem1.Click += new System.EventHandler(this.CutToolStripMenuItemClick);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.pasteToolStripMenuItem1.Text = "Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.PasteToolStripMenuItemClick);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // toolStripMenuItemSplitViaWaveform
            // 
            this.toolStripMenuItemSplitViaWaveform.Name = "toolStripMenuItemSplitViaWaveform";
            this.toolStripMenuItemSplitViaWaveform.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSplitViaWaveform.Text = "Split text at cursor/waveform position";
            this.toolStripMenuItemSplitViaWaveform.Click += new System.EventHandler(this.ToolStripMenuItemSplitViaWaveformClick);
            // 
            // toolStripMenuItemSplitTextAtCursor
            // 
            this.toolStripMenuItemSplitTextAtCursor.Name = "toolStripMenuItemSplitTextAtCursor";
            this.toolStripMenuItemSplitTextAtCursor.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemSplitTextAtCursor.Text = "Split text at cursor position";
            this.toolStripMenuItemSplitTextAtCursor.Click += new System.EventHandler(this.ToolStripMenuItemSplitTextAtCursorClick);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(292, 6);
            // 
            // selectAllToolStripMenuItem1
            // 
            this.selectAllToolStripMenuItem1.Name = "selectAllToolStripMenuItem1";
            this.selectAllToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.selectAllToolStripMenuItem1.Text = "Select all";
            this.selectAllToolStripMenuItem1.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(292, 6);
            // 
            // normalToolStripMenuItem1
            // 
            this.normalToolStripMenuItem1.Name = "normalToolStripMenuItem1";
            this.normalToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.normalToolStripMenuItem1.Text = "Normal";
            this.normalToolStripMenuItem1.Click += new System.EventHandler(this.NormalToolStripMenuItem1Click);
            // 
            // boldToolStripMenuItem1
            // 
            this.boldToolStripMenuItem1.Name = "boldToolStripMenuItem1";
            this.boldToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.boldToolStripMenuItem1.Text = "Bold";
            this.boldToolStripMenuItem1.Click += new System.EventHandler(this.BoldToolStripMenuItem1Click);
            // 
            // italicToolStripMenuItem1
            // 
            this.italicToolStripMenuItem1.Name = "italicToolStripMenuItem1";
            this.italicToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.italicToolStripMenuItem1.Text = "Italic";
            this.italicToolStripMenuItem1.Click += new System.EventHandler(this.ItalicToolStripMenuItem1Click);
            // 
            // boxToolStripMenuItem1
            // 
            this.boxToolStripMenuItem1.Name = "boxToolStripMenuItem1";
            this.boxToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.boxToolStripMenuItem1.Text = "Box";
            this.boxToolStripMenuItem1.Click += new System.EventHandler(this.BoxToolStripMenuItem1Click);
            // 
            // underlineToolStripMenuItem1
            // 
            this.underlineToolStripMenuItem1.Name = "underlineToolStripMenuItem1";
            this.underlineToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.underlineToolStripMenuItem1.Text = "Underline";
            this.underlineToolStripMenuItem1.Click += new System.EventHandler(this.UnderlineToolStripMenuItem1Click);
            // 
            // colorToolStripMenuItem1
            // 
            this.colorToolStripMenuItem1.Name = "colorToolStripMenuItem1";
            this.colorToolStripMenuItem1.Size = new System.Drawing.Size(295, 22);
            this.colorToolStripMenuItem1.Text = "Color...";
            this.colorToolStripMenuItem1.Click += new System.EventHandler(this.ColorToolStripMenuItem1Click);
            // 
            // toolStripMenuItemHorizontalDigits
            // 
            this.toolStripMenuItemHorizontalDigits.Name = "toolStripMenuItemHorizontalDigits";
            this.toolStripMenuItemHorizontalDigits.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemHorizontalDigits.Text = "Horizontal digits";
            this.toolStripMenuItemHorizontalDigits.Click += new System.EventHandler(this.ToolStripMenuItemHorizontalDigitsClick);
            // 
            // toolStripMenuItemBouten
            // 
            this.toolStripMenuItemBouten.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boutendotbeforeToolStripMenuItem,
            this.boutendotafterToolStripMenuItem,
            this.boutendotoutsideToolStripMenuItem,
            this.boutenfilledcircleoutsideToolStripMenuItem,
            this.boutenopencircleoutsideToolStripMenuItem,
            this.boutenopendotoutsideToolStripMenuItem,
            this.boutenfilledsesameoutsideToolStripMenuItem,
            this.boutenopensesameoutsideToolStripMenuItem,
            this.boutenautooutsideToolStripMenuItem,
            this.boutenautoToolStripMenuItem});
            this.toolStripMenuItemBouten.Name = "toolStripMenuItemBouten";
            this.toolStripMenuItemBouten.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemBouten.Text = "Bouten";
            // 
            // boutendotbeforeToolStripMenuItem
            // 
            this.boutendotbeforeToolStripMenuItem.Name = "boutendotbeforeToolStripMenuItem";
            this.boutendotbeforeToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutendotbeforeToolStripMenuItem.Text = "bouten-dot-before";
            this.boutendotbeforeToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutendotafterToolStripMenuItem
            // 
            this.boutendotafterToolStripMenuItem.Name = "boutendotafterToolStripMenuItem";
            this.boutendotafterToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutendotafterToolStripMenuItem.Text = "bouten-dot-after";
            this.boutendotafterToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutendotoutsideToolStripMenuItem
            // 
            this.boutendotoutsideToolStripMenuItem.Name = "boutendotoutsideToolStripMenuItem";
            this.boutendotoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutendotoutsideToolStripMenuItem.Text = "bouten-dot-outside";
            this.boutendotoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenfilledcircleoutsideToolStripMenuItem
            // 
            this.boutenfilledcircleoutsideToolStripMenuItem.Name = "boutenfilledcircleoutsideToolStripMenuItem";
            this.boutenfilledcircleoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenfilledcircleoutsideToolStripMenuItem.Text = "bouten-filled-circle-outside";
            this.boutenfilledcircleoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenopencircleoutsideToolStripMenuItem
            // 
            this.boutenopencircleoutsideToolStripMenuItem.Name = "boutenopencircleoutsideToolStripMenuItem";
            this.boutenopencircleoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenopencircleoutsideToolStripMenuItem.Text = "bouten-open-circle-outside";
            this.boutenopencircleoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenopendotoutsideToolStripMenuItem
            // 
            this.boutenopendotoutsideToolStripMenuItem.Name = "boutenopendotoutsideToolStripMenuItem";
            this.boutenopendotoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenopendotoutsideToolStripMenuItem.Text = "bouten-open-dot-outside";
            this.boutenopendotoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenfilledsesameoutsideToolStripMenuItem
            // 
            this.boutenfilledsesameoutsideToolStripMenuItem.Name = "boutenfilledsesameoutsideToolStripMenuItem";
            this.boutenfilledsesameoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenfilledsesameoutsideToolStripMenuItem.Text = "bouten-filled-sesame-outside";
            this.boutenfilledsesameoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenopensesameoutsideToolStripMenuItem
            // 
            this.boutenopensesameoutsideToolStripMenuItem.Name = "boutenopensesameoutsideToolStripMenuItem";
            this.boutenopensesameoutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenopensesameoutsideToolStripMenuItem.Text = "bouten-open-sesame-outside";
            this.boutenopensesameoutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenautooutsideToolStripMenuItem
            // 
            this.boutenautooutsideToolStripMenuItem.Name = "boutenautooutsideToolStripMenuItem";
            this.boutenautooutsideToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenautooutsideToolStripMenuItem.Text = "bouten-auto-outside";
            this.boutenautooutsideToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // boutenautoToolStripMenuItem
            // 
            this.boutenautoToolStripMenuItem.Name = "boutenautoToolStripMenuItem";
            this.boutenautoToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.boutenautoToolStripMenuItem.Text = "bouten-auto";
            this.boutenautoToolStripMenuItem.Click += new System.EventHandler(this.BoutenToolStripMenuItemClick);
            // 
            // toolStripMenuItemRuby
            // 
            this.toolStripMenuItemRuby.Name = "toolStripMenuItemRuby";
            this.toolStripMenuItemRuby.Size = new System.Drawing.Size(295, 22);
            this.toolStripMenuItemRuby.Text = "Ruby...";
            this.toolStripMenuItemRuby.Click += new System.EventHandler(this.ToolStripMenuItemRubyClick);
            // 
            // fontNameToolStripMenuItem
            // 
            this.fontNameToolStripMenuItem.Name = "fontNameToolStripMenuItem";
            this.fontNameToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.fontNameToolStripMenuItem.Text = "Font name...";
            this.fontNameToolStripMenuItem.Click += new System.EventHandler(this.FontNameToolStripMenuItemClick);
            // 
            // superscriptToolStripMenuItem
            // 
            this.superscriptToolStripMenuItem.Name = "superscriptToolStripMenuItem";
            this.superscriptToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.superscriptToolStripMenuItem.Text = "Superscript";
            this.superscriptToolStripMenuItem.Click += new System.EventHandler(this.SuperscriptToolStripMenuItemClick);
            // 
            // subscriptToolStripMenuItem
            // 
            this.subscriptToolStripMenuItem.Name = "subscriptToolStripMenuItem";
            this.subscriptToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.subscriptToolStripMenuItem.Text = "Subscript";
            this.subscriptToolStripMenuItem.Click += new System.EventHandler(this.SubscriptToolStripMenuItemClick);
            // 
            // toolStripSeparatorInsertUnicode
            // 
            this.toolStripSeparatorInsertUnicode.Name = "toolStripSeparatorInsertUnicode";
            this.toolStripSeparatorInsertUnicode.Size = new System.Drawing.Size(292, 6);
            // 
            // insertUnicodeCharactersToolStripMenuItem
            // 
            this.insertUnicodeCharactersToolStripMenuItem.Name = "insertUnicodeCharactersToolStripMenuItem";
            this.insertUnicodeCharactersToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.insertUnicodeCharactersToolStripMenuItem.Text = "Insert Unicode characters";
            // 
            // insertUnicodeControlCharactersToolStripMenuItem
            // 
            this.insertUnicodeControlCharactersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lefttorightMarkLRMToolStripMenuItem,
            this.righttoleftMarkRLMToolStripMenuItem,
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1,
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1,
            this.startOfLefttorightOverrideLROToolStripMenuItem1,
            this.startOfRighttoleftOverrideRLOToolStripMenuItem1});
            this.insertUnicodeControlCharactersToolStripMenuItem.Name = "insertUnicodeControlCharactersToolStripMenuItem";
            this.insertUnicodeControlCharactersToolStripMenuItem.Size = new System.Drawing.Size(295, 22);
            this.insertUnicodeControlCharactersToolStripMenuItem.Text = "Insert Unicode control characters";
            // 
            // lefttorightMarkLRMToolStripMenuItem
            // 
            this.lefttorightMarkLRMToolStripMenuItem.Name = "lefttorightMarkLRMToolStripMenuItem";
            this.lefttorightMarkLRMToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.lefttorightMarkLRMToolStripMenuItem.Text = "Left-to-right mark (LRM)";
            this.lefttorightMarkLRMToolStripMenuItem.Click += new System.EventHandler(this.LeftToolStripMenuItemClick);
            // 
            // righttoleftMarkRLMToolStripMenuItem
            // 
            this.righttoleftMarkRLMToolStripMenuItem.Name = "righttoleftMarkRLMToolStripMenuItem";
            this.righttoleftMarkRLMToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.righttoleftMarkRLMToolStripMenuItem.Text = "Right-to-left mark (RLM)";
            this.righttoleftMarkRLMToolStripMenuItem.Click += new System.EventHandler(this.RighttoleftMarkToolStripMenuItemClick);
            // 
            // startOfLefttorightEmbeddingLREToolStripMenuItem1
            // 
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1.Name = "startOfLefttorightEmbeddingLREToolStripMenuItem1";
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1.Size = new System.Drawing.Size(294, 22);
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1.Text = "Start of left-to-right embedding (LRE)";
            this.startOfLefttorightEmbeddingLREToolStripMenuItem1.Click += new System.EventHandler(this.StartOfLefttorightEmbeddingLREToolStripMenuItemClick);
            // 
            // startOfRighttoleftEmbeddingRLEToolStripMenuItem1
            // 
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1.Name = "startOfRighttoleftEmbeddingRLEToolStripMenuItem1";
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1.Size = new System.Drawing.Size(294, 22);
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1.Text = "Start of right-to-left embedding (RLE)";
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem1.Click += new System.EventHandler(this.StartOfRighttoleftEmbeddingRLEToolStripMenuItemClick);
            // 
            // startOfLefttorightOverrideLROToolStripMenuItem1
            // 
            this.startOfLefttorightOverrideLROToolStripMenuItem1.Name = "startOfLefttorightOverrideLROToolStripMenuItem1";
            this.startOfLefttorightOverrideLROToolStripMenuItem1.Size = new System.Drawing.Size(294, 22);
            this.startOfLefttorightOverrideLROToolStripMenuItem1.Text = "Start of left-to-right override (LRO)";
            this.startOfLefttorightOverrideLROToolStripMenuItem1.Click += new System.EventHandler(this.StartOfLefttorightOverrideLROToolStripMenuItem_Click);
            // 
            // startOfRighttoleftOverrideRLOToolStripMenuItem1
            // 
            this.startOfRighttoleftOverrideRLOToolStripMenuItem1.Name = "startOfRighttoleftOverrideRLOToolStripMenuItem1";
            this.startOfRighttoleftOverrideRLOToolStripMenuItem1.Size = new System.Drawing.Size(294, 22);
            this.startOfRighttoleftOverrideRLOToolStripMenuItem1.Text = "Start of right-to-left override (RLO)";
            // 
            // labelOriginalSingleLinePixels
            // 
            this.labelOriginalSingleLinePixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOriginalSingleLinePixels.AutoSize = true;
            this.labelOriginalSingleLinePixels.Location = new System.Drawing.Point(870, 139);
            this.labelOriginalSingleLinePixels.Name = "labelOriginalSingleLinePixels";
            this.labelOriginalSingleLinePixels.Size = new System.Drawing.Size(62, 13);
            this.labelOriginalSingleLinePixels.TabIndex = 43;
            this.labelOriginalSingleLinePixels.Text = "AltSinLinPix";
            // 
            // labelOriginalSingleLine
            // 
            this.labelOriginalSingleLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOriginalSingleLine.AutoSize = true;
            this.labelOriginalSingleLine.Location = new System.Drawing.Point(839, 139);
            this.labelOriginalSingleLine.Name = "labelOriginalSingleLine";
            this.labelOriginalSingleLine.Size = new System.Drawing.Size(48, 13);
            this.labelOriginalSingleLine.TabIndex = 36;
            this.labelOriginalSingleLine.Text = "AltSinLin";
            // 
            // labelTextOriginalLineLengths
            // 
            this.labelTextOriginalLineLengths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTextOriginalLineLengths.AutoSize = true;
            this.labelTextOriginalLineLengths.Location = new System.Drawing.Point(786, 139);
            this.labelTextOriginalLineLengths.Name = "labelTextOriginalLineLengths";
            this.labelTextOriginalLineLengths.Size = new System.Drawing.Size(57, 13);
            this.labelTextOriginalLineLengths.TabIndex = 35;
            this.labelTextOriginalLineLengths.Text = "AltLineLen";
            // 
            // labelOriginalText
            // 
            this.labelOriginalText.AutoSize = true;
            this.labelOriginalText.Location = new System.Drawing.Point(803, 11);
            this.labelOriginalText.Name = "labelOriginalText";
            this.labelOriginalText.Size = new System.Drawing.Size(28, 13);
            this.labelOriginalText.TabIndex = 34;
            this.labelOriginalText.Text = "Text";
            this.labelOriginalText.Visible = false;
            // 
            // textBoxListViewTextOriginal
            // 
            this.textBoxListViewTextOriginal.AllowDrop = true;
            this.textBoxListViewTextOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxListViewTextOriginal.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxListViewTextOriginal.ContextMenuStrip = this.contextMenuStripTextBoxListView;
            this.textBoxListViewTextOriginal.CurrentLanguage = "";
            this.textBoxListViewTextOriginal.CurrentLineIndex = 0;
            this.textBoxListViewTextOriginal.Enabled = false;
            this.textBoxListViewTextOriginal.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.textBoxListViewTextOriginal.HideSelection = false;
            this.textBoxListViewTextOriginal.IsDictionaryDownloaded = true;
            this.textBoxListViewTextOriginal.IsSpellCheckerInitialized = false;
            this.textBoxListViewTextOriginal.IsSpellCheckRequested = false;
            this.textBoxListViewTextOriginal.IsWrongWord = false;
            this.textBoxListViewTextOriginal.LanguageChanged = false;
            this.textBoxListViewTextOriginal.Location = new System.Drawing.Point(946, 28);
            this.textBoxListViewTextOriginal.MaxLength = 32767;
            this.textBoxListViewTextOriginal.Multiline = true;
            this.textBoxListViewTextOriginal.Name = "textBoxListViewTextOriginal";
            this.textBoxListViewTextOriginal.Padding = new System.Windows.Forms.Padding(1);
            this.textBoxListViewTextOriginal.ReadOnly = false;
            this.textBoxListViewTextOriginal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.textBoxListViewTextOriginal.SelectedText = "";
            this.textBoxListViewTextOriginal.SelectionLength = 0;
            this.textBoxListViewTextOriginal.SelectionStart = 0;
            this.textBoxListViewTextOriginal.Size = new System.Drawing.Size(16, 108);
            this.textBoxListViewTextOriginal.TabIndex = 33;
            this.textBoxListViewTextOriginal.TextBoxFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.textBoxListViewTextOriginal.UseSystemPasswordChar = false;
            this.textBoxListViewTextOriginal.Visible = false;
            this.textBoxListViewTextOriginal.TextChanged += new System.EventHandler(this.textBoxListViewTextOriginal_TextChanged);
            this.textBoxListViewTextOriginal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxListViewTextOriginalKeyDown);
            this.textBoxListViewTextOriginal.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxListViewTextOriginalMouseClick);
            this.textBoxListViewTextOriginal.Enter += new System.EventHandler(this.TextBoxListViewTextOriginalEnter);
            this.textBoxListViewTextOriginal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxListViewTextOriginalKeyUp);
            this.textBoxListViewTextOriginal.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextBoxListViewTextOriginalMouseMove);
            // 
            // labelTextLineTotal
            // 
            this.labelTextLineTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTextLineTotal.AutoSize = true;
            this.labelTextLineTotal.Location = new System.Drawing.Point(1001, 139);
            this.labelTextLineTotal.Name = "labelTextLineTotal";
            this.labelTextLineTotal.Size = new System.Drawing.Size(94, 13);
            this.labelTextLineTotal.TabIndex = 21;
            this.labelTextLineTotal.Text = "labelTextLineTotal";
            // 
            // textBoxSource
            // 
            this.textBoxSource.AllowDrop = true;
            this.textBoxSource.ContextMenuStrip = this.contextMenuStripTextBoxSourceView;
            this.textBoxSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSource.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.textBoxSource.HideSelection = false;
            this.textBoxSource.Location = new System.Drawing.Point(3, 0);
            this.textBoxSource.MaxLength = 0;
            this.textBoxSource.Multiline = true;
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSource.Size = new System.Drawing.Size(740, 291);
            this.textBoxSource.TabIndex = 12;
            this.textBoxSource.WordWrap = false;
            this.textBoxSource.Click += new System.EventHandler(this.TextBoxSourceClick);
            this.textBoxSource.TextChanged += new System.EventHandler(this.TextBoxSourceTextChanged);
            this.textBoxSource.VisibleChanged += new System.EventHandler(this.SourceViewVisibleChanged);
            this.textBoxSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextBoxSourceDragDrop);
            this.textBoxSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextBoxSourceDragEnter);
            this.textBoxSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSourceKeyDown);
            this.textBoxSource.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxSource_KeyUp);
            this.textBoxSource.Leave += new System.EventHandler(this.TextBoxSourceLeave);
            // 
            // contextMenuStripTextBoxSourceView
            // 
            this.contextMenuStripTextBoxSourceView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemGoToListView,
            this.toolStripSeparator18,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator17,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator25,
            this.toolStripMenuItemInsertUnicodeSymbol,
            this.toolStripMenuItemInsertUnicodeControlCharacters});
            this.contextMenuStripTextBoxSourceView.Name = "contextMenuStripTextBoxSourceView";
            this.contextMenuStripTextBoxSourceView.Size = new System.Drawing.Size(263, 198);
            this.contextMenuStripTextBoxSourceView.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.MenuClosed);
            this.contextMenuStripTextBoxSourceView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripTextBoxListViewOpening);
            this.contextMenuStripTextBoxSourceView.Opened += new System.EventHandler(this.MenuOpened);
            // 
            // toolStripMenuItemGoToListView
            // 
            this.toolStripMenuItemGoToListView.Name = "toolStripMenuItemGoToListView";
            this.toolStripMenuItemGoToListView.Size = new System.Drawing.Size(262, 22);
            this.toolStripMenuItemGoToListView.Text = "Go to list view";
            this.toolStripMenuItemGoToListView.Click += new System.EventHandler(this.ToolStripMenuItemGoToListViewClick);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(259, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItemClick);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItemClick);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(259, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(259, 6);
            // 
            // toolStripMenuItemInsertUnicodeSymbol
            // 
            this.toolStripMenuItemInsertUnicodeSymbol.Name = "toolStripMenuItemInsertUnicodeSymbol";
            this.toolStripMenuItemInsertUnicodeSymbol.Size = new System.Drawing.Size(262, 22);
            this.toolStripMenuItemInsertUnicodeSymbol.Text = "Insert unicode character";
            // 
            // toolStripMenuItemInsertUnicodeControlCharacters
            // 
            this.toolStripMenuItemInsertUnicodeControlCharacters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftToolStripMenuItem,
            this.righttoleftMarkToolStripMenuItem,
            this.startOfLefttorightEmbeddingLREToolStripMenuItem,
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem,
            this.startOfLefttorightOverrideLROToolStripMenuItem,
            this.startOfRighttoleftOverrideRLOToolStripMenuItem});
            this.toolStripMenuItemInsertUnicodeControlCharacters.Name = "toolStripMenuItemInsertUnicodeControlCharacters";
            this.toolStripMenuItemInsertUnicodeControlCharacters.Size = new System.Drawing.Size(262, 22);
            this.toolStripMenuItemInsertUnicodeControlCharacters.Text = "Insert unicode control character";
            // 
            // leftToolStripMenuItem
            // 
            this.leftToolStripMenuItem.Name = "leftToolStripMenuItem";
            this.leftToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.leftToolStripMenuItem.Text = "Left-to-right mark (LRM)";
            this.leftToolStripMenuItem.Click += new System.EventHandler(this.LeftToolStripMenuItemClick);
            // 
            // righttoleftMarkToolStripMenuItem
            // 
            this.righttoleftMarkToolStripMenuItem.Name = "righttoleftMarkToolStripMenuItem";
            this.righttoleftMarkToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.righttoleftMarkToolStripMenuItem.Text = "Right-to-left mark (RLM)";
            this.righttoleftMarkToolStripMenuItem.Click += new System.EventHandler(this.RighttoleftMarkToolStripMenuItemClick);
            // 
            // startOfLefttorightEmbeddingLREToolStripMenuItem
            // 
            this.startOfLefttorightEmbeddingLREToolStripMenuItem.Name = "startOfLefttorightEmbeddingLREToolStripMenuItem";
            this.startOfLefttorightEmbeddingLREToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.startOfLefttorightEmbeddingLREToolStripMenuItem.Text = "Start of left-to-right embedding (LRE)";
            this.startOfLefttorightEmbeddingLREToolStripMenuItem.Click += new System.EventHandler(this.StartOfLefttorightEmbeddingLREToolStripMenuItemClick);
            // 
            // startOfRighttoleftEmbeddingRLEToolStripMenuItem
            // 
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem.Name = "startOfRighttoleftEmbeddingRLEToolStripMenuItem";
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem.Text = "Start of right-to-left embedding (RLE)";
            this.startOfRighttoleftEmbeddingRLEToolStripMenuItem.Click += new System.EventHandler(this.StartOfRighttoleftEmbeddingRLEToolStripMenuItemClick);
            // 
            // startOfLefttorightOverrideLROToolStripMenuItem
            // 
            this.startOfLefttorightOverrideLROToolStripMenuItem.Name = "startOfLefttorightOverrideLROToolStripMenuItem";
            this.startOfLefttorightOverrideLROToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.startOfLefttorightOverrideLROToolStripMenuItem.Text = "Start of left-to-right override (LRO)";
            this.startOfLefttorightOverrideLROToolStripMenuItem.Click += new System.EventHandler(this.StartOfLefttorightOverrideLROToolStripMenuItem_Click);
            // 
            // startOfRighttoleftOverrideRLOToolStripMenuItem
            // 
            this.startOfRighttoleftOverrideRLOToolStripMenuItem.Name = "startOfRighttoleftOverrideRLOToolStripMenuItem";
            this.startOfRighttoleftOverrideRLOToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.startOfRighttoleftOverrideRLOToolStripMenuItem.Text = "Start of right-to-left override (RLO)";
            this.startOfRighttoleftOverrideRLOToolStripMenuItem.Click += new System.EventHandler(this.StartOfRighttoleftOverrideRLOToolStripMenuItemClick);
            // 
            // panelVideoPlayer
            // 
            this.panelVideoPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideoPlayer.Location = new System.Drawing.Point(1, 1);
            this.panelVideoPlayer.Name = "panelVideoPlayer";
            this.panelVideoPlayer.Size = new System.Drawing.Size(220, 286);
            this.panelVideoPlayer.TabIndex = 5;
            // 
            // contextMenuStripEmpty
            // 
            this.contextMenuStripEmpty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertLineToolStripMenuItem,
            this.toolStripMenuItemEmptyGoToSourceView,
            this.aSSStylesToolStripMenuItem});
            this.contextMenuStripEmpty.Name = "contextMenuStripEmpty";
            this.contextMenuStripEmpty.Size = new System.Drawing.Size(182, 70);
            this.contextMenuStripEmpty.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripEmptyOpening);
            // 
            // insertLineToolStripMenuItem
            // 
            this.insertLineToolStripMenuItem.Name = "insertLineToolStripMenuItem";
            this.insertLineToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.insertLineToolStripMenuItem.Text = "Insert line";
            this.insertLineToolStripMenuItem.Click += new System.EventHandler(this.InsertLineToolStripMenuItemClick);
            // 
            // aSSStylesToolStripMenuItem
            // 
            this.aSSStylesToolStripMenuItem.Name = "aSSStylesToolStripMenuItem";
            this.aSSStylesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aSSStylesToolStripMenuItem.Text = "ASS styles...";
            this.aSSStylesToolStripMenuItem.Click += new System.EventHandler(this.AssaStylesToolStripMenuItemClick);
            // 
            // timerTextUndo
            // 
            this.timerTextUndo.Interval = 700;
            this.timerTextUndo.Tick += new System.EventHandler(this.TimerTextUndoTick);
            // 
            // timerOriginalTextUndo
            // 
            this.timerOriginalTextUndo.Interval = 700;
            this.timerOriginalTextUndo.Tick += new System.EventHandler(this.TimerOriginalTextUndoTick);
            // 
            // contextMenuStripShowVideoControls
            // 
            this.contextMenuStripShowVideoControls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemShowVideoControls});
            this.contextMenuStripShowVideoControls.Name = "contextMenuStripVideoControls";
            this.contextMenuStripShowVideoControls.Size = new System.Drawing.Size(195, 26);
            // 
            // toolStripMenuItemShowVideoControls
            // 
            this.toolStripMenuItemShowVideoControls.Name = "toolStripMenuItemShowVideoControls";
            this.toolStripMenuItemShowVideoControls.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemShowVideoControls.Text = "Show video controls";
            this.toolStripMenuItemShowVideoControls.Click += new System.EventHandler(this.ToolStripMenuItemShowVideoControlsClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 646);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 554);
            this.Name = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.ResizeBegin += new System.EventHandler(this.Main_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Main_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainKeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_MouseDown);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripListView.ResumeLayout(false);
            this.contextMenuStripHideVideoControls.ResumeLayout(false);
            this.contextMenuStripWaveform.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerListViewAndText.Panel1.ResumeLayout(false);
            this.splitContainerListViewAndText.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListViewAndText)).EndInit();
            this.splitContainerListViewAndText.ResumeLayout(false);
            this.groupBoxEdit.ResumeLayout(false);
            this.groupBoxEdit.PerformLayout();
            this.contextMenuStripTextBoxListView.ResumeLayout(false);
            this.contextMenuStripTextBoxSourceView.ResumeLayout(false);
            this.contextMenuStripEmpty.ResumeLayout(false);
            this.contextMenuStripShowVideoControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reopenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoLineNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private Nikse.SubtitleEdit.Controls.SubtitleListView SubtitleListview1;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxSource;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBoxSourceView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoToListView;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripMenuItem adjustDisplayTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startNumberingFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeTextForHearImpairedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendTextVisuallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHistoryforUndoToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripListView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem boldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem italicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem underlineToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxEdit;
        private System.Windows.Forms.ToolStripMenuItem splitLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeBeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem removeFormattinglToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.Label labelTextLineTotal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem ChangeCasingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMergeLines;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSortBy;
        private System.Windows.Forms.ToolStripMenuItem sortNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortStartTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortEndTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortDisplayTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortTextMaxLineLengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortTextTotalLengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortTextNumberOfLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortTextAlphabeticallytoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeLanguageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCompare;
        private System.Windows.Forms.ToolStripMenuItem multipleReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSelected;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertUnicodeCharacter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorInsertUnicodeCharacter;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoMergeShortLines;
        private System.Windows.Forms.ToolStripMenuItem setMinimumDisplayTimeBetweenParagraphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFont;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.Timer ShowSubtitleTimer;
        private System.Windows.Forms.Timer timerAutoDuration;
        private System.Windows.Forms.Timer timerAutoContinue;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckMain;
        private System.Windows.Forms.ToolStripMenuItem spellCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDoubleWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem GetDictionariesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addWordToNameListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSynchronization;
        private System.Windows.Forms.ToolStripMenuItem visualSyncToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPointSync;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAdjustAllTimes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoTranslate;
        private System.Windows.Forms.ToolStripMenuItem translateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVideo;
        private System.Windows.Forms.ToolStripMenuItem openVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenContainingFolder;
        private System.Windows.Forms.Panel panelVideoPlayer;
        private System.Windows.Forms.Timer timerWaveform;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripWaveform;
        private System.Windows.Forms.ToolStripMenuItem addParagraphHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteParagraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mergeWithPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeWithNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWaveformPlaySelection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNetworking;
        private System.Windows.Forms.ToolStripMenuItem startServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusNetworking;
        private System.Windows.Forms.ToolStripMenuItem leaveSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSessionKeyLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chatToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.ToolStripMenuItem undockVideoControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redockVideoControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoSplitLongLines;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEmpty;
        private System.Windows.Forms.ToolStripMenuItem insertLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeVideoToolStripMenuItem;
        private Nikse.SubtitleEdit.Controls.NikseLabel labelOriginalText;
        private System.Windows.Forms.Label labelOriginalSingleLine;
        private System.Windows.Forms.Label labelTextOriginalLineLengths;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem saveOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveOriginalAstoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetAudioTrack;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeFrameRate2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopySourceText;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem editSelectAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBoxListView;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSplitTextAtCursor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem boldToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem italicToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem underlineToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fontNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImport;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportImages;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportTimeCodes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportFromVideo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportBluRaySup;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportBluraySupFileForEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportSubIdx;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportDvdSubtitles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportOcrHardSub;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportManualAnsi;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportAdobeEncoreFABImageScript;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportAvidStl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportAyato;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportPngXml;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportBluraySup;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportBdTextSt;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportCapMakerPlus;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportCaptionInc;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportCavena890;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportCheetahCap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportDcinemaInterop;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportDcinemaSMPTE2014;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportDost;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportDvdStudioProStl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportEBUSTL;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportEdl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportEdlClipName;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportFcpIImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportFcpXmlAdvanced;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportImagePerFrame;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportTextTimeCodePair;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportPACScreenElectronics;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportUniPac;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportPlainText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportSpumux;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportUltech130;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportVobSubSubIdx;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorExportCustomText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportCustomTextFormat;
        private System.Windows.Forms.ToolStripMenuItem pointSyncViaOtherSubtitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoogleMicrosoftTranslateSelLine;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMakeEmptyFromCurrent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripMenuItem showWaveformAndSpectrogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyWaveformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlySpectrogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertUnicodeSymbol;
        private System.Windows.Forms.ToolStripMenuItem setStylesForSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FindDoubleLinesToolStripMenuItem;
        private Controls.SETextBox textBoxListViewTextOriginal;
        private System.Windows.Forms.ToolStripMenuItem textCharssecToolStripMenuItem;
        private System.Windows.Forms.Timer timerTextUndo;
        private System.Windows.Forms.Timer timerOriginalTextUndo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMergeDialog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSurroundWithMusicSymbols;
        private System.Windows.Forms.ToolStripMenuItem superscriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subscriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemApplyDurationLimits;
        private System.Windows.Forms.ToolStripMenuItem generateDatetimeInfoFromVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRightToLeftMode;
        private System.Windows.Forms.ToolStripMenuItem joinSubtitlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReverseRightToLeftStartEnd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAssStyles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAlignment;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRestoreAutoBackup;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatistics;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileFormatProperties;
        private System.Windows.Forms.ToolStripMenuItem textWordsPerMinutewpmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addParagraphAndPasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorGuessTimeCodes;
        private System.Windows.Forms.ToolStripMenuItem guessTimeCodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUndo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRedo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowOriginalInPreview;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPlugins;
        private System.Windows.Forms.ToolStripMenuItem seekSilenceToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerListViewAndText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemColumn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemColumnDeleteText;
        private System.Windows.Forms.ToolStripMenuItem ShiftTextCellsDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPasteSpecial;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemColumnImportText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertTextFromSub;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenKeepVideo;
        private System.Windows.Forms.ToolStripMenuItem changeSpeedInPercentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnDeleteTextOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBatchConvert;
        private System.Windows.Forms.ToolStripMenuItem copyOriginalTextToCurrentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMergeDuplicateText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWebVTT;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSpellCheckSuggestions;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckSkipOnce;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckSkipAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckAddToDictionary;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckAddToNames;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSpellCheck;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWebVttVoice;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuWebVttBrowserPreview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorWebVTT;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemModifySelection;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInverseSelection;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpellCheckFromCurrentLine;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMeasurementConverter;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportChapters;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImportShotChanges;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemListShotChanges;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSubtitlesBridgeGaps;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenDvd;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFocusTextbox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorAscOrDesc;
        private System.Windows.Forms.ToolStripMenuItem AscendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem descendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetLanguage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertUnicodeControlCharacters;
        private System.Windows.Forms.ToolStripMenuItem leftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem righttoleftMarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfLefttorightEmbeddingLREToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfRighttoleftEmbeddingRLEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfLefttorightOverrideLROToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfRighttoleftOverrideRLOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRtlUnicodeControlChars;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveUnicodeControlChars;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMergeLinesWithSameTimeCodes;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItemSplitterCheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem setVideoOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddWaveformBatch;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelProgress;
        private System.Windows.Forms.ToolStripMenuItem removeShotChangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addShotChangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem netflixQualityCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setActorForSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetRegion;
        private System.Windows.Forms.ToolStripMenuItem insertSubtitleHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenVideoFromUrl;
        private System.Windows.Forms.ToolStripMenuItem smpteTimeModedropFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveTextUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveTextDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSplitViaWaveform;
        private System.Windows.Forms.ToolStripMenuItem boxToolStripMenuItem;
        private System.Windows.Forms.ImageList imageListBookmarks;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBookmark;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoToSourceView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEmptyGoToSourceView;
        private System.Windows.Forms.ToolStripMenuItem removeAllFormattingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBoldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeItalicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUnderlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFontNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBouten;
        private System.Windows.Forms.ToolStripMenuItem boutendotbeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutendotafterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutendotoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenfilledcircleoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenopencircleoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenopendotoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenfilledsesameoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenopensesameoutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenautooutsideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boutenautoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHorizontalDigits;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetParagraphAsSelection;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRuby;
        private System.Windows.Forms.Label labelOriginalSingleLinePixels;
        private System.Windows.Forms.ToolStripMenuItem extendBeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extendAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extendToPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extendToNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem goToPreviousSubtitleStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToNextSubtitleStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boxToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAssaStyles;
        private System.Windows.Forms.ToolStripMenuItem openSecondSubtitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSSStylesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autotranslateViaCopypasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorInsertUnicode;
        private System.Windows.Forms.ToolStripMenuItem insertUnicodeCharactersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertUnicodeControlCharactersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lefttorightMarkLRMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem righttoleftMarkRLMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfLefttorightEmbeddingLREToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startOfRighttoleftEmbeddingRLEToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startOfLefttorightOverrideLROToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startOfRighttoleftOverrideRLOToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBeautifyTimeCodes;
        private System.Windows.Forms.ToolStripMenuItem mergeSentencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem breaksplitLongLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectedLines;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoBreakLines;
        private System.Windows.Forms.ToolStripMenuItem visualSyncSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSelectedLinesEarlierlaterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem karaokeEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnbreakLines;
        private System.Windows.Forms.ToolStripMenuItem typeEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adjustDisplayTimeForSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveSelectedLines;
        private System.Windows.Forms.ToolStripMenuItem changeCasingForSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixCommonErrorsInSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTranslateSelected;
        private System.Windows.Forms.ToolStripMenuItem genericTranslateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAssaTools;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorAssa;
        private System.Windows.Forms.ToolStripMenuItem applyCustomStylesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateBlankVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateVideoWithHardcodedSubtitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem progressBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoResolutionResamplerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertBefore;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertAfter;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertSubtitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripHideVideoControls;
        private System.Windows.Forms.ToolStripMenuItem hideVideoControlsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripShowVideoControls;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowVideoControls;
        private System.Windows.Forms.ToolStripMenuItem listErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateBackgroundBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoaudioToTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByGapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordListsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorPickerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertColorsToDialogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioToTextWhisperTolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeTranslationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateVideoWithSoftcodedSubtitlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportTtmlImage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4Extend;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetLayer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWebVttStyle;
        private System.Windows.Forms.ToolStripMenuItem beautifyTimeCodesOfSelectedLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autotranslateNLLBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEvenlyDistributeLines;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVerifyCompleteness;
        private System.Windows.Forms.ToolStripMenuItem runWhiperOnParagraphToolStripMenuItem;
    }
}