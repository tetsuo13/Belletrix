/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\bootstrap\bootstrap.d.ts" />
/// <reference path="..\typings\bootstrap.multiselect\bootstrap.multiselect.d.ts" />
/// <reference path="..\typings\jquery.validation\jquery.validation.d.ts" />
'use strict';
var Belletrix;
(function (Belletrix) {
    /**
    * Generic utility methods used throughout application.
    */
    var Common = (function () {
        function Common() {
        }
        /**
        * Disable the form submit button after clicking it.
        */
        Common.singleSubmit = function () {
            $('form').submit(function () {
                // We're doing to disable all submit buttons unless otherwise
                // told to.
                var disableButtons = true;
                // If the jQuery Validate plugin has been activated for this
                // form, only disable the submit buttons if the form is valid.
                // Otherwise leave the buttons enabled and allow the user to
                // complete their corrections.
                if (typeof $(this).valid === 'function' && !$(this).valid()) {
                    disableButtons = false;
                }
                if (disableButtons) {
                    $(this).find('button[type="submit"]').prop('disabled', true);
                }
            });
        };
        /**
        * Show an error message as a modal dialog.
        *
        * @param message Error message.
        */
        Common.errorMessage = function (message) {
            var setup = '<div class="modal fade">' + '<div class="modal-dialog">' + '<div class="modal-content">' + '<div class="modal-header">' + '<button type="button" class="close" data-dismiss="modal">' + '<span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' + '<h4 class="modal-title">Error</h4>' + '</div>' + '<div class="modal-body">' + message + '</div>' + '<div class="modal-footer">' + '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>' + '</div>' + '</div>' + '</div>' + '</div>';
            $(setup).modal();
        };
        /**
        * Have the server process something often.
        *
        * This is to avoid the application pool from shutting down after a
        * period of inactivity.
        *
        * @param pingUrl
        */
        Common.initPinger = function (pingUrl) {
            setInterval(function () {
                $.ajax({
                    url: pingUrl,
                    cache: false,
                    type: 'GET'
                });
            }, this.idleKillerInterval);
        };
        /**
        * Initialize the Bootstrap Multiselect plugin.
        *
        * @param numberToDisplay Number of selected elements to summarize.
        * @param maxHeight Maximum height in pixels.
        */
        Common.initMultiselect = function (numberToDisplay, maxHeight) {
            var options = {
                numberDisplayed: numberToDisplay,
                buttonContainer: '<div class="button-default" />'
            };
            if (maxHeight) {
                options.maxHeight = maxHeight;
            }
            $('.multiselect').multiselect(options);
        };
        /**
        * Adds the "form-control" Bootstrap class to any input types that were
        * generated with the MVC EditorFor() HTML helper.
        */
        Common.handleMvcEditor = function () {
            $('input[type=date], input[type=email]').addClass('form-control');
        };
        /**
        * Generate a random string.
        *
        * @return A random string.
        */
        Common.randomString = function () {
            return (Math.random() + 1).toString(36).slice(2);
        };
        /** Number of milliseconds between pinging the server. */
        Common.idleKillerInterval = 1000 * 60 * 10;
        return Common;
    })();
    Belletrix.Common = Common;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=Common.js.map