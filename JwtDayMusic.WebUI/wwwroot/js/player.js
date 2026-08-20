const player = {
    queue: [],       // [{id, title, artist}]
    index: -1,
    shuffle: false,
    repeat: false,
    audio: null,
    _attempts: 0,

    el(id) { return document.getElementById(id); },

    init() {
        this.audio = this.el('globalAudio');
        if (!this.audio) return;
        this.audio.addEventListener('ended', () => this.handleEnded());
        this.audio.addEventListener('play', () => this.syncPlayIcon());
        this.audio.addEventListener('pause', () => this.syncPlayIcon());
    },

    // Sayfadaki tüm .queue-item öğelerinden sıralı kuyruk kur.
    buildQueueFromPage() {
        return Array.from(document.querySelectorAll('.queue-item')).map(el => ({
            id: parseInt(el.getAttribute('data-song-id'), 10),
            title: el.getAttribute('data-song-title') || 'Şarkı',
            artist: el.getAttribute('data-song-artist') || ''
        }));
    },

    // Bir şarkıya basınca: sayfanın listesini kuyruk yap, o şarkıdan başla.
    playFrom(songId) {
        const q = this.buildQueueFromPage();
        const i = q.findIndex(s => s.id === songId);
        if (i === -1) {
            this.queue = [{ id: songId, title: 'Şarkı', artist: '' }];
            this.index = 0;
        } else {
            this.queue = q;
            this.index = i;
        }
        this._attempts = 0;
        this.playCurrent(false);
    },

    async playCurrent(auto) {
        if (this.index < 0 || this.index >= this.queue.length) return;
        const song = this.queue[this.index];
        try {
            const res = await fetch('/Song/Play/' + song.id);
            if (res.status === 200) {
                const data = await res.json();
                this._attempts = 0;
                this.audio.src = data.audioUrl;
                this.el('gp-title').innerText = data.title;
                this.el('gp-artist').innerText = song.artist || '';
                this.el('globalPlayer').style.display = 'block';
                this.audio.play();
                return;
            }
            if (res.status === 401) {
                notify('Oynatmak için giriş yapmalısınız.', 'info');
                window.location.href = '/Login/SignIn';
                return;
            }
            if (res.status === 403) {
                const data = await res.json();
                if (auto) {
                    this.skipForward();        // otomatik ilerlerken erişilemeyeni atla
                } else {
                    notify(data.message, 'error');
                }
                return;
            }
        } catch (e) { console.error(e); }
    },

    advanceIndex() {
        if (this.shuffle) {
            this.index = Math.floor(Math.random() * this.queue.length);
        } else {
            this.index++;
            if (this.index >= this.queue.length) {
                this.index = this.repeat ? 0 : this.queue.length; // taşarsa dur
            }
        }
    },

    // Otomatik ilerlemede 403'leri atla (sonsuz döngüyü önlemek için sınırlı).
    skipForward() {
        this._attempts++;
        if (this._attempts > this.queue.length) { this._attempts = 0; return; }
        this.advanceIndex();
        if (this.index >= this.queue.length) return;
        this.playCurrent(true);
    },

    handleEnded() {
        this._attempts = 0;
        if (this.index + 1 >= this.queue.length && !this.repeat && !this.shuffle) return; // kuyruk bitti
        this.advanceIndex();
        if (this.index >= this.queue.length) return;
        this.playCurrent(true);
    },

    next() {
        if (this.queue.length === 0) return;
        this._attempts = 0;
        this.advanceIndex();
        if (this.index >= this.queue.length) this.index = this.queue.length - 1;
        this.playCurrent(false);
    },

    prev() {
        if (this.queue.length === 0) return;
        this.index--;
        if (this.index < 0) this.index = 0;
        this.playCurrent(false);
    },

    togglePlayPause() {
        if (!this.audio || !this.audio.src) return;
        if (this.audio.paused) this.audio.play(); else this.audio.pause();
    },

    syncPlayIcon() {
        const b = this.el('btn-playpause');
        if (b) b.innerText = this.audio.paused ? '▶' : '⏸';
    },

    toggleShuffle() {
        this.shuffle = !this.shuffle;
        const b = this.el('btn-shuffle');
        if (b) b.style.opacity = this.shuffle ? '1' : '0.5';
    },

    toggleRepeat() {
        this.repeat = !this.repeat;
        const b = this.el('btn-repeat');
        if (b) b.style.opacity = this.repeat ? '1' : '0.5';
    }
};

window.addEventListener('DOMContentLoaded', () => player.init());