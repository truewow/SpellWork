using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using SpellWorkLib.DBC;

namespace SpellWorkWeb
{
    public class SpellsModule : NancyModule
    {
        private readonly ISpellRepository _spellRepository;

        public SpellsModule(ISpellRepository spellRepository) : base("/spells")
        {
            _spellRepository = spellRepository;

            Get["/"] = _ =>
            {
                IEnumerable<Spell> result;

                try
                {
                    string searchQuery = Request.Query["search"];
                    if (string.IsNullOrWhiteSpace(searchQuery))
                        result = _spellRepository.All();
                    else
                    {
                        int id;
                        if (int.TryParse(searchQuery, out id))
                        {
                            result = _spellRepository.All()
                                .Where(entry => entry.Id.ToString().StartsWith(id.ToString()));
                        }
                        else
                        {
                            result = _spellRepository.All()
                                .Where(entry => entry.Name.StartsWith(searchQuery, StringComparison.InvariantCultureIgnoreCase));
                        }
                    }
                }
                catch (Exception)
                {
                    result = new List<Spell>();
                }

                return Response.AsJson(result.Take(2000));
            };

            Get["/{id:int}"] = parameters =>
            {
                int id = parameters.id;
                if (id <= 0)
                    return HttpStatusCode.BadRequest;

                var spellById = _spellRepository.Get(id);
                if (spellById == null)
                    return HttpStatusCode.NotFound;
                return Response.AsJson(new { Spell = spellById, html = _spellRepository.GetSpellHTML(spellById) });
            };
        }
    }
}
