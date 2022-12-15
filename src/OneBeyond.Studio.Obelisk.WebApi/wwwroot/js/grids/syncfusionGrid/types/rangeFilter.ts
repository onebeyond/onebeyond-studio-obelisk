import CustomFilter from "./customFilter";
import { NumericTextBox } from "@syncfusion/ej2-inputs";
import { DatePicker } from "@syncfusion/ej2-calendars";

export default class RangeFilter extends CustomFilter {

    constructor(field: string, rangeType: string, dateFormat?: string) {
        super(field);

        switch (rangeType) {
            case 'NumberRange':
                this.instance = new NumericTextBox({
                    placeholder: "Greater than or equal to",
                    popupHeight: '200px',
                } as any);
                this.secondaryInstance = new NumericTextBox({
                    placeholder: "Less than or equal to...",
                    popupHeight: '200px',
                } as any);
                break;
            case "DateRange":
                this.instance = new DatePicker({
                    placeholder: "Greater than or equal to...",
                    format: dateFormat
                });
                this.secondaryInstance = new DatePicker({
                    placeholder: "Less than or equal to...",
                    format: dateFormat
                });
                break;
            default:
                throw new Error("Filter not initialised correctly");
        }
    }

    get getFilterTemplate(): any {
        return {
            ui: {
                create: (args) => {
                    args.getOptrInstance.dropOptr.element.parentElement.parentElement.style.display = "none";

                    let firstInputField = document.createElement('input', { className: 'flm-input' } as ElementCreationOptions);
                    let secondInputField = document.createElement('input', { className: 'flm-input' } as ElementCreationOptions);
                    args.target.appendChild(firstInputField);
                    args.target.appendChild(secondInputField);

                    let currentValues = new Array<any>();
                    if (this.instance.value != null
                        || this.secondaryInstance.value != null) {
                        currentValues.push(this.instance.value);
                        currentValues.push(this.secondaryInstance.value);
                    }
                    this.instance.value = currentValues[0];
                    this.secondaryInstance.value = currentValues[1];

                    this.instance.appendTo(firstInputField);
                    this.secondaryInstance.appendTo(secondInputField);
                },
                write: (args) => { },
                read: (args) => {

                    args.fltrObj.filterSettings.columns = args.fltrObj.filterSettings.columns.filter(x => x.field != args.column.field);

                    let operator = "greaterthanorequal";
                    let value = this.instance.value;

                    if (this.instance.value == null) {
                        operator = "lessthanorequal";
                        value = this.secondaryInstance.value;
                    }

                    args.fltrObj.filterByColumn(args.column.field, operator, value);
                }
            }
        }
    }

    clear(): void {
        this.instance.value = null;
        this.secondaryInstance.value = null;
    }

    public getSecondFilter(): any {
        return {
            actualFilterValue: {},
            actualOperator: {},
            field: this.field,
            ignoreAccent: false,
            isForeignKey: false,
            matchCase: false,
            operator: "lessthanorequal",
            predicate: "and",
            value: this.secondaryInstance.value
        }
    }
}

