import DcslApiClient from "@js/api/dcslApiClient";
export default abstract class DcslResourceApiClient extends DcslApiClient {

    constructor(
        resource: string,
        version: string | null,
    ) {
        const sanitizedVersion = !!version ? `${version}/` : "";
        const apiUrl = `${DcslResourceApiClient.WebApiRoot}api/${resource}/${sanitizedVersion}`;

        super(apiUrl);
    }
}