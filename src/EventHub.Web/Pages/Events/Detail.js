$(function () {
    var initSocialShareLinks = function () {
        var pageHeader = $('#EventTitle').text();

        $('#TwitterShareLink').attr(
            'href',
            'https://twitter.com/intent/tweet?text=' +
            encodeURI(
                pageHeader + ' |  EventHub  | ' + window.location.href
            )
        );
        
        $('#LinkedinShareLink').attr(
            'href',
            'https://www.linkedin.com/shareArticle?' +
            'url=' +
            encodeURI(window.location.href) +
            '&' +
            'mini=true'
        );
        $('#FacebookShareLink').attr(
            'href',
            'https://www.facebook.com/sharer/sharer.php?' +
            'u=' +
            encodeURI(window.location.href)
        );
    };

    initSocialShareLinks();
});