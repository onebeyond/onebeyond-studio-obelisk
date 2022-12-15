import { EntityGridAction, EntityGrid } from '../entityGrid';
import { DataStateChangeEventArgs, DataSourceChangedEventArgs, FilterType, RowDragEventArgs, RowDropEventArgs, Sorts, RowDataBoundEventArgs } from '@syncfusion/ej2-vue-grids';
import { DataAdaptor } from './dataAdaptor'
import SyncfusionGridColumn from "./types/syncfusionGridColumn";
import { CrudAction, SyncfusionGrid, Command, Action, ColumnParams, SortParams, ColumnFreezePositionOptions } from './types/types';
import { CommandColumnButton, CommandColumnButtonOptions } from './types/commandColumnButton';
import { FilterOption } from './types/filterOption';

export class SyncfusionEntityGrid extends EntityGrid {

    //#region SETTINGS
    public editSettings: any = {
        allowEditing: true
    };

    public pageSettings: any = {
        currentPage: 1,
        pageSize: 10,
        pageSizes: [10, 25, 50, 100]
    };

    public sortSettings: any = {
        columns: []
    }

    public searchSettings = { key: "", fields: [], operator: "contains", ignoreCase: true, ignoreAccent: false };

    //These are the defaults that we support
    //Provide type for each (string, date, bool, number) and allow override from limited selection based on type in case where not all operators are needed?
    public readonly filterSettings: any = {
        columns: [],
        type: "Menu",
        operators: {
            stringOperator:
                [new FilterOption('equal', 'Equals'),
                new FilterOption('startswith', 'Starts With'),
                new FilterOption('endswith', 'Ends With'),
                new FilterOption('contains', 'Contains'),
                new FilterOption('notequal', 'Not')],
            dateOperator:
                [new FilterOption('equal', 'Equals'),
                new FilterOption('greaterthanorequal', 'Greater than or equal'),
                new FilterOption('lessthanorequal', 'Less than or equal')],
            numberOperator:
                [new FilterOption('equal', 'Equals'),
                new FilterOption('greaterthanorequal', 'Greater than or equal'),
                new FilterOption('lessthanorequal', 'Less than or equal')],
        }
    }

    private _currentPage: number;
    private _sortMaps: any = {}; //Passed to data adaptor to sort a field by another (id/name)
    protected _instance: SyncfusionGrid | null;
    private _dataAdaptor: DataAdaptor | null;

    public id = "";
    public state: DataStateChangeEventArgs = { skip: 0, take: this.pageSettings.pageSize, sorted: [] }
    public toolbarOptions: Array<any> = [];

    public gridEntity: any = {}; //Inline edit row data is mapped to this object
    public data: any = {}; //Grid data
    public commands: Array<CommandColumnButton> = [];
    public columns: Array<SyncfusionGridColumn> = [];
    public excelFormatters: Array<any> = [];

    public handleEdit: CrudAction = () => new Promise<void>(resolve => resolve());
    public handleInlineEdit: CrudAction = () => new Promise<void>(resolve => resolve());
    public handleDelete: any = () => { return };
    public handleViewDetails: any = () => { return };

    public actionsWidth: number | null = null;
    public columnBreakpointCss = "(min-width: 800px)";
    public mobileViewBreakpointCss = "(max-width: 800px)";
    public mobileTemplate: any | null = null;
    public detailTemplate: any | null = null;
    public getDetailTemplate: any = null; //must be null or func that returns {template: template }
    public getMobileTemplate: any = null;

    //on/off controls for particular features
    public isRowDragAndDropEnabled = false;
    public isCommandColumnVisible = true;
    public commandColumnFreezePosition: string | undefined;

    public isContextMenuColumnVisible = false;
    public contextMenuColumnFreezePosition: string | undefined;

    public isMobileColumnVisible = true;
    public isPersistenceSupported = false;

    //Selection column config
    public hasSelectedItems = false;
    public selectionColumnFreezePosition: string | undefined;
    public isSelectionColumnVisible = false;

