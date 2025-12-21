# 📚 Kütüphane Otomasyon Sistemi

Modern, hızlı ve kullanıcı dostu bir kütüphane yönetim çözümü. **Clean Architecture** prensipleriyle geliştirilmiş, ölçeklenebilir ve sürdürülebilir bir altyapıya sahiptir.

---

## ✨ Özellikler

- 📖 **Gelişmiş Kitap Yönetimi**: Kitap ekleme, silme, güncelleme ve detaylı arama.
- 👤 **Üye Takibi**: Üye kayıtları, profil yönetimi ve ödünç alma geçmişi.
- 🔄 **Emanet/İade Sistemi**: Kitapların ödünç verilmesi ve iade süreçlerinin takibi.
- 📊 **Raporlama**: Grafiklerle desteklenmiş dashboard ve istatistik paneli.
- 🛡️ **Rol Tabanlı Yetkilendirme**: Admin ve Personel yetki seviyeleri.

---

## 🛠️ Teknoloji Yığını

- **Backend**: .NET 8.0
- **Frontend**: ASP.NET Core MVC, Bootstrap, jQuery
- **Veritabanı**: MS SQL Server
- **ORM**: Entity Framework Core
- **Kimlik Doğrulama**: Microsoft Identity
- **Dokümantasyon**: Swagger / OpenAPI

---

## 📂 Proje Yapısı

- `Presentation/Kutuphane.WebUI`: Kullanıcı arayüzü ve API katmanı.
- `Infrastructure/Kutuphane.Persistence`: Veritabanı context ve repositoriyer.
- `Infrastructure/Kutuphane.Infrastructure`: Servisler (Dosya yönetimi vb.).
- `Core/Kutuphane.Application`: Business logic ve CQRS.
- `Core/Kutuphane.Domain`: Entity'ler ve Domain logic.

---

## 🚀 Kurulum ve Başlatma

Sistemi bilgisayarınızda çalıştırmak için detaylı kurulum rehberine göz atın:

👉 **[KURULUM REHBERİ (KURULUM.md)](file:///c:/Users/Erkan/Desktop/KutuphaneOtomasyon/Kutuphane/KURULUM.md)**

---

## 🖼️ Ekran Görüntüleri

Uygulamanın arayüzüne ait güncel ekran görüntüleri:

### 🏠 Ana Sayfa Dashboard
![Ana Sayfa Dashboard](docs/AnaSayfa.png)

### 🔑 Giriş Paneli
![Giriş Paneli](docs/GirisPaneli.png)

### 📚 Kitap Kataloğu
![Kitap Kataloğu](docs/Kitapkatalog.png)

### 👤 Kullanıcı Profili
![Kullanıcı Profili](docs/KullaniciProfil.png)

---


