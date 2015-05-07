/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
/// <reference path="..\typings\jquery.dataTables\jquery.dataTables.d.ts" />

'use strict';

module Belletrix {
    class Student {
        private ajaxUrls: any = {
            nameCheck: ''
        };

        public handleNewNote(formSelector: string, noteSelector: string, userFirstName: string,
            userLastName: string): void {

            $(formSelector).submit(function (event) {
                var noteElement: JQuery = $(noteSelector),
                    noteValue: string = noteElement.val();

                event.preventDefault();

                $.post($(this).attr('action'),
                    $(formSelector).serialize(),
                    function () {
                        var anchor: JQuery = $('<a href="#" class="list-group-item"></a>'),
                            listGroup: JQuery = $('<div class="list-group"></div>'),
                            paraNote: JQuery = $('<p class="list-group-item-text"></p>');

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
        }

        /**
        * Initialize student add/edit page.
        *
        * @param nameCheckUrl URL for unique name check.
        */
        public initStudentAddEdit(nameCheckUrl: string): void {
            this.ajaxUrls.nameCheck = nameCheckUrl;

            Common.initMultiselect(1);
            Common.handleMvcEditor();

            $('a#studyAbroadDestinations').click(function (e) {
                e.preventDefault();
                this.addStudyAbroadRows();
            })

            this.prepareForm();
        }

        /**
        * Enable/disable all form fields.
        *
        * @param disabled True to disable all fields.
        */
        private toggleAllFormFields(disabled: boolean): void {
            var button: JQuery = $('#student-form input, #student-form select, #student-form button');

            if (disabled) {
                button.attr('disabled', 'disabled');
            } else {
                button.removeAttr('disabled');
            }
        }

        /**
        * Disable all fields except first and last name, force the user to
        * enter that information first.
        */
        private prepareForm(): void {
            // https://stackoverflow.com/a/1909508
            var delay = (function () {
                var timer: number = 0;

                return function (callback, ms) {
                    /// <summary>Execute callback function after certain timeframe.</summary>
                    /// <param name="callback" type="Function">Function to execute.</param>
                    /// <param name="ms" type="Number">Milliseconds to wait before executing callback.</param>

                    clearTimeout(timer);
                    timer = setTimeout(callback, ms);
                };
            })();

            this.toggleAllFormFields(true);
            $('#FirstName, #LastName').removeAttr('disabled');

            $('#FirstName, #LastName').keyup(function () {
                delay(function () {
                    this.checkNameUniqueness($('#FirstName').val(), $('#LastName').val())
                }, 500);
            });
        }

        /**
        * Submit first and last name for unique check. If unique, enable all
        * form fields; otherwise display a list of links to other students
        * matching.
        *
        * @param firstName Student's first name.
        * @param lastName Student's last name.
        */
        private checkNameUniqueness(firstName: string, lastName: string): void {
            if (firstName.length == 0 || lastName.length == 0) {
                return;
            }

            $.ajax({
                url: this.ajaxUrls.nameCheck,
                data: {
                    firstName: firstName,
                    lastName: lastName
                },
                method: 'GET',
                cache: false,
                success: function (result) {
                    var uniqueNameContainer: JQuery = $('#unique-name').empty();

                    if (result.trim().length > 0) {
                        this.toggleAllFormFields(true);
                        $('#FirstName, #LastName').removeAttr('disabled');
                        uniqueNameContainer.html(result);
                    } else {
                        // No duplicates found. Enable all form fields and move on.
                        this.toggleAllFormFields(false);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var message: string = '<p>An unknown error occurred while checking student name.</p>' +
                        '<p>' + textStatus + '</p>';
                    Common.errorMessage(message);
                }
            });
        }

        private setupNoteModal(): void {
            // Load cached modal content and then refetch remote content.
            $(document.body).on('hidden.bs.modal', function () {
                $('#noteModal').removeData('bs.modal');
            });
        }

        /**
        * Initialize the student view page.
        */
        public initStudentView(): void {
            this.setupNoteModal();
        }

        /**
        * Initialize the student list page.
        */
        public initStudentList(): void {
            $('#studentlist').DataTable({
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });

            $('a.studentlisttooltop').tooltip();
            $('a.studentlistmodal').modal();
            $('.collapse').collapse();

            this.setupNoteModal();
            Common.initMultiselect(0, 300);
        }

        /**
        * Add a country, year, semester row group.
        *
        * The three parameters won't exist when the "Another destination"
        * link is used.
        *
        * @param countries
        * @param years
        * @param periods
        */
        public addStudyAbroadRows(countries?: number[], years?: number[], periods?: number[]): void {
            var destinationFieldNames: any = {
                'StudyAbroadCountry': countries,
                'StudyAbroadYear': years,
                'StudyAbroadPeriod': periods
            };

            var addExistingValues: boolean = false;

            if (countries && years && periods) {
                addExistingValues = true;
            }

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
                for (var i: number = 1; i < countries.length; i++) {
                    this.addStudyAbroadRow(destinationFieldNames, i);
                }
            } else {
                this.addStudyAbroadRow(destinationFieldNames);
            }
        }

        /**
        * Adds country, year, semester row with optionally selected values.
        *
        * The "desired study abroad" country, year, and semester group is done
        * by taking advantage of IEnumerable type and the MVC framework. When
        * a form is submitted containing two or more of the exact same element
        * names, MVC will string them up as an array. So two inputs both named
        * "foo" will end up as the "foo" array.
        *
        * @param destinationFieldNames Collection of IDs to use.
        * @param selIndex Index to select.
        */
        private addStudyAbroadRow(destinationFieldNames: any, selIndex?: number): void {
            var rowId = Common.randomString(),
                enclosure,
                anchor;

            $.each(destinationFieldNames, function (idName, values) {
                var firstSelect: JQuery = $('select#' + idName),
                    newSelect: JQuery = firstSelect.clone().removeAttr('id');

                // Get rid of the selected value cloned over.
                $('option', newSelect).removeAttr('selected');

                if (selIndex) {
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
    }
}
 