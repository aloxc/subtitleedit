﻿using Nikse.SubtitleEdit.Core.Enums;
using System.Text.RegularExpressions;

namespace Nikse.SubtitleEdit.Logic
{
    public interface IFindAndReplace
    {
        void FindDialogFind(string findText, ReplaceType findReplaceType, Regex regex);
        void FindDialogFindPrevious(string findText);
        void FindDialogClose();

        void ReplaceDialogFind(FindReplaceDialogHelper findReplaceDialogHelper);
        void ReplaceDialogReplace(FindReplaceDialogHelper findReplaceDialogHelper);
        void ReplaceDialogReplaceAll(FindReplaceDialogHelper findReplaceDialogHelper);
        void ReplaceDialogClose();
        bool GetAllowReplaceInOriginal();
    }
}
