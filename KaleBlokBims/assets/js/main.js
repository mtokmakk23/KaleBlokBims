﻿// Main Js File
$(document).ready(function () {
    'use strict';
   
    
    log();
    $("table").attr("data-pagination", "true");
    $("table").attr("data-export-data-type", "all");
    dovizCek();
    acikSiparisler();
    setInterval(function () {
        kisitlamalar();
        $(".export .bi-download").attr('class', 'fa-solid fa-download');
        $(".detail-icon").html('>');
    }, 100);
    setInterval(function () { dovizCek(); }, 1000 * 60 * 10);
    owlCarousels();
    quantityInputs();

    // Header Search Toggle

    var $searchWrapper = $('.header-search-wrapper'),
        $body = $('body'),
        $searchToggle = $('.search-toggle');

    $searchToggle.on('click', function (e) {
        $searchWrapper.toggleClass('show');
        $(this).toggleClass('active');
        $searchWrapper.find('input').focus();
        e.preventDefault();
    });

    $body.on('click', function (e) {
        if ($searchWrapper.hasClass('show')) {
            $searchWrapper.removeClass('show');
            $searchToggle.removeClass('active');
            $body.removeClass('is-search-active');
        }
    });

    $('.header-search').on('click', function (e) {
        e.stopPropagation();
    });

    // Sticky header 
    var catDropdown = $('.category-dropdown'),
        catInitVal = catDropdown.data('visible');

    if ($('.sticky-header').length && $(window).width() >= 992) {
        var sticky = new Waypoint.Sticky({
            element: $('.sticky-header')[0],
            stuckClass: 'fixed',
            offset: -300,
            handler: function (direction) {
                // Show category dropdown
                if (catInitVal && direction == 'up') {
                    catDropdown.addClass('show').find('.dropdown-menu').addClass('show');
                    catDropdown.find('.dropdown-toggle').attr('aria-expanded', 'true');
                    return false;
                }

                // Hide category dropdown on fixed header
                if (catDropdown.hasClass('show')) {
                    catDropdown.removeClass('show').find('.dropdown-menu').removeClass('show');
                    catDropdown.find('.dropdown-toggle').attr('aria-expanded', 'false');
                }
            }
        });
    }

    // Menu init with superfish plugin
    if ($.fn.superfish) {
        $('.menu, .menu-vertical').superfish({
            popUpSelector: 'ul, .megamenu',
            hoverClass: 'show',
            delay: 0,
            speed: 80,
            speedOut: 80,
            autoArrows: true
        });
    }

    // Mobile Menu Toggle - Show & Hide
    $('.mobile-menu-toggler').on('click', function (e) {
        $body.toggleClass('mmenu-active');
        $(this).toggleClass('active');
        e.preventDefault();
    });

    $('.mobile-menu-overlay, .mobile-menu-close').on('click', function (e) {
        $body.removeClass('mmenu-active');
        $('.menu-toggler').removeClass('active');
        e.preventDefault();
    });

    // Add Mobile menu icon arrows to items with children
    $('.mobile-menu').find('li').each(function () {
        var $this = $(this);

        if ($this.find('ul').length) {
            $('<span/>', {
                'class': 'mmenu-btn'
            }).appendTo($this.children('a'));
        }
    });

    // Mobile Menu toggle children menu
    $('.mmenu-btn').on('click', function (e) {
        var $parent = $(this).closest('li'),
            $targetUl = $parent.find('ul').eq(0);

        if (!$parent.hasClass('open')) {
            $targetUl.slideDown(300, function () {
                $parent.addClass('open');
            });
        } else {
            $targetUl.slideUp(300, function () {
                $parent.removeClass('open');
            });
        }

        e.stopPropagation();
        e.preventDefault();
    });

    // Sidebar Filter - Show & Hide
    var $sidebarToggler = $('.sidebar-toggler');
    $sidebarToggler.on('click', function (e) {
        $body.toggleClass('sidebar-filter-active');
        $(this).toggleClass('active');
        e.preventDefault();
    });

    $('.sidebar-filter-overlay').on('click', function (e) {
        $body.removeClass('sidebar-filter-active');
        $sidebarToggler.removeClass('active');
        e.preventDefault();
    });

    // Clear All checkbox/remove filters in sidebar filter
    $('.sidebar-filter-clear').on('click', function (e) {
        $('.sidebar-shop').find('input').prop('checked', false);

        e.preventDefault();
    });

    // Popup - Iframe Video - Map etc.
    if ($.fn.magnificPopup) {
        $('.btn-iframe').magnificPopup({
            type: 'iframe',
            removalDelay: 600,
            preloader: false,
            fixedContentPos: false,
            closeBtnInside: false
        });
    }

    // Product hover
    if ($.fn.hoverIntent) {
        $('.product-3').hoverIntent(function () {
            var $this = $(this),
                animDiff = ($this.outerHeight() - ($this.find('.product-body').outerHeight() + $this.find('.product-media').outerHeight())),
                animDistance = ($this.find('.product-footer').outerHeight() - animDiff);

            $this.find('.product-footer').css({ 'visibility': 'visible', 'transform': 'translateY(0)' });
            $this.find('.product-body').css('transform', 'translateY(' + -animDistance + 'px)');

        }, function () {
            var $this = $(this);

            $this.find('.product-footer').css({ 'visibility': 'hidden', 'transform': 'translateY(100%)' });
            $this.find('.product-body').css('transform', 'translateY(0)');
        });
    }

    // Slider For category pages / filter price
    if (typeof noUiSlider === 'object') {
        var priceSlider = document.getElementById('price-slider');

        // Check if #price-slider elem is exists if not return
        // to prevent error logs
        if (priceSlider == null) return;

        noUiSlider.create(priceSlider, {
            start: [0, 750],
            connect: true,
            step: 50,
            margin: 200,
            range: {
                'min': 0,
                'max': 1000
            },
            tooltips: true,
            format: wNumb({
                decimals: 0,
                prefix: '$'
            })
        });

        // Update Price Range
        priceSlider.noUiSlider.on('update', function (values, handle) {
            $('#filter-price-range').text(values.join(' - '));
        });
    }

    // Product countdown
    if ($.fn.countdown) {
        $('.product-countdown').each(function () {
            var $this = $(this),
                untilDate = $this.data('until'),
                compact = $this.data('compact'),
                dateFormat = (!$this.data('format')) ? 'DHMS' : $this.data('format'),
                newLabels = (!$this.data('labels-short')) ?
                    ['Years', 'Months', 'Weeks', 'Days', 'Hours', 'Minutes', 'Seconds'] :
                    ['Years', 'Months', 'Weeks', 'Days', 'Hours', 'Mins', 'Secs'],
                newLabels1 = (!$this.data('labels-short')) ?
                    ['Year', 'Month', 'Week', 'Day', 'Hour', 'Minute', 'Second'] :
                    ['Year', 'Month', 'Week', 'Day', 'Hour', 'Min', 'Sec'];

            var newDate;

            // Split and created again for ie and edge 
            if (!$this.data('relative')) {
                var untilDateArr = untilDate.split(", "), // data-until 2019, 10, 8 - yy,mm,dd
                    newDate = new Date(untilDateArr[0], untilDateArr[1] - 1, untilDateArr[2]);
            } else {
                newDate = untilDate;
            }

            $this.countdown({
                until: newDate,
                format: dateFormat,
                padZeroes: true,
                compact: compact,
                compactLabels: ['y', 'm', 'w', ' days,'],
                timeSeparator: ' : ',
                labels: newLabels,
                labels1: newLabels1

            });
        });

        // Pause
        // $('.product-countdown').countdown('pause');
    }

    // Quantity Input - Cart page - Product Details pages
    function quantityInputs() {
        if ($.fn.inputSpinner) {
            $("input[type='number']").inputSpinner({
                decrementButton: '<i class="icon-minus"></i>',
                incrementButton: '<i class="icon-plus"></i>',
                groupClass: 'input-spinner',
                buttonsClass: 'btn-spinner',
                buttonsWidth: '26px'
            });
        }
    }

    // Sticky Content - Sidebar - Social Icons etc..
    // Wrap elements with <div class="sticky-content"></div> if you want to make it sticky
    if ($.fn.stick_in_parent && $(window).width() >= 992) {
        $('.sticky-content').stick_in_parent({
            offset_top: 80,
            inner_scrolling: false
        });
    }

    function owlCarousels($wrap, options) {
        if ($.fn.owlCarousel) {
            var owlSettings = {
                items: 1,
                loop: true,
                margin: 0,
                responsiveClass: true,
                nav: true,
                navText: ['<i class="icon-angle-left">', '<i class="icon-angle-right">'],
                dots: true,
                smartSpeed: 400,
                autoplay: false,
                autoplayTimeout: 15000
            };
            if (typeof $wrap == 'undefined') {
                $wrap = $('body');
            }
            if (options) {
                owlSettings = $.extend({}, owlSettings, options);
            }

            // Init all carousel
            $wrap.find('[data-toggle="owl"]').each(function () {
                var $this = $(this),
                    newOwlSettings = $.extend({}, owlSettings, $this.data('owl-options'));

                $this.owlCarousel(newOwlSettings);

            });
        }
    }

    // Product Image Zoom plugin - product pages
    if ($.fn.elevateZoom) {
        $('#product-zoom').elevateZoom({
            gallery: 'product-zoom-gallery',
            galleryActiveClass: 'active',
            zoomType: "inner",
            cursor: "crosshair",
            zoomWindowFadeIn: 400,
            zoomWindowFadeOut: 400,
            responsive: true
        });

        // On click change thumbs active item
        $('.product-gallery-item').on('click', function (e) {
            $('#product-zoom-gallery').find('a').removeClass('active');
            $(this).addClass('active');

            e.preventDefault();
        });

        var ez = $('#product-zoom').data('elevateZoom');

        // Open popup - product images
        $('#btn-product-gallery').on('click', function (e) {
            if ($.fn.magnificPopup) {
                $.magnificPopup.open({
                    items: ez.getGalleryList(),
                    type: 'image',
                    gallery: {
                        enabled: true
                    },
                    fixedContentPos: false,
                    removalDelay: 600,
                    closeBtnInside: false
                }, 0);

                e.preventDefault();
            }
        });
    }

    // Product Gallery - product-gallery.html 
    if ($.fn.owlCarousel && $.fn.elevateZoom) {
        var owlProductGallery = $('.product-gallery-carousel');

        owlProductGallery.on('initialized.owl.carousel', function () {
            owlProductGallery.find('.active img').elevateZoom({
                zoomType: "inner",
                cursor: "crosshair",
                zoomWindowFadeIn: 400,
                zoomWindowFadeOut: 400,
                responsive: true
            });
        });

        owlProductGallery.owlCarousel({
            loop: false,
            margin: 0,
            responsiveClass: true,
            nav: true,
            navText: ['<i class="icon-angle-left">', '<i class="icon-angle-right">'],
            dots: false,
            smartSpeed: 400,
            autoplay: false,
            autoplayTimeout: 15000,
            responsive: {
                0: {
                    items: 1
                },
                560: {
                    items: 2
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 3
                }
            }
        });

        owlProductGallery.on('change.owl.carousel', function () {
            $('.zoomContainer').remove();
        });

        owlProductGallery.on('translated.owl.carousel', function () {
            owlProductGallery.find('.active img').elevateZoom({
                zoomType: "inner",
                cursor: "crosshair",
                zoomWindowFadeIn: 400,
                zoomWindowFadeOut: 400,
                responsive: true
            });
        });
    }

    // Product Gallery Separeted- product-sticky.html 
    if ($.fn.elevateZoom) {
        $('.product-separated-item').find('img').elevateZoom({
            zoomType: "inner",
            cursor: "crosshair",
            zoomWindowFadeIn: 400,
            zoomWindowFadeOut: 400,
            responsive: true
        });

        // Create Array for gallery popup
        var galleryArr = [];
        $('.product-gallery-separated').find('img').each(function () {
            var $this = $(this),
                imgSrc = $this.attr('src'),
                imgTitle = $this.attr('alt'),
                obj = { 'src': imgSrc, 'title': imgTitle };

            galleryArr.push(obj);
        })

        $('#btn-separated-gallery').on('click', function (e) {
            if ($.fn.magnificPopup) {
                $.magnificPopup.open({
                    items: galleryArr,
                    type: 'image',
                    gallery: {
                        enabled: true
                    },
                    fixedContentPos: false,
                    removalDelay: 600,
                    closeBtnInside: false
                }, 0);

                e.preventDefault();
            }
        });
    }

    // Checkout discount input - toggle label if input is empty etc...
    $('#checkout-discount-input').on('focus', function () {
        // Hide label on focus
        $(this).parent('form').find('label').css('opacity', 0);
    }).on('blur', function () {
        // Check if input is empty / toggle label
        var $this = $(this);

        if ($this.val().length !== 0) {
            $this.parent('form').find('label').css('opacity', 0);
        } else {
            $this.parent('form').find('label').css('opacity', 1);
        }
    });

    // Dashboard Page Tab Trigger
    $('.tab-trigger-link').on('click', function (e) {
        var targetHref = $(this).attr('href');

        $('.nav-dashboard').find('a[href="' + targetHref + '"]').trigger('click');

        e.preventDefault();
    });

    // Masonry / Grid layout fnction
    function layoutInit(container, selector) {
        $(container).each(function () {
            var $this = $(this);

            $this.isotope({
                itemSelector: selector,
                layoutMode: ($this.data('layout') ? $this.data('layout') : 'masonry')
            });
        });
    }

    function isotopeFilter(filterNav, container) {
        $(filterNav).find('a').on('click', function (e) {
            var $this = $(this),
                filter = $this.attr('data-filter');

            // Remove active class
            $(filterNav).find('.active').removeClass('active');

            // Init filter
            $(container).isotope({
                filter: filter,
                transitionDuration: '0.7s'
            });

            // Add active class
            $this.closest('li').addClass('active');
            e.preventDefault();
        });
    }

    /* Masonry / Grid Layout & Isotope Filter for blog/portfolio etc... */
    if (typeof imagesLoaded === 'function' && $.fn.isotope) {
        // Portfolio
        $('.portfolio-container').imagesLoaded(function () {
            // Portfolio Grid/Masonry
            layoutInit('.portfolio-container', '.portfolio-item'); // container - selector
            // Portfolio Filter
            isotopeFilter('.portfolio-filter', '.portfolio-container'); //filterNav - .container
        });

        // Blog
        $('.entry-container').imagesLoaded(function () {
            // Blog Grid/Masonry
            layoutInit('.entry-container', '.entry-item'); // container - selector
            // Blog Filter
            isotopeFilter('.entry-filter', '.entry-container'); //filterNav - .container
        });

        // Product masonry product-masonry.html
        $('.product-gallery-masonry').imagesLoaded(function () {
            // Products Grid/Masonry
            layoutInit('.product-gallery-masonry', '.product-gallery-item'); // container - selector
        });

        // Products - Demo 11
        $('.products-container').imagesLoaded(function () {
            // Products Grid/Masonry
            layoutInit('.products-container', '.product-item'); // container - selector
            // Product Filter
            isotopeFilter('.product-filter', '.products-container'); //filterNav - .container
        });
    }

    // Count
    var $countItem = $('.count');
    if ($.fn.countTo) {
        if ($.fn.waypoint) {
            $countItem.waypoint(function () {
                $(this.element).countTo();
            }, {
                offset: '90%',
                triggerOnce: true
            });
        } else {
            $countItem.countTo();
        }
    } else {
        // fallback
        // Get the data-to value and add it to element
        $countItem.each(function () {
            var $this = $(this),
                countValue = $this.data('to');
            $this.text(countValue);
        });
    }

    // Scroll To button
    var $scrollTo = $('.scroll-to');
    // If button scroll elements exists
    if ($scrollTo.length) {
        // Scroll to - Animate scroll
        $scrollTo.on('click', function (e) {
            var target = $(this).attr('href'),
                $target = $(target);
            if ($target.length) {
                // Add offset for sticky menu
                var scrolloffset = ($(window).width() >= 992) ? ($target.offset().top - 52) : $target.offset().top
                $('html, body').animate({
                    'scrollTop': scrolloffset
                }, 600);
                e.preventDefault();
            }
        });
    }

    // Review tab/collapse show + scroll to tab
    $('#review-link').on('click', function (e) {
        var target = $(this).attr('href'),
            $target = $(target);

        if ($('#product-accordion-review').length) {
            $('#product-accordion-review').collapse('show');
        }

        if ($target.length) {
            // Add offset for sticky menu
            var scrolloffset = ($(window).width() >= 992) ? ($target.offset().top - 72) : ($target.offset().top - 10)
            $('html, body').animate({
                'scrollTop': scrolloffset
            }, 600);

            $target.tab('show');
        }

        e.preventDefault();
    });

    // Scroll Top Button - Show
    var $scrollTop = $('#scroll-top');

    $(window).on('load scroll', function () {
        if ($(window).scrollTop() >= 400) {
            $scrollTop.addClass('show');
        } else {
            $scrollTop.removeClass('show');
        }
    });

    // On click animate to top
    $scrollTop.on('click', function (e) {
        $('html, body').animate({
            'scrollTop': 0
        }, 800);
        e.preventDefault();
    });

    // Google Map api v3 - Map for contact pages
    if (document.getElementById("map") && typeof google === "object") {

        var content = '<address>' +
            '88 Pine St,<br>' +
            'New York, NY 10005, USA<br>' +
            '<a href="#" class="direction-link" target="_blank">Get Directions <i class="icon-angle-right"></i></a>' +
            '</address>';

        var latLong = new google.maps.LatLng(40.8127911, -73.9624553);

        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 14,
            center: latLong, // Map Center coordinates
            scrollwheel: false,
            mapTypeId: google.maps.MapTypeId.ROADMAP

        });

        var infowindow = new google.maps.InfoWindow({
            content: content,
            maxWidth: 360
        });

        var marker;
        marker = new google.maps.Marker({
            position: latLong,
            map: map,
            animation: google.maps.Animation.DROP
        });

        google.maps.event.addListener(marker, 'click', (function (marker) {
            return function () {
                infowindow.open(map, marker);
            }
        })(marker));
    }

    var $viewAll = $('.view-all-demos');
    $viewAll.on('click', function (e) {
        e.preventDefault();
        $('.demo-item.hidden').addClass('show');
        $(this).addClass('disabled-hidden');
    })

    var $megamenu = $('.megamenu-container .sf-with-ul');
    $megamenu.hover(function () {
        $('.demo-item.show').addClass('hidden');
        $('.demo-item.show').removeClass('show');
        $viewAll.removeClass('disabled-hidden');
    });

    // Product quickView popup
    $('.btn-quickview').on('click', function (e) {
        var ajaxUrl = $(this).attr('href');
        if ($.fn.magnificPopup) {
            setTimeout(function () {
                $.magnificPopup.open({
                    type: 'ajax',
                    mainClass: "mfp-ajax-product",
                    tLoading: '',
                    preloader: false,
                    removalDelay: 350,
                    items: {
                        src: ajaxUrl
                    },
                    callbacks: {
                        ajaxContentAdded: function () {
                            owlCarousels($('.quickView-content'), {
                                onTranslate: function (e) {
                                    var $this = $(e.target),
                                        currentIndex = ($this.data('owl.carousel').current() + e.item.count - Math.ceil(e.item.count / 2)) % e.item.count;
                                    $('.quickView-content .carousel-dot').eq(currentIndex).addClass('active').siblings().removeClass('active');
                                }
                            });
                            quantityInputs();
                        },
                        open: function () {
                            $('body').css('overflow-x', 'visible');
                            $('.sticky-header.fixed').css('padding-right', '1.7rem');
                        },
                        close: function () {
                            $('body').css('overflow-x', 'hidden');
                            $('.sticky-header.fixed').css('padding-right', '0');
                        }
                    },

                    ajax: {
                        tError: '',
                    }
                }, 0);
            }, 500);

            e.preventDefault();
        }
    });
    $('body').on('click', '.carousel-dot', function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
    });

    $('body').on('click', '.btn-fullscreen', function (e) {
        var galleryArr = [];
        $(this).parents('.owl-stage-outer').find('.owl-item:not(.cloned)').each(function () {
            var $this = $(this).find('img'),
                imgSrc = $this.attr('src'),
                imgTitle = $this.attr('alt'),
                obj = { 'src': imgSrc, 'title': imgTitle };
            galleryArr.push(obj);
        });

        var ajaxUrl = $(this).attr('href');

        var mpInstance = $.magnificPopup.instance;
        if (mpInstance.isOpen)
            mpInstance.close();

        setTimeout(function () {
            $.magnificPopup.open({
                type: 'ajax',
                mainClass: "mfp-ajax-product",
                tLoading: '',
                preloader: false,
                removalDelay: 350,
                items: {
                    src: ajaxUrl
                },
                callbacks: {
                    ajaxContentAdded: function () {
                        owlCarousels($('.quickView-content'), {
                            onTranslate: function (e) {
                                var $this = $(e.target),
                                    currentIndex = ($this.data('owl.carousel').current() + e.item.count - Math.ceil(e.item.count / 2)) % e.item.count;
                                $('.quickView-content .carousel-dot').eq(currentIndex).addClass('active').siblings().removeClass('active');
                                $('.curidx').html(currentIndex + 1);
                            }
                        });
                        quantityInputs();
                    },
                    open: function () {
                        $('body').css('overflow-x', 'visible');
                        $('.sticky-header.fixed').css('padding-right', '1.7rem');
                    },
                    close: function () {
                        $('body').css('overflow-x', 'hidden');
                        $('.sticky-header.fixed').css('padding-right', '0');
                    }
                },

                ajax: {
                    tError: '',
                }
            }, 0);
        }, 500);

        e.preventDefault();
    });

    if (document.getElementById('newsletter-popup-form')) {
        setTimeout(function () {
            var mpInstance = $.magnificPopup.instance;
            if (mpInstance.isOpen) {
                mpInstance.close();
            }

            setTimeout(function () {
                $.magnificPopup.open({
                    items: {
                        src: '#newsletter-popup-form'
                    },
                    type: 'inline',
                    removalDelay: 350,
                    callbacks: {
                        open: function () {
                            $('body').css('overflow-x', 'visible');
                            $('.sticky-header.fixed').css('padding-right', '1.7rem');
                        },
                        close: function () {
                            $('body').css('overflow-x', 'hidden');
                            $('.sticky-header.fixed').css('padding-right', '0');
                        }
                    }
                });
            }, 500)
        }, 10000)
    }


});

