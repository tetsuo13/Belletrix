/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    export class User {
        /**
         * Initialize the user list page.
         * @param tableSelector Main user listing table.
         * @param deleteModalSelector User delete confirm dialog.
         * @param deleteUrl URL to call for user deletion.
         */
        public initUserList(tableSelector: string, deleteModalSelector: string,
            deleteUrl: string): void {

            $(tableSelector).DataTable({
                columns: [
                    null,
                    null,
                    null,
                    null,
                    { orderable: false },
                    { orderable: false }
                ]
            });

            this.handleUserDelete(deleteModalSelector, deleteUrl);
        }

        private handleUserDelete(deleteModalSelector: string, deleteUrl: string): void {
            let deleteDialog: JQuery = $(deleteModalSelector);
            let confirmDeleteSelector: string = ".btn-danger";

            deleteDialog.on("show.bs.modal", function (event: JQueryEventObject): void {
                let button: JQuery = $(event.relatedTarget);
                let promoId: number = button.data("userid");

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