    public selectionSettings: any = {
        persistSelection: true,
        checkboxOnly: true
    }

    //#endregion

    constructor() {
        super();
        this._instance = null;
        this._dataAdaptor = null;
        this._currentPage = 1;
    }

    public setInstance(gridRef: any) {
        this._instance = gridRef;
    }

    public initDataAdaptor(apiUrl: string, errorCallback: Function): void {
        this._dataAdaptor = new DataAdaptor(apiUrl, errorCallback, this._sortMaps);
    }

    public init(id: string, apiUrl: string, errorCallback: Function, parentId?: string): void {
        this.id = id;
        this.initDataAdaptor(apiUrl, errorCallback);

        if (parentId) {
            this.addParameter({ parentId: parentId });
        }
    }

    //Query API for grid data
    public refresh(): void {
        this.onDataStateChanged(this.state);
    }

    public addParameter(newParameter: any): void {
        if (this._dataAdaptor) {
            this._dataAdaptor.addSingleParameter(newParameter);
        }
    }

    //#region INITIALISATION
    //Set up of primary functionality of grid

    public setErrorHandler(errorHandler: any): SyncfusionEntityGrid {
        this.handleServerError = errorHandler
        return this;
    }

    //Use an empty array to search on all fields
    public useGlobalSearch(fieldNames): SyncfusionEntityGrid {
        this.toolbarOptions.push('Search');
        this.searchSettings.fields = fieldNames;
        return this;
    }

    public useSelectionColumn(): SyncfusionEntityGrid {
        this.isSelectionColumnVisible = true;
        return this;
    }

    //Disables inline edit
    public disableEdit(): SyncfusionEntityGrid {
        this.editSettings = { allowEditing: false };
        return this;
    }

    public setFilterType(filterType: FilterType): SyncfusionEntityGrid {
        this.filterSettings.type = filterType
        return this;
    }

    public setMobileOptions(mobileTemplate: any, mobileViewBreakpointCss: string, columnBreakpointCss): SyncfusionEntityGrid {
        this.mobileTemplate = mobileTemplate;
        this.getMobileTemplate = () => this.mobileTemplateFunction();
        this.mobileViewBreakpointCss = mobileViewBreakpointCss;
        this.columnBreakpointCss = columnBreakpointCss;
        return this;
    }

    public useDetailTemplate(detailTemplate: any): SyncfusionEntityGrid {
        this.detailTemplate = detailTemplate;
        this.getDetailTemplate = () => this.detailTemplateFunction();
        return this;
    }

    //For now only equals operator is used (e.g for dropdowns/enums/booleans)
    public setInitialFilters(): SyncfusionEntityGrid {
        const presetFilterColumns = this.columns.filter(x => x.filter && x.filter!.initialValue != undefined);

        for (let column of presetFilterColumns) {
            column.filter!.instance.value = column.filter!.initialValue;
            column.filter!.instance.text = "";
            this.filterSettings.columns.push({ field: column.fieldName, matchCase: false, operator: 'equal', predicate: 'and', value: column.filter!.initialValue });
        }

        this.state.where = this.constructWherePredicate(this.filterSettings.columns);
        return this;
    }

    public setInitialSorting(field: string, direction: string): SyncfusionEntityGrid {
        if (!this.state.sorted!.find(x => x.name === field)) {
            this.state.sorted!.push({ name: field, direction: direction });
            this.sortSettings['columns'].push({ field: field, direction: direction });
        }
        return this;
    }

    // Note! First column sorted is the last in the array on SF implementation dataAdaptor.ts
    public setInitialSortingMultiple(sortParams: SortParams[]): SyncfusionEntityGrid {
        const sorts = [...sortParams].reverse();
        sorts.map(s => {
            this.state.sorted!.push({ name: s.field, direction: s.direction });
        })
        // need to reverse otherwise the number indicators on columns will be wrong
        this.sortSettings['columns'].push(sorts);

        return this;
    }

