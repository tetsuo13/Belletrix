/// <reference path="..\typings\jquery\jquery.d.ts" />

namespace Belletrix {
    export class ActivityLog {
        /** Selector for panel that contains each each participant line item. */
        private participantsPanelSelector: string = "#participants-panel";

        private participantModalSelector: string = "#personModal .modal-content";

        /** Selectors for the existing person "form." */
        private existingPersonSelectors = {
            /// <field type="String">Person selectlist.</field>
            PersonSelectList: "#existingpersonselect",

            /// <field type="String">Type selectlist.</field>
            TypeSelectList: "#existingpersontype",

            /// <field type="String">Submit button to "form."</field>
            SubmitButton: "#existingpersonsubmit"
        };

        /** URL to call to remove existing participant from session. */
        private removeExistingPersonIdUrl: string = "";

        /** Current session ID. **/
        private sessionId: string = "";

        /**
         * Activity log add/edit initialize routine.
         * @param addPersonUrl
         * @param addPersonIdUrl URL to submit existing select person to.
         * @param removePersonIdUrl URL to remove existing select person from session.
         * @param sessionId Current activity session ID.
         */
        public initAddEdit(addPersonUrl: string, addPersonIdUrl: string, removePersonIdUrl: string,
            sessionId: string): void {
            this.removeExistingPersonIdUrl = removePersonIdUrl;
            this.sessionId = sessionId;

            const self = this;

            Belletrix.Common.handleMvcEditor();
            $("#StartDate, #EndDate").datepicker();
            Belletrix.Common.initMultiselect(1);

            $("#addPersonButton").click(function (): void {
                $.ajax({
                    url: addPersonUrl,
                    success: function (data: any): void {
                        const addPersonModal: JQuery = bootbox.dialog({
                            message: data,
                            backdrop: true,
                            title: "Create New Person"
                        });

                        addPersonModal.on("shown.bs.modal", function (): void {
                            self.bindParticipantForm(addPersonModal);
                            self.bindExistingParticipantForm(addPersonModal, addPersonIdUrl);
                            $("#FullName").focus();
                        });
                    }
                });
            });
        };

        /**
         * Initializes the back-end session for participants.
         * @param startSessionUrl URL to trigger starting session.
         * @param populateSessionUrl URL to populate existing participants into the session for an existing activity.
         * @param participantsUrl URL to fetch existing participants.
         */
        public initSession(startSessionUrl: string, populateSessionUrl?: string, participantsUrl?: string): void {
            const self = this;

            $.ajax({
                url: startSessionUrl,
                cache: false,
                success: function (): void {
                    if (populateSessionUrl) {
                        // TODO: Could these actions someday be consolidated into one server-side action perhaps? Hmmm...

                        // Get the server-side to populate session.
                        $.ajax({
                            url: populateSessionUrl,
                            cache: false,
                            success: function (): void {
                                // Then retrieve participants from session to
                                // populate form.
                                $.ajax({
                                    dataType: "json",
                                    url: participantsUrl,
                                    cache: false,
                                    success: function (data): void {
                                        $.each(data, function (i, value): void {
                                            self.addParticipantRow(value.Person.FullName, value.Person.Id);
                                        });
                                    }
                                });
                            }
                        });
                    }
                }
            });
        };

        /**
         * Handle selecting existing person. When the existing person and
         * their participation type is selected, the submit button should:
         * initiate an ajax request to save the selection to the current
         * activity, add the person and type to the activity form, and finally
         * close the modal dialog.
         * @param modalDialog Modal dialog.
         * @param addPersonIdUrl URL to submit existing select person to.
         */
        private bindExistingParticipantForm(modalDialog: JQuery, addPersonIdUrl: string): void {
            const personSelect: JQuery = $(this.existingPersonSelectors.PersonSelectList);
            const typeSelect: JQuery = $(this.existingPersonSelectors.TypeSelectList);
            const submitButton: JQuery = $(this.existingPersonSelectors.SubmitButton);

            // The modal dialog will only show the existing people fields if
            // there are existing people to begin with.
            if (personSelect.length) {
                const self = this;

                submitButton.click(function (event: JQueryEventObject): void {
                    event.preventDefault();

                    $.ajax({
                        url: addPersonIdUrl,
                        type: "POST",
                        data: {
                            id: personSelect.val(),
                            type: typeSelect.val(),
                            sessionId: self.sessionId
                        },
                        success: function (result: any): void {
                            if (!result.hasOwnProperty("Success") ||
                                !result.hasOwnProperty("Message")) {
                                Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);

                            } else if (result.Success === true) {
                                self.addParticipantRow($("option:selected", personSelect).text(), personSelect.val());
                                modalDialog.modal("hide");

                            } else {
                                // Show server-side error message under the
                                // full name field.

                                const validator: JQueryValidation.Validator = modalDialog.validate();
                                validator.showErrors({
                                    "FullName": result.Message
                                })
                            }
                        }
                    });
                });
            }
        }

