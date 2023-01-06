import { EntityGrid, EntityGridAction } from "../entityGrid";
import { DataAdaptor } from "./dataAdaptor";

//This interface represents functionality of a vue-tables-2 ServerTable control we're using at the moment
export interface VueGrid {
    Page: number;
    data: Array<any>;
    refresh(): void;
    setPage(page: number): void;
}
//encapsulates all the logic related to the grid vue component we're currently using in the template (vue-server-grid)
export class VueEntityGrid extends EntityGrid {
    protected _instance: VueGrid | null; //This is an instance of the backing grid, e.g. vue server grid

    public readonly columns: string[]; //List of columns to be displayed
    public readonly options: any; //Grid options

    private _currentPage: number; //Currently selected page (used to return to the correct grid page after an entity was added/edited/deleted)

    constructor(columns: string[]) {
        super();
        this._instance = null; //Note, we set instance separately, when the vue component is mounted already;

        this.columns = columns;

        this.options = {
            headings: this.getColumnHeadings(this.columns),
            filterByColumn: false,
            filterable: this.filterCols(this.columns),
            sortable: this.filterCols(this.columns),
            columnsClasses: this.getColumnsClasses(this.columns),
            orderBy: {},
            params: {},
            listColumns: {}
        };

        this._currentPage = 1;

    }

    public get currentPage(): number {
        return this._currentPage;
    }

    public setInstance(instance: any): void {
        this._instance = instance;
    }

    public initDataAdaptor(apiUrl: string, errorCallback: Function): void {
        const dataAdaptor = new DataAdaptor(apiUrl, errorCallback);
        this.options.requestFunction = (params) => dataAdaptor.executeApi(params);
    }

    public get instance(): VueGrid {
        if (this._instance == null) {
            throw new Error("Grid instance is not set");
        }
        return this._instance;
    }

    public refresh(): void {
        this.instance.refresh();
    }

    public rememberCurrentPageBeforeGridAction(gridAction: EntityGridAction): void {
        switch (gridAction) {
        case EntityGridAction.EntityAdd:
            this._currentPage = 1;
            break;
        case EntityGridAction.EntityEdit:
            this._currentPage = this.instance.Page;
            break;
        case EntityGridAction.EntityDelete:
            {
                let currPage = this.instance.Page;

                if (this.instance.data.length === 1 && currPage > 1) {
                    currPage--; //if this is the only record on the page - when it is deleted we need to return to the prev page
                }
                this._currentPage = currPage;
            }
            break;
        }
    }

    public restoreCurrentPage(): void {
        this.instance.setPage(this._currentPage);
    }

    public setCustomHeaders(colsToOverride: any): VueEntityGrid {
        if (colsToOverride != null) {
            for (const colToOverride in colsToOverride) {
                this.options.headings[colToOverride] = colsToOverride[colToOverride];
            }
        }
        return this;
    }

    public setDefaultSortOrder(sortOrder: any): VueEntityGrid {
        this.options.orderBy = sortOrder;
        return this;
    }

    public setListColumns(cols: Array<string>): VueEntityGrid {
        for (const col of cols) {
            this.options.listColumns[col] = [];
        }
        return this;
    }

    public setListColumnValues(colName: string, colValues: Array<any>): VueEntityGrid {
        this.options.listColumns[colName] = colValues;
        return this;
    }

    public excludeColumnsFromSorting(colsToExclude: Array<string>): VueEntityGrid {
        this.options.sortable = this.filterCols(this.options.sortable, colsToExclude);
        return this;
    }

    public excludeColumnsFromFiltering(colsToExclude: Array<string>): VueEntityGrid {
        this.options.filterable = this.filterCols(this.options.filterable, colsToExclude);
        return this;
    }

    /**
    * getColumnHeadings method
    * @param columns array of column names;
    * @param colsToOverride object with column names and heading descriptions we want to use to override the default behaviour. ex.: { columnName : "Name I Want"};
    * @returns returns an object with column names and related heading descriptions;
    */
    private getColumnHeadings(columns: string[]) {

        const newObj = {};

        for (let i = 0; i < columns.length; i++) {
            const columnName = columns[i];
            newObj[columnName] = columnName === "mobile" || columnName === "actions"
                ? ""
                : columnName.removeEndingIds().fillWithSpaces().trim().capitalize();
        }

        return newObj;
    }

    /**
    * getCols method
    * @param columns array of column names;
    * @param colsToRemove array of column names we want to be removed; default ["mobile", "actions"];
    * @returns returns an array of column names;
    */
    private filterCols(columns: string[], colsToRemove?: string[]): string[] {

        const defaultColsToRemove = ["mobile", "actions"];

        if (!colsToRemove || !(colsToRemove instanceof Array) || colsToRemove.length === 0) {
            colsToRemove = defaultColsToRemove;
        } else {
            colsToRemove = colsToRemove.concat(defaultColsToRemove);
        }

        return columns.filter(x => (colsToRemove as string[]).indexOf(x) < 0);

    }

    /**
    * getColumnsClasses method
    * @param columns array of column names;
    * @returns returns an object with column names and related css classes;
    */
    private getColumnsClasses(columns: string[]): any {

        const newObj = {};

        for (let i = 0; i < columns.length; i++) {
            const columnName = columns[i];
            newObj[columnName] = columnName === "mobile"
                ? "hide-on-desktop"
                : "hide-on-mobile " + columnName + "-width"
        }

        return newObj;

    }
}
