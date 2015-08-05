window.Belletrix = window.Belletrix || {};
window.Belletrix.Student = window.Belletrix.Student || {};

(function (Note, $, undefined) {
    'use strict';

    Note.initModal = function () {
        // Load cached modal content and then refetch remote content.
        $(document.body).on('hidden.bs.modal', function () {
            $('#noteModal').removeData('bs.modal');
        });
    };

    Note.handleNewNote = function (formSelector, noteSelector,
        userFirstName, userLastName) {
        /// <summary></summary>
        /// <param name="formSelector" type="String"></param>
        /// <param name="noteSelector" type="String"></param>
        /// <param name="userFirstName" type="String"></param>
        /// <param name="userLastName" type="String"></param>

        $(formSelector).submit(function (event) {
            var noteElement = $(noteSelector),
                noteValue = noteElement.val();

            event.preventDefault();

            $.post($(this).attr('action'),
                $(formSelector).serialize(),
                function () {
                    var anchor = $('<a href="#" class="list-group-item"></a>'),
                        listGroup = $('<div class="list-group"></div>'),
                        paraNote = $('<p class="list-group-item-text"></p>');

                    paraNote.text(noteValue);

                    anchor.append('<h4 class="list-group-item-heading">' + userFirstName + ' ' + userLastName + '</h4>');
                    anchor.append(paraNote);

                    listGroup.append(anchor);
                    listGroup
                        .hide()
                        .insertAfter('div.modal-body div.panel-default')
                        .fadeIn(750);

                    noteElement.val('');
                }
            );
        });
    };
})(window.Belletrix.Student.Note = window.Belletrix.Student.Note || {}, jQuery);
