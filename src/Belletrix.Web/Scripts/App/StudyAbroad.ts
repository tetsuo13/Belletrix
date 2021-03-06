﻿/// <reference path="..\typings\jquery\jquery.d.ts"/>
/// <reference path="..\typings\bootstrap.datepicker\bootstrap.datepicker.d.ts" />

namespace Belletrix {
    export class StudyAbroad {
        public initAddEdit(): void {
            Belletrix.Common.handleMvcEditor();
            Belletrix.Common.initMultiselect(1);

            $("#StartDate, #EndDate").datepicker();
        }

        public bindDelete(deleteUrl: string, dataString: string): void {
            $("button.studyabroad-list-delete").click(function (event: JQueryEventObject): void {
                const studyAbroadId: number = parseInt($(this).data(dataString));

                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result: boolean): void {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, studyAbroadId, function (): void {
                                window.location.reload();
                            });
                        }
                    }
                });
            });
        }
    }
}
