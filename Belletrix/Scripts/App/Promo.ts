/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\jquery.dataTables\jquery.dataTables.d.ts" />

'use strict';

module Belletrix {
    export class Promo {
        /**
        * Pass name along to ajax request to check for uniqueness.
        *
        * Used on the promo add view to prevent creating a duplicate promo.
        *
        * @param uniqueNameCheckUrl URL for the name check ajax function.
        * @param nameSelector Selector for name input.
        * @param resultImageSelector Selector for inline image for results.
        */
        private checkNameForUniqueness(uniqueNameCheckUrl: string, nameSelector: string,
            resultImageSelector: string): void {

            var feedbackImage: JQuery = $(resultImageSelector);

            feedbackImage.removeClass('fa-check').show().addClass('fa-spinner fa-spin');

            $.ajax({
                url: uniqueNameCheckUrl,
                data: {
                    name: $(nameSelector).val()
                },
                success: function (data: string) {
                    var submitButton: JQuery = $('button[type="submit"]');

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
        }

        /**
        * Student entry from promo form.
        *
        * @param pingerUrl
        */
        private initForm(pingerUrl: string): void {
            Common.initPinger(pingerUrl);
            Common.initMultiselect(0, 300);
            Common.handleMvcEditor();

            $('a#studyAbroadDestinations').click((e: JQueryEventObject) => {
                e.preventDefault();
                this.addStudyAbroadRows();
            })
        }

        /**
        * Add a year and semester row group.
        */
        private addStudyAbroadRows(): void {
            var rowId: string = Common.randomString(),
                enclosure: JQuery,
                anchor: JQuery,
                paragraph: JQuery = $('<p class="help-block text-right"></p>');

            $.each(['StudyAbroadYear', 'StudyAbroadPeriod'], function (i, fieldName) {
                var firstSelect: JQuery = $('select#' + fieldName),
                    newSelect: JQuery = firstSelect.clone().removeAttr('id');

                $('<div class="form-group reducePadding" />')
                    .addClass(rowId)
                    .append(newSelect)
                    .appendTo(firstSelect.parent().parent());
            });

            enclosure = $('<div class="form-group reducePadding" style="margin-top:-20px">&nbsp;</div>')
                .addClass(rowId);

            anchor = $('<a href="" title="Remove destination row"></a>')
                .html('<i class="fa fa-minus-circle"></i> Del')
                .click(function (e) {
                e.preventDefault();
                $('.' + rowId).remove();
            });

            paragraph.append(anchor);

            enclosure
                .append(paragraph)
                .appendTo($('div#studyAbroadRowRemovals').parent());
        }

        /**
        * Initialize the student list page for a promo.
        */
        public initStudentList(): void {
            $('#studentlist').DataTable({
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });
        }

        /**
        * Promotions initialization.
        *
        * @param uniqueNameCheckUrl URL for the name check ajax function.
        * @param nameSelector Selector for name input.
        * @param resultImageSelector Selector for inline image for results.
        */
        public init(uniqueNameCheckUrl, nameSelector, resultImageSelector): void {
            $(resultImageSelector).hide();

            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event would
            // trigger many times before the user would be finished.
            $(nameSelector)
                .data('timeout', null)
                .keyup(() => {
                    var element: JQuery = $(this);

                    clearTimeout(element.data('timeout'));

                    element.data('timeout',
                        setTimeout(() => this.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector), 500));
                });
        }
    }
}
