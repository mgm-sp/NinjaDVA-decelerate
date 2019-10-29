(function () {
    window.addEventListener('load', function () {
        /* Register event for slider change: */
        $('#sliderInput').change(function () {
            $('#sliderForm').submit();
        });
    }, false);
})();