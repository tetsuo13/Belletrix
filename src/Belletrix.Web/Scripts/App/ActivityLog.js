var Belletrix;
(function (Belletrix) {
    var ActivityLog = (function () {
        /**
         * Activity log add/edit initialize routine.
         * @param addPersonUrl
         * @param addPersonIdUrl URL to submit existing select person to.
         * @param removePersonIdUrl URL to remove existing select person from session.
         * @param sessionId Current activity session ID.
         */
        function ActivityLog(addPersonUrl, addPersonIdUrl, removePersonIdUrl, sessionId) {
            /** Selector for panel that contains each each participant line item. */
            this.participantsPanelSelector = "#participants-panel";
            this.participantModalSelector = "#personModal .modal-content";
            /** Selectors for the existing person "form." */
            this.existingPersonSelectors = {
                /// <field type="String">Person selectlist.</field>
                PersonSelectList: "#existingpersonselect",
                /// <field type="String">Type selectlist.</field>
                TypeSelectList: "#existingpersontype",
                /// <field type="String">Submit button to "form."</field>
                SubmitButton: "#existingpersonsubmit"
            };
            /** URL to call to remove existing participant from session. */
            this.removeExistingPersonIdUrl = "";
            /** Current session ID. **/
            this.sessionId = "";
            this.removeExistingPersonIdUrl = removePersonIdUrl;
            this.sessionId = sessionId;
            var self = this;
            Belletrix.Common.handleMvcEditor();
            $("#StartDate, #EndDate").datepicker();
            Belletrix.Common.initMultiselect(1);
            $("#addPersonButton").click(function () {
                $.ajax({
                    url: addPersonUrl,
                    success: function (data) {
                        var addPersonModal = bootbox.dialog({
                            message: data,
                            backdrop: true,
                            title: "Create New Person"
                        });
                        addPersonModal.on("shown.bs.modal", function () {
                            self.bindParticipantForm(addPersonModal);
                            self.bindExistingParticipantForm(addPersonModal, addPersonIdUrl);
                            $("#FullName").focus();
                        });
                    }
                });
            });
        }
        ;
        /**
         * Initializes the back-end session for participants.
         * @param startSessionUrl URL to trigger starting session.
         * @param populateSessionUrl URL to populate existing participants into the session for an existing activity.
         * @param participantsUrl URL to fetch existing participants.
         */
        ActivityLog.prototype.initSession = function (startSessionUrl, populateSessionUrl, participantsUrl) {
            var self = this;
            $.ajax({
                url: startSessionUrl,
                cache: false,
                success: function () {
                    if (populateSessionUrl) {
                        // TODO: Could these actions someday be consolidated into one server-side action perhaps? Hmmm...
                        // Get the server-side to populate session.
                        $.ajax({
                            url: populateSessionUrl,
                            cache: false,
                            success: function () {
                                // Then retrieve participants from session to
                                // populate form.
                                $.ajax({
                                    dataType: "json",
                                    url: participantsUrl,
                                    cache: false,
                                    success: function (data) {
                                        $.each(data, function (i, value) {
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
        ;
        /**
         * Handle selecting existing person. When the existing person and
         * their participation type is selected, the submit button should:
         * initiate an ajax request to save the selection to the current
         * activity, add the person and type to the activity form, and finally
         * close the modal dialog.
         * @param modalDialog Modal dialog.
         * @param addPersonIdUrl URL to submit existing select person to.
         */
        ActivityLog.prototype.bindExistingParticipantForm = function (modalDialog, addPersonIdUrl) {
            var personSelect = $(this.existingPersonSelectors.PersonSelectList);
            var typeSelect = $(this.existingPersonSelectors.TypeSelectList);
            var submitButton = $(this.existingPersonSelectors.SubmitButton);
            // The modal dialog will only show the existing people fields if
            // there are existing people to begin with.
            if (personSelect.length) {
                var self_1 = this;
                submitButton.click(function (event) {
                    event.preventDefault();
                    $.ajax({
                        url: addPersonIdUrl,
                        type: "POST",
                        data: {
                            id: personSelect.val(),
                            type: typeSelect.val(),
                            sessionId: self_1.sessionId
                        },
                        success: function (result) {
                            if (!result.hasOwnProperty("Success") ||
                                !result.hasOwnProperty("Message")) {
                                Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);
                            }
                            else if (result.Success === true) {
                                self_1.addParticipantRow($("option:selected", personSelect).text(), personSelect.val());
                                modalDialog.modal("hide");
                            }
                            else {
                                // Show server-side error message under the
                                // full name field.
                                var validator = modalDialog.validate();
                                validator.showErrors({
                                    "FullName": result.Message
                                });
                            }
                        }
                    });
                });
            }
        };
        /**
         * Add jQuery Validator to the new person form. The form should
         * perform client-side validation and, if successful, submit the data
         * via ajax, add the person and type to the activity form, and finally
         * close the modal dialog.
         * @param modalDialog Modal dialog.
         */
        ActivityLog.prototype.bindParticipantForm = function (modalDialog) {
            var self = this;
            var modalDialogForm = $("form", modalDialog);
            $.validator.unobtrusive.parse(modalDialogForm);
            modalDialogForm.submit(function (event) {
                event.preventDefault();
                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result) {
                        if (!result.hasOwnProperty("Success") ||
                            !result.hasOwnProperty("Message") ||
                            !result.hasOwnProperty("Id")) {
                            Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);
                        }
                        else if (result.Success === true) {
                            self.addParticipantRow($("#FullName", modalDialogForm).val(), result.Id);
                            modalDialog.modal("hide");
                        }
                        else {
                            // Show server-side error message under the full
                            // name field.
                            var validator = modalDialogForm.validate();
                            validator.showErrors({
                                "FullName": result.Message
                            });
                        }
                    }
                });
            });
        };
        /**
         * Remove participant from session.
         * @param id Participant ID.
         */
        ActivityLog.prototype.deleteParticipant = function (id) {
            var self = this;
            $.ajax({
                url: self.removeExistingPersonIdUrl,
                type: "DELETE",
                data: {
                    id: id,
                    sessionId: self.sessionId
                },
                success: function (result) {
                    if (!result.hasOwnProperty("Success") ||
                        !result.hasOwnProperty("Message")) {
                        Belletrix.Common.errorMessage("Unexpected result. Try again?\n" + result);
                    }
                    else if (result.Success === true) {
                        $("#participant-" + id).fadeOut(300, function () {
                            $(this).remove();
                        });
                    }
                    else {
                        Belletrix.Common.errorMessage(result.Message);
                    }
                }
            });
        };
        /**
         * Add a new row to the participant panel for the newly created person.
         * @param fullName Person's full name.
         * @param id Internal unique identifier.
         */
        ActivityLog.prototype.addParticipantRow = function (fullName, id) {
            var row = $('<div class="row" id="participant-' + id + '"></div>');
            var deleteIcon = $('<a href="" class="btn btn-xs btn-danger pull-right" title="Remove"></a>');
            var actionColumn;
            var self = this;
            deleteIcon.append('<i class="glyphicon glyphicon-remove"></i>');
            deleteIcon.click(function (event) {
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
        };
        /**
         * Display partial view beneath the activity log title showing an
         * existing activity log by the same title if found.
         * @param titleSelector Title input selector.
         * @param resultSelector Selector for container to display any duplicate info.
         * @param titleCheckUrl URL to send title to.
         */
        ActivityLog.prototype.initTitleCheck = function (titleSelector, resultSelector, titleCheckUrl) {
            $(titleSelector).blur(function () {
                var title = $(this).val();
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
                    success: function (result) {
                        var uniqueNameContainer = $(resultSelector).empty();
                        if (result.trim().length > 0) {
                            uniqueNameContainer.html(result);
                        }
                    },
                    error: function (jqXHR, textStatus) {
                        var message = "<p>An unknown error occurred while checking title.</p>" +
                            "<p>" + textStatus + "</p>";
                        Belletrix.Common.errorMessage(message);
                    }
                });
            });
        };
        return ActivityLog;
    }());
    Belletrix.ActivityLog = ActivityLog;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=ActivityLog.js.map