/*
Template Name: ShopGrids - Bootstrap 5 eCommerce HTML Template.
Author: GrayGrids
*/

$(function () {
    //===== Preloader
    $(window).on('load', function () {
        setTimeout(function () {
            $('.preloader').css('opacity', '0').hide();
        }, 500);
    });

    /*=====================================
    Sticky / Back-to-top
    ======================================= */
    $(window).on('scroll', function () {
        var scrollTop = $(document).scrollTop();
        if (scrollTop > 50) {
            $('.scroll-top').css('display', 'flex');
        } else {
            $('.scroll-top').hide();
        }
    });

    //===== mobile-menu-btn
    $('.mobile-menu-btn').on('click', function () {
        $(this).toggleClass('active');
    });
});

//===== Hero Slider
if ($('.hero-slider').length) {
    tns({
        container: '.hero-slider',
        slideBy: 'page',
        autoplay: true,
        autoplayButtonOutput: false,
        mouseDrag: true,
        gutter: 0,
        items: 1,
        nav: false,
        controls: true,
        controlsText: ['<i class="lni lni-chevron-left"></i>', '<i class="lni lni-chevron-right"></i>'],
    });
}

//===== Brand Slider
if ($('.brands-logo-carousel').length) {
    tns({
        container: '.brands-logo-carousel',
        autoplay: true,
        autoplayButtonOutput: false,
        mouseDrag: true,
        gutter: 15,
        nav: false,
        controls: false,
        responsive: {
            0: {
                items: 1,
            },
            540: {
                items: 3,
            },
            768: {
                items: 5,
            },
            992: {
                items: 6,
            }
        }
    });
}
