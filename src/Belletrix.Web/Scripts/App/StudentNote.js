/// <reference path="..\typings\bootbox\index.d.ts" />
var Belletrix;
(function (Belletrix) {
    var StudentNote = (function () {
        function StudentNote() {
            var self = this;
            $("a.studentnote").on("click", function (e) {
                var anchor = $(this);
                var studentFullName = anchor.attr("data-bt-studentname");
                e.preventDefault();
                $.ajax({
                    url: anchor.attr("href"),
                    cache: false,
                    success: function (data) {
                        var noteModal = bootbox.dialog({
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
        StudentNote.prototype.handleNewNote = function (formSelector, noteSelector) {
            $(formSelector).submit(function (event) {
                var noteElement = $(noteSelector);
                var noteValue = noteElement.val();
                event.preventDefault();
                $.post($(this).attr("action"), $(formSelector).serialize(), function () {
                    var anchor = $('<a href="#" class="list-group-item"></a>');
                    var listGroup = $('<div class="list-group"></div>');
                    var paraNote = $('<p class="list-group-item-text"></p>');
                    var name = Belletrix.Common.UserFirstName + " " + Belletrix.Common.UserLastName;
                    paraNote.text(noteValue);
                    anchor.append('<h4 class="list-group-item-heading">' + name + "</h4>");
                    anchor.append(paraNote);
                    listGroup.append(anchor);
                    listGroup
                        .hide()
                        .insertAfter("div.modal-body div.panel-default")
                        .fadeIn(750);
                    noteElement.val("");
                });
            });
        };
        return StudentNote;
    }());
    Belletrix.StudentNote = StudentNote;
})(Belletrix || (Belletrix = {}));
//# sourceMappingURL=StudentNote.js.map