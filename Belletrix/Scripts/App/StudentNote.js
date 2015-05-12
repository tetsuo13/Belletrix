/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />
'use strict';
var Belletrix;
(function (Belletrix) {
    var StudentNote = (function () {
        function StudentNote() {
        }
        /**
        * Initialize Bootstrap Modal for note popup.
        */
        StudentNote.prototype.initModal = function () {
            // Load cached modal content and then refetch remote content.
            $(document.body).on('hidden.bs.modal', function () {
                $('#noteModal').removeData('bs.modal');
            });
        };
        /**
        * Initialize note form.
        *
        * @param formSelector
        * @param noteSelector
        * @param userFirstName
        * @param userLastName
        */
        StudentNote.prototype.handleNewNote = function (formSelector, noteSelector, userFirstName, userLastName) {
            $(formSelector).submit(function (event) {
                var noteElement = $(noteSelector), noteValue = noteElement.val();
                event.preventDefault();
                $.post($(this).attr('action'), $(formSelector).serialize(), function () {
                    var anchor = $('<a href="#" class="list-group-item"></a>'), listGroup = $('<div class="list-group"></div>'), paraNote = $('<p class="list-group-item-text"></p>');
                    paraNote.text(noteValue);
                    anchor.append('<h4 class="list-group-item-heading">' + userFirstName + ' ' + userLastName + '</h4>');
                    anchor.append(paraNote);
                    listGroup.append(anchor);
                    listGroup.hide().insertAfter('div.modal-body div.panel-default').fadeIn(750);
                    noteElement.val('');
                });
            });
        };
        return StudentNote;
    })();
    Belletrix.StudentNote = StudentNote;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=StudentNote.js.map