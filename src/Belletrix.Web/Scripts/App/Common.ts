﻿/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\bootstrap\bootstrap.d.ts" />
/// <reference path="..\typings\bootstrap.multiselect\bootstrap.multiselect.d.ts" />

module Belletrix {
    /**
     * Generic utility methods used throughout application.
     */
    export class Common {
        /** Number of milliseconds between pinging the server. */
        private static idleKillerInterval: number = 1000 * 60 * 10;

        public static UserFirstName: string;
        public static UserLastName: string;

        /**
         * Disable the form submit button after clicking it.
         */
        public static singleSubmit(): void {
            $("form").submit(function (): void {
                // We're going to disable all submit buttons unless otherwise
                // told to.
                let disableButtons: boolean = true;

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
        }

        /**
         * Show an error message as a modal dialog.
         * @param message Error message.
         */
        public static errorMessage(message: string): void {
            const setup: string = `
<div class="modal fade">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">
                    <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                <h4 class="modal-title">Error</h4>
            </div>
            <div class="modal-body">
                ${ message }
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>`;

            $(setup).modal();
        }

        /**
         * Have the server process something often. This is to avoid the
         * application pool from shutting down after a period of inactivity.
         * @param pingUrl
         */
        public static initPinger(pingUrl: string): void {
            setInterval(function (): void {
                $.ajax({
                    url: pingUrl,
                    cache: false,
                    type: "GET"
                });
            }, this.idleKillerInterval);
        }

        /**
         * Initialize the Bootstrap Multiselect plugin.
         * @param numberToDisplay Number of selected elements to summarize.
         * @param maxHeight Maximum height in pixels.
         */
        public static initMultiselect(numberToDisplay: number, maxHeight?: number): void {
            let options: any = {
                numberDisplayed: numberToDisplay,
                buttonContainer: '<div class="button-default" />',
                maxHeight: undefined
            };

            if (maxHeight) {
                options.maxHeight = maxHeight;
            }

            $(".multiselect").multiselect(options);
        }

        /**
         * Adds the "form-control" Bootstrap class to any input types that
         * were generated with the MVC EditorFor() HTML helper.
         */
        public static handleMvcEditor(): void {
            $("input[type=email]").addClass("form-control");
        }

        /**
         * Generate a random string.
         */
        public static randomString(): string {
            return (Math.random() + 1).toString(36).slice(2);
        };

        /**
         * Make generic ajax request to delete a resource.
         * @param deleteUrl URL to call.
         * @param deleteId Resource ID to delete.
         * @param successCallback Function to call after a successful ajax request.
         */
        public static handleDeleteCall(deleteUrl: string, deleteId: number, successCallback: Function): void {
            $.ajax({
                method: "DELETE",
                url: deleteUrl,
                data: {
                    id: deleteId
                },
                success: function (data: GenericResult): void {
                    if (!data.Result) {
                        Belletrix.Common.errorMessage("Something went wrong: " + data.Message);
                        return;
                    }

                    successCallback();
                }
            });
        }
    }
}
