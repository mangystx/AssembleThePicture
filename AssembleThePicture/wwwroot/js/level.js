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

const pieceContainers = document.getElementsByClassName('puzzle-piece');
const pieces = document.getElementsByClassName('piece');

let draggedPiece = null;

Array.from(pieces).forEach(piece => {
    piece.addEventListener('dragstart', (e) => {
        draggedPiece = e.target;
        e.dataTransfer.setData('text/plain', JSON.stringify({
            crtCol: e.target.getAttribute('crt-col'),
            crtRow: e.target.getAttribute('crt-row')
        }));
    });

    piece.addEventListener('dragend', () => {
        draggedPiece = null;
    });
});

Array.from(pieceContainers).forEach(container => {
    container.addEventListener('dragover', (e) => {
        e.preventDefault();
    });

    container.addEventListener('drop', (e) => {
        e.preventDefault();

        if (draggedPiece) {
            const data = JSON.parse(e.dataTransfer.getData('text/plain'));
            
            const targetContainer = document
                .querySelector(`.puzzle-piece[col="${data.crtCol}"][row="${data.crtRow}"]`);
            
            if (targetContainer) {
                const targetPiece = container.querySelector('.piece');
                
                draggedPiece.setAttribute('crt-col', container.getAttribute('col'));
                draggedPiece.setAttribute('crt-row', container.getAttribute('row'));
                
                targetPiece.setAttribute('crt-col', targetContainer.getAttribute('col'));
                targetPiece.setAttribute('crt-row', targetContainer.getAttribute('row'));
                
                container.appendChild(draggedPiece);
                targetContainer.appendChild(targetPiece);
            }
            
            draggedPiece = null;
        }
    });
});
