/* =====================================================================
   seed-data.js — Dashboard SPA üçün başlanğıc (mock) data.
   YALNIZ boş bölmələri doldurur — mövcud data heç vaxt üzərinə yazılmır.
   dashboard.js və new-invoice.js localStorage-dan oxuduğu üçün bu skript
   onlardan ƏVVƏL yüklənməlidir.

   Açarlar dashboard.js / new-invoice.js ilə eynidir (lesa_*_v4 / _v1).
   Borc/depozit müştəri history-dən, ödəniş paymentHistory-dən,
   inventar icarəsi qaimələrdən hesablanır — rəqəmlər uyğunlaşdırılıb.
   ===================================================================== */
(function () {
  "use strict";

  var KEYS = {
    customers: "lesa_customers_v4",
    invoices: "lesa_invoices_v4",
    extra: "lesa_extra_categories_v4",
    service: "lesa_service_categories_v4",
    pole: "lesa_pole_categories_v1",
    inventory: "lesa_inventory_v1",
  };

  function read(k) {
    try { return JSON.parse(localStorage.getItem(k)); } catch (e) { return null; }
  }
  function isEmpty(v) {
    if (v == null) return true;
    if (Array.isArray(v)) return v.length === 0;
    if (typeof v === "object") return Object.keys(v).length === 0;
    return false;
  }
  function seed(k, val) {
    if (isEmpty(read(k))) {
      try { localStorage.setItem(k, JSON.stringify(val)); } catch (e) {}
    }
  }

  /* ----------------------------------------------------------- Müştərilər
     History BOŞ qalır: dashboard.js başlanğıcda qaimələrdən borc/depozit/ödəniş
     girişlərini özü qurur (syncInvoiceCustomerHistory). Manual giriş ikiqat
     say yaradardı. Nəticə ledger (qaimələrdən):
       c1 borc 1000 / depozit 300 · c2 borc 800 · c3 borc 0 / depozit 200
       c4 borc 1300 / depozit 500 · c5 borc 400 */
  var customers = [
    { id: "c1", name: "Bəyaz Tikinti MMC", phone: "+994 50 555 11 22", extraPhone: "", address: "Bakı, Nizami r., Atatürk pr. 12", history: [] },
    { id: "c2", name: "Akkord İnşaat", phone: "+994 55 666 22 33", extraPhone: "", address: "Bakı, Yasamal r., Şərifzadə 45", history: [] },
    { id: "c3", name: "Murad Səfərov", phone: "+994 70 777 33 44", extraPhone: "", address: "Sumqayıt, 18-ci m/r", history: [] },
    { id: "c4", name: "Günay Construction", phone: "+994 51 888 44 55", extraPhone: "+994 12 488 00 11", address: "Bakı, Xətai r., Babək pr. 88", history: [] },
    { id: "c5", name: "Elvin Tağıyev", phone: "+994 77 999 55 66", extraPhone: "", address: "Gəncə, Kəpəz r.", history: [] }
  ];

  /* ----------------------------------------------------------- Qaimələr
     Status: returnDate < bu gün → Gecikir; isClosed → Bağlanıb; əks halda Aktiv.
     paidAmount paymentHistory-dən; remainingDebt = total - paid. */
  function pay(id, amount, date) {
    return [{ id: id, date: date, amount: amount, note: "İlkin ödəniş", direction: "in" }];
  }
  var invoices = [
    {
      id: "inv-101", invoiceDate: "2026-05-12", invoiceNo: "QA-101",
      customerId: "c1", customer: "Bəyaz Tikinti MMC", phone: "+994 50 555 11 22",
      address: "Bakı, Nizami r., Atatürk pr. 12", note: "Daimi müştəri",
      returnDate: "2026-07-12",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 200, unit: "ədəd", customPrice: 6, subtotal: 1200 },
        { category: "Taxta", size: "5/15", note: "", quantity: 500, unit: "m", customPrice: 0.6, subtotal: 300 }
      ],
      totalAmount: 1500, paidAmount: 500, paymentHistory: pay("pay-101", 500, "2026-05-20T10:00:00.000Z"),
      depositAmount: 300, remainingDebt: 1000, isClosed: false,
      createdAt: "2026-05-12T09:00:00.000Z", updatedAt: "2026-05-20T10:00:00.000Z",
      extensionHistory: [], returnHistory: []
    },
    {
      id: "inv-102", invoiceDate: "2026-04-28", invoiceNo: "QA-102",
      customerId: "c2", customer: "Akkord İnşaat", phone: "+994 55 666 22 33",
      address: "Bakı, Yasamal r., Şərifzadə 45", note: "",
      returnDate: "2026-05-28",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 100, unit: "ədəd", customPrice: 6, subtotal: 600 },
        { category: "Ksok dikt", size: "", note: "", quantity: 200, unit: "m²", customPrice: 1, subtotal: 200 }
      ],
      totalAmount: 800, paidAmount: 0, paymentHistory: [],
      depositAmount: 0, remainingDebt: 800, isClosed: false,
      createdAt: "2026-04-28T11:00:00.000Z", updatedAt: "2026-04-28T11:00:00.000Z",
      extensionHistory: [], returnHistory: []
    },
    {
      id: "inv-103", invoiceDate: "2026-06-01", invoiceNo: "QA-103",
      customerId: "c3", customer: "Murad Səfərov", phone: "+994 70 777 33 44",
      address: "Sumqayıt, 18-ci m/r", note: "Fərdi sifariş",
      returnDate: "2026-07-01",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 100, unit: "ədəd", customPrice: 6, subtotal: 600 }
      ],
      totalAmount: 600, paidAmount: 600, paymentHistory: pay("pay-103", 600, "2026-06-03T14:00:00.000Z"),
      depositAmount: 200, remainingDebt: 0, isClosed: false,
      createdAt: "2026-06-01T09:00:00.000Z", updatedAt: "2026-06-03T14:00:00.000Z",
      extensionHistory: [], returnHistory: []
    },
    {
      id: "inv-104", invoiceDate: "2026-03-15", invoiceNo: "QA-104",
      customerId: "c4", customer: "Günay Construction", phone: "+994 51 888 44 55",
      address: "Bakı, Xətai r., Babək pr. 88", note: "Böyük layihə",
      returnDate: "2026-04-15",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 300, unit: "ədəd", customPrice: 6, subtotal: 1800 },
        { category: "Bir tərəfi boy dikt", size: "1.52", note: "", quantity: 200, unit: "m²", customPrice: 1, subtotal: 200 }
      ],
      totalAmount: 2000, paidAmount: 700, paymentHistory: pay("pay-104", 700, "2026-04-02T13:00:00.000Z"),
      depositAmount: 500, remainingDebt: 1300, isClosed: false,
      createdAt: "2026-03-15T08:30:00.000Z", updatedAt: "2026-04-02T13:00:00.000Z",
      extensionHistory: [], returnHistory: []
    },
    {
      id: "inv-105", invoiceDate: "2026-06-05", invoiceNo: "QA-105",
      customerId: "c5", customer: "Elvin Tağıyev", phone: "+994 77 999 55 66",
      address: "Gəncə, Kəpəz r.", note: "",
      returnDate: "2026-07-05",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 50, unit: "ədəd", customPrice: 6, subtotal: 300 },
        { category: "Ksok dikt", size: "", note: "", quantity: 100, unit: "m²", customPrice: 1, subtotal: 100 }
      ],
      totalAmount: 400, paidAmount: 0, paymentHistory: [],
      depositAmount: 0, remainingDebt: 400, isClosed: false,
      createdAt: "2026-06-05T10:00:00.000Z", updatedAt: "2026-06-05T10:00:00.000Z",
      extensionHistory: [], returnHistory: []
    },
    {
      id: "inv-106", invoiceDate: "2026-02-10", invoiceNo: "QA-106",
      customerId: "c1", customer: "Bəyaz Tikinti MMC", phone: "+994 50 555 11 22",
      address: "Bakı, Nizami r., Atatürk pr. 12", note: "Tamamlanıb",
      returnDate: "2026-03-10",
      items: [
        { category: "Boy dikt", size: "", note: "", quantity: 100, unit: "ədəd", customPrice: 6, subtotal: 600 }
      ],
      totalAmount: 600, paidAmount: 600, paymentHistory: pay("pay-106", 600, "2026-03-08T12:00:00.000Z"),
      depositAmount: 0, remainingDebt: 0, isClosed: true, closedAt: "2026-03-10T16:00:00.000Z",
      createdAt: "2026-02-10T09:00:00.000Z", updatedAt: "2026-03-10T16:00:00.000Z",
      extensionHistory: [], returnHistory: []
    }
  ];

  /* ----------------------------------------------------------- Kateqoriyalar */
  var extraCategories = [
    { id: "ex1", name: "Konuslu birləşdirici", price: 2, unit: "ədəd", note: "Əlavə hissə" },
    { id: "ex2", name: "Vint-qayka dəsti", price: 0.5, unit: "ədəd", note: "" }
  ];
  var serviceCategories = [
    { id: "sv1", name: "Quraşdırma", price: 50, unit: "xidmət", note: "Ustalar tərəfindən" },
    { id: "sv2", name: "Sökülmə", price: 40, unit: "xidmət", note: "" }
  ];
  var poleCategories = [
    { id: "pl1", name: "3.85", price: 8, unit: "ədəd", note: "Standart ölçü" },
    { id: "pl2", name: "1.70", price: 5, unit: "ədəd", note: "" },
    { id: "pl3", name: "5.50", price: 12, unit: "ədəd", note: "Uzun" }
  ];

  /* ----------------------------------------------------------- Anbar (inventar)
     { ad: ümumi say }. İcarədə olan say qaimələrdən hesablanır;
     available = total - rented. Bəzi mallar qəsdən az qalıqlı. */
  var inventory = {
    "Boy dikt": 1000,
    "Taxta": 800,
    "Ksok dikt": 300,
    "Bir tərəfi boy dikt": 500,
    "Pales": 400,
    "Təkərli lesa başlıq": 50
  };

  /* ----------------------------------------------------------- Yaz (yalnız boşları) */
  seed(KEYS.customers, customers);
  seed(KEYS.invoices, invoices);
  seed(KEYS.extra, extraCategories);
  seed(KEYS.service, serviceCategories);
  seed(KEYS.pole, poleCategories);
  seed(KEYS.inventory, inventory);
})();
