$(function () {
    $('.main-slider .owl-carousel').owlCarousel({
        loop:true,
        center:true,
        margin:0,
        padding:0,
        nav:true,
        items:1,
        dots: false,
    });
    
    $('.card-slider .owl-carousel').owlCarousel({
        loop:false,
        center:false,
        margin:30,
        padding:0,
        nav:true,
        responsive:{
            0:{
                items:1,
            },
            600:{
                items:2,
            },
        },
        dots: true,
    });
});