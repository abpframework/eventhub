(function () {
    abp.widgets.MembersArea = function ($wrapper) {
        var widgetManager = $wrapper.data('abp-widget-manager');

        var organizationId = $wrapper.find('[data-organization-id]').attr('data-organization-id');
        var totalCount = Number($wrapper.find('[data-total-count]').attr('data-total-count'))
        var skipCount = Number($wrapper.find('[data-skip-count]').attr('data-skip-count'))
        var maxResultCount = Number($wrapper.find('[data-max-result-count]').attr('data-max-result-count'))
        var isPagination = $wrapper.find('[data-is-pagination]').attr('data-is-pagination')
        var isMoreDetail = $wrapper.find('[data-is-more-detail]').attr('data-is-more-detail')
        var hashCode = Number($wrapper.find('[data-hash-code]').attr('data-hash-code'))
        skipCount += maxResultCount;

        function getFilters() {
            return {
                organizationId: organizationId,
                maxResultCount: maxResultCount,
                isPagination: isPagination,
                isMoreDetail: isMoreDetail
            };
        }

        function manipulateDOMForMoreDetail() {
            var hashCode = Number($wrapper.find('[data-hash-code]').attr('data-hash-code'))
            $('#MemberList-' + hashCode + '> div').addClass('col-6 col-md-4 col-lg-2');
            $('#MemberList-' + hashCode + '> * > div > .member-name').removeAttr('style');
            $('#MemberList-' + hashCode + '> * > div').addClass('member-container text-center');
            $('#load-more-section-' + hashCode).addClass('text-center mt-3');
        }

        if (isMoreDetail === 'True') {
            manipulateDOMForMoreDetail()
        }

        function init() {
            var loadMoreButton = $wrapper.find('#LoadMoreButton-' + hashCode);
            loadMoreButton.click(function (e) {
                e.preventDefault();
                loadMoreButton.buttonBusy(true);
                abp.ajax({
                    type: 'GET',
                    dataType: 'html',
                    contentType: 'text/html; charset=utf-8',
                    url: (loadMoreButton.attr('data-url') + '&skipCount=' + skipCount)
                }).then(function (response) {
                    var memberList = $wrapper.find('#MemberList-' + hashCode)
                    memberList.append(response);
                    skipCount += maxResultCount;
                }).always(function () {
                    if (isMoreDetail === 'True') {
                        manipulateDOMForMoreDetail()
                    }
                    if (skipCount >= totalCount) {
                        loadMoreButton.hide();
                    }
                    loadMoreButton.buttonBusy(false);
                });
            });
        }
        
        abp.event.on("EventHub.Organization.JoinStatusChanged", function () {
            var memberAreaWidgets = document.querySelectorAll('[data-widget-name="MembersArea"]')
            memberAreaWidgets.forEach(function(entry) {
                $(entry).data('abp-widget-manager')
                    .refresh();
            });
        });

        return {
            getFilters: getFilters,
            init: init
        };
    };
})();
