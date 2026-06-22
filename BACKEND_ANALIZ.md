# Backend Analiz Sənədi — Tikinti Mallarının İcarə İdarəetmə Sistemi

**Texnologiya:** ASP.NET MVC (.NET 8) + Entity Framework Core + SQL Server
**Mənbə:** Mövcud frontend (Vanilla JS SPA — `Frontend/KapitalAsMMC/dashboard`)
**Məqsəd:** Frontend-in tam funksional analizi əsasında backend dizaynı (kod yox, analiz).

> Qeyd: Frontend hazırda bütün datanı `sessionStorage`-da filial-əhatəli açarlarda (`lesa_*__<filialId>`) saxlayır. Backend gələndə bu, SQL Server + EF Core ilə əvəz olunacaq; aşağıdakı model həmin keçidi nəzərə alır.

---

## 1. MODULLAR

| # | Modul | Səhifə/Bölmə | Təsvir |
|---|-------|--------------|--------|
| 1 | **Authentication** | `login.html` | İstifadəçi adı + şifrə ilə giriş; filial seçimi (işçinin filialına uyğun olmalıdır) |
| 2 | **Dashboard** | `dashboardSection` | Statistika kartları + bildirişlər (vaxtı çatan günlük mallar) |
| 3 | **Qaimələr** | `invoicesSection`, `new-invoice.html` | Qaimə siyahısı, yaratma, redaktə, ödəniş, qaytarma, müddət artırma, çap |
| 4 | **Müştərilər** | `customersSection` | Müştəri siyahısı + profil (aktiv/köhnə qaimələr, borc, depozit, tarixçə) |
| 5 | **Borclar** | `debtsSection` | Yalnız borcu > 0 olan müştərilər |
| 6 | **Depozitlər** | `depositsSection` | Yalnız depoziti > 0 olan müştərilər |
| 7 | **Anbar** | `inventorySection` | Mal qalıqları, icarədə olan, boş qalıq, kimdə olduğu |
| 8 | **Kateqoriyalar** | `productsSection` | Standart mallar + əlavə/xidmət kateqoriyaları + dəmir dirək ölçüləri |
| 9 | **Hesabatlar** | `reportsSection` | Ümumi statistika, bu ayın göstəriciləri, ən çox icarə |
| 10 | **İşçilər** | `usersSection` | (Yalnız Admin) işçi yaratma/silmə/redaktə |
| 11 | **Filiallar** | (sistem konteksti) | Bütün modulların datası filiala görə izolyasiya olunur |
| 12 | **Bildirişlər** | Dashboard `alertsBox` | Günlük malların vaxtının bitməsi |

---

## 2. ENTITY-LƏR

### 2.1 Branch (Filial)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| Code | string | "merdekan", "pirsagi", "baku" (unikal) |
| Name | string | "Mərdəkan filialı" və s. |
| IsActive | bool | |

### 2.2 User (İşçi)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| Name | string | Ad Soyad |
| Username | string | Unikal (login) |
| PasswordHash | string | **Backend-də hash olunmalı** (frontend mock-da açıq idi) |
| Role | enum/string | admin / manager / user |
| BranchId | int (FK → Branch) | İşçinin aid olduğu filial |
| Phone | string | |
| IsActive | bool | Deaktiv işçi giriş edə bilmir |
| CreatedAt | DateTime | |

### 2.3 Customer (Müştəri)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| BranchId | int (FK → Branch) | İzolyasiya |
| Name | string | Ad / Şirkət |
| Phone | string | |
| ExtraPhone | string | Əlavə telefon (axtarışda iştirak edir) |
| Address | string | |
| Note | string | Qeyd |
| CreatedAt | DateTime | |

> Frontend-də müştərinin borc/depoziti `history[]` (ledger) cəmindən hesablanır → bax **CustomerLedgerEntry**.

