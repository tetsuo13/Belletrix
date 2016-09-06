/// <reference path="..\typings\jquery\jquery.d.ts" />

module Belletrix {
    /**
     * User portal side of Promo.
     */
    export class StudentPromo extends StudentBase {
        public initForm(nameCheckUrl: string): void {
            super.initForm(nameCheckUrl);

            $("a#studyAbroadDestinations").click((e): void => {
                e.preventDefault();
                this.addStudyAbroadRows();
            })
        }

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
        }
    }
}
