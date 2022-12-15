import { CustomInlineEdit } from "./customInlineEdit";
import { DateRangePicker } from "@syncfusion/ej2-calendars";

export default class DateRangeEdit extends CustomInlineEdit {
    minDate?: Date;
    maxDate?: Date;

    constructor(field: string, editHandler: any, minDate?: Date, maxDate?: Date) {
        super(field, editHandler);
        if (minDate != null)
            this.minDate = minDate;
        if (maxDate != null)
            this.maxDate = maxDate;
    }

    get getEditTemplate(): any {
        return {
            create: (args) => {
                this.element = document.createElement('input');
                this.instance = new DateRangePicker();
                if (this.minDate != null)
                    this.instance.min = this.minDate;
                if (this.maxDate != null)
                    this.instance.max = this.maxDate;
                this.instance.value = args.value;
                this.instance.input = (args) => { this.editHandler(args, this.field); };
                return this.element;
            },
            read: (args) => {
                return args.value;
            },
            write: (args) => {
                this.instance.appendTo(this.element);
            }
        };
    }

    clear(): void {
        this.instance.value = null;
    }
}