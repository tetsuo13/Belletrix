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
         */
        User.prototype.initUserList = function (tableSelector, deleteModalSelector, deleteUrl) {
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
            this.handleUserDelete(deleteModalSelector, deleteUrl);
        };
        User.prototype.handleUserDelete = function (deleteModalSelector, deleteUrl) {
            var deleteDialog = $(deleteModalSelector);
            var confirmDeleteSelector = ".btn-danger";
            deleteDialog.on("show.bs.modal", function (event) {
                var button = $(event.relatedTarget);
                var promoId = button.data("userid");
                $(confirmDeleteSelector, deleteDialog).click(function () {
                    $(this).addClass("disabled");
                    $.ajax({
                        method: "DELETE",
                        url: deleteUrl,
                        data: {
                            id: promoId
                        },
                        success: function (data) {
                            console.log(data);
                            if (!data.Result) {
                                deleteDialog.modal("hide");
                                Belletrix.Common.errorMessage("Something went wrong: " + data.Message);
                                return;
                            }
                            window.location.reload();
                        }
                    });
                });
            });
            deleteDialog.on("hide.bs.modal", function (event) {
                $(confirmDeleteSelector, deleteDialog).off();
            });
        };
        ;
        return User;
    }());
    Belletrix.User = User;
})(Belletrix || (Belletrix = {}));