var GuncelUSD = 0;
var GuncelEUR = 0;

function log() {
    $.getJSON("https://api.ipify.org?format=json", function (data) {

        var theData = {
            ip: data.ip,
            sayfaLinki: window.location.pathname
        }

        $.ajax({
            url: '/Logger/LogKayit/',
            type: 'POST',
            dataType: 'text',
            data: theData,
            success: function (sonuc) {
                

            }
        });

    });
}
function dovizCek() {
    $(".GuncelUSD").html(GuncelUSD);
    $(".GuncelEUR").html(GuncelEUR);
    $.ajax({
        url: '/Dealer_Anasayfa/DovizCek/',
        type: 'POST',
        dataType: 'text',
        success: function (sonuc) {
            var result = JSON.parse(sonuc);
            for (var i = 0; i < result.length; i++) {
                GuncelEUR = result[i].EUR;
                GuncelUSD = result[i].USD;
                $(".KurGuncellemeSaati").html(result[i].guncellemeSaati);
                $(".GuncelUSD").html(GuncelUSD);
                $(".GuncelEUR").html(GuncelEUR);
            }
        }
    });
}

function sistemKalemleriEkle() {
    loading(true);
    $.ajax({
        url: '/Dealer_Anasayfa/sistemKalemleriEkle/',
        type: 'POST',
        dataType: 'text',
        success: function (sonuc) {
            loading(false);
            acikSiparisler();
        }
    });
}

