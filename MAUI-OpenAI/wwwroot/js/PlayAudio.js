function playAudio(base64Audio) {
    const audio = new Audio("data:audio/mp3;base64," + base64Audio);
    audio.play();
}