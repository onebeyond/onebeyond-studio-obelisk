export abstract class EntityGrid {
    abstract initDataAdaptor(apiUrl: string, errorCallback: Function); // eslint-disable-line @typescript-eslint/ban-types
    abstract rememberCurrentPageBeforeGridAction(action: any);
    abstract restoreCurrentPage();
    abstract setInstance(gridRef: any);
}

export enum EntityGridAction {
    EntityAdd = 1,
    EntityEdit = 2,
    EntityDelete = 3
}
