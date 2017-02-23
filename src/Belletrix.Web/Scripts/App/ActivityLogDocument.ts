/// <reference path="Common.ts" />
/// <reference path="..\typings\bootbox\index.d.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

namespace Belletrix {
    export class ActivityLogDocument {
        private readonly documentListUrl: string;
        private readonly documentBlockSelector: string;
        private readonly activityLogId: number;
        private readonly deleteDocumentUrl: string;

        public constructor(documentListUrl: string, documentListSelector: string, activityLogId: number,
            deleteDocumentUrl: string) {
            this.documentListUrl = documentListUrl;
            this.documentBlockSelector = documentListSelector;
            this.activityLogId = activityLogId;
            this.deleteDocumentUrl = deleteDocumentUrl;
        }

        /**
         * Replace the document block with the result from executing a partial
         * view.
         */
        public refreshList(): void {
            const self = this;

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
                $("button.document-list-delete").click(function (event: JQueryEventObject): void {
                    const documentId: string = $(this).data("document-public-id");

                    bootbox.confirm({
                        size: "small",
                        message: "Are you sure?",
                        callback: function (result: boolean): void {
                            if (result) {
                                Belletrix.Common.handleDeleteCall(self.deleteDocumentUrl, documentId, function (): void {
                                    self.refreshList();
                                });
                            }
                        }
                    });
                });
            });
        }
    }
}
