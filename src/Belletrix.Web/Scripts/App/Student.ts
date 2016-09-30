module Belletrix {
    export class Student extends StudentBase {
        /**
         * Initialize student add/edit page.
         * @param nameCheckUrl URL for unique name check.
         */
        public initStudentAddEdit(nameCheckUrl: string): void {
            super.initForm(nameCheckUrl);

            $("#InitialMeeting").datepicker();

            $("a#studyAbroadDestinations").click((e: JQueryEventObject): void => {
                e.preventDefault();
                this.addStudyAbroadRows();
            })
        }

        /**
         * Add click event to Study Abroad tab. Will fetch student's
         * experiences in a list and display partial inline.
         * @param tabSelector Selector to tab.
         * @param dataUrl URL to call for student experiences.
         * @param experiencesTableSelector Selector for experiences table.
         * @param experienceDeleteUrl
         * @param experienceDataString
         */
        public initStudyAbroadTab(tabSelector: string, dataUrl: string, experiencesTableSelector: string,
            experienceDeleteUrl: string, experienceDataString: string): void {

            $('a[href="' + tabSelector + '"]').on("show.bs.tab", function (e: JQueryEventObject): void {
                $.ajax({
                    url: dataUrl,
                    method: "GET",
                    cache: false,
                    success: function (data: string): void {
                        $(tabSelector).html(data);

                        let timer = setInterval(function (): void {
                            let studyAbroadTable: JQuery = $(experiencesTableSelector);

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

                                $("button.experience-list-delete").click(function (event: JQueryEventObject): void {
                                    let experienceId: number = parseInt($(this).data(experienceDataString));

                                    bootbox.confirm({
                                        size: "small",
                                        message: "Are you sure?",
                                        callback: function (result: boolean): void {
                                            if (!result) {
                                                return;
                                            }

                                            Belletrix.Common.handleDeleteCall(experienceDeleteUrl,
                                                experienceId, function () {
                                                window.location.reload();
                                            });
                                        }
                                    });
                                });
                            }
                        });
                    }
                });
            });
        }

        /**
         * Initialize the student list page.
         * @param tableSelector
         * @param deleteUrl
         * @param dataString
         */
        public initStudentList(tableSelector: string, deleteUrl: string, dataString: string): void {
            new StudentNote();
            $("a.studentlisttooltop").tooltip();

            $(tableSelector).DataTable({
                orderClasses: false,
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }]
            });

            $(".collapse").collapse();

            this.handleStudentDelete("button.student-list-delete", deleteUrl, dataString, function (): void {
                window.location.reload();
            });

            Belletrix.Common.initMultiselect(0, 300);
        };

        public initView(deleteUrl: string, dataString: string, listUrl: string): void {
            this.handleStudentDelete("button.student-view-delete", deleteUrl, dataString, function (): void {
                window.location.href = listUrl;
            });
        }

        private handleStudentDelete(classSelector: string, deleteUrl: string, dataString: string,
            successCallback: Function) {

            $(classSelector).on("click", function (event: JQueryEventObject): void {
                let studentId: number = parseInt($(this).data(dataString));

                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result: boolean): void {
                        if (!result) {
                            return;
                        }

                        Belletrix.Common.handleDeleteCall(deleteUrl, studentId, successCallback);
                    }
                });
            });
        }

        /**
         * Add a country, year, semester row group.
         * 
         * The three parameters won't exist when the "Another destination"
         * link is used.
         * @param countries
         * @param years
         * @param periods
         */
        public addStudyAbroadRows(countries?: Array<number>, years?: Array<number>, periods?: Array<number>): void {
            let destinationFieldNames = {
                "StudyAbroadCountry": countries,
                "StudyAbroadYear": years,
                "StudyAbroadPeriod": periods
            };
            const addExistingValues = countries && years && periods;

            if (addExistingValues && countries.length == 0) {
                return;
            }

            if (addExistingValues) {
                // Populate the first row.
                $.each(destinationFieldNames, function (idName, values) {
                    // Select the "Please Select" option in cases of a zero. This
                    // is an invalid value.
                    const optionSelector: string = values[0] == "" ? "option:first" : "option[value=" + values[0] + "]";

                    $("select#" + idName + " " + optionSelector).attr("selected", "selected");
                });

                // Now add every additional row that's needed.
                for (let i: number = 1; i < countries.length; i++) {
                    this.addStudyAbroadRow(destinationFieldNames, i);
                }
            } else {
                this.addStudyAbroadRow(destinationFieldNames);
            }
        };

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
        private addStudyAbroadRow(destinationFieldNames: any, selIndex?: number): void {
            const rowId: string = Belletrix.Common.randomString();
            let enclosure: JQuery;
            let anchor: JQuery;

            $.each(destinationFieldNames, function (idName, values): void {
                let firstSelect: JQuery = $("select#" + idName);
                let newSelect: JQuery = firstSelect.clone().removeAttr('id');

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
                .click(function (e: JQueryEventObject): void {
                    e.preventDefault();
                    $("." + rowId).remove();
                });

            enclosure
                .append(anchor)
                .appendTo($("div#studyAbroadRowRemovals").parent());
        }
    }
}
