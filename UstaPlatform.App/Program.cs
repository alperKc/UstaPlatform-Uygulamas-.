using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UstaPlatform.App;
using UstaPlatform.Domain;
using UstaPlatform.Pricing.Rules;
using UstaPlatform.Application;
using UstaPlatform.Infrastructure;

namespace UstaPlatform.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- UstaPlatform Başlatılıyor: Plug-in Mimarisi Demosu ---");

            Console.WriteLine("\n--- YENİ İŞ TALEBİ GİRİŞİ ---");

            Console.Write("1. Müşteri Adı (Örn: Alper Kılıç Gold): ");
            string customerName = Console.ReadLine() ?? "Anonim";

            Console.Write("2. Adres/Konum (Örn: Ana Cadde No: 50): ");
            string customerAddress = Console.ReadLine() ?? "Bilinmiyor";

            Console.Write("3. Gerekli Uzmanlık (Örn: Tesisat/Elektrik): ");
            string requestedJob = Console.ReadLine() ?? "Tesisat";

            Console.Write("4. Talep Tarihi (dd.MM.yyyy) veya Boş (Bugün): ");
            string dateInput = Console.ReadLine() ?? string.Empty;

            DateTime requestDateTime = DateTime.Now;
            if (DateTime.TryParse(dateInput, out DateTime parsedDate))
            {
                requestDateTime = parsedDate;
            }

            bool isWeekend = requestDateTime.DayOfWeek == DayOfWeek.Saturday || requestDateTime.DayOfWeek == DayOfWeek.Sunday;

            Console.Write("5. Acil Servis? (E/H): ");
            bool isUrgent = (Console.ReadLine()?.ToUpper() == "E");

            // --- SADAKAT SEVİYESİ SİMÜLASYONU (GÜNCELLEME BURADA) ---
            LoyaltyTier customerTier = LoyaltyTier.None;
            if (customerName.Contains("Gold"))
            {
                customerTier = LoyaltyTier.Gold;
            }
            else if (customerName.Contains("Platinum"))
            {
                customerTier = LoyaltyTier.Platinum;
            }

            var defaultRules = new List<IPricingRule> { new WeekendSurchargeRule(), new UrgentCallRule() };
            string pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
            var pluginRules = PluginRuleLoader.LoadRulesFromPlugins(pluginPath);
            var allRules = defaultRules.Concat(pluginRules).ToList();
            var pricingEngine = new PricingEngine(allRules);
            var matchingEngine = new MatchingEngine();

            Console.WriteLine($"[SUCCESS] Toplam {allRules.Count} adet IPricingRule yüklendi.");
            Console.WriteLine("----------------------------------------------------------");

            var masterPool = new List<Master>
            {
                new Master { Id = Guid.NewGuid(), Name = "Elektrikçi Engin", Expertise = "Elektrik", CurrentLoad = 1, Address = "A Mah. Köşe Sokak" },
                new Master { Id = Guid.NewGuid(), Name = "Tesisatçı Tufan", Expertise = "Tesisat", CurrentLoad = 3, Address = "Merkez Blv. 12" },
                new Master { Id = Guid.NewGuid(), Name = "Tesisatçı Selim", Expertise = "Tesisat", CurrentLoad = 1, Address = "Park Yanı 5" },
                new Master { Id = Guid.NewGuid(), Name = "Marangoz Metin", Expertise = "Marangoz", CurrentLoad = 0, Address = "Sanayi Sitesi" },
                new Master { Id = Guid.NewGuid(), Name = "Tesisatçı Barış", Expertise = "Tesisat", CurrentLoad = 2, Address = "Göl Mah. Sahil Cd." }
            };

            // --- GÜNCELLENMİŞ TALEP OLUŞTURMA ---
            var request = new Request
            {
                Requester = new Citizen
                {
                    Name = customerName,
                    Address = customerAddress,
                    Tier = customerTier // <-- GÜNCELLEME BURADA
                },
                RequiredExpertise = requestedJob,
                IsUrgent = isUrgent,
                IsWeekend = isWeekend,
                RequestTime = requestDateTime
            };

            Console.WriteLine($"\n--- İŞ TALEBİ SONUÇLANIYOR ---");

            Master? selectedMaster = matchingEngine.FindBestMatch(request, masterPool);

            if (selectedMaster != null)
            {
                var workOrder = new WorkOrder { AssignedMaster = selectedMaster, Request = request, BasePrice = 100.00m };

                Console.WriteLine($"\n[EŞLEŞTİRME SONUCU] EN UYGUN USTA: {selectedMaster.Name}");
                double distance = DistanceHelper.CalculateDistance(customerAddress, selectedMaster.Address);
                Console.WriteLine($"Mesafe: {distance:N1} km, Mevcut Yoğunluk: {selectedMaster.CurrentLoad}");

                (decimal finalPrice, List<string> appliedRules) result = pricingEngine.CalculatePrice(workOrder.BasePrice, workOrder);
                workOrder.FinalPrice = result.finalPrice;

                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"[FİYAT] Son Fiyat: {workOrder.FinalPrice:C}");
                Console.WriteLine($"[KURALLAR] Uygulanıyor: {string.Join(" -> ", result.appliedRules)}");
                selectedMaster.CurrentLoad++;

                DateOnly workDay = DateOnly.FromDateTime(requestDateTime);
                var schedule1 = new Schedule { };
                schedule1.AddWorkOrder(workOrder, workDay);

                Console.WriteLine($"\n[ÇİZELGE] Indexer Testi: {workDay} tarihli iş sayısı: {schedule1[workDay].Count}");

                (int X, int Y) startCoord = GeoHelper.GetCoordinates(customerAddress);

                var route = new Route {
                    { startCoord.X, startCoord.Y },
                    { 150, 200 }
                };
                Console.WriteLine($"[ROUTE] Rotanın ilk durağı (Koleksiyon Başlatıcı): X={route.First().X}, Y={route.First().Y}");
            }
            else
            {
                Console.WriteLine($"[HATA] {requestedJob} uzmanlığına uygun müsait usta bulunamadı.");
            }
        }
    }
}