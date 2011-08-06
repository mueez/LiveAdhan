$(document).ready(function () {
    var offset = -(new Date().getTimezoneOffset() / 60);
    if (window.location.search.indexOf('timeOffset') == -1) {
        window.location = '/?timeOffset=' + offset;
    }
    $('#refine-location').live('click', function () {
        // Handler for .ready() called.
        if (navigator.geolocation) {
            // snapshot of position:
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    window.location = '/?latitude=' + position.coords.latitude + '&longitude=' + position.coords.longitude + '&timeOffset=' + offset
                    + '&city=' + position.address.city + '&country=' + position.address.country;
                },
                function () {
                }
            );
            // updating position:
            // navigator.geolocation.watchPosition(renderPosition, renderError);
        } else {
            // NO
            // PUT ERROR MESSAGE HERE - OR SIMPLY DO NOTHING
        }
    });
    $('#location-label').live('mouseenter', function () {
        $('#location-map').fadeIn(100);
    });
    $('#location-label').live('mouseleave', function () {
        $('#location-map').fadeOut(100);
    });
});