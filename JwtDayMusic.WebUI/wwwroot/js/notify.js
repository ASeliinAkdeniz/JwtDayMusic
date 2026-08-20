// Site genelinde kullanılan tema uyumlu bildirim (toast) sistemi.
// Eskiden player.js / like.js / playlist-add.js gibi dosyalarda tarayıcının
// çirkin ve tema dışı alert() kutuları kullanılıyordu; artık hepsi bu
// notify() fonksiyonunu çağırıyor.
//
// Kullanım: notify('Mesaj', 'success' | 'error' | 'info' | 'warning')
(function () {
    const ICONS = { success: '✅', error: '⛔', info: 'ℹ️', warning: '⚠️' };

    function ensureStack() {
        let stack = document.getElementById('swToastStack');
        if (!stack) {
            stack = document.createElement('div');
            stack.id = 'swToastStack';
            stack.className = 'sw-toast-stack';
            document.body.appendChild(stack);
        }
        return stack;
    }

    window.notify = function (message, type, timeout) {
        type = type || 'info';
        timeout = timeout || 4000;

        const stack = ensureStack();

        const toast = document.createElement('div');
        toast.className = 'sw-toast sw-toast-' + type;

        const icon = document.createElement('span');
        icon.className = 'sw-toast-icon';
        icon.textContent = ICONS[type] || ICONS.info;

        const msg = document.createElement('span');
        msg.className = 'sw-toast-msg';
        msg.textContent = message;

        toast.appendChild(icon);
        toast.appendChild(msg);
        stack.appendChild(toast);

        requestAnimationFrame(() => toast.classList.add('show'));

        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 250);
        }, timeout);
    };

    // Global oynatıcı ekrandaysa toast'ları onun üstüne taşı ki üst üste binmesin.
    window.addEventListener('DOMContentLoaded', () => {
        const gp = document.getElementById('globalPlayer');
        if (gp) {
            document.documentElement.style.setProperty('--sw-player-offset', gp.offsetHeight + 'px');
        }
    });
})();
