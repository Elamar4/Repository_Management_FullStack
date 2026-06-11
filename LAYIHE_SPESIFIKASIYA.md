# Tikinti Mallarının İcarə İdarəetmə Sistemi — Layihə Spesifikasiyası

**Texnologiya:** ASP.NET MVC (Fullstack), SQL verilənlər bazası (Entity Framework)
**Məqsəd:** Tikinti mallarının icarəsi üçün anbar, müştəri, qaimə, borc və depozitlərin idarə edilməsi.

---

## 1. Sistemin Əsas Modulları

1. Müştərilər
2. Qaimələr (icarə sənədləri)
3. Anbar
4. Borclar
5. Depozitlər
6. Kateqoriyalar (1 aylıq / günlük)
7. Hesabatlar / Dashboard
8. İstifadəçilər və Rollar (Admin / Manager / User)

---

## 2. Rol Sistemi (İcazələr)

| Əməliyyat | Admin | Manager | User |
|---|---|---|---|
| İstifadəçi yaratmaq / silmək | ✅ | ❌ | ❌ |
| Qaimə yaratmaq | ✅ | ✅ | ❌ |
| Qaimə silmək | ✅ | ✅ | ❌ |
| Anbarı idarə etmək | ✅ | ✅ | ❌ |
| Kateqoriya idarə etmək | ✅ | ✅ | ❌ |
| Qaimələrə baxmaq | ✅ | ✅ | ✅ |
| Çap etmək | ✅ | ✅ | ✅ |
| Ödəniş əlavə etmək | ✅ | ✅ | ✅ |
| Borclara / Depozitlərə baxmaq | ✅ | ✅ | ✅ |

**Qısa qayda:**
- **Admin** — hər şeyi edə bilər.
- **Manager** — Admin kimidir, AMMA istifadəçi yarada/silə bilməz.
- **User** — yalnız baxır, çap edir, ödəniş əlavə edir. Yarada/silə/dəyişə bilməz.

---

## 3. Anbar Sistemi

Sistemin ən vacib hissəsidir.

**Hər malın sahələri:**
- Ad
- Kateqoriya
- Say (mövcud qalıq)

**Nümunə anbar:**

| Mal | Say |
|---|---|
| Lesa başlıq | 250 |
| Lesa uzun çubuq | 600 |
| Lesa balaca çubuq | 350 |
| Əlavə taxta | 70 |
| Dəmir dirək 3.85 | 120 |

### Qaimə yaradılarkən anbardan çıxılma
Qaiməyə əlavə edilən hər mal anbar qalığından avtomatik çıxılır.

> Anbarda Lesa başlıq = 100 → Qaimədə 15 əlavə olunur → Yeni qalıq = 85

### ⚠️ Sıfır qalıq qaydası (çox vacib)
- Anbarda 0 (və ya kifayət qədər olmayan) mal olsa belə, **qaimə yaratmaq bloklanmır**.
- Sadəcə xəbərdarlıq çıxır:

> ⚠️ Anbarda kifayət qədər mal yoxdur. Yenə də davam edilsin?
> **[Bəli]** / **[Xeyr]**

- "Bəli" seçilərsə qaimə yaranır (qalıq mənfi və ya 0 ola bilər).

---

## 4. Kateqoriya Sistemi

Standart kateqoriyalar + əlavə kateqoriyalar yaradıla bilər.

**Yeni kateqoriya yaradanda mütləq seçilməlidir — mal tipi:**
- 🗓️ **1 Aylıq mal** (aylıq icarə)
- 📅 **Günlük mal** (gündəlik icarə, məs: təkərli lesa)

Bu seçim qaimə hesablanması və bildirişlər üçün vacibdir (günlük mallar üçün vaxt-bitmə bildirişi işləyir).

### Alt kateqoriyalar (ölçü + qiymət)
Bəzi kateqoriyaların ölçüyə görə alt variantları var. Hər alt kateqoriyada:
- Ölçü
- Qiymət

**Nümunə — Dəmir Dirək:**
- 1.70
- 3.85
- 5.50 və s.

Qaimədə seçim: `Dəmir Dirək → 3.85 → Say: 20`.
Anbarda hər ölçü **ayrıca** saxlanılır (məs: "Dəmir Dirək 3.85").

