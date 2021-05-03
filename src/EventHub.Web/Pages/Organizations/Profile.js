$(function () {
    $('.events > .nav-item > a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href")
        if (target === "#UpcomingEvent"){
            console.log("UpCommg")
            $("a[href='#PastEvent']").removeClass('selected');
            $("a[href='#UpcomingEvent']").addClass('selected');

        }else{
            $("a[href='#UpcomingEvent']").removeClass('selected');
            $("a[href='#PastEvent']").addClass('selected');
        }
    });
});