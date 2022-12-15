import CustomFilter from "./customFilter";
import { ColumnFieldType, InlineEditType, CustomFilterType, ColumnParams, ColumnFreezePositionOptions } from "./types";
import DropDownFilter from "./dropDownFilter";
import CheckBoxFilter from "./checkBoxFilter";
import RangeFilter from "./rangeFilter";
import { CustomInlineEdit } from "./customInlineEdit";
import TextBoxEdit from "./textBoxEdit";
import DropDownEdit from "./dropDownEdit";
import DateTimePickerEdit from "./dateTimePickerEdit";
import DateRangeEdit from "./dateRangeEdit";

export default class SyncfusionGridColumn {
    allowFiltering?: boolean = true;
    allowSorting?: boolean = true;
    allowEditing?: boolean = false;
    isVisible?: boolean = true;
    customFilterType?: CustomFilterType;
    excelFormatter?: any;
    editData?: Array<any>;
    format?: string | undefined;
    displayAsCheckBox?: boolean = false;
    fieldName!: string;
    fieldType?: ColumnFieldType = "string";
    filter?: CustomFilter;
    filterSettings?: any;
    freezePosition?: ColumnFreezePositionOptions;
    getColumnTemplate?: any = null;
    headerText?: string;
    inlineEdit?: CustomInlineEdit;
    inlineEditType?: InlineEditType;
    inlineEditSettings?: any;
    isPrimaryKey?: boolean;
    lookupValue?: string = "value";
    lookupText?: string = "name";
    sortField?: any;
    template?: any;
    valueAccessor?: any;
    validationRules?: any;
    defaultValue?: string;
    multiline: boolean = false;
    minDate?: Date;
    maxDate?: Date;

    constructor(fields: ColumnParams) {
        Object.assign(this, fields);
        switch (fields.customFilterType) {
            case 'DropDown':
                this.filter = new DropDownFilter(fields.fieldName!, fields.lookupValue, fields.lookupText, fields.editData)
                break;
            case 'CheckBox':
                this.filter = new CheckBoxFilter(fields.fieldName!, fields.lookupValue, fields.lookupText, fields.editData);
                break;
            case 'NumberRange':
            case 'DateRange':
                this.filter = new RangeFilter(fields.fieldName!, fields.customFilterType, this.format);
                break;
            default:
                break;
        }

        if (this.template) {
            this.getColumnTemplate = () => this.getTemplate();
        }
    }

    public setInlineEdit(inlineEditHandler: any): void {
        let editType = this.inlineEditType == undefined ? 'stringedit' : this.inlineEditType;
        switch (editType) {
            case 'dropdownedit':
                this.inlineEdit = new DropDownEdit(this.fieldName, inlineEditHandler, this.lookupValue, this.lookupText, this.editData);
                this.inlineEditSettings = this.inlineEdit.getEditTemplate;
                break;
            case "datepickeredit":
                this.inlineEditSettings = {
                    params: { change: inlineEditHandler, format: this.format, min: this.minDate, max: this.maxDate }
                }
                break;
            case "datetimepickeredit":
                this.inlineEdit = new DateTimePickerEdit(this.fieldName, inlineEditHandler, this.minDate, this.maxDate);
                this.inlineEditSettings = this.inlineEdit.getEditTemplate;
                break;
            case "daterangepickeredit":
                this.inlineEdit = new DateRangeEdit(this.fieldName, inlineEditHandler, this.minDate, this.maxDate);
                this.inlineEditSettings = this.inlineEdit.getEditTemplate;
                break;
            case "booleanedit":
                this.inlineEditSettings = {
                    params: { change: inlineEditHandler }
                };
                break;
            case 'stringedit':
            default:
                this.inlineEdit = new TextBoxEdit(this.fieldName, inlineEditHandler, this.multiline);
                this.inlineEditSettings = this.inlineEdit.getEditTemplate;
                break;
        }
    }

    public getTemplate(): any {
        return {
            template: this.template
        }
    }
}