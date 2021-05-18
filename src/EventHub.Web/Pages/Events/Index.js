$(function () {
    var params = new window.URLSearchParams(window.location.search);
    var minDate = Date.parse(params.get('MinDate')) || 0;
    var maxDate = Date.parse(params.get('MaxDate')) || 0;
    var isOnline = params.get('IsOnline') || "false";
    var countryId = params.get('CountryId');
    
    if (isOnline === "true" || countryId === "00000000-0000-0000-0000-000000000000"){
        $('#CountrySelect').val("00000000-0000-0000-0000-000000000000")
        isOnline = true
    }
    
    if (!isNaN(minDate)){
        minDate = params.get('MinDate')
    }else{
        minDate = ''
    }

    if (!isNaN(maxDate)){
        maxDate = params.get('MaxDate')
    }else{
        maxDate = ''
    }
    
    function cb(start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
    }

    function getRanges() {
        return {
            'Today': [moment(), moment()],
            'Tomorow': [moment().add(1, 'days'), moment().add(1, 'days')],
            'Next 7 Days': [moment(), moment().add(6, 'days')],
        };
    }
    
    var when = $("#WhenInput");
    if (minDate !== null && maxDate !== null && minDate.length > 0 && maxDate.length > 0){
        when.val(minDate + ' - ' + maxDate);
        $("#MinDate").val(minDate);
        $("#MaxDate").val(maxDate);
        when.daterangepicker({
            startDate: minDate,
            endDate: maxDate,
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            },
            ranges: getRanges()
        }, cb);
    }else {
        when.daterangepicker({
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            },
            ranges: getRanges()
        }, cb);
    }

    function submitEventFilter() {
        var language = $('#LanguageSelect').find(":selected").val();
        var countryId = $('#CountrySelect').find(":selected").val();
        var minDate = $("#MinDate").val()
        var maxDate = $("#MaxDate").val()

        var location = "/Events?"

        if (!isNullOrEmpty(minDate)) {
            location += "MinDate=" + minDate
        }

        if (!isNullOrEmpty(maxDate)) {
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
            $('#EventListFilterForm').submit()
        }
    }
    
    when.on('apply.daterangepicker', function(ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
        $("#MinDate").val(picker.startDate.format('MM/DD/YYYY'));
        $("#MaxDate").val(picker.endDate.format('MM/DD/YYYY'));
        submitEventFilter()
    });
    
    when.on('cancel.daterangepicker', function(ev, picker) {
        $(this).val('');
        $("#MinDate").val('');
        $("#MaxDate").val('');
        submitEventFilter()
    });

    function isNullOrEmpty(str){
        return str === null || str.match(/^ *$/) !== null;
    }

    $("#CountrySelect, #LanguageSelect").on('change', function () {
        submitEventFilter();
    });
});