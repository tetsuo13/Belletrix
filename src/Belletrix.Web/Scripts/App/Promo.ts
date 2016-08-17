/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

'use strict';

module Belletrix {
    export class Promo {
        /**
         * Promotions initialization.
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        constructor(uniqueNameCheckUrl: string, nameSelector: string, resultImageSelector: string) {
            let self = this;

            $(resultImageSelector).hide();

            // Add a delay when checking the name to give the user a chance to
            // complete their typing. Otherwise, a straight "onkeyup" event would
            // trigger many times before the user would be finished.
            $(nameSelector)
                .data("timeout", null)
                .keyup(function (event: JQueryEventObject): void {
                    let element: JQuery = $(this);

                    clearTimeout(element.data("timeout"));

                    element.data("timeout", setTimeout(function (): void {
                        self.checkNameForUniqueness(uniqueNameCheckUrl, nameSelector, resultImageSelector);
                    }, 500));
                });
        }

        /**
         * Pass name along to ajax request to check for uniqueness.
         *
         * Used on the promo add view to prevent creating a duplicate promo.
         * @param uniqueNameCheckUrl URL for the name check ajax function.
         * @param nameSelector Selector for name input.
         * @param resultImageSelector Selector for inline image for results.
         */
        private checkNameForUniqueness(uniqueNameCheckUrl: string, nameSelector: string, resultImageSelector: string): void {
            let feedbackImage: JQuery = $(resultImageSelector);

            feedbackImage.removeClass("glyphicon-ok").show().addClass("glyphicon-refresh");

            $.ajax({
                url: uniqueNameCheckUrl,
                data: { name: $(nameSelector).val() },
                success: function (data: string): void {
                    var submitButton = $('button[type="submit"]');

                    switch (data) {
                        case "win":
                            submitButton.removeClass("disabled");
                            feedbackImage.hide();
                            break;

                        default:
                            if (!submitButton.hasClass("disabled")) {
                                submitButton.addClass("disabled");
                            }
                            feedbackImage.removeClass("glyphicon-ok glyphicon-refresh").addClass("glyphicon-remove");
                            break;
                    }
                },
                type: "POST"
            });
        }

        /**
         * Student entry from promo form.
         * @param pingerUrl
         */
        initForm(pingerUrl: string): void {
            Belletrix.Common.initPinger(pingerUrl);
            Belletrix.Common.initMultiselect(0, 300);
            Belletrix.Common.handleMvcEditor();
            $("#DateOfBirth").datepicker();

            $("a#studyAbroadDestinations").click((e): void => {
                e.preventDefault();
                this.addStudyAbroadRows();
            })
        };

        /**
         * Add a year and semester row group.
         */
        public addStudyAbroadRows(): void {
            let rowId: string = Belletrix.Common.randomString();
            let enclosure: JQuery;
            let anchor: JQuery;
            let paragraph: JQuery = $('<p class="help-block text-right"></p>');

            $.each(["StudyAbroadYear", "StudyAbroadPeriod"], function (i: number, fieldName: string): void {
                let firstSelect: JQuery = $("select#" + fieldName);
                let newSelect: JQuery = firstSelect.clone().removeAttr("id");

                $('<div class="form-group reducePadding" />')
                    .addClass(rowId)
                    .append(newSelect)
                    .appendTo(firstSelect.parent().parent());
            });

            enclosure = $('<div class="form-group reducePadding" style="margin-top:-20px">&nbsp;</div>')
                .addClass(rowId);

            anchor = $('<a href="" title="Remove destination row"></a>')
                .html('<i class="glyphicon glyphicon-minus-sign"></i> Del')
                .click(function (e: JQueryEventObject) {
                    e.preventDefault();
                    $("." + rowId).remove();
                });

            paragraph.append(anchor);

            enclosure
                .append(paragraph)
                .appendTo($("div#studyAbroadRowRemovals").parent());
        };

        /**
         * Initialize the student list page for a promo.
         */
        public initStudentList(): void {
            $("#studentlist").DataTable({
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });
        };
    }
}
