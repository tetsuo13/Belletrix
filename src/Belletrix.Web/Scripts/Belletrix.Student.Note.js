window.Belletrix = window.Belletrix || {};
window.Belletrix.Student = window.Belletrix.Student || {};

(function (Note, $, undefined) {
    'use strict';

    Note.initLinks = function () {
        $('a.studentnote').click(function (e) {
            var anchor = $(this),
                studentFullName = anchor.attr('data-bt-studentname');

            e.preventDefault();

            $.ajax({
                url: anchor.attr('href'),
                cache: false,
                success: function (data) {
                    var noteModal;
                    
                    noteModal = bootbox.dialog({
                        message: data,
                        onEscape: true,
                        backdrop: true,
                        title: 'Notes for ' + studentFullName
                    });

                    noteModal.on('shown.bs.modal', function () {
                        $('#newnotebutton').click(function () {
                            $('#newnote').submit();
                        });

                        handleNewNote('#newnote', 'textarea#Note');
                        Belletrix.Common.singleSubmit();
                    });
                }
            });
        });
    };

    function handleNewNote(formSelector, noteSelector) {
        /// <summary></summary>
        /// <param name="formSelector" type="String"></param>
        /// <param name="noteSelector" type="String"></param>

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

                    anchor.append('<h4 class="list-group-item-heading">' + Belletrix.UserFirstName + ' ' + Belletrix.UserLastName + '</h4>');
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
    }
})(window.Belletrix.Student.Note = window.Belletrix.Student.Note || {}, jQuery);
