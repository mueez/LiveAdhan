$(document).ready(function () {
    var offset = -(new Date().getTimezoneOffset() / 60);
    if (offset != $('#TimeOffset').val()) {
        $('#TimeOffset').val(offset);
        $('#Settings').submit();
    }
    $('#refine-location').live('click', function (event) {
        // Handler for .ready() called.
        if (navigator.geolocation) {
            // snapshot of position:
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    if (position.coords) {
                        $('#Latitude').val(position.coords.latitude);
                        $('#Longitude').val(position.coords.longitude);
                    }
                    if (position.address) {
                        $('#Country').val(position.address.country);
                        $('#City').val(position.address.city);
                    }
                    $('#Settings').submit();
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
        event.preventDefault();
        return false;
    });
    $('#location-label').live('mouseenter', function () {
        $('#location-map').fadeIn(100);
    });
    $('#location-label').live('mouseleave', function () {
        $('#location-map').fadeOut(100);
    });
});