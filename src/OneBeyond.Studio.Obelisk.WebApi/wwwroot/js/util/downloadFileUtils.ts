export abstract class DownloadFileUtils {

    public static downloadFileInBrowser(file: Blob, fileName: string): void {
        const link = document.createElement('a');
        link.setAttribute("type", "hidden");
        link.href = window.URL.createObjectURL(file);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        link.remove();
    }

    public static extractFileName(contentDisposition: string): string {
        const matches = /.*filename=(.*);.*/.exec(contentDisposition);
        let fileName = (matches != null && matches[1])
            ? matches[1]
            : 'file';
        if (fileName.startsWith('"')
            && fileName.endsWith('"')) {
            fileName = fileName.substr(1, fileName.length - 2);
        }

        return fileName;
    }
}