/* =====================================================================
   Kapital A.S. MMC — Paylaşılan mock data + rol/icazə sistemi
   Bütün yeni səhifələr bu faylı yükləyir. localStorage YOXDUR — mock data.
   Real yoxlama backend-də olacaq; buradakı icazələr yalnız UX üçündür.
   ===================================================================== */
(function (global) {
  "use strict";

  /* ----------------------------------------------------------------- Rol/icazə */
  const ROLE_KEY = "kapital_role";
  const ROLE_LABELS = { admin: "Admin", manager: "Menecer", user: "İstifadəçi" };

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

  /* ----------------------------------------------------------------- İstifadəçilər */
  const users = [
    { id: 1, name: "Elmar Əliyev",   username: "elmar",   role: "admin",   phone: "+994 50 111 22 33", active: true,  created: "2025-01-10" },
    { id: 2, name: "Rəşad Quliyev",  username: "rashad",  role: "manager", phone: "+994 55 222 33 44", active: true,  created: "2025-02-04" },
    { id: 3, name: "Nigar Hüseynova",username: "nigar",   role: "user",    phone: "+994 70 333 44 55", active: true,  created: "2025-03-18" },
    { id: 4, name: "Tural Məmmədov", username: "tural",   role: "user",    phone: "+994 51 444 55 66", active: false, created: "2025-04-22" },
  ];

  /* ----------------------------------------------------------------- Kateqoriyalar
     type: "monthly" (aylıq icarə) | "daily" (günlük icarə)
     parent: alt-kateqoriya üçün ana kateqoriya id-si (məs. Dəmir Dirək) */
  const categories = [
    { id: 1, name: "Lesa — Adi",        type: "monthly", parentId: null, note: "Standart lesa, aylıq icarə" },
    { id: 2, name: "Lesa — 60-lıq",     type: "monthly", parentId: null, note: "60-lıq lesa, aylıq icarə" },
    { id: 3, name: "Lesa — Təkərli",    type: "daily",   parentId: null, note: "Təkərli lesa — GÜNLÜK icarə, vaxtı bitəndə bildiriş" },
    { id: 4, name: "Dəmir Dirək",       type: "monthly", parentId: null, note: "Ana kateqoriya" },
    { id: 5, name: "Dəmir Dirək 2 m",   type: "monthly", parentId: 4,    note: "2 metrlik dirək" },
    { id: 6, name: "Dəmir Dirək 3 m",   type: "monthly", parentId: 4,    note: "3 metrlik dirək" },
    { id: 7, name: "Dəmir Dirək 4 m",   type: "monthly", parentId: 4,    note: "4 metrlik dirək" },
    { id: 8, name: "Qəlib",             type: "monthly", parentId: null, note: "Beton qəlibi" },
    { id: 9, name: "Domkrat",           type: "daily",   parentId: null, note: "Günlük icarə" },
  ];

  /* ----------------------------------------------------------------- Məhsullar (anbar)
     stock: anbarda qalan; rented: icarədə olan; price: vahid aylıq/günlük qiymət */
  const products = [
    { id: 1, name: "Adi Lesa 2x1",      categoryId: 1, unit: "ədəd", stock: 120, rented: 80,  price: 6,  min: 20 },
    { id: 2, name: "Adi Lesa çarpaz",   categoryId: 1, unit: "ədəd", stock: 0,   rented: 60,  price: 4,  min: 15 },
    { id: 3, name: "60-lıq Lesa paneli",categoryId: 2, unit: "ədəd", stock: 45,  rented: 30,  price: 9,  min: 10 },
    { id: 4, name: "Təkərli Lesa qüllə", categoryId: 3, unit: "ədəd", stock: 8,  rented: 4,   price: 25, min: 3 },
    { id: 5, name: "Dəmir Dirək 2 m",   categoryId: 5, unit: "ədəd", stock: 200, rented: 140, price: 3,  min: 40 },
    { id: 6, name: "Dəmir Dirək 3 m",   categoryId: 6, unit: "ədəd", stock: 5,   rented: 95,  price: 4,  min: 30 },
    { id: 7, name: "Dəmir Dirək 4 m",   categoryId: 7, unit: "ədəd", stock: 60,  rented: 40,  price: 5,  min: 25 },
    { id: 8, name: "Beton Qəlibi 1m²",  categoryId: 8, unit: "ədəd", stock: 35,  rented: 20,  price: 12, min: 8 },
    { id: 9, name: "Hidravlik Domkrat", categoryId: 9, unit: "ədəd", stock: 0,   rented: 6,   price: 15, min: 2 },
  ];

  /* ----------------------------------------------------------------- Müştərilər */
  const customers = [
    { id: 1, name: "Bəyaz Tikinti MMC",  phone: "+994 50 555 11 22", address: "Bakı, Nizami r., Atatürk pr. 12", note: "Daimi müştəri" },
    { id: 2, name: "Akkord İnşaat",      phone: "+994 55 666 22 33", address: "Bakı, Yasamal r., Şərifzadə 45",   note: "" },
    { id: 3, name: "Murad Səfərov",      phone: "+994 70 777 33 44", address: "Sumqayıt, 18-ci m/r",              note: "Fərdi" },
    { id: 4, name: "Günay Construction", phone: "+994 51 888 44 55", address: "Bakı, Xətai r., Babək pr. 88",     note: "Böyük layihə" },
    { id: 5, name: "Elvin Tağıyev",      phone: "+994 77 999 55 66", address: "Gəncə, Kəpəz r.",                  note: "" },
    { id: 6, name: "Standart Build",     phone: "+994 50 121 66 77", address: "Bakı, Binəqədi r.",                note: "" },
  ];

  /* ----------------------------------------------------------------- Qaimələr
     number: ƏL ilə yazılır (manual); id: avtomatik.
     status: "Aktiv" | "Gecikir" | "Bağlanıb"
     total: ümumi icarə məbləği; paid: ödənilən; deposit: depozit;
     debt = total - paid - deposit (mənfi olmaz, hesablanır)
     transport/extraService: vaxtı artırılanda YENİDƏN hesablanmır */
  const invoices = [
    { id: 1001, number: "QA-101", customerId: 1, date: "2026-05-12", dueDate: "2026-06-12", status: "Aktiv",    transport: 40, extraService: 20, monthlyTotal: 760, paid: 400, deposit: 200 },
    { id: 1002, number: "QA-102", customerId: 2, date: "2026-04-28", dueDate: "2026-05-28", status: "Gecikir",  transport: 60, extraService: 0,  monthlyTotal: 540, paid: 100, deposit: 0 },
    { id: 1003, number: "QA-103", customerId: 3, date: "2026-05-20", dueDate: "2026-06-20", status: "Aktiv",    transport: 0,  extraService: 30, monthlyTotal: 300, paid: 300, deposit: 0 },
    { id: 1004, number: "QA-125", customerId: 4, date: "2026-03-15", dueDate: "2026-04-15", status: "Gecikir",  transport: 80, extraService: 50, monthlyTotal: 1320, paid: 500, deposit: 300 },
    { id: 1005, number: "QA-126", customerId: 1, date: "2026-06-01", dueDate: "2026-07-01", status: "Aktiv",    transport: 35, extraService: 0,  monthlyTotal: 420, paid: 420, deposit: 0 },
    { id: 1006, number: "QA-130", customerId: 5, date: "2026-05-30", dueDate: "2026-06-30", status: "Aktiv",    transport: 25, extraService: 15, monthlyTotal: 250, paid: 0, deposit: 100 },
    { id: 1007, number: "QA-140", customerId: 6, date: "2026-02-10", dueDate: "2026-03-10", status: "Bağlanıb", transport: 50, extraService: 0,  monthlyTotal: 600, paid: 600, deposit: 0 },
    { id: 1008, number: "QA-141", customerId: 4, date: "2026-05-05", dueDate: "2026-06-05", status: "Gecikir",  transport: 0,  extraService: 0,  monthlyTotal: 480, paid: 80, deposit: 0 },
  ];

  /* ----------------------------------------------------------------- Qaimə malları */
  const invoiceItems = [
    { id: 1, invoiceId: 1001, productId: 1, qty: 40, price: 6,  note: "" },
    { id: 2, invoiceId: 1001, productId: 5, qty: 60, price: 3,  note: "2 metrlik" },
    { id: 3, invoiceId: 1002, productId: 3, qty: 30, price: 9,  note: "" },
    { id: 4, invoiceId: 1002, productId: 6, qty: 20, price: 4,  note: "" },
    { id: 5, invoiceId: 1003, productId: 4, qty: 4,  price: 25, note: "Təkərli — günlük" },
    { id: 6, invoiceId: 1004, productId: 1, qty: 40, price: 6,  note: "" },
    { id: 7, invoiceId: 1004, productId: 6, qty: 50, price: 4,  note: "" },
    { id: 8, invoiceId: 1004, productId: 8, qty: 15, price: 12, note: "Qəlib" },
    { id: 9, invoiceId: 1005, productId: 7, qty: 40, price: 5,  note: "" },
    { id: 10, invoiceId: 1006, productId: 5, qty: 50, price: 3,  note: "" },
    { id: 11, invoiceId: 1007, productId: 3, qty: 30, price: 9,  note: "" },
    { id: 12, invoiceId: 1008, productId: 1, qty: 40, price: 6,  note: "" },
    { id: 13, invoiceId: 1008, productId: 5, qty: 80, price: 3,  note: "" },
  ];

  /* ----------------------------------------------------------------- Ödənişlər */
  const payments = [
    { id: 1, invoiceId: 1001, amount: 200, date: "2026-05-12", note: "İlkin ödəniş" },
    { id: 2, invoiceId: 1001, amount: 200, date: "2026-05-25", note: "" },
    { id: 3, invoiceId: 1002, amount: 100, date: "2026-04-28", note: "" },
    { id: 4, invoiceId: 1003, amount: 300, date: "2026-05-20", note: "Tam ödəniş" },
    { id: 5, invoiceId: 1004, amount: 500, date: "2026-03-15", note: "" },
    { id: 6, invoiceId: 1005, amount: 420, date: "2026-06-01", note: "Tam ödəniş" },
    { id: 7, invoiceId: 1007, amount: 600, date: "2026-02-10", note: "Bağlandı" },
    { id: 8, invoiceId: 1008, amount: 80,  date: "2026-05-05", note: "" },
  ];

  /* ----------------------------------------------------------------- Depozitlər */
  const deposits = [
    { id: 1, customerId: 1, invoiceId: 1001, amount: 200, date: "2026-05-12", status: "Saxlanılır" },
    { id: 2, customerId: 4, invoiceId: 1004, amount: 300, date: "2026-03-15", status: "Saxlanılır" },
    { id: 3, customerId: 5, invoiceId: 1006, amount: 100, date: "2026-05-30", status: "Saxlanılır" },
  ];

  /* ----------------------------------------------------------------- Hesablanan dəyərlər */
  // Qaimənin ümumi məbləği = aylıq icarə + nəqliyyat + əlavə xidmət
  function invoiceTotal(inv) {
    return (inv.monthlyTotal || 0) + (inv.transport || 0) + (inv.extraService || 0);
  }
  // Borc = total - paid - deposit (0-dan aşağı düşmür)
  function invoiceDebt(inv) {
    const d = invoiceTotal(inv) - (inv.paid || 0) - (inv.deposit || 0);
    return d > 0 ? d : 0;
  }

  /* ----------------------------------------------------------------- Getterlər */
  function getUsers() { return users.slice(); }
  function getCustomers() { return customers.slice(); }
  function getCategories() { return categories.slice(); }
  function getProducts() { return products.slice(); }
  function getInvoices() { return invoices.slice(); }
  function getPayments() { return payments.slice(); }
  function getDeposits() { return deposits.slice(); }

  function getCustomer(id) { return customers.find(function (c) { return c.id === +id; }) || null; }
  function getProduct(id) { return products.find(function (p) { return p.id === +id; }) || null; }
  function getCategory(id) { return categories.find(function (c) { return c.id === +id; }) || null; }
  function getInvoice(id) { return invoices.find(function (i) { return i.id === +id; }) || null; }

  function getInvoiceItems(invoiceId) {
    return invoiceItems.filter(function (it) { return it.invoiceId === +invoiceId; });
  }
  function getInvoicePayments(invoiceId) {
    return payments.filter(function (p) { return p.invoiceId === +invoiceId; });
  }
  function getCustomerInvoices(customerId) {
    return invoices.filter(function (i) { return i.customerId === +customerId; });
  }

  // Müştərinin ümumi borcu (bütün qaimələr üzrə)
  function getCustomerDebt(customerId) {
    return getCustomerInvoices(customerId).reduce(function (s, inv) { return s + invoiceDebt(inv); }, 0);
  }
  // Müştərinin ümumi depoziti
  function getCustomerDeposit(customerId) {
    return deposits
      .filter(function (d) { return d.customerId === +customerId && d.status === "Saxlanılır"; })
      .reduce(function (s, d) { return s + d.amount; }, 0);
  }

  // Yalnız borcu >0 olan müştərilər
  function getDebtors() {
    return customers
      .map(function (c) { return { customer: c, debt: getCustomerDebt(c.id) }; })
      .filter(function (x) { return x.debt > 0; })
      .sort(function (a, b) { return b.debt - a.debt; });
  }
  // Yalnız depoziti >0 olanlar
  function getActiveDeposits() {
    return deposits.filter(function (d) { return d.amount > 0 && d.status === "Saxlanılır"; });
  }

  // № ilə axtarış: "125" → "QA-125" tapır (endsWith + dəqiq uyğunluq)
  function searchInvoices(term) {
    term = (term || "").trim().toLowerCase();
    if (!term) return getInvoices();
    return invoices.filter(function (inv) {
      const num = (inv.number || "").toLowerCase();
      const cust = (getCustomer(inv.customerId) || {}).name || "";
      const phone = (getCustomer(inv.customerId) || {}).phone || "";
      return (
        num === term ||
        num.endsWith(term) ||
        num.indexOf(term) !== -1 ||
        cust.toLowerCase().indexOf(term) !== -1 ||
        phone.toLowerCase().indexOf(term) !== -1
      );
    });
  }

  function money(n) {
    n = Number(n) || 0;
    return n.toLocaleString("az-AZ") + " ₼";
  }

  /* ----------------------------------------------------------------- İxrac */
  global.DB = {
    // rol/icazə
    getCurrentRole: getCurrentRole, setRole: setRole, can: can, roleLabel: roleLabel,
    ROLE_LABELS: ROLE_LABELS,
    // xam massivlər
    users: users, customers: customers, categories: categories, products: products,
    invoices: invoices, invoiceItems: invoiceItems, payments: payments, deposits: deposits,
    // getterlər
    getUsers: getUsers, getCustomers: getCustomers, getCategories: getCategories,
    getProducts: getProducts, getInvoices: getInvoices, getPayments: getPayments,
    getDeposits: getDeposits, getCustomer: getCustomer, getProduct: getProduct,
    getCategory: getCategory, getInvoice: getInvoice, getInvoiceItems: getInvoiceItems,
    getInvoicePayments: getInvoicePayments, getCustomerInvoices: getCustomerInvoices,
    getCustomerDebt: getCustomerDebt, getCustomerDeposit: getCustomerDeposit,
    getDebtors: getDebtors, getActiveDeposits: getActiveDeposits,
    searchInvoices: searchInvoices,
    // hesablama
    invoiceTotal: invoiceTotal, invoiceDebt: invoiceDebt, money: money,
  };
})(window);