### 2.4 Invoice (Qaimə)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | Daxili ID (avtomatik) |
| BranchId | int (FK → Branch) | |
| InvoiceNo | string | Ə.ı. avtomatik generasiya (`QM-2026-000126`), redaktə oluna bilər, filial daxilində unikal |
| CustomerId | int (FK → Customer) | |
| CustomerNameSnapshot | string | Qaimə anındakı ad (snapshot) |
| Phone | string | |
| ExtraPhone | string | Bu qaimə üçün dəyişilə bilər |
| Address | string | |
| Note | string | |
| InvoiceDate | DateTime | İcarə tarixi |
| ReturnDate | DateTime | Ümumi qaytarma/təhvil tarixi |
| TotalAmount | decimal | = malların subtotal cəmi |
| PaidAmount | decimal | = PaymentHistory cəmi (computed) |
| DepositAmount | decimal | |
| RemainingDebt | decimal | = TotalAmount − PaidAmount (computed, ≥0) |
| IsClosed | bool | Bağlanıb → siyahıda görünmür, yalnız müştəri profili "Köhnə qaimələr"də |
| ClosedAt | DateTime? | |
| CreatedAt / UpdatedAt | DateTime | |

### 2.5 InvoiceItem (Qaimə malı)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| InvoiceId | int (FK → Invoice) | |
| Category | string | "Lesa", "60-lıq Lesa", "Təkərli lesa", "Dəmir dirək", "Taxta", "Boy dikt", "Bir tərəfi boy dikt", "Ksok dikt", "Nəqliyyat", "Xidmət", "Əlavə kateqoriya" |
| Label | string | Göstərilən ad |
| VariantId | string? | Əlavə/xidmət kateqoriya id-si |
| Size | string? | Ölçü/növ (məs. Taxta "5/15 / 3.00 m", Dəmir dirək "3.85") |
| Unit | string | ədəd / m / m² / gün / xidmət |
| Quantity | decimal | Günlük mal üçün = gün sayı |
| CustomPrice | decimal | Vahid qiymət |
| Subtotal | decimal | |
| Note | string? | |
| IsReturnable | bool | Anbara qayıdırmı (Xidmət/Nəqliyyat = false) |
| IsRecurring | bool | Aylıq icarədə təkrar hesablanırmı (uzatmada) |
| IsFixedFee | bool | Birdəfəlik (xidmət) |
| RentMode | string? | "daily" → günlük mal |
| DueDate | DateTime? | Günlük mal üçün bitmə tarixi (bildiriş bunula) |
| DayCount | int? | Günlük mal üçün gün sayı |
| DailyPrice | decimal? | Günlük qiymət |

**Lesa komponent sahələri** (Category = "Lesa"/"60-lıq Lesa"): `LesaHeadCount`, `LesaHeadPrice`, `LesaLongRodCount`, `LesaShortRodCount`, `LesaFreeTaxtaCount`, `LesaExtraTaxtaCount`, `LesaExtraTaxtaPrice`. Komponentlər ayrıca **InvoiceItemComponent** kimi də modelləşdirilə bilər (key, label, quantity, returnedQuantity, unit, unitPrice).
> Qiymət qaydası: **Başlıq + Əlavə taxta** hesablanır; adi taxta PULSUZ.

**Təkərli/Dəmir dirək sahələri:** `HeadCount`, `RodCount`, `VilkaCount`, `BoardCount`, `ExtraBoardCount`, `PalesCount`, `ReturnedQuantity` və müvafiq `Returned*` sahələri.

### 2.6 Payment (Ödəniş — PaymentHistory)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| InvoiceId | int (FK → Invoice) | |
| Amount | decimal | |
| Direction | enum | "in" (ödəniş) / "out" (düzəliş/geri) |
| Date | DateTime | |
| Note | string | |

> Qaimənin `PaidAmount` = Σ(in) − Σ(out).

### 2.7 CustomerLedgerEntry (Müştəri tarixçəsi / ledger)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| CustomerId | int (FK → Customer) | |
| InvoiceId | int? (FK → Invoice) | |
| Date | DateTime | |
| Type | string | "Mal götürüb", "Borc əlavə olunub", "Borc ödədi", "Mal qaytarıb", "Depozit əlavə olunub", "Depozit çıxılıb", "Depozitlə borc ödədi", "Qaimə bağlanıb" |
| Amount | decimal | |
| DebtChange | decimal | + borc artımı / − azalma |
| DepositChange | decimal | + depozit artımı / − azalma |
| Note | string | |
| Source | string | "invoice" (avtomatik) / "manual" |

