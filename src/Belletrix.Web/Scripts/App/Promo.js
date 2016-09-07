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
         * Initialize the student list page for a promo.
         */
        Promo.prototype.initStudentList = function () {
            $("#studentlist").DataTable({
                columnDefs: [{
                        targets: -1,
                        orderable: false
                    }]
            });
        };
        ;
        return Promo;
    }());
    Belletrix.Promo = Promo;
})(Belletrix || (Belletrix = {}));
