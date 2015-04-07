window.Bennett.AbroadAdvisor = window.Bennett.AbroadAdvisor || {};

(function (Student, $, undefined) {
    'use strict';

    var ajaxUrls = {
        nameCheck: ''
    };

    Student.handleNewNote = function (formSelector, noteSelector,
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

    Student.initStudentAddEdit = function (nameCheckUrl) {
        /// <summary>Initialize student add/edit page.</summary>
        /// <param name="nameCheckUrl" type="String">URL for unique name check.</param>

        ajaxUrls.nameCheck = nameCheckUrl;

        Bennett.AbroadAdvisor.initMultiselect(1);
        Bennett.AbroadAdvisor.handleMvcEditor();

        $('a#studyAbroadDestinations').click(function (e) {
            e.preventDefault();
            Student.addStudyAbroadRows();
        })

        prepareForm();
    };

    function toggleAllFormFields(disabled) {
        /// <summary>Enable/disable all form fields.</summary>
        /// <param name="disabled" type="Boolean">True to disable all fields.</param>

        $('#student-form input, #student-form select, #student-form button').attr('disabled', disabled);
    }

    function prepareForm() {
        /// <summary>
        /// Disable all fields except first and last name, force the user to
        /// enter that information first.
        /// </summary>

        // https://stackoverflow.com/a/1909508
        var delay = (function () {
            var timer = 0;

            return function (callback, ms) {
                /// <summary>Execute callback function after certain timeframe.</summary>
                /// <param name="callback" type="Function">Function to execute.</param>
                /// <param name="ms" type="Number">Milliseconds to wait before executing callback.</param>

                clearTimeout(timer);
                timer = setTimeout(callback, ms);
            };
        })();

        toggleAllFormFields(true);
        $('#FirstName, #LastName').attr('disabled', false);

        $('#FirstName, #LastName').keyup(function () {
            delay(function () {
                checkNameUniqueness($('#FirstName').val(), $('#LastName').val())
            }, 500);
        });
    }

    function checkNameUniqueness(firstName, lastName) {
        /// <summary>
        /// Submit first and last name for unique check. If unique, enable all
        /// form fields; otherwise display a list of links to other students
        /// matching.
        /// </summary>
        /// <param name="firstName" type="String">Student's first name.</param>
        /// <param name="lastName" type="String">Student's last name.</param>

        if (firstName.length == 0 || lastName.length == 0) {
            return;
        }

        $.ajax({
            url: ajaxUrls.nameCheck,
            data: {
                firstName: firstName,
                lastName: lastName
            },
            method: 'GET',
            cache: false,
            success: function (result) {
                var uniqueNameContainer = $('#unique-name').empty();

                if (result.trim().length > 0) {
                    toggleAllFormFields(true);
                    $('#FirstName, #LastName').attr('disabled', false);
                    uniqueNameContainer.html(result);
                } else {
                    // No duplicates found. Enable all form fields and move on.
                    toggleAllFormFields(false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var message = '<p>An unknown error occurred while checking student name.</p>' +
                    '<p>' + textStatus + '</p>';
                Bennett.AbroadAdvisor.errorMessage(message);
            }
        })
    }

    function setupNoteModal() {
        // Load cached modal content and then refetch remote content.
        $(document.body).on('hidden.bs.modal', function () {
            $('#noteModal').removeData('bs.modal');
        });
    }

    Student.initStudentView = function () {
        /// <summary>Initialize the student view page.</summary>

        setupNoteModal();
    };

    Student.initStudentList = function () {
        /// <summary>Initialize the student list page.</summary>

        $('#studentlist').dataTable({
            columnDefs: [{
                targets: -1,
                orderable: false
            }]
        });

        $('a.studentlisttooltop').tooltip();
        $('a.studentlistmodal').modal();
        $('.collapse').collapse();

        setupNoteModal();
        Bennett.AbroadAdvisor.initMultiselect(0, 300);
    };

    Student.addStudyAbroadRows = function (countries, years, periods) {
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
                // Select the "Please Select" option in cases of a zero. This
                // is an invalid value.
                var optionSelector = values[0] == '' ? 'option:first' : 'option[value=' + values[0] + ']';

                $('select#' + idName + ' ' + optionSelector).attr('selected', 'selected');
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

        var rowId = Bennett.AbroadAdvisor.randomString(),
            enclosure,
            anchor;

        $.each(destinationFieldNames, function (idName, values) {
            var firstSelect = $('select#' + idName),
                newSelect = firstSelect.clone().removeAttr('id');

            // Get rid of the selected value cloned over.
            $('option', newSelect).removeAttr('selected');

            if (selIndex !== undefined) {
                $('option[value=' + values[selIndex] + ']', newSelect).attr('selected', 'selected');
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
})(window.Bennett.AbroadAdvisor.Student = window.Bennett.AbroadAdvisor.Student || {}, jQuery);
