(function () {
    abp.widgets.EventsArea = function ($wrapper) {
        var totalCount = Number($wrapper.find('[data-total-count]').attr('data-total-count'))
        var skipCount = Number($wrapper.find('[data-skip-count]').attr('data-skip-count'))
        var maxResultCount = Number($wrapper.find('[data-max-result-count]').attr('data-max-result-count'))
        var hashCode = Number($wrapper.find('[data-hash-code]').attr('data-hash-code'))

        function init() {
            var loadMoreButton = $wrapper.find('#LoadMoreButton-' + hashCode);
            loadMoreButton.click(function (e) {
                e.preventDefault();
                skipCount += maxResultCount;
                loadMoreButton.buttonBusy(true);
                abp.ajax({
                    type: 'GET',
                    dataType: 'html',
                    contentType: 'text/html; charset=utf-8',
                    url: (loadMoreButton.attr('data-url') + '&skipCount=' + skipCount)
                }).then(function (response) {
                    var eventList = $wrapper.find('#EventList')
                    eventList.append(response);
                }).always(function () {
                    var eventCount = $('.event').length;
                    if (Number(eventCount) >= totalCount) {
                        $('.load-more-section-' + hashCode).hide();
                    }
                    loadMoreButton.buttonBusy(false);
                });
            });
        }

        return {
            init: init
        };
    };
})();
