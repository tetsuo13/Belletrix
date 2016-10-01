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
            $("button.promo-list-delete").click(function (event) {
                var promoId = parseInt($(this).data(dataString));
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result) {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, promoId, function () {
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
                    { orderable: false }
                ]
            });
            $('[data-toggle="tooltip"]').tooltip();
        };
        return Promo;
    }());
    Belletrix.Promo = Promo;
})(Belletrix || (Belletrix = {}));
