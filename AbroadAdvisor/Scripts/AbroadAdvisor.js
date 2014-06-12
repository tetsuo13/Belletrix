window.Bennett = window.Bennett || {};

(function (AbroadAdvisor, $) {
    'use strict';

    /// <var type="Number">Number of milliseconds between pinging the server</var>
    var idleKillerInterval = 1000 * 60 * 10;

    AbroadAdvisor.initPinger = function (pingUrl) {
        /// <summary>
        /// Have the server process something often. This is to avoid the
        /// application pool from shutting down after a period of inactivity.
        /// </summary>
        /// <param name="pingUrl" type="String"></param>

        setInterval(function () {
            $.ajax({
                url: pingUrl,
                cache: false,
                type: 'GET'
            });
        }, idleKillerInterval);
    };

    AbroadAdvisor.initMultiselect = function (numberToDisplay, maxHeight) {
        /// <summary>Initialize the Bootstrap Multiselect plugin.</summary>
        /// <param name="numberToDisplay" type="Number">
        /// Number of selected elements to summarize.
        /// </param>
        /// <param name="maxHeight" type="Number" elementMayBeNull="true">
        /// Maximum height in pixels.
        /// </param>

        var options = {
            numberDisplayed: numberToDisplay,
            buttonContainer: '<div class="button-default" />'
        }

        if (typeof maxHeight !== 'undefined') {
            options.maxHeight = maxHeight;
        }

        $('.multiselect').multiselect(options);
    };

    AbroadAdvisor.handleMvcEditor = function () {
        /// <summary>
        /// Adds the "form-control" Bootstrap class to any input types that
        /// were generated with the MVC EditorFor() HTML helper.
        /// </summary>

        $('input[type=date], input[type=email]').addClass('form-control');
    };

    AbroadAdvisor.randomString = function () {
        /// <summary>Generate a random string.</summary>
        /// <returns type="String">A random string.</summary>

        return (Math.random() + 1).toString(36).slice(2);
    };
})(window.Bennett.AbroadAdvisor = window.Bennett.AbroadAdvisor || {}, jQuery, undefined);
