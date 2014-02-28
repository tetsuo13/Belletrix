/**
 */

window.Bennett = window.Bennett || {};

(function (AbroadAdvisor, $) {
    'use strict';

    /// <var type="Number">Number of milliseconds between pinging the server</var>
    var idleKillerInterval = 1000 * 60 * 10;

    AbroadAdvisor.initStudentAddEdit = function () {
        /// <summary>Initialize student add/edit page.</summary>

        $.each(['#DateOfBirth', '#InitialMeeting'], function (i, val) {
            $(val).addClass('form-control');
            $(val).datepicker({
                format: 'm/d/yyyy'
            });
        });

        $('.multiselect').multiselect({
            numberDisplayed: 1,
            buttonContainer: '<div class="button-default" />'
        });

        $('a#studyAbroadDestinations').click(function (e) {
            e.preventDefault();
            AbroadAdvisor.addStudyAbroadRows();
        })
    };

    AbroadAdvisor.addStudyAbroadRows = function (countries, years, periods) {
        /// <summary>
        /// Add a country, year, semester row group.
        ///
        /// The three parameters won't exist when the "Another destination"
        /// link is used.
        /// </summary>
        /// <param name="countries" type="Array" elementType="Number" optional="true"></param>
        /// <param name="year" type="Array" elementType="Number" optional="true"></param>
        /// <param name="periods" type="Array" elementType="Number" optional="true"></param>

        var destinationFieldNames = {
                'StudyAbroadCountry': countries,
                'StudyAbroadYear': years,
                'StudyAbroadPeriod': periods
            },
            addExistingValues = countries !== undefined && years !== undefined && periods !== undefined,
            i;

        if (addExistingValues && countries.length == 0) {
            return;
        }

        if (addExistingValues) {
            // Populate the first row.
            $.each(destinationFieldNames, function (idName, values) {
                $('select#' + idName).val(values[0]);
            });

            // Now add every additional row that's needed.
            for (i = 1; i < countries.length; i++) {
                addStudyAbroadRow(destinationFieldNames, i);
            }
        } else {
            addStudyAbroadRow(destinationFieldNames);
        }
    };

    
    function addStudyAbroadRow(destinationFieldNames, selIndex) {
        /// <summary>
        /// Adds country, year, semester row with optionally selected values.
        ///
        /// The "desired study abroad" country, year, and semester group is
        /// done by taking advantage of IEnumerable type and the MVC
        /// framework. When a form is submitted containing two or more of the
        /// exact same element names, MVC will string them up as an array. So
        /// two inputs both named "foo" will end up as the "foo" array.
        /// </summary>
        /// <param name="destinationFieldNames" type="Object">
        /// Collection of IDs to use.
        /// </param>
        /// <param name="selIndex" type="Number" optional="true">
        /// Index to select
        /// </param>

        var rowId = (Math.random() + 1).toString(36).slice(2),
            enclosure,
            anchor;

        $.each(destinationFieldNames, function (idName, values) {
            var firstSelect = $('select#' + idName),
                newSelect = firstSelect.clone().removeAttr('id');

            if (selIndex !== undefined) {
                newSelect.val(values[selIndex]);
            }

            $('<div class="form-group reducePadding" />')
                .addClass(rowId)
                .append(newSelect)
                .appendTo(firstSelect.parent().parent());
        });

        enclosure = $('<div class="form-group reducePadding">&nbsp;</div>')
            .addClass(rowId);

        anchor = $('<a href="" title="Remove destination row"></a>')
            .html('<i class="fa fa-minus-circle"></i>')
            .click(function (e) {
                e.preventDefault();
                $('.' + rowId).remove();
            });

        enclosure
            .append(anchor)
            .appendTo($('div#studyAbroadRowRemovals').parent());
    }

    AbroadAdvisor.handleNewNote = function (formSelector, noteSelector,
                                            userFirstName, userLastName) {
        /// <summary></summary>
        /// <param name="formSelector" type="String"></param>
        /// <param name="noteSelector" type="String"></param>
        /// <param name="userFirstName" type="String"></param>
        /// <param name="userLastName" type="String"></param>

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
