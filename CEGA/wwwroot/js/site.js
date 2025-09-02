// Helpers y manejo de modales + POST dentro de modales (data-modal-post)
(() => {
    // Obtiene el modal global y su instancia de Bootstrap
    function getModal() {
        const el = document.getElementById('modalDynamic');
        if (!el) return null;
        const instance = bootstrap.Modal.getOrCreateInstance(el);
        return { el, instance };
    }

    // Abre contenido (parcial) en el modal global
    window.openModal = async function (url, title) {
        const modal = getModal();
        if (!modal) { window.location.href = url; return; } // fallback si no existe el modal

        const { el, instance } = modal;
        const header = el.querySelector('.modal-title');
        const body = el.querySelector('.modal-body');
        if (header) header.textContent = title || '';
        if (body) body.innerHTML = '<div class="p-3 text-center">Cargando…</div>';

        try {
            const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' }, credentials: 'same-origin' });
            if (res.redirected) {
                modal?.instance.hide();
                window.location.href = res.url;
                return;
            }
            const html = await res.text();
            if (body) body.innerHTML = html;
            instance.show();
        } catch (err) {
            if (body) body.innerHTML = '<div class="p-3 text-danger text-center">No se pudo cargar el contenido.</div>';
            console.error(err);
        }
    };

    // Intercepta formularios con data-modal-post para que NO naveguen y se re-rendericen en el modal
    document.addEventListener('submit', async (e) => {
        const form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (!form.matches('[data-modal-post]')) return;

        e.preventDefault();

        const modal = getModal();
        const body = modal?.el.querySelector('.modal-body');

        try {
            const res = await fetch(form.action, {
                method: form.method || 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                credentials: 'same-origin'
            });

            // Si el servidor redirige (registro exitoso), seguimos la redirección
            // justo tras obtener 'res'
            if (!res.ok) {
                const ct = res.headers.get('content-type') || '';
                if (ct.includes('text/html')) {
                    const html = await res.text();
                    if (body) body.innerHTML = html;
                    return;
                }
            }

            // Normalmente el servidor devuelve HTML de la vista con validaciones
            const ct = res.headers.get('content-type') || '';
            if (ct.includes('text/html')) {
                const html = await res.text();
                if (body) body.innerHTML = html;
            } else if (ct.includes('application/json')) {
                const data = await res.json();
                if (data.id && data.nombre) {
                    const sel = document.getElementById('PuestoId');
                    if (sel) { /* actualizar select como ya haces */ modal?.instance.hide(); return; }
                    openModal('/Account/Register?selectPuestoId=' + data.id, 'Crear cuenta'); // si estás dentro del modal
                    return;
                }

                if (data.error) {
                    const msg = document.querySelector('#puestoModalMsg') || document.querySelector('#puestoInlineMsg');
                    if (msg) { msg.className = 'small text-danger'; msg.textContent = data.error; }
                    return;
                }
                const url = data.redirectUrl || data.redirectTo;   // ← clave
                if (url) { modal?.instance.hide(); window.location.href = url; return; }
                if (data.html && body) body.innerHTML = data.html;
            } else {
                if (body) body.innerHTML = '<div class="p-3 text-danger text-center">Respuesta inesperada del servidor.</div>';
            }
        } catch (err) {
            console.error(err);
            if (body) body.innerHTML = '<div class="p-3 text-danger text-center">Ocurrió un error. Intenta de nuevo.</div>';
        }
    }, false);
})();