var acikSiparislerYuklendiMi = false;
var bekleyenBaslikBilgileri;
function paletEkle() {
    $.ajax({
        url: '/Dealer_Anasayfa/PaletHesapla/',
        type: 'POST',
        dataType: 'text',
        success: function (sonuc) {

            var result = JSON.parse(sonuc);
            if (Boolean(result.IsSuccessStatusCode)) {
                acikSiparisler();
            } else {
                alert(result.ErrorMessage);
            }
        }
    });
}
function acikSiparisler() {
    $(".dropdown-cart-products").html("");
    $(".cart-count").html("0");
    $(".araToplam").html("");
    $(".kdvToplam").html("");
    $(".genelToplam").html("");
    $.ajax({
        url: '/Dealer_Anasayfa/acikSiparisler/',
        type: 'POST',
        dataType: 'text',
        success: function (sonuc) {

            var result = JSON.parse(sonuc);
            if (Boolean(result.IsSuccessStatusCode)) {
                if (result.Content == "Açık Sipariş Yok") {
                    acikSiparislerYuklendiMi = true;
                    return;
                }
                bekleyenBaslikBilgileri = JSON.parse(result.Content);
                var icerik = JSON.parse(bekleyenBaslikBilgileri[0].Malzemeler);
                acikSiparislerYuklendiMi = true;
                $(".dropdown-cart-products").html("");
                $(".cart-count").html(icerik.length);
                var araToplam = 0;
                var KdvToplam = 0;
                for (var i = 0; i < icerik.length; i++) {
                    $(".dropdown-cart-products").append('' +
                        ' <div class="product">' +
                        ' <div class= "product-cart-details" style="max-width: 100%;">' +
                        '<h4 class="product-title">' +
                        '    <a>' + icerik[i].MalzemeAdi + '</a>' +
                        '</h4>' +
                        '<span class="cart-product-info">' +
                        '    <span class="cart-product-qty">' + formatMoney(icerik[i].Miktar) + '</span> x ' + formatMoney(icerik[i].HesaplanmisBirimFiyatiTL) + ' TL' +
                        '=' + formatMoney(parseFloat(icerik[i].Miktar) * parseFloat(icerik[i].HesaplanmisBirimFiyatiTL)) + ' TL</span>' +
                        ' </div >' +
                        '<figure class="product-image-container">' +
                        '    <a class="product-image" style="font-family: Arial, Helvetica, sans-serif">' +
                        '        <img src="/assets/images/gorselHazirlanıyor.png" alt="product">' +
                        '    </a>' +
                        '</figure>' +
                        '    <a onclick="UrunSepettenCikar(' + icerik[i].LOGICALREF + ')" class="btn-remove" title="Remove Product"><i class="icon-close"></i></a>' +
                        '</div>' +
                        '');
                    araToplam += parseFloat(icerik[i].Miktar) * parseFloat(icerik[i].HesaplanmisBirimFiyatiTL);
                    KdvToplam += (parseFloat(icerik[i].Miktar) * parseFloat(icerik[i].HesaplanmisBirimFiyatiTL)) * parseFloat(icerik[i].Kdv) / 100;

                }
                $(".araToplam").html("₺ "+formatMoney(araToplam) + " TL");
                $(".kdvToplam").html("₺ " +formatMoney(KdvToplam) + " TL");
                $(".genelToplam").html("₺ " +formatMoney(araToplam + KdvToplam) + " TL");
            }
        }
    });
}


