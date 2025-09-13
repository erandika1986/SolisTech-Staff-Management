window.videoInterop = {
    registerProgress: function (id, dotnetHelper) {
        const video = document.getElementById(id);
        if (!video) return;

        function updateProgress() {
            if (video.duration > 0) {
                let percent = (video.currentTime / video.duration) * 100;
                dotnetHelper.invokeMethodAsync(
                    "UpdateProgress",
                    percent,          // % completed
                    video.volume,     // current volume
                    video.muted       // mute state
                );
            }
        }

        // Register events
        video.addEventListener("timeupdate", updateProgress);
        video.addEventListener("volumechange", updateProgress);
        video.addEventListener("loadedmetadata", updateProgress);

        // Store refs for cleanup
        video._dotnetHelper = dotnetHelper;
        video._updateProgressHandler = updateProgress;
    },

    playVideo: id => {
        const v = document.getElementById(id);
        if (v) v.play();
    },

    pauseVideo: id => {
        const v = document.getElementById(id);
        if (v) v.pause();
    },

    stopVideo: id => {
        const v = document.getElementById(id);
        if (v) {
            v.pause();
            v.currentTime = 0;
        }
    },

    seekVideo: (id, percent) => {
        const v = document.getElementById(id);
        if (v && v.duration > 0) {
            v.currentTime = (percent / 100) * v.duration;
        }
    },

    changeVolume: (id, value) => {
        const v = document.getElementById(id);
        if (v) v.volume = Math.min(1, Math.max(0, value));
    },

    toggleMute: id => {
        const v = document.getElementById(id);
        if (v) v.muted = !v.muted;
    },

    toggleFullscreen: function (id) {
        const v = document.getElementById(id);
        if (!v) return;

        if (!document.fullscreenElement) {
            if (v.requestFullscreen) v.requestFullscreen();
            else if (v.mozRequestFullScreen) v.mozRequestFullScreen();
            else if (v.webkitRequestFullscreen) v.webkitRequestFullscreen();
            else if (v.msRequestFullscreen) v.msRequestFullscreen();
        } else {
            if (document.exitFullscreen) document.exitFullscreen();
            else if (document.mozCancelFullScreen) document.mozCancelFullScreen();
            else if (document.webkitExitFullscreen) document.webkitExitFullscreen();
            else if (document.msExitFullscreen) document.msExitFullscreen();
        }
    },

    cleanup: function (id) {
        const v = document.getElementById(id);
        if (v && v._updateProgressHandler) {
            v.removeEventListener("timeupdate", v._updateProgressHandler);
            v.removeEventListener("volumechange", v._updateProgressHandler);
            v.removeEventListener("loadedmetadata", v._updateProgressHandler);
            v._dotnetHelper = null;
            v._updateProgressHandler = null;
        }
    }
};
