# 🏢 UstaPlatform - Şehrin Uzmanlık Platformu

Proje, Nesne Yönelimli Programlama (NYP) ve İleri C# dersi kapsamında, Arcadia şehrindeki uzmanları (usta) vatandaş talepleriyle eşleştiren, dinamik fiyatlama ve akıllı rota planlama yapabilen, genişletilebilir (plug-in) bir platform geliştirmeyi amaçlar.

---

## 1. Kurulum Adımları ve Çalıştırma Bilgisi

Bu bölüm, projenin nasıl derleneceğini ve **Açık/Kapalı Prensibi (OCP)** test senaryosunun nasıl çalıştırılacağını açıklar.

### Gereksinimler
* Visual Studio 2022
* .NET 8.0 SDK (veya projenin hedeflendiği .NET sürümü)

### Kurulum ve Çalıştırma

1.  **Çözümü Açın:** `UstaPlatform.sln` dosyasını Visual Studio'da açın.
2.  **Projeyi Derleyin:** Çözümdeki tüm projelerin derlendiğinden emin olmak için **Oluştur (Build) > Çözümü Yeniden Oluştur (Rebuild Solution)** seçeneğini seçin. Bu adım, varsayılan kuralları (`UstaPlatform.Pricing.Rules`) ve harici eklentileri (`UstaPlatform.Plugins.*`) derleyecektir.
3.  **Başlangıç Projesini Ayarlayın:** Çözüm Gezgini'nde (Solution Explorer) **`UstaPlatform.App`** projesine sağ tıklayın ve **"Başlangıç Projesi Olarak Ayarla"** (Set as Startup Project) seçeneğini seçin.

### ⚠️ OCP Testi İçin Zorunlu Manuel Adım (Plug-in DLL Kopyalama)

Projenin en kritik gereksinimi olan dinamik eklenti yüklemeyi test etmek için, derlenen Plug-in DLL'lerini ana uygulamanın çalışma dizinine manuel olarak kopyalamanız gerekir:

1.  **Kaynak DLL'leri Bulun:**
    * `[Proje Klasörü]\plugins\UstaPlatform.Plugins.LoyaltyRule\bin\Debug\net8.0\UstaPlatform.Plugins.LoyaltyRule.dll`
    * *(Eğer oluşturduysanız) Diğer plugin DLL'leri...*