    public setPageSize(size: number): SyncfusionEntityGrid {
        this.pageSettings.pageSize = size;
        this.state = { skip: 0, take: size, sorted: [] }
        return this;
    }

    public addColumn({ ...columnArgs }: ColumnParams): SyncfusionEntityGrid {
        const column = new SyncfusionGridColumn(columnArgs);

        //required for default filter type e.g menu etc
        if (column.filter == undefined) {
            column.filterSettings = this.filterSettings;
        }
        else {
            column.filterSettings = column.filter.getFilterTemplate;
        }

        //Allows id val to be sorted on name for example
        if (column.sortField) {
            this._sortMaps[column.fieldName] = column.sortField;
        }

        //Context of 'this' must be kept for edit function as we edit rowData (entity) stored in this class
        if (column.allowEditing) {
            column.setInlineEdit((args) => this.editGridEntity(args, column.fieldName));
        }

        this.columns.push(column);
        return this;
    }

    public enableRowDragDrop(handleRowDropFunction: (args: RowDropEventArgs) => void): SyncfusionEntityGrid {
        this.isRowDragAndDropEnabled = true;
        this.onRowDropped = handleRowDropFunction;
        return this;
    }

    //NOTE: Context menu will not work correctly if:
    //Using drag and drop and freezing columns
    //Using selection column and not freezing BOTH Selection col and Context col
    public showContextMenuColumn(freezePosition?: ColumnFreezePositionOptions): SyncfusionEntityGrid {
        this.isCommandColumnVisible = false;
        this.isContextMenuColumnVisible = true;
        this.contextMenuColumnFreezePosition = freezePosition;
        if (this.contextMenuColumnFreezePosition && this.isSelectionColumnVisible) {
            this.selectionColumnFreezePosition = freezePosition;
        }
        return this;
    }

    public hideMobileColumn(): SyncfusionEntityGrid {
        if (this._instance) {
            this.hideColumnByFieldName("Mobile")
        }
        else {
            this.isMobileColumnVisible = false;
        }

        return this;
    }

    public hideCommandColumn(): SyncfusionEntityGrid {
        if (this._instance) {
            this.hideColumnByFieldName("Actions")
        }
        else {
            this.isCommandColumnVisible = false;
        }

        return this;
    }

    public hideColumnByFieldName(fieldName: string): void {
        (this.instance as any).ej2Instances.hideColumns(fieldName, "field");
    }

    public enablePersistence(gridId: string): SyncfusionEntityGrid {
        this.isPersistenceSupported = true;
        this.constructStateFromStoredData(gridId);
        return this;
    }

    //columnWidth in px
    public configureColumnWidth(columnWidth: string): SyncfusionEntityGrid {
        this.columns.forEach((column) => {
            column['width'] = columnWidth;
        })
        return this;
    }

    public rememberCurrentPageBeforeGridAction(gridAction: EntityGridAction): void {
        switch (gridAction) {
            case EntityGridAction.EntityAdd:
                this._currentPage = 1;
                break;
            case EntityGridAction.EntityEdit:
                this._currentPage = (this.instance as any).pageSettings.currentPage;
                break;
            case EntityGridAction.EntityDelete:
                {
                    let currPage = (this.instance as any).pageSettings.currentPage;

                    if (this.data.length === 1 && currPage > 1) {
                        currPage--; //if this is the only record on the page - when it is deleted we need to return to the prev page
                    }
                    this._currentPage = currPage;
                }
                break;
        }
    }

    public restoreCurrentPage(): void {
        //need to figure out how to handle this;
        //Calling refresh here because a side effect of setPage in vue grids is to fetch data again
        this.refresh()
    }

    public get currentPage(): number {
        return this._currentPage;
    }

    public get dataAdaptor(): DataAdaptor {
        if (this._dataAdaptor == null) {
            throw new Error("Adaptor has not been initialised");
        }
        return this._dataAdaptor;
    }

