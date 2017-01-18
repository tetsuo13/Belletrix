/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    export class ActivityLogDocument {
        private documentListUrl: string;
        private documentBlockSelector: string;
        private activityLogId: number;

        public constructor(documentListUrl: string, documentListSelector: string, activityLogId: number) {
            this.documentListUrl = documentListUrl;
            this.documentBlockSelector = documentListSelector;
            this.activityLogId = activityLogId;
        }

        /**
         * Replace the document block with the result from executing a partial
         * view.
         */
        public refreshList(): void {
            $(this.documentBlockSelector).load(this.documentListUrl, {
                id: this.activityLogId
            }, function (): void {
                // Initialize DataTables using the fewest possible options in
                // order to not enlargen the table.
                $("#documents-table").DataTable({
                    columns: [
                        undefined,
                        undefined,
                        { orderable: false }
                    ],
                    paging: false,
                    searching: false,
                    info: false
                });

                // Bind the delete buttons.
            });
        }
    }
}