2.  **Hedef Klasörü Bulun:**
    * Ana uygulamanın çalıştığı yeri açın:
        `[Proje Klasörü]\src\UstaPlatform.App\bin\Debug\net8.0\`

3.  **`Plugins` Klasörünü Oluşturun:**
    * Yukarıdaki hedef klasörün içine, tam olarak **`Plugins`** adında **yeni bir klasör** oluşturun.

4.  **Kopyalayın:**
    * Adım 1'de bulduğunuz tüm Plug-in DLL'lerini bu yeni `Plugins` klasörünün içine yapıştırın.

### Test Senaryosu

Uygulamayı çalıştırın (F5) ve kuralların tetiklendiğini görmek için aşağıdaki dinamik verileri girin:

* **Müşteri Adı:** `Alper Kılıç Gold` (Sadakat Kuralını tetikler)
* **Talep Tarihi:** `01.11.2025` (Haftasonu Kuralını tetikler - Cumartesi)
* **Acil Servis?:** `E` (Acil Durum Kuralını tetikler)

---

## 2. Tasarım Kararları ve Mimari

Proje, SOLID prensiplerine sıkı sıkıya bağlı, **Katmanlı Mimari (Layered Architecture)** kullanılarak tasarlanmıştır.

### A. Mimari Katmanlar (SRP Prensibi)

Tek Sorumluluk Prensibi (SRP) gereği, projenin her ana işlevi ayrı bir katmana (projeye) bölünmüştür:

* **`UstaPlatform.Domain`**: Çekirdek katmandır. `Master`, `WorkOrder` gibi temel iş varlıklarını (Entities) ve `IPricingRule` gibi arayüzleri (Kontratları) içerir. Başka hiçbir projeye bağımlılığı yoktur.
* **`UstaPlatform.Application`**: İş mantığı ve akışlarını yönetir. `PricingEngine` (Fiyatlama Motoru) ve `MatchingEngine` (Eşleştirme Motoru) gibi servis sınıflarını barındırır.
* **`UstaPlatform.Infrastructure`**: Dış dünya ile ilgili teknik detayları içerir. `GeoHelper` ve `DistanceHelper` gibi statik yardımcı sınıflar bu katmandadır.
* **`UstaPlatform.Pricing.Rules`**: Varsayılan, çekirdek fiyatlandırma kurallarını (`WeekendSurchargeRule` vb.) içeren ayrı bir kütüphanedir.
* **`UstaPlatform.App`**: Ana başlatıcıdır (Console Application). Tüm bağımlılıkları bir araya getirir (Composition Root) ve iş akışını başlatır.

### B. En Kritik Kısım: Plug-in (Eklenti) Mimarisi (OCP & DIP)

Projenin kalbi, ana uygulama kodunu değiştirmeden sisteme yeni fiyat kuralları ekleyebilme yeteneğidir. Bu, **Açık/Kapalı Prensibi (OCP)** ve **Bağımlılıkların Tersine Çevrilmesi (DIP)** ile sağlanmıştır.

**Tasarım Adımları:**

1.  **Kontrat (DIP):** `UstaPlatform.Domain` içinde **`IPricingRule`** adında bir arayüz tanımlanmıştır. 
2.  **Motor (DIP):** `UstaPlatform.Application` içindeki **`PricingEngine`**, somut kural sınıflarına değil, yalnızca `IPricingRule` arayüzüne bağımlıdır.
3.  **Dinamik Yükleyici (OCP):** `UstaPlatform.App` içindeki **`PluginRuleLoader`** sınıfı, uygulama başladığında **Reflection** (`Assembly.LoadFrom`) kullanarak `Plugins/` klasöründeki tüm DLL dosyalarını tarar.
4.  **Entegrasyon (OCP):** `PluginRuleLoader`, taranan DLL'ler içinde `IPricingRule` arayüzünü uygulayan tüm sınıfları bulur, örneklerini oluşturur (`Activator.CreateInstance`) ve bu listeyi `PricingEngine`'e enjekte eder.
5.  **Sonuç:** `UstaPlatform.App`'in kaynak kodunu **hiç değiştirmeden**, sadece `Plugins` klasörüne yeni bir DLL bırakarak (Örn: `LoyaltyDiscountRule.dll`) sistemin fiyat hesaplama mantığı genişletilebilir.

### C. İleri C# Özellikleri

* **`init-only` Özelliği:** `Domain` katmanındaki varlıklarda (`Master`, `WorkOrder` vb.) `Id`, `RequestTime` gibi alanlar `init` olarak ayarlanmıştır.
* **Dizinleyici (Indexer):** `Schedule.cs` sınıfı, `schedule[DateOnly date]` sözdizimini destekleyen bir dizinleyici uygular.
* **Özel `IEnumerable<T>` Koleksiyonu:** `Route.cs` sınıfı, `IEnumerable<(int X, int Y)>` arayüzünü ve `public void Add(int X, int Y)` metodunu uygular. Bu, `new Route { {10, 20}, {30, 40} }` şeklinde **Koleksiyon Başlatıcı (Collection Initializer)** kullanımına olanak tanır.
* **Statik Yardımcılar:** `GeoHelper` ve `DistanceHelper` gibi sınıflar `static` olarak tasarlanmıştır.

---

## 3. Kısa Demo Akışı (Çıktı)

Aşağıdaki çıktı, hem varsayılan kuralların (Haftasonu, Acil) hem de `Plugins` klasöründen dinamik olarak yüklenen `Kademeli Sadakat İndirimi Kuralı`'nın başarıyla çalıştığını göstermektedir.