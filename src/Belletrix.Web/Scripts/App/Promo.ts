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
         */
        public initAddForm(): void {
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
