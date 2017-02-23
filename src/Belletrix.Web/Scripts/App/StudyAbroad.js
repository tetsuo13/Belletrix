/// <reference path="..\typings\jquery\jquery.d.ts"/>
/// <reference path="..\typings\bootstrap.datepicker\bootstrap.datepicker.d.ts" />
var Belletrix;
(function (Belletrix) {
    var StudyAbroad = (function () {
        function StudyAbroad() {
        }
        StudyAbroad.prototype.initAddEdit = function () {
            Belletrix.Common.handleMvcEditor();
            Belletrix.Common.initMultiselect(1);
            $("#StartDate, #EndDate").datepicker();
        };
        StudyAbroad.prototype.bindDelete = function (deleteUrl, dataString) {
            $("button.studyabroad-list-delete").click(function (event) {
                var studyAbroadId = parseInt($(this).data(dataString));
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result) {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, studyAbroadId, function () {
                                window.location.reload();
                            });
                        }
                    }
                });
            });
        };
        return StudyAbroad;
    }());
    Belletrix.StudyAbroad = StudyAbroad;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=StudyAbroad.js.map