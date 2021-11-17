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
            991: {
                items: 2,
            },
        },
        dots: true,
    });
    
    function cb(start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
    }

    var minDate = moment();
    var maxDate = moment();

    var $inputWhen = $("#inputWhen");
    $inputWhen.daterangepicker({
        startDate: minDate,
        endDate: maxDate,
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        },
        ranges: {
            'Today': [moment(), moment()],
            'Tomorow': [moment().add(1, 'days'), moment().add(1, 'days')],
            'Next 7 Days': [moment(), moment().add(6, 'days')],
        }
    }, cb);

    $inputWhen.on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
        minDate = picker.startDate.format('MM/DD/YYYY')
        maxDate = picker.endDate.format('MM/DD/YYYY')
    });

    $inputWhen.on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
        minDate = ""
        maxDate = ""
    });

    function isNullOrEmpty(str) {
        return str === null || str.match(/^ *$/) !== null;
    }

    $('#SearchButton').on('click', '', function () {
        var language = $('#LanguageSelect').find(":selected").val();
        var countryId = $('#CountrySelect').find(":selected").val();
        var location = "/Events?"

        if (minDate.length > 0 && !isNullOrEmpty(minDate)) {
            location += "MinDate=" + minDate
        }

        if (maxDate.length > 0 && !isNullOrEmpty(maxDate)) {
            location += "&MaxDate=" + maxDate
        }

        if (!isNullOrEmpty(language)) {
            location += "&Language=" + language
        }

        if (!isNullOrEmpty(countryId)) {
            if (countryId === "00000000-0000-0000-0000-000000000000") {
                location += "&IsOnline=true"
            } else {
                location += "&CountryId=" + countryId + "&IsOnline=false"
            }
        }

        if (!isNullOrEmpty(location) && location !== "/Events?") {
            window.location.replace(location)
        } else {
            abp.notify.error("Please select a filter", "Search")
        }
    });
});