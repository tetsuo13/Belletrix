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
         * @param deleteUrl URL to call for promo deletion.
         * @param dataString
         */
        Promo.prototype.initPromoList = function (tableSelector, deleteUrl, dataString) {
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
            $("button.promo-list-delete").click(function (event) {
                var promoId = parseInt($(this).data(dataString));
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result) {
                        if (!result) {
                            return;
                        }
                        Belletrix.Common.handleDeleteCall(deleteUrl, promoId, function () {
                            window.location.reload();
                        });
                    }
                });
            });
        };
        return Promo;
    }());
    Belletrix.Promo = Promo;
})(Belletrix || (Belletrix = {}));
