import { CustomInlineEdit } from "./customInlineEdit";
import { TextBox } from "@syncfusion/ej2-inputs";

export default class TextBoxEdit extends CustomInlineEdit {
    multiline: boolean;

    constructor(field: string, editHandler: any, multiline: boolean) {
        super(field, editHandler);
        this.multiline = multiline;
    }

    get getEditTemplate(): any {
        return {
            create: (args) => {
                this.element = document.createElement('input');
                this.instance = new TextBox();
                this.instance.multiline = this.multiline;
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