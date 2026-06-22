# Frontend → ASP.NET MVC İnteqrasiyası (Mərhələ 1 — yalnız UI)

**Layihə:** `Backend/Repository-management-backend/Repository-management-backend/` (.NET 8, MVC)
**Bu mərhələ:** yalnız UI köçürülməsi (business məntiqi YOXDUR).

> ⚠️ **Vacib dürüstlük qeydi:** Mən bu mühitdə `dotnet build` edə bilmədim (sandbox-da .NET/disk əlçatmaz). Aşağıdakı kod **build ilə yoxlanmayıb** — yalnız məntiqi analiz olunub. Siz lokal kompüterdə `dotnet build` etməlisiniz; xəta çıxsa, mənə deyin, düzəldim. Heç nə "yoxlanıb" kimi təqdim olunmur.

---

## 1. YARADILAN / DƏYİŞDİRİLƏN FAYLLAR

**Yaradılan:**
- `Views/Shared/_Sidebar.cshtml` — yan menyu (partial)
- `Views/Shared/_Header.cshtml` — üst header (partial), "+ Yeni qaimə" linki `id="newInvoiceLink"`
- `Views/Shared/_Footer.cshtml` — sidebar-footer / Çıxış (partial)
- `Views/Account/Login.cshtml` — login səhifəsi (Layout=null, standalone)
- `Views/Invoice/Create.cshtml` — yeni qaimə səhifəsi (Layout=null, standalone)
- `Controllers/AccountController.cs` — `Login()` action
- `Controllers/InvoiceController.cs` — `Create()` action

**Dəyişdirilən:**
- `Views/Shared/_Layout.cshtml` — bootstrap layout → **dashboard shell** (sidebar+header+content, fontlar, `~/css/dashboard.css`)
- `Views/Home/Index.cshtml` — Welcome → **dashboard** (bütün bölmələr + modallar + skript section)
- `Program.cs` — default route `Home/Index` → **`Account/Login`** (frontend axınına uyğun)

---

## 2. MƏNTİQİ ANALİZ / POTENSİAL RİSKLƏR

1. **Razor `@` escape:** Google Fonts linkində `wght@400` var; Razor `@`-ı kod kimi qəbul edir. Bütün view-lərdə `wght@@400` kimi escape edilib. ✅ (əks halda build xətası olardı.)
2. **Layout:** `_ViewStart.cshtml` qlobal `Layout="_Layout"` təyin edir. Login və Create view-lərində `Layout = null` ilə ləğv olunub (onların öz tam HTML-i var). ✅
3. **Tag helper-lər:** `_Header` `asp-controller/asp-action` işlədir; `_ViewImports.cshtml`-də `@addTagHelper *` var. ✅
4. **Partial-lar:** `_Layout` → `_Sidebar` → `_Footer`, `_Header`. Hamısı `Views/Shared`-dədir. ✅
5. **Model yoxdur:** View-lər statik HTML-dir (`@model` yoxdur), datanı JS idarə edir. ✅
6. **NuGet:** Əlavə paket lazım deyil — MVC `Microsoft.NET.Sdk.Web` içindədir. ✅
7. **Privacy / Error view-ləri:** Hələ də `_Layout` işlədir → indi dashboard shell-i ilə render olunur (sidebar görünər). **Build pozulmur**, sadəcə vizual qəribəlik. İstəsəniz `Home/Privacy` linkini/səhifəsini silin. ⚠️ (aşağı risk)
8. **Statik fayllar:** `wwwroot`-a köçürülməsə, build keçər, amma runtime-da CSS/JS/şəkil **404** olar (səhifə üslubsuz görünər). Bölmə 3-ə bax.
9. **JS naviqasiya URL-ləri:** JS-dəki `dashboard.html`, `login.html`, `new-invoice.html` keçidləri MVC marşrutu deyil → düzəldilməlidir (Bölmə 4). Bu **runtime** məsələsidir, build-i pozmur.
10. **Ölü JS faylları:** `app-shell.js`, `customers.js`, `invoices.js`, `debts.js`, `deposits.js`, `customer-detail.js`, `invoice-detail.js`, `warehouse.js`, `categories.js`, `users.js` — köhnə shell səhifələrinə aiddir, heç bir MVC view onları yükləmir. Zərərsizdir; istəsəniz köçürməyin/silin.

