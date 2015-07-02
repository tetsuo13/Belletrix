window.Belletrix = window.Belletrix || {};

(function (ActivityLog, $, undefined) {
    'use strict';

    ActivityLog.init = function () {
        Belletrix.handleMvcEditor();
        $('#StartDate, #EndDate').datepicker();
        Belletrix.initMultiselect(1);
    };
})(window.Belletrix.ActivityLog = window.Belletrix.ActivityLog || {}, jQuery);
