//console.log("Hero Slider Loaded");

//const heroArea = document.getElementById('heroArea');
//const images = [
//    '../assets/img/bg/bg-intro.jpg',
//    '../assets/img/bg/dd.jpg',
//    '../assets/img/bg/bg-intro.jpg'
//];

//let index = 0;

//function changeBackground() {
//    heroArea.style.backgroundImage = `url(${images[index]})`;
//    index = (index + 1) % images.length;
//}

//changeBackground();
//setInterval(changeBackground, 5000);

//-------------------------------------------------------------------------------------

console.log("Hero Background Slider (Slide Left) Loaded");

const images = [
    '/assets/img/bg/bg-intro.jpg',
    '/assets/img/bg/dd.jpg',
    '/assets/img/bg/bg-intro.jpg'
];

const heroArea = document.getElementById('heroArea');

// 1. إنشاء عناصر السلايدر
const sliderWrapper = document.createElement('div');
sliderWrapper.className = 'hero-slider-wrapper';

const sliderTrack = document.createElement('div');
sliderTrack.className = 'hero-slider-track';

images.forEach(imgUrl => {
    const slide = document.createElement('div');
    slide.className = 'hero-slide';
    slide.style.backgroundImage = `url(${imgUrl})`;
    sliderTrack.appendChild(slide);
});

sliderWrapper.appendChild(sliderTrack);
heroArea.prepend(sliderWrapper); // نضيفه أول عنصر في hero-area

// 2. تحريك الصور تلقائيًا
let index = 0;
setInterval(() => {
    index = (index + 1) % images.length;
    sliderTrack.style.transform = `translateX(-${index * 100}%)`;
}, 5000);