---

## 5. Lesa Sistemi (3 növ)

### a) Adi Lesa
Komponentlər: Başlıq, Uzun çubuq, Balaca çubuq, Taxta, Əlavə taxta.
**Qiymət hesabı:** `Başlıq qiyməti + Əlavə taxta qiyməti`
- Adi taxta pulsuzdur.

### b) 60-lıq Lesa
Adi lesa kimi işləyir, sadəcə ayrıca kateqoriyadır.

### c) Təkərli Lesa (xüsusi)
- **Günlük** verilən maldır.
- Vaxtı bitəndə Dashboard-da bildiriş çıxmalıdır.

---

## 6. Müştəri Sistemi

**Sahələr:**
- Ad
- Soyad
- Telefon
- Əlavə telefon
- Ünvan
- Qeyd

**Müştəri səhifəsində göstərilir:**
- Aktiv qaimələr (hazırda üzərində olan)
- Köhnə qaimələr (əvvəlki)
- Ümumi borc
- Ümumi depozit

---

## 7. Depozit Sistemi

- Depozit **borc deyil**, müştərinin verdiyi girovdur.
- Müştəri 100 AZN depozit verib → Sistemdə `Depozit = 100`.
- **Depozitlər səhifəsi:** yalnız `Depozit > 0` olanlar görünür. 0 olanlar gizlidir.

---

## 8. Borc Sistemi

Borc avtomatik hesablanır.

**Formula:** `Borc = Ümumi qaimə məbləği − Ödənilən məbləğ`

> Qaimə = 500, Ödəniş = 350 → Borc = 150

- **Borclar səhifəsi:** yalnız `Borc > 0` olanlar görünür. 0 olanlar gizlidir.

---

## 9. Qaimə Sistemi (əsas modul)

**Qaimənin sahələri:**
- Müştəri
- Mallar (siyahı + say)
- İcarə tarixi
- Qaytarma tarixi
- Depozit
- Əlavə xidmətlər
- Nəqliyyat
- Qeyd

### Qaimə ID və Nömrə
- Yeni qaimə yaradılanda **ID avtomatik** verilir və qaiməyə baxanda görünür.
- **Qaimə nömrəsi** xanası boş gəlir — istifadəçi əl ilə daxil edir (məs: QA-125).

---

## 10. Axtarış Sistemi

Qaimələrdə axtarış aşağıdakıların hamısında işləməlidir:
- Ad / Soyad (məs: Əli)
- Telefon (məs: 0501234567)
- Əlavə telefon (məs: 0557654321)
- Qaimə nömrəsi (məs: QA-125)

**Xüsusi qayda:** nömrə ilə axtarışda əsasən **son 4 rəqəm** ilə axtarış.
> Təklif olunan həll: axtarış mətni qaimə nömrəsinin sonu (`EndsWith`) və ya tam uyğunluq kimi yoxlanılsın. Beləliklə "125" yazanda "QA-125" tapılır.

---

## 11. Qaimə Müddətinin Artırılması (Vaxtı artır)

Qaimə yaradıldıqdan sonra **"Vaxtı artır"** düyməsi var. Basanda modal açılır:

**Modalda göstərilir:**
- Cari bitmə tarixi
- Yeni bitmə tarixi
- Əlavə yazılacaq məbləğ

**Hesablama:**
- 1 ay uzadılır → `Əlavə borc = qaimənin aylıq məbləği` → borca əlavə olunur.

---

## 12. Nəqliyyat və Əlavə Xidmətlər Qaydası (çox vacib)

- Nəqliyyat və əlavə xidmətlər **yalnız ilk qaimədə** hesablanır.
- Vaxt uzadılanda **TƏKRAR hesablanmır**.

> Nəqliyyat = 30 → 1-ci ay hesablanır → 2-ci, 3-cü ay hesablanmır.

*(Bu artıq qismən qurulub — yenidən yoxlanılacaq.)*

---

## 13. Bildirişlər (Dashboard)

### Təkərli Lesa vaxt bitmə bildirişi
Günlük verilən mallar üçün (təkərli lesa və digər günlük kateqoriyalar):

