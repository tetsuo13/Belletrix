/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    /**
     * Promo management only available to internal users.
     */
    export class Promo {
        /**
         * Promotions initialization.
         *
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        constructor(uniqueNameCheckUrl: string, nameSelector: string, resultImageSelector: string) {
            let self = this;

            $(resultImageSelector).hide();

            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event would
            // trigger many times before the user would be finished.
            $(nameSelector)
                .data("timeout", null)
                .keyup(function (event: JQueryEventObject): void {
                    let element: JQuery = $(this);

                    clearTimeout(element.data("timeout"));

                    element.data("timeout", setTimeout(function (): void {
                        self.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector);
                    }, 500));
                });
        }

        /**
         * Pass name along to ajax request to check for uniqueness.
         *
         * Used on the promo add view to prevent creating a duplicate promo.
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        private checkNameForUniqueness(uniqueNameCheckUrl: string, nameSelector: string, resultImageSelector: string): void {
            let feedbackImage: JQuery = $(resultImageSelector);

            feedbackImage.removeClass("glyphicon-ok").show().addClass("glyphicon-refresh");

            $.ajax({
                url: uniqueNameCheckUrl,
                data: { name: $(nameSelector).val() },
                success: function (data: string): void {
                    var submitButton = $('button[type="submit"]');

                    switch (data) {
                        case "win":
                            submitButton.removeClass("disabled");
                            feedbackImage.hide();
                            break;

                        default:
                            if (!submitButton.hasClass("disabled")) {
                                submitButton.addClass("disabled");
                            }
                            feedbackImage.removeClass("glyphicon-ok glyphicon-refresh").addClass("glyphicon-remove");
                            break;
                    }
                },
                type: "POST"
            });
        }

        /**
         * Initialize the student list page for a promo.
         */
        public initStudentList(): void {
            $("#studentlist").DataTable({
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });
        };
    }
}