> **Müştəri borcu = Σ DebtChange**, **depoziti = Σ DepositChange**.
> Frontend bu girişləri qaimələrdən avtomatik qurur (`source="invoice"`). Backend-də: ya hesablanmış (computed) saxlanılsın, ya da əməliyyatlar zamanı yazılsın. Tövsiyə: əməliyyat baş verəndə ledger entry yaradılsın (audit izi).

### 2.8 Category-lər
Frontend-də 4 növ var; backend-də bir **Category** cədvəli + `CategoryKind` enum ilə birləşdirilə bilər:

| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| BranchId | int (FK) | |
| Kind | enum | Standard / Extra / Service / Pole(DəmirDirəkÖlçü) |
| Name | string | Standard üçün "category", Pole üçün ölçü ("3.85") |
| Info | string? | (Standard) |
| Price | decimal | |
| Unit | string | |
| Note | string? | |
| RentType | enum | "monthly" / "daily" (yeni kateqoriya yaradılarkən məcburi seçim) |
| ParentId | int? | (alt-kateqoriyalar üçün, məs. Dəmir Dirək ölçüləri) |

> **Standart mallar** (Lesa, Dəmir dirək, Boy dikt, Bir tərəfi boy dikt, Ksok dikt, Taxta, Təkərli lesa, Nəqliyyat) sistemdə defolt mövcuddur.
> **Təkərli lesa** = günlük; **Adi/60-lıq Lesa** = aylıq.

### 2.9 InventoryStock (Anbar qalığı)
| Sahə | Tip | Qeyd |
|------|-----|------|
| Id | int (PK) | |
| BranchId | int (FK) | |
| Name | string | Mal adı (məs. "Boy dikt", "Taxta 5/15", "Dəmir dirək 3.85", "Lesa başlıq") |
| TotalCount | decimal | Ümumi sahib olunan say |

> **İcarədə olan (Rented)** anbarda saxlanılmır — aktiv (bağlanmamış) qaimələrin mallarından hesablanır.
> **Boş qalıq (Available) = TotalCount − Rented**. Mənfi ola bilər (bloklama yox).

---

## 3. ENTITY-LƏR ARASINDAKI ƏLAQƏLƏR

```
Branch (1) ───< (N) User
Branch (1) ───< (N) Customer
Branch (1) ───< (N) Invoice
Branch (1) ───< (N) Category
Branch (1) ───< (N) InventoryStock

Customer (1) ───< (N) Invoice
Customer (1) ───< (N) CustomerLedgerEntry

Invoice (1) ───< (N) InvoiceItem
Invoice (1) ───< (N) Payment
Invoice (1) ───< (N) CustomerLedgerEntry   (invoiceId nullable)
Invoice (1) ───< (N) ExtensionHistory       (müddət artırma)
Invoice (1) ───< (N) ReturnHistory          (qaytarma)

Category (Pole) (N) ──> (1) Category (Dəmir Dirək ana) [self ParentId]
InvoiceItem (N) ──> (1) Category (VariantId/Category ad ilə — zəif əlaqə)
```

**Əsas qaydalar:**
- Hər biznes entity-də `BranchId` var → filial izolyasiyası.
- `Invoice` müştəri məlumatlarını həm FK (CustomerId), həm snapshot (ad/telefon/ünvan) saxlayır (tarixi dəqiqlik üçün).
- `InventoryStock` ilə `InvoiceItem` arasında birbaşa FK yoxdur — ad uyğunluğu ilə (Name) bağlanır (frontend məntiqinə uyğun). İstəsəniz `ProductId` ilə güclü əlaqə qurula bilər.

---

## 4. HƏR SƏHİFƏNİN BACKEND ƏMƏLİYYATLARI

### Login (`login.html`)
- `POST /api/auth/login` { username, password, branchCode } → istifadəçini yoxla, `branch == user.branch` olmalı, JWT/cookie + rol qaytarsın.
- `GET /api/branches` → giriş formasında filial siyahısı.

