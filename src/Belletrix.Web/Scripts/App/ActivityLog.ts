module Belletrix {
    export class ActivityLog {
        /** Selector for panel that contains each each participant line item. */
        private _participantsPanelSelector: string = "#participants-panel";

        private _participantModalSelector: string = "#personModal .modal-content";

        /** Selectors for the existing person "form." */
        private _existingPersonSelectors = {
            /// <field type="String">Person selectlist.</field>
            PersonSelectList: "#existingpersonselect",

            /// <field type="String">Type selectlist.</field>
            TypeSelectList: "#existingpersontype",

            /// <field type="String">Submit button to "form."</field>
            SubmitButton: "#existingpersonsubmit"
        };

        /** URL to call to remove existing participant from session. */
        private _removeExistingPersonIdUrl: string = "";

        /** Current session ID. **/
        private _sessionId: string = "";

        /**
         * Activity log add/edit initialize routine.
         * @param addPersonUrl
         * @param addPersonIdUrl URL to submit existing select person to.
         * @param removePersonIdUrl URL to remove existing select person from session.
         * @param sessionId Current activity session ID.
         */
        constructor(addPersonUrl: string, addPersonIdUrl: string, removePersonIdUrl: string, sessionId: string) {
            this._removeExistingPersonIdUrl = removePersonIdUrl;
            this._sessionId = sessionId;

            let self = this;

            Belletrix.Common.handleMvcEditor();
            $("#StartDate, #EndDate").datepicker();
            Belletrix.Common.initMultiselect(1);

            $("#addPersonButton").click(function () {
                $.ajax({
                    url: addPersonUrl,
                    success: function (data) {
                        let addPersonModal: JQuery = bootbox.dialog({
                            message: data,
                            //onEscape: true,
                            backdrop: true,
                            title: "Create New Person"
                        });

                        addPersonModal.on('shown.bs.modal', function () {
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
        initSession(startSessionUrl: string, populateSessionUrl?: string, participantsUrl?: string): void {
            $.ajax({
                url: startSessionUrl,
                cache: false
            });

            if (populateSessionUrl) {
                let self = this;
                // TODO: Could these actions someday be consolidated into one server-side action perhaps? Hmmm...

                // Get the server-side to populate session.
                $.ajax({
                    url: populateSessionUrl,
                    cache: false,
                    success: function () {
                        // Then retrieve participants from session to populate
                        // form.
                        $.ajax({
                            dataType: "json",
                            url: participantsUrl,
                            cache: false,
                            success: function (data, textStatus, jqXHR) {
                                $.each(data, function (index, value) {
                                    self.addParticipantRow(value.Person.FullName, value.Person.Id);
                                });
                            }
                        });
                    }
                });
            }
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
            let personSelect: JQuery = $(this._existingPersonSelectors.PersonSelectList);
            let typeSelect: JQuery = $(this._existingPersonSelectors.TypeSelectList);
            let submitButton: JQuery = $(this._existingPersonSelectors.SubmitButton);

            // The modal dialog will only show the existing people fields if
            // there are existing people to begin with.
            if (personSelect.length) {
                let self = this;

                submitButton.click(function (event: JQueryEventObject) {
                    event.preventDefault();

                    $.ajax({
                        url: addPersonIdUrl,
                        type: "POST",
                        data: {
                            id: personSelect.val(),
                            type: typeSelect.val(),
                            sessionId: self._sessionId
                        },
                        success: function (result: any) {
                            if (!result.hasOwnProperty("Success") ||
                                !result.hasOwnProperty("Message")) {
                                Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);

                            } else if (result.Success === true) {
                                self.addParticipantRow($("option:selected", personSelect).text(), personSelect.val());
                                modalDialog.modal("hide");

                            } else {
                                // Show server-side error message under the
                                // full name field.

                                let validator: JQueryValidation.Validator = modalDialog.validate();
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
            let self = this;
            let modalDialogForm: JQuery = $("form", modalDialog);

            $.validator.unobtrusive.parse(modalDialogForm);

            modalDialogForm.submit(function (event: JQueryEventObject) {
                event.preventDefault();

                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result: any) {
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

                            let validator: JQueryValidation.Validator = modalDialogForm.validate();
                            validator.showErrors({
                                "FullName": result.Message
                            })
                        }
                    }
                });

                return false;
            });
        }

        /**
         * Remove participant from session.
         * @param id Participant ID.
         */
        private deleteParticipant(id: number): void {
            let self = this;

            $.ajax({
                url: self._removeExistingPersonIdUrl,
                type: "DELETE",
                data: {
                    id: id,
                    sessionId: self._sessionId
                },
                success: function (result: any) {
                    if (!result.hasOwnProperty("Success") ||
                        !result.hasOwnProperty("Message")) {
                        Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);

                    } else if (result.Success === true) {
                        $("#participant-" + id).fadeOut(300, function () {
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
            let row: JQuery = $('<div class="row" id="participant-' + id + '"></div>');
            let deleteIcon: JQuery = $('<a href="" class="btn btn-xs btn-danger pull-right" title="Remove"></a>');
            let actionColumn: JQuery;
            let self = this;

            deleteIcon.append('<i class="glyphicon glyphicon-remove"></i>');

            deleteIcon.click((event: JQueryEventObject): void => {
                event.preventDefault();
                self.deleteParticipant(id);
            });

            row.append('<div class="col-lg-10"><div class="form-group">' + fullName + '</div></div>');

            actionColumn = $('<div class="form-group"></div>')
                .append(deleteIcon);

            $('<div class="col-lg-2"></div>')
                .append(actionColumn)
                .appendTo(row);

            row.insertBefore($(".last-row", this._participantsPanelSelector));
        }
    }
}
