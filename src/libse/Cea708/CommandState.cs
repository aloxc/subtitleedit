using Nikse.SubtitleEdit.Core.Cea708.Commands;
using System.Collections.Generic;

namespace Nikse.SubtitleEdit.Core.Cea708
{
    public class CommandState
    {
        public List<ICea708Command> Commands { get; set; }
        public int StartLineIndex { get; set; }

        public CommandState()
        {
            Commands = new List<ICea708Command>();
        }
    }
}
