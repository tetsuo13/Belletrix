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
        public initUserList(tableSelector: string, deleteModalSelector: string,
            deleteUrl: string, dataString: string): void {

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

            Belletrix.Common.handleDeleteModal(deleteModalSelector, deleteUrl, dataString);
        }
    }
}
