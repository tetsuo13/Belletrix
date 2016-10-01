/// <reference path="..\typings\jquery\jquery.d.ts"/>
/// <reference path="..\typings\bootstrap.datepicker\bootstrap.datepicker.d.ts" />

module Belletrix {
    export class Experience {
        public initAdd(): void {
            Belletrix.Common.handleMvcEditor();
            Belletrix.Common.initMultiselect(1);

            $('#StartDate, #EndDate').datepicker();
        }

        public bindDelete(deleteUrl: string, dataString: string): void {
            $("button.experience-list-delete").click(function (event: JQueryEventObject): void {
                let experienceId: number = parseInt($(this).data(dataString));

                bootbox.confirm({
                    size: "small",
                    message: "Are you sure?",
                    callback: function (result: boolean): void {
                        if (result) {
                            Belletrix.Common.handleDeleteCall(deleteUrl, experienceId, function (): void {
                                window.location.reload();
                            });
                        }
                    }
                });
            });
        }
    }
}
