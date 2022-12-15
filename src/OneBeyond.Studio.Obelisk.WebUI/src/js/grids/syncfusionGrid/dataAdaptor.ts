import * as _ from "lodash";

import DcslApiClient from "@js/api/dcslApiClient";
import {
    DataStateChangeEventArgs,
    DataResult
} from '@syncfusion/ej2-vue-grids'

export class GridFilterApplied {
    public field: string;
    public value: string;
    public operator: string;
    public listIndex?: number | null = null;

    constructor(field: string, value: string, operator: string, listIndex?: number) {
        this.field = field;
        this.value = value;
        this.operator = operator;
        this.listIndex = listIndex;
    }
}

export class DataAdaptor extends DcslApiClient {
    private additionalParameters: any = {};
    private activeHttpGetRequests: AbortController[] = [];
    private sortMaps: any = {};
    private readonly errorCallback: Function;

    constructor(apiUrl: string, errorCallback: Function, sortMaps: any, additionalParameters: any = {}) {
        super(apiUrl);
        this.additionalParameters = additionalParameters;
        this.errorCallback = errorCallback;
        this.sortMaps = sortMaps;
    }

    public execute(state: DataStateChangeEventArgs): Promise<DataResult> {
        return this.getData(state);
    }

    public async add(data: any): Promise<string> {
        const response = await this.post("", data);
        return await response.json();
    }

    public async edit(data: any): Promise<void> {
        await this.put(`${data.id}`, data);
    }

    private getData(state: DataStateChangeEventArgs): Promise<DataResult> {
        let page: number = (state.take == null || state.skip == null || state.skip == 0) ? 1 : (state.skip / state.take) + 1;
        let pageQuery: string = `&limit=${state.take}&page=${page}`;

        let sortQuery: string = '';

        if (state.sorted != null && state.sorted.length > 0) {
            sortQuery = this.constructSortQuery(state.sorted);
        }

        if (sortQuery != '') sortQuery = '&' + sortQuery;

        //search version
        let searchQuery: string = '';
        if (state.search != null && state.search.length > 0) {
            let search: any = state.search[0];
            searchQuery = `&search=${search.key}`;
        }

        //by column version
        let appliedFilters = this.extractFiltersApplied(state)
        let filterQuery = this.constructSearchQuery(appliedFilters);

        return this.executeApi(pageQuery, sortQuery, searchQuery, filterQuery)
    }

    public constructSortQuery(columns: Array<any>): string {
        var sortQuery = "&orderBy=";
        //first col sorted is last in array
        let reversedCols = [...columns].reverse();
        sortQuery += reversedCols.map(x => {
            let dir = x.direction.toLowerCase() == "ascending" ? "asc" : "desc";
            let field = this.sortMaps[x.name] ? this.sortMaps[x.name] : x.name;
            return `${field}:${dir}`;
        }).join(',');
        return sortQuery;
    }

    public constructSearchQuery(appliedFilters: Array<GridFilterApplied>): string {
        let searchQuery: string = '';
        for (let filter of appliedFilters) {
            searchQuery += (searchQuery == '' ? '' : '&');

            switch (filter.operator) {
                case 'equal':
                    searchQuery += `${filter.field}=equals(${filter.value})`;
                    break;
                case 'greaterthanorequal':
                    searchQuery += `${filter.field}=${filter.value}%26`;
                    break;
                case 'lessthanorequal':
                    searchQuery += `${filter.field}=%26${filter.value}`;
                    break;
                case 'list':
                    searchQuery += `${filter.field}[${filter.listIndex}]=${filter.value}`;
                    break;
                case 'range':
                    searchQuery += `${filter.field}=${filter.value}`;
                    break;
                default:
                    searchQuery += `${filter.field}=${filter.operator}(${filter.value})`;
                    break;
            }
        }

        if (searchQuery != '') searchQuery = '&' + searchQuery;
        if (this.additionalParameters) {
            searchQuery += Object.keys(this.additionalParameters).map((p) => {
                if (this.additionalParameters[p] instanceof Array) {
                    return this.additionalParameters[p].map((x) => {
                        return `&${p}=${x}`;
                    }).join("");
                }

                return `&${p}=${this.additionalParameters[p]}`;
            }).join("");
        }
        return searchQuery;
    }

