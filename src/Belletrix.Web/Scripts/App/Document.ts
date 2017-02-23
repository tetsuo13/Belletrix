/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\jquery.fileupload\jquery.fileupload.d.ts" />

namespace Belletrix {
    export class Document {
        /**
         * Activity Log view page. Allows user to add a new document to the
         * Activity Log they're viewing.
         * @param addNewDocumentUrl URL to upload document to.
         * @param activityLogId The ID of the Activity Log to attach to.
         * @param buttonSelector Selector for the "Add" button.
         */
        public initActivityLog(addNewDocumentUrl: string, activityLogId: number,
            buttonSelector: string, successFunction: Function): void {
            $(buttonSelector).fileupload({
                url: addNewDocumentUrl,
                dataType: "json",
                paramName: "File",
                formData: [{
                    name: "ActivityLogId",
                    value: activityLogId
                }],
                done: function (e: JQueryEventObject, data: JQueryFileUploadDone): void {
                    console.log(e);
                    console.log(data);

                    successFunction();
                }
            });
        }
    }
}