    public get instance(): SyncfusionGrid {
        if (this._instance == null) {
            throw new Error("Grid instance is not set");
        }
        return this._instance;
    }

    //#endregion

    //#region FEATURE: EVENT HANDLERS

    //This is called whenever filtering/sorting/searching the grid
    //And when opening a string filter (for autocomplete, it wants a datasource, I've hidden this using css for now, but may be implemented later)
    public async onDataStateChanged(state: any): Promise<void> {
        if (state.action != undefined && (state.action.requestType == 'stringfilterrequest' || state.action.requestType == 'save')) return;
        this.state = state;
        let data = await this.dataAdaptor.execute(this.state);
        (this.instance as any).ej2Instances.dataSource = data;
        this.data = data;
    }

    //This is called when toolbar actions are used, such as the in-built add, edit functions
    public async onDataSourceChanged(state: DataSourceChangedEventArgs): Promise<void> {
        const action = state.action != undefined ? state.action : state.requestType;
        switch (action) {
            case Action.Add:
                try {
                    await this.dataAdaptor.add(this.gridEntity);
                    this.endEdit(state);
                }
                catch (e) {
                    this.handleServerError(e);
                    this.cancelEdit(state);
                }
                break;
            case Action.Edit:
                try {
                    await this.dataAdaptor.edit(this.gridEntity);
                    this.endEdit(state);
                }
                catch (e) {
                    this.handleServerError(e);
                    this.cancelEdit(state);
                }
                break;
            default:
                console.log('UNHANDLED ACTION', action);
                return;
        }
    }

    public onActionBegin(actionArgs: any): void {

        const action = actionArgs.action
            ? actionArgs.action
            : actionArgs.requestType
                ? actionArgs.requestType
                : null;

        if (action == null) {
            throw new Error(`Action could not be processed correctly: ${actionArgs}`);
        }

        switch (action) {
            case Action.ClearFilter:
                this.clearColumnFilter(actionArgs);
                break;
            case Action.Filtering:
            case Action.Filter:
                this.applyRangeFilter(actionArgs);
                break;
            case Action.Cancel:
                this.refresh(); //Grid does not restore data correctly after modifying a row and canceling the edit, this is obviously not ideal 
        }
    }

    private clearColumnFilter(actionArgs: any): void {

        if (!this.columns.some(x => x.filter != undefined)) {
            return;
        }

        const field = actionArgs.currentFilteringColumn
            ? actionArgs.currentFilteringColumn.field
            : actionArgs.currentFilterColumn.field;

        if (!field) {
            console.log(`Could not clear filter ${actionArgs}`)
            return;
        }

        const column = this.columns.find(col => col.fieldName == field);

        if (!column || !column.filter) {
            console.log(`Could not clear filter ${actionArgs}`)
            return;
        }

        column!.filter!.clear();
    }

    public clearAllFilters(): void {
        (this.instance as any).ej2Instances.clearFiltering();
    }

    private applyRangeFilter(actionArgs: any): void {

        if (!this.columns.some(x => x.filter != undefined)) {
            return;
        }

        const field = actionArgs.currentFilteringColumn
            ? actionArgs.currentFilteringColumn :
            actionArgs.currentFilterColumn ? actionArgs.currentFilterColumn.field : null;

        if (!field) {
            console.log(`Could not apply filter, arguments do not contain required prop 'field'; ${actionArgs}`)
            return;
        }

        const column = this.columns.find(col => col.fieldName == field);

        if (!column || !column.filter) {
            console.log(`Could not apply filter, grid column not found for field: ${field}, args: ${actionArgs}`)
            return;
        }

        if (column!.filter!.instance.value != null
            && column!.filter!.secondaryInstance != null
            && column!.filter!.secondaryInstance.value) {
            actionArgs.columns.push(column!.filter!.getSecondFilter());
        }
    }

    public handleServerError(error: any): void {
        console.log(`Something went wrong server side, reassign this method to entityCrudMixin onError to show the alert modal, ERROR: ${error}.`)
    }

