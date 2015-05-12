/// <reference path="Common.ts" />
/// <reference path="..\typings\jquery\jquery.d.ts" />

'use strict';

module Belletrix {
    export class StudentNote {
        /**
        * Initialize Bootstrap Modal for note popup.
        */
        public initModal(): void {
            // Load cached modal content and then refetch remote content.
            $(document.body).on('hidden.bs.modal', function () {
                $('#noteModal').removeData('bs.modal');
            });
        }

        /**
        * Initialize note form.
        *
        * @param formSelector
        * @param noteSelector
        * @param userFirstName
        * @param userLastName
        */
        public handleNewNote(formSelector: string, noteSelector: string, userFirstName: string,
            userLastName: string): void {

            $(formSelector).submit(function (event: JQueryEventObject) {
                var noteElement: JQuery = $(noteSelector),
                    noteValue: string = noteElement.val();

                event.preventDefault();

                $.post($(this).attr('action'),
                    $(formSelector).serialize(),
                    function () {
                        var anchor: JQuery = $('<a href="#" class="list-group-item"></a>'),
                            listGroup: JQuery = $('<div class="list-group"></div>'),
                            paraNote: JQuery = $('<p class="list-group-item-text"></p>');

                        paraNote.text(noteValue);

                        anchor.append('<h4 class="list-group-item-heading">' + userFirstName + ' ' + userLastName + '</h4>');
                        anchor.append(paraNote);

                        listGroup.append(anchor);
                        listGroup
                            .hide()
                            .insertAfter('div.modal-body div.panel-default')
                            .fadeIn(750);

                        noteElement.val('');
                    });
            });
        }
    }
}
 