### Dashboard
- `GET /api/dashboard/stats?branchId` → aktiv qaimə sayı, ümumi borc, ümumi depozit, bu ay gəlir (ödənişlər), bu ay verilən qaimələr, vaxtı keçmiş qaimələr, vaxtı bitmiş günlük mallar, ən çox icarəyə verilən mal.
- `GET /api/dashboard/alerts?branchId` → vaxtı çatan/keçən günlük mallar (RentMode=daily, DueDate ≤ bu gün, qaimə bağlı deyil).

### Qaimələr (siyahı/detal/yaratma)
- `GET /api/invoices?branchId&search&status&from&to` → siyahı (bağlanmış = false), status üzrə filtr, tarix aralığı, axtarış (InvoiceNo, Customer, Phone, ExtraPhone, Address, items, son rəqəmlər), **təcillilik üzrə sıralama** (gecikmiş → bu gün → ≤3 gün → qalan).
- `GET /api/invoices/{id}` → detal (items, payments, extension/return history).
- `POST /api/invoices` → yarat (anbardan çıxma yox, hesablama; sıfır qalıq xəbərdarlığı frontenddə, backend bloklamır).
- `PUT /api/invoices/{id}` → redaktə.
- `POST /api/invoices/{id}/payments` → ödəniş əlavə et.
- `POST /api/invoices/{id}/extend` → müddət artır (yalnız aylıq icarə əlavə olunur; nəqliyyat/xidmət təkrar yox; günlük malların DueDate uzanır).
- `POST /api/invoices/{id}/return` → qismən/tam qaytarma.
- `POST /api/invoices/{id}/close` → qaiməni bağla.
- `GET /api/invoices/next-number?branchId` → növbəti avtomatik nömrə.
- Çap: frontend print-friendly (backend-dən detal data kifayətdir).

### Müştərilər
- `GET /api/customers?branchId&search` → siyahı.
- `GET /api/customers/{id}` → profil: aktiv qaimələr, köhnə (bağlanmış) qaimələr, ümumi borc, ümumi depozit, tarixçə (ledger).
- `POST /api/customers`, `PUT /api/customers/{id}`, `DELETE /api/customers/{id}`.
- `POST /api/customers/{id}/transactions` → borc əlavə, ödəniş, depozit əlavə/çıx, depozitlə borc ödə (ledger entry yaradır).

### Borclar
- `GET /api/debts?branchId` → yalnız borc > 0 olan müştərilər (Σ DebtChange > 0). CSV export / çap.

### Depozitlər
- `GET /api/deposits?branchId` → yalnız depozit > 0 olan müştərilər. CSV export / çap.

### Anbar
- `GET /api/inventory?branchId&search` → mal, ümumi say, icarədə (hesablanır), boş qalıq, kimdə olduğu (holders: müştəri, qaimə, say, qaytarma tarixi).
- `POST /api/inventory` → anbara mal əlavə (kateqoriyadan və ya custom ad).
- `PUT /api/inventory/{id}` → say redaktə.
- `DELETE /api/inventory/{id}`.

### Kateqoriyalar
- `GET /api/categories?branchId&kind` → standart/əlavə/xidmət/pole.
- `POST/PUT/DELETE` → əlavə, xidmət, dəmir dirək ölçüsü (RentType: aylıq/günlük məcburi). Pole ölçüsü əlavə edəndə anbar stoku da yaranır.

### Hesabatlar
- `GET /api/reports?branchId` → ümumi statistika; backup export/import (admin).

### İşçilər (yalnız Admin)
- `GET /api/users` → bütün işçilər (filial sütunu ilə).
- `POST /api/users`, `PUT /api/users/{id}`, `DELETE /api/users/{id}` (özünü silmək olmaz).

---

## 5. CRUD ƏMƏLİYYATLARI (XÜLASƏ)