var sepetOnaylaOdemeTipi = "";
var sepetOnaylaSiparisNotu = "";
function SepetiOnayla(step) {
    if (step == 0) {
        sepetOnaylaOdemeTipi = "";
        sepetOnaylaSiparisNotu = "";
        Swal.fire({
            title: 'Sipariş Notu Ekle',
            html:
                '<b style="float:left">Ödeme Tipi:</b><select id="swal-input1" class="swal2-select" style="float:right"><option>Seçiniz</option><option>Nakit</option><option>Bağlantı Ödemesi</option><option>Kredi Kartı Tek Çekim</option><option>Kredi Kartı Taksit</option><option>30 GÜN VADE</option><option>60 GÜN VADE</option><option>90 GÜN VADE</option></select>' +
                '<div style="clear:both"></div>' +
                '<b style="float:left">Sipariş Notu:</b><textarea id="swal-input2" class="form-control"  maxlength="100"></textarea>',
            focusConfirm: false,
            preConfirm: () => {
                sepetOnaylaOdemeTipi = $("#swal-input1 option:selected").text();
                sepetOnaylaSiparisNotu = $("#swal-input2").val();

                SepetiOnayla(1);
            }
        });
       
    }
    if (step == 1) {

        Swal.fire({
            title: 'Sipariş Onaylansın Mı?',
        //    html:'<b>deneme</b>',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'EVET',
            denyButtonText: `HAYIR`,
        }).then((result) => {
            if (result.isConfirmed) {

                sepetOnaylaSiparisNotu = $("#swal-input2").val();
                var theData = {
                    sepetOnaylaOdemeTipi: "",
                    sepetOnaylaSiparisNotu: sepetOnaylaSiparisNotu
                }
                loading(true);
                $.ajax({
                    url: '/Dealer_Anasayfa/SepetiOnayla/',
                    type: 'POST',
                    dataType: 'text',
                    data: theData,
                    success: function (sonuc) {
                        loading(false);
                        var result = JSON.parse(sonuc);
                        if (Boolean(result.IsSuccessStatusCode)) {
                            var path = window.location.href;
                            if (path.indexOf("SiparisOlustur" != -1)) {
                                $(".siparisBilgileri").remove();
                                $(".urunlerDivi").remove();
                            }
                            acikSiparisler();
                            Swal.fire({
                                title: '<strong>Sipariş Onaylandı</strong>',
                                icon: 'success',
                                showCloseButton: true,
                                showCancelButton: false,
                                focusConfirm: false,
                                confirmButtonText:
                                    '<a download="Sipariş Formu.pdf" href="data:application/octet-stream;base64,' + result.Content + '" class="text-white"><i class="fa-solid fa-download"></i>Sipariş Formunu İndir</a>',

                            });

                        } else {
                            HataMesaji(result.ErrorMessage);
                        }
                    }
                });

            } else if (result.isDenied) {
               
            }
        })

       
    }

}

