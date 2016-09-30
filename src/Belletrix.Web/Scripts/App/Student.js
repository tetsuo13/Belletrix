var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Belletrix;
(function (Belletrix) {
    var Student = (function (_super) {
        __extends(Student, _super);
        function Student() {
            _super.apply(this, arguments);
        }
        /**
         * Initialize student add/edit page.
         * @param nameCheckUrl URL for unique name check.
         */
        Student.prototype.initStudentAddEdit = function (nameCheckUrl) {
            var _this = this;
            _super.prototype.initForm.call(this, nameCheckUrl);
            $("#InitialMeeting").datepicker();
            $("a#studyAbroadDestinations").click(function (e) {
                e.preventDefault();
                _this.addStudyAbroadRows();
            });
        };
        /**
         * Add click event to Study Abroad tab. Will fetch student's
         * experiences in a list and display partial inline.
         * @param tabSelector Selector to tab.
         * @param dataUrl URL to call for student experiences.
         * @param experiencesTableSelector Selector for experiences table.
         * @param experienceDeleteModalSelector
         * @param experienceDeleteUrl
         * @param experienceDataString
         */
        Student.prototype.initStudyAbroadTab = function (tabSelector, dataUrl, experiencesTableSelector, experienceDeleteModalSelector, experienceDeleteUrl, experienceDataString) {
            $('a[href="' + tabSelector + '"]').on("show.bs.tab", function (e) {
                $.ajax({
                    url: dataUrl,
                    method: "GET",
                    cache: false,
                    success: function (data) {
                        $(tabSelector).html(data);
                        var timer = setInterval(function () {
                            var studyAbroadTable = $(experiencesTableSelector);
                            if (studyAbroadTable.length) {
                                studyAbroadTable.DataTable({
                                    columns: [
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        { orderable: false }
                                    ]
                                });
                                clearInterval(timer);
                                Belletrix.Common.handleDeleteModal(experienceDeleteModalSelector, experienceDeleteUrl, experienceDataString);
                            }
                        });
                    }
                });
            });
        };
        /**
         * Initialize the student list page.
         * @param tableSelector
         * @param deleteModalSelector
         * @param deleteUrl
         * @param dataString
         */
        Student.prototype.initStudentList = function (tableSelector, deleteModalSelector, deleteUrl, dataString) {
            new Belletrix.StudentNote();
            $("a.studentlisttooltop").tooltip();
            $(tableSelector).DataTable({
                orderClasses: false,
                columnDefs: [{
                        targets: -1,
                        orderable: false
                    }]
            });
            $(".collapse").collapse();
            Belletrix.Common.initMultiselect(0, 300);
            Belletrix.Common.handleDeleteModal(deleteModalSelector, deleteUrl, dataString);
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
    }(Belletrix.StudentBase));
    Belletrix.Student = Student;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=Student.js.map