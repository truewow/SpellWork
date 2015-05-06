using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SpellWork.DBC;
using SpellWork.Extensions;
using SpellWork.Spell;

namespace SpellWork.Forms
{
    public sealed partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            splitContainer3.SplitterDistance = 128;

            Text = DBC.DBC.Version;

            _cbSpellFamilyName.SetEnumValues<SpellFamilyNames>("SpellFamilyName");
            _cbSpellAura.SetEnumValues<AuraType>("Aura");
            _cbSpellEffect.SetEnumValues<SpellEffects>("Effect");
            _cbTarget1.SetEnumValues<Targets>("Target A");
            _cbTarget2.SetEnumValues<Targets>("Target B");
            
            _status.Text = String.Format("DBC Locale: {0}", DBC.DBC.Locale);

            _cbAdvancedFilter1.SetStructFields<SpellEntry>();
            _cbAdvancedFilter2.SetStructFields<SpellEntry>();

            _cbAdvancedFilter1CompareType.SetEnumValuesDirect<CompareType>(true);
            _cbAdvancedFilter2CompareType.SetEnumValuesDirect<CompareType>(true);
        }

        #region FORM

        private void ExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutClick(object sender, EventArgs e)
        {
            var ab = new FormAboutBox();
            ab.ShowDialog();
        }

        private void FormMainResize(object sender, EventArgs e)
        {
            _scCompareRoot.SplitterDistance = (((Form)sender).Size.Width / 2) - 25;
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((Char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        #endregion

        #region SPELL INFO PAGE

        private void LvSpellListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lvSpellList.SelectedIndices.Count > 0)
                new SpellInfo(_rtSpellInfo, _spellList[_lvSpellList.SelectedIndices[0]]);
        }

        private void TbSearchIdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AdvancedSearch();
        }

        private void BSearchClick(object sender, EventArgs e)
        {
            AdvancedSearch();
        }

        private void CbSpellFamilyNamesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex != 0)
                AdvancedFilter();
        }

        private void TbAdvansedFilterValKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AdvancedFilter();
        }

        private void AdvancedSearch()
        {
            var name = _tbSearchId.Text;
            var id = name.ToUInt32();
            var ic = _tbSearchIcon.Text.ToUInt32();
            var at = _tbSearchAttributes.Text.ToUInt32();

            _spellList = (from spell in DBC.DBC.Spell.Values
                          where
                              ((id == 0 || spell.ID == id) && (ic == 0 || spell.SpellIconID == ic) &&
                               (at == 0 || (spell.Attributes & at) != 0 || (spell.AttributesEx & at) != 0 ||
                                (spell.AttributesEx2 & at) != 0 || (spell.AttributesEx3 & at) != 0 ||
                                (spell.AttributesEx4 & at) != 0 || (spell.AttributesEx5 & at) != 0 ||
                                (spell.AttributesEx6 & at) != 0 || (spell.AttributesEx7 & at) != 0)) &&
                              ((id != 0 || ic != 0 && at != 0) || spell.SpellName.ContainsText(name))
                          select spell).ToList();

            _lvSpellList.VirtualListSize = _spellList.Count();
            if (_lvSpellList.SelectedIndices.Count > 0)
                _lvSpellList.Items[_lvSpellList.SelectedIndices[0]].Selected = false;
        }

        private void AdvancedFilter()
        {
            var bFamilyNames = _cbSpellFamilyName.SelectedIndex != 0;
            var fFamilyNames = _cbSpellFamilyName.SelectedValue.ToInt32();

            var bSpellAura = _cbSpellAura.SelectedIndex != 0;
            var fSpellAura = _cbSpellAura.SelectedValue.ToInt32();

            var bSpellEffect = _cbSpellEffect.SelectedIndex != 0;
            var fSpellEffect = _cbSpellEffect.SelectedValue.ToInt32();

            var bTarget1 = _cbTarget1.SelectedIndex != 0;
            var fTarget1 = _cbTarget1.SelectedValue.ToInt32();

            var bTarget2 = _cbTarget2.SelectedIndex != 0;
            var fTarget2 = _cbTarget2.SelectedValue.ToInt32();

            // additional filtert
            var advVal1 = _tbAdvancedFilter1Val.Text;
            var advVal2 = _tbAdvancedFilter2Val.Text;

            var field1 = (MemberInfo)_cbAdvancedFilter1.SelectedValue;
            var field2 = (MemberInfo)_cbAdvancedFilter2.SelectedValue;

            var use1Val = advVal1 != string.Empty;
            var use2Val = advVal2 != string.Empty;

            var field1Ct = (CompareType)_cbAdvancedFilter1CompareType.SelectedIndex;
            var field2Ct = (CompareType)_cbAdvancedFilter2CompareType.SelectedIndex;

            _spellList = (from spell in DBC.DBC.Spell.Values
                          where
                              (!bFamilyNames || spell.SpellFamilyName == fFamilyNames) &&
                              (!bSpellEffect || spell.Effect.ContainsElement((uint)fSpellEffect)) &&
                              (!bSpellAura || spell.EffectApplyAuraName.ContainsElement((uint)fSpellAura)) &&
                              (!bTarget1 || spell.EffectImplicitTargetA.ContainsElement((uint)fTarget1)) &&
                              (!bTarget2 || spell.EffectImplicitTargetB.ContainsElement((uint)fTarget2)) &&
                              (!use1Val || spell.CreateFilter(field1, advVal1, field1Ct)) &&
                              (!use2Val || spell.CreateFilter(field2, advVal2, field2Ct))
                          select spell).ToList();

            _lvSpellList.VirtualListSize = _spellList.Count();
            if (_lvSpellList.SelectedIndices.Count > 0)
                _lvSpellList.Items[_lvSpellList.SelectedIndices[0]].Selected = false;
        }

        #endregion
        
        #region COMPARE PAGE

        private void CompareFilterSpellTextChanged(object sender, EventArgs e)
        {
            var spell1 = _tbCompareFilterSpell1.Text.ToUInt32();
            var spell2 = _tbCompareFilterSpell2.Text.ToUInt32();

            if (DBC.DBC.Spell.ContainsKey(spell1) && DBC.DBC.Spell.ContainsKey(spell2))
                new SpellCompare(_rtbCompareSpell1, _rtbCompareSpell2, DBC.DBC.Spell[spell1], DBC.DBC.Spell[spell2]);
        }

        private void CompareSearch1Click(object sender, EventArgs e)
        {
            var form = new FormSearch();
            form.ShowDialog(this);
            if (form.DialogResult == DialogResult.OK)
                _tbCompareFilterSpell1.Text = form.Spell.ID.ToString();
            form.Dispose();
        }

        private void CompareSearch2Click(object sender, EventArgs e)
        {
            var form = new FormSearch();
            form.ShowDialog(this);
            if (form.DialogResult == DialogResult.OK)
                _tbCompareFilterSpell2.Text = form.Spell.ID.ToString();
            form.Dispose();
        }

        #endregion

        #region SQL PAGE
        
        #endregion

        #region VIRTUAL MODE

        private List<SpellEntry> _spellList = new List<SpellEntry>();

        private List<SpellEntry> _spellProcList = new List<SpellEntry>();

        private void LvSpellListRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item =
                new ListViewItem(new[] { _spellList[e.ItemIndex].ID.ToString(), _spellList[e.ItemIndex].SpellNameRank });
        }

        #endregion
    }
}
