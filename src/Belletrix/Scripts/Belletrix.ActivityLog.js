window.Belletrix = window.Belletrix || {};

(function (ActivityLog, $, undefined) {
    'use strict';

    /// <var type="String">
    /// Selector for panel that contains each each participant line item.
    /// </var>
    var participantsPanelSelector = '#participants-panel',

        participantModalSelector = '#personModal .modal-content';

    function bindParticipantForm(modalDialog) {
        /// <summary></summary>
        /// <param name="modalDialog" type="Object"></param>

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
        // TODO: Call WebAPI to delete this person.

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

    ActivityLog.init = function (addPersonUrl) {
        /// <summary></summary>
        /// <param name="addPersonUrl" type="String"></param>

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
            });
        });
    };
})(window.Belletrix.ActivityLog = window.Belletrix.ActivityLog || {}, jQuery);
