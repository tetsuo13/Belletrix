/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
var Belletrix;
(function (Belletrix) {
    var GenericResult = (function () {
        function GenericResult() {
        }
        return GenericResult;
    }());
    /**
     * Promo management only available to internal users.
     */
    var Promo = (function () {
        function Promo() {
        }
        /**
         * Promotions initialization.
         *
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        Promo.prototype.initAddForm = function (uniqueNameCheckUrl, nameSelector, resultImageSelector) {
            var self = this;
            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event
            // would trigger many times before the user would be finished.
            $(nameSelector)
                .data("timeout", null)
                .keyup(function (event) {
                var element = $(this);
                clearTimeout(element.data("timeout"));
                element.data("timeout", setTimeout(function () {
                    self.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector);
                }, 500));
            });
            $('[data-toggle="tooltip"]').tooltip();
        };
        /**
         * Pass name along to ajax request to check for uniqueness.
         *
         * Used on the promo add view to prevent creating a duplicate promo.
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        Promo.prototype.checkNameForUniqueness = function (uniqueNameCheckUrl, nameSelector, resultImageSelector) {
            var feedbackImage = $(resultImageSelector);
            feedbackImage.removeClass("glyphicon-remove").addClass("glyphicon-refresh");
            $.ajax({
                url: uniqueNameCheckUrl,
                type: "POST",
                data: { name: $(nameSelector).val() },
                success: function (data) {
                    var submitButton = $('button[type="submit"]');
                    feedbackImage.removeClass("glyphicon-ok glyphicon-refresh");
                    switch (data) {
                        case "win":
                            submitButton.removeClass("disabled");
                            feedbackImage.addClass("glyphicon-ok");
                            break;
                        default:
                            if (!submitButton.hasClass("disabled")) {
                                submitButton.addClass("disabled");
                            }
                            feedbackImage.addClass("glyphicon-remove");
                            break;
                    }
                }
            });
        };
        /**
         * Initialize the promo list page.
         * @param tableSelector Main promo listing table.
         * @param deleteModalSelector Promo delete confirm dialog.
         */
        Promo.prototype.initPromoList = function (tableSelector, deleteModalSelector, deleteUrl) {
            $(tableSelector).DataTable({
                columns: [
                    null,
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