    public onRowDropped(args: RowDragEventArgs): void {
        console.log("Re-assign to handle row drop", args);
    }

    public onRowDataBound(row: RowDataBoundEventArgs) {
        if (this.isCommandColumnVisible) {
            this.setCommandColumnActionsVisibilityForRow(row);
        }

        this.customRowDataBoundHandler(row);
    }

    //Override this if any custom handling for specific rows is needed 
    public customRowDataBoundHandler(row: RowDataBoundEventArgs): void { return };

    public onCheckboxChanged(event: any) {
        this.hasSelectedItems = event.selectedRowIndexes.length > 0;
    }

    //This handler is called after a button in the command column is clicked
    public onCommandClick(event): void {
        //entity crud mixin methods take id as parameter
        switch (event.commandColumn.type) {
            case Command.Edit:
                this.handleEdit(event.rowData.id);
                break;
            case Command.Delete:
                this.handleDelete(event.rowData.id);
                break;
            case Command.Details:
                this.handleViewDetails(event.rowData.id);
                break;
            default: {
                const command = this.commands.find(x => x.type == event.commandColumn.type);
                if (command == null || command == undefined) {
                    throw new Error("Command does not have a handler defined");
                }
                command.handleCommand(event.rowData);
            }
        }
    }

    //#endregion

    //#region FEATURE: COMMANDS

    public addCustomCommandButton(button: CommandColumnButton): SyncfusionEntityGrid {
        this.commands.push(button);
        return this;
    }

    public setEditBehaviour(editAction: CrudAction, buttonOptions?: CommandColumnButtonOptions, hiddenIf?: any, hiddenIconIf?: any): SyncfusionEntityGrid {

        this.addCustomCommandButton(
            {
                type: "Edit",
                buttonOption: buttonOptions ? buttonOptions : CommandColumnButtonOptions.GetEditOptions(),
                hiddenIf,
                hiddenIconIf
            });

        this.handleEdit = editAction;
        this.disableEdit();
        return this;
    }

    public setDeleteBehaviour(deleteAction: any, buttonOptions?: CommandColumnButtonOptions, hiddenIf?: any, hiddenIconIf?: any): SyncfusionEntityGrid {
        this.addCustomCommandButton(
            {
                type: "Delete",
                buttonOption: buttonOptions ? buttonOptions : CommandColumnButtonOptions.GetDeleteOptions(),
                hiddenIf,
                hiddenIconIf
            });
        this.handleDelete = deleteAction;
        return this;
    }

    public setViewDetailsBehaviour(viewDetails: any, buttonOptions?: CommandColumnButtonOptions, hiddenIf?: any, hiddenIconIf?: any): SyncfusionEntityGrid {

        this.addCustomCommandButton(
            {
                type: "Details",
                buttonOption: buttonOptions ? buttonOptions : CommandColumnButtonOptions.GetDetailsOptions(),
                hiddenIf,
                hiddenIconIf
            });

        this.handleViewDetails = viewDetails;
        return this;
    }

    private setCommandColumnActionsVisibilityForRow(row: any) {

        const conditionalButtons = this.commands.filter(c => c.hiddenIf);
        if (conditionalButtons.length > 0) {
            conditionalButtons.forEach(cb => {
                let filter = cb.hiddenIf;
                if (row.data[filter!.fieldName] == filter!.value) {
                    var title = cb.buttonOption.content == undefined ? cb.type : cb.buttonOption.content;
                    row.row.querySelector([`[title="${title}"]`]).classList.add("e-hide");
                }
            })
        }

        const hideIconConditions = this.commands.filter(c => c.hiddenIconIf);
        if (hideIconConditions.length > 0) {
            hideIconConditions.forEach(cb => {
                const filter = cb.hiddenIconIf;
                if (row.data[filter!.fieldName] == filter!.value) {
                    var title = cb.buttonOption.content == undefined ? cb.type : cb.buttonOption.content;
                    row.row.querySelector([`[title="${title}"] span.e-btn-icon`]).classList.add("e-hide");
                }
            })
        }
    }

