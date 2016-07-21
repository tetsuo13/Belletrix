/// <reference path="..\typings\jquery.dataTables\jquery.dataTables.d.ts" />
var Belletrix;
(function (Belletrix) {
    var Student = (function () {
        function Student() {
            this.ajaxUrls = {
                nameCheck: ""
            };
        }
        /**
         * Initialize student add/edit page.
         * @param nameCheckUrl URL for unique name check.
         */
        Student.prototype.initStudentAddEdit = function (nameCheckUrl) {
            this.ajaxUrls.nameCheck = nameCheckUrl;
            Belletrix.Common.initMultiselect(1);
            Belletrix.Common.handleMvcEditor();
            $("#DateOfBirth, #InitialMeeting").datepicker();
            $("a#studyAbroadDestinations").click(function (e) {
                e.preventDefault();
                this.addStudyAbroadRows();
            });
            this.prepareForm();
        };
        ;
        /**
         * Enable/disable all form fields.
         * @param disabled True to disable all fields.
         */
        Student.prototype.toggleAllFormFields = function (disabled) {
            $("#student-form input, #student-form select, #student-form button")
                .prop("disabled", disabled);
        };
        /**
         * Disable all fields except first and last name, force the user to
         * enter that information first.
         */
        Student.prototype.prepareForm = function () {
            // https://stackoverflow.com/a/1909508
            var delay = (function () {
                var timer = 0;
                return function (callback, ms) {
                    clearTimeout(timer);
                    timer = setTimeout(callback, ms);
                };
            })();
            this.toggleAllFormFields(true);
            $("#FirstName, #LastName").prop("disabled", false);
            $("#FirstName, #LastName").keyup(function () {
                delay(function () {
                    this.checkNameUniqueness($("#FirstName").val(), $("#LastName").val());
                }, 500);
            });
        };
        /**
         * Submit first and last name for unique check. If unique, enable all
         * form fields; otherwise display a list of links to other students
         * matching.
         * @param firstName Student's first name.
         * @param lastName Student's last name.
         */
        Student.prototype.checkNameUniqueness = function (firstName, lastName) {
            if (firstName.length == 0 || lastName.length == 0) {
                return;
            }
            $.ajax({
                url: this.ajaxUrls.nameCheck,
                data: {
                    firstName: firstName,
                    lastName: lastName
                },
                method: "GET",
                cache: false,
                success: function (result) {
                    var uniqueNameContainer = $("#unique-name").empty();
                    if (result.trim().length > 0) {
                        this.toggleAllFormFields(true);
                        $("#FirstName, #LastName").prop("disabled", false);
                        this.uniqueNameContainer.html(result);
                    }
                    else {
                        // No duplicates found. Enable all form fields and move on.
                        this.toggleAllFormFields(false);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var message = "<p>An unknown error occurred while checking student name.</p>" +
                        "<p>" + textStatus + "</p>";
                    Belletrix.Common.errorMessage(message);
                }
            });
        };
        /**
         * Initialize the student list page.
         */
        Student.prototype.initStudentList = function () {
            /*$("#studentlist").dataTable({
                orderClasses: false,
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });*/
            //$("#studentlist").dataTable({
            //    aoColumnDefs: [{
            //        aTargets: -1,
            //        bSortable: false
            //    }]
            //});
            $('#studentlist').DataTable({
                columnDefs: [{
                        targets: -1,
                        orderable: false
                    }]
            });
            $("a.studentlisttooltop").tooltip();
            $(".collapse").collapse();
            new Belletrix.StudentNote();
            Belletrix.Common.initMultiselect(0, 300);
        };
        ;
        /**
         * Add a country, year, semester row group.
         *
         * The three parameters won't exist when the "Another destination"
         * link is used.
         * @param countries
         * @param years
         * @param periods
         */
        Student.prototype.addStudyAbroadRows = function (countries, years, periods) {
            var destinationFieldNames = {
                "StudyAbroadCountry": countries,
                "StudyAbroadYear": years,
                "StudyAbroadPeriod": periods
            };
            var addExistingValues = countries && years && periods;
            if (addExistingValues && countries.length == 0) {
                return;
            }
            if (addExistingValues) {
                // Populate the first row.
                $.each(destinationFieldNames, function (idName, values) {
                    // Select the "Please Select" option in cases of a zero. This
                    // is an invalid value.
                    var optionSelector = values[0] == "" ? "option:first" : "option[value=" + values[0] + "]";
                    $("select#" + idName + " " + optionSelector).attr("selected", "selected");
                });
                // Now add every additional row that's needed.
                for (var i = 1; i < countries.length; i++) {
                    this.addStudyAbroadRow(destinationFieldNames, i);
                }
            }
            else {
                this.addStudyAbroadRow(destinationFieldNames);
            }
        };
        ;
        /**
         * Adds country, year, semester row with optionally selected values.
         *
         * The "desired study abroad" country, year, and semester group is
         * done by taking advantage of IEnumerable type and the MVC
         * framework. When a form is submitted containing two or more of the
         * exact same element names, MVC will string them up as an array. So
         * two inputs both named "foo" will end up as the "foo" array.
         * @param destinationFieldNames Collection of IDs to use.
         * @param selIndex Index to select.
         */
        Student.prototype.addStudyAbroadRow = function (destinationFieldNames, selIndex) {
            var rowId = Belletrix.Common.randomString();
            var enclosure;
            var anchor;
            $.each(destinationFieldNames, function (idName, values) {
                var firstSelect = $("select#" + idName);
                var newSelect = firstSelect.clone().removeAttr('id');
                // Get rid of the selected value cloned over.
                $("option", newSelect).removeAttr("selected");
                if (selIndex) {
                    $("option[value=" + values[selIndex] + "]", newSelect).attr("selected", "selected");
                }
                $('<div class="form-group reducePadding" />')
                    .addClass(rowId)
                    .append(newSelect)
                    .appendTo(firstSelect.parent().parent());
            });
            enclosure = $('<div class="form-group reducePadding">&nbsp;</div>')
                .addClass(rowId);
            anchor = $('<a href="" title="Remove destination row"></a>')
                .html('<i class="glyphicon glyphicon-minus-sign"></i>')
                .click(function (e) {
                e.preventDefault();
                $("." + rowId).remove();
            });
            enclosure
                .append(anchor)
                .appendTo($("div#studyAbroadRowRemovals").parent());
        };
        return Student;
    }());
    Belletrix.Student = Student;
})(Belletrix || (Belletrix = {}));
