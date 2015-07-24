window.Belletrix = window.Belletrix || {};

(function (ActivityLog, $, undefined) {
    'use strict';

    /// <var type="String">
    /// Selector for panel that contains each each participant line item.
    /// </var>
    var participantsPanelSelector = '#participants-panel',

        participantModalSelector = '#personModal .modal-content',

        /// <var>Selectors for the existing person "form."</var>
        existingPersonSelectors = {
            /// <field type="String">Person selectlist.</field>
            PersonSelectList: '#existingpersonselect',

            /// <field type="String">Type selectlist.</field>
            TypeSelectList: '#existingpersontype',

            /// <field type="String">Submit button to "form."</field>
            SubmitButton: '#existingpersonsubmit'
        };

    function bindExistingParticipantForm(modalDialog, addPersonIdUrl, sessionId) {
        /// <summary>
        /// Handle selecting existing person. When the existing person and
        /// their participation type is selected, the submit button should:
        /// initiate an ajax request to save the selection to the current
        /// activity, add the person and type to the activity form, and
        /// finally close the modal dialog.
        /// </summary>
        /// <param name="modalDialog" type="Object">Modal dialog.</param>
        /// <param name="addPersonIdUrl" type="String">URL to submit existing select person to.</param>
        /// <param name="sessionId" type="Guid">Current activity session ID.</param>

        var personSelect = $(existingPersonSelectors.PersonSelectList),
            typeSelect = $(existingPersonSelectors.TypeSelectList),
            submitButton = $(existingPersonSelectors.SubmitButton);

        // The modal dialog will only show the existing people fields if there
        // are existing people to begin with.
        if (personSelect.length) {
            submitButton.click(function (event) {
                event.preventDefault();

                $.ajax({
                    url: addPersonIdUrl,
                    type: 'POST',
                    data: {
                        id: personSelect.val(),
                        type: typeSelect.val(),
                        sessionId: sessionId
                    },
                    success: function (result) {
                        if (!result.hasOwnProperty('Success') ||
                            !result.hasOwnProperty('Message')) {
                            Belletrix.errorMessage('Unexpected result. Try again?\n' + result);

                        } else if (result.Success === true) {
                            addParticipantRow($('option:selected', personSelect).text(), personSelect.val());
                            $(participantModalSelector).modal('hide');

                        } else {
                            // Show server-side error message under the full name
                            // field.

                            var validator = modalDialogForm.validate();
                            validator.showErrors({
                                "FullName": result.Message
                            })
                        }
                    }
                });
            });
        }
    }

    function bindParticipantForm(modalDialog) {
        /// <summary>
        /// Add jQuery Validator to the new person form. The form should
        /// perform client-side validation and, if successful, submit the data
        /// via ajax, add the person and type to the activity form, and
        /// finally close the modal dialog.
        /// </summary>
        /// <param name="modalDialog" type="Object">Modal dialog.</param>

        var modalDialogForm = $('form', modalDialog);

        $.validator.unobtrusive.parse(modalDialogForm);

        modalDialogForm.submit(function (event) {
            event.preventDefault();

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (!result.hasOwnProperty('Success') ||
                        !result.hasOwnProperty('Message') ||
                        !result.hasOwnProperty('Id')) {
                        Belletrix.errorMessage('Unexpected result. Try again?\n' + result);

                    } else if (result.Success === true) {
                        addParticipantRow($('#FullName', modalDialogForm).val(), result.Id);
                        $(participantModalSelector).modal('hide');

                    } else {
                        // Show server-side error message under the full name
                        // field.

                        var validator = modalDialogForm.validate();
                        validator.showErrors({
                            "FullName": result.Message
                        })
                    }
                }
            });

            return false;
        });
    }

    function deleteParticipant(id) {
        // TODO: Call ajax to delete this person from session.

        console.log('removing ' + id);
        $('#participant-' + id).fadeOut(300, function () {
            $(this).remove();
        });
    }

    function addParticipantRow(fullName, id) {
        /// <summary>
        /// Add a new row to the participant panel for the newly created
        /// person.
        /// </summary>
        /// <param name="fullName" type="String">Person's full name.</param>
        /// <param name="id" type="Number">Internal unique identifier.</param>

        var row = $('<div class="row" id="participant-' + id + '"></div>'),
            deleteIcon = $('<a href="" class="btn btn-xs btn-danger pull-right" title="Remove"></a>'),
            actionColumn;

        deleteIcon.append('<i class="fa fa-remove fa-fw"></i>');

        deleteIcon.click(function (event) {
            event.preventDefault();
            deleteParticipant(id);
        });

        row.append('<div class="col-lg-10"><div class="form-group">' + fullName + '</div></div>');

        actionColumn = $('<div class="form-group"></div>')
            .append(deleteIcon);

        $('<div class="col-lg-2"></div>')
            .append(actionColumn)
            .appendTo(row);

        row.insertBefore($('.last-row', participantsPanelSelector));
    }

    ActivityLog.init = function (addPersonUrl, addPersonIdUrl, sessionId) {
        /// <summary></summary>
        /// <param name="addPersonUrl" type="String"></param>
        /// <param name="addPersonIdUrl" type="String">URL to submit existing select person to.</param>
        /// <param name="sessionId" type="Guid">Current activity session ID.</param>

        Belletrix.handleMvcEditor();
        $('#StartDate, #EndDate').datepicker();
        Belletrix.initMultiselect(1);

        // Autofocus the full name field when the person modal is shown.
        $('#personModal').on('shown.bs.modal', function () {
            $('#FullName').focus();
        });

        $('#addPersonButton').click(function () {
            $(participantModalSelector).load(addPersonUrl, function () {
                $(this).modal({
                    show: true
                });

                bindParticipantForm(this);
                bindExistingParticipantForm(this, addPersonIdUrl, sessionId);
            });
        });
    };
})(window.Belletrix.ActivityLog = window.Belletrix.ActivityLog || {}, jQuery);
