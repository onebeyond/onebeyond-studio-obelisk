export { };

declare global {
    interface String {
        capitalize(): string;
        removeEndingIds(): string;
        fillWithSpaces(): string;
    }
}

String.prototype.capitalize = function (this: string) {
    return this.replace(/(?:^|\s)\S/g, (a) => a.toUpperCase());
};

String.prototype.removeEndingIds = function (this: string) {
    return this.replace(/Id$|ID$/g, "");
};

String.prototype.fillWithSpaces = function (this: string) {
    return this.replace(/([A-Z])/g, " $1");
};