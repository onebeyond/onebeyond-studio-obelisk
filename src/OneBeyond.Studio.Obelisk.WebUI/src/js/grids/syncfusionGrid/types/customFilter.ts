import { DataManager } from "@syncfusion/ej2-data";

export default abstract class CustomFilter {
    field: string; // the field being filtered
    data: any; //data, can be provided in ctor if known or set later (from a lookup)
    instance: any;
    secondaryInstance: any;
    initialValue: any;

    constructor(field: string, data?: any, initialValue?: any) {
        this.field = field;
        this.data = data;
        this.initialValue = initialValue;
    }

    public clear(): any { throw Error("Implement me") }

    public setData(data: any): CustomFilter {
        this.instance.dataSource = new DataManager(data);
        return this;
    }

    public getSecondFilter(): any { throw Error("Not implemented.") }

    get getFilterTemplate(): any { throw Error("Implement me") }
}
