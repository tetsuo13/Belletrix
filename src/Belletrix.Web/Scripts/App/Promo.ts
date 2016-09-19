/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    class GenericResult {
        public Result: boolean;
        public Message: string;
    }

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
        public initAddForm(uniqueNameCheckUrl: string, nameSelector: string, resultImageSelector: string): void {
            let self = this;

            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event
            // would trigger many times before the user would be finished.
            $(nameSelector)
                .data("timeout", null)
                .keyup(function (event: JQueryEventObject): void {
                    let element: JQuery = $(this);

                    clearTimeout(element.data("timeout"));

                    element.data("timeout", setTimeout(function (): void {
                        self.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector);
                    }, 500));
                });

            $('[data-toggle="tooltip"]').tooltip();
        }

        /**
         * Pass name along to ajax request to check for uniqueness.
         *
         * Used on the promo add view to prevent creating a duplicate promo.
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        private checkNameForUniqueness(uniqueNameCheckUrl: string, nameSelector: string,
            resultImageSelector: string): void {

            let feedbackImage: JQuery = $(resultImageSelector);

            feedbackImage.removeClass("glyphicon-remove").addClass("glyphicon-refresh");

            $.ajax({
                url: uniqueNameCheckUrl,
                type: "POST",
                data: { name: $(nameSelector).val() },
                success: function (data: string): void {
                    let submitButton: JQuery = $('button[type="submit"]');

                    feedbackImage.removeClass("glyphicon-ok glyphicon-refresh");

                    switch (data) {
                        case "win":
                            submitButton.removeClass("disabled");
                            feedbackImage.addClass("glyphicon-ok");
                            break;

                        default:
                            if (!submitButton.hasClass("disabled")) {
                                submitButton.addClass("disabled");
                            }
                            feedbackImage.addClass("glyphicon-remove");
                            break;
                    }
                }
            });
        }

        /**
         * Initialize the promo list page.
         * @param tableSelector Main promo listing table.
         * @param deleteModalSelector Promo delete confirm dialog.
         */
        public initPromoList(tableSelector: string, deleteModalSelector: string,
            deleteUrl: string): void {
            $(tableSelector).DataTable({
                columns: [
                    null,
                    null,
                    null,
                    null,
                    null,
                    { orderable: false }
                ]
            });

            $('[data-toggle="tooltip"]').tooltip();
            this.handlePromoDelete(deleteModalSelector, deleteUrl);
        }

        private handlePromoDelete(deleteModalSelector: string, deleteUrl: string): void {
            let deleteDialog: JQuery = $(deleteModalSelector);
            let confirmDeleteSelector: string = ".btn-danger";

            deleteDialog.on("show.bs.modal", function (event: JQueryEventObject): void {
                let button: JQuery = $(event.relatedTarget);
                let promoId: number = button.data("promoid");

                $(confirmDeleteSelector, deleteDialog).click(function (): void {
                    $(this).addClass("disabled");

                    $.ajax({
                        method: "DELETE",
                        url: deleteUrl,
                        data: {
                            id: promoId
                        },
                        success: function (data: GenericResult): void {
                            console.log(data);
                            if (!data.Result) {
                                deleteDialog.modal("hide");
                                Belletrix.Common.errorMessage("Something went wrong: " + data.Message);
                                return;
                            }

                            window.location.reload();
                        }
                    });
                });
            });

            deleteDialog.on("hide.bs.modal", function (event: JQueryEventObject): void {
                $(confirmDeleteSelector, deleteDialog).off();
            });
        };
    }
}
