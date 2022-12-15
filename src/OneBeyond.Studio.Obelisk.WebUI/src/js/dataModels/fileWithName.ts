export default class FileWithName {

    public readonly blob: Blob;
    public readonly fileName: string;

    constructor(
        blob: Blob,
        fileName: string
    ) {
        this.blob = blob;
        this.fileName = fileName;
    }
}