| Entity | Create | Read | Update | Delete | Qeyd |
|--------|:-:|:-:|:-:|:-:|------|
| Customer | ✓ | ✓ | ✓ | ✓ | |
| Invoice | ✓ | ✓ | ✓ | ✓ | + close, extend, return, payment |
| InvoiceItem | ✓ | ✓ | ✓ | ✓ | Invoice ilə birgə (owned) |
| Payment | ✓ | ✓ | — | (✓) | Adətən əlavə-only |
| InventoryStock | ✓ | ✓ | ✓ | ✓ | |
| Category (extra/service/pole) | ✓ | ✓ | ✓ | ✓ | İstifadədə olan pole silinə bilməz |
| Standard product | — | ✓ | ✓ (qiymət) | — | Defolt sabit |
| User | ✓ | ✓ | ✓ | ✓ | Yalnız Admin |
| Branch | (seed) | ✓ | — | — | Sabit siyahı |
| LedgerEntry | ✓ (əməliyyatla) | ✓ | — | (✓) | Audit |

---

## 6. ROLLAR VƏ İCAZƏLƏR

| İcazə | Admin | Manager | User |
|-------|:-:|:-:|:-:|
| view (baxış) | ✓ | ✓ | ✓ |
| create (yaratma) | ✓ | ✓ | ✗ |
| edit (redaktə) | ✓ | ✓ | ✗ |
| delete (silmə) | ✓ | ✓ | ✗ |
| addPayment (ödəniş əlavə) | ✓ | ✓ | ✓ |
| print (çap) | ✓ | ✓ | ✓ |
| manageUsers (işçi idarəsi) | ✓ | ✗ | ✗ |

**Menyu görünürlüyü (rol əsaslı):**
- **Admin:** Dashboard, Qaimələr, Müştərilər, Borclar, Depozitlər, Anbar, Kateqoriyalar, Hesabatlar, **İşçilər**.
- **Manager:** İşçilərdən başqa hamısı.
- **User:** yalnız **Dashboard, Qaimələr, Borclar, Depozitlər** (baxış + çap + ödəniş).

> Frontend gizlətməsi yalnız UX-dir. **Backend hər endpoint-də icazəni yoxlamalıdır** (server-side authorization).

---

## 7. BRANCH (FİLİAL) MƏNTİQİ

- Filiallar: **Mərdəkan, Pirşağı, Bakı Mərkəz** (genişlənə bilər).
- Hər işçi bir filiala bağlıdır (`User.BranchId`). Giriş zamanı seçilən filial işçinin filialına uyğun olmalıdır.
- Aktiv filial sessiya konteksti kimi saxlanılır (backend-də JWT claim / session).
- **Bütün sorğular aktiv filiala görə filtrlənməlidir** — bir filialın müştəri/qaimə/anbar/hesabat datası digərinə görünməməlidir.
- Tövsiyə: EF Core **Global Query Filter** (`HasQueryFilter(e => e.BranchId == _currentBranchId)`) ilə avtomatik izolyasiya; `_currentBranchId` cari istifadəçinin claim-indən gəlsin.
- İstifadəçi idarəsi (İşçilər) Admin üçün filiallar arası ola bilər (qlobal), amma biznes datası ciddi izolyasiyalıdır.

---

## 8. VALIDATION TƏLƏBLƏRİ

**Login:** username, password, branch məcburi; branch == user.branch; user.IsActive == true.

**Customer:** Name və Phone məcburi; filial daxilində ad təkrarı yox (frontend yoxlayır).

**Invoice:**
- InvoiceNo məcburi, filial daxilində unikal (təkrar nömrə xətası).
- Müştəri seçilməli; telefon boş olmamalı; InvoiceDate və ReturnDate məcburi.
- Ən azı 1 mal olmalı.
- **Sıfır/kifayətsiz qalıq BLOKLAMIR** — yalnız xəbərdarlıq (hər mal sətri ayrıca). Backend qalığı mənfi qəbul edir.
- Məbləğlər ≥ 0; RemainingDebt = max(0, Total − Paid).

**InvoiceItem:** quantity > 0; price ≥ 0; günlük mal üçün gün sayı > 0 və InvoiceDate olmalı (DueDate hesablanır).

**Payment:** amount ≥ 0.

**Category:** Name məcburi; price ≥ 0; RentType (aylıq/günlük) məcburi seçim.

