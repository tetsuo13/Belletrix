/// <reference path="..\typings\jquery\jquery.d.ts"/>
/// <reference path="..\typings\bootstrap.datepicker\bootstrap.datepicker.d.ts" />

module Belletrix {
    export class Experience {
        public initAdd(): void {
            Belletrix.Common.handleMvcEditor();
            Belletrix.Common.initMultiselect(1);

            $('#StartDate, #EndDate').datepicker();
        }

        public bindDelete(deleteModalSelector: string, deleteUrl: string, dataString: string): void {
            Belletrix.Common.handleDeleteModal(deleteModalSelector, deleteUrl, dataString);
        }
    }
}
