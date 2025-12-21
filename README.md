# 📚 Kütüphane Otomasyon Sistemi

Modern, hızlı ve kullanıcı dostu bir kütüphane yönetim çözümü. Clean Architecture prensipleriyle geliştirilmiş, ölçeklenebilir ve sürdürülebilir bir altyapıya sahiptir.

---

## ✨ Özellikler

- **Gelişmiş Kitap Yönetimi**: Kitap ekleme, silme, güncelleme ve detaylı arama.
- **Üye Takibi**: Üye kayıtları, profil yönetimi ve ödünç alma geçmişi.
- **Emanet/İade Sistemi**: Kitapların ödünç verilmesi ve iade süreçlerinin takibi.
- **Raporlama**: Grafiklerle desteklenmiş dashboard ve istatistik paneli.
- **E-posta Bildirimleri**: Emanet süresi yaklaşan kitaplar için otomatik bilgilendirmeler.
- **Rol Tabanlı Yetkilendirme**: Admin ve Personel yetki seviyeleri.

---

## 🛠️ Teknoloji Yığını

- **Backend**: .NET 8.0 (Clean Architecture - Domain, Application, Infrastructure, Presentation)
- **Frontend**: ASP.NET Core MVC, Bootstrap, jQuery
- **Veritabanı**: MS SQL Server
- **ORM**: Entity Framework Core
- **Kimlik Doğrulama**: Microsoft Identity
- **Dokümantasyon**: Swagger / OpenAPI

---

## 📂 Proje Yapısı

- `Presentation/Kutuphane.WebUI`: Kullanıcı arayüzü ve API katmanı.
- `Infrastructure/Kutuphane.Persistence`: Veritabanı context ve repositoriyer.
- `Infrastructure/Kutuphane.Infrastructure`: E-posta, dosya yönetimi vb. servisler.
- `Core/Kutuphane.Application`: Business logic, CQRS ve servis arayüzleri.
- `Core/Kutuphane.Domain`: Entity'ler, value object'ler ve domain logic.

---

## 🚀 Kurulum ve Başlatma

Sistemi bilgisayarınızda çalıştırmak için detaylı kurulum rehberine göz atın:

👉 **[KURULUM REHBERI (KURULUM.md)](file:///c:/Users/Erkan/Desktop/KutuphaneOtomasyon/Kutuphane/KURULUM.md)**

---

## 🖼️ Ekran Görüntüleri

Uygulamanın arayüzüne ait örneklere `docs/` klasöründen ulaşabilirsiniz:

- [Dashboard Paneli](file:///c:/Users/Erkan/Desktop/KutuphaneOtomasyon/Kutuphane/docs/dashboard.png)
- [Kitap Listesi](file:///c:/Users/Erkan/Desktop/KutuphaneOtomasyon/Kutuphane/docs/book_list.png)
- [Üye Profili](file:///c:/Users/Erkan/Desktop/KutuphaneOtomasyon/Kutuphane/docs/member_profile.png)

---
*Antigravity tarafından modernize edilmiştir.*