**InventoryStock:** Name məcburi; count ≥ 0.

**User:** Name, Username, Password məcburi; Username unikal; Role və Branch düzgün dəyər.

> Backend: DataAnnotations + FluentValidation, həm də DB constraint-ləri (unique index: `(BranchId, InvoiceNo)`, `Username`).

---

## 9. AUTHENTICATION VƏ AUTHORIZATION

**Authentication:**
- Username + Password (backend-də **hash + salt**, məs. ASP.NET Identity və ya `PasswordHasher`).
- Giriş zamanı filial seçimi; filial işçinin filialına uyğun olmalıdır.
- Deaktiv (`IsActive=false`) işçi giriş edə bilmir.
- Sessiya: Cookie auth və ya JWT. Claim-lər: `userId`, `role`, `branchId`, `branchCode`, `name`.

**Authorization:**
- Rol əsaslı: `[Authorize(Roles="admin")]` (İşçilər), `admin,manager` (yaratma/silmə/redaktə), hamı (baxış/çap/ödəniş).
- Policy-lər: `CanCreate`, `CanEdit`, `CanDelete`, `CanManageUsers`, `CanAddPayment` (icazə cədvəlinə uyğun).
- **Filial təhlükəsizliyi:** hər sorğuda resursun `BranchId`-i cari istifadəçinin `branchId`-i ilə yoxlanmalı (başqa filialın resursuna birbaşa ID ilə daxil olmağın qarşısı alınmalı).
- Bütün yazma əməliyyatları üçün server-side icazə yoxlaması məcburdir (frontend gizlətməyə güvənmə).

---

## 10. ƏSAS BİZNES QAYDALARI (Computed / Logic)

1. **Qaimə statusu:** `IsClosed` → Bağlanıb; `ReturnDate < bu gün` → Gecikir; əks halda Aktiv.
2. **Total** = Σ InvoiceItem.Subtotal. **Paid** = Σ Payment(in) − Σ Payment(out). **Borc** = max(0, Total − Paid).
3. **Müştəri borcu/depoziti** = Σ LedgerEntry.DebtChange / Σ DepositChange.
4. **Nəqliyyat və Xidmət** yalnız ilk qaimədə; **müddət artırmada** yalnız aylıq icarə (recurring mallar) əlavə olunur, nəqliyyat/xidmət təkrar yox.
5. **Günlük mallar** (Təkərli lesa + RentType=daily əlavə kateqoriyalar): DueDate var; vaxtı çatanda Dashboard bildirişi; qaytarılana və ya uzadılana qədər görünür.
6. **Lesa qiyməti** = Başlıq + Əlavə taxta (adi taxta pulsuz). 60-lıq Lesa = Adi Lesa kimi, ayrıca kateqoriya.
7. **Dəmir dirək** ölçü ilə (1.70 / 3.85 / 5.50...); hər ölçü ayrıca qiymət və anbar stoku.
8. **Anbar:** Available = Total − Rented(aktiv qaimələrdən). Mənfi mümkün (bloklama yox, xəbərdarlıq).
9. **Axtarış:** InvoiceNo, Customer, Phone, ExtraPhone, Address, mallar; son rəqəmlərlə (məs. "1257" → "QM-2026-001257").
10. **Qaimə siyahısı:** bağlanmışlar görünmür; təcillilik üzrə sıralama + rəng (gecikmiş/bu gün/≤3 gün/qalan).

---

## 11. TÖVSİYƏ OLUNAN DbSet-lər (xülasə)

```
Branches, Users, Customers, Invoices, InvoiceItems,
Payments, CustomerLedgerEntries, Categories, InventoryStocks,
ExtensionHistories, ReturnHistories
```

**Seed data:** 3 filial, hər filiala işçilər (Mərdəkan: admin+manager, Pirşağı: manager+user, Bakı: user), standart mallar, nümunə müştəri/qaimə/anbar.

---

*Bu sənəd frontend-in tam funksional analizidir. Backend qurularkən hər endpoint-də filial izolyasiyası və server-side authorization mütləqdir; frontend-dəki gizlətmə/yoxlamalar yalnız UX məqsədlidir.*
