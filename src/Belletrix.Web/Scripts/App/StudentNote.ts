/// <reference path="..\typings\bootbox\bootbox.d.ts" />

module Belletrix {
    export class StudentNote {
        constructor() {
            let self = this;

            $("a.studentnote").click(function (e) {
                let anchor: JQuery = $(this);
                let studentFullName: string = anchor.attr("data-bt-studentname");

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

                        noteModal.on("shown.bs.modal", function () {
                            $("#newnotebutton").click(function () {
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
                let noteValue: string = noteElement.val();

                event.preventDefault();

                $.post($(this).attr("action"),
                    $(formSelector).serialize(),
                    function () {
                        let anchor: JQuery = $('<a href="#" class="list-group-item"></a>');
                        let listGroup: JQuery = $('<div class="list-group"></div>');
                        let paraNote: JQuery = $('<p class="list-group-item-text"></p>');
                        let name: string = Belletrix.Common.UserFirstName + ' ' + Belletrix.Common.UserLastName;

                        paraNote.text(noteValue);

                        anchor.append('<h4 class="list-group-item-heading">' + name + '</h4>');
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
