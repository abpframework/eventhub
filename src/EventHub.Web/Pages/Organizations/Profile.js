$(function () {
    $('.events > .nav-item > a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href")
        if (target === "#UpcomingEvent") {
            $("a[href='#PastEvent']").removeClass('selected');
            $("a[href='#UpcomingEvent']").addClass('selected');

        } else {
            $("a[href='#UpcomingEvent']").removeClass('selected');
            $("a[href='#PastEvent']").addClass('selected');
        }
    });

    var searchParams = new URLSearchParams(window.location.search);
    var paymentRequestId = searchParams.get('paymentRequestId');
    if (paymentRequestId && paymentRequestId.length === 36) {
        payment.paymentRequests.paymentRequest
            .get(paymentRequestId)
            .then(function (r) {
                if (r.extraProperties.Status === "COMPLETED") {
                    Swal.fire({
                        title: 'Thank you for your purchase!',
                        text: 'Your Organization has been upgraded.',
                        imageUrl: '/assets/success.svg',
                        imageHeight: "70",
                        imageWidth: "70",
                        showCloseButton: true,
                        confirmButtonText: 'Close'
                    }).then(() => {
                        window.location.replace(window.location.pathname)
                    });
                }
            });
    }

});
