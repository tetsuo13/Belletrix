/**
 */

window.Bennett = window.Bennett || {};

(function (AbroadAdvisor, $) {
    'use strict';

    // Number of milliseconds between pinging the server.
    var idleKillerInterval = 1000 * 60 * 15;

    /**
     * @param {string} formSelector
     * @param {string} noteSelector
     * @param {string} userFirstName
     * @param {string} userLastName
     * @public
     */
    AbroadAdvisor.handleNewNote = function (formSelector, noteSelector,
                                            userFirstName, userLastName) {
        $(formSelector).submit(function (event) {
            var noteElement = $(noteSelector),
                noteValue = noteElement.val();

            event.preventDefault();

            $.post($(this).attr('action'),
                $(formSelector).serialize(),
                function () {
                    var anchor = $('<a href="#" class="list-group-item"></a>'),
                        listGroup = $('<div class="list-group"></div>'),
                        paraNote = $('<p class="list-group-item-text"></p>');

                    paraNote.text(noteValue);

                    anchor.append('<h4 class="list-group-item-heading">' + userFirstName + ' ' + userLastName + '</h4>');
                    anchor.append(paraNote);

                    listGroup.append(anchor);
                    listGroup
                        .hide()
                        .insertAfter('div.modal-body div.panel-default')
                        .fadeIn(750);

                    noteElement.val('');
                }
            );
        });
    };

    /**
     * Have the server process something often.
     *
     * This is to avoid the application pool from shutting down after a period
     * of inactivity.
     *
     * @param {string} pingUrl
     * @public
     */
    AbroadAdvisor.initPinger = function (pingUrl) {
        setInterval(function () {
            $.ajax({
                url: pingUrl,
                cache: false,
                type: 'GET'
            });
        }, idleKillerInterval);
    };
})(window.Bennett.AbroadAdvisor = window.Bennett.AbroadAdvisor || {}, jQuery, undefined);
