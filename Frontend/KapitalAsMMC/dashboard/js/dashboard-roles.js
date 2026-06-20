/* =====================================================================
   dashboard-roles.js — Dashboard SPA üçün rol/icazə qatı.
   dashboard.js-ə TOXUNMUR. data.js (window.DB) əvvəlcədən yüklənməlidir.

   Rol SEÇİLMİR — giriş edən istifadəçinin rolundan götürülür.
   Header-də yalnız istifadəçi adı + rol etiketi göstərilir.

   Menyu bölmələri rola görə göstərilir/gizlədilir:
     Admin   — bütün bölmələr (+ İstifadəçilər).
     Manager — İstifadəçilərdən başqa hamısı.
     User    — yalnız Dashboard və Qaimələr.
   Düymələr də icazəyə görə gizlədilir (yalnız UX — əsl yoxlama backend-də).
   ===================================================================== */
(function () {
  "use strict";

  var DB = window.DB;
  if (!DB) { console.warn("dashboard-roles: data.js (DB) tapılmadı"); return; }

  // Giriş edən istifadəçinin rolu (seçici yoxdur)
  var user = DB.getCurrentUser ? DB.getCurrentUser() : null;
  var role = user ? user.role : DB.getCurrentRole();
  if (DB.setRole) DB.setRole(role); // DB.can() ilə sinxron

  // Hansı rol hansı menyu bölməsini görür
  var SECTION_ACCESS = {
    dashboardSection: ["admin", "manager", "user"],
    invoicesSection: ["admin", "manager", "user"],
    debtsSection: ["admin", "manager", "user"],
    depositsSection: ["admin", "manager", "user"],
    customersSection: ["admin", "manager"],
    inventorySection: ["admin", "manager"],
    productsSection: ["admin", "manager"],
    reportsSection: ["admin", "manager"],
    usersSection: ["admin"],
  };

  function canSeeSection(sec) {
    return (SECTION_ACCESS[sec] || ["admin", "manager", "user"]).indexOf(role) !== -1;
  }

  // İcazəyə görə gizlədiləcək STATİK düymələr (unikal id/selector)
  var GATED = [
    { sel: '.header-right a[href="new-invoice.html"]', perm: "create" },
    { sel: "#addCustomerQuickBtn", perm: "create" },
    { sel: "#addExtraCategoryBtn", perm: "create" },
    { sel: "#addServiceCategoryBtn", perm: "create" },
    { sel: "#addPoleCategoryBtn", perm: "create" },
    { sel: "#addInventoryItemBtn", perm: "create" },
    { sel: "#exportBackupBtn", perm: "create" },
    { sel: "#importBackupBtn", perm: "create" },
    { sel: "#addUserBtn", perm: "manageUsers" },
  ];

  function applyPermissions() {
    GATED.forEach(function (g) {
      document.querySelectorAll(g.sel).forEach(function (el) {
        el.classList.toggle("perm-hidden", !DB.can(g.perm));
      });
    });
    document.body.classList.remove("role-admin", "role-manager", "role-user");
    document.body.classList.add("role-" + role);
  }

  // Menyu (nav) bölmələrini rola görə göstər/gizlət
  function applyMenuVisibility() {
    document.querySelectorAll(".nav-link[data-section]").forEach(function (link) {
      var sec = link.getAttribute("data-section");
      var allowed = canSeeSection(sec);
      link.classList.toggle("perm-hidden", !allowed);
      var section = document.getElementById(sec);
      // İcazəsi olmayan bölmə aktivdirsə → Dashboard-a keç
      if (!allowed && section && section.classList.contains("active-section")) {
        var dashLink = document.querySelector('.nav-link[data-section="dashboardSection"]');
        if (typeof window.switchSection === "function" && dashLink) {
          window.switchSection("dashboardSection", dashLink);
        }
      }
    });
  }

  function buildUserChip() {
    var right = document.querySelector(".header-right");
    if (!right || document.getElementById("currentUserChip")) return;
    // Aktiv filial nişanı
    if (DB.getCurrentBranchName) {
      var branchChip = document.createElement("span");
      branchChip.className = "current-user branch-chip";
      branchChip.id = "currentBranchChip";
      branchChip.title = "Aktiv filial";
      branchChip.textContent = "\u{1F3E2} " + DB.getCurrentBranchName();
      right.insertBefore(branchChip, right.firstChild);
    }
    var chip = document.createElement("span");
    chip.className = "current-user";
    chip.id = "currentUserChip";
    chip.title = "Daxil olmuş işçi";
    var name = user ? user.name : "Qonaq";
    chip.textContent = "\u{1F464} " + name + " · " + DB.roleLabel(role);
    right.insertBefore(chip, right.firstChild);
  }

  // İstifadəçilər bölməsi (yalnız Admin görür)
  function renderUsers() {
    var tbody = document.getElementById("usersTableBody");
    if (!tbody || !DB.getUsers) return;
    var users = DB.getUsers();
    tbody.innerHTML = users.map(function (u) {
      var status = u.active
        ? '<span class="inventory-positive">Aktiv</span>'
        : '<span class="inventory-low">Deaktiv</span>';
      return (
        "<tr>" +
        "<td><strong>" + esc(u.name) + "</strong></td>" +
        "<td>" + esc(u.username) + "</td>" +
        "<td>" + esc(DB.roleLabel(u.role)) + "</td>" +
        "<td>" + esc(DB.branchLabel ? DB.branchLabel(u.branch) : (u.branch || "-")) + "</td>" +
        "<td>" + esc(u.phone || "-") + "</td>" +
        "<td>" + status + "</td>" +
        '<td><div class="action-cell"><button class="action-btn delete" data-del-user="' + u.id + '">Sil</button></div></td>' +
        "</tr>"
      );
    }).join("");

    tbody.querySelectorAll("[data-del-user]").forEach(function (b) {
      b.addEventListener("click", function () {
        var id = b.getAttribute("data-del-user");
        var u = DB.getUser ? DB.getUser(id) : null;
        if (!confirm((u ? u.name : "İşçi") + " silinsin?")) return;
        var res = DB.deleteUser(id);
        if (!res || !res.ok) {
          alert(res && res.reason === "self" ? "Öz hesabınızı silə bilməzsiniz." : "Silinmədi.");
          return;
        }
        renderUsers();
      });
    });

    var grid = document.getElementById("usersSummaryGrid");
    if (grid) {
      var total = users.length;
      var active = users.filter(function (u) { return u.active; }).length;
      var admins = users.filter(function (u) { return u.role === "admin"; }).length;
      grid.innerHTML =
        '<div class="report-box"><h4>Ümumi işçi</h4><p>' + total + "</p></div>" +
        '<div class="report-box"><h4>Aktiv</h4><p>' + active + "</p></div>" +
        '<div class="report-box"><h4>Adminlər</h4><p>' + admins + "</p></div>";
    }
  }

  function esc(v) {
    return String(v == null ? "" : v)
      .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
  }

  // Yeni işçi əlavə et modalı (yalnız Admin)
  function showUserModal() {
    if (!DB.can("manageUsers")) return;
    if (document.getElementById("userModalOverlay")) return;
    var overlay = document.createElement("div");
    overlay.className = "modal-overlay";
    overlay.id = "userModalOverlay";
    overlay.innerHTML =
      '<div class="modal-card small-modal">' +
      '  <div class="modal-head"><h3>Yeni işçi</h3>' +
      '    <button type="button" class="secondary-btn" id="nuClose">Bağla</button></div>' +
      '  <div class="form-grid-2">' +
      '    <div class="form-group full"><label>Ad Soyad</label><input type="text" id="nuName" /></div>' +
      '    <div class="form-group"><label>İstifadəçi adı</label><input type="text" id="nuUsername" /></div>' +
      '    <div class="form-group"><label>Şifrə</label><input type="text" id="nuPassword" /></div>' +
      '    <div class="form-group"><label>Rol</label><select id="nuRole">' +
      '      <option value="admin">Admin</option><option value="manager">Menecer</option>' +
      '      <option value="user" selected>İşçi</option></select></div>' +
      '    <div class="form-group"><label>Filial</label><select id="nuBranch">' +
      (DB.getBranches ? DB.getBranches().map(function (b) { return '<option value="' + b.id + '">' + esc(b.name) + '</option>'; }).join("") : "") +
      '    </select></div>' +
      '    <div class="form-group"><label>Telefon</label><input type="text" id="nuPhone" /></div>' +
      '    <div class="form-group full"><label><input type="checkbox" id="nuActive" checked /> Aktiv</label></div>' +
      '  </div>' +
      '  <div class="modal-actions"><button type="button" class="primary-btn" id="nuSave">Yadda saxla</button></div>' +
      '</div>';
    document.body.appendChild(overlay);
    function close() { if (overlay.parentNode) overlay.parentNode.removeChild(overlay); }
    overlay.addEventListener("click", function (e) { if (e.target === overlay) close(); });
    document.getElementById("nuClose").onclick = close;
    document.getElementById("nuSave").onclick = function () {
      var branchEl = document.getElementById("nuBranch");
      var res = DB.addUser({
        name: document.getElementById("nuName").value,
        username: document.getElementById("nuUsername").value,
        password: document.getElementById("nuPassword").value,
        role: document.getElementById("nuRole").value,
        branch: branchEl ? branchEl.value : undefined,
        phone: document.getElementById("nuPhone").value,
        active: document.getElementById("nuActive").checked,
      });
      if (!res || !res.ok) {
        alert(res && res.reason === "duplicate"
          ? "Bu istifadəçi adı artıq mövcuddur."
          : "Ad, istifadəçi adı və şifrə mütləqdir.");
        return;
      }
      close();
      renderUsers();
    };
    var nameEl = document.getElementById("nuName");
    if (nameEl) nameEl.focus();
  }

  function init() {
    buildUserChip();
    renderUsers();
    var addUserBtn = document.getElementById("addUserBtn");
    if (addUserBtn) addUserBtn.addEventListener("click", showUserModal);
    applyPermissions();
    applyMenuVisibility();
    // Bölmə dəyişdikdə statik düymələr yenidən görünə bilər — yenidən tətbiq
    document.addEventListener("click", function (e) {
      if (e.target && e.target.closest && e.target.closest(".nav-link")) {
        setTimeout(applyPermissions, 0);
      }
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
