/// <reference path="..\typings\jquery\jquery.d.ts" />
var Belletrix;
(function (Belletrix) {
    var ActivityLogDocument = (function () {
        function ActivityLogDocument(documentListUrl, documentListSelector, activityLogId) {
            this.documentListUrl = documentListUrl;
            this.documentBlockSelector = documentListSelector;
            this.activityLogId = activityLogId;
        }
        /**
         * Replace the document block with the result from executing a partial
         * view.
         */
        ActivityLogDocument.prototype.refreshList = function () {
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
            });
        };
        return ActivityLogDocument;
    }());
    Belletrix.ActivityLogDocument = ActivityLogDocument;
})(Belletrix || (Belletrix = {}));