    //TODO: Add logging if in dev
    public extractFiltersApplied(state: DataStateChangeEventArgs): Array<GridFilterApplied> {
        console.log("FILTER STATE", state);
        let filtersApplied: Array<GridFilterApplied> = [];
        if (state.where != null && state.where.length > 0) {
            for (let predicate of (state.where[0] as any).predicates) {
                if (!predicate.isComplex) {
                    if (predicate.value != null) {
                        filtersApplied.push(this.getAppliedFilter(predicate))
                    }
                }
                else {
                    if (predicate.predicates.length == 2
                        && predicate.predicates[0].field == predicate.predicates[1].field
                        && predicate.predicates[0].operator == 'greaterthan'
                        && predicate.predicates[1].operator == 'lessthan') {
                        //We expect that this is the case for date filter "equal" ONLY!
                        //In case if we implement multiselect filters - this is not going to work!
                        filtersApplied.push(new GridFilterApplied(predicate.predicates[0].field, `${predicate.predicates[0].value}%26${predicate.predicates[1].value}`, 'range'))
                    }
                    else if (predicate.predicates.length == 2
                        && predicate.condition == 'and'
                        && predicate.predicates[0].field == predicate.predicates[1].field) {
                        //RANGES
                        //Need to manage val& , &val (will be greaterthan, lessthan operators)
                        filtersApplied.push(new GridFilterApplied(predicate.predicates[0].field, `${predicate.predicates[0].value}%26${predicate.predicates[1].value}`, 'range'))
                    }
                    else if (predicate.predicates.length >= 2
                        && predicate.condition == 'or') {

                        //This case is for LIST checkbox filters
                        //If it is less than two, query[fieldName]=value is used
                        //If it is two or more, syncf uses OR as condition
                        //It may be the case that other filters produce this case, it hasn't been seen yet
                        predicate.predicates.forEach((filter, index) => {
                            filtersApplied.push(new GridFilterApplied(filter.field, `${filter.value}`, 'list', index))
                        });
                    }
                    else if (predicate.predicates[0].operator == 'equal') {
                        filtersApplied.push(this.getAppliedFilter(predicate.predicates[0]));
                    }
                    else {
                        console.log('UNSUPPORTED PREDICATE', predicate)
                    }
                }
            }
        }

        return filtersApplied;
    }

    private getAppliedFilter(predicate: any): GridFilterApplied {
        let operator = predicate.operator == "notequal" ? "not" : predicate.operator;
        return new GridFilterApplied(predicate.field, predicate.value, operator);
    }

    public async executeApi(
        pageQuery: string = "",
        sortQuery: string = "",
        searchQuery = "",
        filterQuery = "",
        useErrorCallback = true): Promise<DataResult> {
        const url = `?${searchQuery}${pageQuery}${sortQuery}${filterQuery}`;

        const controller = new AbortController();
        this.activeHttpGetRequests.push(controller);
        let request = DcslApiClient.buildRequest('GET');
        request.signal = controller.signal;

        try {
            const response = await this.fetch(url, request);
            this.activeHttpGetRequests.pop();
            const responseBody = await response.json();
            return <DataResult>{ result: responseBody.data, count: responseBody.count };
        }
        catch (e: any) {
            this.activeHttpGetRequests.pop();
            if (useErrorCallback && !!this.errorCallback) {
                this.errorCallback(e);
            } else {
                throw new Error(e);
            }
            return <DataResult>{ result: [], count: 0 };
        }
    }

    // Note that nav property parameters need to be of the form { "[nested.property]": this.value }
    public setAdditionalParameters(additionalParameters: any): void {
        this.additionalParameters = additionalParameters;
    }

    // Add or replace a single parameter of the form { param: value }
    public addSingleParameter(newParameter: any): void {
        var key = Object.keys(newParameter)[0];

        this.additionalParameters == null
            ? this.additionalParameters = newParameter
            : this.additionalParameters[`${key}`] = newParameter[key];
    }

    public cancelExcelDataGetRequest(): void {
        //if this method is accessed - the last http get request will be the excel one
        this.activeHttpGetRequests[this.activeHttpGetRequests.length - 1].abort();
    }
}
