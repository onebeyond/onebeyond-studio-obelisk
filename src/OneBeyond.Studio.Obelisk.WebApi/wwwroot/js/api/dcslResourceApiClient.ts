import DcslApiClient from "@js/api/dcslApiClient";

export default abstract class DcslResourceApiClient extends DcslApiClient {

    constructor(
        resource: string,
        version: string | null,
    ) {
        const sanitizedVersion = !!version ? `${version}/` : "";
        const apiUrl = `${(window as any).BaseUrl}api/${resource}/${sanitizedVersion}`;

        super(apiUrl);
    }
}