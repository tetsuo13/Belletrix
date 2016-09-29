/// <reference path="..\typings\jquery\jquery.d.ts" />
var Belletrix;
(function (Belletrix) {
    var User = (function () {
        function User() {
        }
        /**
         * Initialize the user list page.
         * @param tableSelector Main user listing table.
         * @param deleteModalSelector User delete confirm dialog.
         * @param deleteUrl URL to call for user deletion.
         * @param dataString Part after the "data-" attribute containing the ID value.
         */
        User.prototype.initUserList = function (tableSelector, deleteModalSelector, deleteUrl, dataString) {
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
        };
        return User;
    }());
    Belletrix.User = User;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=User.js.map