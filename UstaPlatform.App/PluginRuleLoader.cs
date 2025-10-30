using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UstaPlatform.Domain;

namespace UstaPlatform.App
{
    public static class PluginRuleLoader
    {

       
 
        public static IEnumerable<IPricingRule> LoadRulesFromPlugins(string pluginDirectory)
        {
            var rules = new List<IPricingRule>();
            Console.WriteLine($"[INFO] Plugin klasörü taranıyor: {pluginDirectory}");

            if (!Directory.Exists(pluginDirectory))
            {
                Console.WriteLine("[WARN] Plugin klasörü bulunamadı. Devam ediliyor.");
                return rules;
            }
            foreach (string file in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);

                    var pluginRules = assembly.GetTypes()
                        .Where(t => typeof(IPricingRule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .Select(t => Activator.CreateInstance(t))
                        .Cast<IPricingRule>();

                    rules.AddRange(pluginRules);
                    Console.WriteLine($"[INFO] {Path.GetFileName(file)} içinden {pluginRules.Count()} kural yüklendi.");
                }
                catch (Exception ex)
                {

                    string errorMessage = ex.Message.Length > 50 ? ex.Message.Substring(0, 50) + "..." : ex.Message;
                    Console.WriteLine($"[ERROR] DLL yüklenirken hata: {Path.GetFileName(file)}. Hata: {errorMessage}");
                }
            }

            return rules;
        }
    }
}