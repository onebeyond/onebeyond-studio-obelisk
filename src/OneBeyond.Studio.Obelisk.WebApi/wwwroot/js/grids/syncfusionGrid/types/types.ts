import CustomFilter from "./customFilter";
import { CustomInlineEdit } from "./customInlineEdit";

export type ColumnFieldType = 'number' | 'string' | 'boolean' | 'date';
export type InlineEditType = 'dropdownedit' | 'datepickeredit' | 'datetimepickeredit' | 'daterangepickeredit' | 'numericedit' | 'stringedit' | 'booleanedit';
export type CustomFilterType = 'NumberRange' | 'DateRange' | 'DropDown' | 'CheckBox';
export type FilterOperator = 'equal' | 'notequal' | 'startswith' | 'endswith' | 'contains' | 'greaterthanorequal' | 'lessthanorequal';
export type ColumnFreezePositionOptions = 'Left' | 'Right';
export type CrudAction = (id: any) => Promise<void>;

export interface SyncfusionGrid {
    data: Array<any>;
    init(): void;
    exportToExcel;
    page;
    setPage(page: number): void;
}

export enum EntityGridAction {
    EntityAdd = 1,
    EntityEdit = 2,
    EntityDelete = 3
}

export enum Command {
    Add = "Add",
    Edit = "Edit",
    Delete = "Delete",
    Details = "Details"
}

export enum Action {
    Add = "add",
    Edit = "edit",
    BatchSave = "batchsave",
    Filter = "filter",
    Filtering = "filtering",
    Cancel = "cancel",
    ClearFilter = "clearFilter"
}

export enum SortDirection {
    Asc = "Ascending",
    Desc = "Descending"
}

export interface ColumnParams {
    allowFiltering?: boolean | undefined;
    allowSorting?: boolean | undefined;
    allowEditing?: boolean | undefined;
    isVisible?: boolean | undefined;
    customFilterType?: CustomFilterType;
    customAttributes?: any;
    format?: string | undefined;
    displayAsCheckBox?: boolean;
    decimalPoints?: number;
    editData?: Array<any>; //Preset inline edit data
    excelFormatter?: any;
    fieldName: string;
    fieldType?: ColumnFieldType;
    filter?: CustomFilter;
    filterSettings?: any;
    freezePosition?: ColumnFreezePositionOptions;
    headerText?: string;
    inlineEdit?: CustomInlineEdit;
    inlineEditType?: InlineEditType;
    inlineEditSettings?: any;
    isPrimaryKey?: boolean;
    lookupValue?: string; //FK Fields in edit or filter
    lookupText?: string; //FK Fields in edit or filter
    sortField?: any;
    template?: any;
    truncateCellText?: boolean;
    valueAccessor?: any;
    validationRules?: any;
    width?: string | number;
    defaultValue?: string;
    multiline?: boolean;
    minDate?: Date;
    maxDate?: Date;
}

export interface SortParams {
    field: string;
    direction: SortDirection;
}