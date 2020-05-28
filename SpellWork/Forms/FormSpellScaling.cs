using System;
using System.Windows.Forms;

namespace SpellWork.Forms
{
    public partial class FormSpellScaling : Form
    {
        public FormSpellScaling()
        {
            InitializeComponent();
        }

        private void LevelValueChanged(object sender, EventArgs e)
        {
            SelectedLevel = (uint)((TrackBar)sender).Value;
            _tbxLevel.Text = ((TrackBar)sender).Value.ToString();
        }

        private void ItemLevelValueChanged(object sender, EventArgs e)
        {
            SelectedItemLevel = (uint)((TrackBar)sender).Value;
            _tbxItemLevel.Text = ((TrackBar)sender).Value.ToString();
        }

        private void LevelTextKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Escape && e.KeyChar != (char)Keys.Enter)
                e.Handled = true;
        }

        private void LevelTextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text.Length <= 0)
            {
                SelectedLevel = 1;
                tb.Text = "1";
                return;
            }

            var val = uint.Parse(tb.Text);
            if (val > DBC.DBC.MaxLevel)
                tb.Text = DBC.DBC.MaxLevel.ToString();
            else if (val <= 0)
                tb.Text = "1";

            val = uint.Parse(tb.Text);
            _tbLevel.Value = (int)val;
            SelectedLevel = val;
        }

        private void ItemLevelTextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text.Length <= 0)
            {
                SelectedItemLevel = 1;
                tb.Text = "1";
                return;
            }

            var val = uint.Parse(tb.Text);
            if (val > DBC.DBC.MaxItemLevel)
                tb.Text = DBC.DBC.MaxItemLevel.ToString();
            else if (val <= 0)
                tb.Text = "1";

            val = uint.Parse(tb.Text);
            _tbItemLevel.Value = (int)val;
            SelectedItemLevel = val;
        }

        public uint SelectedLevel;
        public uint SelectedItemLevel;
    }
}
