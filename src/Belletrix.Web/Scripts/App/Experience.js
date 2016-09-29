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
        Experience.prototype.bindDelete = function (deleteModalSelector, deleteUrl, dataString) {
            Belletrix.Common.handleDeleteModal(deleteModalSelector, deleteUrl, dataString);
        };
        return Experience;
    }());
    Belletrix.Experience = Experience;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=Experience.js.map