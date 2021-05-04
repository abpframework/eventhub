$(function () {
    var params = new window.URLSearchParams(window.location.search);
    var minDate = Date.parse(params.get('MinDate')) || 0;
    var maxDate = Date.parse(params.get('MaxDate')) || 0;

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
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        }, cb);
    }else {
        when.daterangepicker({
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            },
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        }, cb);
    }
    
    when.on('apply.daterangepicker', function(ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
        $("#MinDate").val(picker.startDate.format('MM/DD/YYYY'));
        $("#MaxDate").val(picker.endDate.format('MM/DD/YYYY'));
        $('#EventListFilterForm').submit()
    });
    
    when.on('cancel.daterangepicker', function(ev, picker) {
        $(this).val('');
        $("#MinDate").val('');
        $("#MaxDate").val('');
        $('#EventListFilterForm').submit()
    });

    $("#EventType").on('change', function () {
        $('#EventListFilterForm').submit()
    });
});