> Təkərli lesa 10 günə verilib, bitmə tarixi 20.06.2026.
> Həmin tarix gələndə Dashboard-da:
> ⚠️ **Təkərli Lesa vaxtı bitib** — Müştəri: X, Qaimə: Y

Bu, günlük hesablanan yeganə mal tipi olduğu üçün kritikdir.

---

## 14. Hesabatlar / Dashboard

Göstərilməlidir:
- Ümumi aktiv qaimə sayı
- Ümumi borc
- Ümumi depozit
- Bu ay gəlir
- Bu ay verilən qaimələr
- Vaxtı keçmiş qaimələr
- Günlük malların (təkərli lesa) vaxt-bitmə bildirişləri

---

## 15. Verilənlər Bazası Modeli (təxmini)

- **User** (Id, Ad, Login, Şifrə, Rol)
- **Customer** (Id, Ad, Soyad, Telefon, ƏlavəTelefon, Ünvan, Qeyd)
- **Category** (Id, Ad, Tip [Aylıq/Günlük], ParentId — alt kateqoriya üçün)
- **Product / StockItem** (Id, Ad, CategoryId, Say/Qalıq, Qiymət, Ölçü)
- **Invoice** (Id, Nömrə, CustomerId, İcarəTarixi, QaytarmaTarixi, Depozit, Nəqliyyat, ƏlavəXidmət, Qeyd, Status)
- **InvoiceItem** (Id, InvoiceId, ProductId, Say, Qiymət)
- **InvoiceExtension** (Id, InvoiceId, KöhnəTarix, YeniTarix, ƏlavəMəbləğ)
- **Payment** (Id, InvoiceId, Məbləğ, Tarix)
- **Deposit** (Id, CustomerId, Məbləğ)

---

## 16. Əsas İş Axınları

**Qaimə yaratma:**
Müştəri seçilir → mallar əlavə olunur → anbar qalığı yoxlanılır (0-dırsa xəbərdarlıq, amma bloklamır) → nəqliyyat/əlavə xidmət bir dəfə əlavə olunur → qaimə yaranır → anbardan çıxılır → borc hesablanır.

**Vaxt uzatma:**
"Vaxtı artır" → modal → yeni tarix → aylıq məbləğ borca əlavə → nəqliyyat/xidmət təkrar əlavə OLUNMUR.

**Ödəniş:**
Qaiməyə ödəniş əlavə olunur → borc avtomatik azalır.

---

## 17. Mərhələli İcra Planı

1. **Mərhələ 1 — Təməl:** Proyekt qurulması, DB modeli, miqrasiyalar, rol/giriş sistemi.
2. **Mərhələ 2 — Anbar & Kateqoriya:** mal əlavə/redaktə, alt kateqoriyalar, mal tipi (aylıq/günlük).
3. **Mərhələ 3 — Müştəri:** müştəri CRUD + müştəri səhifəsi (qaimələr/borc/depozit).
4. **Mərhələ 4 — Qaimə:** qaimə yaratma, anbardan çıxma, sıfır-qalıq xəbərdarlığı, ID/nömrə.
5. **Mərhələ 5 — Borc & Depozit:** avtomatik hesablama, ödəniş, filtrlər (>0).
6. **Mərhələ 6 — Vaxt uzatma & Nəqliyyat qaydası:** modal, aylıq borc, təkrar hesablanmama.
7. **Mərhələ 7 — Axtarış:** ad/telefon/əlavə telefon/nömrə (son 4 rəqəm).
8. **Mərhələ 8 — Bildirişlər & Dashboard & Hesabatlar:** təkərli lesa vaxtı, statistikalar.
9. **Mərhələ 9 — Test & yoxlama:** hər modulun yoxlanması, nəqliyyat qaydasının təsdiqi.

---

## 18. Dəqiqləşdirilməli Suallar (sonra cavablandırılacaq)

- Qiymətlər mala görə anbarda saxlanır, yoxsa qaimədə əl ilə daxil edilir?
- "Aylıq məbləğ" hər mal üçün ayrıca hesablanır, yoxsa qaimə cəmi kimi saxlanır?
- Qaimə "Aktiv / Köhnə" statusu qaytarma tarixinə görəmi avtomatik dəyişir?
- Depozit qaytarılanda borca tutulurmu, yoxsa ayrıca idarə olunur?
