/// <reference path="..\typings\jquery\jquery.d.ts"/>
/// <reference path="..\typings\bootstrap.datepicker\bootstrap.datepicker.d.ts" />
var Belletrix;
(function (Belletrix) {
    var Experience = (function () {
        function Experience() {
        }
        Experience.prototype.initAdd = function () {
            Belletrix.Common.handleMvcEditor();
            Belletrix.Common.initMultiselect(1);
            $('#StartDate, #EndDate').datepicker();
        };
        Experience.prototype.bindDelete = function (deleteUrl, dataString) {
            $("button.experience-list-delete").click(function (event) {
                var experienceId = parseInt($(this).data(dataString));
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result) {
                        if (!result) {
                            return;
                        }
                        Belletrix.Common.handleDeleteCall(deleteUrl, experienceId, function () {
                            window.location.reload();
                        });
                    }
                });
            });
        };
        return Experience;
    }());
    Belletrix.Experience = Experience;
})(Belletrix || (Belletrix = {}));
