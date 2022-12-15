export abstract class CustomInlineEdit {
    field: string; 
    fields: any;
    data: any;
    instance: any; //the instance of the control, should be privately set based on request type of edit
    element: any;
    editHandler: any;
    editTemplate: any;

    constructor(field: string, editHandler: any, valueFieldName?: string, textFieldName?: string, data?: any) {
        this.field = field;
        this.data = data;
        this.editHandler = editHandler;
        this.fields = { value: valueFieldName, text: textFieldName };
    }

    public setData(data: any) {
        this.data = data;
    }

    get getEditTemplate(): any { throw Error("Implement me") };
}