let timerValue = 18000;
let timerInterval;
const pieceContainers = document.getElementsByClassName('puzzle-piece');
const pieces = document.getElementsByClassName('piece');
let draggedPiece = null;

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

// document.querySelectorAll('.puzzle-piece').forEach(puzzlePiece => {
//     let row = puzzlePiece.getAttribute('row');
//     let col = puzzlePiece.getAttribute('col');
//    
//     console.log(`Puzzle Piece - Row: ${row}, Col: ${col}`);
//    
//     let imgElement = puzzlePiece.querySelector('.piece');
//    
//     let crtRow = imgElement.getAttribute('crt-row');
//     let crtCol = imgElement.getAttribute('crt-col');
//
//     console.log(`   Image - Current Row: ${crtRow}, Current Col: ${crtCol}`);
// });

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

    container.addEventListener('drop', async (e) => {
        e.preventDefault();

        if (draggedPiece) {
            const data = JSON.parse(e.dataTransfer.getData('text/plain'));
            
            const targetContainer = document
                .querySelector(`.puzzle-piece[col="${data.crtCol}"][row="${data.crtRow}"]`);

            if (!targetContainer) {
            } else {
                const targetPiece = container.querySelector('.piece');

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
                
                console.log('-----');
                console.log(piece1Row);
                console.log(piece1Col);
                console.log(piece2Row);
                console.log(piece2Col);
                console.log('-----');
                
                const movePieceRequest = {
                    Piece1Row: parseInt(piece1Row),
                    Piece1Col: parseInt(piece1Col),
                    Piece2Row: parseInt(piece2Row),
                    Piece2Col: parseInt(piece2Col),
                };

                const response = await fetch('Picture/MovePiece/', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(movePieceRequest),
                });

                let value = await response.json();

                if (value) {
                    alert('You win!');
                    window.location.href = 'Home/Index/';
                }
            }

            draggedPiece = null;
            
            timerValue -= 100;
        }
    });
});

StartGame();

document.querySelector('.hint__icon').addEventListener('click', () => {
    const modal = document.getElementById('pictureModal');

    modal.style.display = 'block';
    
    setTimeout(() => {
        const modal = document.getElementById('pictureModal');
        timerValue -= 2000
        modal.style.display = 'none';
    }, 2000); 
});

function StartGame() {
    clearInterval(timerInterval);
    document.querySelector('.timer').innerText = `${timerValue}`;

    timerInterval = setInterval(() => {
        timerValue--;
        document.querySelector('.timer').innerText = `${timerValue}`;
        if (timerValue <= 0) {
            clearInterval(timerInterval);
            timerValue = 0;
            alert('Game Over!'); 
        }
    }, 10);
}