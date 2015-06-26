window.Belletrix = window.Belletrix || {};

(function (ActivityLog, $, undefined) {
    'use strict';

    ActivityLog.init = function () {
        Belletrix.handleMvcEditor();
        $('#StartDate, #EndDate').datepicker();
    };
})(window.Belletrix.ActivityLog = window.Belletrix.ActivityLog || {}, jQuery);
