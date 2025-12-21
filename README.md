# 📚 Kütüphane Otomasyon Sistemi - Teslimat Paketi

Bu klasör, projenin "Depo & Paketleme Kontrolü" standartlarına uygun olarak hazırlanmış teslimat versiyonudur.

## 📂 Klasör Yapısı

- **`/schema`**: `database.sql` - Veritabanı oluşturma scripti.
- **`/programmability`**: SQL programlanabilirlik objeleri.
- **`/app`**: Uygulama kaynak kodları (Clean Architecture).
- **`/docs`**: Kullanım dökümanları ve ekran görüntüleri.
- **`/tests`**: Unit ve Integration testleri.

## 🚀 Hızlı Kurulum

### 1. Veritabanı Kurulumu
`schema/database.sql` dosyasını SQL Server Management Studio (SSMS) üzerinden çalıştırarak veritabanını ve tabloları oluşturun.

### 2. Uygulama Yapılandırması
`app/Presentation/Kutuphane.WebUI/appsettings.json` dosyasını açın ve `ConnectionStrings` bölümünü kendi sunucunuza göre düzenleyin:

```json
"DefaultConnection": "Server=YOUR_SERVER;Database=KutuphaneOtomasyonDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### 3. Çalıştırma
Visual Studio 2022 ile `app/Kutuphane.sln` dosyasını açın ve `Kutuphane.WebUI` projesini başlangıç projesi yaparak çalıştırın.

---

## 🖼️ Ekran Görüntüleri

Uygulama arayüzüne ait ekran görüntüleri `docs/` klasöründe yer almaktadır.

- `dashboard.png`: Ana yönetim paneli.
- `book_list.png`: Kitap listeleme ve arama ekranı.
- `member_profile.png`: Üye detay sayfası.

---
*Bu paket Antigravity tarafından otomatik olarak organize edilmiştir.*
