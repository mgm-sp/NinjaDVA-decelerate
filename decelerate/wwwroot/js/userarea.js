(function () {
    window.addEventListener('load', function () {
        /* Register event for slider change: */
        $('#sliderInput').on('change input', function () {
            $('#sliderForm').submit();
        })
    }, false);
})();