using System;
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

            Get["/"] = _ => Response.AsJson(_spellRepository.All().Take(10).ToList());
            Get["/{id:int}"] = parameters =>
            {
                int id = parameters.id;
                if (id <= 0)
                    return HttpStatusCode.BadRequest;

                var spellById = _spellRepository.Get(id);
                if (spellById == null)
                    return HttpStatusCode.NotFound;
                return Response.AsJson(new { SpellEntry = spellById.Value, html = _spellRepository.GetSpellHTML(spellById.Value) });
            };
        }
    }
}
