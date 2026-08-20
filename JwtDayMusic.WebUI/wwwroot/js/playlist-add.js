// Bir şarkıyı playlist'e ekleme akışı — ortada açılan modal üzerinden.
// 1) modalı aç, 2) kullanıcının playlist'lerini çekip listele, 3) seçilene ekle.

let pladdCurrentSongId = null;

// Şarkı kartlarındaki "Playlist'e ekle" butonları bunu çağırır.
function addToPlaylist(songId) {
    pladdCurrentSongId = songId;

    const overlay = document.getElementById('pladdOverlay');
    const body = document.getElementById('pladdBody');
    if (!overlay || !body) return; // modal bu sayfada yoksa sessizce çık

    overlay.classList.add('show');
    document.body.classList.add('pladd-open');
    body.innerHTML = '<p class="pladd-status">Playlistler yükleniyor...</p>';

    loadPlaylistOptions();
}

function closePlaylistModal() {
    const overlay = document.getElementById('pladdOverlay');
    if (!overlay) return;
    overlay.classList.remove('show');
    document.body.classList.remove('pladd-open');
    pladdCurrentSongId = null;
}

async function loadPlaylistOptions() {
    const body = document.getElementById('pladdBody');
    if (!body) return;

    try {
        const res = await fetch('/Playlist/MyPlaylistsJson');
        if (res.status === 401) {
            closePlaylistModal();
            notify('Playlist için giriş yapmalısınız.', 'info');
            window.location.href = '/Login/SignIn';
            return;
        }

        const playlists = await res.json();

        if (!playlists || playlists.length === 0) {
            body.innerHTML =
                '<div class="pladd-empty">' +
                '<span class="material-symbols-outlined">queue_music</span>' +
                '<p>Henüz playlistin yok.</p>' +
                '<a href="/Playlist/Index" class="pladd-empty-btn">' +
                '<span class="material-symbols-outlined">add</span>Playlist Oluştur</a>' +
                '</div>';
            return;
        }

        body.innerHTML = '<div class="pladd-list"></div>';
        const list = body.querySelector('.pladd-list');

        playlists.forEach(function (p) {
            const item = document.createElement('button');
            item.type = 'button';
            item.className = 'pladd-item';
            item.innerHTML =
                '<span class="pladd-item-icon material-symbols-outlined">queue_music</span>' +
                '<span class="pladd-item-info">' +
                '<span class="pladd-item-name"></span>' +
                '<span class="pladd-item-count">' + p.songCount + ' şarkı</span>' +
                '</span>' +
                '<span class="pladd-item-arrow material-symbols-outlined">chevron_right</span>';
            item.querySelector('.pladd-item-name').textContent = p.name;
            item.addEventListener('click', function () {
                selectPlaylist(p.playlistId, p.name, item);
            });
            list.appendChild(item);
        });
    } catch (e) {
        body.innerHTML = '<p class="pladd-status pladd-status--error">Playlistler yüklenemedi.</p>';
    }
}

async function selectPlaylist(playlistId, playlistName, itemEl) {
    if (!pladdCurrentSongId) return;

    const items = document.querySelectorAll('.pladd-item');
    items.forEach(function (el) { el.disabled = true; });
    if (itemEl) itemEl.classList.add('pladd-item--loading');

    try {
        const addRes = await fetch(
            '/Playlist/AddSong?playlistId=' + playlistId + '&songId=' + pladdCurrentSongId,
            { method: 'POST' });

        if (addRes.ok) {
            notify('Şarkı "' + playlistName + '" playlistine eklendi.', 'success');
            closePlaylistModal();
        } else {
            notify('Şarkı eklenemedi.', 'error');
            items.forEach(function (el) { el.disabled = false; });
            if (itemEl) itemEl.classList.remove('pladd-item--loading');
        }
    } catch (e) {
        notify('İstek gönderilemedi: ' + e, 'error');
        items.forEach(function (el) { el.disabled = false; });
        if (itemEl) itemEl.classList.remove('pladd-item--loading');
    }
}

// Modal kapatma etkileşimleri: kapat butonu, dış alana tıklama, ESC tuşu.
document.addEventListener('DOMContentLoaded', function () {
    const overlay = document.getElementById('pladdOverlay');
    if (!overlay) return;

    const closeBtn = document.getElementById('pladdClose');
    if (closeBtn) closeBtn.addEventListener('click', closePlaylistModal);

    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) closePlaylistModal();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && overlay.classList.contains('show')) closePlaylistModal();
    });
});