---

## 3. STATİK FAYLLAR — `wwwroot/assets/{css,js,img}`

Bütün CSS/JS/şəkil indi **`wwwroot/assets/css`, `wwwroot/assets/js`, `wwwroot/assets/img`** strukturunda gözlənilir. View-lər `~/assets/...` yollarını işlədir.

**Müvəqqəti işləmə (köçürmədən):** `Program.cs`-də əlavə statik provayder `/assets`-i frontend qovluğundan xidmət edir, ona görə **köçürmədən əvvəl də** səhifə üslubla açılır.

**Fiziki köçürmə (siz işlədin — tövsiyə, deploy üçün də lazımdır):**
```powershell
$src = "C:\Users\Elmar\OneDrive\İş masası\Repository_Management_System\Frontend\KapitalAsMMC\dashboard"
$dst = "C:\Users\Elmar\OneDrive\İş masası\Repository_Management_System\Backend\Repository-management-backend\Repository-management-backend\wwwroot\assets"

New-Item -ItemType Directory -Force -Path "$dst\css","$dst\js","$dst\img" | Out-Null
Copy-Item "$src\css\*" "$dst\css\" -Recurse -Force
Copy-Item "$src\js\*"  "$dst\js\"  -Recurse -Force
Copy-Item "$src\img\*" "$dst\img\" -Recurse -Force
```

> Köçürmədən sonra `wwwroot/assets` faylları üstünlük təşkil edir (default `UseStaticFiles` əvvəl yoxlanılır); frontend provayderi zərərsiz fallback kimi qalır. Böyük `dashboard.js`/`dashboard.css`-i mən fayl alətləri ilə təhlükəsiz köçürə bilmədiyim üçün bu əmri **siz** icra edin.

---

## 4. JS NAVİQASİYA URL-LƏRİ — ARTIQ DÜZƏLDİLİB ✅

`.html` keçidləri MVC marşrutlarına mən tərəfimdən frontend `js` faylların özündə artıq dəyişdirilib (köçürmədə hazır gələcək):
- `login.js`, `new-invoice.js` → `/Home/Index`
- `dashboard.js` → `/Account/Login`, `/Invoice/Create?id=...`
- `dashboard-roles.js` → "+ Yeni qaimə" seçicisi `#newInvoiceLink` (`_Header`-də `id="newInvoiceLink"` var)

> Əlavə əməliyyat lazım deyil — sadəcə Bölmə 3-dəki köçürməni edin.

---

## 5. BUILD VƏ İŞƏ SAL (siz işlədin)

```powershell
cd "C:\Users\Elmar\OneDrive\İş masası\Repository_Management_System\Backend\Repository-management-backend"
dotnet restore
dotnet build
dotnet run --project "Repository-management-backend\Repository-management-backend.csproj"
```

Açılış: `https://localhost:xxxx/` → **Login** səhifəsi.
- `nigar / user123` + **Pirşağı** seç → Dashboard (`/Home/Index`).
- "+ Yeni qaimə" → `/Invoice/Create`.
- Çıxış → `/Account/Login`.

---

## 6. NƏ YOXLANMAYIB / NÖVBƏTİ ADDIMLAR

- ❌ `dotnet build` mən tərəfimdən **icra olunmayıb** (sandbox əlçatmaz). Siz build edib nəticəni deyin.
- Build xətası, çatışmayan `using`, namespace uyğunsuzluğu görsəniz — tam mətnini göndərin, düzəldim.
- Bu mərhələ **yalnız UI**-dir; data hələ də JS + `sessionStorage`-dadır. Backend (EF Core + SQL Server, Controller/API, model) növbəti mərhələdir (`BACKEND_ANALIZ.md`-yə uyğun).
- İstəsəniz: Privacy/Error səhifələrini ayrıca sadə layout-a keçirim, ya da silim.
