﻿/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
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
         * @param deleteUrl URL to call for promo deletion.
         * @param dataString
         */
        public initPromoList(tableSelector: string, deleteUrl: string, dataString: string): void {
            $("button.promo-list-delete").click(function (event: JQueryEventObject): void {
                let promoId: number = parseInt($(this).data(dataString));

                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result: boolean): void {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, promoId, function (): void {
                                window.location.reload();
                            });
                        }
                    }
                });
            });

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
        }
    }
}
