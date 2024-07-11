let audio = null;

function playAudio(base64Audio) {
    if (audio) {
        audio.pause();
        audio = null;
    }
    audio = new Audio("data:audio/mp3;base64," + base64Audio);
    audio.play();
}

function stopAudio() {
    if (audio) {
        audio.pause();
        audio = null;
    }
}