import { DropDownList } from "@syncfusion/ej2-vue-dropdowns";
import { CustomInlineEdit } from "./customInlineEdit";
import { DataManager } from "@syncfusion/ej2-data";

export default class DropDownEdit extends CustomInlineEdit {

    constructor(field: string, editHandler: any, valueFieldName?: string, textFieldName?: string, data?: any) {
        super(field, editHandler, data);
        this.fields = { value: valueFieldName, text: textFieldName };
    }

    get getEditTemplate(): any {
        return {
            create: (args) => {
                this.element = document.createElement('input');
                this.instance = new DropDownList({
                    dataSource: new DataManager(this.data),
                    fields: this.fields,
                    placeholder: 'Select'
                });
                this.instance.value = args.data[this.field];
                this.instance.change = (args) => { this.editHandler(args, this.field); };
                return this.element;
            },
            read: (args) => {
                return args.value;
            },
            write: (_) => {
                this.instance.appendTo(this.element);
            }
        };
    }
}
