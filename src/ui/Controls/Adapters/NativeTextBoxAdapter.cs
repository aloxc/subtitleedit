using Nikse.SubtitleEdit.Controls.Interfaces;
using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Controls.Adapters
{
    public class NativeTextBoxAdapter : ISelectedText
    {
        private readonly TextBoxBase _textBox;

        public string SelectedText
        {
            get => _textBox.SelectedText;
            set => _textBox.SelectedText = value;
        }

        public NativeTextBoxAdapter(TextBoxBase textBox) => _textBox = textBox;
    }
}