/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\jquery.fileupload\jquery.fileupload.d.ts" />
var Belletrix;
(function (Belletrix) {
    var Document = (function () {
        function Document() {
        }
        /**
         * Activity Log view page. Allows user to add a new document to the
         * Activity Log they're viewing.
         * @param addNewDocumentUrl URL to upload document to.
         * @param activityLogId The ID of the Activity Log to attach to.
         * @param buttonSelector Selector for the "Add" button.
         */
        Document.prototype.initActivityLog = function (addNewDocumentUrl, activityLogId, buttonSelector, successFunction) {
            $(buttonSelector).fileupload({
                url: addNewDocumentUrl,
                dataType: "json",
                paramName: "File",
                formData: [{
                        name: "ActivityLogId",
                        value: activityLogId
                    }],
                done: function (e, data) {
                    console.log(e);
                    console.log(data);
                    successFunction();
                }
            });
        };
        return Document;
    }());
    Belletrix.Document = Document;
})(Belletrix || (Belletrix = {}));
