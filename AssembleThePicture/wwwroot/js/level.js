document.querySelectorAll('path').forEach(pathEl => {
    let offset = anime.setDashoffset(pathEl);
    pathEl.setAttribute('stroke-dashoffset', offset);
    anime({
        targets: pathEl,
        strokeDashoffset: [offset, 0],
        duration: anime.random(1500, 3500),
        delay: anime.random(0, 500),
        loop: true,
        direction: 'alternate',
        easing: 'easeInOutQuad',
        autoplay: true
    });
})

document.querySelectorAll('.letter').forEach(letterEl => {
    let offset = anime.setDashoffset(letterEl);
    letterEl.setAttribute('stroke-dashoffset', offset);
    anime({
        targets: letterEl,
        duration: anime.random(800, 1200),
        delay: anime.random(300, 800),
        opacity: [
            { value: 0, duration: anime.random(300, 500) },
        ],
        loop: true,
        easing: 'easeInOutQuad',
        elasticity: 400
    });
});