function UrunSepettenCikar(LOGICALREF) {
    var theData = {
        LOGICALREF: LOGICALREF
    }
    $.ajax({
        url: '/Dealer_Anasayfa/UrunSepettenCikar/',
        type: 'POST',
        dataType: 'text',
        data: theData,
        success: function (sonuc) {
            var result = JSON.parse(sonuc);
            if (Boolean(result.IsSuccessStatusCode)) {
                acikSiparisler()
            } else {
                HataMesaji(result.ErrorMessage);
            }
        }
    });
}
function kisitlamalar() {
    $(".sadeceSayi").attr("onkeypress", "return TextboxaHarfGirisiniEngelleVirgulHaric(this,event)");

}

function TextboxaHarfGirisiniEngelleVirgulHaric(myfield, e, dec) {
    var key;
    var keychar;

    if (window.event)
        key = window.event.keyCode;
    else if (e)
        key = e.which;
    else
        return true;
    keychar = String.fromCharCode(key);
    // control keys
    if ((key == null) || (key == 0) || (key == 8) ||
        (key == 9) || (key == 13) || (key == 27) || (key == 44))
        return true;

    // numbers
    else if ((("0123456789").indexOf(keychar) > -1))
        return true;

    // decimal point jump
    else if (dec && (keychar == ".")) {
        myfield.form.elements[dec].focus();
        return false;
    }
    else
        return false;
}
function formatMoney(n, c, d, t) {//sayı noktalı olarak gelmeli
    var c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "," : d,
        t = t == undefined ? "." : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;

    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
}

