const scrollHeaders = document.querySelectorAll('.scroll-header');
const btnLogin = document.querySelector('.btn-login');
const wrapper = document.querySelector('.wrapper');
const registerLink = document.querySelector('.register-link');
const iconClose = document.querySelector('.icon-close');

document.addEventListener('DOMContentLoaded', OnLoad);

let pathEls = document.querySelectorAll('path');
pathEls.forEach(pathEl => {
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

let letterEls = document.querySelectorAll('.letter');
letterEls.forEach(letterEl => {
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

btnLogin.addEventListener('click', BtnLoginOnClick);

iconClose.addEventListener('click', () => {
    wrapper.classList.remove('active-login-menu');
});

registerLink.addEventListener('click', () => {
    wrapper.classList.add('active');
});

document.addEventListener('click', async event => {
    if (event.target.closest('.btn-logout')) {
        const response = await fetch('/Home/Logout', {
            method: 'GET'
        });
        if (response.ok) {
            location.reload();
            const button = document.querySelector('.btn-logout');
            button.classList.remove('btn-logout');
            button.classList.add('btn-login');
        } else {
            console.error('Failed to logout');
        }
    }
});

scrollHeaders.forEach(header => {
    header.addEventListener('click', event => {
        event.preventDefault();
        document.querySelector('header').scrollIntoView({ behavior: 'smooth' });
    });
});

const uploadButton = document.getElementById('uploadBtn');

uploadButton.addEventListener('click', () => {
    let input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    
    const handleFileSelection = function () {
        let file = input.files[0];
        if (file) {
            uploadFile(file);
            setTimeout(() => {
                location.reload();
            },1000);
        }
    };

    input.addEventListener('change', handleFileSelection);

    // Append the input to the body so that the click event works as expected
    document.body.appendChild(input);

    // Trigger the click event after the input has been appended
    input.click();

    // Remove the input from the DOM after the file is selected
    input.remove();
});

function uploadFile(file) {
    console.log("in upload file")
    let formData = new FormData();
    formData.append('file', file);

    fetch('/Home/AddImage', {
        method: 'POST',
        body: formDataоиріґвисгпцщиавгнвмсзгіфнврмжіфвм
        
        signal
        headersічлдіь
    }).catch(error => {
        console.error('Error uploading file:', error);
    });
}

WE=function (=EFWFWSC`PIOJCгшщчяпсзшгні`)    fetch('/Picture/Puzzle', {
        method: 'POST',
        headers: {
            '
        body: JSON.stringify(pictureId)
    }).then(response => response.text())
        .then(result => {
            document.open();
            document.write(result);
            document.close();
        })
}

async function  BtnLoginOnClick() {
    await fetch("/Home/Login/");
    wrapper.classList.add('active-login-menu');
    document.querySelector('header').scrollIntoView({ behavior: 'smooth' });
}

async function OnLoad(){
    const response = await fetch("/Home/IsAuthorized");
    if (response.ok) {
        btnLogin.addEventListener('click', () => {

        });
        btnLogin.removeEventListener('click', BtnLoginOnClick)
        btnLogin.classList.remove('btn-login');
        btnLogin.classList.add('btn-logout');
    }
}