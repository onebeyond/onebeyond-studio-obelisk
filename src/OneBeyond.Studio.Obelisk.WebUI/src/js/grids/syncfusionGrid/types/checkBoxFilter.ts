import CustomFilter from "./customFilter";
import { MultiSelect } from "@syncfusion/ej2-vue-dropdowns";

export default class CheckboxFilter extends CustomFilter {
    fields: any;
    constructor(field: string, valueFieldName?: string, textFieldName?: string, data?: any) {
        super(field, data);
        this.fields = { value: valueFieldName, text: textFieldName };

        this.instance = this.createInstance();
    }

    get getFilterTemplate(): any {
        return {
            ui: {
                create: (args) => {
                    args.getOptrInstance.dropOptr.element.parentElement.parentElement.style.display = "none";
                    const inputField = document.createElement('input', { className: 'flm-input' } as ElementCreationOptions);
                    args.target.appendChild(inputField);
                    //Set the value for the filter
                    let curVal = new Array<any>();
                    if (this.instance.value != []) {
                        curVal = this.instance.value;
                    }
                    const data = this.instance.dataSource;
                    this.instance = this.createInstance();
                    this.instance.value = curVal;
                    this.instance.dataSource = data;
                    this.instance.appendTo(inputField);
                },
                write: (args) => { },
                read: (args) => {
                    //Required to reset the predicate otherwise it contains previous filter value as well as new one
                    //Filters out stored filtering info for the column we are currently filtering
                    args.fltrObj.filterSettings.columns = args.fltrObj.filterSettings.columns.filter(x => x.field != args.column.field);
                    args.fltrObj.filterByColumn(args.column.field, "equal", this.instance.value);
                }
            }
        }
    }

    private createInstance(): any {
        return new MultiSelect({
            enablePersistance: true,
            fields: this.fields,
            placeholder: 'All',
            popupHeight: '200px',
            allowFiltering: true,
            showSelectAll: true,
            mode: 'CheckBox'
        } as any);    }

    clear(): void {
        this.instance.value = null;
    }
}

