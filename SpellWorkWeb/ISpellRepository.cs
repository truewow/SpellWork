using System;
using System.Collections.Generic;
using System.Linq;
using SpellWorkLib.DBC;
using SpellWorkLib.Spell;

namespace SpellWorkWeb
{
    public interface ISpellRepository
    {
        List<Spell> All();
        Spell Get(int id);
        string GetSpellHTML(Spell spell);
    }

    public class SpellRepository : ISpellRepository
    {
        private readonly Dictionary<int, Spell> _spells;
 
        public SpellRepository()
        {
            _spells = DBC.Spell.ToDictionary(pair => (int) pair.Key, pair => new Spell { Id = (int) pair.Key, Name = pair.Value.SpellName });
        }

        public List<Spell> All()
        {
            try
            {
                return _spells.Values.ToList();
            }
            catch (ArgumentNullException)
            {
                return new List<Spell>();
            }
        }

        public Spell Get(int id)
        {
            try
            {
                Spell spell;
                if (_spells.TryGetValue(id, out spell))
                    return spell;
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public string GetSpellHTML(Spell spell)
        {
            try
            {
                SpellEntry spellEntry;
                if (!DBC.Spell.TryGetValue((uint) spell.Id, out spellEntry))
                    return null;

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
