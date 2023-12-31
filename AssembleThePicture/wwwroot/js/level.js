let score = 18000;
let timerInterval;
const pieceContainers = document.getElementsByClassName('puzzle-piece');
const pieces = document.getElementsByClassName('piece');

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

Array.from(pieces).forEach(piece => {
    piece.addEventListener('dragstart', (e) => {
        e.dataTransfer.setData('text/plain', JSON.stringify({
            crtCol: e.target.getAttribute('crt-col'),
            crtRow: e.target.getAttribute('crt-row')
        }));
    });

    piece.addEventListener('dragend', (e) => {
        e.preventDefault();
    });
});

Array.from(pieceContainers).forEach(container => {
    container.addEventListener('dragover', (e) => {
        e.preventDefault();
    });

    container.addEventListener('drop', async (e) => {
        e.preventDefault();
        console.log("event drop")
        const data = JSON.parse(e.dataTransfer.getData('text/plain'));
        
        const targetContainer = document.querySelector(`.puzzle-piece[col="${data.crtCol}"][row="${data.crtRow}"]`);
        const targetPiece = container.querySelector('.piece');
        let draggedPiece = targetContainer.querySelector('.piece');
        
        draggedPiece.setAttribute('crt-col', container.getAttribute('col'));
        draggedPiece.setAttribute('crt-row', container.getAttribute('row'));
        targetPiece.setAttribute('crt-col', targetContainer.getAttribute('col'));
        targetPiece.setAttribute('crt-row', targetContainer.getAttribute('row'));
        
        container.appendChild(draggedPiece);
        targetContainer.appendChild(targetPiece);
        
        let piece1Row = container.getAttribute('row');
        let piece1Col = container.getAttribute('col');
        let piece2Row = targetContainer.getAttribute('row');
        let piece2Col = targetContainer.getAttribute('col');
        
        const movePieceRequest = {
            Piece1Row: parseInt(piece1Row),
            Piece1Col: parseInt(piece1Col),
            Piece2Row: parseInt(piece2Row),
            Piece2Col: parseInt(piece2Col),
        };

        const response = await fetch(new URL('Picture/MovePiece/', window.location.origin), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(movePieceRequest),
        });
        
        let value = await response.json();
        if (value) {
            setTimeout(() => {
                alert('You win!');
                const result = fetch(new URL('Picture/AddNewScore/', window.location.origin), {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(parseInt(score))
                });
                window.location.href = '';
            }, 300); 
        }
        
        score -= 100;
    });
});

StartGame();

document.querySelector('.hint__icon').addEventListener('click', () => {
    const modal = document.getElementById('pictureModal');

    modal.style.display = 'block';
    
    setTimeout(() => {
        const modal = document.getElementById('pictureModal');
        score -= 2000
        modal.style.display = 'none';
    }, 2000); 
});

function StartGame() {
    clearInterval(timerInterval);
    document.querySelector('.right-side-menu__score').innerText = `${score}`;

    timerInterval = setInterval(() => {
        score--;
        document.querySelector('.right-side-menu__score').innerText = `${score}`;
        if (score <= 0) {
            clearInterval(timerInterval);
            document.querySelector('.right-side-menu__score').innerText = `0`;
            alert('Game Over!');
            
            window.location.href = '';
        }
    }, 10);
}