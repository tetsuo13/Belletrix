/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
var Belletrix;
(function (Belletrix) {
    /**
     * Promo management only available to internal users.
     */
    var Promo = (function () {
        function Promo() {
        }
        /**
         * Promotions initialization.
         */
        Promo.prototype.initAddForm = function () {
        };
        /**
         * Initialize the promo list page.
         * @param tableSelector Main promo listing table.
         * @param deleteModalSelector Promo delete confirm dialog.
         * @param deleteUrl URL to call for promo deletion.
         */
        Promo.prototype.initPromoList = function (tableSelector, deleteModalSelector, deleteUrl) {
            $(tableSelector).DataTable({
                columns: [
                    null,
                    null,
                    null,
                    null,
                    { orderable: false }
                ]
            });
            $('[data-toggle="tooltip"]').tooltip();
            this.handlePromoDelete(deleteModalSelector, deleteUrl);
        };
        Promo.prototype.handlePromoDelete = function (deleteModalSelector, deleteUrl) {
            var deleteDialog = $(deleteModalSelector);
            var confirmDeleteSelector = ".btn-danger";
            deleteDialog.on("show.bs.modal", function (event) {
                var button = $(event.relatedTarget);
                var promoId = button.data("promoid");
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
        return Promo;
    }());
    Belletrix.Promo = Promo;
})(Belletrix || (Belletrix = {}));
