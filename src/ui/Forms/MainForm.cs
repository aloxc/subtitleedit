using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void subtitleListView1_DragDrop(object sender, DragEventArgs e)
        {
            _dragAndDropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (_dragAndDropFiles.Length == 1)
            {
                _dragAndDropTimer.Start();
            }
            else
            {
                MessageBox.Show(_language.DropOnlyOneFile);
            }
        }

        private void subtitleListView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }
    }
}
