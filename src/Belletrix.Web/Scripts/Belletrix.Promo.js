window.Belletrix = window.Belletrix || {};

(function (Promo, $, undefined) {
    'use strict';

    function checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector) {
        /// <summary>
        /// Pass name along to ajax request to check for uniqueness.
        /// 
        /// Used on the promo add view to prevent creating a duplicate promo.
        /// </summary>
        /// <param name="uniqueNameCheckUrl" type="String">URL for the name check ajax function.</param>
        /// <param name="nameSelector" type="String">Selector for name input.</param>
        /// <param name="resultImageSelector" type="String">Selector for inline image for results.</param>

        var feedbackImage = $(resultImageSelector);

        feedbackImage.removeClass('glyphicon-ok').show().addClass('glyphicon-refresh');

        $.ajax({
            url: uniqueNameCheckUrl,
            data: { name: $(nameSelector).val() },
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
                        feedbackImage.removeClass('glyphicon-ok glyphicon-refresh').addClass('glyphicon-remove');
                        break;
                }
            },
            type: 'POST'
        });
    }

    Promo.initForm = function (pingerUrl) {
        /// <summary>Student entry from promo form.</summary>

        Belletrix.initPinger(pingerUrl);
        Belletrix.initMultiselect(0, 300);
        Belletrix.handleMvcEditor();
        $('#DateOfBirth').datepicker();

        $('a#studyAbroadDestinations').click(function (e) {
            e.preventDefault();
            Promo.addStudyAbroadRows();
        })
    };

    Promo.addStudyAbroadRows = function () {
        /// <summary>Add a year and semester row group.</summary>

        var rowId = Belletrix.randomString(),
            enclosure,
            anchor,
            paragraph = $('<p class="help-block text-right"></p>');

        $.each(['StudyAbroadYear', 'StudyAbroadPeriod'], function (i, fieldName) {
            var firstSelect = $('select#' + fieldName),
                newSelect = firstSelect.clone().removeAttr('id');

            $('<div class="form-group reducePadding" />')
                .addClass(rowId)
                .append(newSelect)
                .appendTo(firstSelect.parent().parent());
        });

        enclosure = $('<div class="form-group reducePadding" style="margin-top:-20px">&nbsp;</div>')
            .addClass(rowId);

        anchor = $('<a href="" title="Remove destination row"></a>')
            .html('<i class="glyphicon glyphicon-minus-sign"></i> Del')
            .click(function (e) {
                e.preventDefault();
                $('.' + rowId).remove();
            });

        paragraph.append(anchor);

        enclosure
            .append(paragraph)
            .appendTo($('div#studyAbroadRowRemovals').parent());
    };

    Promo.initStudentList = function () {
        /// <summary>Initialize the student list page for a promo.</summary>

        $('#studentlist').dataTable({
            columnDefs: [{
                targets: -1,
                orderable: false
            }]
        });
    };

    Promo.init = function (uniqueNameCheckUrl, nameSelector, resultImageSelector) {
        /// <summary>Promotions initialization.</summary>
        /// <param name="uniqueNameCheckUrl" type="String">URL for the name check ajax function.</param>
        /// <param name="nameSelector" type="String">Selector for name input.</param>
        /// <param name="resultImageSelector" type="String">Selector for inline image for results.</param>

        $(resultImageSelector).hide();

        // Add a delay when checking the name to give the user a chance to
        // complete their typing. Otherwise, a straight "onkeyup" event would
        // trigger many times before the user would be finished.
        $(nameSelector)
            .data('timeout', null)
            .keyup(function (event) {
                var element = $(this);

                clearTimeout(element.data('timeout'));

                element.data('timeout', setTimeout(function () {
                    checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector);
                }, 500));
            });
    };
})(window.Belletrix.Promo = window.Belletrix.Promo || {}, jQuery);
