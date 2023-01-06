import CustomFilter from "./customFilter";
import { DropDownList } from "@syncfusion/ej2-vue-dropdowns";

export default class DropdownFilter extends CustomFilter {

    constructor(field: string, valueFieldName?: string, textFieldName?: string, data?: any) {
        super(field, data);
        const fields = { value: valueFieldName, text: textFieldName };

        this.instance = new DropDownList({
            fields: fields,
            placeholder: 'All',
            popupHeight: '200px',
            allowFiltering: true
        } as any);
    }

    get getFilterTemplate(): any {
        return {
            ui: {
                create: (args) => {
                    args.getOptrInstance.dropOptr.element.parentElement.parentElement.style.display = "none";
                    const inputField = document.createElement('input', { className: 'flm-input' } as ElementCreationOptions);
                    args.target.appendChild(inputField);

                    let curVal = null;
                    if (this.instance.value != null) {
                        curVal = this.instance.value;
                    }
                    this.instance.value = curVal;
                    setTimeout(() => {
                        this.instance.appendTo(inputField); //NOTE: required otherwise text is not populated for initial filters
                    }, 1)
                },
                //Write function is not needed for functionality, but syncfusion complains if it is not there
                write: (args) => { },
                read: (args) => {
                    args.fltrObj.filterByColumn(args.column.field, "equal", this.instance.value);
                }
            }
        }
    }

    clear(): void {
        this.instance.value = null;
    }
}