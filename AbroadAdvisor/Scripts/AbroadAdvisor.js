window.Bennett = window.Bennett || {};

(function (AbroadAdvisor, $) {
    'use strict';

    /// <var type="Number">Number of milliseconds between pinging the server</var>
    var idleKillerInterval = 1000 * 60 * 10;

    AbroadAdvisor.initPinger = function (pingUrl) {
        /// <summary>
        /// Have the server process something often. This is to avoid the
        /// application pool from shutting down after a period of inactivity.
        /// </summary>
        /// <param name="pingUrl" type="String"></param>

        setInterval(function () {
            $.ajax({
                url: pingUrl,
                cache: false,
                type: 'GET'
            });
        }, idleKillerInterval);
    };
})(window.Bennett.AbroadAdvisor = window.Bennett.AbroadAdvisor || {}, jQuery, undefined);
