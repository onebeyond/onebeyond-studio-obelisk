export class CommandColumnButtonOptions {
    public static GetEditOptions(): CommandColumnButtonOptions { return new CommandColumnButtonOptions("e-flat", "far fa-edit") };
    public static GetDeleteOptions(): CommandColumnButtonOptions { return new CommandColumnButtonOptions("e-flat", "far fa-trash-alt") };
    public static GetDetailsOptions(): CommandColumnButtonOptions { return new CommandColumnButtonOptions("e-flat", "far fa-eye") };

    constructor(public cssClass?: string, public iconCss?: string, public content?: string) { }
}

//Hidden If = { field: "fieldName", value: value that means button should be hidden }
export class HiddenIfCondition {
    constructor(public fieldName: string, public value: any) {
    }
}

export class CommandColumnButton {
    constructor(
        public type: string,
        public buttonOption: CommandColumnButtonOptions,
        public handleCommand?: any,
        public hiddenIf?: HiddenIfCondition,
        public hiddenIconIf?: HiddenIfCondition) {
    }
}