//function htmlDecode(input) {
//    var doc = new DOMParser().parseFromString(input, "text/html");
//    return doc.documentElement.textContent;
//}
function loading(step) {
    if (step == true) {
        Swal.fire({
            title: 'Lütfen Bekleyin!',
            timerProgressBar: true,
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading()
                //const b = Swal.getHtmlContainer().querySelector('b')
                //timerInterval = setInterval(() => {
                //   // b.textContent = Swal.getTimerLeft()
                //}, 100)
            },

        })
    } else {
        Swal.hideLoading();
        Swal.close();
    }

}


function OnayMesajiToast(text, milisecond) {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: milisecond,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: 'success',
        title: text
    })
}

function HataMesajiToast(text, milisecond) {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: milisecond,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: 'error',
        title: text
    })
}

function OnayMesaji(text) {
    Swal.fire(
        'BAŞARILI',
        text,
        'success'
    )
}
function UyariMesaji(text) {
    Swal.fire(
        'DİKKAT!',
        text,
        'warning'
    )
}
function HataMesaji(text) {
    Swal.fire(
        'HATA!',
        text,
        'error'
    )
}
function ResimliModal() {
    Swal.fire({
        title: 'Sweet!',
        text: 'Modal with a custom image.',
        imageUrl: 'https://unsplash.it/400/200',
        imageWidth: 400,
        imageHeight: 200,
        imageAlt: 'Custom image',
    })
}

