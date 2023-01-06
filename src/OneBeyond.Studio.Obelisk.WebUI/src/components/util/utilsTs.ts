//TODO All these utils moved to EntityGrid, but at the moment they are still used in baseCrudMixin.
//Our ultimate goal is to get rid of this class
export default class Utils {

    /**
    * Utils getCols function
    * @param columns array of column names;
    * @param colsToRemove array of column names we want to be removed; default ["mobile", "actions"];
    * @returns returns an array of column names;
    */
    static getCols(columns: string[], colsToRemove?: string[]): string[] {
        const defaultColsToRemove = ["mobile", "actions"];
        if (!colsToRemove || !(colsToRemove instanceof Array) || colsToRemove.length === 0) {
            colsToRemove = defaultColsToRemove;
        } else {
            colsToRemove = colsToRemove.concat(defaultColsToRemove);
        }
        return columns.filter(x => (colsToRemove as string[]).indexOf(x) < 0);
    }

    /**
    * Utils getColumnsClasses function
    * @param columns array of column names;
    * @returns returns an object with column names and related css classes;
    */
    static getColumnsClasses(columns: string[]): any {
        const newObj = {};
        for (let i = 0; i < columns.length; i++) {
            const columnName = columns[i];
            if (columnName === "mobile") {
                newObj[columnName] = "hide-on-desktop";
            } else {
                newObj[columnName] = "hide-on-mobile " + columnName + "-width";
            }
        }

        return newObj;
    }

    /**
    * Utils getColumnHeadings function
    * @param columns array of column names;
    * @param colsToOverride object with column names and heading descriptions we want to use to override the default behaviour. ex.: { columnName : "Name I Want"};
    * @returns returns an object with column names and related heading descriptions;
    */
    static getColumnHeadings(columns: string[], colsToOverride?: any) {
        const newObj = {};
        for (let i = 0; i < columns.length; i++) {
            const columnName = columns[i];
            if (colsToOverride && columnName in colsToOverride) {
                newObj[columnName] = colsToOverride[columnName];
            } else if (columnName !== "mobile" && columnName !== "actions") {
                const colNameWithSpaces = columnName.removeEndingIds().fillWithSpaces().trim().capitalize();
                newObj[columnName] = colNameWithSpaces;
            } else {
                newObj[columnName] = "";
            }
        }
        return newObj;
    }

    /**
    * Utils getActiveListColumnOptions function
    * @returns returns an array of option objects;
    */
    static getActiveListColumnOptions(): any[] {
        return [
            {
                id: "Yes",
                text: "Yes"
            }, {
                id: "No",
                text: "No"
            }
        ];
    }
}
