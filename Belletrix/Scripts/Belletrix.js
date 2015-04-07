window.Bennett = window.Bennett || {};

(function (Belletrix, $) {
    'use strict';

    /// <var type="Number">Number of milliseconds between pinging the server</var>
    var idleKillerInterval = 1000 * 60 * 10;

    Belletrix.singleSubmit = function () {
        /// <summary>Disable the form submit button after clicking it.</summary>

        $('form').submit(function () {
            // We're doing to disable all submit buttons unless otherwise
            // told to.
            var disableButtons = true;

            // If the jQuery Validate plugin has been activated for this form,
            // only disable the submit buttons if the form is valid. Otherwise
            // leave the buttons enabled and allow the user to complete their
            // corrections.
            if (typeof $(this).valid === 'function' && !$(this).valid()) {
                disableButtons = false;
            }

            if (disableButtons) {
                $(this).find('button[type="submit"]').prop('disabled', true);
            }
        });
    };

    Belletrix.errorMessage = function (message) {
        /// <summary>Show an error message as a modal dialog.</summary>
        /// <param name="message" type="String">Error message.</param>

        var setup =
            '<div class="modal fade">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                        '<div class="modal-header">' +
                            '<button type="button" class="close" data-dismiss="modal">' +
                                '<span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
                            '<h4 class="modal-title">Error</h4>' +
                        '</div>' +
                        '<div class="modal-body">' +
                            message +
                        '</div>' +
                        '<div class="modal-footer">' +
                            '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>';

        $(setup).modal();
    };

    Belletrix.initPinger = function (pingUrl) {
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

    Belletrix.initMultiselect = function (numberToDisplay, maxHeight) {
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

    Belletrix.handleMvcEditor = function () {
        /// <summary>
        /// Adds the "form-control" Bootstrap class to any input types that
        /// were generated with the MVC EditorFor() HTML helper.
        /// </summary>

        $('input[type=date], input[type=email]').addClass('form-control');
    };

    Belletrix.randomString = function () {
        /// <summary>Generate a random string.</summary>
        /// <returns type="String">A random string.</summary>

        return (Math.random() + 1).toString(36).slice(2);
    };
})(window.Belletrix = window.Belletrix || {}, jQuery, undefined);
