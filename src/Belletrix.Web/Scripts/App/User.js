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
        User.prototype.initUserList = function (tableSelector, deleteUrl, dataString) {
            $("button.user-list-delete").click(function (event) {
                var userId = parseInt($(this).data(dataString));
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result) {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, userId, function () {
                                window.location.reload();
                            });
                        }
                    }
                });
            });
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
        };
        return User;
    }());
    Belletrix.User = User;
})(Belletrix || (Belletrix = {}));
