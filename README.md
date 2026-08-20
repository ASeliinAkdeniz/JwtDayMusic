# 🎵 JwtDayMusic

> **ASP.NET Core 9** ile geliştirilmiş; hiyerarşik paket üyeliği, JWT tabanlı dinamik yetkilendirme ve ML.NET destekli öneri motoruna sahip iki katmanlı müzik streaming platformu.

---

## 📌 Proje Nedir?

**JwtDayMusic**, kullanıcıların kayıt olup paket kademelerine göre müzik akışı dinleyebildikleri modern bir müzik streaming platformudur. 

Uygulamanın merkezinde **JWT tabanlı hiyerarşik yetkilendirme** yer alır: Her şarkının bir erişim kademesi bulunur (`Basic` → `Gold` → `Premium` → `Elit`). Kullanıcılar yalnızca kendi kademelerine eşit veya daha düşük seviyedeki şarkıları dinleyebilirler.

Mimari olarak uygulama bağımsız iki ana katmandan oluşur:
* **`JwtDayMusic.WebApi`**: Kimlik doğrulama, JWT üretimi, iş kuralları (business logic), makine öğrenimi ve veritabanı işlemlerini yürüten RESTful API katmanı.
* **`JwtDayMusic.WebUI`**: API'yi tüketen, son kullanıcıya retro/synthwave temalı arayüz sunan ASP.NET Core MVC katmanı. Tarayıcıdan gelen istekler WebUI proxy üzerinden JWT eklenerek API'ye güvenli şekilde iletilir.

---

## ✨ Kapsanan Özellikler

### 🔐 1. Üyelik ve Dinamik Yetkilendirme
- **Kayıt & Giriş:** Rol atanmadan kayıt, güvenli giriş ve JWT (Bearer) tabanlı oturum yönetimi.
- **Hiyerarşik Paket Sistemi:** `Basic` ➔ `Gold` ➔ `Premium` ➔ `Elit`.
- **Çalışma Anı (Runtime) Kademe Kontrolü:** Şarkı çalınırken kullanıcının seviyesi şarkının seviyesiyle anlık kıyaslanır (erişim yetersizse `403 Forbidden`).
- **Paket Yükseltme:** Simüle edilmiş paket satın alma akışı ile anında kademe artırma ve yeni yetkiyle güncel JWT üretimi.

### 🎧 2. Müzik Akışı & Keşif
- **Gelişmiş Müzik Çalar:** Kuyruk yönetimi, otomatik sıradaki parçaya geçiş, önceki/sonraki parça, karıştır (shuffle) ve tekrarla (repeat) modları.
- **Sanatçı & Tür Yönetimi:** Sanatçı detay sayfaları (biyografi, dinleyici istatistikleri) ve türe göre filtreleme.
- **Arama Motoru:** Türkçe karakter duyarlı şarkı, sanatçı ve tür araması.
- **ML.NET ile Akıllı Öneri:** *Matrix Factorization* algoritması kullanılarak kişiselleştirilmiş "Bunları da Sevebilirsiniz" öneri sistemi.

### 📚 3. Kişisel Kütüphane & Profil
- **Çalma Listeleri (Playlists):** Özel çalma listesi oluşturma, düzenleme ve parça ekleme/çıkarma.
- **Favoriler:** Tek tıkla şarkı beğenme/favorilere ekleme.
- **Kullanıcı Paneli:** Kullanıcı bilgileri, dinleme istatistikleri ve profil fotoğrafı yükleme desteği.

### 🌐 4. Gerçek Dünya İçeriği
- **iTunes Search API Entegrasyonu:** Gerçek şarkı verileri, albüm kapakları, sanatçı bilgileri ve 30 saniyelik yüksek kaliteli `.mp3` önizlemeleri.

---

## 🛠️ Kullanılan Teknolojiler

| Alan | Teknoloji / Kütüphane |
| :--- | :--- |
| **Backend Framework** | ASP.NET Core 9 (Web API) |
| **Authentication & Auth** | ASP.NET Core Identity, JWT (JSON Web Token - Bearer) |
| **ORM & Veritabanı** | Entity Framework Core, Code First Migrations, MS SQL Server |
| **Makine Öğrenimi** | ML.NET (Matrix Factorization Öneri Algoritması) |
| **Mapping & Mimari** | AutoMapper, Katmanlı Servis Mimarisi (`DTO` → `Service` → `Controller`) |
| **Frontend Framework** | ASP.NET Core MVC (Razor Engine) |
| **API İletişimi** | `IHttpClientFactory`, `DelegatingHandler` |
| **Oturum & Script** | Session Tabanlı Token Depolama, Vanilla JavaScript (Modern Fetch API) |
| **Stil & Tasarım** | Tailwind CSS + Özel Retro/Synthwave CSS |
| **Dış Servisler** | iTunes Search API |

---

## 🧠 Öne Çıkan Teknik Detaylar

1. **Hiyerarşik JWT Yetkilendirmesi:** Klasik statik rol kontrollerinin ötesine geçilerek; kaynağın (şarkının) veritabanındaki seviyesi ile token içerisindeki seviye claim'i runtime'da dinamik olarak karşılaştırılır.
2. **JWT'nin Durağanlığı (Stateless Token Yönetimi):** Kullanıcı paketini yükselttiğinde eski token geçersiz kılınmadan yeni claim'lerle taze bir token üretilir ve oturum güncellenir.
3. **Katman Köprüsü (Reverse-Proxy / BFF Yaklaşımı):** İstemci doğrudan WebApi'ye erişmez. WebUI, gelen istekleri yakalar, session'daki JWT'yi `Authorization: Bearer <token>` başlığıyla ekler ve API'ye proxy eder.
4. **Kaynak Sahipliği Doğrulaması:** Çalma listeleri ve favoriler gibi kişisel verilerde sadece kimlik doğrulaması yetmez; kullanıcının yalnızca kendi oluşturduğu verileri modifiye edebilmesi garanti altına alınır.
5. **Modüler Ön Yüz Motoru:** Çalar motoru, beğeni aksiyonları ve playlist yönetimi tek bir merkezi JavaScript motorunda soyutlanarak sayfalar arası kod tekrarı önlenmiştir.

---

## Proje Görselleri

<img width="1659" height="885" alt="Ekran görüntüsü 2026-08-20 165601" src="https://github.com/user-attachments/assets/ce4ebecd-652d-4201-9287-d851a4c254f2" />

<img width="1842" height="897" alt="Ekran görüntüsü 2026-08-20 165739" src="https://github.com/user-attachments/assets/c0f7eef0-e685-4045-9a69-31fd1dc820d9" />

<img width="1834" height="906" alt="Ekran görüntüsü 2026-08-20 165753" src="https://github.com/user-attachments/assets/a319c6ee-1292-4c4a-a4b7-ca7a1db55a5d" />

