﻿/// <reference path="..\typings\bootbox\bootbox.d.ts" />

module Belletrix {
    export class StudentNote {
        constructor() {
            let self = this;

            $("a.studentnote").on("click", function (e: JQueryEventObject): void {
                const anchor: JQuery = $(this);
                const studentFullName: string = anchor.attr("data-bt-studentname");

                e.preventDefault();

                $.ajax({
                    url: anchor.attr("href"),
                    cache: false,
                    success: (data: string) => {
                        let noteModal: JQuery = bootbox.dialog({
                            message: data,
                            //onEscape: true,
                            backdrop: true,
                            title: "Notes for " + studentFullName
                        });

                        noteModal.on("shown.bs.modal", function (): void {
                            $("#newnotebutton").click(function (): void {
                                $("#newnote").submit();
                            });

                            self.handleNewNote("#newnote", "textarea#Note");
                            Belletrix.Common.singleSubmit();
                        });
                    }
                });
            });
        }

        private handleNewNote(formSelector: string, noteSelector: string): void {
            $(formSelector).submit(function (event) {
                let noteElement: JQuery = $(noteSelector);
                const noteValue: string = noteElement.val();

                event.preventDefault();

                $.post($(this).attr("action"),
                    $(formSelector).serialize(),
                    function (): void {
                        let anchor: JQuery = $('<a href="#" class="list-group-item"></a>');
                        let listGroup: JQuery = $('<div class="list-group"></div>');
                        let paraNote: JQuery = $('<p class="list-group-item-text"></p>');
                        let name: string = Belletrix.Common.UserFirstName + " " + Belletrix.Common.UserLastName;

                        paraNote.text(noteValue);

                        anchor.append('<h4 class="list-group-item-heading">' + name + "</h4>");
                        anchor.append(paraNote);

                        listGroup.append(anchor);
                        listGroup
                            .hide()
                            .insertAfter("div.modal-body div.panel-default")
                            .fadeIn(750);

                        noteElement.val("");
                    }
                );
            });
        }
    }
}
