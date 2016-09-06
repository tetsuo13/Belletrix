/// <reference path="Common.ts" />

module Belletrix {
    export class StudentBase {
        /** Selector for first name input field. */
        private firstNameSelector: string = "#FirstName";

        /** Selector for last name input field. */
        private lastNameSelector: string = "#LastName";

        /** Selector for both first and last name fields. */
        private firstLastNamesSelector: string = this.firstNameSelector + ", " + this.lastNameSelector;

        /**
         * Selector for container that will get the partial view after
         * performing a unique name check.
         */
        private nameCheckResultSelector: string = "#unique-name";

        /**
         * Standard initialization for internal student form and public promo
         * form.
         *
         * @param nameCheckUrl URL for unique name check.
         */
        protected initForm(nameCheckUrl: string): void {
            Belletrix.Common.initMultiselect(1);
            Belletrix.Common.handleMvcEditor();
            $("#DateOfBirth").datepicker();

            this.prepareForm(nameCheckUrl);
        }

        /**
         * Enable/disable all form fields.
         *
         * @param disabled True to disable all fields.
         */
        private toggleAllFormFields(disabled: boolean): void {
            $("#student-form input, #student-form select, #student-form button")
                .prop("disabled", disabled);
        }

        /**
         * Disable all fields except first and last name, force the user to
         * enter that information first.
         *
         * @param nameCheckUrl URL for unique name check.
         */
        private prepareForm(nameCheckUrl: string): void {
            // https://stackoverflow.com/a/1909508
            let delay = (function () {
                let timer: number = 0;

                return function (callback, ms) {
                    clearTimeout(timer);
                    timer = setTimeout(callback, ms);
                };
            })();

            this.toggleAllFormFields(true);
            $(this.firstLastNamesSelector).prop("disabled", false);

            $(this.firstLastNamesSelector).keyup(() => {
                delay(() => {
                    this.checkNameUniqueness(nameCheckUrl,
                        $(this.firstLastNamesSelector).val(), $(this.lastNameSelector).val())
                }, 500);
            });
        }

        /**
         * Submit first and last name for unique check. If unique, enable all
         * form fields; otherwise display a list of links to other students
         * matching.
         *
         * @param nameCheckUrl URL for unique name check.
         * @param firstName Student's first name.
         * @param lastName Student's last name.
         */
        private checkNameUniqueness(nameCheckUrl: string, firstName: string, lastName: string): void {
            if (firstName.length == 0 || lastName.length == 0) {
                return;
            }

            let self = this;

            $.ajax({
                url: nameCheckUrl,
                data: {
                    firstName: firstName,
                    lastName: lastName
                },
                method: "GET",
                cache: false,
                success: function (result: string): void {
                    // Clear it every time we enter this function. Possible
                    // that the user is performing multiple checks in which
                    // case the previous check result needs to be cleared.
                    let uniqueNameContainer: JQuery = $(self.nameCheckResultSelector).empty();

                    if (result.trim().length > 0) {
                        self.toggleAllFormFields(true);
                        $(self.firstLastNamesSelector).prop("disabled", false);
                        uniqueNameContainer.html(result);
                    } else {
                        // No duplicates found. Enable all form fields and
                        // move on.
                        self.toggleAllFormFields(false);
                    }
                },
                error: function (jqXHR: JQueryXHR, textStatus: any, errorThrown: any): void {
                    let message: string = "<p>An unknown error occurred while checking student name.</p>" +
                        "<p>" + textStatus + "</p>";
                    Belletrix.Common.errorMessage(message);
                }
            })
        }
    }
}
