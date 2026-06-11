/* =====================================================================
   data.js — Giriş (login) + rol/icazə sistemi.
   Kanonik tətbiq Dashboard SPA-dır; biznes datası localStorage-dadır
   (seed-data.js + dashboard.js). Bu fayl yalnız işçi hesabları,
   rol etiketləri və icazələri saxlayır.
   Backend gələndə authenticate() və users serverə bağlanacaq.
   ===================================================================== */
(function (global) {
  "use strict";

  /* ----------------------------------------------------------------- Rol/icazə */
  const ROLE_KEY = "kapital_role";
  const USER_KEY = "kapital_user";
  const ROLE_LABELS = { admin: "Admin", manager: "Menecer", user: "İşçi" };

  // İcazə cədvəli (yalnız UX üçün — düymə/menyu gizlətmək)
  const PERMISSIONS = {
    admin:   ["view", "create", "edit", "delete", "addPayment", "print", "manageUsers"],
    manager: ["view", "create", "edit", "delete", "addPayment", "print"],
    user:    ["view", "print", "addPayment"],
  };

  function getCurrentRole() {
    let r = null;
    try { r = global.localStorage.getItem(ROLE_KEY); } catch (e) { r = null; }
    if (!r || !PERMISSIONS[r]) r = "admin";
    return r;
  }
  function setRole(role) {
    if (!PERMISSIONS[role]) return;
    try { global.localStorage.setItem(ROLE_KEY, role); } catch (e) {}
  }
  function roleLabel(role) { return ROLE_LABELS[role] || role; }
  function can(action) { return PERMISSIONS[getCurrentRole()].indexOf(action) !== -1; }

  /* ----------------------------------------------------------------- İşçi hesabları */
  const users = [
    { id: 1, name: "Elmar Əliyev",   username: "elmar",   password: "admin123",   role: "admin",   phone: "+994 50 111 22 33", active: true,  created: "2025-01-10" },
    { id: 2, name: "Rəşad Quliyev",  username: "rashad",  password: "manager123", role: "manager", phone: "+994 55 222 33 44", active: true,  created: "2025-02-04" },
    { id: 3, name: "Nigar Hüseynova",username: "nigar",   password: "user123",    role: "user",    phone: "+994 70 333 44 55", active: true,  created: "2025-03-18" },
    { id: 4, name: "Tural Məmmədov", username: "tural",   password: "user123",    role: "user",    phone: "+994 51 444 55 66", active: false, created: "2025-04-22" },
  ];

  /* ----------------------------------------------------------------- Giriş / sessiya */
  function setCurrentUser(id) {
    try { global.localStorage.setItem(USER_KEY, String(id)); } catch (e) {}
  }
  function getCurrentUser() {
    let id = null;
    try { id = global.localStorage.getItem(USER_KEY); } catch (e) { id = null; }
    if (!id) return null;
    return users.find(function (u) { return u.id === +id; }) || null;
  }
  function logout() {
    try { global.localStorage.removeItem(USER_KEY); } catch (e) {}
  }
  // İşçi adı + şifrə yoxlanışı (mock). Uğurda rol və sessiya təyin olunur.
  function authenticate(username, password) {
    const uname = (username || "").trim().toLowerCase();
    const user = users.find(function (u) { return u.username.toLowerCase() === uname; });
    if (!user || user.password !== password) return { ok: false, reason: "invalid" };
    if (!user.active) return { ok: false, reason: "inactive" };
    setRole(user.role);
    setCurrentUser(user.id);
    return { ok: true, user: user };
  }
  function getUsers() { return users.slice(); }

  /* ----------------------------------------------------------------- İxrac */
  global.DB = {
    // rol/icazə
    getCurrentRole: getCurrentRole, setRole: setRole, can: can,
    roleLabel: roleLabel, ROLE_LABELS: ROLE_LABELS,
    // giriş/sessiya
    authenticate: authenticate, setCurrentUser: setCurrentUser,
    getCurrentUser: getCurrentUser, logout: logout,
    // işçilər
    users: users, getUsers: getUsers,
  };
})(window);