        /**
         * Add jQuery Validator to the new person form. The form should
         * perform client-side validation and, if successful, submit the data
         * via ajax, add the person and type to the activity form, and finally
         * close the modal dialog.
         * @param modalDialog Modal dialog.
         */
        private bindParticipantForm(modalDialog: JQuery): void {
            const self = this;
            const modalDialogForm: JQuery = $("form", modalDialog);

            $.validator.unobtrusive.parse(modalDialogForm);

            modalDialogForm.submit(function (event: JQueryEventObject): void {
                event.preventDefault();

                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result: any): void {
                        if (!result.hasOwnProperty("Success") ||
                            !result.hasOwnProperty("Message") ||
                            !result.hasOwnProperty("Id")) {
                            Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);

                        } else if (result.Success === true) {
                            self.addParticipantRow($("#FullName", modalDialogForm).val(), result.Id);
                            modalDialog.modal("hide");

                        } else {
                            // Show server-side error message under the full
                            // name field.

                            const validator: JQueryValidation.Validator = modalDialogForm.validate();
                            validator.showErrors({
                                "FullName": result.Message
                            })
                        }
                    }
                });
            });
        }

        /**
         * Remove participant from session.
         * @param id Participant ID.
         */
        private deleteParticipant(id: number): void {
            const self = this;

            $.ajax({
                url: self.removeExistingPersonIdUrl,
                type: "DELETE",
                data: {
                    id: id,
                    sessionId: self.sessionId
                },
                success: function (result: any): void {
                    if (!result.hasOwnProperty("Success") ||
                        !result.hasOwnProperty("Message")) {
                        Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);

                    } else if (result.Success === true) {
                        $("#participant-" + id).fadeOut(300, function (): void {
                            $(this).remove();
                        });

                    } else {
                        Belletrix.Common.errorMessage(result.Message);
                    }
                }
            });
        }

        /**
         * Add a new row to the participant panel for the newly created person.
         * @param fullName Person's full name.
         * @param id Internal unique identifier.
         */
        private addParticipantRow(fullName: string, id: number): void {
            const row: JQuery = $('<div class="row" id="participant-' + id + '"></div>');
            const deleteIcon: JQuery = $('<a href="" class="btn btn-xs btn-danger pull-right" title="Remove"></a>');
            let actionColumn: JQuery;
            const self = this;

            deleteIcon.append('<i class="glyphicon glyphicon-remove"></i>');

            deleteIcon.click((event: JQueryEventObject): void => {
                event.preventDefault();
                self.deleteParticipant(id);
            });

            row.append('<div class="col-lg-10"><div class="form-group">' + fullName + "</div></div>");

            actionColumn = $('<div class="form-group"></div>')
                .append(deleteIcon);

            $('<div class="col-lg-2"></div>')
                .append(actionColumn)
                .appendTo(row);

            row.insertBefore($(".last-row", this.participantsPanelSelector));
        }

        /**
         * Display partial view beneath the activity log title showing an
         * existing activity log by the same title if found.
         * @param titleSelector Title input selector.
         * @param resultSelector Selector for container to display any duplicate info.
         * @param titleCheckUrl URL to send title to.
         */
        public initTitleCheck(titleSelector: string, resultSelector: string, titleCheckUrl: string): void {
            $(titleSelector).blur(function (): void {
                const title: string = $(this).val();

                if (title.trim().length == 0) {
                    return;
                }

                $.ajax({
                    url: titleCheckUrl,
                    data: {
                        title: title
                    },
                    method: "GET",
                    cache: false,
                    success: function (result: string): void {
                        const uniqueNameContainer: JQuery = $(resultSelector).empty();

                        if (result.trim().length > 0) {
                            uniqueNameContainer.html(result);
                        }
                    },
                    error: function (jqXHR: JQueryXHR, textStatus: string): void {
                        const message: string = "<p>An unknown error occurred while checking title.</p>" +
                            "<p>" + textStatus + "</p>";
                        Belletrix.Common.errorMessage(message);
                    }
                })
            });
        }

        /**
         * Set up things for viewing an Activity Log like document upload.
         * @param addNewDocumentUrl URL to upload document to.
         * @param activityLogId The ID of the Activity Log to attach to.
         * @param buttonSelector Selector for the "Add" document button.
         * @param activityLogDocument
         */
        public initView(addNewDocumentUrl: string, activityLogId: number, buttonSelector: string,
            activityLogDocument: Belletrix.ActivityLogDocument): void {

            new Belletrix.Document().initActivityLog(addNewDocumentUrl, activityLogId, buttonSelector, function (): void {
                activityLogDocument.refreshList();
            });
        }
    }
}
