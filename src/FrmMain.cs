namespace GitHub_Release_Downloader
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var current = txtPath.Text.Trim();

            if (current.Length > 0 && Directory.Exists(current))
            {
                fbdTarget.SelectedPath = current;
            }

            if (fbdTarget.ShowDialog(this) == DialogResult.OK)
            {
                txtPath.Text = fbdTarget.SelectedPath;
            }
        }
    }
}