    public hideCommand(type: string) {
        let command = this.commands.find(c => c.type == type);
        if (command) {
            this.commands.splice(this.commands.indexOf(command), 1);
        }
    }

    //#endregion

    //#region FEATURE: EXCEL EXPORT
    public async exportToExcel(includeHiddenColumns: boolean = true, fileName: string = "Export") {
        try {
            const data = await this.getDataForExcel();
            (this.instance as any).excelExport({
                dataSource: data,
                exportType: 'AllPages',
                fileName: fileName + ".xlsx",
                includeHiddenColumn: includeHiddenColumns
            });
        }
        catch (e) {
            throw new Error("Excel export failed:" + e);
        }
    }

    public async getDataForExcel() {
        try {
            const appliedFilters = this.dataAdaptor.extractFiltersApplied(this.state);
            const searchQuery = this.dataAdaptor.constructSearchQuery(appliedFilters);
            const data = await this.dataAdaptor.executeApi("&limit=&page=1", "", searchQuery, "", false);
            return data.result;
        }
        catch (e: any) {
            throw new Error("Failed to retrieve data for Excel: " + (e as Error).message);
        }
    }

    public formatExcelExportData(args) {
        const column = this.columns.find(x => x.fieldName == args.column.field);
        if (column != null
            && column.excelFormatter) {
            args.value = column.excelFormatter(args.value);
        }
    }

    //#endregion

    //#region FEATURE: GRID PERSISTENCE
    public constructStateFromStoredData(id: string): void {

        const storedSettingsJson = window.localStorage.getItem(`grid${id}`) //component name (grid) + component id

        if (!storedSettingsJson) {
            return;
        }

        //clear sorting 
        this.sortSettings.columns = [];
        this.state.sorted = [];


        //clear filtering (inc preset filters)
        this.filterSettings.columns = [];
        this.columns.filter(x => x.filter && x.filter!.initialValue != undefined).forEach(x => x.filter!.instance.value = null);

        const gridSettings = JSON.parse(storedSettingsJson);

        this.pageSettings = gridSettings.pageSettings;

        const sorted: any = [];
        for (let sorting of gridSettings.sortSettings.columns) {
            sorted.push({ name: sorting.field, direction: sorting.direction });
        }

        this.sortSettings = gridSettings.sortSettings

        this.state = {
            skip: (this.pageSettings.currentPage - 1) * this.pageSettings.pageSize,
            take: this.pageSettings.pageSize,
            sorted: sorted,
            search: [gridSettings.searchSettings]
        }

        this.searchSettings = gridSettings.searchSettings;

        if (gridSettings.filterSettings.columns != null && gridSettings.filterSettings.columns.length > 0) {
            this.state.where = this.constructWherePredicate(gridSettings.filterSettings.columns);
            this.repopulateFilterValues(gridSettings.filterSettings.columns);
        }
    }

    private repopulateFilterValues(columns: Array<any>) {
        columns.forEach(col => {

            const gridColumn = this.getColumnByFieldName(col.field);

            switch (gridColumn.customFilterType) {
                case 'CheckBox':
                    gridColumn.filter!.instance.value == null ? gridColumn.filter!.instance.value = [col.value] : gridColumn.filter!.instance.value.push(col.value);
                    this.filterSettings.columns.push({ field: gridColumn.fieldName, matchCase: false, operator: col.operator, predicate: 'or', value: col.value });
                    break;
                case 'DropDown':
                    gridColumn.filter!.instance.value = col.value;
                    this.filterSettings.columns.push({ field: gridColumn.fieldName, matchCase: false, operator: col.operator, predicate: 'and', value: col.value });
                    break;
                case 'NumberRange':
                case 'DateRange':
                    if (col.operator == 'greaterthanorequal') {
                        gridColumn.filter!.instance.value = col.value;
                    };
                    if (col.operator == 'lessthanorequal') {
                        gridColumn.filter!.secondaryInstance.value = col.value;
                    }
                    //For some reason, for ranges, only one object should be pushed to columns, otherwise the filter will not appear "active"
                    if (!this.filterSettings.columns.some(x => x.field == gridColumn.fieldName)) {
                        this.filterSettings.columns.push({ field: gridColumn.fieldName, matchCase: false, operator: col.operator, predicate: 'and', value: col.value });
                    };
                    break;
                default:
                    gridColumn!.filterSettings!.value = col.value;
                    this.filterSettings.columns.push({ field: gridColumn.fieldName, matchCase: false, operator: col.operator, predicate: 'and', value: col.value });
            }
        })
    }

