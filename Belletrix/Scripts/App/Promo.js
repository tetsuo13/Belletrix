/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\jquery.dataTables\jquery.dataTables.d.ts" />
'use strict';
var Belletrix;
(function (Belletrix) {
    var Promo = (function () {
        function Promo() {
        }
        /**
        * Pass name along to ajax request to check for uniqueness.
        *
        * Used on the promo add view to prevent creating a duplicate promo.
        *
        * @param uniqueNameCheckUrl URL for the name check ajax function.
        * @param nameSelector Selector for name input.
        * @param resultImageSelector Selector for inline image for results.
        */
        Promo.prototype.checkNameForUniqueness = function (uniqueNameCheckUrl, nameSelector, resultImageSelector) {
            var feedbackImage = $(resultImageSelector);
            feedbackImage.removeClass('fa-check').show().addClass('fa-spinner fa-spin');
            $.ajax({
                url: uniqueNameCheckUrl,
                data: {
                    name: $(nameSelector).val()
                },
                success: function (data) {
                    var submitButton = $('button[type="submit"]');
                    switch (data) {
                        case 'win':
                            submitButton.removeClass('disabled');
                            feedbackImage.hide();
                            break;
                        default:
                            if (!submitButton.hasClass('disabled')) {
                                submitButton.addClass('disabled');
                            }
                            feedbackImage.removeClass('fa-check fa-spinner fa-spin').addClass('fa-times');
                            break;
                    }
                },
                type: 'POST'
            });
        };
        /**
        * Student entry from promo form.
        *
        * @param pingerUrl
        */
        Promo.prototype.initForm = function (pingerUrl) {
            var _this = this;
            Belletrix.Common.initPinger(pingerUrl);
            Belletrix.Common.initMultiselect(0, 300);
            Belletrix.Common.handleMvcEditor();
            $('a#studyAbroadDestinations').click(function (e) {
                e.preventDefault();
                _this.addStudyAbroadRows();
            });
        };
        /**
        * Add a year and semester row group.
        */
        Promo.prototype.addStudyAbroadRows = function () {
            var rowId = Belletrix.Common.randomString(), enclosure, anchor, paragraph = $('<p class="help-block text-right"></p>');
            $.each(['StudyAbroadYear', 'StudyAbroadPeriod'], function (i, fieldName) {
                var firstSelect = $('select#' + fieldName), newSelect = firstSelect.clone().removeAttr('id');
                $('<div class="form-group reducePadding" />').addClass(rowId).append(newSelect).appendTo(firstSelect.parent().parent());
            });
            enclosure = $('<div class="form-group reducePadding" style="margin-top:-20px">&nbsp;</div>').addClass(rowId);
            anchor = $('<a href="" title="Remove destination row"></a>').html('<i class="fa fa-minus-circle"></i> Del').click(function (e) {
                e.preventDefault();
                $('.' + rowId).remove();
            });
            paragraph.append(anchor);
            enclosure.append(paragraph).appendTo($('div#studyAbroadRowRemovals').parent());
        };
        /**
        * Initialize the student list page for a promo.
        */
        Promo.prototype.initStudentList = function () {
            $('#studentlist').DataTable({
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });
        };
        /**
        * Promotions initialization.
        *
        * @param uniqueNameCheckUrl URL for the name check ajax function.
        * @param nameSelector Selector for name input.
        * @param resultImageSelector Selector for inline image for results.
        */
        Promo.prototype.init = function (uniqueNameCheckUrl, nameSelector, resultImageSelector) {
            var _this = this;
            $(resultImageSelector).hide();
            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event would
            // trigger many times before the user would be finished.
            $(nameSelector).data('timeout', null).keyup(function () {
                var element = $(_this);
                clearTimeout(element.data('timeout'));
                element.data('timeout', setTimeout(function () { return _this.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector); }, 500));
            });
        };
        return Promo;
    })();
    Belletrix.Promo = Promo;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=Promo.js.map