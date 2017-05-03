using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellWork
{
    static class Program
    {
        static void Main(string[] args)
        {
            var dbcPath = $"dbc";
            if (!Directory.Exists(dbcPath))
            {
                Console.WriteLine($"Files in {Path.GetFullPath(dbcPath)} missing");
                return;
            }

            try
            {
                Task.Run(() => DBC.DBC.Load()).GetAwaiter().GetResult();
                var sb = new StringBuilder();
                var spellInfo = DBC.DBC.SpellInfoStore[int.Parse(args[0])];
                spellInfo.Write(sb);
                Console.WriteLine(sb.ToString());

                // Application.Run(new FormMain());
            }
            catch (DirectoryNotFoundException dnfe)
            {
                Console.WriteLine(dnfe.Message, @"Missing required DBC file!");
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message, @"DBC file has wrong structure!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, @"SpellWork Error!");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
