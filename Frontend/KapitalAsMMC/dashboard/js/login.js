/* login.js — istifadəçi adı + şifrə ilə giriş (frontend mock yoxlama) */
(function () {
  "use strict";
  var form = document.getElementById("loginForm");
  var userEl = document.getElementById("username");
  var passEl = document.getElementById("password");
  var errEl = document.getElementById("loginError");

  function showError(msg) {
    errEl.textContent = msg;
    errEl.classList.remove("hidden");
  }
  function clearError() { errEl.classList.add("hidden"); }

  userEl.addEventListener("input", clearError);
  passEl.addEventListener("input", clearError);

  form.addEventListener("submit", function (e) {
    e.preventDefault();
    clearError();
    var u = userEl.value.trim();
    var p = passEl.value;
    if (!u || !p) { showError("İstifadəçi adı və şifrəni daxil edin."); return; }

    var res = window.DB.authenticate(u, p);
    if (res.ok) {
      window.location.href = "dashboard.html";
    } else if (res.reason === "inactive") {
      showError("Bu hesab deaktivdir. Admin ilə əlaqə saxlayın.");
    } else {
      showError("İstifadəçi adı və ya şifrə yanlışdır.");
    }
  });
})();
