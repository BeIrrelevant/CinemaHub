
$('.slick-track').slick({
    slidesToShow: 2,
    slidesToScroll: 1,
    autoplay: true,
    autoplaySpeed: 1000,
    responsive: [
        {
            breakpoint: 1000,
            setting: {
                slidesToShow:1,
            }
        }
    ]
});
$('.coming-soon-slider').slick({
    slidesToShow: 3,
    slidesToScroll: 1,
    autoplay: true,
    autoplaySpeed: 1120,
    responsive: [
        {
            breakpoint: 1000,
            setting: {
                slidesToShow: 2,
            },
        },
        {
            breakpoint: 800,
            setting: {
                slidesToShow: 1,
            },
        }
    ]
});
$('.old-movies-slider').slick({
    slidesToShow: 3,
    slidesToScroll: 1,
    autoplay: true,
    autoplaySpeed: 1100,
    responsive: [
        {
            breakpoint: 1000,
            setting: {
                slidesToShow: 2,
            }
        },
        {
            breakpoint: 800,
            setting: {
                slidesToShow: 1,
            }
        }
    ]
});


$("#trailer-btn").click(function (){
    $("#trailer").toggleClass("shown");
    return false;
});


function makeVisible(id) {
    var element = document.getElementById("c " + id);
    element.classList.toggle("invisible");
}

function cinema() {
    var cities = document.getElementById("cities");
    var value = cities.value;

    var op = document.getElementById("cinemas").getElementsByTagName("option");
    for (var i = 0; i < op.length; i++) {
        op[i].hidden = (op[i].id != value);
    }
}