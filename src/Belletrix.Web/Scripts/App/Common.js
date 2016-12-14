/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\bootstrap\bootstrap.d.ts" />
/// <reference path="..\typings\bootstrap.multiselect\bootstrap.multiselect.d.ts" />
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
            $("form").submit(function () {
                // We're going to disable all submit buttons unless otherwise
                // told to.
                var disableButtons = true;
                // If the jQuery Validate plugin has been activated for this
                // form, only disable the submit buttons if the form is valid.
                // Otherwise leave the buttons enabled and allow the user to
                // complete their corrections.
                if (typeof $(this).valid === "function" && !$(this).valid()) {
                    disableButtons = false;
                }
                if (disableButtons) {
                    $(this).find('button[type="submit"]').prop("disabled", true);
                }
            });
        };
        /**
         * Show an error message as a modal dialog.
         * @param message Error message.
         */
        Common.errorMessage = function (message) {
            var setup = "\n<div class=\"modal fade\">\n    <div class=\"modal-dialog\">\n        <div class=\"modal-content\">\n            <div class=\"modal-header\">\n                <button type=\"button\" class=\"close\" data-dismiss=\"modal\">\n                    <span aria-hidden=\"true\">&times;</span><span class=\"sr-only\">Close</span></button>\n                <h4 class=\"modal-title\">Error</h4>\n            </div>\n            <div class=\"modal-body\">\n                " + message + "\n            </div>\n            <div class=\"modal-footer\">\n                <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>\n            </div>\n        </div>\n    </div>\n</div>";
            $(setup).modal();
        };
        /**
         * Have the server process something often. This is to avoid the
         * application pool from shutting down after a period of inactivity.
         * @param pingUrl
         */
        Common.initPinger = function (pingUrl) {
            setInterval(function () {
                $.ajax({
                    url: pingUrl,
                    cache: false,
                    type: "GET"
                });
            }, this.idleKillerInterval);
        };
        /**
         * Initialize the Bootstrap Multiselect plugin.
         * @param numberToDisplay Number of selected elements to summarize.
         * @param maxHeight Maximum height in pixels.
         */
        Common.initMultiselect = function (numberToDisplay, maxHeight) {
            var options = {
                numberDisplayed: numberToDisplay,
                buttonContainer: '<div class="button-default" />',
                maxHeight: undefined
            };
            if (maxHeight) {
                options.maxHeight = maxHeight;
            }
            $(".multiselect").multiselect(options);
        };
        /**
         * Adds the "form-control" Bootstrap class to any input types that
         * were generated with the MVC EditorFor() HTML helper.
         */
        Common.handleMvcEditor = function () {
            $("input[type=email]").addClass("form-control");
        };
        /**
         * Generate a random string.
         */
        Common.randomString = function () {
            return (Math.random() + 1).toString(36).slice(2);
        };
        ;
        /**
         * Make generic ajax request to delete a resource.
         * @param deleteUrl URL to call.
         * @param deleteId Resource ID to delete.
         * @param successCallback Function to call after a successful ajax request.
         */
        Common.handleDeleteCall = function (deleteUrl, deleteId, successCallback) {
            $.ajax({
                method: "DELETE",
                url: deleteUrl,
                data: {
                    id: deleteId
                },
                success: function (data) {
                    if (!data.Result) {
                        Belletrix.Common.errorMessage("Something went wrong: " + data.Message);
                        return;
                    }
                    successCallback();
                }
            });
        };
        /** Number of milliseconds between pinging the server. */
        Common.idleKillerInterval = 1000 * 60 * 10;
        return Common;
    }());
    Belletrix.Common = Common;
})(Belletrix || (Belletrix = {}));
