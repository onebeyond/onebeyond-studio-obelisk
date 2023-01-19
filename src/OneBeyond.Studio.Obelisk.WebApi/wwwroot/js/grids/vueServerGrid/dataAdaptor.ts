import DcslApiClient from "@js/api/dcslApiClient";

export class DataAdaptor extends DcslApiClient {
    private readonly errorCallback: Function;

    constructor(
        apiBaseUrl: string,
        errorCallback: Function) {
        super(apiBaseUrl);
        this.errorCallback = errorCallback;
    }

    public async executeApi(params: any): Promise<any> {
        const filterOrSearchParam = params["query"];
        let filters: string[] = [];

        if (filterOrSearchParam !== '') {
            // Search box
            if (typeof filterOrSearchParam === 'string') {
                filters.push(`search=${filterOrSearchParam}`);
            } else {
                // Column filter
                for (let filter in filterOrSearchParam) {
                    filters.push(`query[${encodeURIComponent(filter)}]=${encodeURIComponent(filterOrSearchParam[filter])}`);
                }
            }
        }

        // OrderBy/Limit/Page/etc.
        const otherParams = Object.keys(params)
            .filter(k => k != 'query')
            .map(k => `${encodeURIComponent(k)}=${encodeURIComponent(params[k])}`);

        const finalQuery = filters.length > 0 || otherParams.length > 0
            ? `?${otherParams.concat(filters).join('&')}`
            : "";

        try {
            let response = await this.get(finalQuery);
            return await response.json();
        }
        catch (e) {
            if (!!this.errorCallback) {
                this.errorCallback(e);
            }
        }
    }
}