    private constructWherePredicate(columns: any[]): any {

        const predicates: any[] = [];
        const countOccurrences = (arr, val) => arr.reduce((a, v) => (v === val ? a + 1 : a), 0);

        for (const column of columns) {
            //Is a range or list
            if (countOccurrences(columns.map(x => x.field), column.field) > 1) {
                this.addComplexPredicate(column, predicates);
            }
            else {
                predicates.push(
                    {
                        isComplex: false,
                        field: column.field,
                        operator: column.operator,
                        value: column.value,
                        ignoreCase: true,
                        ignoreAccent: true
                    }
                )
            }
        }
        return [{
            isComplex: false,
            ignoreAccent: false,
            condition: "and",
            predicates: predicates
        }];
    }

    private addComplexPredicate(column: any, predicates: Array<any>): any {
        //create a complex predicate object
        if (!predicates.some(x => x.isComplex && x.field == column.field)) {
            const condition = column.operator == "equal" ? 'or' : 'and';
            predicates.push(
                {
                    isComplex: true,
                    field: column.field,
                    ignoreCase: true,
                    ignoreAccent: true,
                    condition: condition,
                    predicates: []
                }
            )
        }

        const existingComplexPredicate = predicates.find(x => x.isComplex && x.field == column.field);

        existingComplexPredicate.predicates.push({
            isComplex: false,
            field: column.field,
            operator: column.operator,
            value: column.value,
            ignoreCase: true,
            ignoreAccent: true
        })
    }

    //#endregion

    //#region Internals for making syncfusion functionality work correctly

    //Map row data to grid entity
    public onBeginEdit(args): void {
        this.gridEntity = args.rowData;
    }

    private editGridEntity(args, field): void {
        let value = args.value;
        if (args.hasOwnProperty('checked')) {
            value = args.checked; // Boolean fields use checked not value
        }
        this.gridEntity[field] = value;
    }

    //Required for grid to end toolbar actions correctly
    private async endEdit(state: any): Promise<void> {
        if (state.endEdit != undefined) {
            await state.endEdit();
            (this.instance as any).refreshColumns(true);
        }
    }

    private cancelEdit(state: any): void {
        if (state.cancelEdit != undefined) {
            state.cancelEdit();
            (this.instance as any).refreshColumns(true);
            (this.instance as any).ej2Instances.hideSpinner();
        }
    }

    public mobileTemplateFunction(): any {
        return { template: this.mobileTemplate };
    }

    public detailTemplateFunction(): any {
        return { template: this.detailTemplate };
    }

    //Set inline edit dropdown data
    public setEditData(fieldName: string, data) {
        const column = this.getColumnByFieldName(fieldName);
        if (!column) {
            throw new Error(`Column with ${fieldName} could not be found`);
        }
        column.inlineEdit!.setData(data);
    }

    public getColumnByFieldName(fieldName: string): SyncfusionGridColumn {
        const column = this.columns.find(column => column.fieldName == fieldName);
        if (!column) {
            throw new Error(`Column with fieldName ${fieldName} doesn't exist`);
        }
        return column;
    }

    get sortState(): Sorts[] | undefined {
        return this.state.sorted;
    }
    //#endregion
}