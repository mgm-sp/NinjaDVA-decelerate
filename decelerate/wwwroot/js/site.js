(function () {
    window.addEventListener('load', function () {
        /* Register event for slider change: */
        $('#sliderInput').change(function () {
            console.log($(this).val());
        });
    }, false);
})();