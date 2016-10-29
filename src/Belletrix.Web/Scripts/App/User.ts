/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    export class User {
        /**
         * Initialize the user list page.
         * @param tableSelector Main user listing table.
         * @param deleteModalSelector User delete confirm dialog.
         * @param deleteUrl URL to call for user deletion.
         * @param dataString Part after the "data-" attribute containing the ID value.
         */
        public initUserList(tableSelector: string, deleteUrl: string, dataString: string): void {
            $("button.user-list-delete").click(function (event: JQueryEventObject): void {
                const userId: number = parseInt($(this).data(dataString));

                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result: boolean): void {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, userId, function (): void {
                                window.location.reload();
                            });
                        }
                    }
                });
            });

            $(tableSelector).DataTable({
                columns: [
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    { orderable: false },
                    { orderable: false }
                ]
            });
        }
    }
}
