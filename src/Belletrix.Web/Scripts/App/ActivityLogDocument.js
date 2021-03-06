/// <reference path="Common.ts" />
/// <reference path="..\typings\bootbox\index.d.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
var Belletrix;
(function (Belletrix) {
    var ActivityLogDocument = (function () {
        function ActivityLogDocument(documentListUrl, documentListSelector, activityLogId, deleteDocumentUrl) {
            this.documentListUrl = documentListUrl;
            this.documentBlockSelector = documentListSelector;
            this.activityLogId = activityLogId;
            this.deleteDocumentUrl = deleteDocumentUrl;
        }
        /**
         * Replace the document block with the result from executing a partial
         * view.
         */
        ActivityLogDocument.prototype.refreshList = function () {
            var self = this;
            $(this.documentBlockSelector).load(this.documentListUrl, {
                id: this.activityLogId
            }, function () {
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
                $("button.document-list-delete").click(function (event) {
                    var documentId = $(this).data("document-public-id");
                    bootbox.confirm({
                        size: "small",
                        message: "Are you sure?",
                        callback: function (result) {
                            if (result) {
                                Belletrix.Common.handleDeleteCall(self.deleteDocumentUrl, documentId, function () {
                                    self.refreshList();
                                });
                            }
                        }
                    });
                });
            });
        };
        return ActivityLogDocument;
    }());
    Belletrix.ActivityLogDocument = ActivityLogDocument;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=ActivityLogDocument.js.map