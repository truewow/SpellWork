using System;
using System.Collections.Generic;
using System.Linq;
using SpellWorkLib;
using SpellWorkLib.DBC;
using SpellWorkLib.Spell;

namespace SpellWorkWeb
{
    public interface ISpellRepository
    {
        List<SpellEntry> All();
        SpellEntry? Get(int id);
        string GetSpellHTML(SpellEntry spellEntry);
    }

    public class SpellRepository : ISpellRepository
    {
        private readonly Dictionary<int, SpellEntry> _spells;
 
        public SpellRepository()
        {
            _spells = DBC.Spell.ToDictionary(pair => (int) pair.Key, pair => pair.Value);
        }

        public List<SpellEntry> All()
        {
            try
            {
                return _spells.Values.ToList();
            }
            catch (ArgumentNullException)
            {
                return new List<SpellEntry>();
            }
        }

        public SpellEntry? Get(int id)
        {
            try
            {
                SpellEntry spell;
                if (_spells.TryGetValue(id, out spell))
                    return spell;
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public string GetSpellHTML(SpellEntry spellEntry)
        {
            try
            {
                var spellInfo = new SpellInfo(new HTMLSpellInfoWriter(), spellEntry);
                return spellInfo.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
