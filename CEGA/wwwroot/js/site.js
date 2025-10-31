// Helpers y manejo de modales + POST dentro de modales (data-modal-post)
(() => {
    function getModal() {
        const el = document.getElementById('modalDynamic');
        if (!el) return null;
        const instance = bootstrap.Modal.getOrCreateInstance(el);
        return { el, instance };
    }

    // Re-parse unobtrusive validation en el bloque que acabamos de inyectar
    function reparseValidation(body) {
        if (window.jQuery && window.$ && $.validator && $.validator.unobtrusive) {
            const $body = $(body);
            $body.find('form').removeData('validator').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse($body);
        }
    }

    window.openModal = async function (url, title) {
        const modal = getModal();
        if (!modal) { window.location.href = url; return; }

        const { el, instance } = modal;
        const header = el.querySelector('.modal-title');
        const body = el.querySelector('.modal-body');
        if (header) header.textContent = title || '';
        if (body) body.innerHTML = '<div class="p-3 text-center">Cargando…</div>';

        try {
            const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' }, credentials: 'same-origin' });
            if (res.redirected) { instance.hide(); window.location.href = res.url; return; }
            const html = await res.text();
            if (body) {
                body.innerHTML = html;
                reparseValidation(body); // 👈 IMPORTANTE
            }
            instance.show();
        } catch (err) {
            if (body) body.innerHTML = '<div class="p-3 text-danger text-center">No se pudo cargar el contenido.</div>';
            console.error(err);
        }
    };

    document.addEventListener('submit', async (e) => {
        const form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (!form.matches('[data-modal-post]')) return;

        e.preventDefault();

        const modal = getModal();
        const body = modal?.el.querySelector('.modal-body');
        const fd = new FormData(form); // incluye __RequestVerificationToken

        try {
            const res = await fetch(form.action, {
                method: form.method || 'POST',
                body: fd,
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                credentials: 'same-origin'
            });

            // Seguir redirección si la hay
            if (res.redirected) { modal?.instance.hide(); window.location.href = res.url; return; }

            const ct = (res.headers.get('content-type') || '').toLowerCase();

            if (ct.includes('application/json')) {
                const data = await res.json();

                // ✅ Manejo de éxito genérico del forgot
                if (data.ok) {
                    if (body) {
                        body.innerHTML = `
              <div class="alert alert-success m-0" role="alert">
                ${data.message || 'Operación completada correctamente.'}
              </div>`;
                    }
                    // opcional: cerrar modal después de 2s
                    setTimeout(() => modal?.instance.hide(), 1800);
                    return;
                }

                // Otros casos JSON (errores, selects, etc.)
                if (data.error && body) {
                    body.innerHTML = `<div class="alert alert-danger">${data.error}</div>`;
                    return;
                }
                const url = data.redirectUrl || data.redirectTo;
                if (url) { modal?.instance.hide(); window.location.href = url; return; }

                // Si vino HTML embebido en JSON
                if (data.html && body) {
                    body.innerHTML = data.html;
                    reparseValidation(body); // 👈
                    return;
                }

                // Fallback
                if (body) body.innerHTML = '<div class="p-3 text-danger text-center">Respuesta JSON no reconocida.</div>';
                return;
            }

            // HTML (validaciones del servidor o partial actualizado)
            if (ct.includes('text/html')) {
                const html = await res.text();
                if (body) {
                    body.innerHTML = html;
                    reparseValidation(body); // 👈
                }
                return;
            }

            if (body) body.innerHTML = '<div class="p-3 text-danger text-center">Respuesta inesperada del servidor.</div>';
        } catch (err) {
            console.error(err);
            if (body) body.innerHTML = '<div class="p-3 text-danger text-center">Ocurrió un error. Intenta de nuevo.</div>';
        }
    }, false);
})();
