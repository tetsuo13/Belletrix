/// <reference path="Common.ts" />
/// <reference path="..\typings\bootbox\bootbox.d.ts" />
var Belletrix;
(function (Belletrix) {
    var StudentBase = (function () {
        function StudentBase() {
            /** Selector for first name input field. */
            this.firstNameSelector = "#FirstName";
            /** Selector for last name input field. */
            this.lastNameSelector = "#LastName";
            /** Selector for both first and last name fields. */
            this.firstLastNamesSelector = this.firstNameSelector + ", " + this.lastNameSelector;
            /**
             * Selector for container that will get the partial view after
             * performing a unique name check.
             */
            this.nameCheckResultSelector = "#unique-name";
        }
        /**
         * Standard initialization for internal student form and public promo
         * form.
         * @param nameCheckUrl URL for unique name check.
         */
        StudentBase.prototype.initForm = function (nameCheckUrl) {
            Belletrix.Common.initMultiselect(1);
            Belletrix.Common.handleMvcEditor();
            $("#DateOfBirth").datepicker();
            this.prepareForm(nameCheckUrl);
        };
        /**
         * Enable/disable all form fields.
         * @param disabled True to disable all fields.
         */
        StudentBase.prototype.toggleAllFormFields = function (disabled) {
            $("#student-form input, #student-form select, #student-form button")
                .prop("disabled", disabled);
        };
        /**
         * Disable all fields except first and last name, force the user to
         * enter that information first.
         *
         * @param nameCheckUrl URL for unique name check.
         */
        StudentBase.prototype.prepareForm = function (nameCheckUrl) {
            var _this = this;
            // https://stackoverflow.com/a/1909508
            var delay = (function () {
                var timer = 0;
                return function (callback, ms) {
                    clearTimeout(timer);
                    timer = setTimeout(callback, ms);
                };
            })();
            this.toggleAllFormFields(true);
            $(this.firstLastNamesSelector).prop("disabled", false);
            $(this.firstLastNamesSelector).keyup(function () {
                delay(function () {
                    _this.checkNameUniqueness(nameCheckUrl, $(_this.firstLastNamesSelector).val(), $(_this.lastNameSelector).val());
                }, 500);
            });
        };
        /**
         * Submit first and last name for unique check. If unique, enable all
         * form fields; otherwise display a list of links to other students
         * matching.
         *
         * @param nameCheckUrl URL for unique name check.
         * @param firstName Student's first name.
         * @param lastName Student's last name.
         */
        StudentBase.prototype.checkNameUniqueness = function (nameCheckUrl, firstName, lastName) {
            if (firstName.length == 0 || lastName.length == 0) {
                return;
            }
            var self = this;
            $.ajax({
                url: nameCheckUrl,
                data: {
                    firstName: firstName,
                    lastName: lastName
                },
                method: "GET",
                cache: false,
                success: function (result) {
                    // Clear it every time we enter this function. Possible
                    // that the user is performing multiple checks in which
                    // case the previous check result needs to be cleared.
                    var uniqueNameContainer = $(self.nameCheckResultSelector).empty();
                    if (result.trim().length > 0) {
                        self.toggleAllFormFields(true);
                        $(self.firstLastNamesSelector).prop("disabled", false);
                        uniqueNameContainer.html(result);
                        self.handleStudentInlineView(uniqueNameContainer);
                    }
                    else {
                        // No duplicates found. Enable all form fields and
                        // move on.
                        self.toggleAllFormFields(false);
                    }
                },
                error: function (jqXHR, textStatus) {
                    var message = "<p>An unknown error occurred while checking student name.</p>" +
                        "<p>" + textStatus + "</p>";
                    Belletrix.Common.errorMessage(message);
                }
            });
        };
        StudentBase.prototype.handleStudentInlineView = function (uniqueNameContainer) {
            $("a.studentview", uniqueNameContainer).on("click", function (e) {
                var anchor = $(this);
                var studentId = anchor.attr("data-bt-studentid");
                e.preventDefault();
                $.ajax({
                    url: anchor.attr("href"),
                    cache: false,
                    success: function (data) {
                        bootbox.dialog({
                            message: data,
                            //onEscape: true,
                            backdrop: true,
                            title: "Student Info",
                            size: "large"
                        });
                    }
                });
            });
        };
        return StudentBase;
    }());
    Belletrix.StudentBase = StudentBase;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=StudentBase.js.map