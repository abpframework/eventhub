$(function () {
    $('.main-slider .owl-carousel').owlCarousel({
        loop: true,
        center: true,
        margin: 0,
        padding: 0,
        nav: true,
        items: 1,
        dots: false,
    });

    $('.card-slider .owl-carousel').owlCarousel({
        loop: false,
        center: false,
        margin: 30,
        padding: 0,
        nav: true,
        slideBy: 2,
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 2,
            },
        },
        dots: true,
    });

    var countryId = "00000000-0000-0000-0000-000000000000"
    $('#inputLocation').on('change', function (e) {
        countryId =  this.value
        if (countryId !== "00000000-0000-0000-0000-000000000000" && countryId !== "Location"){
            $('#inputLocation').hide()
            $('.locationGroup').append("<label class=\"labelCity\" for=\"inputCity\">City</label>")
            $('.backToLocation').css('visibility', 'visible')
            $('#inputCity').attr('type', 'text').show()
        }
    });

    $('.backToLocation').on('click', '', function (e) {
        $('.labelCity').remove()
        $('.backToLocation').css('visibility', 'hidden')
        $('#inputCity').val('').attr('type', 'text').hide()
        $('#inputLocation').show()
    });

    var $inputWhen = $("#inputWhen");
    $inputWhen.daterangepicker({
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }
    });
    $inputWhen.on('apply.daterangepicker', function(ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });
    $inputWhen.on('cancel.daterangepicker', function(ev, picker) {
        $(this).val('');
    });
});