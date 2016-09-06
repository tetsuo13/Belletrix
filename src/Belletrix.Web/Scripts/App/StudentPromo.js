/// <reference path="..\typings\jquery\jquery.d.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Belletrix;
(function (Belletrix) {
    /**
     * User portal side of Promo.
     */
    var StudentPromo = (function (_super) {
        __extends(StudentPromo, _super);
        function StudentPromo() {
            _super.apply(this, arguments);
        }
        StudentPromo.prototype.initForm = function (nameCheckUrl) {
            var _this = this;
            _super.prototype.initForm.call(this, nameCheckUrl);
            $("a#studyAbroadDestinations").click(function (e) {
                e.preventDefault();
                _this.addStudyAbroadRows();
            });
        };
        /**
         * Add a year and semester row group.
         */
        StudentPromo.prototype.addStudyAbroadRows = function () {
            var rowId = Belletrix.Common.randomString();
            var enclosure;
            var anchor;
            var paragraph = $('<p class="help-block text-right"></p>');
            $.each(["StudyAbroadYear", "StudyAbroadPeriod"], function (i, fieldName) {
                var firstSelect = $("select#" + fieldName);
                var newSelect = firstSelect.clone().removeAttr("id");
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
                $("." + rowId).remove();
            });
            paragraph.append(anchor);
            enclosure
                .append(paragraph)
                .appendTo($("div#studyAbroadRowRemovals").parent());
        };
        return StudentPromo;
    }(Belletrix.StudentBase));
    Belletrix.StudentPromo = StudentPromo;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=StudentPromo.js.map